namespace Books
open System.Runtime.Serialization.Formatters.Binary
open System.Runtime.Serialization
open MoveGeneration
open BoardCombinators
open System
open Searching.Heuristics
open System.Collections.Generic
open System.IO
open BoardConstants

module EndGameDB =
    let positions = [ for i in 0y..4y do for j in 0y..5y -> (i,j)]

    let positions2 = 
        seq { for i in positions do
                 for j in positions do
                    if i<>j 
                    then yield (i,j)}

    let positions3 = 
        seq { for i in positions do
                 for j in positions do
                    for k in positions do
                        if i<>j && j<>k && i <> k
                        then  yield (i,j,k)}
    let positions4 = 
        seq { for i in positions do
                 for j in positions do
                    for k in positions do
                        for l in positions do
                            if i<>j && j<>k && k<>l 
                            && i<>k && i<>l && j<>l
                            then  yield (i,j,k,l)}

    let pieces2 = pieceCombinations 0 |> Set.toSeq
    let pieces3 = pieceCombinations 1 |> Set.toSeq
    let pieces4 = pieceCombinations 2 |> Set.toSeq

    let states2 =
        seq { for i in pieces2 do
                for (j,k) in positions2 do
                   yield  ((j,i.[0]),(k,i.[1]))}
    let states3 =
        seq { for i in pieces3 do
                for (j,k,l) in positions3 do
                   yield  ((j,i.[0]),(k,i.[1]),(l,i.[2]))}
    let states4 =
        seq { for i in pieces4 do
                for (j,k,l,m) in positions4 do
                   yield  ((j,i.[0]),(k,i.[1]),(l,i.[2]),(m,i.[3]))}



