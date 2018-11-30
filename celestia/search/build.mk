
search_all : binaries/DepthFirstSearch.dll \
             binaries/AlphaBetaID.dll \
             binaries/Heuristics.dll \
             binaries/Quiescence.dll \
             binaries/MTDF.dll \
             binaries/AlphaBeta.dll

binaries/Heuristics.dll : binaries/Elements.dll \
                           binaries/ValidMoves.dll \
                           binaries/MoveHelpers.dll \
                           search/Heuristics.fs
	$(fsc) $(optimize) -r:binaries/Elements.dll \
                       -r:binaries/ValidMoves.dll \
                       -r:binaries/MoveHelpers.dll \
                       -a \
                       --out:binaries/Heuristics.dll \
                       search/Heuristics.fs

binaries/Quiescence.dll : binaries/MoveHelpers.dll \
                           binaries/Elements.dll \
                           binaries/ValidMoves.dll \
                           search/Quiescence.fs
	$(fsc) $(optimize) -r:binaries/MoveHelpers.dll \
                       -r:binaries/Elements.dll \
                       -r:binaries/ValidMoves.dll \
                       -a \
                       --out:binaries/Quiescence.dll \
                       search/Quiescence.fs

binaries/AlphaBeta.dll : binaries/MoveHelpers.dll \
                          binaries/DepthFirstSearch.dll \
                          binaries/Elements.dll \
                          binaries/Quiescence.dll \
                          binaries/ValidMoves.dll \
                          binaries/TranspositionTable.dll \
                          search/AlphaBeta2.fs
	$(fsc) $(optimize) -r:binaries/MoveHelpers.dll \
                       -r:binaries/DepthFirstSearch.dll \
                       -r:binaries/Elements.dll \
                       -r:binaries/Quiescence.dll \
                       -r:binaries/ValidMoves.dll \
                       -r:binaries/TranspositionTable.dll \
                       -a \
                       --out:binaries/AlphaBeta.dll \
                       search/AlphaBeta2.fs

binaries/AlphaBetaID.dll : binaries/MoveHelpers.dll \
                           binaries/DepthFirstSearch.dll \
                           binaries/Elements.dll \
                           binaries/Quiescence.dll \
                           binaries/ValidMoves.dll \
                           binaries/AlphaBeta.dll \
                           binaries/TranspositionTable.dll \
                           search/AlphaBetaID.fs
	$(fsc) $(optimize) -r:binaries/MoveHelpers.dll \
                       -r:binaries/DepthFirstSearch.dll \
                       -r:binaries/Elements.dll \
                       -r:binaries/Quiescence.dll \
                       -r:binaries/ValidMoves.dll \
                       -r:binaries/AlphaBeta.dll \
                       -r:binaries/TranspositionTable.dll \
                       -a \
                        --out:binaries/AlphaBetaID.dll \
                       search/AlphaBetaID.fs

binaries/DepthFirstSearch.dll : binaries/Elements.dll \
                                binaries/ValidMoves.dll \
                                binaries/MoveHelpers.dll \
                                search/DepthFirstSearch.fs
	$(fsc) $(optimize) -r:binaries/Elements.dll \
                       -r:binaries/ValidMoves.dll \
                       -r:binaries/MoveHelpers.dll \
                       -a \
                       --out:binaries/DepthFirstSearch.dll \
                       search/DepthFirstSearch.fs
