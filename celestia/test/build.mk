
test_all : $(libs)/DepthFirstSearchRegression.dll \
           $(libs)/MoveRegression.dll \
           $(libs)/RegressionRunner.exe \
           $(libs)/TestTypes.dll

$(libs)/RegressionRunner.exe : $(libs)/IMCSConnection.dll \
                               $(libs)/Primitives.dll \
                               $(libs)/ValidMoves.dll \
                               $(libs)/AlphaBeta.dll \
                               $(libs)/DepthFirstSearch.dll \
                               $(libs)/Heuristics.dll \
                               $(libs)/MTDF.dll \
                               $(libs)/TranspositionTable.dll \
                               $(libs)/AlphaBetaID.dll \
                               $(libs)/ServerConnection.dll \
                               $(libs)/TestTypes.dll \
                               $(libs)/MoveRegression.dll \
                               $(libs)/TypedInput.dll \
                               $(libs)/DepthFirstSearchRegression.dll \
                               $(libs)/ZobristKeys.dll \
                               test/RegressionRunner.fs 
	$(fsc) $(optimize) -r:IMCSConnection.dll  \
                       -r:Primitives.dll \
                       -r:ValidMoves.dll \
                       -r:AlphaBeta.dll \
                       -r:DepthFirstSearch.dll \
                       -r:Heuristics.dll \
                       -r:MTDF.dll  \
                       -r:TranspositionTable.dll \
                       -r:AlphaBetaID.dll  \
                       -r:ServerConnection.dll \
                       -r:TestTypes.dll \
                       -r:MoveRegression.dll \
                       -r:TypedInput.dll \
                       -r:DepthFirstSearchRegression.dll \
                       -r:ZobristKeys.dll \
                       --standalone \
                       --out:$(bin)/RegressionRunner.exe \
                       test/RegressionRunner.fs 

$(libs)/DepthFirstSearchRegression.dll : $(libs)/IMCSConnection.dll \
                                         $(libs)/Primitives.dll \
                                         $(libs)/ValidMoves.dll \
                                         $(libs)/AlphaBeta.dll \
                                         $(libs)/DepthFirstSearch.dll \
                                         $(libs)/Heuristics.dll \
                                         $(libs)/MTDF.dll \
                                         $(libs)/TranspositionTable.dll \
                                         $(libs)/AlphaBetaID.dll \
                                         $(libs)/ServerConnection.dll \
                                         $(libs)/TestTypes.dll \
                                         test/DepthFirstSearch.fs
	$(fsc) $(optimize) -r:IMCSConnection.dll \
                       -r:Primitives.dll \
                       -r:ValidMoves.dll \
                       -r:AlphaBeta.dll \
                       -r:DepthFirstSearch.dll \
                       -r:Heuristics.dll \
                       -r:MTDF.dll \
                       -r:TranspositionTable.dll \
                       -r:AlphaBetaID.dll \
                       -r:ServerConnection.dll \
                       -r:TestTypes.dll \
                       -a \
                       --out:$(libs)/DepthFirstSearchRegression.dll \
                       test/DepthFirstSearch.fs

$(libs)/MoveRegression.dll : $(libs)/Primitives.dll \
                             $(libs)/ValidMoves.dll \
                             $(libs)/TestTypes.dll \
                             $(libs)/TypedInput.dll \
                             test/MoveRegression.fs
	$(fsc) $(optimize) -r:Primitives.dll \
                       -r:ValidMoves.dll \
                       -r:TestTypes.dll \
                       -r:TypedInput.dll \
                       -a \
                       --out:$(libs)/MoveRegression.dll \
                       test/MoveRegression.fs

$(libs)/TestTypes.dll : test/TestTypes.fs
	$(fsc) $(optimize) -a \
                       --out:$(libs)/TestTypes.dll \
                       test/TestTypes.fs
