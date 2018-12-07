//Lewis Coates (c) April 7, 2011
namespace Celestia

open Primitives
open ZobristHash
open MoveGeneration

module GameState =
    
    ///An undo struct that should have every necessary to unwork a move
    [<NoEquality; NoComparison>]
    type private Undo = { OldMoves: Ply [] Lazy;
                          OldWhite: (Pieces * Position) list;
                          OldBlack: (Pieces * Position) list;
                          OldHash: int64;
                          OldState: Incrementor;
                          OldValue: Score;
                          OldPrevMove1: Pieces option * Position;
                          OldPrevMove2: Pieces option * Position; }


    ///State of a game
    ///This is mutable to be able to use a do/undo style semantics for
    ///performance
    [<NoEquality; NoComparison>]
    type GameState(fitness : Evaluator, ?board : Board, ?turn : Color) =

        let EvalFunc = fitness
        let TimeOut = 81
        let BoardState = defaultArg board (DefaultBoard ())

        let mutable Turn = defaultArg turn White
        let mutable IsPlaying = true
        let mutable Index = 1

        let mutable WhitePieces = piecesOfGame BoardState White
        let mutable BlackPieces = piecesOfGame BoardState Black


        let mutable ZobristHash = zobristAdder BoardState
        let mutable ScoreIncrementor = { BlackScore = 0;
                                         WhiteScore = 0;
                                         Advancement = 0;
                                         BlackPawnScore = 0;
                                         WhitePawnScore = 0 }
        let mutable Score = 0 

        let mutable UndoStack = []
        
        member val AvailableMoves = lazy(movesFrom (if Turn = White
                                                    then WhitePieces
                                                    else BlackPieces)
                                                   BoardState)
                                        with get, set


        ///undoes an update given an input
        member this.undoUpdate() : unit =
                match UndoStack with
                | prevState::newStack -> 
                    do this.AvailableMoves <- prevState.OldMoves
                    do WhitePieces <- prevState.OldWhite
                    do BlackPieces <- prevState.OldBlack
                    do ZobristHash <- prevState.OldHash
                    do ScoreIncrementor<- prevState.OldState
                    do Score <- prevState.OldValue
                    do Turn <- Turn.Not()
                    do IsPlaying <- true
                    do Index <- Index-1

                    let (piece1, (oldsx, oldsy)) = prevState.OldPrevMove1
                    let (piece2, (oldex, oldey)) = prevState.OldPrevMove2

                    do BoardState.[oldsx, oldsy] <- piece1
                    do BoardState.[oldex, oldey] <- piece2
                    do UndoStack <- newStack
                | [] -> ()

        ///an unchecked update function, applies the move to the gamestate
        member this.doUpdate((startx, starty), (endx, endy) : Position) =

            Turn <- Turn.Not()
            let capturepiece = BoardState.[endx, endy]
            let initialpiece = BoardState.[startx, starty]

            let willplay = match capturepiece with
                           //king is captured, game over
                           | Some(King(_)) -> false
                           //carry on
                           | _ -> true

            do IsPlaying <- willplay

            // Update the undo stack with a new undoer                
            UndoStack <- { OldMoves = this.AvailableMoves;
                           OldWhite = WhitePieces;
                           OldBlack = BlackPieces;
                           OldHash = ZobristHash;
                           OldState = ScoreIncrementor;
                           OldValue = Score;
                           OldPrevMove1 = (BoardState.[startx,starty],
                                           (startx,starty));
                           OldPrevMove2 = (BoardState.[endx,endy],
                                           (endx,endy)); } :: UndoStack

            //white goes to 0, black goes to N
            //These cases handle the pawn Promotion
            let newPiece = match endx, initialpiece with
                           | 0, Some(Pawn(Black)) -> Queen(Black)
                           | MaxXVal, Some(Pawn(White)) -> Queen(White)
                           | _, Some(other) -> other
                           | _, None -> InvalidMove(((startx, starty),
                                                     (endx, endy)), BoardState)
                                        |> raise

            //assign the correct piece to the 
            //endpoint clear the beginning piece, determine whether
            //the game is continuing return a new board of the
            //appropriate type

            BoardState.[endx, endy] <- Some(newPiece)
            BoardState.[startx, starty] <- None

            let removeStart (_,x) = not (x = (startx, starty))
            let removeEnd (_,x) = not (x = (endx, endy))

            let (blackPieces, whitePieces) = 
                 match Turn.Not() with
                 | Black -> (List.filter removeEnd BlackPieces,
                             (newPiece,(endx, endy))
                             :: List.filter removeStart WhitePieces)
                 | White -> ((newPiece,(endx, endy))
                             :: List.filter removeStart BlackPieces,
                             List.filter removeEnd WhitePieces)

            let newmoves = match Turn.Not() with
                                   | Black -> blackPieces
                                   | White -> whitePieces

            do BlackPieces <- blackPieces
            do WhitePieces <- whitePieces

            let x1 = (None,(startx, starty))
            let x2 = (Some(newPiece), (endx, endy))
            let x3 = (capturepiece, (endx, endy))
            let x4 = (initialpiece,(startx, starty))
            do ZobristHash <- incrementalZobristAdder ZobristHash x1 x2 x3 x4
            do this.AvailableMoves <- lazy(movesFrom newmoves BoardState)


            do Turn <- Turn.Not()
            do Index <- Index+1

            let updater = 
                    { OldPiece =(match initialpiece with 
                                 | Some(t) -> t 
                                 | _ -> raise <| Hell("invalid initialpiece"));
                      NewPiece = newPiece;
                      Taken = capturepiece;
                      Move = ((startx, starty),(endx, endy)); }

            let (newval, newinc) = EvalFunc updater
            do Score <- newval
            do ScoreIncrementor <- newinc

            assert(BoardState.[startx, starty] <> None)
            Score

        ///returns whether or not the tested game is terminal
        member this.IsTerminal() : bool =
                    if IsPlaying && Index <= TimeOut
                    then
                        false
                    else 
                        true

        ///prints out the board state
        override this.ToString() : string =

                     //no piece is represented by a .
                     let printer piece = 
                         match piece with
                         | Some(p) -> p.ToString()
                         | None -> "."

                     [ for i in 0 .. BoardLength do
                           yield Array.toList BoardState.[i, *] ]
                     |> List.map (fun i -> List.map printer i
                                           |> String.concat "")
                     |> String.concat "\n"
                     |> sprintf "turn:%d\n%s" Index

