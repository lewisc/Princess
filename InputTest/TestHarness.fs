module TestHarness
open MoveGen
open System.IO
open System
open BoardCombinators

let newgame = Board()

let rec play (board:Board) cont action =
        match (Board.tryPlay board action),cont with
        | ((true, newboard),true) -> printfn "\n%s\n" (newboard.ToString())
                                     play newboard (newboard.InPlay) (Console.ReadLine())
        | (_,true) -> printfn "invalid"
                      play board (board.InPlay) (Console.ReadLine())
        | (_,false) -> printfn "done"

play newgame (newgame.InPlay) (Console.ReadLine())