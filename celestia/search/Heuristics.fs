namespace Celestia

open Primitives
open GameState

module Heuristics =

    let private pieceValue piece =
                match piece with
                | King(_) -> Inf 
                | Queen(_) -> 90
                | Rook(_) -> 50
                | Bishop(_) -> 30 
                | Knight(_) -> 30 
                | Pawn(_) -> 10

    let initialIncrementalSimple (game : GameState) = 
        let whites = List.sum (List.map pieceValue (List.map fst game.WhitePieces))
        let black = List.sum (List.map pieceValue (List.map fst game.BlackPieces))


        let score = whites - black * (if game.Turn = Black then -1 else 1)

        (score, 0)

    let incrementalSimple (input : StateUpdate) (game : GameState) =
        let newScore = match input.Taken with
                       | Some(piece) -> game.Score - (pieceValue piece)
                       | None -> 0
        (newScore, input.Taken)
