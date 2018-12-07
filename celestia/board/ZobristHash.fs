//Lewis Coates (c) April 7, 2011
namespace Celestia

open System
open System.Collections.Generic

open Primitives


module ZobristHash =

    let private piecesOption = [| Some(King(Black));
                                  Some(Queen(Black));
                                  Some(Knight(Black));
                                  Some(Rook(Black));
                                  Some(Pawn(Black));
                                  Some(Bishop(Black)); 
                                  Some(King(White));
                                  Some(Queen(White));
                                  Some(Knight(White));
                                  Some(Rook(White));
                                  Some(Pawn(White));
                                  Some(Bishop(White)); |]

    let private pieceXPosition = piecesOption
                                 |> Array.map (fun x ->
                                        Array.map (fun y ->
                                            (x, y)) AllPositions)
                                 |> Array.concat

    let private rand = Random()

    let private add64bit ()= 
        let retval = Array.create 8 0uy
        do rand.NextBytes(retval)
        BitConverter.ToInt64(retval, 0)

    let private associater64 (elements:'a []) =
        let rec adder (map : Set<int64>) =
            if map.Count < elements.Length
            then    
                    adder (map.Add(add64bit()))
            else map
        let ret map = Set.toArray map
                   |> Array.zip elements
        let mymap = adder Set.empty
        ret mymap

    let private zobristMap = 
                let keys = associater64 pieceXPosition
                let todictionary = Dictionary<(Pieces option * Position),
                                               int64>(390)
                Array.iter(fun (x, y) -> todictionary.Add(x, y)) keys
                todictionary

    let zobristAdder (input : Board) =
        let ret = Array.fold (fun x (z1, z2) ->
                x ^^^ (zobristMap.[input.[z1, z2], (z1, z2)])) 0L AllPositions
        ret

    let incrementalZobristAdder zobristHash x1 x2 x3 x4 = 
            zobristHash 
            ^^^ (zobristMap.[x1])
            ^^^ (zobristMap.[x2])
            ^^^ (zobristMap.[x3])
            ^^^ (zobristMap.[x4])
