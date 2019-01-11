namespace CustomLib


module FoldBreak = 

    let rec foldBreak<'In, 'Acc> (app : 'In -> 'Acc -> Choice<'Acc, 'Acc>)
                                 (values : 'In list)
                                 (initialAcc : 'Acc) : 'Acc = 
                    match values with
                    | [] -> initialAcc
                    | head :: tail ->
                      match (app head initialAcc) with
                      | Choice2Of2(value) -> value
                      | Choice1Of2(value) -> foldBreak app tail value


