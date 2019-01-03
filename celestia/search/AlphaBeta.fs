namespace Celestia

open DepthFirstSearch
open TranspositionTable
open Quiescence
open Primitives 
open GameState

open System.Diagnostics

//TODO: Cleanup
//TODO: DOcument
module AlphaBeta2 =

    [<Literal>]
    let sortdepth = 5 

    //TODO Reintroduce transposition tables
    let sortMoves moveslist (game: GameState) depth =
        if depth < sortdepth then moveslist else
        moveslist
        |> List.map (fun move -> 
            let score = game.DoUpdate move
            do game.UndoUpdate ()
            (score, move))
        |> List.sortBy fst
        |> List.map snd


    
    let AlphaBeta (node:GameState) depth alpha beta =
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
                                 | x when x <= 0 || node.IsTerminal() -> node.Value
                                 | _ -> searcher -currentbeta -currentalpha (currentdepth - 1) (node.AvailableMoves.Force()))
                do node.UndoUpdate ()
                //alpha prune
                let newalpha = max currentalpha newScore
                //beta prune
                if newalpha >= currentbeta then newalpha
                //continue traveling breadthwise
                else searcher newalpha currentbeta currentdepth more

        searcher alpha beta depth (node.AvailableMoves.Force())

    let AlphaBetaTT (node:GameState) depth alpha beta =
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
                 let score = node.DoUpdate move
                 //travel the depth
                 let newtest = -(match (currentdepth - 1) with 
                                 | x when x <= 0 || node.IsTerminal() -> quiesce node -currentbeta -currentalpha
                                 | _ -> match (getTranspose node.ZobristHash (currentdepth - 1) node.Turn) with
                                        | Some(Transpose.Exact, x) -> x
                                        | Some(Transpose.Upper, x) when x <= -currentbeta -> x
                                        | Some(Transpose.Lower, x) when x >= -currentalpha -> x
                                        | _ -> searcher -currentbeta -currentbeta -currentalpha (currentdepth - 1) (sortMoves (node.AvailableMoves.Force()) node (currentdepth - 1)))
                 //do undo
                 do node.UndoUpdate ()
                 //alpha prune
                 let newalpha = max currentalpha newtest
                 //beta prune
                 if newalpha >= currentbeta then
                 //since we didn't continue this is a lower bound
                             do setTranspose node.ZobristHash newalpha Transpose.Lower currentdepth node.Turn
                             newalpha
                 //continue traveling breadthwise
                 else searcher origalpha newalpha currentbeta currentdepth more
                                
        searcher alpha alpha beta depth (sortMoves (node.AvailableMoves.Force()) node (depth))

    let AlphaBetaDFSID node time alpha beta =
        let timer = Stopwatch.StartNew()
        let rec searcher currentalpha currentbeta movelist currentdepth : int =
            match movelist with
            //travel along the breadth
            |  move :: more ->  
                 let score = node.DoUpdate head
                 //negamax depth
                 let newtest = -(match (currentdepth - 1) with 
                                 | x when x <= 0 || newnode.IsTerminal() -> score
                                 | _ -> searcher -currentbeta -currentalpha (newnode.AvailableMoves.Force()) (currentdepth-1))
                 do node.UndoUpdate () 
                 //alpha prune
                 let newalpha = max currentalpha newtest
                 //beta prune
                 if newalpha >= currentbeta then newalpha
                 //TODO: move this out of thematch
                 elif timer.ElapsedMilliseconds > time then -inf-1000
                 //continue traveling breadthwise
                 else searcher newalpha currentbeta movelist currentdepth

            | [] -> currentalpha

        //iterative deepening
        let rec deepener appdepth currbestmove =

            let newmove = Array.map (fun (y) -> let score = node.DoUpdate y
                                                //TODO: Failout if terminal
                                                let retval = (y, -(searcher alpha beta node (newnode.AvailableMoves.Force()) (appdepth - 1)))
                                                do node.undoUpdate 
                                                retval) (node.AvailableMoves.Force())
                          |> Array.maxBy snd
            if timer.ElapsedMilliseconds >= time then 
                    do printfn "searchdepth %d" (appdepth-2)
                    currbestmove
            //if we hit an endgame
            elif (snd newmove) >= inf / 2 then newmove
            elif (snd newmove) < -inf / 2 then currbestmove
            //otherwise keep going by 2
            else deepener (appdepth+2) newmove
        deepener 0 (botMove, -inf - 1000)

    let AlphaBetaDFSTT node time alpha beta =
        let timer = Stopwatch.StartNew()
        let moves = sortMoves (node.AvailableMoves.Force()) node 10
        let rec searcher origalpha currentalpha currentbeta movelist currentdepth =
            match movelist with 
            //travel along the breadth
            | move :: more ->  
                let score = node.DoUpdate move
                //travel the depth
                let newtest =  -(match currentdepth-1 with 
                                 | x when x <= 0 || newnode.IsTerminal() -> newnode.Value
                                 | _ ->  match (getTranspose newnode.ZobristHash (currentdepth-1) newnode.Turn) with
                                         | Some(Transpose.Exact,x) -> x
                                         | Some(Transpose.Upper,x) when x <= -currentbeta-> x
                                         | Some(Transpose.Lower,x) when x >= -currentalpha -> x
                                         | _ ->  searcher -currentbeta -currentbeta -currentalpha newnode 0 (newnode.AvailableMoves.Force()) (currentdepth-1))                                
                //do undo
                do undoUpdate newnode newundo 
                //alpha prune
                let newalpha = max currentalpha newtest
                //beta prune
                if newalpha >= currentbeta then
                //since we didn't continue this is a lower bound
                            do setTranspose currentnode.ZobristHash newalpha Transpose.Lower currentdepth currentnode.Turn
                            newalpha
                //continue traveling breadthwise
                elif timer.ElapsedMilliseconds > time then -inf-1000
                else searcher origalpha newalpha currentbeta currentnode (x+1) movelist currentdepth
                   
            | _ -> if currentalpha > origalpha && currentalpha < currentbeta then 
                                                                                    //in window, perfect hit
                                                                                    do setTranspose currentnode.ZobristHash currentalpha Transpose.Exact currentdepth currentnode.Turn
                                                                                    //since we don't know if we got this or something less than this, this is
                                                                                    //an upperbound
                   elif currentalpha <= origalpha then do setTranspose currentnode.ZobristHash currentalpha Transpose.Upper currentdepth currentnode.Turn
                   else ()
                   currentalpha
        //iterative deepening
        let rec deepener appdepth currbestmove =

            let mutable curralpha = -inf
            let mutable movelist = []
            let mutable bestmove = (botMove,-inf-1000)

            for i in moves do
                let (newnode,undo) = doUpdate node i 
                let newalpha = -(searcher -inf -inf -curralpha newnode 0 (newnode.AvailableMoves.Force()) (appdepth-1))
                do movelist <- (i,newalpha)::movelist
                do curralpha <- newalpha
                if newalpha > (snd bestmove) then
                    do bestmove <- (i,newalpha)
                else ()
                do undoUpdate newnode undo


            if timer.ElapsedMilliseconds >= time then 
                    do printfn "searchdepth %d" (appdepth-2)
                    currbestmove
            //if we hit an endgame
            elif (snd bestmove) >= inf/2 then bestmove
            elif (snd bestmove) < -inf/2 then currbestmove
            //otherwise keep going by 2
            else deepener (appdepth+2) bestmove
        deepener 0 (botMove,-inf-1000)

    let Ponder pred node =
        let rec searcher origalpha currentalpha currentbeta currentnode index (movelist:Ply []) currentdepth =
            if pred() then -inf-1000
            else
            match index with 
            //travel along the breadth
            | x when x < movelist.Length ->  
                             let head = movelist.[x]
                             let (newnode,newundo) = doUpdate currentnode head
                             //travel the depth
                             let newtest =  -(match currentdepth-1 with 
                                              | x when x <= 0 || newnode.IsTerminal() -> newnode.Value
                                              | _ ->  match (getTranspose newnode.ZobristHash (currentdepth-1) newnode.Turn) with
                                                      | Some(Transpose.Exact,x) -> x
                                                      | Some(Transpose.Upper,x) when x <= -currentbeta-> x
                                                      | Some(Transpose.Lower,x) when x >= -currentalpha -> x
                                                      | _ ->  searcher -currentbeta -currentbeta -currentalpha newnode 0 (newnode.AvailableMoves.Force()) (currentdepth-1))
                             //do undo
                             do undoUpdate newnode newundo 
                             //alpha prune
                             let newalpha = max currentalpha newtest
                             //beta prune
                             if newalpha >= currentbeta then
                             //since we didn't continue this is a lower bound
                                         do setTranspose currentnode.ZobristHash newalpha Transpose.Lower currentdepth currentnode.Turn
                                         newalpha
                             //continue traveling breadthwise
                             else searcher origalpha newalpha currentbeta currentnode (index+1) movelist currentdepth
                                
            | _  -> if currentalpha > origalpha && currentalpha < currentbeta then 
                                 //in window, perfect hit
                                 do setTranspose currentnode.ZobristHash currentalpha Transpose.Exact currentdepth currentnode.Turn
                                 //since we don't know if we got this or something less than this, this is
                                 //an upperbound
                    elif currentalpha <= origalpha then
                                 do setTranspose currentnode.ZobristHash currentalpha Transpose.Upper currentdepth currentnode.Turn
                    else ()
                    currentalpha
                        

        //iterative deepening
        let rec deepener appdepth currbestmove =

            let newmove = searcher -inf -inf inf node 0 (node.AvailableMoves.Force()) appdepth
            if pred() then 
                    do printfn "fake searchdepth %d" (appdepth-2)
                    (botMove,currbestmove)
            //if we hit an endgame
            //otherwise keep going by 2
            else deepener (appdepth+2) newmove 
        deepener 1 0


    let NewDFSABID time input = AlphaBetaDFSID input time (-inf) inf
    let NewDFSABIDTT time input = AlphaBetaDFSTT input time (-inf) inf
    let Ponderer pred input = Ponder pred input
