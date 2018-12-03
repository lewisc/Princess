//(c) Lewis Coates 2018
//A network player to play minichess

namespace Celestia
open Actions
open System
open AlphaBeta2
open AlphaBetaID
open MTDF
open DepthFirstSearch
open TranspositionTable
open Heuristics

module NetworkPlayer =


    [<EntryPoint>]
    let main args =

        let player = {EvalFun = SimpleCount;
                      EvalInit = initialSimple;
                      SearchPrime = MTDFID;
                      SearchPonder = Ponderer;}

        // This needs a real UI at some point
        use conn = new IMCSConnection(@"svcs.cs.pdx.edu", 3589)
        // Bind with a name and a default password
        let bind = bindName conn "princess" "1234"
        try 
            let res = 
                match args.Length with
                | 0 -> Some(doOfferGame conn None None None player)
                | 1 ->
                    Some(doAcceptGame conn (Int32.Parse(args.[0])) None player)
                | _ -> printfn "failure, invalid args length"
                       None
            ()
        with
        | ProtocolError(i) -> printfn "fixthat. %s" i
                   
        0
