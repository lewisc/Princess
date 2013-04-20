namespace MoveGeneration
open System.Text
open System.Text.RegularExpressions
///a module for parsing strongly typed input
module TypedInput =
        ///parses a regex and a string and then returns the groups of that regex
        val ( |ParseRegex|_| ) : string -> string -> string list option
        ///parses a string and returns either that string as an int, or nothing
        val ( |Integer|_| ) : string -> int option

        ///parses a string and returns the assoicated column(a,b,c...)
        val ( |ColToNum|_| ) : string -> int option
        ///reads in an a string of an algebraic move and parses it
        val ( |ReadInput|_| ) : string -> ((int * int) * (int * int)) option