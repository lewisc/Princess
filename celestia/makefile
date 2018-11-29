fsc=fsharpc --nologo --warn:5 -I:binaries
optimize=--optimize+ --tailcalls+ --crossoptimize+
debug=-g

all  : binaries/DepthFirstSearchRegression.dll binaries/MoveRegression.dll binaries/TestTypes.dll binaries/RegressionRunner.exe binaries/EndGame.dll \
       binaries/IMCSConnection.dll binaries/ServerConnection.dll \
       binaries/ZobristKeys.dll binaries/ValidMoves.dll binaries/TypedInput.dll binaries/BoardConstants.dll binaries/Elements.dll \
       binaries/AlphaBetaID.dll binaries/Heuristics.dll binaries/DepthFirstSearch.dll binaries/MTDF.dll binaries/Quiescence.dll binaries/AlphaBeta.dll \
       binaries/RandomPlayer.exe binaries/NetworkPlayer.exe binaries/TranspositionTable.dll

binaries/RandomPlayer.exe : binaries/MoveHelpers.dll binaries/Elements.dll binaries/ValidMoves.dll binaries/Heuristics.dll players/RandomPlayer.fs
	$(fsc) $(optimize) --standalone --out:binaries/RandomPlayer.exe -r:MoveHelpers.dll -r:Elements.dll -r:ValidMoves.dll -r:Heuristics.dll players/RandomPlayer.fs

binaries/NetworkPlayer.exe : binaries/IMCSConnection.dll binariesElements.dll binaries/ValidMoves.dll binaries/AlphaBeta.dll binaries/DepthFirstSearch.dll \
                             binaries/Heuristics.dll binaries/MTDF.dll binaries/TranspositionTable.dll binaries/AlphaBetaID.dll binaries/ServerConnection.dll players/NetPlayer.fs
	$(fsc) $(optimize) --standalone --out:binaries/NetworkPlayer.exe -r:IMCSConnection.dll  -r:Elements.dll -r:ValidMoves.dll -r:AlphaBeta.dll \
                       -r:DepthFirstSearch.dll -r:Heuristics.dll -r:MTDF.dll -r:TranspositionTable.dll -r:AlphaBetaID.dll -r:ServerConnection.dll players/NetPlayer.fs
	
binaries/DepthFirstSearchRegression.dll : binaries/IMCSConnection.dll binaries/Elements.dll binaries/Validmoves.dll binaries/AlphaBeta.dll \
                                          binaries/DepthFirstSearch.dll binaries/Heuristics.dll binaries/MTDF.dll binaries/TranspositionTable.dll \
                                          binaries/AlphaBetaID.dll binaries/ServerConnection.dll binaries/TestTypes.dll DepthFirstSearch.fs
	$(fsc) $(optimize) -a --out:binaries/DepthFirstSearchRegression.dll -r:IMCSConnection.dll  -r:Elements.dll \
                       -r:ValidMoves.dll -r:AlphaBeta.dll -r:DepthFirstSearch.dll -r:Heuristics.dll -r:MTDF.dll -r:TranspositionTable.dll -r:AlphaBetaID.dll -r:ServerConnection.dll -r:TestTypes.dll DepthFirstSearch.fs

binaries/MoveRegression.dll : binaries/Elements.dll binaries/ValidMoves.dll binaries/TestTypes.dll binaries/TypedInput.dll binaries/Resources.dll MoveRegression.fs
	$(fsc) $(optimize) -a--out:binaries/MoveRegression.dll -r:Elements.dll -r:ValidMoves.dll -r:TestTypes.dll -r:TypedInput.dll -r:Resources.dll MoveRegression.fs

binaries/EndGame.dll : binaries/Heuristics.dll binaries/Elements.dll binaries/ValidMoves.dll binaries/AlphaBeta.dll binaries/BoardConstants.dll endgame/EndGameDB.fs
	$(fsc) $(optimize) -a  --out:binaries/EndGame.dll -r:Heuristics.dll -r:Elements.dll -r:ValidMoves.dll  -r:AlphaBeta.dll -r:BoardConstants.dll endgame/EndGameDB.fs

binaries/RegressionRunner.exe : binaries/IMCSConnection.dll  binaries/Elements.dll binaries/ValidMoves.dll binaries/AlphaBeta.dll binaries/DepthFirstSearch.dll binaries/Heuristics.dll binaries/MTDF.dll binaries/TranspositionTable.dll binaries/AlphaBetaID.dll binaries/ServerConnection.dll binaries/TestTypes.dll binaries/MoveRegression.dll binaries/TypedInput.dll binaries/DepthFirstSearchRegression.dll binaries/BoardConstants.dll binaries/Resources.dll binaries/ZobristKeys.dll RegressionRunner.fs 
	$(fsc) $(optimize) --standalone --out:binaries/RegressionRunner.exe -r:IMCSConnection.dll  -r:Elements.dll -r:ValidMoves.dll -r:AlphaBeta.dll -r:DepthFirstSearch.dll -r:Heuristics.dll -r:MTDF.dll -r:TranspositionTable.dll -r:AlphaBetaID.dll -r:ServerConnection.dll -r:TestTypes.dll -r:MoveRegression.dll -r:TypedInput.dll -r:DepthFirstSearchRegression.dll -r:BoardConstants.dll -r:Resources.dll -r:ZobristKeys.dll RegressionRunner.fs 

binaries/ZobristKeys.dll : binaries/Elements.dll binaries binaries/BoardConstants.dll ZobristKeys.fs
	$(fsc) $(optimize) -a --out:binaries/ZobristKeys.dll -r:Elements.dll -r:BoardConstants.dll ZobristKeys.fs

binaries/ValidMoves.dll : binaries binaries/TypedInput.dll ValidMoves.fs binaries/Elements.dll binaries/ZobristKeys.dll binaries/BoardConstants.dll binaries/MoveHelpers.dll binaries/MoveGen.dll
	$(fsc) $(optimize) -a --out:binaries/ValidMoves.dll -r:TypedInput.dll -r:Elements.dll -r:ZobristKeys.dll -r:BoardConstants.dll -r:MoveHelpers.dll -r:MoveGen.dll ValidMoves.fs

binaries/MoveGen.dll : binaries/Elements.dll binaries MoveGen.fs binaries/BoardConstants.dll binaries/ZobristKeys.dll binaries/TypedInput.dll binaries/MoveHelpers.dll
	$(fsc) $(optimize) -a --out:binaries/MoveGen.dll -r:MoveHelpers.dll -r:Elements.dll -r:BoardConstants.dll -r:ZobristKeys.dll -r:TypedInput.dll MoveGen.fs

binaries/MoveHelpers.dll : binaries/Elements.dll binaries MoveHelpers.fs binaries/BoardConstants.dll binaries/ZobristKeys.dll binaries/TypedInput.dll
	$(fsc) $(optimize) -a --out:binaries/MoveHelpers.dll -r:Elements.dll -r:BoardConstants.dll -r:ZobristKeys.dll -r:TypedInput.dll MoveHelpers.fs

binaries/BoardConstants.dll : binaries/Elements.dll binaries BoardConstants.fs
	$(fsc) $(optimize) -a --out:binaries/BoardConstants.dll -r:Elements.dll BoardConstants.fs

binaries/TranspositionTable.dll  : binaries TranspositionTable.fs 
	$(fsc) $(optimize) --out:binaries/TranspositionTable.dll -a -r:Elements.dll -r:ValidMoves.dll -r:ZobristKeys.dll TranspositionTable.fs

binaries/ServerConnection.dll : binaries ServerConnection.fs binaries/IMCSConnection.dll
	$(fsc) $(optimize) -a --out:binaries/ServerConnection.dll -r:Elements.dll -r:MoveHelpers.dll  -r:IMCSConnection.dll -r:ValidMoves.dll -r:TypedInput.dll ServerConnection.fs

binaries/Heuristics.dll  : binaries Heuristics.fs
	$(fsc) $(optimize) --out:binaries/Heuristics.dll -a  -r:Elements.dll -r:ValidMoves.dll -r:MoveHelpers.dll Heuristics.fs

binaries/Quiescence.dll  : binaries Quiescence.fs
	$(fsc) $(optimize) --out:binaries/Quiescence.dll -a -r:MoveHelpers.dll -r:Elements.dll -r:ValidMoves.dll Quiescence.fs

binaries/AlphaBeta.dll  : binaries AlphaBeta2.fs binaries/DepthFirstSearch.dll binaries/Quiescence.dll
	$(fsc) $(optimize) --out:binaries/AlphaBeta.dll -a -r:MoveHelpers.dll -r:DepthFirstSearch.dll -r:Elements.dll -r:Quiescence.dll -r:ValidMoves.dll -r:TranspositionTable.dll AlphaBeta2.fs

binaries/AlphaBetaID.dll  : binaries AlphaBetaID.fs binaries/AlphaBeta.dll
	$(fsc) $(optimize) --out:binaries/AlphaBetaID.dll -a  -r:MoveHelpers.dll -r:DepthFirstSearch.dll -r:Elements.dll -r:Quiescence.dll -r:ValidMoves.dll -r:AlphaBeta.dll -r:TranspositionTable.dll AlphaBetaID.fs

binaries/DepthFirstSearch.dll  : binaries DepthFirstSearch.fs binaries/Elements.dll
	$(fsc) $(optimize) --out:binaries/DepthFirstSearch.dll -a -r:Elements.dll -r:ValidMoves.dll -r:MoveHelpers.dll DepthFirstSearch.fs

binaries/MTDF.dll  : MTDF.fs binaries/AlphaBeta.dll
	$(fsc) $(optimize) --out:binaries/MTDF.dll -a -r:MoveHelpers.dll -r:AlphaBeta.dll -r:Elements.dll -r:ValidMoves.dll -r:BoardConstants.dll -r:TranspositionTable.dll -r:DepthFirstSearch.dll MTDF.fs

binaries/Elements.dll : Elements.fs
	$(fsc) $(optimize) -a --out:binaries/Elements.dll Elements.fs

binaries/TestTypes.dll : TestTypes.fs
	$(fsc) $(optimize) -a --out:binaries/TestTypes.dll  TestTypes.fs

binaries/TypedInput.dll : board/TypedInput.fs
	$(fsc) $(optimize) -a --out:binaries/TypedInput.dll board/TypedInput.fs

binaries/IMCSConnection.dll : connection/IMCSConnection.fs
	$(fsc) $(optimize) -a --out:binaries/IMCSConnection.dll connection/IMCSConnection.fs

clean :
	rm -rf binaries/*.dll
	rm -rf binaries/*.exe
