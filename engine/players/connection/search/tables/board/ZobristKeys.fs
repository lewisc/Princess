namespace MoveGeneration
open BoardConstants
open System
open System.Collections.Generic

module ZobristKeys =
    let piecesOption = None::((List.map (fun x -> Some(x)) allPieces))
    let pieceXPosition = 
        List.collect (fun x -> (List.map (fun y -> x,y ) allPositions)) piecesOption 

    let rand = Random()

    let add64bit () = 
        let retval = Array.create 8 0uy
        do rand.NextBytes(retval)
        BitConverter.ToInt64(retval,0)

    let associater (rand:Random) (elements:'a list) =
        let rec adder (map:Set<int64>) = 
                if map.Count < elements.Length
                then adder (map.Add(Convert.ToInt64(rand.Next())))
                else map
        let ret map = Set.toList map
                   |> List.zip elements

        let mymap = adder Set.empty
        ret mymap

    let associater64 (elements:'a list) =
        let rec adder (map:Set<int64>) =
            if map.Count < elements.Length
            then adder (map.Add(add64bit()))
            else map
        let ret map = Set.toList map
                   |> List.zip elements
        let mymap = adder Set.empty
        ret mymap

    let zobristMap32 = 
                     let keys = associater rand pieceXPosition
                     let todictionary = Dictionary<(Pieces option *Position),int64>(390)
                     List.iter(fun (x,y) -> todictionary.Add(x,y)) keys
                     todictionary

    let zobristMap64 = 
                     let keys = associater64 pieceXPosition
                     let todictionary = Dictionary<(Pieces option *Position),int64>(390)
                     List.iter(fun (x,y) -> todictionary.Add(x,y)) keys
                     todictionary

    let zobristMap = zobristMap64

                     

    let zobristAdder (input:Pieces option [,]) =
        let ret = List.fold (fun x (z1,z2) -> x^^^(zobristMap.[input.[z1,z2],(z1,z2)])) 0L allPositions
        ret