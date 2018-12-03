//Lewis Coates (c) April 7, 2011                                                
namespace MoveGeneration


module BoardHelpers =
    open System
    open TypedInput
    open ZobristKeys
    open BoardConstants

    ///Invalid move exception
    [<NoEquality;NoComparison>]
    exception public InvalidMove of Ply * GameState

    // infinity val, means one side haswon
    [<Literal>]
    let inf = 1000

    let botMove = ((-1,-1),(-1,-1))

    //return the color
    let inline pieceColor piece = 
                 match piece with
                 | King(x) | Queen(x)| Knight(x) 
                 | Rook(x) | Pawn(x) | Bishop(x) -> x

    //inverts the given color(i.e. gives the color of opponent)
    let inline notColor (color:Color) : Color = 
                  match color with
                  | Black -> White
                  | White -> Black

    
    //gets the pieces on the board of the associated color
    let piecesOfGame (board:Board) (color:Color) : (Pieces * Position) list =
        Array.fold (fun acc (x,y) -> match board.[x,y] with 
                                     | Some(p) when pieceColor p = color -> (p,(x,y))::acc
                                     | _ -> acc) [] allPositions

    //takes in a 2 dimensional array and turns it into a seq<seq<'a>>
    let arrayAsSeq (input: 'a [,]) =
       (seq { for i in (input.GetLowerBound 0) .. (input.GetUpperBound 0) do
              yield seq { for j in (input.GetLowerBound 1) .. 
                                                    (input.GetUpperBound 1) do
                                yield input.[i,j] }})
        
    //prints out the board state
    let sprintBoard (board:GameState) =
                    //no piece is represented by a .
                  let printer retstr piece = 
                        match piece with
                        |Some(p)->(p.ToString()+retstr) 
                        | None -> "."+retstr
                  //fold the outer
                  let output=
                    List.fold 
                        //take in 1 line of the outer and fold it into a string
                        (fun retstr list -> 
                            (List.fold 
                                   //take in a line of the innter and fold it
                                   (fun retstr piece -> printer retstr piece)
                                            //endof line and initial conditions
                                            "" (List.ofSeq list))+"\n"+retstr)
                                           //initial conditions and intput
                                            "" (List.ofSeq (arrayAsSeq board.BoardState))
                  sprintf "%s turn:%d\n" output board.Index


    //pretty print for moves
    let sprintMove ((from1,from2),(to1,to2)) =
        let chars = [|'e';'d';'c';'b';'a'|]
        (sprintf "%c%d-%c%d"  (chars.[from2]) (from1+1) (chars.[to2]) (to1+1))

     //prints the ascii character representing the color, IMCS compliant
    let printColor col =
        match col with
        | White -> "W"
        | Black -> "B"

    //pretty print for moves, prints to console
    let printMove x =
        printf "%s" (sprintMove x)

    //returns whether or not the tested game is terminal
    let isTerminal input =
        if input.IsPlaying && input.Index <= input.TimeOut then false
        else true
