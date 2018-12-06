//Lewis Coates (c) April 7, 2011
namespace Celestia

open System
open System.Collections.Generic

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
         member self.Color = match self with
                             | King(x) | Queen(x)| Knight(x) 
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
    let sprintMove (((from1, from2), (to1, to2))) : string =
        let chars = [| 'e'; 'd'; 'c'; 'b'; 'a' |]
        (sprintf "%c%d-%c%d" (chars.[from2]) (from1+1) (chars.[to2]) (to1+1))

    ///generate a board from a given list of positions and pieces
    let buildBoard (positions) : Board =

        let emptyBoard = List.replicate 5 None
                         |> List.replicate 6
                         |> array2D

        do List.map (fun ((x,y), piece) ->
                    emptyBoard.[y, x] <- Some(piece))
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


    ///the type that incrementally evaluates the gamestate
    type Evaluator = StateUpdate -> Score * Incrementor


    let private piecesOption = [| Some(King(Black));
                                  Some(Queen(Black));
                                  Some(Knight(Black));
                                  Some(Rook(Black));
                                  Some(Pawn(Black));
                                  Some(Bishop(Black)); 
                                  Some(King(White));
                                  Some(Queen(White));
                                  Some(Knight(White));
                                  Some(Rook(White));
                                  Some(Pawn(White));
                                  Some(Bishop(White)); |]

    let private pieceXPosition = piecesOption
                                 |> Array.map (fun x -> Array.map (fun y -> (x, y)) AllPositions)
                                 |> Array.concat

    let private rand = Random()

    let private add64bit ()= 
        let retval = Array.create 8 0uy
        do rand.NextBytes(retval)
        BitConverter.ToInt64(retval,0)

    let private associater64 (elements:'a []) =
        let rec adder (map:Set<int64>) =
            if map.Count < elements.Length
            then    
                    adder (map.Add(add64bit()))
            else map
        let ret map = Set.toArray map
                   |> Array.zip elements
        let mymap = adder Set.empty
        ret mymap

    let private zobristMap = 
                let keys = associater64 pieceXPosition
                let todictionary = Dictionary<(Pieces option * Position),
                                               int64>(390)
                Array.iter(fun (x, y) -> todictionary.Add(x, y)) keys
                todictionary

    let zobristAdder (input : Pieces option [,]) =
        let ret = Array.fold (fun x (z1, z2) ->
                x ^^^ (zobristMap.[input.[z1, z2], (z1, z2)])) 0L AllPositions
        ret

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
        let mutable AvailableMoves = lazy([||]) //lazy(movesFrom pieces board)
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

        ///undoes an update given an input
        member this.undoUpdate() : unit =
                match UndoStack with
                | prevState::newStack -> 
                    do AvailableMoves <- prevState.OldMoves
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

            // Update the undo stack with a new undoer                
            UndoStack <- { OldMoves = AvailableMoves;
                           OldWhite = WhitePieces;
                           OldBlack = BlackPieces;
                           OldHash = ZobristHash;
                           OldState = ScoreIncrementor;
                           OldValue = Score;
                           OldPrevMove1 = (BoardState.[startx,starty], (startx,starty));
                           OldPrevMove2 = (BoardState.[endx,endy], (endx,endy)); } :: UndoStack

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

            let x1 = (None,(startx, starty))
            let x2 = (Some(newPiece), (endx, endy))
            let x3 = (capturepiece, (endx, endy))
            let x4 = (initialpiece,(startx, starty))
            let newHash = ZobristHash 
                      ^^^ (zobristMap.[x1])
                      ^^^ (zobristMap.[x2])
                      ^^^ (zobristMap.[x3])
                      ^^^ (zobristMap.[x4])
            assert(newHash = zobristAdder BoardState)


            do Turn <- Turn.Not();
            do IsPlaying <- willplay;
            do Index <- Index+1;
            do BlackPieces <- blackPieces;
            do WhitePieces <- whitePieces;
            do AvailableMoves <- lazy([||])//lazy(movesFrom newmoves BoardState)
            do ZobristHash <- newHash;

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

