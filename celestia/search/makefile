fsc=fsharpc --nologo -I:binaries --warn:5
optimize=--optimize --tailcalls+ --crossoptimize+
debug=-g

all : binaries/AlphaBetaID.dll binaries/Heuristics.dll binaries/DepthFirstSearch.dll binaries/MTDF.dll binaries/Quiescence.dll binaries/AlphaBeta.dll
	

binaries/Heuristics.dll  : binaries Heuristics.fs
	$(fsc) $(optimize) --out:binaries/Heuristics.dll -a  -r:Elements.dll -r:ValidMoves.dll -r:MoveHelpers.dll Heuristics.fs

binaries/Quiescence.dll  : binaries Quiescence.fs
	$(fsc) $(optimize) --out:binaries/Quiescence.dll -a -r:MoveHelpers.dll -r:Elements.dll -r:ValidMoves.dll Quiescence.fs

binaries/AlphaBeta.dll  : binaries AlphaBeta2.fs binaries/DepthFirstSearch.dll binaries/Quiescence.dll
	$(fsc) $(optimize) --out:binaries/AlphaBeta.dll -a -r:MoveHelpers.dll -r:DepthFirstSearch.dll -r:Elements.dll -r:Quiescence.dll -r:ValidMoves.dll -r:TranspositionTable.dll AlphaBeta2.fs

binaries/AlphaBetaID.dll  : binaries AlphaBetaID.fs binaries/AlphaBeta.dll
	$(fsc) $(optimize) --out:binaries/AlphaBetaID.dll -a  -r:MoveHelpers.dll -r:DepthFirstSearch.dll -r:Elements.dll -r:Quiescence.dll -r:ValidMoves.dll -r:AlphaBeta.dll -r:TranspositionTable.dll AlphaBetaID.fs

binaries/DepthFirstSearch.dll  : binaries DepthFirstSearch.fs 
	$(fsc) $(optimize) --out:binaries/DepthFirstSearch.dll -a -r:Elements.dll -r:ValidMoves.dll -r:MoveHelpers.dll DepthFirstSearch.fs

binaries/MTDF.dll  : binaries MTDF.fs binaries/AlphaBeta.dll
	$(fsc) $(optimize) --out:binaries/MTDF.dll -a -r:MoveHelpers.dll -r:AlphaBeta.dll -r:Elements.dll -r:ValidMoves.dll -r:BoardConstants.dll -r:TranspositionTable.dll -r:DepthFirstSearch.dll MTDF.fs

binaries :
	mkdir -p binaries

clean :
	rm -rf binaries
