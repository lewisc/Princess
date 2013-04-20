namespace Books
open MoveGeneration
open System

//3 cases, high cutoff, low cutoff, or exact
type Transpose =
        | Exact = 0
        | Lower = 1
        | Upper = 2


type Contents= {mutable depth:int;mutable value:int; mutable strength:Transpose;mutable Hash:int64;mutable IsValid:bool;mutable Turn:Color}

module TranspositionTable =
    [<Literal>]
    let private tableSize = 2000000

    let (localTable:Contents []) = let ret = Array.create tableSize {depth=0;value=0;strength=Transpose.Exact;Hash=0L;IsValid=false;Turn=White}
                                   for i in 0..(tableSize-1) do
                                        ret.[i] <- {depth=0;value=0;strength=Transpose.Exact;Hash=0L;IsValid=false;Turn=White}
                                   ret

    let getDiag (hash:int64) =
        let retval = localTable.[abs(int32(hash))%tableSize]
        match retval with
        | t when t.Hash = hash -> Some(t)
        | _ -> None


    let getTranspose (hash:int64) depth turn = 
        match localTable.[abs(int32(hash))%tableSize] with
        | t -> match (t.strength) with
               | Transpose.Exact when t.depth >= depth
                            && hash = t.Hash && t.IsValid && turn = t.Turn-> Some((Transpose.Exact,(t.value)))
               | Transpose.Lower when t.depth >= depth
                                   && hash = t.Hash && t.IsValid && turn = t.Turn -> Some((Transpose.Lower,(t.value)))
               | Transpose.Upper when t.depth >= depth
                                    && hash = t.Hash && t.IsValid && turn = t.Turn -> Some((Transpose.Upper,(t.value)))
               | _ -> None

    let setTranspose (hash:int64) value flag depth turn=
                    match localTable.[abs(int32(hash))%tableSize] with
                    | t ->  do t.Hash <- hash
                            do t.strength <- flag
                            do t.value <- value
                            do t.depth <- depth
                            do t.IsValid<- true
                            do t.Turn <- turn