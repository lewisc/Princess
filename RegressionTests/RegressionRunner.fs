//module RegressionRunner
open System
open IMCS
open MoveGeneration
open BoardCombinators
open Config.Properties
open Searching
open Heuristics
open BoardCombinators
open DepthFirstSearch
open System.Diagnostics
open MoveRegression
open DFSRegression
open System
open TestTypes
open Heuristics
open AlphaBeta2
open ZobristKeys
open MTDF
open BoardConstants
open System.IO
open Books
open TranspositionTable
//open EndGameDB


#if MOVETEST
do tester (MoveRegression ())
#endif
#if SEARCHTEST
let foo = localTable
do tester (DFSTests())
#endif

#if ENDGAMETEST
let foo = states2
let bar = states3
let blort = states4

let counter = ref 0

for i in foo do
    counter := !counter+1

printfn "%d" !counter
counter := 0

for i in bar do
    counter := !counter+1

printfn "%d" !counter
counter := 0

for i in blort do
    counter := !counter+1

printfn "%d" !counter
counter := 0
#endif

#if COMBINATIONSTEST
let piece0 = pieceCombinations 0 |> Set.toList  |> List.sort 
let piece1 = pieceCombinations 1 |> Set.toList  |> List.sort 
let piece2 = pieceCombinations 2 |> Set.toList  |> List.sort
let piece3 = pieceCombinations 3 |> Set.toList  |> List.sort
let piece4 = pieceCombinations 4 |> Set.toList  |> List.sort


let s0 = List.map (fun n -> List.fold (fun y x -> String.Concat((sprintf "%s" (x.ToString())),y)) "" n) piece0 
let s1 = List.map (fun n -> List.fold (fun y x -> String.Concat((sprintf "%s" (x.ToString())),y)) "" n) piece1 
let s2 = List.map (fun n -> List.fold (fun y x -> String.Concat((sprintf "%s" (x.ToString())),y)) "" n) piece2
let s3 = List.map (fun n -> List.fold (fun y x -> String.Concat((sprintf "%s" (x.ToString())),y)) "" n) piece3
let s4 = List.map (fun n -> List.fold (fun y x -> String.Concat((sprintf "%s" (x.ToString())),y)) "" n) piece4

let writer = new StreamWriter(@"F:\test.txt")

let write  y = List.iter (fun (x:string) -> printfn "%s" x) y

do write  s0
do write  s1
do write  s2
do write  s3
do write  s4
#endif

#if TESTGAME
let rand = Random();
let RandomMove (board:GameState) (rand:Random) =
         let newnodes = board.AvailableMoves.Force()
         if (newnodes).Length > 0
         then ((Array.ofList (newnodes)).[rand.Next(0,(newnodes.Length-1))],-1)
         else raise (Hell("boarked"))

let counter = ref 1
let rec play input color =
    do printfn "%d" !counter
    do counter := !counter + 1
    if (input.IsPlaying = false) then input
    else
    
    match color with 
    | White ->let mymove = DepthFirstSearch 4 input
              let testmove = DFSAB 4 input
              let testmove2 = DepthFirstSearchdoUndo 4 input
              //let testmove3 = IterativeDeepenerAB 4 100000L SimpleCount input
              //let testmove4 = IterativeDeepenerDoUndo 8000L input
              //let testmove5 = DFSABTTID 8000L input

              let testmove6 = MTDF 4 input
              let testmove7 = DFSABTT 4 input
              let testmove8 = DFSABTTtest 4 input

              let inbook7 = getDiag (update input (fst testmove7)).ZobristHash
              let inbook6 = getDiag (update input (fst testmove6)).ZobristHash
              assert((snd mymove) = (snd testmove))

              assert(if (snd testmove) < (inf/2) || (snd testmove) > (-inf/2) 
                     then (snd testmove2) = (snd testmove)
                     else Math.Abs(snd testmove) > (inf/2) 
                       && Math.Abs(snd mymove) > (inf/2) 
                       && Math.Sign(snd testmove) = Math.Sign(snd mymove))

              //assert((snd testmove3) = (snd testmove))
              if not(isTerminal input) then let newgame = (update input (fst mymove))
                                            do printfn "%d" (snd mymove)
                                            do printfn "%s" (sprintBoard newgame)
                                            do printfn "%d" newgame.Value
                                            play newgame Black
              else input

    | Black -> let mymove = RandomMove input rand
               let testmove = DepthFirstSearch 4 input
               let testmove2 = DFSAB 4  input
               let testmove3 = DepthFirstSearchdoUndo 4 input
               let testmove6 = MTDF 4 input
               let testmove7 = DFSABTT 4 input
               let testmove8 = DFSABTTtest 4 input
               //let testmove9 = MTDFID 8000L input
               let inbook7 = getDiag (update input (fst testmove7)).ZobristHash
               let inbook6 = getDiag (update input (fst testmove6)).ZobristHash
               //let testmove4 = IterativeDeepenerAB 4 100000L SimpleCount input
               assert((snd testmove2) = (snd testmove))
               assert((snd testmove2) = (snd testmove3))
               //assert((snd testmove2) = (snd testmove4))
               if not(isTerminal input) then let newgame = (update input (fst testmove6))
                                             do printfn "%d" (snd testmove6)
                                             do printfn "%s" (sprintBoard newgame)
                                             do printfn "%d" newgame.Value
                                             play newgame White
               else input
let test = play (initialState Advancement initialAdvancement) White
do printfn "any key to continue..."


//let ret = connectAndPlayGame "svcs.cs.pdx.edu" 3589 SimpleCount (IterativeDeepener) "princess" "1234"
//do BoardCombinators.boardToString ret |> printfn "%s"

#endif
do printfn "black %d white %d" !newblackhit !newwhitehit
//do Console.ReadKey() |> ignore