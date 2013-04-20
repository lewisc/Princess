//Lewis Coates (c) April 7, 2011
namespace MoveGeneration

//Type alias for position
type Position = int * int

//type for score, probably an int, might have units attached eventually
type Score = int

//Exception used when a provably invalid state occurs
exception Hell of string

//type alias for a move
type Ply = Position * Position

//type alias for a variation
type Variation = Ply list

//Type of possible Players(white or black)
type Color =
     | Black
     | White
//used for the IMCS server, and other diagnostics
//needs to be captiol
with override self.ToString() =
        match self with
        | Black -> "B"
        | White -> "W"

//Different possible pieces
type Pieces =
     | Bishop of Color
     | Pawn of Color
     | Queen of Color
     | Knight of Color
     | Rook of Color
     | King of Color
//Print out a character representing the piece and it's color
with  override self.ToString() = 
//prints out a character representing the piece
                    match self with
                    | King(White) -> "K"
                    | King(Black) -> "k"
                    | Queen(White) -> "Q"
                    | Queen(Black) -> "q"
                    | Knight(Black) -> "n"
                    | Knight(White) -> "N"
                    | Rook(White) -> "R"
                    | Rook(Black) -> "r"
                    | Pawn(Black) -> "p"
                    | Pawn(White) -> "P"
                    | Bishop(Black) -> "b"
                    | Bishop(White) -> "B"
//type alias for the board as a sequence of sequences
type BoardSeq = Pieces option seq seq

type Incrementor =
            {BlackScore:Score;
             WhiteScore:Score;
             Advancement:Score;
             BlackPawnScore:Score;
             WhitePawnScore:Score;
             }

//the type that gets passed into the evaluator
//to update the state
type StateUpdate =
    {OldPiece:Pieces;
     NewPiece:Pieces;
     Taken:Pieces option;
     Move:Ply;}


//new record for the new model of code
type GameState = 
            {mutable Turn:Color;
             BoardState:Pieces option [,];
             TimeOut:int;
             mutable IsPlaying:bool;
             mutable AvailableMoves:Ply list Lazy;
             mutable Index:int;
             mutable WhitePieces:(Pieces * Position) list;
             mutable BlackPieces:(Pieces*Position) list;
             mutable ZobristHash:int64;
             mutable State:Incrementor;
             EvalFunc: StateUpdate -> GameState -> Score * Incrementor;
             mutable Value:Score;
             }

//the type that incrementally evaluates the gamestate
type Evaluator = StateUpdate -> GameState -> Score * Incrementor


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