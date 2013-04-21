fsc=fsharpc --nologo --warn:5
optimize=--optimize --tailcalls+ --crossoptimize+
debug=-g

../output/DepthFirstSearchRegression.dll ../output/MoveRegression.dll ../output/TestTypes.dll ../output/RegressionRunner.exe  : binaries/DepthFirstSearchRegression.exe binaries/MoveRegression.dll binaries/TestTypes.dll binaries/RegressionRunner.exe
	cp binaries/* ../output

binaries/MoveRegression.dll : binaries binaries/TestTypes.dll ../output/Elements.dll ../output/ValidMoves.dll MoveRegression.fs ../output/TypedInput.dll ../output/Resources.dll
	$(fsc) $(optimize) -a -I:../output -r:Elements.dll -r:ValidMoves.dll -r:binaries/TestTypes.dll --out:binaries/MoveRegression.dll -r:TypedInput.dll  -r:Resources.dll MoveRegression.fs

binaries/TestTypes.dll : binaries TestTypes.fs
	$(fsc) $(optimize) -a --out:binaries/TestTypes.dll  TestTypes.fs

binaries/RegressionRunner.exe : binaries RegressionRunner.fs ../output/Elements.dll ../output/ValidMoves.dll ../output/IMCSConnection.dll ../output/AlphaBeta.dll ../output/DepthFirstSearch.dll ../output/Heuristics.dll ../output/MTDF.dll ../output/TranspositionTable.dll ../output/AlphaBetaID.dll ../output/ServerConnection.dll binaries/TestTypes.dll binaries/MoveRegression.dll ../output/TypedInput.dll ../output/Resources.dll binaries/DepthFirstSearchRegression.dll ../output/ZobristKeys.dll ../output/BoardConstants.dll
	$(fsc) $(optimize) --standalone --out:binaries/RegressionRunner.exe --lib:../output -r:IMCSConnection.dll  -r:Elements.dll -r:ValidMoves.dll -r:AlphaBeta.dll -r:DepthFirstSearch.dll -r:Heuristics.dll -r:MTDF.dll -r:TranspositionTable.dll -r:AlphaBetaID.dll -r:ServerConnection.dll -r:binaries/TestTypes.dll -r:binaries/MoveRegression.dll -r:TypedInput.dll -r:binaries/DepthFirstSearchRegression.dll -r:BoardConstants.dll -r:Resources.dll -r:ZobristKeys.dll RegressionRunner.fs 

binaries/DepthFirstSearchRegression.dll : binaries DepthFirstSearch.fs ../output/Elements.dll ../output/ValidMoves.dll ../output/IMCSConnection.dll ../output/AlphaBeta.dll ../output/DepthFirstSearch.dll ../output/Heuristics.dll ../output/MTDF.dll ../output/TranspositionTable.dll ../output/AlphaBetaID.dll ../output/ServerConnection.dll binaries/TestTypes.dll
	$(fsc) $(optimize) -a --out:binaries/DepthFirstSearchRegression.dll --lib:../output -r:IMCSConnection.dll  -r:Elements.dll -r:ValidMoves.dll -r:AlphaBeta.dll -r:DepthFirstSearch.dll -r:Heuristics.dll -r:MTDF.dll -r:TranspositionTable.dll -r:AlphaBetaID.dll -r:ServerConnection.dll -r:binaries/TestTypes.dll DepthFirstSearch.fs

binaries :
	mkdir binaries

clean :
	rm -rf binaries
   


