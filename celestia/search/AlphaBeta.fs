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

    let AlphaBetaSearch (node:GameState) depth =
        node.AvailableMoves.Force ()
        |> List.map (fun move -> do node.DoUpdate move |> ignore
                                 let score = AlphaBeta node depth -Inf Inf
                                 do node.UndoUpdate ()
                                 (score, move))
        |> List.maxBy snd

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
