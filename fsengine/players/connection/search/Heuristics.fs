namespace Searching
open MoveGeneration
open BoardCombinators
open BoardHelpers

module Heuristics =
    [<Literal>]
    let advancementValue = 1

    let onPlay c = match c with
                   | White -> 1
                   | Black -> -1

    let pieceValue piece =
        match piece with
        | King(_) -> inf 
        | Queen(_) -> 90
        | Rook(_) -> 50
        | Bishop(_) -> 30 
        | Knight(_) -> 30 
        | Pawn(_) -> 10

    let initialSimple game = 
        let whites = List.sum (List.map pieceValue (List.map fst game.WhitePieces))
        let black = List.sum (List.map pieceValue (List.map fst game.BlackPieces))

        let score = whites - black 

        (score, new Incrementor (whitescore=whites,
                                 blackscore=black,
                                 blackpawnscore=0,
                                 whitepawnscore=0,
                                 advancement=0))

    let SimpleCount (input) (game:GameState) : Score * Incrementor=
        let old = input.OldPiece
        let newpiece = input.NewPiece
        let taken = input.Taken
        assert(pieceColor old = pieceColor newpiece)
        let (blackscore,whitescore) = 
            match pieceColor newpiece with
            | Black -> match taken with
                       | None -> (game.State.BlackScore - (pieceValue old) + (pieceValue newpiece), game.State.WhiteScore)
                       | Some(y) ->
                                assert(pieceColor y <> pieceColor old)
                                (game.State.BlackScore -  (pieceValue old) + (pieceValue newpiece), game.State.WhiteScore - (pieceValue y))
            | White -> match taken with 
                       | None -> (game.State.BlackScore, game.State.WhiteScore - (pieceValue old) + (pieceValue newpiece))
                       | Some(y) ->
                                assert(pieceColor y <> pieceColor old)
                                (game.State.BlackScore-(pieceValue y), game.State.WhiteScore - (pieceValue old) + (pieceValue newpiece))

        let retval = (whitescore-blackscore)*(onPlay game.Turn)
        assert(retval = (fst (initialSimple game))*(onPlay game.Turn))
        ((retval),new Incrementor (whitescore=whitescore,
                                   blackscore=blackscore,
                                   blackpawnscore=0,
                                   whitepawnscore=0,
                                   advancement=0))

    let initialAdvancement game = 
        let whites = List.sum (List.map pieceValue (List.map fst game.WhitePieces))
        let black = List.sum (List.map pieceValue (List.map fst game.BlackPieces))

        let score = whites - black 

        (score, new Incrementor (whitescore=whites,
                                 blackscore=black,
                                 blackpawnscore=0,
                                 whitepawnscore=0,
                                 advancement=0))

    let Advancement (input) (game:GameState) : Score * Incrementor=
        let old = input.OldPiece
        let newpiece = input.NewPiece
        let taken = input.Taken
        let ((startx,starty),(endx,endy)) = input.Move
        assert(pieceColor old = pieceColor newpiece)


        let (blackscore,whitescore) = 
            match pieceColor newpiece with
            | Black -> match taken with
                       | None -> (game.State.BlackScore - (pieceValue old) + (pieceValue newpiece), game.State.WhiteScore)
                       | Some(y) ->
                                assert(pieceColor y <> pieceColor old)
                                (game.State.BlackScore -  (pieceValue old) + (pieceValue newpiece), game.State.WhiteScore - (pieceValue y))
            | White -> match taken with 
                       | None -> (game.State.BlackScore, game.State.WhiteScore - (pieceValue old) + (pieceValue newpiece))
                       | Some(y) ->
                                assert(pieceColor y <> pieceColor old)
                                (game.State.BlackScore-(pieceValue y), game.State.WhiteScore - (pieceValue old) + (pieceValue newpiece))
        let pawncapture =
            match taken with
            | Some(Pawn(color)) -> match color with
                                   | White -> ((fst(snd(input.Move))-1)*advancementValue)
                                   | Black -> ((4-(fst(snd(input.Move))))*advancementValue)
            | _ -> 0
        let (whitepawn,blackpawn) =
            match newpiece with
            | Pawn(color) -> let eleft =  if endy-1 >= 0 then game.BoardState.[endx,endy-1] else None
                             let eright = if endy+1 <= 4 then game.BoardState.[endx,endy+1] else None
                             let sleft =  if starty-1 >= 0 then game.BoardState.[startx,starty-1] else None
                             let sright = if starty+1 <= 4 then game.BoardState.[startx,starty+1] else None

                             match color with
                             | White ->let eleft =  if endy-1 >= 0 then game.BoardState.[endx,endy-1] else None
                                       let eright = if endy+1 <= 4 then game.BoardState.[endx,endy+1] else None
                                       game.State.WhitePawnScore+advancementValue, game.State.BlackPawnScore-pawncapture
                             | Black-> game.State.WhitePawnScore-pawncapture, game.State.BlackPawnScore+advancementValue
            | _ -> game.State.WhitePawnScore, game.State.BlackPawnScore

        let retval = (whitescore-blackscore + whitepawn-blackpawn)*(onPlay game.Turn)
        ((retval),new Incrementor (whitescore=whitescore,
                                   blackscore=blackscore,
                                   blackpawnscore=blackpawn,
                                   whitepawnscore=whitepawn,
                                   advancement=0))
