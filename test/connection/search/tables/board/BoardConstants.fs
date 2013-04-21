namespace MoveGeneration
module BoardConstants =
    [<Literal>]
    let maxXVal = 5

    [<Literal>]
    let maxYVal = 4

    [<Literal>]
    let boardWidth = 5

    [<Literal>]
    let boardLength = 6

    let blackPieces= [King(Black); 
                      Queen(Black);
                      Knight(Black);
                      Rook(Black);
                      Pawn(Black);
                      Bishop(Black);]
    
    let whitePieces= [King(White); 
                      Queen(White);
                      Knight(White);
                      Rook(White);
                      Pawn(White);
                      Bishop(White);]

    let allPieces = blackPieces@whitePieces

    let allPositions = [for i in 0..maxXVal do 
                            for j in 0..maxYVal-> (i,j)]

    let weightedWhitePieces =
        [King(White); 
         Queen(White);
         Knight(White);
         Rook(White);
         Bishop(White);
         Pawn(White);
         Pawn(White);
         Pawn(White);
         Pawn(White);
         Pawn(White);]

    let weightedBlackPieces =
        [King(Black); 
         Queen(Black);
         Knight(Black);
         Rook(Black);
         Bishop(Black);
         Pawn(Black);
         Pawn(Black);
         Pawn(Black);
         Pawn(Black);
         Pawn(Black);]

    let allWeightedPieces = weightedWhitePieces@weightedBlackPieces

    let pieceCombinations n =
        let ret = [King(Black);King(White)]
        let remainingPieces = List.choose (fun x -> match x with
                                                    | King(_) -> None
                                                    | x -> Some(x)) allWeightedPieces
        let rec searcher d (set:Pieces list Set) currlist listremaining =
                match d with
                | 0 -> set.Add(currlist)
                | _ -> match listremaining with
                       | head::tail ->let newres = searcher (d-1) set (head::currlist) tail
                                      searcher d newres currlist tail
                       | [] -> set
        searcher n Set.empty ret remainingPieces
