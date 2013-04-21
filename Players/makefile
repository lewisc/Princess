fsc=fsharpc --nologo --warn:5
optimize=--optimize --tailcalls+ --crossoptimize+
debug=-g

../output/RandomPlayer.exe ../output/NetworkPlayer.exe  : binaries/RandomPlayer.exe binaries/NetworkPlayer.exe
	cp binaries/* ../output

binaries/RandomPlayer.exe : binaries RandomPlayer.fs ../output/Elements.dll ../output/ValidMoves.dll ../output/Heuristics.dll
	$(fsc) $(optimize) --standalone --out:binaries/RandomPlayer.exe -I:../output/ -r:Elements.dll -r:ValidMoves.dll -r:Heuristics.dll RandomPlayer.fs

binaries/NetworkPlayer.exe : binaries RandomPlayer.fs ../output/Elements.dll ../output/ValidMoves.dll ../output/IMCSConnection.dll ../output/AlphaBeta.dll ../output/DepthFirstSearch.dll ../output/Heuristics.dll ../output/MTDF.dll ../output/TranspositionTable.dll ../output/AlphaBetaID.dll ../output/ServerConnection.dll
	$(fsc) $(optimize) --standalone --out:binaries/NetworkPlayer.exe --lib:../output -r:IMCSConnection.dll  -r:Elements.dll -r:ValidMoves.dll -r:AlphaBeta.dll -r:DepthFirstSearch.dll -r:Heuristics.dll -r:MTDF.dll -r:TranspositionTable.dll -r:AlphaBetaID.dll -r:ServerConnection.dll NetPlayer.fs

binaries :
	mkdir binaries

clean :
	rm -rf binaries
   


