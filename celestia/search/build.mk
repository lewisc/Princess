
search_all : $(libs)/DepthFirstSearch.dll \
             $(libs)/AlphaBetaID.dll \
             $(libs)/Heuristics.dll \
             $(libs)/Quiescence.dll \
             $(libs)/MTDF.dll \
             $(libs)/AlphaBeta.dll

$(libs)/MTDF.dll : $(libs)/AlphaBeta.dll \
                   search/MTDF.fs
	$(fsc) $(optimize) -r:MoveHelpers.dll \
                       -r:AlphaBeta.dll \
                       -r:Elements.dll \
                       -r:ValidMoves.dll \
                       -r:BoardConstants.dll \
                       -r:TranspositionTable.dll \
                       -r:DepthFirstSearch.dll \
                       -a \
                       --out:$(libs)/MTDF.dll \
                       search/MTDF.fs

$(libs)/Heuristics.dll : $(libs)/Elements.dll \
                         $(libs)/ValidMoves.dll \
                         $(libs)/MoveHelpers.dll \
                         search/Heuristics.fs
	$(fsc) $(optimize) -r:Elements.dll \
                       -r:ValidMoves.dll \
                       -r:MoveHelpers.dll \
                       -a \
                       --out:$(libs)/Heuristics.dll \
                       search/Heuristics.fs

$(libs)/Quiescence.dll : $(libs)/MoveHelpers.dll \
                         $(libs)/Elements.dll \
                         $(libs)/ValidMoves.dll \
                         search/Quiescence.fs
	$(fsc) $(optimize) -r:MoveHelpers.dll \
                       -r:Elements.dll \
                       -r:ValidMoves.dll \
                       -a \
                       --out:$(libs)/Quiescence.dll \
                       search/Quiescence.fs

$(libs)/AlphaBeta.dll : $(libs)/MoveHelpers.dll \
                        $(libs)/DepthFirstSearch.dll \
                        $(libs)/Elements.dll \
                        $(libs)/Quiescence.dll \
                        $(libs)/ValidMoves.dll \
                        $(libs)/TranspositionTable.dll \
                        search/AlphaBeta2.fs
	$(fsc) $(optimize) -r:MoveHelpers.dll \
                       -r:DepthFirstSearch.dll \
                       -r:Elements.dll \
                       -r:Quiescence.dll \
                       -r:ValidMoves.dll \
                       -r:TranspositionTable.dll \
                       -a \
                       --out:$(libs)/AlphaBeta.dll \
                       search/AlphaBeta2.fs

$(libs)/AlphaBetaID.dll : $(libs)/MoveHelpers.dll \
                          $(libs)/DepthFirstSearch.dll \
                          $(libs)/Elements.dll \
                          $(libs)/Quiescence.dll \
                          $(libs)/ValidMoves.dll \
                          $(libs)/AlphaBeta.dll \
                          $(libs)/TranspositionTable.dll \
                          search/AlphaBetaID.fs
	$(fsc) $(optimize) -r:MoveHelpers.dll \
                       -r:DepthFirstSearch.dll \
                       -r:Elements.dll \
                       -r:Quiescence.dll \
                       -r:ValidMoves.dll \
                       -r:AlphaBeta.dll \
                       -r:TranspositionTable.dll \
                       -a \
                        --out:$(libs)/AlphaBetaID.dll \
                       search/AlphaBetaID.fs

$(libs)/DepthFirstSearch.dll : $(libs)/Elements.dll \
                               $(libs)/ValidMoves.dll \
                               $(libs)/MoveHelpers.dll \
                               search/DepthFirstSearch.fs
	$(fsc) $(optimize) -r:Elements.dll \
                       -r:ValidMoves.dll \
                       -r:MoveHelpers.dll \
                       -a \
                       --out:$(libs)/DepthFirstSearch.dll \
                       search/DepthFirstSearch.fs
