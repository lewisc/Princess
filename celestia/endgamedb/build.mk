endgamedb_all : $(libs)/EndGame.dll

$(libs)/EndGame.dll : $(libs)/Heuristics.dll \
                      $(libs)/Primitives.dll \
                      $(libs)/AlphaBeta.dll \
                      endgamedb/EndGameDB.fs
	$(fsc) $(optimize) -r:Heuristics.dll \
                       -r:Primitives.dll \
                       -r:AlphaBeta.dll \
                       --out:$(libs)/EndGame.exe \
                       endgamedb/EndGameDB.fs
