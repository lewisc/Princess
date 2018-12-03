//Lewis Coates (c) April 7, 2011
//Simplest player, plays purely random moves

namespace Celestia.Players


module TestLoop =

    open System
    open System.IO

    open MoveGeneration
    open BoardCombinators
    open Searching.Heuristics
    open BoardHelpers

    [<EntryPoint>]
    //Play random moves, print to a file.
    //Used for diagnostics mostly
    //Requires an argument for a file to print to 
    let randPlayer args =

        if args.Length < 1 then
            printfn "requires a filename" 
            0

        else
            use printer = new StreamWriter(args.[1])
            let max = 100

            let rand = Random()

            // Determine a random move to play
            let randomMove (board:GameState) =
                let moves = board.AvailableMoves.Force()
                moves.[rand.Next(0, (moves.Length-1))]


            [1 .. max] |> List.map (fun i ->

                printer.WriteLine("=Game " + i.ToString())
                printfn "%d" i

                //play an entire game
                let rec playEngine gameState = 

                    //Determine a move, print it and apply it
                    let moveVal = randomMove gameState
                    printer.WriteLine(sprintMove moveVal)
                    let game = fst (doUpdate gameState (moveVal))

                    //Determine if the game is still playing, or if it's over
                    if game.IsPlaying then playEngine game else ()
 
                //Start a game with 
                playEngine (initialState SimpleCount initialSimple))

                |> ignore
            0
