//Lewis Coates (c) April 7, 2011                                                
namespace MoveGeneration
open System
open TypedInput
open ZobristKeys
open BoardConstants
open BoardHelpers
open MoveCalculation


module BoardCombinators=

    ///undoes an update given an input
    let inline undoUpdate input prevstate=
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
    //no copy of gamestate
    let inline doUpdate input ((startx,starty), (endx,endy))=

        let nextturn = notColor input.Turn
        let capturepiece = input.BoardState.[endx,endy]
        let initialpiece = input.BoardState.[startx,starty]

        let willplay = match capturepiece with
                       //king is captured, game over
                       | Some(King(_)) -> false
                       //carry on
                       | _ -> true


        //Todo: Make this mutable
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
        let inline returnfunc newpiece =

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
            do input.AvailableMoves<- lazy(movesFrom newmoves input.BoardState)
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
        if (Array.exists (fun ((sx, sy), (ex,ey)) -> 
             (sx=startx && 
              ex=endx   &&
              sy=starty &&
              ey=endy)) ((input.AvailableMoves.Force()))) = false
        then raise <| InvalidMove(((startx,starty),(endx,endy)),input)
        else fst (doUpdate input ((startx,starty), (endx,endy)))

    //TODO:Get rid of this
    //constructor for an initial state game
    let initialState fitness start = 
        let pieces =  piecesOfGame (defaultBoard ()) White 
        let newgame = 
           {Turn=White;
            BoardState=(defaultBoard ());
            TimeOut=81;
            IsPlaying=true;
            Index=0;
            BlackPieces=(piecesOfGame (defaultBoard ()) Black);
            WhitePieces=pieces;
            AvailableMoves=lazy(movesFrom pieces (defaultBoard ()));
            ZobristHash=zobristAdder (defaultBoard ());
            EvalFunc=fitness;
            Value=0;
            State= new Incrementor();
            }
        let (initval,initinc)= start newgame
        do newgame.Value<-initval
        do newgame.State<-initinc
        newgame

    //TODO:Move this out
    //tests whether a game executes and completes
    let testGame (totest:Variation) : bool =
                try
                    let complete = 
                         List.fold (fun (board:GameState) (play) -> 
                             checkedUpdate board play) (initialState (fun  _ _-> 
                                        (0,(new Incrementor()))) (fun _ -> (0,(new Incrementor())))) (totest)
                    isTerminal complete
                with
                |  InvalidMove(x,y) -> printMove x
                                       false
    
    //tests whether a game executes, disables checks so runs much faster
    let perfTest (totest:Variation) : bool =
                    let complete = List.fold  (fun (board:GameState) play -> 
                                    fst(doUpdate board play)) (initialState (fun _ _-> 
                                            (0,(new Incrementor()))) (fun _ -> (0,(new Incrementor())))) totest 
                    isTerminal complete
    
    //trys to play the action on the board, if it fails it returns
    //the unmodified board and a value of false
    //TODO:
    //This is done for input validation and can probably be moved
    let tryPlay (board:GameState) action =
        match (action, not (isTerminal board)) with
        | ReadInput(z), true-> 
                 if Array.exists (fun x -> x = z) (board.AvailableMoves.Force())
                 then (true, fst (doUpdate board z))
                 else (false, board)
        | _  -> (false, board)
