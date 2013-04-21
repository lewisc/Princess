fsc=fsharpc --nologo --warn:5 -I:binaries
optimize=--optimize --tailcalls+ --crossoptimize+
debug=-g

all  : binaries/RandomPlayer.exe binaries/NetworkPlayer.exe

binaries/RandomPlayer.exe : binaries RandomPlayer.fs
	$(fsc) $(optimize) --standalone --out:binaries/RandomPlayer.exe -r:Elements.dll -r:ValidMoves.dll -r:Heuristics.dll RandomPlayer.fs

binaries/NetworkPlayer.exe : binaries RandomPlayer.fs 
	$(fsc) $(optimize) --standalone --out:binaries/NetworkPlayer.exe -r:IMCSConnection.dll  -r:Elements.dll -r:ValidMoves.dll -r:AlphaBeta.dll -r:DepthFirstSearch.dll -r:Heuristics.dll -r:MTDF.dll -r:TranspositionTable.dll -r:AlphaBetaID.dll -r:ServerConnection.dll NetPlayer.fs

binaries : connection/binaries
	mkdir binaries
	cp connection/binaries/* binaries

connection/binaries : 
	cd connection && $(MAKE)

clean : cleanconnection
	rm -rf binaries

cleanconnection : 
	cd connection && $(MAKE) clean
