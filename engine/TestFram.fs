//Lewis Coates (c) April 7, 2011
module TestLoop
open MoveGen
open System
open System.IO
open System.Diagnostics



let Rand = Random()

let Args = System.Environment.GetCommandLineArgs

let RandomMove (board:Board) =
         if board.Moves.Length > 0
         then (Array.ofList board.Moves).[Rand.Next(0,(board.Moves.Length-1))]
         else raise (Hell("boarked"))


let Playout input runs =
    let rec play (game:Board) cont =
        if cont
        then let (newgame:Board) = game.Update(RandomMove game)
             play newgame newgame.InPlay
        else ()

    let rec execute count : unit =
                        play input true
                        if count > 0
                        then execute (count-1)
                        else ()

    let stopwatch  = new Stopwatch()
    stopwatch.Start()
    execute runs
    stopwatch.Stop()
    let output = stopwatch.Elapsed
    output

[1; 1; 10; 100; 1000; 1000] 
|> List.map (fun x -> (Playout (Board()) x, x))
|> List.map (fun (x,y) -> printfn "%s" (x.ToString())
                          printfn "%d" y)
|> ignore

//Console.ReadKey() |> ignore