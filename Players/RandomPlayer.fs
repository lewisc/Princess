//Lewis Coates (c) April 7, 2011
module TestLoop
open MoveGeneration
open BoardCombinators
open Searching.Heuristics
open System
open System.IO

let Rand = Random()

let Args = System.Environment.GetCommandLineArgs()


let RandomMove (board:GameState) =
         let newnodes = board.AvailableMoves.Force()
         if (newnodes).Length > 0
         then (Array.ofList (newnodes)).[Rand.Next(0,(newnodes.Length-1))]
         else raise (Hell("boarked"))


let mutable toplay = initialState SimpleCount initialSimple

let mutable toprint = RandomMove toplay

if Args.Length < 2 then printfn "requires a filename" 
else
use Printer = new StreamWriter(Args.[1])

for i = 1 to 1000 do
    toplay <- initialState SimpleCount initialSimple
    
    Printer.WriteLine("=Game " + i.ToString())
    printfn "%d" i
    while toplay.IsPlaying = true do
        toprint <- RandomMove toplay
        toplay <- update toplay (toprint)
        Printer.WriteLine(sprintMove toprint)
//x        Printer.WriteLine(toplay.ToString())