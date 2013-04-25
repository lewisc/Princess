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

///Board Type alias
type Board = Pieces option [,]


//Type that get's passed into the Evaluator
//TODO:Should be genericized
type Incrementor =
    struct
        val BlackScore     :Score;
        val WhiteScore     :Score;
        val Advancement    :Score;
        val BlackPawnScore :Score;
        val WhitePawnScore :Score;
        new(blackscore:Score, whitescore:Score, advancement:Score, blackpawnscore:Score, whitepawnscore:Score) = {BlackScore = blackscore;
                                                                                                                 WhiteScore = whitescore;
                                                                                                                 Advancement = advancement;
                                                                                                                 BlackPawnScore = blackpawnscore;
                                                                                                                 WhitePawnScore = whitepawnscore;}
    end
            

//the type that gets passed into the evaluator
//to update the state
type StateUpdate =
    {OldPiece:Pieces;
     NewPiece:Pieces;
     Taken:Pieces option;
     Move:Ply;}


//Do/Undo State of a game. This is why this struct is all mutable
[<NoEquality; NoComparison>]
type GameState = 
            { 
              EvalFunc: StateUpdate -> GameState -> Score * Incrementor;
              TimeOut:int;
              BoardState:Pieces option [,];
              mutable Turn:Color;
              mutable IsPlaying:bool;
              mutable AvailableMoves:((Ply []) Lazy);
              mutable Index:int;
              mutable WhitePieces:(Pieces * Position) list;
              mutable BlackPieces:(Pieces*Position) list;
              mutable ZobristHash:int64;
              mutable State:Incrementor;
              mutable Value:Score;
             }

//the type that incrementally evaluates the gamestate
type Evaluator = StateUpdate -> GameState -> Score * Incrementor


///An undo struct that should have every necessary to unwork a move
[<NoEquality; NoComparison>]
type Undo = 
       {OldMoves:(Ply []) Lazy;
        OldWhite:(Pieces * Position) List;
        OldBlack:(Pieces * Position) List;
        OldHash:int64;
        OldState:Incrementor;
        OldValue:Score;
        mutable OldPrevMove1:Pieces option * Position;
        mutable OldPrevMove2:Pieces option * Position;
        }
