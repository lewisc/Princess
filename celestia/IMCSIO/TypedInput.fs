namespace Celestia

open System.Text
open System.Text.RegularExpressions

//TODO: DOcument
module TypedInput =
    //active pattern for regex parsing
    //Shamelessly cribbed from MSDN
    //http://msdn.microsoft.com/en-us/library/dd233248.aspx
    let private (|ParseRegex|_|) regex str =
        let m = Regex(regex).Match(str)
        match m.Success with
        | true -> Some (List.tail [ for x in m.Groups -> x.Value ])
        | false -> None
    
    //parse the integer
    let private (|Integer|_|) (str: string) =
        let mutable intvalue = 0
        match System.Int32.TryParse(str, &intvalue)with
        | true -> Some(intvalue)
        | false -> None
    
    //parse the column
    let private (|ColToNum|_|) input =
        let values = Map([ ("a" , 4); ("b", 3); ("c", 2); ("d", 1); ("e", 0) ])
        match Map.containsKey input values with
        | true -> Some(values.[input])
        | false -> None
    
    //strongly typed input
    let (|ReadInput|_|) input =
        match input with 
        | None -> None
        | Some(s) ->
            match s with 
            | ParseRegex "!\s?([a-e])([1-6])-([a-e])([1-6])" 
                    [ ColToNum fx; Integer fy; ColToNum tx; Integer ty ] 
                        -> Some(((fy-1), fx), ((ty-1), tx))
            | _ -> None

    let (|PacketLine|_|) (inputString : string option)
                         : (string * (string list)) option =
        match inputString with
        | None -> None
        | Some(s) -> match Array.toList (s.Trim().Split[|' '|]) with
                     | (marker :: _) as packet -> Some(marker, packet)
                     | [] -> None
