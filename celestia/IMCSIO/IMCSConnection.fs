namespace Celestia

open System
open System.IO
open System.Net.Sockets

open Primitives
open GameState
open TypedInput

module IMCSConnection = 

    ///Something went wrong while playing
    exception PlayingError of string

    ///Something went wrong with the connection
    exception ConnectionError of string

    ///Invalid protocol exception, for when unknown protocol events occur
    exception ProtocolError of string

        
    ///This type encapsulates a number of helper pieces for
    ///connecting to an IMCS server, namely the reader the writer
    ///the client and the network stream. Everything is wired up
    ///properly by the constructor
    ///this class implements IDisposable so as to be able to 
    ///clean up the underlying streams
    type IMCSConnection(server, port) as self = 

        do (self.Writer : StreamWriter).AutoFlush <- true
        //this is a helper variable for IDisposed
        //
        let mutable disposed = false

        //a helper function to dispose the underlying
        //streams, code heavily influenced by Don Symes,
        //Expert F#
        let cleanup() = if not disposed then
                            disposed <- true
                            (self.Client : TcpClient).Close() 
                            (self.NetStream : NetworkStream).Dispose()
                            (self.Reader : StreamReader).Dispose()
                            (self.Writer : StreamWriter).Dispose()
                        else ()

            
        ///returns the underlying TcpClient
        member this.Client = new TcpClient(server, port)

        ///returns the underlying netstream
        member this.NetStream = this.Client.GetStream()

        ///returns the underlying reader, note
        ///the reader does not have a time out
        //so some care must be taken when using this
        member this.Reader = new StreamReader(this.NetStream)

        ///returns the underlying writer, autoflush is
        ///enabled
        member this.Writer = new StreamWriter(this.NetStream)

        //the constructor just creates everything and
        //wraps everything
        ///The Dispose method destroys the underlying
        ///Streams
        interface IDisposable with
            member x.Dispose() = cleanup()
            

        ///attempts to read a single line from a connection, has a timeout
        ///of timeout milliseconds
        member private self.ReadLine (timeout : int) : string option =
            //this reads the old timeout, writes the new one
            // andthen returns the old one after everything is done
            let oldto = self.NetStream.ReadTimeout
            do self.NetStream.ReadTimeout <- timeout
            try
                try 
                   let test = self.Reader.ReadLine()
                   printfn "%s" test
                   Some(test)
                with
                | :? IOException -> None
            finally
                self.NetStream.ReadTimeout <- oldto

        ///Reads the input to a given symbol, if the server waits more than 
        ///timeout ms the function will return all data gathered up to that point
        member private self.ReadAdminPacket () : string list list =
            let rec reader agg =
                match (self.ReadLine(500)) with
                | PacketLine (".", packet) -> (agg @ [packet])
                | PacketLine (_, line) -> reader (agg @ [line])
                // Drop empty lines
                | _ -> reader agg
            reader []

        ///executes a command that returns a single line of result
        member private self.ExecuteLine (input:string) retCode =
            do self.Writer.WriteLine(input)
            do self.Writer.Flush()
            match (self.ReadLine 500) with
            | PacketLine (seen, content) when seen = retCode -> content
            | PacketLine (wrong, _) ->
                ProtocolError(sprintf "Error: Expected %s got %s" retCode wrong)
                |> raise
            | _ ->  raise (ProtocolError("Error: no response"))
    
        ///executes a command that returns a . delimeted packet
        member private self.ExecuteCommand (input : string)
                                           (retCode : string)
                                           : string list list =
            do self.Writer.WriteLine(input)
            do self.Writer.Flush()
            match self.ReadAdminPacket() with
            | (seen :: x) :: y when seen = retCode -> x :: y
            | (wrong :: x) :: y -> 
                ProtocolError(sprintf "Error: Expected %s got %s" retCode wrong)
                |> raise
            | _ ->  raise (ProtocolError("Error: no response"))
            
        ///clears outthe buffer and insures that the connection went through
        member self.Connect with get ()  = self.ExecuteLine "" "100"
    
        ///gets the list of available games
        member self.GetList with get () = self.ExecuteCommand "list" "211"
    
        ///gets the help contents
        member self.GetHelp with get () = self.ExecuteCommand "help" "210"
    
        ///quits the session
        member self.DoQuit () = self.ExecuteLine "quit" "200"
    
        ///authenticates as user=name, password = pass
        member self.BindName name pass =
            self.ExecuteLine (sprintf "me %s %s" name pass) "201"
    
        ///cleans all the stalled games that are active
        member self.DoClean () = self.ExecuteLine "clean" "204"
    
        ///gets the results of the ratings
        member self.GetRatings with get () = self.ExecuteCommand
                                                "ratings" "212"
    
        member private self.ReadGamePacket () =
            let rec reader (move, packet) =
                match (self.ReadLine(500)) with
                | ReadInput x -> reader (Some(x), packet)
                | PacketLine ("?", line) -> (move, packet @ [line])
                | PacketLine ("=", line) ->
                    match self.ReadLine 500 with
                    | PacketLine summary-> (None, packet @ [line; snd summary])
                    | _ -> (None, packet @ [line])
                | PacketLine line -> reader (move, packet @ [snd line])
                // Drop empty lines
                | _ -> reader (move, packet)
            reader (None, [])
            
        ///plays a move and then gets the output
        member private self.PlayMove (input : Ply) =
            do self.Writer.WriteLine(sprintMove input)
            do self.Writer.Flush()

        member self.PlayGame (payload : Player) (initialcolor) =
            //initialize the gamestate
            let gamestate = new GameState(payload.EvalFun)
            
            //search for 8 seconds
            //TODO:make this dynamic
            let searchprime = payload.SearchPrime 6000L
            //ponder until we have an update
            let searchponder = payload.SearchPonder (fun () -> self.NetStream.DataAvailable)

            //mainloop, play the game
            let rec play (color:Color) =
                //
                //determine the turn
                match color with
                //my turn, calculate the nexmove, play it, call play with 
                //the opponents color(which should switch to pondering)
                | x when x = initialcolor -> 
                    let (newmove, _) = searchprime gamestate
                    let score = gamestate.DoUpdate(newmove)
                    //TODO: Gate this
                    //Also print everything including the packet info
                    do printfn "Move %s, score %d" (sprintMove newmove) score
                    do printfn "%s" (gamestate.ToString())

                    do self.PlayMove newmove

                    play (color.Not())
                //get the result from pondering, we don't do anything with it, but
                //future diagnostics may
                //get the response fromt he server, play it internally
                | _ -> let result = searchponder gamestate
                       let (response, returnval) = self.ReadGamePacket ()
                       match response with
                       | Some(move) -> do gamestate.DoUpdate(move) |> ignore
                                       play (color.Not())
                       //or exit out, this is the endcase that should always occur
                       | None -> returnval
            //first move is by white, if that's me then search activates, if that's not me then ponder
            //activates
            match initialcolor with
            | Black -> play Black
            | White -> do self.ReadGamePacket |> ignore
                       play White

        ///offers a game as color, for my time, with their time
        //TODO: Default arguments are a thing
        member self.DoOfferGame color mytime theirtime player =
            let mycolor = defaultArg (color) ""
            let play1time = defaultArg (mytime) ""
            let play2time = defaultArg (theirtime) ""
        
            let retval = self.ExecuteLine (sprintf "offer %s %s %s" mycolor play1time play2time) "103"
            let color  = match retval with
                         | _ :: "W" :: _ | _ :: "w" :: _ -> White
                         | _ :: "B" :: _ | _ :: "b" :: _ -> Black
                         | _ -> raise (Hell("invalid color"))
            self.PlayGame player color


        ///accepts a game as color
        member self.DoAcceptGame id color player =
            let activecolor = match color with
                              | Some(x) -> x.ToString()
                              | None -> ""
            do self.Writer.WriteLine(sprintf "accept %d %s" id activecolor)
            do self.Writer.Flush()
            let mycolor = 
                match (self.ReadLine 500) with
                | PacketLine ("105", _)  -> White
                | PacketLine ("106", _) -> Black
                | _ -> raise (ProtocolError(sprintf "Expected 105 or 106"))
            self.PlayGame player mycolor
