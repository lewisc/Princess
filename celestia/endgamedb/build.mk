endgamedb_all : $(libs)/EndGame.dll

$(libs)/EndGame.dll : $(libs)/Heuristics.dll \
                      $(libs)/Elements.dll \
                      $(libs)/ValidMoves.dll \
                      $(libs)/AlphaBeta.dll \
                      $(libs)/BoardConstants.dll \
                      endgamedb/EndGameDB.fs
	$(fsc) $(optimize) -r:Heuristics.dll \
                       -r:Elements.dll \
                       -r:ValidMoves.dll  \
                       -r:AlphaBeta.dll \
                       -r:BoardConstants.dll \
                       -a \
                       --out:$(libs)/EndGame.dll \
                       endgamedb/EndGameDB.fs
