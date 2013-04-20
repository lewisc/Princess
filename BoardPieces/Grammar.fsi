module ValidMoves
    open Elements
    type Board =
        class
            new : unit -> Board
            member Width : int
            member Length : int
            member WhitePieces : (Pieces * Position) List
            member BlackPieces : (Pieces * Position) List
            member Moves : Move List
            member Update : Move -> Board
            member Item : (int * int) -> Pieces option with get
            member Turn : Color
            member InPlay : bool
            override ToString: unit -> string
        end

    val getColor : Pieces -> Color
    val testValid : int -> int -> Board -> Color -> MoveType
    val validPieceMoves : Pieces -> Position -> Board -> Position List
    val scan : Position -> Board -> Color -> int option -> int -> int -> Position List
    val availableMoves : (Pieces * Position) List -> Board -> Move List


