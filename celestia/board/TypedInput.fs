namespace MoveGeneration

module TypedInput =

    open System.Text
    open System.Text.RegularExpressions

    //active pattern for regex parsing
    //Shamelessly cribbed from MSDN
    //http://msdn.microsoft.com/en-us/library/dd233248.aspx
    let (|ParseRegex|_|) regex str =
            let m = Regex(regex).Match(str)
            if m.Success
            then Some (List.tail [ for x in m.Groups -> x.Value ])
            else None
    
    //parse the integer
    let (|Integer|_|) (str: string) =
       let mutable intvalue = 0
       if System.Int32.TryParse(str, &intvalue) then Some(intvalue)
       else None
    
    //parse the column
    let (|ColToNum|_|) input =
        let foo = Map([("a",4);("b",3);("c",2);("d",1);("e",0)])
        if Map.containsKey input foo
        then Some(foo.[input])
        else None
    
    //strongly typed input
    let (|ReadInput|_|) input =
        match input with 
        | ParseRegex "!\s?([a-e])([1-6])-([a-e])([1-6])" 
                [ColToNum fx;Integer fy;ColToNum tx;Integer ty] 
                    -> Some(((fy-1),fx),((ty-1),tx))
        |_ -> None

