
search_all : $(libs)/DepthFirstSearch.dll \
             $(libs)/AlphaBetaID.dll \
             $(libs)/Heuristics.dll \
             $(libs)/Quiescence.dll \
             $(libs)/MTDF.dll \
             $(libs)/AlphaBeta.dll

$(libs)/MTDF.dll : $(libs)/AlphaBeta.dll \
                   search/MTDF.fs
	$(fsc) $(optimize) -r:AlphaBeta.dll \
                       -r:Primitives.dll \
                       -r:ValidMoves.dll \
                       -r:TranspositionTable.dll \
                       -r:DepthFirstSearch.dll \
                       -a \
                       --out:$(libs)/MTDF.dll \
                       search/MTDF.fs

$(libs)/Heuristics.dll : $(libs)/Primitives.dll \
                         $(libs)/ValidMoves.dll \
                         search/Heuristics.fs
	$(fsc) $(optimize) -r:Primitives.dll \
                       -r:ValidMoves.dll \
                       -a \
                       --out:$(libs)/Heuristics.dll \
                       search/Heuristics.fs

$(libs)/Quiescence.dll : $(libs)/Primitives.dll \
                         $(libs)/ValidMoves.dll \
                         search/Quiescence.fs
	$(fsc) $(optimize) -r:Primitives.dll \
                       -r:ValidMoves.dll \
                       -a \
                       --out:$(libs)/Quiescence.dll \
                       search/Quiescence.fs

$(libs)/AlphaBeta.dll : $(libs)/DepthFirstSearch.dll \
                        $(libs)/Primitives.dll \
                        $(libs)/Quiescence.dll \
                        $(libs)/ValidMoves.dll \
                        $(libs)/TranspositionTable.dll \
                        search/AlphaBeta2.fs
	$(fsc) $(optimize) -r:DepthFirstSearch.dll \
                       -r:Primitives.dll \
                       -r:Quiescence.dll \
                       -r:ValidMoves.dll \
                       -r:TranspositionTable.dll \
                       -a \
                       --out:$(libs)/AlphaBeta.dll \
                       search/AlphaBeta2.fs

$(libs)/AlphaBetaID.dll : $(libs)/DepthFirstSearch.dll \
                          $(libs)/Primitives.dll \
                          $(libs)/Quiescence.dll \
                          $(libs)/ValidMoves.dll \
                          $(libs)/AlphaBeta.dll \
                          $(libs)/TranspositionTable.dll \
                          search/AlphaBetaID.fs
	$(fsc) $(optimize) -r:DepthFirstSearch.dll \
                       -r:Primitives.dll \
                       -r:Quiescence.dll \
                       -r:ValidMoves.dll \
                       -r:AlphaBeta.dll \
                       -r:TranspositionTable.dll \
                       -a \
                        --out:$(libs)/AlphaBetaID.dll \
                       search/AlphaBetaID.fs

$(libs)/DepthFirstSearch.dll : $(libs)/Primitives.dll \
                               $(libs)/ValidMoves.dll \
                               search/DepthFirstSearch.fs
	$(fsc) $(optimize) -r:Primitives.dll \
                       -r:ValidMoves.dll \
                       -a \
                       --out:$(libs)/DepthFirstSearch.dll \
                       search/DepthFirstSearch.fs
