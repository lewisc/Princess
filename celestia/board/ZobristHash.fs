//Lewis Coates (c) April 7, 2011
namespace Celestia

open System
open System.Collections.Generic

open Primitives


///Calculates a Zobrist key for the minichess board
//TODO: Document
module ZobristHash =

    type ZobristHash(board : Board) = 

        static let piecesOption : Pieces option [] =
            [| Some(King(Black));
               Some(Queen(Black));
               Some(Knight(Black));
               Some(Rook(Black));
               Some(Pawn(Black));
               Some(Bishop(Black)); 
               Some(King(White));
               Some(Queen(White));
               Some(Knight(White));
               Some(Rook(White));
               Some(Pawn(White));
               Some(Bishop(White)); |]

        static let pieceXPosition : (Pieces option * Position) list =
            [ for pos in AllPositions do
                  for piece in piecesOption do
                      yield (piece, pos) ]

        static let rand : Random = Random()

        static let get64bit () : int64 = 
            let retval = Array.create 8 0uy
            do rand.NextBytes(retval)
            BitConverter.ToInt64(retval, 0)

        static let zobristMap :
            IDictionary<(Pieces option * Position), (int64)> =
            pieceXPosition
            |> Seq.map (fun piece -> (piece, get64bit ()))
            |> dict

        let mutable zobristValue : int64 =
                Array.fold (fun x (z1, z2) ->
                                x ^^^ (zobristMap.[board.[z1, z2], (z1, z2)]))
                           0L AllPositions

        member self.ZobristValue
            with get () = zobristValue
            and private set(value) = zobristValue <- value
            

        member self.IncrementalAdder x1 x2 x3 x4 = 
                self.ZobristValue  <- self.ZobristValue
                                      ^^^ (zobristMap.[x1])
                                      ^^^ (zobristMap.[x2])
                                      ^^^ (zobristMap.[x3])
                                      ^^^ (zobristMap.[x4])
