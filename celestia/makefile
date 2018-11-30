fsc=fsharpc --nologo --warn:5 -I:binaries
optimize=--optimize+ --tailcalls+ --crossoptimize+
debug=-g

all  : binaries/DepthFirstSearchRegression.dll \
       binaries/MoveRegression.dll \
       binaries/TestTypes.dll \
       binaries/RegressionRunner.exe \
       binaries/EndGame.dll \
       binaries/IMCSConnection.dll \
       binaries/ServerConnection.dll \
       binaries/ZobristKeys.dll \
       binaries/ValidMoves.dll \
       binaries/TypedInput.dll \
       binaries/BoardConstants.dll \
       binaries/Elements.dll \
       binaries/AlphaBetaID.dll \
       binaries/Heuristics.dll \
       binaries/DepthFirstSearch.dll \
       binaries/MTDF.dll \
       binaries/Quiescence.dll \
       binaries/AlphaBeta.dll \
       binaries/RandomPlayer.exe \
       binaries/NetworkPlayer.exe \
       binaries/TranspositionTable.dll

# Main executables
binaries/RandomPlayer.exe : binaries/MoveHelpers.dll \
                            binaries/Elements.dll \
                            binaries/ValidMoves.dll \
                            binaries/Heuristics.dll \
                            players/RandomPlayer.fs
	$(fsc) $(optimize) -r:MoveHelpers.dll \
                       -r:Elements.dll \
                       -r:ValidMoves.dll \
                       -r:Heuristics.dll\
                       --standalone \
                       --out:binaries/RandomPlayer.exe \
                       players/RandomPlayer.fs

binaries/NetworkPlayer.exe : binaries/IMCSConnection.dll \
                             binariesElements.dll \
                             binaries/ValidMoves.dll \
                             binaries/AlphaBeta.dll \
                             binaries/DepthFirstSearch.dll \
                             binaries/Heuristics.dll \
                             binaries/MTDF.dll \
                             binaries/TranspositionTable.dll \
                             binaries/AlphaBetaID.dll \
                             binaries/ServerConnection.dll \
                             players/NetPlayer.fs
	$(fsc) $(optimize) -r:IMCSConnection.dll \
                       -r:Elements.dll \
                       -r:ValidMoves.dll \
                       -r:AlphaBeta.dll \
                       -r:DepthFirstSearch.dll \
                       -r:Heuristics.dll \
                       -r:MTDF.dll \
                       -r:TranspositionTable.dll \
                       -r:AlphaBetaID.dll \
                       -r:ServerConnection.dll \
                       --standalone \
                       --out:binaries/NetworkPlayer.exe \
                       players/NetPlayer.fs

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
                       -r:Resources.dll \
                       --standalone \
                       --out:binaries/RegressionRunner.exe \
                       RegressionRunner.fs 


# top level dll's
binaries/DepthFirstSearchRegression.dll : binaries/IMCSConnection.dll \
                                          binaries/Elements.dll \
                                          binaries/Validmoves.dll \
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
                       -r:Resources.dll \
                       -a \
                       --out:binaries/MoveRegression.dll \
                       MoveRegression.fs


# Endgame Db dll's
binaries/EndGame.dll : binaries/Heuristics.dll \
                       binaries/Elements.dll \
                       binaries/ValidMoves.dll \
                       binaries/AlphaBeta.dll \
                       binaries/BoardConstants.dll \
                       endgame/EndGameDB.fs
	$(fsc) $(optimize) -r:binaries/Heuristics.dll \
                       -r:binaries/Elements.dll \
                       -r:binaries/ValidMoves.dll  \
                       -r:binaries/AlphaBeta.dll \
                       -r:binaries/BoardConstants.dll \
                       -a \
                       --out:binaries/EndGame.dll \
                       endgame/EndGameDB.fs



# board dll's
binaries/ZobristKeys.dll : binaries/Elements.dll \
                           binaries/BoardConstants.dll \
                           board/ZobristKeys.fs
	$(fsc) $(optimize) -r:binaries/Elements.dll \
                       -r:binaries/BoardConstants.dll  \
                       -a \
                       --out:binaries/ZobristKeys.dll \
                       ZobristKeys.fs

binaries/ValidMoves.dll : binaries/TypedInput.dll \
                          binaries/Elements.dll \
                          binaries/ZobristKeys.dll \
                          binaries/BoardConstants.dll \
                          binaries/MoveHelpers.dll \
                          binaries/MoveGen.dll \
                          board/ValidMoves.fs 
	$(fsc) $(optimize) -r:TypedInput.dll \
                       -r:Elements.dll \
                       -r:ZobristKeys.dll \
                       -r:BoardConstants.dll \
                       -r:MoveHelpers.dll \
                       -r:MoveGen.dll \
                       -a \
                       --out:binaries/ValidMoves.dll \
                       board/ValidMoves.fs

binaries/MoveGen.dll : binaries/Elements.dll \
                       binaries/BoardConstants.dll \
                       binaries/ZobristKeys.dll \
                       binaries/TypedInput.dll \
                       binaries/MoveHelpers.dll \
                       board/MoveGen.fs
	$(fsc) $(optimize) -r:MoveHelpers.dll \
                       -r:Elements.dll \
                       -r:BoardConstants.dll \
                       -r:ZobristKeys.dll \
                       -r:TypedInput.dll \
                        -a \
                       --out:binaries/MoveGen.dll \
                       board/MoveGen.fs

binaries/MoveHelpers.dll : binaries/Elements.dll \
                           binaries/BoardConstants.dll \
                           binaries/ZobristKeys.dll \
                           binaries/TypedInput.dll
                           MoveHelpers.fs \
	$(fsc) $(optimize) -r:binaries/Elements.dll \
                       -r:binaries/BoardConstants.dll \
                       -r:binaries/ZobristKeys.dll \
                       -r:binaries/TypedInput.dll \
                       -a \
                       --out:binaries/MoveHelpers.dll \
                       MoveHelpers.fs

binaries/BoardConstants.dll : binaries/Elements.dll \
                              BoardConstants.fs
	$(fsc) $(optimize) -r:binaries/Elements.dll \
                       -a \
                       --out:binaries/BoardConstants.dll \
                       BoardConstants.fs

binaries/TranspositionTable.dll : binaries/Elements.dll \
                                   binaries/ValidMoves.dll \
                                   binaries/ZobristKeys.dll \
                                   TranspositionTable.fs 
	$(fsc) $(optimize) -r:binaries/Elements.dll \
                       -r:binaries/ValidMoves.dll \
                       -r:binaries/ZobristKeys.dll \
                       -a \
                       --out:binaries/TranspositionTable.dll \
                       TranspositionTable.fs

binaries/ServerConnection.dll : binaries/Elements.dll \
                                binaries/MovieHelpers.dll \
                                binaries/IMCSConnection.dll \
                                binaries/ValidMoves.dll \
                                binaries/TypedInput.dll \
                                ServerConnection.fs 
	$(fsc) $(optimize) -r:binaries/Elements.dll \
                       -r:binaries/MoveHelpers.dll \
                       -r:binaries/IMCSConnection.dll \
                       -r:binaries/ValidMoves.dll \
                       -r:binaries/TypedInput.dll \
                        -a \
                       --out:binaries/ServerConnection.dll \
                       ServerConnection.fs

binaries/Heuristics.dll : binaries/Elements.dll \
                           binaries/ValidMoves.dll \
                           binaries/MoveHelpers.dll \
                           binaries/Heuristics.fs
	$(fsc) $(optimize) -r:binaries/Elements.dll \
                       -r:binaries/ValidMoves.dll \
                       -r:binaries/MoveHelpers.dll \
                       -a \
                       --out:binaries/Heuristics.dll \
                       Heuristics.fs

binaries/Quiescence.dll : binaries/MoveHelpers.dll \
                           binaries/Elements.dll \
                           binaries/ValidMoves.dll \
                           Quiescence.fs
	$(fsc) $(optimize) -r:binaries/MoveHelpers.dll \
                       -r:binaries/Elements.dll \
                       -r:binaries/ValidMoves.dll \
                       -a \
                       --out:binaries/Quiescence.dll \
                       Quiescence.fs

binaries/AlphaBeta.dll : binaries/MoveHelpers.dll \
                          binaries/DepthFirstSearch.dll \
                          binaries/Elements.dll Quiescence.dll \
                          binaries/ValidMoves.dll \
                          binaries/TranspositionTable.dll \
                          AlphaBeta2.fs
	$(fsc) $(optimize) -r:binaries/MoveHelpers.dll \
                       -r:binaries/DepthFirstSearch.dll \
                       -r:binaries/Elements.dll \
                       -r:binaries/Quiescence.dll \
                       -r:binaries/ValidMoves.dll \
                       -r:binaries/TranspositionTable.dll \
                       -a \
                       --out:binaries/AlphaBeta.dll \
                       AlphaBeta2.fs

binaries/AlphaBetaID.dll : binaries/MoveHelpers.dll \
                           binaries/DepthFirstSearch.dll \
                           binaries/Elements.dll \
                           binaries/Quiescence.dll \
                           binaries/ValidMoves.dll \
                           binaries/AlphaBeta.dll \
                           binaries/TranspositionTable.dll \
                           AlphaBetaID.fs
	$(fsc) $(optimize) -r:binaries/MoveHelpers.dll \
                       -r:binaries/DepthFirstSearch.dll \
                       -r:binaries/Elements.dll \
                       -r:binaries/Quiescence.dll \
                       -r:binaries/ValidMoves.dll \
                       -r:binaries/AlphaBeta.dll \
                       -r:binaries/TranspositionTable.dll \
                       -a \
                        --out:binaries/AlphaBetaID.dll \
                       AlphaBetaID.fs

binaries/DepthFirstSearch.dll : binaries/Elements.dll \
                                binaries/ValidMoves.dll \
                                binaries/MoveHelpers.dll \
                                DepthFirstSearch.fs
	$(fsc) $(optimize) -r:binaries/Elements.dll \
                       -r:binaries/ValidMoves.dll \
                       -r:binaries/MoveHelpers.dll \
                       -a \
                       -out:binaries/DepthFirstSearch.dll \
                       DepthFirstSearch.fs

binaries/MTDF.dll : binaries/MoveHelpers.dll \
                    binaries/AlphaBeta.dll \
                    binaries/Elements.dll \
                    binaries/ValidMoves.dll \
                    binaries/BoardConstants.dll \
                    binaries/TranspositionTable.dll \
                    binaries/DepthFirstSearch.dll \
                    MTDF.fs
	$(fsc) $(optimize) -r:MoveHelpers.dll \
                       -r:AlphaBeta.dll \
                       -r:Elements.dll \
                       -r:ValidMoves.dll \
                       -r:BoardConstants.dll \
                       -r:TranspositionTable.dll \
                       -r:DepthFirstSearch.dll \
                       -a \
                       --out:binaries/MTDF.dll \
                       MTDF.fs

binaries/Elements.dll : Elements.fs
	$(fsc) $(optimize) -a \
                       --out:binaries/Elements.dll \
                       Elements.fs

binaries/TestTypes.dll : TestTypes.fs
	$(fsc) $(optimize) -a \
                       --out:binaries/TestTypes.dll \
                       TestTypes.fs

binaries/TypedInput.dll : board/TypedInput.fs
	$(fsc) $(optimize) -a \
                       --out:binaries/TypedInput.dll \
                       board/TypedInput.fs

binaries/IMCSConnection.dll : connection/IMCSConnection.fs
	$(fsc) $(optimize) -a \
                       --out:binaries/IMCSConnection.dll \
                       connection/IMCSConnection.fs

clean :
	rm -rf binaries/*.dll
	rm -rf binaries/*.exe
