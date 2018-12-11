namespace Celestia

open System
open System.IO
open System.Net.Sockets

open Primitives
open GameState
open TypedInput

module IMCSConnection = 

    ///used to determine whether the game is continuting  or someone has one
    type Status =
        | Inplay of Ply option
        | End of Color


    [<NoEquality;NoComparison>]
    type Player = { EvalFun : Evaluator;
                    SearchPrime : int64->GameState->(Ply*Score);
                    SearchPonder : ((unit->bool)->GameState->(Ply*Score));}

    ///Something went wrong while playing
    exception PlayingError of string

    ///Something went wrong with the connection
    exception ConnectionError of string

    ///Invalid protocol exception, for when unknown protocol events occur
    exception ProtocolError of string

    let private (|Matched|_|) (expectedValue : string)
                              (inputString : string option) : string [] option =
        match inputString with
        | None -> None
        | Some(s) -> let trimmedString = s.Trim().Split[|' '|]
                     if trimmedString.Length > 0
                     then
                         if String.Equals(inputString, expectedValue)
                         then Some(trimmedString)
                         else None
                     else
                         None

    let private (|Unmatched|_|) (expectedValue : string)
                                (inputString : string option) : string [] option =
        match inputString with
        | None -> None
        | Some(s) -> let trimmedString = s.Trim().Split[|' '|]
                     if trimmedString.Length > 0
                     then
                         if String.Equals(inputString, expectedValue)
                         then None
                         else Some(trimmedString)
                     else
                         None
        
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
        member self.ReadLine (timeout : int) : string option =
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
        member self.ReadToSymbol (sym : string) timeout : string list =
            let rec reader agg =
                match (self.ReadLine(timeout)) with
                | Matched sym line -> List.rev ((Array.toList line) @ agg)
                | Unmatched sym line -> reader ((Array.toList line) @ agg)
                | _ -> List.rev agg
            reader []

        ///Reads to . waiting up to 500ms before transmission
        member self.ReadToEOM () : string list =
            self.ReadToSymbol "." 500
    
    
        ///executes a command that returns a single line of result
        member self.ExecuteLine (input:string) retcode =
            do self.Writer.WriteLine(input)
            do self.Writer.Flush()
            match (self.ReadLine 500) with
            | Matched retcode content -> content :: []
            | Unmatched retcode _ -> raise (ProtocolError(sprintf "Expected %s" retcode))
            | _ ->  raise (ProtocolError("invalid response, not correct string"))
    
    
        ///executes a command that returns a . delimeted packet
        member self.ExecuteCommand (input:string) retcode : string list =
            do self.Writer.WriteLine(input)
            do self.Writer.Flush()
            let message = self.ReadToEOM ()
            match (None) with
            | Matched retcode content -> Array.toList content
            | _ ->  raise (ProtocolError("invalid response, not correct string"))
            
        ///clears outthe buffer and insures that the connection went through
        member self.Connect = 
            let test = self.ReadLine 500
            match self.ReadLine 500 with 
            | Matched "100" content -> content
            | _ ->  raise (ProtocolError("invalid response, not correct string"))
    
        ///gets the list of available games
        member self.GetList with get () = self.ExecuteCommand "list" "211"
    
        ///gets the help contents
        member self.GetHelp with get () = self.ExecuteCommand "help" "210"
    
        ///quits the session
        member self.DoQuit () = self.ExecuteLine "quit" "200"
    
        ///authenticates as user=name, password = pass
        member self.BindName name pass = self.ExecuteLine (sprintf "me %s %s" name pass) "201"
    
        ///cleans all the stalled games that are active
        member self.DoClean () = self.ExecuteLine "clean" "204"
    
        ///gets the results of the ratings
        member self.GetRatings with get () = self.ExecuteCommand "ratings" "212"
    
        ///parses an input of games
        ///TODO: Clean this up
        member self.GetGames input =
            let getgameval (inval : string) =
                match Int32.TryParse(inval.Trim().Split([|' '|]).[0]) with
                | (false, _) -> None
                | (true, x) -> Some(x, inval.Trim())
            List.choose getgameval input
    
                  
        member self.ReadToGameStop () =
            //parses the input
            let chooser x = match x with
                             | ReadInput(y) -> Some(y)
                             | _ -> None
        
            let rec reader agg =
                    match (self.ReadLine -1) with
                    | Some(content) 
                        when not (String.Equals(content.Trim(),String.Empty)) -> 
                                match content.Trim().Split([|' '|]).[0] with
                                //if we got an symbol stop and return 
                                //the list in the correct(backwards) order
                                | line when String.Equals(line, "?") ->   
                                            let moves =  List.choose chooser agg
                                            match List.length moves with 
                                            | x when x > 1 -> 
                                               (ProtocolError("more than 1 move"))
                                               |> raise
                                            | 1 -> Inplay(Some(moves.Head))
                                            | _ -> Inplay(None)
                                       //endgame case
                                | line when String.Equals(line, "=") -> 
                                            //note using content not line
                                     End(match content.Split([|' '|]).[1] with  
                                         | "W" -> White 
                                         | "B" -> Black 
                                         | x -> raise <| ProtocolError
                                                   (sprintf "invalid winner: %s" x))
                                | line -> reader (content::agg)
                    | Some(content) ->  reader agg
                    //if we got an error code stop and return the list 
                    //in the correct(backwards) order
                    | None -> raise <| ProtocolError("invalid game string")
            reader []
            
        ///plays a move and then gets the output
        member self.PlayMove (input : Ply) =
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
                | x when x = initialcolor-> 
                    let (newmove, x) = searchprime gamestate
                    let score = gamestate.DoUpdate(newmove)
                    do printfn "Move %s, score %d" (sprintMove newmove) score
                    do printfn "%s" (gamestate.ToString())
                    if newmove <> ((-1,-1),(-1,-1)) then 
                        do self.PlayMove newmove
                    else do self.PlayMove (gamestate.AvailableMoves.Force().[0])
                    play (color.Not())
                //get the result from pondering, we don't do anything with it, but
                //future diagnostics may
                //get the response fromt he server, play it internally
                | _ -> let result = searchponder gamestate
                       let response = self.ReadToGameStop ()
                       match response with
                       | Inplay(t) -> match t with
                                      | Some(move) -> do gamestate.DoUpdate(move) |> ignore
                                                      play (color.Not())
                                   //this indicates that a parse error occurred, but technically
                                   //we might be able to muscle past
                                      | None ->  printfn "Didn't read a move when should have"
                                                 play (color)
                       //or exit out, this is the endcase that should always occur
                       | End(t) -> 1000000, t
            //first move is by white, if that's me then search activates, if that's not me then ponder
            //activates
            play White

                        
            
        ///offers a game as color, for my time, with their time
        member self.DoOfferGame color mytime theirtime player =
            let mycolor = defaultArg (color) ""
            let play1time = defaultArg (mytime) ""
            let play2time = defaultArg (theirtime) ""
        
            let retval = self.ExecuteLine (sprintf "offer %s %s %s" mycolor play1time play2time) "103"
            let tester = self.Reader.ReadLine()
            let color  = match (tester.Split(' ').[1]) with
                         | "W" | "w" -> White
                         | "B" | "b" -> Black
                         | _ -> raise (Hell("invalid color"))
            match color with
            | Black -> self.PlayGame player color
            | White -> do self.ReadToGameStop |> ignore
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
                | Matched "105" _ -> White
                | Matched "106" _ -> Black
                | _ -> raise (ProtocolError(sprintf "Expected 105 or 106"))
            match mycolor with
            | Black -> self.PlayGame player Black
            | White -> do self.ReadToGameStop |> ignore
                       self.PlayGame player White


