namespace Books


//This Module represents a transposition table using a Zobrist hashkey
//This is mutable code (effectively a database)
//TODO: Convert this to a class
module TranspositionTable =

    open MoveGeneration
    open System

    //3 cases, high cutoff, low cutoff, or exact
    type Transpose =
            | Exact = 0
            | Lower = 1
            | Upper = 2

    //the information necessary to determine if the transpose
    //can be used (i.e. if the cache is valid)
    type Contents = {mutable depth:int;
                     mutable value:int;
                     mutable strength:Transpose;
                     mutable hashValue:int64;
                     mutable isValid:bool;
                     mutable turn:Color}

    //Arbitrary number of rows in table, not tuned at all
    [<Literal>]
    let private TableSize = 2000000

    let (localTable:Contents []) = 
        Array.create TableSize {depth=0;
                                value=0;
                                strength=Transpose.Exact;
                                hashValue=0L;
                                isValid=false;
                                turn=White}

    //Getters and setters for transpositions
    let getTranspose (hashValue:int64) depth turn = 
        match localTable.[abs(int32(hashValue))%TableSize] with
        | t when t.depth >= depth
              && t.hashValue = hashValue
              && t.isValid
              && turn = t.turn ->
                  Some((t.strength, (t.value)))
        | _ -> None

    let setTranspose (hashValue:int64) value flag depth turn =
                    match localTable.[abs(int32(hashValue))%TableSize] with
                    | t ->  do t.hashValue <- hashValue
                            do t.strength <- flag
                            do t.value <- value
                            do t.depth <- depth
                            do t.isValid<- true
                            do t.turn <- turn
