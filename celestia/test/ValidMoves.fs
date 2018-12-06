//Lewis Coates (c) April 7, 2011                                                
namespace Celestia
open System

open Primitives
open TypedInput
open ZobristKeys
open MoveCalculation


module BoardCombinators =

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


    //TODO:Move this out
    //tests whether a game executes and completes
    let testGame (totest:Variation) : bool =
                try
                    let complete = 
                         List.fold (fun (board:GameState) (play) -> 
                             checkedUpdate board play) (initialState (fun  _ _-> 
                                        (0,(defaultIncrementor))) (fun _ -> (0,(defaultIncrementor)))) (totest)

        //            do printfn "%s" (sprintBoard complete )
                    complete.IsTerminal()
                with
                |  InvalidMove(x,y) -> do printfn "%s" (x.ToString())
                                       do printfn "\nFailed to apply the above move to:\n"
                                       do printfn "%s" (y.ToString())
                                       do printfn "\n"
                                       false
    
    //tests whether a game executes, disables checks so runs much faster
    let perfTest (totest:Variation) : bool =
                    let complete = List.fold  (fun (board:GameState) play -> 
                                    fst(doUpdate board play)) (initialState (fun _ _-> 
                                            (0,(defaultIncrementor))) (fun _ -> (0,(defaultIncrementor)))) totest 
                    complete.IsTerminal()
    
    //trys to play the action on the board, if it fails it returns
    //the unmodified board and a value of false
    //TODO:
    //This is done for input validation and can probably be moved
    let tryPlay (board:GameState) action =
        match (action, not (board.IsTerminal())) with
        | ReadInput(z), true-> 
                            if Array.exists (fun x -> x = z) (board.AvailableMoves.Force())
                            then (true, fst (doUpdate board z))
                            else (false, board)
        | _  -> (false, board)
