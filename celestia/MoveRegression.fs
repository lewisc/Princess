//Lewis Coates (c) April 11th 2011, All rights reserved
module MoveRegression
open MoveGeneration
open BoardCombinators
open TypedInput
open TestTypes
open System.IO

open System.Threading.Tasks
open System.Linq

//get the data and preprocess it(it's from a file so you
//split on newlines
let splitchars = [| '\n';'\r'|]
//turn it int a list and filtor out any empty strings
let testdata = List.ofArray <| File.ReadAllLines(@"testdata/testdata.txt")
               |> List.filter (fun x -> if x = "" then false else true)


//preprocess the data(regex the hell out of it)
let processeddata = List.fold ( fun (worker, master) str -> 
                                    match str with
                                    | ReadInput x -> (x::worker, master)
                                    | _ -> ([], worker::master)) ([],[]) testdata
                    |> fun (x,y) -> x::y
                    //this is unneccessary I think
                    |> List.filter (fun x-> x <> [])

//with checks
//let ret1  = testFunc (fun () -> (Array.Parallel.map (fun x -> testGame (List.rev x)) (Array.ofList processeddata) |> List.ofArray))

//without checks
//let ret2  = testFunc (fun () -> (Array.Parallel.map (fun x -> perfTest (List.rev x)) (Array.ofList processeddata) |> List.ofArray))

//with checks
let ret3  = testFunc (fun () -> (List.map (fun x -> testGame (List.rev x)) processeddata))

//without checks
let ret4  = testFunc (fun () -> (List.map (fun x -> perfTest (List.rev x)) processeddata))


let aggregater n = List.forall (fun x-> x=true) n

let results = List.map (fun (x,(y,z)) -> (x,aggregater y,z)) (("Series",ret3)::("series unchecked",ret4)::[])


let MoveRegression () = results
