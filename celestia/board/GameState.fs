//Lewis Coates (c) April 7, 2011
namespace Celestia

open Primitives
open ZobristHash
open MoveGeneration

module GameState =
    
    ///An undo struct that should have every necessary to unwork a move
    [<NoEquality; NoComparison>]
    type private Undo = { OldMoves: Ply list Lazy;
                          OldWhite: (Pieces * Position) list;
                          OldBlack: (Pieces * Position) list;
                          OldValue: Score;
                          OldPrevMove1: Pieces option * Position;
                          OldPrevMove2: Pieces option * Position; }


    ///State of a game
    ///This is mutable to be able to use a do/undo style semantics for
    ///performance
    [<NoEquality; NoComparison>]
    type GameState(fitness : Evaluator,
                   initialIncrementor : Board -> Score,
                   ?board : Board,
                   ?turn : Color) =

        let evalFunc = fitness
        let timeOut = 81
        let boardState = defaultArg board (DefaultBoard ())

        let mutable turn = defaultArg turn White
        let mutable isPlaying = true
        let mutable index = 1

        let mutable whitePieces = piecesOfGame boardState White
        let mutable blackPieces = piecesOfGame boardState Black


        let zobristHash = new ZobristHash(boardState)
        let mutable score = initialIncrementor boardState

        let mutable undoStack = []
        let mutable availableMoves = lazy(movesFrom (if turn = White
                                                     then whitePieces
                                                     else blackPieces)
                                                    boardState)
        
        member this.AvailableMoves with get () = availableMoves

        //TODO: REconsider hbing these have a backing value
        member this.Turn with get () = turn
        member this.Score with get () = score
        member this.ZobristHash with get () = zobristHash.ZobristValue
        member this.WhitePieces with get () = whitePieces
        member this.BlackPieces with get () = blackPieces 

        ///undoes an update given an input
        member this.UndoUpdate() : unit =
                match undoStack with
                | prevState::newStack -> 
                    do availableMoves <- prevState.OldMoves
                    do whitePieces <- prevState.OldWhite
                    do blackPieces <- prevState.OldBlack
                    do score <- prevState.OldValue
                    do turn <- turn.Not()
                    do isPlaying <- true
                    do index <- index-1


                    let (piece1, (oldsx, oldsy)) = prevState.OldPrevMove1
                    let (piece2, (oldex, oldey)) = prevState.OldPrevMove2

                    let x1 = (piece2, (oldex, oldey))
                    let x4 = (piece2, (oldsx, oldsy))
                    let x2 = (piece1, (oldsx, oldsy))
                    let x3 = (piece1, (oldex, oldey))
                    do zobristHash.IncrementalAdder x1 x2 x3 x4

                    do boardState.[oldsx, oldsy] <- piece1
                    do boardState.[oldex, oldey] <- piece2
                    do undoStack <- newStack
                | [] -> ()

        ///an unchecked update function, applies the move to the gamestate
        member this.DoUpdate((startx, starty), (endx, endy) : Position) =

            turn <- turn.Not()
            let capturepiece = boardState.[endx, endy]
            let initialpiece = boardState.[startx, starty]

            let willplay = match capturepiece with
                           //king is captured, game over
                           | Some(King(_)) -> false
                           //carry on
                           | _ -> true

            do isPlaying <- willplay

            // Update the undo stack with a new undoer                
            undoStack <- { OldMoves = availableMoves;
                           OldWhite = whitePieces;
                           OldBlack = blackPieces;
                           OldValue = score;
                           OldPrevMove1 = (boardState.[startx, starty],
                                           (startx, starty));
                           OldPrevMove2 = (boardState.[endx, endy],
                                           (endx, endy)); } :: undoStack

            //white goes to 0, black goes to N
            //These cases handle the pawn Promotion
            let newPiece = match (endx, initialpiece) with
                           | (0, Some(Pawn(Black))) -> Queen(Black)
                           | (MaxXVal, Some(Pawn(White))) -> Queen(White)
                           | (_, Some(other)) -> other
                           | (_, None) -> InvalidMove(((startx, starty),
                                                       (endx, endy)),
                                                      boardState)
                                          |> raise

            //assign the correct piece to the 
            //endpoint clear the beginning piece, determine whether
            //the game is continuing return a new board of the
            //appropriate type

            boardState.[endx, endy] <- Some(newPiece)
            boardState.[startx, starty] <- None

            let removeStart (_,x) = x <> (startx, starty)
            let removeEnd (_,x) = x <> (endx, endy)

            let (newBlackPieces, newWhitePieces) = 
                 match turn.Not() with
                 | Black -> (List.filter removeEnd blackPieces,
                             (newPiece, (endx, endy))
                             :: List.filter removeStart whitePieces)
                 | White -> ((newPiece, (endx, endy))
                             :: List.filter removeStart blackPieces,
                             List.filter removeEnd whitePieces)

            let newMoves = match turn.Not() with
                           | Black -> blackPieces
                           | White -> whitePieces

            do blackPieces <- newBlackPieces
            do whitePieces <- newWhitePieces

            let x1 = (None, (startx, starty))
            let x2 = (Some(newPiece), (endx, endy))
            let x3 = (capturepiece, (endx, endy))
            let x4 = (initialpiece, (startx, starty))
            do zobristHash.IncrementalAdder x1 x2 x3 x4
            do availableMoves <- lazy(movesFrom newMoves boardState)


            do turn <- turn.Not()
            do index <- index+1

            let updater = 
                    { OldPiece = match initialpiece with 
                                 | Some(t) -> t 
                                 | _ -> raise <| Hell("invalid initialpiece");
                      NewPiece = newPiece;
                      Taken = capturepiece;
                      Move = ((startx, starty),(endx, endy)); }

            let newval = evalFunc updater
            do score <- newval

            assert(boardState.[startx, starty] <> None)
            score

        ///returns whether or not the tested game is terminal
        member this.IsTerminal() : bool =
                    isPlaying && index <= timeOut

        ///prints out the board state
        override this.ToString() : string =

                     //no piece is represented by a .
                     let printer piece = 
                         match piece with
                         | Some(p) -> p.ToString()
                         | None -> "."

                     [ for i in 0 .. BoardLength do
                           yield Array.toList boardState.[i, *] ]
                     |> List.map (fun i -> List.map printer i
                                           |> String.concat "")
                     |> String.concat "\n"
                     |> sprintf "turn:%d\n%s" index


