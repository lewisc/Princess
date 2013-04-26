module DFSRegression
open MoveGeneration
open BoardCombinators
open TestTypes
open Searching.Heuristics
open Searching.DepthFirstSearch
open Searching.MTDF
open Searching.AlphaBeta2
open Searching.AlphaBetaID



let dfsdoUndo n = ((DepthFirstSearchdoUndo n (initialState SimpleCount initialSimple)))
let MTDFtest n = ((MTDF n (initialState SimpleCount initialSimple)))
let MTDFIDtest n = ((MTDFID 10000L (initialState SimpleCount initialSimple)))
let MTDFAdvtest n = ((MTDF n (initialState Advancement initialAdvancement)))
let AbIDTTtest n = ((NewDFSABIDTT 10000L (initialState Advancement initialAdvancement)))
let AbIDtest n = ((NewDFSABID 10000L (initialState Advancement initialAdvancement)))





//let MTDFres = List.map (fun(x,y) -> ("MTDFRes TTable",true,y)) (List.map (fun x -> testFunc (fun () -> MTDFtest  x)) [0;2;4;])
//let MTDadvFres = List.map (fun(x,y) -> ("MTDF adv TTable",true,y)) (List.map (fun x -> testFunc (fun () -> MTDFAdvtest x)) [0;2;4;])
let AbIDTTtestres = List.map (fun(x,y) -> ("DFS ABadv TTable",true,y)) (List.map (fun x -> testFunc (fun () -> AbIDTTtest x)) [0])
let AbIDtestres = List.map (fun(x,y) -> ("DFS AB Adv",true,y)) (List.map (fun x -> testFunc (fun () -> AbIDtest x)) [0])

    
let DFSTests () = 
                  //MTDFres@
                  //MTDadvFres@
                  AbIDtestres@
                  []
