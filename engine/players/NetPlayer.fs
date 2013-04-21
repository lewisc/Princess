open IMCS
open Actions
open System
open Searching
open AlphaBeta2
open AlphaBetaID
open MTDF
open DepthFirstSearch
open Books.TranspositionTable
open Heuristics

[<EntryPoint>]
let main args =

    let player = {EvalFun = SimpleCount;
                  EvalInit = initialSimple;
                  SearchPrime = MTDFID;
                  SearchPonder = Ponderer;}
    do getDiag 0L |>ignore

    use conn = new IMCSConnection(@"svcs.cs.pdx.edu",3589)
    let foo = connect conn
    let bind = bindName conn "princess" "1234"
    try 
        let res = 
            match args.Length with
            | 0 -> Some(doOfferGame conn None None None player)
            | 1 -> Some(doAcceptGame conn (Int32.Parse(args.[0])) None player)
            | _ -> printfn "failure, invalid args length"
                   None
        ()
    with
    | ProtocolError(i) -> printfn "fixthat. %s" i
               
    0
