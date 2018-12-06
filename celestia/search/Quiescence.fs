namespace Celestia

open BoardCombinators
open Primitives

module Quiescence =
    

    let rec quiesce node alpha beta =
        let value = node.Value

        if value >= beta then
            beta
        else
        let localalpha = if value > alpha then value else alpha
        let captures = Array.choose (fun (x,(i,j)) -> match (node.BoardState.[i,j]) with
                                                      | Some(p) when p.Color <> node.Turn -> Some(x,(i,j))
                                                      | _ -> None) (node.AvailableMoves.Force())
        let rec traverser alpha index =
            match index with
            | x -> let (newgame,newundo) = doUpdate node captures.[x]
                   let neweval = -(quiesce newgame -beta -alpha)
                   do undoUpdate newgame newundo
                   if neweval >= beta then
                       neweval
                   else traverser (max alpha neweval) (x+1)
            | y when y > captures.Length -> alpha
        traverser localalpha 0



