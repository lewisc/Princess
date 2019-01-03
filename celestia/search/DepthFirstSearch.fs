namespace Celestia

open System.Diagnostics

open Primitives
open GameState


// TODO: Document
// TODO: Cleanup
module DepthFirstSearch =

    let doUndoDfs driver (depth : int) (game : GameState) =

        let searcher (highMove, score) move =
                do game.DoUpdate move |> ignore
                let (_, value) =  driver (depth - 1) game
                let retVal = -value
                do game.UndoUpdate ()
                (if retVal > score then (Some(move), retVal) else (highMove, score))

        List.fold searcher (None, -Inf) (game.AvailableMoves.Force())

    ///naive depth first search using depth limiter
    let DepthFirstSearchFrame maxer (depth:int) (game:GameState) : (Ply option * Score) =

        let rec searcher d (game : GameState) =
            match d with
            //terminal Node
            | x when x = 0 || (game.IsTerminal()) -> (None, game.Score)
            //the max of the child moves, each child move gets mapped to 
            //it's associated score
            | _ -> maxer searcher d game

        searcher depth game

                            

    ///a timed version of the naive dfs, frameworked to have swappable
    ///underlying behavior
    let IterativeDeepenerFrame maxer time (game:GameState) : Ply option * Score =
        let timer = Stopwatch.StartNew()

        // seacher, has a time check
        let rec searcher depth (game:GameState) : (Ply option * Score) =
            if timer.ElapsedMilliseconds >= time 
            then
                (None, -Inf)
            else
                match depth with
                | x when x = 0 || (game.IsTerminal()) -> (None, game.Score)
                | _ -> maxer searcher depth game

        //driver, keeps going until the alarm rings
        let rec driver currdepth currbestmove =
            let newmove = searcher currdepth game

            //if we found a win, return it, if we found a lose, return our next best move
            if timer.ElapsedMilliseconds >= time then currbestmove 
            elif (snd newmove) >= (Inf/2) then newmove
            elif (snd newmove) <= (-Inf/2) then currbestmove
            else driver (currdepth+2) newmove 

        driver 2 (None, -Inf)

    let IterativeDeepenerDoUndo time game : Ply option * Score = IterativeDeepenerFrame doUndoDfs time game
    let DepthFirstSearchdoUndo time game : Ply option * Score = DepthFirstSearchFrame doUndoDfs time game
