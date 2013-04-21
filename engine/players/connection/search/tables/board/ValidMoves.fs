//Lewis Coates (c) April 7, 2011                                                
namespace MoveGeneration
open System
open TypedInput
open ZobristKeys
open BoardConstants

//Invalid move exception
[<NoEquality;NoComparison>]
exception public InvalidMove of Ply * GameState

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

module BoardCombinators=
    let initialIncrementor () = {WhiteScore = 0;
                                 BlackScore = 0;
                                 Advancement=0;
                                 BlackPawnScore=0;
                                 WhitePawnScore=0;}
    //return the color
    let pieceColor piece = 
                 match piece with
                 | King(x) | Queen(x)| Knight(x) 
                 | Rook(x) | Pawn(x) | Bishop(x) -> x

    //inverts the given color(i.e. gives the color of opponent)
    let notColor (color:Color) : Color = 
                  match color with
                  | Black -> White
                  | White -> Black

    //represents a default board
    let private defaultBoard  = 
     array2D [|
               [|Some(King(White)); Some(Queen(White));Some(Bishop(White)); Some(Knight(White));Some(Rook(White))|];
               [|Some(Pawn(White)) ;Some(Pawn(White));Some(Pawn(White)); Some(Pawn(White));Some(Pawn(White))|];
               [|None ;None ;None;None;None|];
               [|None ;None ;None;None;None|];
               [|Some(Pawn(Black));Some(Pawn(Black));Some(Pawn(Black)); Some(Pawn(Black));Some(Pawn(Black))|];
               [|Some(Rook(Black));Some(Knight(Black));Some(Bishop(Black)); Some(Queen(Black)); Some(King(Black))|];|]

    
    //gets the pieces on the board of the associated color
    let piecesOfGame (board:Pieces option [,]) color =
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
                  List.fold 
                    //take in 1 line of the outer and fold it into a string
                    (fun retstr list -> 
                        (List.fold 
                               //take in a line of the innter and fold it
                               (fun retstr piece -> printer retstr piece)
                                        //endof line and initial conditions
                                        "" (List.ofSeq list))+"\r\n"+retstr)
                            //initial conditions and intput
                            "" (List.ofSeq (arrayAsSeq board.BoardState))

    //returns the possibilities for a move(capture, invalid, normal).
    //Normal moves can be continued, invalid/captures can't
    let private testValidMove (x:int) (y:int) (board:Pieces option[,]) (hue:Color)  : MoveType =
                if (x > maxXVal  || 
                    x < 0  || 
                    y > maxYVal  || 
                    y < 0)
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
    let private scanMovesGame ((x:int),(y:int)) (board) (hue:Color) (attacktype:PieceCaptures) (endcount:int ) (dx:int) (dy:int) : Ply List =
                //march through the possibilities
                let rec scanloop xnew ynew count retlist = 
                    let newx = xnew+dx
                    let newy = ynew+dy
                    if endcount < count  then retlist else
                    //march through the possibilities
                    match (testValidMove newx newy board hue) with
                    | Invalid -> retlist
                    | Normal  -> match attacktype with
                                 | Take -> scanloop newx newy (count+1) retlist
                                 | Both | Free ->  
                                     scanloop newx newy (count+1) 
                                                (((x,y),(newx,newy))::retlist)
                    | Capture(_) -> 
                                match attacktype with
                                | Free -> retlist
                                | Both | Take -> ((x,y),(newx,newy))::retlist
                scanloop x y 1 []
      
    //gets a list of all valid moves from a 
    //piece at a given coordinate(can be a hypothetical piece)
    let validMoves (piece:Pieces) (pos:Position) (board) : Ply list =

        //scanner 1 scans 1 move ahead
        //scanner A scans as far as possible ahead
        let scanner1 = scanMovesGame pos board (pieceColor piece) Both 1

        let scannerA = scanMovesGame pos board (pieceColor piece) Both  (11)
        let withcap1 = scanMovesGame pos board (pieceColor piece) Take 1
        let nocap1 = scanMovesGame pos board (pieceColor piece) Free 1
    
        //generally the different directions of the scans 
        //are catted together
        match piece with
        | Knight(color) -> scanner1  1  2 @ scanner1 -1  2 @ 
                           scanner1  2  1 @ scanner1 -2  1 @
                           scanner1  1 -2 @ scanner1  2 -1 @
                           scanner1 -1 -2 @ scanner1 -2 -1
                           
        //get all the special cases allowed if normal
        | Bishop(color) -> nocap1 1 0 @ nocap1 -1 0 @
                           nocap1 0 1 @ nocap1 0 -1 @
                           scannerA  1  1 @ scannerA -1  1 @
                           scannerA -1 -1 @ scannerA  1 -1 
    
        | King(color) ->   scanner1  1  1 @ scanner1  -1  1 @
                           scanner1 -1 -1 @ scanner1   1 -1 @
                           scanner1  0  1 @ scanner1   1  0 @
                           scanner1  0 -1 @ scanner1  -1  0 
                           
        | Queen(color) ->  scannerA  1  1 @ scannerA -1  1 @
                           scannerA -1 -1 @ scannerA  1 -1 @
                           scannerA  1  0 @ scannerA  0  1 @
                           scannerA -1  0 @ scannerA  0 -1
     
        //left right and forward, left and right are allowed if capture, 
        //forward is allowed if normal white goes to 0, black goes to N
        | Pawn(color) -> let sign = match color with 
                                    | White -> 1 
                                    | Black -> -1
                         nocap1 (sign) (0) @ 
                         withcap1 (sign) (-1) @
                         withcap1 (sign) (1) 
    
        | Rook(color) ->  scannerA  1  0 @ scannerA  0  1 @
                          scannerA -1  0 @ scannerA  0 -1
    //gets the available moves of a piece at a position on a 
    //given gameboard
    let movesFrom (input:(Pieces * Position) List) (board) = 
          //get all of the available moves
          List.map (fun (piece,pos) -> 
                        validMoves piece pos board) input
         |> List.fold (fun agg input -> agg@input) []
    
    //an unchecked update function, applies the move to the gamestate
    let update input ((startx,starty), (endx,endy))  : GameState=

        let nextturn = notColor input.Turn
        let capturepiece = input.BoardState.[endx,endy]
        let initialpiece = input.BoardState.[startx,starty]

        let willplay = match capturepiece with
                       //king is captured, game over
                       | Some(King(_)) -> false
                       //carry on
                       | _ -> true

        //make a new board, assign the correct piece to the 
        //endpoint clear the beginning piece, determine whether
        //the game is continueing return a new board of the
        //appropriate type
        let returnfunc newpiece =

            let newboard = Array2D.copy input.BoardState

            newboard.[endx,endy]<-Some(newpiece)
            newboard.[startx,starty]<-None

            let x1 = (None,(startx,starty))
            let x2 = (Some(newpiece),(endx,endy))
            let x3 = (capturepiece, (endx,endy))
            let x4 = (initialpiece,(startx,starty))
            let newHash = input.ZobristHash 
                      ^^^ (zobristMap.[x1])
                      ^^^ (zobristMap.[x2])
                      ^^^ (zobristMap.[x3])
                      ^^^ (zobristMap.[x4])
            assert(newHash = zobristAdder newboard)

            let removeStart (_,x) = not (x = (startx,starty))
            let removeEnd (_,x) = not (x = (endx,endy))

            let (blackPieces,whitePieces) = 
                 match nextturn with
                 | Black -> (List.filter removeEnd input.BlackPieces, 
                             (newpiece,(endx,endy))
                          :: List.filter removeStart input.WhitePieces)
                 | White -> ((newpiece,(endx,endy))
                         :: List.filter removeStart input.BlackPieces, 
                                        List.filter removeEnd input.WhitePieces)

            let newmoves = match nextturn with
                                   | Black -> blackPieces
                                   | White -> whitePieces
            let newgame =  
                   {Turn=nextturn;
                    BoardState = newboard;
                    TimeOut=input.TimeOut;
                    IsPlaying = willplay;
                    Index=input.Index+1;
                    BlackPieces=(blackPieces);
                    WhitePieces=(whitePieces);
                    AvailableMoves=
                         lazy(movesFrom newmoves newboard);
                    ZobristHash=newHash;
                    State=input.State;
                    Value=input.Value;
                    EvalFunc=input.EvalFunc}
            let updater = {OldPiece =(match initialpiece with 
                                      |Some(t) ->t 
                                      |_ ->raise <|Hell("invalid initialpiece"));
                           NewPiece = newpiece;
                           Taken=capturepiece;
                           Move=((startx,starty),(endx,endy));}

            let (newval,newinc) = 
                         input.EvalFunc updater newgame
            newgame.Value<-newval
            newgame.State<-newinc
            newgame

        match endx, input.BoardState.[startx,starty] with
        //white goes to 0, black goes to N
        //These cases handle the pawn Promotion
        | 0, Some(Pawn(Black)) -> returnfunc (Queen(Black))
        | _, Some(Pawn(White)) when endx = 5 -> returnfunc (Queen(White))
        | _, Some(other) ->  returnfunc other
        | _, None ->  raise <| InvalidMove(((startx,starty),(endx,endy)),input)

    let undoUpdate input prevstate=
            do input.AvailableMoves<- prevstate.OldMoves
            do input.WhitePieces<- prevstate.OldWhite
            do input.BlackPieces<- prevstate.OldBlack
            do input.ZobristHash<- prevstate.OldHash
            do input.State<- prevstate.OldState
            do input.Value<- prevstate.OldValue
            do input.Turn <- notColor (input.Turn)
            do input.IsPlaying <- true
            do input.Index <- input.Index-1

            let (piece1,(oldsx,oldsy))= prevstate.OldPrevMove1
            let (piece2,(oldex,oldey))= prevstate.OldPrevMove2

            do input.BoardState.[oldsx,oldsy]<- piece1
            do input.BoardState.[oldex,oldey]<- piece2

    //an unchecked update function, applies the move to the gamestate
    let doUpdate input ((startx,starty), (endx,endy))=

        let nextturn = notColor input.Turn
        let capturepiece = input.BoardState.[endx,endy]
        let initialpiece = input.BoardState.[startx,starty]

        let willplay = match capturepiece with
                       //king is captured, game over
                       | Some(King(_)) -> false
                       //carry on
                       | _ -> true


        let newoldstate =
                    {OldMoves=input.AvailableMoves;
                     OldWhite=input.WhitePieces;
                     OldBlack=input.BlackPieces;
                     OldHash=input.ZobristHash;
                     OldState=input.State;
                     OldValue=input.Value;
                     OldPrevMove1=(input.BoardState.[startx,starty],(startx,starty));
                     OldPrevMove2=(input.BoardState.[endx,endy],(endx,endy));
                     }

        //make a new board, assign the correct piece to the 
        //endpoint clear the beginning piece, determine whether
        //the game is continueing return a new board of the
        //appropriate type
        let returnfunc newpiece =

            input.BoardState.[endx,endy]<-Some(newpiece)
            input.BoardState.[startx,starty]<-None

            let removeStart (_,x) = not (x = (startx,starty))
            let removeEnd (_,x) = not (x = (endx,endy))

            let (blackPieces,whitePieces) = 
                 match nextturn with
                 | Black -> (List.filter removeEnd input.BlackPieces, 
                             (newpiece,(endx,endy))
                          :: List.filter removeStart input.WhitePieces)
                 | White -> ((newpiece,(endx,endy))
                         :: List.filter removeStart input.BlackPieces, 
                                        List.filter removeEnd input.WhitePieces)

            let newmoves = match nextturn with
                                   | Black -> blackPieces
                                   | White -> whitePieces
            let x1 = (None,(startx,starty))
            let x2 = (Some(newpiece),(endx,endy))
            let x3 = (capturepiece, (endx,endy))
            let x4 = (initialpiece,(startx,starty))
            let newHash = input.ZobristHash 
                      ^^^ (zobristMap.[x1])
                      ^^^ (zobristMap.[x2])
                      ^^^ (zobristMap.[x3])
                      ^^^ (zobristMap.[x4])
            assert(newHash = zobristAdder input.BoardState)


            do input.Turn<-nextturn;
            do input.IsPlaying<- willplay;
            do input.Index<-input.Index+1;
            do input.BlackPieces<-(blackPieces);
            do input.WhitePieces<-(whitePieces);
            do input.AvailableMoves<-
                 lazy(movesFrom newmoves input.BoardState)
            do input.ZobristHash<-newHash;
            let updater = {OldPiece =(match initialpiece with 
                                      |Some(t) ->t 
                                      |_ ->raise <|Hell("invalid initialpiece"));
                           NewPiece = newpiece;
                           Taken=capturepiece;
                           Move=((startx,starty),(endx,endy));}
            let (newval,newinc) =  
                         input.EvalFunc updater input
            input.Value<-newval
            input.State<-newinc
            (input,newoldstate)

        assert(input.BoardState.[startx,starty] <> None)
        match endx, input.BoardState.[startx,starty] with
        //white goes to 0, black goes to N
        //These cases handle the pawn Promotion
        | 0, Some(Pawn(Black)) -> returnfunc (Queen(Black))
        | _, Some(Pawn(White)) when endx = 5 -> returnfunc (Queen(White))
        | _, Some(other) ->  returnfunc other
        | _, None ->  raise <| InvalidMove(((startx,starty),(endx,endy)),input)

    //updates the array of options. Note this is immutable so a copy is 
    //made this makes this function quite expensive
    let checkedUpdate input ((startx,starty), (endx,endy))  : GameState =  
        if (List.exists (fun ((sx, sy), (ex,ey)) -> 
             (sx=startx && 
              ex=endx   &&
              sy=starty &&
              ey=endy)) (input.AvailableMoves.Force())) = false
        then raise <| InvalidMove(((startx,starty),(endx,endy)),input)
        else update input ((startx,starty), (endx,endy))
    
    
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

    //constructor for an initial state game
    let initialState fitness start = 
        let pieces =  piecesOfGame defaultBoard White 
        let newgame = 
           {Turn=White;
            BoardState=(Array2D.copy defaultBoard);
            TimeOut=81;
            IsPlaying=true;
            Index=0;
            BlackPieces=(piecesOfGame defaultBoard Black);
            WhitePieces=pieces;
            AvailableMoves=lazy(movesFrom pieces defaultBoard);
            ZobristHash=zobristAdder defaultBoard;
            EvalFunc=fitness;
            Value=0;
            State=initialIncrementor();
            }
        let (initval,initinc)= start newgame
        do newgame.Value<-initval
        do newgame.State<-initinc
        newgame

    

    //returns whether or not the tested game is terminal
    let isTerminal input =
        if input.IsPlaying && input.Index <= input.TimeOut then false
        else true

    //trys to play the action on the board, if it fails it returns
    //the unmodified board and a value of false
    let tryPlay (board:GameState) action =
        match (action, not (isTerminal board)) with
        | ReadInput(z), true-> 
                 if List.exists (fun x -> x = z) (board.AvailableMoves.Force())
                 then (true, update board z)
                 else (false, board)
        | _  -> (false, board)
    
    //tests whether a game executes and completes
    let testGame (totest:Variation) : bool =
                try
                    let complete = 
                         List.fold (fun (board:GameState) (play) -> 
                             checkedUpdate board play) (initialState (fun  _ _-> 
                                        (0,(initialIncrementor()))) (fun _ -> (0,(initialIncrementor())))) (totest)
                    isTerminal complete
                with
                |  InvalidMove(x,y) -> printMove x
                                       false
    
    //tests whether a game executes, disables checks so runs much faster
    let perfTest (totest:Variation) : bool =
                    let complete = List.fold  (fun (board:GameState) play -> 
                                    update board play) (initialState (fun _ _-> 
                                            (0,(initialIncrementor()))) (fun _ -> (0,(initialIncrementor())))) totest 
                    isTerminal complete
