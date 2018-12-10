//Lewis Coates (c) April 7, 2011                                                
namespace Celestia


open System

open Primitives
open ZobristHash

module MoveGeneration =

    ///Types of ways that a square can be captured
    type private PieceCaptures =
        | Free
        | Take
        | Both


    //the different things that can happen with a move.
    //invalid represents a move such as trying to capture your own piece
    let private (|Capture|Invalid|NoCapture|) ((x : int),
                                               (y : int),
                                               (board : Board),
                                               (hue : Color),
                                               (attackType : PieceCaptures))  =
 
        if (x > MaxXVal || x < 0 || y > MaxYVal || y < 0)
        //if the move is off the board
        then
            Invalid
        else 
            match (board.[x, y], attackType) with
            //the case where the desitnation is empty
            | None, Free
            | None, Both -> NoCapture
            | Some(z), Take
            | Some(z), Both -> match (z.Color, hue) with 
                               //can't capture own piece
                               | (Black, Black) 
                               | (White, White) -> Invalid
                               | (Black, White) 
                               | (White, Black) -> Capture
            | None, Take
            | Some(_), Free -> Invalid

    //helper function that returns a list of 
    //possible moves along a dx/dy move allowance.
    //this is the main engine
    let inline private scanMovesGame ((x, y) : Position)
                                     (board : Board)
                                     (hue : Color)
                                     (attackType : PieceCaptures)
                                     (endcount : int)
                                     (dx : int)
                                     (dy : int) : Ply list =

        //march through the possibilities
        let rec scanloop count agg = 
            if endcount < count 
            then
                agg
            else

                let xval = x + count * dx
                let yval = y + count * dy
                //march through the possibilities
                match (xval, yval, board, hue, attackType) with
                | Invalid -> agg
                | NoCapture -> let newAgg = ((x, y), (xval, yval)) :: agg
                               scanloop (count + 1) newAgg
                | Capture -> (((x, y), (xval, yval)) :: agg)

        scanloop 1 []
      
    //gets a list of all valid moves from a 
    //piece at a given coordinate(can be a hypothetical piece)
    let inline private validMoves (piece : Pieces)
                                  (pos : Position)
                                  (board : Board) : (Ply list) =

        //scanner 1 scans 1 move ahead
        //scanner A scans as far as possible ahead
        let scanner1 = scanMovesGame pos board piece.Color Both 1
        let scannerA = scanMovesGame pos board piece.Color Both 11
        let withcap1 = scanMovesGame pos board piece.Color Take 1
        let nocap1   = scanMovesGame pos board piece.Color Free 1
    
        //generally the different directions of the scans 
        //are catted together
        match piece with
        | Knight(color) -> scanner1 1 2
                           @ scanner1 -1  2
                           @ scanner1  2  1
                           @ scanner1 -2  1
                           @ scanner1  1 -2
                           @ scanner1  2 -1
                           @ scanner1 -1 -2
                           @ scanner1 -2 -1
                           
        //get all the special cases allowed if normal
        | Bishop(color) -> nocap1 1 0
                           @ nocap1 -1 0   
                           @ nocap1 0 1    
                           @ nocap1 0 -1   
                           @ scannerA  1  1
                           @ scannerA -1  1
                           @ scannerA -1 -1
                           @ scannerA  1 -1
    
        | King(color) ->   scanner1 1 1
                           @ scanner1 -1  1
                           @ scanner1 -1 -1
                           @ scanner1  1 -1
                           @ scanner1  0  1
                           @ scanner1  1  0
                           @ scanner1  0 -1
                           @ scanner1 -1  0 
                           
        | Queen(color) ->  scannerA 1 1
                           @ scannerA -1  1
                           @ scannerA -1 -1
                           @ scannerA  1 -1
                           @ scannerA  1  0
                           @ scannerA  0  1
                           @ scannerA -1  0
                           @ scannerA  0 -1
     
        //left right and forward, left and right are allowed if capture, 
        //forward is allowed if normal white goes to 0, black goes to N
        | Pawn(color) -> let sign = match color with 
                                    | White -> 1 
                                    | Black -> -1
                         nocap1 (sign) 0
                         @ withcap1 (sign) (-1)
                         @ withcap1 (sign) (1) 
    
        | Rook(color) -> scannerA 1 0
                         @ scannerA 0 1
                         @ scannerA -1 0
                         @ scannerA 0 -1   

    //gets the available moves of a piece at a position on a 
    //given gameboard
    let movesFrom (input : PieceLoc list) (board : Board) : (Ply list) = 
        //get all of the available moves
        List.map (fun (piece, pos) -> validMoves piece pos board) input 
        |> List.concat

