namespace Celestia

open GameState
open Primitives

//TODO: DOcument
module Quiescence =
    

    let rec quiesce (node: GameState) alpha beta =
        let value = node.Value

        if value >= beta then
            beta
        else
        let localalpha = if value > alpha then value else alpha
        let captures = (List.map (fun move -> let score = node.DoUpdate move
                                              do node.UndoUpdate ()
                                              (abs(score-value), move))
                                  (node.AvailableMoves.Force()))
                        |> List.filter (fun (x, move) -> x > 10)
                        |> List.map snd

        let traverser alpha move =
            do node.DoUpdate move |> ignore
            let neweval = -(quiesce node -beta -alpha)
            do node.UndoUpdate ()
            if neweval >= beta then
                neweval
            else (max alpha neweval)
        List.fold traverser localalpha captures



