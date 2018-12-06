//Lewis Coates (c) April 7, 2011
namespace Celestia

module Primitives =

    ///0-Indexed Lenght of Board
    [<Literal>]
    let MaxXVal = 5

    ///0-Indexed width of Board
    [<Literal>]
    let MaxYVal = 4

    ///Length of Board
    [<Literal>]
    let BoardWidth = 5

    ///Width of Board
    [<Literal>]
    let BoardLength = 6

    [<Literal>]
    let inf = 1000

    let botMove = ((-1,-1),(-1,-1))

    ///Type alias for position
    type Position = int * int

    ///type for score, probably an int, might have units attached eventually
    type Score = int

    ///Exception used when a provably invalid state occurs
    exception Hell of string

    ///type alias for a move
    type Ply = Position * Position

    ///type alias for a variation
    type Variation = Ply list

    ///Type of possible Players(white or black)
    type Color =
        | Black
        | White

         ///used for the IMCS server, and other diagnostics
         ///needs to be capital
    with override self.ToString() : string = match self with
                                             | Black -> "B"
                                             | White -> "W"

         ///inverts the given color(i.e. gives the color of opponent)
         member this.Not() : Color = match this with
                                     | Black -> White
                                     | White -> Black

    ///Different possible pieces
    type Pieces =
        | Bishop of Color
        | Pawn of Color
        | Queen of Color
        | Knight of Color
        | Rook of Color
        | King of Color

         ///Print out a character representing the piece and it's color
    with override self.ToString() : string = match self with
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

         ///return the color
         member self.Color = match self with
                             | King(x) | Queen(x)| Knight(x) 
                             | Rook(x) | Pawn(x) | Bishop(x) -> x

    ///Board Type alias
    type Board = Pieces option [,]

    ///array of all the different board positions
    let AllPositions = [| for i in 0 .. MaxXVal do
                             for j in 0 .. MaxYVal-> (i, j) |]

    ///represents a default board
    let DefaultBoard () = 
     array2D [| [| Some(King(White)); Some(Queen(White));  Some(Bishop(White)); Some(Knight(White)); Some(Rook(White)) |]
                [| Some(Pawn(White)); Some(Pawn(White));   Some(Pawn(White));   Some(Pawn(White));   Some(Pawn(White)) |]
                [| None ;             None ;               None;                None;                None              |]
                [| None ;             None ;               None;                None;                None              |]
                [| Some(Pawn(Black)); Some(Pawn(Black));   Some(Pawn(Black));   Some(Pawn(Black));   Some(Pawn(Black)) |]
                [| Some(Rook(Black)); Some(Knight(Black)); Some(Bishop(Black)); Some(Queen(Black));  Some(King(Black)) |] |]


    ///Incremental evaluation state
    //TODO:Should be genericized
    [<StructAttribute>]
    type Incrementor = { BlackScore: Score;
                         WhiteScore: Score;
                         Advancement: Score;
                         BlackPawnScore: Score;
                         WhitePawnScore: Score; }

    /// used to update state
    type StateUpdate = { OldPiece: Pieces;
                         NewPiece: Pieces;
                         Taken: Pieces option;
                         Move: Ply; }


    ///State of a game
    ///This is mutable to be able to use a do/undo style semantics for
    ///performance
    [<NoEquality; NoComparison>]
    type GameState =
            { EvalFunc: StateUpdate -> GameState -> Score * Incrementor
              TimeOut: int;
              BoardState: Pieces option [,];
              mutable Turn: Color;
              mutable IsPlaying: bool;
              mutable AvailableMoves: Ply [] Lazy;
              mutable Index: int;
              mutable WhitePieces: (Pieces * Position) list;
              mutable BlackPieces: (Pieces * Position) list;
              mutable ZobristHash: int64;
              mutable State: Incrementor;
              mutable Value: Score; }

         ///returns whether or not the tested game is terminal
    with member this.IsTerminal() : bool =
                    if this.IsPlaying && this.Index <= this.TimeOut
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
                            yield Array.toList this.BoardState.[i, *] ]
                      |> List.map (fun i -> List.map printer i
                                            |> String.concat "")
                      |> String.concat "\n"
                      |> sprintf "turn:%d\n%s" this.Index


    ///the type that incrementally evaluates the gamestate
    type Evaluator = StateUpdate -> GameState -> Score * Incrementor

    ///An undo struct that should have every necessary to unwork a move
    [<NoEquality; NoComparison>]
    type Undo = { OldMoves: Ply [] Lazy;
                  OldWhite: (Pieces * Position) list;
                  OldBlack: (Pieces * Position) list;
                  OldHash: int64;
                  OldState: Incrementor;
                  OldValue: Score;
                  mutable OldPrevMove1: Pieces option * Position;
                  mutable OldPrevMove2: Pieces option * Position; }

    ///Invalid move exception
    [<NoEquality; NoComparison>]
    exception public InvalidMove of Ply * GameState

    ///gets the pieces on the board of the associated color
    let piecesOfGame (board : Pieces option [,]) (color : Color)
                                                   : (Pieces * Position) list =
               board
               |> Array2D.mapi (fun x y p ->
                      match p  with 
                      | Some(p) when p.Color = color -> Some(p, (x, y))
                      | _ -> None) 
               |> Seq.cast<(Pieces * Position) option>
               |> Seq.toList
               |> List.choose id


    ///pretty print for moves
    let sprintMove ((from1,from2),(to1,to2)) =
        let chars = [| 'e'; 'd'; 'c'; 'b'; 'a' |]
        (sprintf "%c%d-%c%d" (chars.[from2]) (from1+1) (chars.[to2]) (to1+1))

    ///generate a board from a given list of positions and pieces
    let buildBoard (positions: ((int * int) * Pieces) list) =

        let emptyBoard = List.replicate 5 None
                         |> List.replicate 6
                         |> array2D

        do List.map (fun ((x,y), piece) ->
                    emptyBoard.[y, x] <- Some(piece))
                    positions
           |> ignore
        emptyBoard
