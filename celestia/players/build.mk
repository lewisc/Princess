players_all : $(libs)/RandomPlayer.exe \
              $(libs)/NetworkPlayer.exe

$(libs)/RandomPlayer.exe : $(libs)/Primitives.dll \
                           $(libs)/ValidMoves.dll \
                           $(libs)/Heuristics.dll \
                           players/RandomPlayer.fs
	$(fsc) $(optimize) -r:Primitives.dll \
                       -r:ValidMoves.dll \
                       -r:Heuristics.dll\
                       --standalone \
                       --out:$(bin)/RandomPlayer.exe \
                       players/RandomPlayer.fs

$(libs)/NetworkPlayer.exe : $(libs)/IMCSConnection.dll \
                            $(libs)/Primitives.dll \
                            $(libs)/ValidMoves.dll \
                            $(libs)/AlphaBeta.dll \
                            $(libs)/DepthFirstSearch.dll \
                            $(libs)/Heuristics.dll \
                            $(libs)/MTDF.dll \
                            $(libs)/TranspositionTable.dll \
                            $(libs)/AlphaBetaID.dll \
                            $(libs)/ServerConnection.dll \
                            players/NetPlayer.fs
	$(fsc) $(optimize) -r:IMCSConnection.dll \
                       -r:Primitives.dll \
                       -r:ValidMoves.dll \
                       -r:AlphaBeta.dll \
                       -r:DepthFirstSearch.dll \
                       -r:Heuristics.dll \
                       -r:MTDF.dll \
                       -r:TranspositionTable.dll \
                       -r:AlphaBetaID.dll \
                       -r:ServerConnection.dll \
                       --standalone \
                       --out:$(bin)/NetworkPlayer.exe \
                       players/NetPlayer.fs
