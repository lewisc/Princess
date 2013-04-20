namespace MoveGeneration
    ///Type alias for score
    type Score = int

    ///type alias for position
    type Position = int * int

    ///exception for unecoverable errors
    exception Hell of string

    ///type alias for ply, needs to be renamed 
    type Ply = Position * Position

    ///type alias for series of moves 
    type Variation = Ply list

    ///the variant representing player, used for anything owned by a player
    ///(such as pieces or turn)
    type Color =
         | Black
         | White
    ///has a special override for converting to a decent name(W for white,
    ///B for black, IMCS standard)
    with override ToString: unit -> string

    ///all the different pieces available
    ///and their respective owner
    type Pieces =
         | Bishop of Color
         | Pawn of Color
         | Queen of Color
         | Knight of Color
         | Rook of Color
         | King of Color
          ///returns standard algebraic character of the piece(capitol = W,
          ///lowercase =B, K = king, n = knight, all other first 
          ///character of piece name
    with  override ToString : unit -> string  

    //the type that gets passed into the evaluator
    //to update the state
    type StateUpdate =
        {OldPiece:Pieces;
         NewPiece:Pieces;
         Taken:Pieces option;
         Move:Ply;}

    ///represents the board as a sequence of sequences to be
    ///folded on
    type BoardSeq = Pieces option seq seq

    type Incrementor =
                {BlackScore:Score;
                 WhiteScore:Score;
                 Advancement:Score;
                 BlackPawnScore:Score;
                 WhitePawnScore:Score;
                 }

    
    ///the current state of the game, everything that needs to be known
    ///by anyone who needs to process it. Note that this carries around some
    ///meta data for convenience
    type GameState = 
                {mutable Turn:Color;
                 BoardState:Pieces option [,];
                 TimeOut:int;
                 mutable IsPlaying:bool;
                 mutable AvailableMoves:Ply List Lazy;
                 mutable Index:int;
                 mutable WhitePieces:(Pieces * Position) list;
                 mutable BlackPieces:(Pieces * Position) list;
                 mutable ZobristHash:int64;
                 mutable State:Incrementor;
                 EvalFunc: StateUpdate -> GameState -> Score * Incrementor;
                 mutable Value:Score;
                 }

    ///an incremental evaluator, the incremental piece is updated 
    ///as a performance hack
    type Evaluator =  StateUpdate -> GameState -> Score * Incrementor

    type Undo = 
           {OldMoves: Ply List Lazy;
            OldWhite:(Pieces * Position) List;
            OldBlack:(Pieces * Position) List;
            OldHash:int64;
            OldState:Incrementor;
            OldValue:Score;
            mutable OldPrevMove1:Pieces option * Position;
            mutable OldPrevMove2:Pieces option * Position;
            }
