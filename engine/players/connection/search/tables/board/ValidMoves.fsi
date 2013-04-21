namespace MoveGeneration
  

  ///an invalid move, with the move and the given state for diagnostics
  exception InvalidMove of Ply * GameState

  ///types of moves that can occur, used to differentiate pawn forward vs
  ///pawn sideways attack
  type private MoveType =
    | Capture of (Pieces * Position)
    | Normal
    | Invalid

  ///what type of capture is acceptable, either a free square, a piece
  ///or either
  type private PieceCaptures =
    | Free
    | Take
    | Both

  ///a number of combinators on the GameState and the other various
  ///move generation types(such as color)
  module BoardCombinators = 
     begin
        ///returns the color of the piece 
        val pieceColor : Pieces -> Color
        ///inverts the color
        val notColor : Color -> Color

        ///prints the current color in a readable(used for imcs)
        val printColor : Color -> string
        ///prints the move in algebraid chess notation(a-...across the top, 
        ///1-...from white back to black back)
        val printMove : Ply -> unit
        ///prints the move to a string following the printmove rules
        val sprintMove : Ply -> string
        ///prints the board as a human readable string, used for diagnostics
        val sprintBoard : GameState -> string

        ///a constructor for the default state
        val initialState : Evaluator -> (GameState -> Score * Incrementor) -> GameState

        ///tests to see if the node is terminal
        val isTerminal : GameState -> bool
        ///attempts to play the move(as a string) on the game, with success
        ///returned as a boolean
        val tryPlay : GameState -> string -> bool * GameState
        ///returns the pieces and their relevant positions
        val piecesOfGame : Pieces option[,] -> Color -> 
                                                    (Pieces * Position) list
        ///updates the gamestate on the assumption it is passed a valid move
        val update : GameState -> Ply -> GameState
        ///updates the gamestate and checks if the move is valid
        ///<exception cref=InvalidMove>thrown when move is invalid</exception>
        val checkedUpdate : GameState -> Ply -> GameState
        ///Applys the undo struct to the Gamestate 
        //and then mutates the gamestate,returning the mutated GameState
        val doUpdate : GameState -> Ply-> GameState * Undo
        ///undoes the applied update
        val undoUpdate : GameState -> Undo -> unit

        ///converts a 2 dimensional array into a sequence of sequences
        val arrayAsSeq : 'a [,] -> seq<seq<'a>>

        val testGame : Variation-> bool
        val perfTest : Variation-> bool
      end
