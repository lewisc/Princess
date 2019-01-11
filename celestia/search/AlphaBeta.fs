namespace Celestia

open DepthFirstSearch
open TranspositionTable
open Quiescence
open Primitives 
open GameState

open System.Diagnostics

//TODO: Cleanup
//TODO: DOcument
module AlphaBeta =

    [<Literal>]
    let sortdepth = 5 

    //TODO Reintroduce transposition tables
    let sortMoves (moveslist : Ply list)
                  (game : GameState) (depth : int) : Ply list =
        if depth < sortdepth then moveslist else
        moveslist
        |> List.map (fun move -> 
            let score = game.DoUpdate move
            do game.UndoUpdate ()
            (score, move))
        |> List.sortBy fst
        |> List.map snd


    let AlphaBeta (node : GameState) (depth : int) : int =
        //Requires explicit searcher recursion since there's no failfast
        //fold

        let rec searcher currentalpha currentbeta currentdepth moves =
            match moves with
            | [] -> currentalpha
            | move :: more ->
                //travel along the breadth
                let score = node.DoUpdate move
                //negamax depth
                //Gdo deeper
                let newScore = -(match (currentdepth - 1) with 
                                 | x when x <= 0 || node.IsTerminal() -> node.Score
                                 | _ -> searcher -currentbeta -currentalpha (currentdepth - 1) (node.AvailableMoves.Force()))
                do node.UndoUpdate ()
                //alpha prune
                let newalpha = max currentalpha newScore
                //beta prune
                if newalpha >= currentbeta then newalpha
                //continue traveling breadthwise
                else searcher newalpha currentbeta currentdepth more

        searcher (-Inf) (Inf) depth (node.AvailableMoves.Force())

    let AlphaBetaTT (node : GameState) (depth : int) : int =

        let rec searcher origalpha currentalpha currentbeta currentdepth moves =
             match moves with 
             | [] -> 
                 if currentalpha > origalpha && currentalpha < currentbeta
                 then 
                    //in window, perfect hit
                    do setTranspose node.ZobristHash currentalpha Transpose.Exact currentdepth node.Turn
                    //since we don't know if we got this or something less than this, this is
                    //an upperbound
                 elif
                    currentalpha <= origalpha then
                    do setTranspose node.ZobristHash currentalpha Transpose.Upper currentdepth node.Turn
                 else ()
                 currentalpha
             //travel along the breadth
             | move :: more ->
                 let score = node.DoUpdate(move)
                 //travel the depth
                 let newtest = -(match (currentdepth - 1) with 
                                 | x when x <= 0 || node.IsTerminal() -> quiesce node -currentbeta -currentalpha
                                 | _ -> match (getTranspose node.ZobristHash (currentdepth - 1) node.Turn) with
                                        | Some(Transpose.Exact, x) -> x
                                        | Some(Transpose.Upper, x) when x <= -currentbeta -> x
                                        | Some(Transpose.Lower, x) when x >= -currentalpha -> x
                                        | _ -> searcher -currentbeta -currentbeta -currentalpha (currentdepth - 1) (sortMoves (node.AvailableMoves.Force()) node (currentdepth - 1)))
                 //do undo
                 do node.UndoUpdate()
                 //alpha prune
                 let newalpha = max currentalpha newtest
                 //beta prune
                 if newalpha >= currentbeta then
                 //since we didn't continue this is a lower bound
                             do setTranspose node.ZobristHash newalpha Transpose.Lower currentdepth node.Turn
                             newalpha
                 //continue traveling breadthwise
                 else searcher origalpha newalpha currentbeta currentdepth more
                                
        searcher (-Inf) (-Inf) Inf depth (sortMoves (node.AvailableMoves.Force()) node (depth))

    let AlphaBetaDFSID (node : GameState) time alpha beta =
        let timer = Stopwatch.StartNew()
        let rec searcher currentalpha currentbeta movelist currentdepth : int =
            match movelist with
            //travel along the breadth
            |  move :: more ->  
                 let score = node.DoUpdate move
                 //negamax depth
                 let newtest = -(match (currentdepth - 1) with 
                                 | x when x <= 0 || node.IsTerminal() -> score
                                 | _ -> searcher -currentbeta -currentalpha (node.AvailableMoves.Force()) (currentdepth-1))
                 do node.UndoUpdate() 
                 //alpha prune
                 let newalpha = max currentalpha newtest
                 //beta prune
                 if newalpha >= currentbeta then newalpha
                 //TODO: move this out of thematch
                 elif timer.ElapsedMilliseconds > time then -Inf-1000
                 //continue traveling breadthwise
                 else searcher newalpha currentbeta movelist currentdepth

            | [] -> currentalpha

        //iterative deepening
        let rec deepener appdepth currbestmove =

            let newmove = List.map (fun (y) -> let score = node.DoUpdate(y)
                                               //TODO: Failout if terminal
                                               let retval = (y, -(searcher alpha beta (node.AvailableMoves.Force()) (appdepth - 1)))
                                               do node.UndoUpdate()
                                               retval) (node.AvailableMoves.Force())
                          |> List.maxBy snd
            if timer.ElapsedMilliseconds >= time then 
                    do printfn "searchdepth %d" (appdepth-2)
                    currbestmove
            //if we hit an endgame
            elif (snd newmove) >= Inf / 2 then newmove
            elif (snd newmove) < -Inf / 2 then currbestmove
            //otherwise keep going by 2
            else deepener (appdepth+2) newmove
        deepener 0 (BotMove, -Inf - 1000)

    let AlphaBetaDFSTT (node : GameState) (time : int64) : Ply * int =

        let timer = Stopwatch.StartNew()
        let moves = sortMoves (node.AvailableMoves.Force()) node 10

        let rec searcher origalpha currentalpha currentbeta movelist currentdepth =
            match movelist with 
            //travel along the breadth
            | move :: more ->  
                let score = node.DoUpdate(move)
                //travel the depth
                let newtest =  -(match currentdepth-1 with 
                                 | x when x <= 0 || node.IsTerminal() -> node.Score
                                 | _ ->  match (getTranspose node.ZobristHash (currentdepth-1) node.Turn) with
                                         | Some(Transpose.Exact,x) -> x
                                         | Some(Transpose.Upper,x) when x <= -currentbeta-> x
                                         | Some(Transpose.Lower,x) when x >= -currentalpha -> x
                                         | _ ->  searcher -currentbeta -currentbeta -currentalpha (node.AvailableMoves.Force()) (currentdepth - 1))
                //do undo
                do node.UndoUpdate()
                //alpha prune
                let newalpha = max currentalpha newtest
                //beta prune
                if newalpha >= currentbeta then
                //since we didn't continue this is a lower bound
                            do setTranspose node.ZobristHash newalpha Transpose.Lower currentdepth node.Turn
                            newalpha
                //continue traveling breadthwise
                elif timer.ElapsedMilliseconds > time then -Inf - 1000
                else searcher origalpha newalpha currentbeta movelist currentdepth
                   
            | _ -> if currentalpha > origalpha && currentalpha < currentbeta then 
                       //in window, perfect hit
                       do setTranspose node.ZobristHash currentalpha Transpose.Exact currentdepth node.Turn
                       //since we don't know if we got this or something less than this, this is
                       //an upperbound
                   elif currentalpha <= origalpha then do setTranspose node.ZobristHash currentalpha Transpose.Upper currentdepth node.Turn
                   else ()
                   currentalpha
        //iterative deepening
        let rec deepener appdepth currbestmove =

            let mutable curralpha = -Inf
            let mutable movelist = []
            let mutable bestmove = (BotMove,-Inf-1000)

            for i in moves do
                let score = node.DoUpdate(i) 
                let newalpha = -(searcher -Inf -Inf -curralpha (node.AvailableMoves.Force()) (appdepth-1))
                do movelist <- (i,newalpha)::movelist
                do curralpha <- newalpha
                if newalpha > (snd bestmove) then
                    do bestmove <- (i,newalpha)
                else ()
                do node.UndoUpdate()


            if timer.ElapsedMilliseconds >= time then 
                    do printfn "searchdepth %d" (appdepth-2)
                    currbestmove
            //if we hit an endgame
            elif (snd bestmove) >= Inf/2 then bestmove
            elif (snd bestmove) < -Inf/2 then currbestmove
            //otherwise keep going by 2
            else deepener (appdepth+2) bestmove
        deepener 0 (BotMove, -Inf - 1000)

    let Ponder pred (node : GameState) =
        let rec searcher origalpha currentalpha currentbeta (movelist:Ply list) currentdepth =
            if pred() then -Inf-1000
            else
            match movelist with 
            //travel along the breadth
            | head :: tail->  let score = node.DoUpdate(head)
                              //travel the depth
                              let newtest =  -(match currentdepth-1 with 
                                               | x when x <= 0 || node.IsTerminal() -> node.Score
                                               | _ ->  match (getTranspose node.ZobristHash (currentdepth-1) node.Turn) with
                                                       | Some(Transpose.Exact,x) -> x
                                                       | Some(Transpose.Upper,x) when x <= -currentbeta-> x
                                                       | Some(Transpose.Lower,x) when x >= -currentalpha -> x
                                                       | _ ->  searcher -currentbeta -currentbeta -currentalpha (node.AvailableMoves.Force()) (currentdepth-1))
                              //do undo
                              do node.UndoUpdate()
                              //alpha prune
                              let newalpha = max currentalpha newtest
                              //beta prune
                              if newalpha >= currentbeta then
                              //since we didn't continue this is a lower bound
                                          do setTranspose node.ZobristHash newalpha Transpose.Lower currentdepth node.Turn
                                          newalpha
                              //continue traveling breadthwise
                              else searcher origalpha newalpha currentbeta tail currentdepth
                              
            | _  -> if currentalpha > origalpha && currentalpha < currentbeta then 
                                 //in window, perfect hit
                                 do setTranspose node.ZobristHash currentalpha Transpose.Exact currentdepth node.Turn
                                 //since we don't know if we got this or something less than this, this is
                                 //an upperbound
                    elif currentalpha <= origalpha then
                                 do setTranspose node.ZobristHash currentalpha Transpose.Upper currentdepth node.Turn
                    else ()
                    currentalpha
                        

        //iterative deepening
        let rec deepener appdepth currbestmove =

            let newmove = searcher -Inf -Inf Inf (node.AvailableMoves.Force()) appdepth
            if pred() then 
                    do printfn "fake searchdepth %d" (appdepth-2)
                    (BotMove,currbestmove)
            //if we hit an endgame
            //otherwise keep going by 2
            else deepener (appdepth+2) newmove 
        deepener 1 0


    let NewDFSABID time input = AlphaBetaDFSID input time
    let NewDFSABIDTT time input = AlphaBetaDFSTT input time
    let Ponderer pred input = Ponder pred input
