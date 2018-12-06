namespace Celestia
open System
open System.IO

open Primitives
open GameState
open TypedInput

              
module Actions =
    ///used to determine whether the game is continuting  or someone has one
    type Status = | Inplay of Ply option
                  | End of Color

    [<NoEquality;NoComparison>]
    type Player = {EvalFun:Evaluator;
                   EvalInit:GameState->(int*Incrementor);
                   SearchPrime:int64->GameState->(Ply*Score);
                   SearchPonder:((unit->bool)->GameState->(Ply*Score));}

    ///attempts to read a single line from a connection, has a timeout
    ///of timeout milliseconds
    let readLine (connection:IMCSConnection) timeout =
        //this reads the old timeout, writes the new one
        // andthen returns the old one after everything is done
        let oldto = connection.NetStream.ReadTimeout
        do connection.NetStream.ReadTimeout <- timeout
        try
            try 
               let test =  connection.Reader.ReadLine()
               printfn "%s" test
               Some(test)
            with
            | :? IOException -> None
        finally
            connection.NetStream.ReadTimeout <- oldto
        
    
    
    ///Reads the input to a given symbol, if the server waits more than 
    ///timeout ms the function will return all data gathered up to that point
    let readToSymbol (connection:IMCSConnection) (sym:string) timeout 
                                                                : string list =
        let rec reader agg =
            match (readLine connection timeout) with
            |Some(content) -> match content.Trim().Split([|' '|]).[0] with
                              //if we got an symbol stop and return 
                              //the list in the correct(backwards) order
                              | line when String.Equals(line, sym) -> 
                                                         List.rev (line::agg)
                              | line -> reader (content::agg)
            //if we got an error code stop and return 
            //the list in the correct(backwards) order
            | None -> List.rev agg
        reader []
            
    
    ///Reads to . waiting up to 500ms before transmission
    let readToEOM (connection:IMCSConnection) : string list =
        readToSymbol connection "." 500
    
    //converts a string into a code
    let private readCode (input:string) codeval =
        match Int32.TryParse(input.Trim().Split([|' '|]).[0]) with
        | (false,_) -> InputError
        | (true,x) when 100 <= x && x <= 199 -> Notification(x)
        | (true,x) when 200 <= x && x <= 299 -> Command(x)
        | (true,x) when 400 <= x && x <= 599 -> Error(x)
        | _ -> raise <| ProtocolError("Unknown response Range")
    
    ///determines if the line contains the expected return code
    let expect input codeval =
        match (readCode input codeval) with
        | InputError -> false
        | Notification(x) when x = codeval -> true
        | Command(x) when x = codeval -> true
        | Error(x) when x = codeval -> true
        | _ -> false
    
    ///executes a command that returns a single line of result
    let executeLine (connection:IMCSConnection) (input:string) retcode =
        do connection.Writer.WriteLine(input)
        do connection.Writer.Flush()
        match (readLine connection 500) with
        | Some(content) -> let resp = content
                           if expect resp retcode
                           then resp::[]
                           else raise 
                             <| ProtocolError(sprintf "Expected %d" retcode)
        | None ->  raise 
                <| ProtocolError("invalid response, not correct string")
    
    
    ///executes a command that returns a . delimeted packet
    let executeCommand (connection:IMCSConnection) (input:string) 
                                                        retcode : string list =
        do connection.Writer.WriteLine(input)
        do connection.Writer.Flush()
        let resp = readToEOM connection
        if expect resp.Head retcode 
        then resp.Tail
        else raise <| ProtocolError(sprintf "Expected %d" retcode)
            
    ///clears outthe buffer and insures that the connection went through
    let connect connection = 
        let test = readLine connection 500
        match test with 
        | Some x -> expect x 100
        | None -> raise <| ConnectionError("couldn't connect") 
    
    ///gets the list of available games
    let getList connection = executeCommand connection "list" 211
    
    ///gets the help contents
    let getHelp connection = executeCommand connection "help" 210
    
    ///quits the session
    let doQuit connection = executeLine connection "quit" 200
    
    ///authenticates as user=name, password = pass
    let bindName connection name pass = 
        executeLine connection (sprintf "me %s %s" name pass) 201
    
    ///cleans all the stalled games that are active
    let doClean connection = 
        executeLine connection "clean" 204
    
    ///gets the results of the ratings
    let getRatings connection = 
        executeCommand connection "ratings" 212
    
    
    
    ///parses an input of games
    let getGames input =
        let getgameval (inval:string) =
            match Int32.TryParse(inval.Trim().Split([|' '|]).[0]) with
            | (false,_) -> None
            | (true,x) -> Some(x, inval.Trim())
        List.choose getgameval input
    
                  
    let readToGameStop (connection:IMCSConnection) =
        //parses the input
        let chooser x = match x with
                         | ReadInput(y) -> Some(y)
                         | _ -> None
    
        let rec reader agg =
                match (readLine connection -1) with
                |Some(content) 
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
                |Some(content) ->  reader agg
                //if we got an error code stop and return the list 
                //in the correct(backwards) order
                | None -> raise <| ProtocolError("invalid game string")
        reader []
            
    ///plays a move and then gets the output
    let playMove (connection:IMCSConnection) (input:Ply) =
        do connection.Writer.WriteLine(sprintMove input)
        do connection.Writer.Flush()

    let playGame (connection:IMCSConnection) (payload:Player) (initialcolor) =
        //initialize the gamestate
        let gamestate = new GameState(payload.EvalFun payload.EvalInit)
        
        //search for 8 seconds
        //TODO:make this dynamic
        let searchprime = payload.SearchPrime 6000L
        //ponder until we have an update
        let searchponder = payload.SearchPonder (fun () -> connection.NetStream.DataAvailable)

        //mainloop, play the game
        let rec play (color:Color) =

            //end condition 1, should never be hit, but doesn't cause any problems
            if gamestate.IsTerminal()
            then do printfn "%s\nthis shouldn't be hit" (gamestate.ToString())
                 gamestate.Value,(color.Not())

            //determine the turn
            else match color with
                 //my turn, calculate the nexmove, play it, call play with 
                 //the opponents color(which should switch to pondering)
                 | x when x = initialcolor-> 
                                   let (newmove,x) = searchprime gamestate
                                   let (newgame,_) = gamestate.doUpdate(newmove)
                                   do printfn "Move %s, score %d" (sprintMove newmove) x
                                   do printfn "%s" (newgame.ToString())
                                   if newmove <> ((-1,-1),(-1,-1)) then 
                                       do playMove connection newmove
                                   else do playMove connection (gamestate.AvailableMoves.Force().[0])
                                   play (color.Not())
                 //get the result from pondering, we don't do anything with it, but
                 //future diagnostics may
                 //get the response fromt he server, play it internally
                 | _ -> let result = searchponder gamestate
                        let response = readToGameStop connection
                        match response with
                        | Inplay(t) -> match t with
                                       | Some(move) -> let (newgame,_) = gamestate.doUpdate(move)
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
    let doOfferGame connection color mytime theirtime player =
        let mycolor = defaultArg (color) ""
        let play1time = defaultArg (mytime) ""
        let play2time = defaultArg (theirtime) ""
    
        let retval = executeLine connection (sprintf "offer %s %s %s" mycolor play1time play2time) 103
        let tester = connection.Reader.ReadLine()
        let color = match (tester.Split(' ').[1]) with
                    | "W" | "w" -> White
                    | "B" | "b" -> Black
                    | _ -> raise (Hell("invalid color"))
        match color with
        | Black -> playGame connection player color
        | White -> do readToGameStop connection |> ignore
                   playGame connection player color


    ///accepts a game as color
    let doAcceptGame (connection:IMCSConnection) id color player =
        let activecolor = match color with
                          | Some(x) -> x.ToString()
                          | None -> ""
        do connection.Writer.WriteLine(sprintf "accept %d %s" id activecolor)
        do connection.Writer.Flush()
        let mycolor = 
            match (readLine connection 500) with
            | Some(content) -> if expect content 105 
                               then White
                               else if expect content 106 then Black
                                    else 
                                        ProtocolError(sprintf "Expected 105 or 106")
                                     |> raise
        
            | None ->  raise 
                    <| ProtocolError("invalid response, not correct string")
        match mycolor with
        | Black -> playGame connection player Black
        | White -> do readToGameStop connection |> ignore
                   playGame connection player White


