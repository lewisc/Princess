namespace Searching
open System.Diagnostics
open MoveGeneration
open BoardCombinators
open BoardHelpers

exception SearchException of string

module DepthFirstSearch =
    let counter = ref 0


    let doUndoDfs driver depth game side =
        let movelist = game.AvailableMoves.Force()
        let maxmove = movelist.Length
        let rec searcher depth side undoer (move,score) index=
            match index with
            | x -> 
                let head = movelist.[x]
                let (newgame,newundo) = doUpdate game head
                let (_,newtest) =  driver (depth-1) newgame -side
                do undoUpdate game newundo
                let testval = -newtest
                searcher depth side (Some(newundo)) (if testval > (score) then (head,testval) else (move,score)) (index+1)

            | y when y >= maxmove  -> (move,score)

        let ret = searcher depth side None (botMove,-inf-1000) 0
        ret

    let onPlay game = match game.Turn with 
                             | Black -> -1
                             | White -> 1

    ///naive depth first search using depth limiter
    let DepthFirstSearchFrame maxer (depth:int) (game:GameState) : (Ply * Score) =
        let myTurn = onPlay game

        let rec searcher d game side =
            match d with
            //terminal Node
            | x when x = 0 || (isTerminal game) -> 
                                    let movescore = game.Value
                                    (botMove,(movescore * side))
            //the max of the child moves, each child move gets mapped to 
            //it's associated score
            | _ -> maxer searcher d game side

        let ret = searcher depth game myTurn
        ret

                            

    ///a timed version of the naive dfs, frameworked to have swappable
    ///underlying behavior
    let IterativeDeepenerFrame maxer time  (game:GameState) : Ply * Score =
        let myturn = onPlay game
        let timer = Stopwatch.StartNew()

        // seacher, has a time check
        let rec searcher depth game side : (Ply * Score) =
            if timer.ElapsedMilliseconds >= time 
            then (botMove,-inf  ) else

            match depth with
            | x when x = 0 || (isTerminal game) -> 
                            let movescore = game.Value
                            (botMove,(movescore * side))
            | _ -> maxer searcher depth game side

        //driver, keeps going until the alarm rings
        let rec driver currdepth currbestmove =
            let newmove = searcher currdepth game myturn

            //if we found a win, return it, if we found a lose, return our next best move
            if timer.ElapsedMilliseconds >= time then currbestmove 
            elif (snd newmove) >= (inf/2) then newmove
            elif (snd newmove) <= (-inf/2) then currbestmove
            else driver (currdepth+2) newmove 
        driver 2 ((game.AvailableMoves.Force()).[0],-1)

    let IterativeDeepenerDoUndo = IterativeDeepenerFrame doUndoDfs
    let DepthFirstSearchdoUndo = DepthFirstSearchFrame doUndoDfs
