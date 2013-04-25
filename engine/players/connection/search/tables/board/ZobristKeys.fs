namespace MoveGeneration
open BoardConstants
open System
open System.Collections.Generic

module ZobristKeys =
    let piecesOption = Array.append [|None|] ((Array.map (fun x -> Some(x)) (allPiecesStart ())))
    let pieceXPosition = 
        Array.collect (fun x -> (Array.map (fun y -> x,y ) allPositions)) piecesOption 

    let rand = Random()

    let add64bit () = 
        let retval = Array.create 8 0uy
        do rand.NextBytes(retval)
        BitConverter.ToInt64(retval,0)

    let associater (rand:Random) (elements:'a []) =
        let rec adder (map:Set<int64>) = 
                if map.Count < elements.Length
                then adder (map.Add(Convert.ToInt64(rand.Next())))
                else map
        let ret map = Set.toArray map
                   |> Array.zip elements

        let mymap = adder Set.empty
        ret mymap

    let associater64 (elements:'a []) =
        let rec adder (map:Set<int64>) =
            if map.Count < elements.Length
            then adder (map.Add(add64bit()))
            else map
        let ret map = Set.toArray map
                   |> Array.zip elements
        let mymap = adder Set.empty
        ret mymap

    let zobristMap32 = 
                     let keys = associater rand pieceXPosition
                     let todictionary = Dictionary<(Pieces option *Position),int64>(390)
                     Array.iter(fun (x,y) -> todictionary.Add(x,y)) keys
                     todictionary

    let zobristMap64 = 
                     let keys = associater64 pieceXPosition
                     let todictionary = Dictionary<(Pieces option *Position),int64>(390)
                     Array.iter(fun (x,y) -> todictionary.Add(x,y)) keys
                     todictionary

    let zobristMap = zobristMap64

                     

    let zobristAdder (input:Pieces option [,]) =
        let ret = Array.fold (fun x (z1,z2) -> x^^^(zobristMap.[input.[z1,z2],(z1,z2)])) 0L allPositions
        ret
