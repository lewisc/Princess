namespace MoveGen
  type Position = int * int
  type Score = int
  exception Hell of string
  type Move = Position * Position
  type Game = Move list
  type Color =
    | Black
    | White
  type Pieces =
    | Bishop of Color
    | Pawn of Color
    | Queen of Color
    | Knight of Color
    | Rook of Color
    | King of Color
    with
      override ToString : unit -> string
      member Color : Color
    end
  type BoardSeq = seq<seq<Pieces option>>
  type GameState =
    {Turn: Color;
     BoardState: Pieces option [,];
     History: List<Move>;
     TimeOut: int;
     IsPlaying: bool;}
  module BoardOperators = begin
    val notColor : Color -> Color
  end

namespace MoveGen
  exception InvalidMove of Move
  type Evaluator = GameState -> Score
  type private MoveType =
    | Capture
    | Normal
    | Invalid
  type private PieceCaptures =
    | Free
    | Take
    | Both
  module BoardBehaviors = begin
    val private defaultBoard : Pieces option [,]
    val initialState : unit -> GameState
    val boardLength : GameState -> int
    val boardWidth : GameState -> int
    val piecesOfGame : GameState -> Color -> (Pieces * (int * int)) list
    val arrayAsSeq : 'a [,] -> seq<seq<'a>>
    val boardToString : GameState -> string
    val private testValidMove : int -> int -> GameState -> Color -> MoveType
    val private scanMovesGame :
      int * int ->
        GameState -> Color -> PieceCaptures -> int -> int -> int -> List<Move>
    val validMoves : Pieces -> int * int -> GameState -> List<Move>
    val movesFrom : List<Pieces * Position> -> GameState -> Move list
    val availableMoves : GameState -> Move list
    val update : GameState -> (int * int) * (int * int) -> GameState
    val checkedUpdate : GameState -> (int * int) * (int * int) -> GameState
    val ( |ParseRegex|_| ) : string -> string -> string list option
    val ( |Integer|_| ) : string -> int option
    val ( |ColToNum|_| ) : string -> int option
    val ( |ReadInput|_| ) : string -> ((int * int) * (int * int)) option
    val printMove : (int * int) * (int * int) -> unit
    val sprintMove : (int * int) * (int * int) -> string
    val isTerminal : GameState -> bool
    val tryPlay : GameState -> string -> bool * GameState
    val testGame : Game -> bool
    val perfTest : Game -> bool
  end

