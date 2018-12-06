namespace Celestia

open DepthFirstSearch
open BoardCombinators
open TranspositionTable
open Quiescence
open Primitives 

open System.Diagnostics

module AlphaBeta2 =

    let newwhitehit = ref 0
    let newblackhit = ref 0
    let counter x =
        match x with
        | White -> newblackhit:=!newblackhit+1
        | Black-> newwhitehit:=!newwhitehit+1

    [<Literal>]
    let sortdepth = 5 

    let sortMoves moveslist game depth =
        if depth < sortdepth then moveslist else
        let mutable undoval = None
        let mutable movelist = []
        for i in moveslist do
            let (newgame,newundo)=doUpdate game i
            do undoval <- Some(newundo)
            let addval = match (getTranspose game.ZobristHash -10 game.Turn) with
                         | Some(_,x) -> -x
                         | None -> -newgame.Value
            do undoUpdate game newundo
            do movelist<-(i,addval)::movelist
    
        let res = List.sortBy snd movelist 
        Array.ofList (List.map(fun (a,_) -> a) res)


    
    let AlphaBeta node depth alpha beta =
        let rec searcher currentalpha currentbeta currentnode index currentdepth (moves:Ply [])=
            match index with
            //travel along the breadth
            | y when y >= moves.Length -> currentalpha
            | x  ->  let (newnode,newundo) = doUpdate currentnode moves.[x]
                     //negamax depth
                     let newtest = -(match (currentdepth-1) with 
                                     | x when x <= 0 || newnode.IsTerminal() -> newnode.Value * (50 - newnode.Index)
                                     | x -> (searcher -currentbeta -currentalpha newnode 0 (currentdepth-1)(newnode.AvailableMoves.Force())  ))
                     do undoUpdate newnode newundo
                     //alpha prune
                     let newalpha = max currentalpha newtest
                     //beta prune
                     if newalpha >= currentbeta then newalpha
                     //continue traveling breadthwise
                     else searcher newalpha currentbeta currentnode (x+1) currentdepth (moves)
        searcher alpha beta node 0 depth (node.AvailableMoves.Force())

    let AlphaBetaSearch node depth =
        List.maxBy snd (List.map (fun i ->
                           //do printfn ("%s") (sprintMove i)
                           let (move, reverse) = doUpdate node i
                           let score = if move.IsTerminal() then inf * (50 - move.Index) else -(AlphaBeta move depth (-inf) (inf))
                           //printfn "%i" score
                           do undoUpdate move reverse
                           (i, score)) (Array.toList (node.AvailableMoves.Force())))



    let AlphaBetaTT node depth alpha beta =
        let rec searcher origalpha currentalpha currentbeta currentnode index currentdepth (moves:Ply []) =
             match index with 
             | y when y>= moves.Length -> 
                     if currentalpha > origalpha && currentalpha < currentbeta then 
                                  //in window, perfect hit
                                  do setTranspose currentnode.ZobristHash currentalpha Transpose.Exact currentdepth currentnode.Turn
                                  //since we don't know if we got this or something less than this, this is
                                  //an upperbound
                     elif currentalpha <= origalpha then
                                  do setTranspose currentnode.ZobristHash currentalpha Transpose.Upper currentdepth currentnode.Turn
                     else ()
                     currentalpha
             //travel along the breadth
             | x ->  let (newnode,newundo) = doUpdate currentnode moves.[x]
                     //travel the depth
                     let newtest = -( match currentdepth-1 with 
                                      | x when x <= 0 || newnode.IsTerminal() -> quiesce newnode -currentbeta -currentalpha
                                      | _ -> match (getTranspose newnode.ZobristHash (currentdepth-1) newnode.Turn) with
                                             | Some(Transpose.Exact,x) -> x
                                             | Some(Transpose.Upper,x) when x <= -currentbeta -> x
                                             | Some(Transpose.Lower,x) when x >= -currentalpha -> x
                                             | _ -> searcher -currentbeta -currentbeta -currentalpha newnode 0 (currentdepth-1) (sortMoves (newnode.AvailableMoves.Force()) newnode (currentdepth-1)))
                     //do undo
                     do undoUpdate newnode newundo 
                     //alpha prune
                     let newalpha = max currentalpha newtest
                     //beta prune
                     if newalpha >= currentbeta then
                     //since we didn't continue this is a lower bound
                                 do setTranspose currentnode.ZobristHash newalpha Transpose.Lower currentdepth currentnode.Turn
                                 newalpha
                     //continue traveling breadthwise
                     else searcher origalpha newalpha currentbeta currentnode (x+1) currentdepth moves
                                
        searcher alpha alpha beta node 0 depth (sortMoves (node.AvailableMoves.Force()) node (depth))
