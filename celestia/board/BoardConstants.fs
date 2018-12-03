namespace MoveGeneration


module BoardConstants =
    ///0-Indexed Lenght of Board
    [<Literal>]
    let maxXVal = 5

    ///0-Indexed width of Board
    [<Literal>]
    let maxYVal = 4

    ///Length of Board
    [<Literal>]
    let boardWidth = 5

    ///Width of Board
    [<Literal>]
    let boardLength = 6

    ///Array of the different kinds of black pieces
    let blackPieces ()= 
                    [|King(Black); 
                      Queen(Black);
                      Knight(Black);
                      Rook(Black);
                      Pawn(Black);
                      Bishop(Black);|]
    
    ///Array of the different kinds of white pieces
    let whitePieces () = 
                    [|King(White); 
                      Queen(White);
                      Knight(White);
                      Rook(White);
                      Pawn(White);
                      Bishop(White);|]

    ///Array of all the different kinds of pieces
    let allPieces () = Array.append (blackPieces ()) (whitePieces ())

    ///array of all the different board positions
    let allPositions = [|for i in 0..maxXVal do 
                            for j in 0..maxYVal-> (i,j)|]

    ///array of all the different white pieces, with appropriate repeats
    let whitePiecesStart () =
        [|King(White); 
         Queen(White);
         Knight(White);
         Rook(White);
         Bishop(White);
         Pawn(White);
         Pawn(White);
         Pawn(White);
         Pawn(White);
         Pawn(White);|]

    ///array of all the different black pieces, with appropriate repeats
    let blackPiecesStart () = 
        [|King(Black); 
         Queen(Black);
         Knight(Black);
         Rook(Black);
         Bishop(Black);
         Pawn(Black);
         Pawn(Black);
         Pawn(Black);
         Pawn(Black);
         Pawn(Black);|]

    ///array of all the different pieces, with appropriate repeats
    let allPiecesStart () = Array.append (whitePiecesStart ()) (blackPiecesStart ())

    ///represents a default board
    let defaultBoard () = 
     array2D [|[|Some(King(White)); Some(Queen(White)); Some(Bishop(White)); Some(Knight(White));Some(Rook(White))|];
               [|Some(Pawn(White)); Some(Pawn(White));  Some(Pawn(White));   Some(Pawn(White));  Some(Pawn(White))|];
               [|None ;             None ;              None;                None;               None             |];
               [|None ;             None ;              None;                None;               None             |];
               [|Some(Pawn(Black)); Some(Pawn(Black));  Some(Pawn(Black));   Some(Pawn(Black));  Some(Pawn(Black))|];
               [|Some(Rook(Black)); Some(Knight(Black));Some(Bishop(Black)); Some(Queen(Black)); Some(King(Black))|];|]

    ///This is used for the endgame DB
    ///Not complete
    let pieceCombinations n =
        let ret = [King(Black);King(White)]
        let remainingPieces = List.choose (fun x -> match x with
                                                     | King(_) -> None
                                                     | x -> Some(x)) (Array.toList (allPiecesStart ()))
        let rec searcher d (set:Pieces list Set) currlist listremaining =
                match d with
                | 0 -> set.Add(currlist)
                | _ -> match listremaining with
                       | head::tail ->let newres = searcher (d-1) set (head::currlist) tail
                                      searcher d newres currlist tail
                       | [] -> set
        searcher n Set.empty ret remainingPieces
