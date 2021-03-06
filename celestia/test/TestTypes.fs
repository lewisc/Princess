﻿module TestTypes
open System
open System.Diagnostics
open System.Threading

type Testvals = (string * bool * int)

let testFunc f =
    let stopwatch = Stopwatch.StartNew()
    let retval = f()
    stopwatch.Stop()
    (retval, stopwatch.ElapsedMilliseconds)

//z is performance, y is passed or failed
//s is a string to print (identifier of test
let tester x = List.iter (fun (x,y,(z:int64)) -> do printf "%s " x; 
                                                 do if y 
                                                    then printf "passed " 
                                                    else printf "failed " ; 
                                                 do printfn "%d" z) x

