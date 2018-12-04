endgamedb_all : $(libs)/EndGame.dll

$(libs)/EndGame.dll : $(libs)/Heuristics.dll \
                      $(libs)/Elements.dll \
                      $(libs)/ValidMoves.dll \
                      $(libs)/AlphaBeta.dll \
                      $(libs)/BoardConstants.dll \
                      $(libs)/AlphaBeta.dll \
                      $(libs)/MTDF.dll \
                      $(libs)/MoveHelpers.dll \
                      endgamedb/EndGameDB.fs
	$(fsc) $(optimize) -r:Heuristics.dll \
                       -r:Elements.dll \
                       -r:ValidMoves.dll  \
                       -r:AlphaBeta.dll \
                       -r:BoardConstants.dll \
                       -r:AlphaBeta.dll \
                       -r:MTDF.dll \
                       -r:MoveHelpers.dll \
                       --out:$(libs)/EndGame.exe \
                       endgamedb/EndGameDB.fs
