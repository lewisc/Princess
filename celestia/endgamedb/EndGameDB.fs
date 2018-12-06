namespace Celestia

open System.Runtime.Serialization.Formatters.Binary
open System.Runtime.Serialization

open MTDF
open AlphaBeta2
open Primitives
open BoardCombinators
open Heuristics

open System
open System.Collections.Generic
open System.IO

module EndGameDB =
///array of all the different white pieces, with appropriate repeats
    let whitePiecesStart () =
        [|King(White); 
         Queen(White);
         Knight(White);
         Rook(White);
         Bishop(White);
         Pawn(White);
         Pawn(White);
         Pawn(White);
         Pawn(White);
         Pawn(White);|]
    ///array of all the different black pieces, with appropriate repeats
    let blackPiecesStart () = 
        [|King(Black); 
         Queen(Black);
         Knight(Black);
         Rook(Black);
         Bishop(Black);
         Pawn(Black);
         Pawn(Black);
         Pawn(Black);
         Pawn(Black);
         Pawn(Black);|]
    ///array of all the different pieces, with appropriate repeats
    let allPiecesStart () = Array.append (whitePiecesStart ()) (blackPiecesStart ())

  ///This is used for the endgame DB
    ///Not complete
    let pieceCombinations n =
        let ret = [King(Black);King(White)]
        let remainingPieces = List.choose (fun x -> match x with
                                                     | King(_) -> None
                                                     | x -> Some(x)) (Array.toList (allPiecesStart ()))
        let rec searcher d (set:Pieces list Set) currlist listremaining =
                match d with
                | 0 -> set.Add(currlist)
                | _ -> match listremaining with
                       | head::tail ->let newres = searcher (d-1) set (head::currlist) tail
                                      searcher d newres currlist tail
                       | [] -> set

        searcher n Set.empty ret remainingPieces
    let positions = [ for i in 0 .. 4 do for j in 0 ..5  -> (i, j)]

    let positions2 = 
        seq { for i in positions do
                 for j in positions do
                    if i<>j 
                    then yield (i, j)}

    let positions3 = 
        seq { for i in positions do
                 for j in positions do
                    for k in positions do
                        if i<>j && j<>k && i <> k
                        then  yield (i, j, k)}
    let positions4 = 
        seq { for i in positions do
                 for j in positions do
                    for k in positions do
                        for l in positions do
                            if i<>j && j<>k && k<>l 
                            && i<>k && i<>l && j<>l
                            then  yield (i,j,k,l)}

    let pieces2 = pieceCombinations 0 |> Set.toSeq
    let pieces3 = pieceCombinations 1 |> Set.toSeq
    let pieces4 = pieceCombinations 2 |> Set.toSeq

    let states2 : ((int * int) * Pieces) list list =
        [ for i in pieces2 do
                for (j,k) in positions2 do
                   yield [(j,i.[0]); (k,i.[1])] ]
    let states3 =
        [for i in pieces3 do
                for (j,k,l) in positions3 do
                    yield  [(j,i.[0]);(k,i.[1]);(l,i.[2])]]
    let states4 =
        seq { for i in pieces4 do
                for (j,k,l,m) in positions4 do
                   yield  ((j,i.[0]),(k,i.[1]),(l,i.[2]),(m,i.[3]))}

    let comparison game1 game2 =
        game1.Turn = game2.Turn &&
        game1.BoardState = game2.BoardState

    //keep iterating until you hit fixpointgg
//    let rec fixCounter

    [<EntryPoint>]
    let main args =

        let boards2 = (List.map (fun i -> setState SimpleCount initialSimple (buildBoard i) White) states2) @ (List.map (fun i -> setState SimpleCount initialSimple (buildBoard i) Black) states2)
        let boards3 = (List.map (fun i -> setState SimpleCount initialSimple (buildBoard i) White) states3) @ (List.map (fun i -> setState SimpleCount initialSimple (buildBoard i) Black) states3)
        let knownKills = (List.filter (fun i -> 
            let value = (AlphaBetaSearch i 2)
            //printfn "%s" (sprintBoard i)
            //printfn "%s" (value.ToString())
            abs(snd(value)) > inf) (boards2))

        List.map (fun i -> printfn "%s" (i.ToString())) knownKills |> ignore

        0
