namespace Searching
open MoveGeneration
open BoardCombinators
module Quiescence =
    

    let rec quiesce node alpha beta =
        let value = node.Value

        if value >= beta then
            beta
        else
        let localalpha = if value > alpha then value else alpha
        let captures = List.choose (fun (x,(i,j)) -> match (node.BoardState.[i,j]) with
                                                     | Some(p) when pieceColor p <> node.Turn -> Some(x,(i,j))
                                                     | _ -> None) (node.AvailableMoves.Force())
        let rec traverser alpha moves =
            match moves with
            | head::tail -> let (newgame,newundo) = doUpdate node head 
                            let neweval = -(quiesce newgame -beta -alpha)
                            do undoUpdate newgame newundo
                            if neweval >= beta then
                                neweval
                            else traverser (max alpha neweval) tail
            | [] -> alpha
        traverser localalpha captures



