fsc=fsharpc --nologo --warn:5 -I:binaries
optimize=--optimize+ --tailcalls+ --crossoptimize+
debug=-g


all  : connection_all \
       search_all \
       board_all \
       endgamedb_all \
       players_all \
       tables_all \
	   global_all

include connection/build.mk \
        search/build.mk \
        board/build.mk \
        endgamedb/build.mk \
        players/build.mk \
        tables/build.mk

global_all : binaries/DepthFirstSearchRegression.dll \
             binaries/MoveRegression.dll \
             binaries/RegressionRunner.exe \
             binaries/TestTypes.dll

binaries/RegressionRunner.exe : binaries/IMCSConnection.dll \
                                binaries/Elements.dll \
                                binaries/ValidMoves.dll \
                                binaries/AlphaBeta.dll \
                                binaries/DepthFirstSearch.dll \
                                binaries/Heuristics.dll \
                                binaries/MTDF.dll \
                                binaries/TranspositionTable.dll \
                                binaries/AlphaBetaID.dll \
                                binaries/ServerConnection.dll \
                                binaries/TestTypes.dll \
                                binaries/MoveRegression.dll \
                                binaries/TypedInput.dll \
                                binaries/DepthFirstSearchRegression.dll \
                                binaries/BoardConstants.dll \
                                binaries/ZobristKeys.dll \
                                RegressionRunner.fs 
	$(fsc) $(optimize) -r:binaries/IMCSConnection.dll  \
                       -r:binaries/Elements.dll \
                       -r:binaries/ValidMoves.dll \
                       -r:binaries/AlphaBeta.dll \
                       -r:binaries/DepthFirstSearch.dll \
                       -r:binaries/Heuristics.dll \
                       -r:binaries/MTDF.dll  \
                       -r:binaries/TranspositionTable.dll \
                       -r:binaries/AlphaBetaID.dll  \
                       -r:binaries/ServerConnection.dll \
                       -r:binaries/TestTypes.dll \
                       -r:binaries/MoveRegression.dll \
                       -r:binaries/TypedInput.dll \
                       -r:binaries/DepthFirstSearchRegression.dll \
                       -r:binaries/BoardConstants.dll \
                       -r:binaries/ZobristKeys.dll \
                       --standalone \
                       --out:binaries/RegressionRunner.exe \
                       RegressionRunner.fs 

binaries/DepthFirstSearchRegression.dll : binaries/IMCSConnection.dll \
                                          binaries/Elements.dll \
                                          binaries/ValidMoves.dll \
                                          binaries/AlphaBeta.dll \
                                          binaries/DepthFirstSearch.dll \
                                          binaries/Heuristics.dll \
                                          binaries/MTDF.dll \
                                          binaries/TranspositionTable.dll \
                                          binaries/AlphaBetaID.dll \
                                          binaries/ServerConnection.dll \
                                          binaries/TestTypes.dll \
                                          DepthFirstSearch.fs
	$(fsc) $(optimize) -r:binaries/IMCSConnection.dll \
                       -r:binaries/Elements.dll \
                       -r:binaries/ValidMoves.dll \
                       -r:binaries/AlphaBeta.dll \
                       -r:binaries/DepthFirstSearch.dll \
                       -r:binaries/Heuristics.dll \
                       -r:binaries/MTDF.dll \
                       -r:binaries/TranspositionTable.dll \
                       -r:binaries/AlphaBetaID.dll \
                       -r:binaries/ServerConnection.dll \
                       -r:binaries/TestTypes.dll \
                       -a \
                       --out:binaries/DepthFirstSearchRegression.dll \
                       DepthFirstSearch.fs

binaries/MoveRegression.dll : binaries/Elements.dll \
                              binaries/ValidMoves.dll \
                              binaries/TestTypes.dll \
                              binaries/TypedInput.dll \
                              MoveRegression.fs
	$(fsc) $(optimize) -r:binaries/Elements.dll \
                       -r:binaries/ValidMoves.dll \
                       -r:binaries/TestTypes.dll \
                       -r:binaries/TypedInput.dll \
                       -a \
                       --out:binaries/MoveRegression.dll \
                       MoveRegression.fs

binaries/TestTypes.dll : TestTypes.fs
	$(fsc) $(optimize) -a \
                       --out:binaries/TestTypes.dll \
                       TestTypes.fs

clean :
	rm -rf binaries/*.dll
	rm -rf binaries/*.exe
