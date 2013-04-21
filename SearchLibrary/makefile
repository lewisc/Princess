fsc=fsharpc --nologo
optimize=--optimize --tailcalls+ --crossoptimize+
debug=-g

../output/MTDF.dll ../output/DepthFirstSearch.dll  ../output/Quiescence.dll ../output/AlphaBeta.dll ../output/Heuristics.dll ../output/AlphaBetaID.dll: binaries/AlphaBetaID.dll binaries/Heuristics.dll binaries/DepthFirstSearch.dll binaries/MTDF.dll binaries/Quiescence.dll binaries/AlphaBeta.dll
	cp binaries/* ../output

binaries/Heuristics.dll  : binaries Heuristics.fs ../output/Elements.dll ../output/ValidMoves.dll  binaries/DepthFirstSearch.dll
	$(fsc) $(optimize) --out:binaries/Heuristics.dll -a  -r:../output/Elements.dll -r:../output/ValidMoves.dll -r:binaries/DepthFirstSearch.dll Heuristics.fs

binaries/Quiescence.dll  : binaries Quiescence.fs ../output/Elements.dll ../output/ValidMoves.dll  
	$(fsc) $(optimize) --out:binaries/Quiescence.dll -a  -r:../output/Elements.dll -r:../output/ValidMoves.dll Quiescence.fs

binaries/AlphaBeta.dll  : binaries AlphaBeta2.fs ../output/Elements.dll ../output/ValidMoves.dll  binaries/Quiescence.dll binaries/DepthFIrstSearch.dll ../output/TranspositionTable.dll
	$(fsc) $(optimize) --out:binaries/AlphaBeta.dll -a -r:binaries/DepthFirstSearch.dll -r:../output/Elements.dll -r:binaries/Quiescence.dll -r:../output/ValidMoves.dll -r:../output/TranspositionTable.dll AlphaBeta2.fs

binaries/AlphaBetaID.dll  : binaries AlphaBetaID.fs ../output/Elements.dll ../output/ValidMoves.dll binaries/DepthFirstSearch.dll  binaries/Quiescence.dll binaries/AlphaBeta.dll ../output/TranspositionTable.dll
	$(fsc) $(optimize) --out:binaries/AlphaBetaID.dll -a -r:binaries/DepthFirstSearch.dll -r:../output/Elements.dll -r:binaries/Quiescence.dll -r:../output/ValidMoves.dll -r:binaries/AlphaBeta.dll -r:../output/TranspositionTable.dll AlphaBetaID.fs

binaries/DepthFirstSearch.dll  : binaries DepthFirstSearch.fs ../output/Elements.dll ../output/ValidMoves.dll  
	$(fsc) $(optimize) --out:binaries/DepthFirstSearch.dll -a -r:../output/Elements.dll -r:../output/ValidMoves.dll DepthFirstSearch.fs

binaries/MTDF.dll  : binaries MTDF.fs ../output/Elements.dll ../output/ValidMoves.dll  binaries/AlphaBeta.dll ../output/BoardConstants.dll ../output/TranspositionTable.dll binaries/DepthFirstSearch.dll
	$(fsc) $(optimize) --out:binaries/MTDF.dll -a -r:binaries/AlphaBeta.dll -r:../output/Elements.dll -r:../output/ValidMoves.dll -r:../output/BoardConstants.dll -r:../output/TranspositionTable.dll -r:binaries/DepthFirstSearch.dll MTDF.fs

binaries :
	mkdir binaries

clean :
	rm -rf binaries
   


