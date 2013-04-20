namespace Searching
open DepthFirstSearch
open MoveGeneration
open BoardCombinators
open Books
open TranspositionTable
open Quiescence
open AlphaBeta2
open System.Diagnostics

module AlphaBetaID =
    let experimentalAB time node alpha beta =
        let timer = Stopwatch.StartNew()
        let rec searcher currentalpha currentbeta currentnode movelist currentdepth =
            let rec traverser movelist newalpha =
                match movelist with
                | head::tail -> let (newnode,newundo) = doUpdate currentnode head
                                //negamax depth
                                let newtest = -(searcher -currentbeta -newalpha newnode (newnode.AvailableMoves) (currentdepth-1))
                                do undoUpdate currentnode newundo
                                //alpha prune
                                let localalpha = max newalpha newtest
                                //beta prune
                                if localalpha >= currentbeta then localalpha
                                //continue traveling breadthwise
                                elif timer.ElapsedMilliseconds > time then -inf-1000
                                else traverser tail localalpha
                | [] -> newalpha
            match currentdepth with
            | x when x <= 0 || isTerminal currentnode -> quiesce currentnode alpha beta
            | _ -> traverser (currentnode.AvailableMoves.Force()) currentalpha

        let rec deepener appdepth currbestmove moves =

            let mutable curralpha = -inf
            let mutable movelist = []
            let mutable bestmove = (botMove,-inf-1000)

            for i in moves do
                let newnode = update node (fst i)
                let newalpha = -(searcher -inf -curralpha newnode (newnode.AvailableMoves) (appdepth-1))
                do movelist <- ((fst i),newalpha)::movelist
                do curralpha <- newalpha
                if newalpha > (snd bestmove) then
                    do bestmove <- ((fst i),newalpha)
                else ()

            if timer.ElapsedMilliseconds >= time then 
                    do printfn "searchdepth %d" (appdepth-2)
                    currbestmove
            //if we hit an endgame
            elif (snd bestmove) >= inf/2 then bestmove
            elif (snd bestmove) < -inf/2 then currbestmove
            //otherwise keep going by 2
            else deepener (appdepth+2) bestmove (List.sortBy (fun (x,y) -> -y) movelist)
        deepener 0 (botMove,-inf-1000) (List.map (fun x -> (x,node.Value)) (node.AvailableMoves.Force()))
            
    let AlphaBetaDFSID node  time alpha beta =
        let timer = Stopwatch.StartNew()
        let rec searcher currentalpha currentbeta currentnode movelist currentdepth =
            match movelist with
            //travel along the breadth
            | head::tail ->  
                             let (newnode,newundo) = doUpdate currentnode head
                             //negamax depth
                             let newtest = -(match (currentdepth-1) with 
                                             | x when x <= 0 || isTerminal newnode-> newnode.Value
                                             | _ -> searcher -currentbeta -currentalpha newnode (newnode.AvailableMoves.Force()) (currentdepth-1))
                             do undoUpdate currentnode newundo
                             //alpha prune
                             let newalpha = max currentalpha newtest
                             //beta prune
                             if newalpha >= currentbeta then newalpha
                             //continue traveling breadthwise
                             elif timer.ElapsedMilliseconds > time then -inf-1000
                             else searcher newalpha currentbeta currentnode tail currentdepth
            | [] -> currentalpha
        //iterative deepening
        let rec deepener appdepth currbestmove =

            let newmove = List.map (fun (y) ->let blah = update node y in (y, -(searcher alpha beta blah (blah.AvailableMoves.Force()) (appdepth-1)))) (node.AvailableMoves.Force())
                          |> List.maxBy snd
            if timer.ElapsedMilliseconds >= time then 
                    do printfn "searchdepth %d" (appdepth-2)
                    currbestmove
            //if we hit an endgame
            elif (snd newmove) >= inf/2 then newmove
            elif (snd newmove) < -inf/2 then currbestmove
            //otherwise keep going by 2
            else deepener (appdepth+2) newmove
        deepener 0 (botMove,-inf-1000)

    let AlphaBetaDFSTT node time alpha beta =
        let timer = Stopwatch.StartNew()
        let moves = sortMoves (node.AvailableMoves.Force()) node 10
        let rec searcher origalpha currentalpha currentbeta currentnode movelist currentdepth =
            match movelist with 
            //travel along the breadth
            | head::tail ->  let (newnode,newundo) = doUpdate currentnode head
                             //travel the depth
                             let newtest =  -(match currentdepth-1 with 
                                              | x when x <= 0 || isTerminal newnode-> newnode.Value
                                              | _ ->  match (getTranspose newnode.ZobristHash (currentdepth-1) newnode.Turn) with
                                                      | Some(Transpose.Exact,x) -> x
                                                      | Some(Transpose.Upper,x) when x <= -currentbeta-> x
                                                      | Some(Transpose.Lower,x) when x >= -currentalpha -> x
                                                      | _ ->  searcher -currentbeta -currentbeta -currentalpha newnode (newnode.AvailableMoves.Force()) (currentdepth-1))                                
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
                             elif timer.ElapsedMilliseconds > time then -inf-1000
                             else searcher origalpha newalpha currentbeta currentnode tail currentdepth
                                
                           | [] -> if currentalpha > origalpha && currentalpha < currentbeta then 
                                                //in window, perfect hit
                                                do setTranspose currentnode.ZobristHash currentalpha Transpose.Exact currentdepth currentnode.Turn
                                                //since we don't know if we got this or something less than this, this is
                                                //an upperbound
                                   elif currentalpha <= origalpha then
                                                do setTranspose currentnode.ZobristHash currentalpha Transpose.Upper currentdepth currentnode.Turn
                                   else ()
                                   currentalpha
        //iterative deepening
        let rec deepener appdepth currbestmove =

            let mutable curralpha = -inf
            let mutable movelist = []
            let mutable bestmove = (botMove,-inf-1000)

            for i in moves do
                let newnode = update node i 
                let newalpha = -(searcher -inf -inf -curralpha newnode (newnode.AvailableMoves.Force()) (appdepth-1))
                do movelist <- (i,newalpha)::movelist
                do curralpha <- newalpha
                if newalpha > (snd bestmove) then
                    do bestmove <- (i,newalpha)
                else ()

            if timer.ElapsedMilliseconds >= time then 
                    do printfn "searchdepth %d" (appdepth-2)
                    currbestmove
            //if we hit an endgame
            elif (snd bestmove) >= inf/2 then bestmove
            elif (snd bestmove) < -inf/2 then currbestmove
            //otherwise keep going by 2
            else deepener (appdepth+2) bestmove
        deepener 0 (botMove,-inf-1000)

    let Ponder pred node =
        let rec searcher origalpha currentalpha currentbeta currentnode movelist currentdepth =
            if pred() then -inf-1000
            else
            match movelist with 
            //travel along the breadth
            | head::tail ->  let (newnode,newundo) = doUpdate currentnode head
                             //travel the depth
                             let newtest =  -(match currentdepth-1 with 
                                              | x when x <= 0 || isTerminal newnode-> newnode.Value
                                              | _ ->  match (getTranspose newnode.ZobristHash (currentdepth-1) newnode.Turn) with
                                                      | Some(Transpose.Exact,x) -> x
                                                      | Some(Transpose.Upper,x) when x <= -currentbeta-> x
                                                      | Some(Transpose.Lower,x) when x >= -currentalpha -> x
                                                      | _ ->  searcher -currentbeta -currentbeta -currentalpha newnode (newnode.AvailableMoves.Force()) (currentdepth-1))                                
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
                             else searcher origalpha newalpha currentbeta currentnode tail currentdepth
                                
                           | [] -> if currentalpha > origalpha && currentalpha < currentbeta then 
                                                //in window, perfect hit
                                                do setTranspose currentnode.ZobristHash currentalpha Transpose.Exact currentdepth currentnode.Turn
                                                //since we don't know if we got this or something less than this, this is
                                                //an upperbound
                                   elif currentalpha <= origalpha then
                                                do setTranspose currentnode.ZobristHash currentalpha Transpose.Upper currentdepth currentnode.Turn
                                   else ()
                                   currentalpha
                        

        //iterative deepening
        let rec deepener appdepth currbestmove =

            let newmove = searcher -inf -inf inf node (node.AvailableMoves.Force()) appdepth
            if pred() then 
                    do printfn "fake searchdepth %d" (appdepth-2)
                    (botMove,currbestmove)
            //if we hit an endgame
            //otherwise keep going by 2
            else deepener (appdepth+2) newmove 
        deepener 1 0


    let NewDFSABID time input = AlphaBetaDFSID input time (-inf) inf
    let NewDFSABIDTT time input = AlphaBetaDFSTT input time (-inf) inf
    let Ponderer pred input = Ponder pred input