
search_all : $(libs)/DepthFirstSearch.dll \
             $(libs)/Heuristics.dll \
             $(libs)/Quiescence.dll \
             $(libs)/MTDF.dll \
             $(libs)/AlphaBeta.dll

$(libs)/MTDF.dll : $(libs)/AlphaBeta.dll \
                   search/MTDF.fs
	$(fsc) $(optimize) -r:AlphaBeta.dll \
                       -r:Primitives.dll \
                       -r:TranspositionTable.dll \
                       -r:DepthFirstSearch.dll \
                       -a \
                       --out:$(libs)/MTDF.dll \
                       search/MTDF.fs

$(libs)/Heuristics.dll : $(libs)/Primitives.dll \
                         $(libs)/GameState.dll \
                         search/Heuristics.fs
	$(fsc) $(optimize) -r:Primitives.dll \
                       -r:GameState.dll \
                       -a \
                       --out:$(libs)/Heuristics.dll \
                       search/Heuristics.fs

$(libs)/Quiescence.dll : $(libs)/Primitives.dll \
						 $(libs)/GameState.dll \
                         search/Quiescence.fs
	$(fsc) $(optimize) -r:Primitives.dll \
                       -r:GameState.dll \
                       -a \
                       --out:$(libs)/Quiescence.dll \
                       search/Quiescence.fs

$(libs)/AlphaBeta.dll : $(libs)/DepthFirstSearch.dll \
                        $(libs)/Primitives.dll \
                        $(libs)/Quiescence.dll \
                        $(libs)/TranspositionTable.dll \
                        search/AlphaBeta.fs
	$(fsc) $(optimize) -r:DepthFirstSearch.dll \
                       -r:Primitives.dll \
                       -r:Quiescence.dll \
                       -r:TranspositionTable.dll \
                       -a \
                       --out:$(libs)/AlphaBeta.dll \
                       search/AlphaBeta.fs

$(libs)/DepthFirstSearch.dll : $(libs)/Primitives.dll \
                               $(libs)/GameState.dll \
                               search/DepthFirstSearch.fs
	$(fsc) $(optimize) -r:Primitives.dll \
                       -r:GameState.dll \
                       -a \
                       --out:$(libs)/DepthFirstSearch.dll \
                       search/DepthFirstSearch.fs
