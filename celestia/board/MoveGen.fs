//Lewis Coates (c) April 7, 2011                                                
namespace MoveGeneration


module MoveCalculation =

    open System
    open TypedInput
    open ZobristKeys
    open BoardConstants
    open BoardHelpers

    //Everything except for the end main move generator is private for information leakage, operations should be performed
    //on lists of moves/arrays of moves, not exposing the internals of the move calculation

    //capture might be useful to expose, but probably not

    //the different things that can happen with a move.
    //invalid represents a move such as trying to capture your own piece
    type private MoveType = 
         | Capture of (Pieces * Position)
         | Normal
         | Invalid

    //Types of ways that a square can be captured
    type private PieceCaptures =
         | Free
         | Take
         | Both

    //returns the possibilities for a move(capture, invalid, normal).
    //Normal moves can be continued, invalid/captures can't
    let inline private testValidMove (x:int) (y:int) (board:Pieces option[,]) (hue:Color)  : MoveType =
                if (x > maxXVal  || x < 0  || y > maxYVal  || y < 0)
                //if the move is off the board
                then Invalid
                else match (board.[x,y]) with
                     //the case where the desitnation is empty
                     | (None) -> Normal
                     | (Some(z)) -> match (pieceColor z,hue) with 
                                        //can't capture own piece
                                       | (Black, Black) 
                                       | (White, White) -> Invalid
                                       | (Black, White) 
                                       | (White, Black) -> Capture(z,(x,y))

    //helper function that returns a list of 
    //possible moves along a dx/dy move allowance.
    //this is the main engine
    let inline private scanMovesGame ((x:int),(y:int)) (board:Board) (hue:Color) (attacktype:PieceCaptures) (endcount:int) (retval:Ply []) (dx:int) (dy:int)  (index:int) : (Ply []) * int =
                //march through the possibiliti
                let rec scanloop xnew ynew count index = 
                    let newx = xnew+dx
                    let newy = ynew+dy
                    if endcount < count  then (retval, (index)) else
                    //march through the possibilities
                    match (testValidMove newx newy board hue) with
                    | Invalid -> (retval, (index))
                    | Normal  -> match attacktype with
                                 | Take -> scanloop newx newy (count+1) index
                                 | Both | Free ->  
                                     let newIndex = index+1
                                     do retval.[newIndex] <- ((x,y),(newx,newy))
                                     scanloop newx newy (count+1) newIndex
                    | Capture(_) -> 
                                match attacktype with
                                | Free -> (retval, index)
                                | Both | Take -> let newIndex = index+1
                                                 do retval.[newIndex] <- ((x,y),(newx,newy))
                                                 (retval, newIndex)
                scanloop x y 1 index 
      
    //gets a list of all valid moves from a 
    //piece at a given coordinate(can be a hypothetical piece)
    let inline private validMoves (piece:Pieces) (pos:Position) (board) (availableMoves:Ply[])  (index:int) : (Ply[]) * int =

        //scanner 1 scans 1 move ahead
        //scanner A scans as far as possible ahead
        let scanner1 = scanMovesGame pos board (pieceColor piece) Both 1 availableMoves

        let scannerA = scanMovesGame pos board (pieceColor piece) Both  (11) availableMoves
        let withcap1 = scanMovesGame pos board (pieceColor piece) Take 1 availableMoves
        let nocap1   = scanMovesGame pos board (pieceColor piece) Free 1 availableMoves
    
        //generally the different directions of the scans 
        //are catted together
        match piece with
        | Knight(color) -> scanner1  1  2 index |> snd
                           |> scanner1 -1  2    |> snd
                           |> scanner1  2  1    |> snd
                           |> scanner1 -2  1    |> snd
                           |> scanner1  1 -2    |> snd
                           |> scanner1  2 -1    |> snd
                           |> scanner1 -1 -2    |> snd
                           |> scanner1 -2 -1
                           
        //get all the special cases allowed if normal
        | Bishop(color) -> nocap1 1 0 index   |> snd
                           |> nocap1 -1 0     |> snd
                           |> nocap1 0 1      |> snd
                           |> nocap1 0 -1     |> snd
                           |> scannerA  1  1  |> snd
                           |> scannerA -1  1  |> snd
                           |> scannerA -1 -1  |> snd
                           |> scannerA  1 -1 
    
        | King(color) ->   scanner1  1  1 index |> snd
                           |> scanner1  -1  1   |> snd
                           |> scanner1 -1 -1    |> snd
                           |> scanner1   1 -1   |> snd
                           |> scanner1  0  1    |> snd
                           |> scanner1   1  0   |> snd
                           |> scanner1  0 -1    |> snd
                           |> scanner1  -1  0 
                           
        | Queen(color) ->  scannerA  1  1 index  |> snd
                           |> scannerA -1  1 |> snd
                           |> scannerA -1 -1 |> snd
                           |> scannerA  1 -1 |> snd
                           |> scannerA  1  0 |> snd
                           |> scannerA  0  1 |> snd
                           |> scannerA -1  0 |> snd
                           |> scannerA  0 -1
     
        //left right and forward, left and right are allowed if capture, 
        //forward is allowed if normal white goes to 0, black goes to N
        | Pawn(color) -> let sign = match color with 
                                    | White -> 1 
                                    | Black -> -1
                         nocap1 (sign) (0) index |> snd
                         |> withcap1 (sign) (-1) |> snd
                         |> withcap1 (sign) (1) 
    
        | Rook(color) ->  scannerA  1  0 index  |> snd
                          |> scannerA  0  1     |> snd
                          |> scannerA -1  0     |> snd
                          |> scannerA  0 -1    

    //gets the available moves of a piece at a position on a 
    //given gameboard
    let movesFrom (input:(Pieces * Position) List) (board) : (Ply []) = 
          //get all of the available moves
          let (retval,endIndex) = List.fold (fun (availableMoves, index)  (piece,pos) -> validMoves piece pos board availableMoves index) ((Array.create 600 ((0,0),(0,0))), (-1)) input 
          retval.[0..endIndex]
