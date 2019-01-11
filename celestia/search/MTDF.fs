namespace Celestia

open TranspositionTable
open System.Diagnostics
open Primitives
open GameState
open AlphaBeta
open DepthFirstSearch

module MTDF =

    //a basic fixed depth MTD(f) algorithm
    let MTDF d (game:GameState) =

        //figure out what side we are playing as, used for
        //scoring purposes(best guess)
        let side = match game.Turn with
                   | White -> 1
                   | Black -> -1

        //the search while loop
        let rec searcher root g up low =
            //update beta cutoff, increment by atomic value
            let beta = if g = low then g+1 else g

            //zerowindow t table AB search(d-1 for unrolled root node)
            let newg = AlphaBetaTT root (d-1) (beta-1) beta 
            //either we found a new high or we found a new low
            let newlow = if not (newg < beta) then newg else low
            let newhigh = if newg < beta then newg else up

            //if our window is valid, we have a good value, if not
            //keep searching
            if not (newlow >= newhigh)
            then searcher root newg newhigh newlow
            //negamax requires degation
            else newg

        //a map function that is used to build best guesses
        //search on those best guesses, and then return the
        //results
        let mapper r x =
            let score = r.DoUpdate(x)
            let guess = match (getTranspose game.ZobristHash 0 game.Turn) with
                        | Some(_,x) -> x
                        | _ -> game.Value 

            let ret = searcher newgame guess Inf (-Inf)
            do r.UndoUpdate()
            (x,ret)
        //get the results
        let retval = Array.map (mapper game) (game.AvailableMoves.Force())
        //and get the correct value
        Array.maxBy snd retval

    //MTD(f) in an iterative deepening framework
    //Note, this is not particularly atomic, can runover by potentially a second or
    //two(not sure if this is a problem yet
    let MTDFID time (game:GameState) =
        //timer for ID
        let timer = Stopwatch.StartNew()
        assert((game.IsPlaying) = true)
        //main loop, identical to above except as noted
        let rec searcher depth root g up low =
            //if time is up, get out and return invalid
            if timer.ElapsedMilliseconds >= time
            then -Inf - 1000
            else
            let beta = if g = low then g+10 else g

            let (newg) = AlphaBetaTT root (depth) (beta-10) beta
            let newlow = if not (newg < beta) then newg else low
            let newhigh = if newg < beta then newg else up
            if not (newlow >= newhigh)
            then searcher depth root newg newhigh newlow
            else newg

        //keep going deeper until the alarm goes off, incrementing depth by 2
        //for horizon effect minimization(a huge deal since we are feeding our
        //results forward as best guesses)
        let rec deepener currdepth currbestmove bestguesslist =

            //unroll the top node
            let mutable curralpha = -Inf
            let (movelist:(Ply*int) option []) = Array.create 600 None
            let mutable index = 0
            let mutable bestmove = (botMove, -Inf - 1000)

            for l in bestguesslist do
                let appmove = fst l
                let appscore = snd l
                let score = game.DoUpdate(appmove)
                let newalpha = -(searcher (currdepth-1) newnode -appscore -curralpha -Inf)
                do movelist.[index] <- Some(appmove,newalpha)
                do index <- index + 1
                do curralpha <- newalpha
                if newalpha > (snd bestmove) then
                    do bestmove <- (appmove,newalpha)
                else ()
                do game.UndoUpdate()
                       
            //if we found a win, return it, if we founda lose, return our next best move
            //note, there's an extremely unlikely bug here
            if timer.ElapsedMilliseconds >= time then 
                    do printfn "searchdepth %d" (currdepth-2)
                    currbestmove
            elif (snd bestmove) >= (Inf / 2) then bestmove
            elif (snd bestmove) <= (-Inf / 2) then currbestmove
            else deepener (currdepth+2) bestmove (Array.sortBy snd (Array.choose (fun x -> x) movelist))

        //run the deepener, start at 0 to get good first guesses, build an initial value with
        //0's which are basically discarded since the depth 0 search just gets the moves score
        deepener 0 (botMove,(-Inf - 1000)) (Array.map (fun x -> x,0) (game.AvailableMoves.Force()))
            
