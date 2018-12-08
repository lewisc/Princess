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

    ///Infinity score, termination conditions
    [<Literal>]
    let Inf = 1000

    /// A move that is in error, can not happen
    let BotMove = ((-1, -1), (-1, -1))

    ///Position on a board
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
         member self.Color : Color = match self with
                                     | King(x) | Queen(x) | Knight(x) 
                                     | Rook(x) | Pawn(x) | Bishop(x) -> x

    ///Board Type alias
    type Board = Pieces option [,]

    ///Piece and where it is on the board
    type PieceLoc = Pieces * Position

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

    ///gets the pieces on the board of the associated color
    let piecesOfGame (board : Board) (color : Color) : PieceLoc list =
               board
               |> Array2D.mapi (fun x y p ->
                      match p  with 
                      | Some(p) when p.Color = color -> Some(p, (x, y))
                      | _ -> None) 
               |> Seq.cast<(Pieces * Position) option>
               |> Seq.toList
               |> List.choose id


    ///pretty print for moves
    let sprintMove (((from1, from2), (to1, to2)) : Ply) : string =
        let chars = [| 'e'; 'd'; 'c'; 'b'; 'a' |]
        (sprintf "%c%d-%c%d" (chars.[from2]) (from1+1) (chars.[to2]) (to1+1))

    ///generate a board from a given list of positions and pieces
    let buildBoard (positions : PieceLoc list ) : Board =

        let emptyBoard = List.replicate 5 None
                         |> List.replicate 6
                         |> array2D

        do List.map (fun (piece, (x, y)) -> emptyBoard.[y, x] <- Some(piece))
                    positions
           |> ignore
        emptyBoard


    ///Invalid move exception
    [<NoEquality; NoComparison>]
    exception public InvalidMove of Ply * Board

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

    ///the type that incrementally evaluates the gamestate
    type Evaluator = StateUpdate -> Score * Incrementor
