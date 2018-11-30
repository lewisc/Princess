endgamedb_all : binaries/EndGame.dll

binaries/EndGame.dll : binaries/Heuristics.dll \
                       binaries/Elements.dll \
                       binaries/ValidMoves.dll \
                       binaries/AlphaBeta.dll \
                       binaries/BoardConstants.dll \
                       endgamedb/EndGameDB.fs
	$(fsc) $(optimize) -r:binaries/Heuristics.dll \
                       -r:binaries/Elements.dll \
                       -r:binaries/ValidMoves.dll  \
                       -r:binaries/AlphaBeta.dll \
                       -r:binaries/BoardConstants.dll \
                       -a \
                       --out:binaries/EndGame.dll \
                       endgamedb/EndGameDB.fs
