#r @"BoardPieces/bin/Release/BoardPieces.dll"
#r @"IMCSConnection/bin/Release/IMCSConnection.dll"

open IMCSServer

let mcecs = new IMCSConnection("svcs.cs.pdx.edu", 3589);;

connect mcecs;;

bindName mcecs "princess" "1234";;

getList mcecs;;
