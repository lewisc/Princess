endgamedb_all : $(libs)/EndGame.dll

$(libs)/EndGame.dll : $(libs)/Heuristics.dll \
                      $(libs)/Primitives.dll \
                      $(libs)/ValidMoves.dll \
                      $(libs)/AlphaBeta.dll \
                      $(libs)/MTDF.dll \
                      $(libs)/MoveHelpers.dll \
                      endgamedb/EndGameDB.fs
	$(fsc) $(optimize) -r:Heuristics.dll \
                       -r:Primitives.dll \
                       -r:ValidMoves.dll  \
                       -r:AlphaBeta.dll \
                       -r:MTDF.dll \
                       -r:MoveHelpers.dll \
                       --out:$(libs)/EndGame.exe \
                       endgamedb/EndGameDB.fs
