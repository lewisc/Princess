players_all : binaries/RandomPlayer.exe \
              binaries/NetworkPlayer.exe

binaries/RandomPlayer.exe : binaries/MoveHelpers.dll \
                            binaries/Elements.dll \
                            binaries/ValidMoves.dll \
                            binaries/Heuristics.dll \
                            players/RandomPlayer.fs
	$(fsc) $(optimize) -r:binaries/MoveHelpers.dll \
                       -r:binaries/Elements.dll \
                       -r:binaries/ValidMoves.dll \
                       -r:binaries/Heuristics.dll\
                       --standalone \
                       --out:binaries/RandomPlayer.exe \
                       players/RandomPlayer.fs

binaries/NetworkPlayer.exe : binaries/IMCSConnection.dll \
                             binaries/Elements.dll \
                             binaries/ValidMoves.dll \
                             binaries/AlphaBeta.dll \
                             binaries/DepthFirstSearch.dll \
                             binaries/Heuristics.dll \
                             binaries/MTDF.dll \
                             binaries/TranspositionTable.dll \
                             binaries/AlphaBetaID.dll \
                             binaries/ServerConnection.dll \
                             players/NetPlayer.fs
	$(fsc) $(optimize) -r:binaries/IMCSConnection.dll \
                       -r:binaries/Elements.dll \
                       -r:binaries/ValidMoves.dll \
                       -r:binaries/AlphaBeta.dll \
                       -r:binaries/DepthFirstSearch.dll \
                       -r:binaries/Heuristics.dll \
                       -r:binaries/MTDF.dll \
                       -r:binaries/TranspositionTable.dll \
                       -r:binaries/AlphaBetaID.dll \
                       -r:binaries/ServerConnection.dll \
                       --standalone \
                       --out:binaries/NetworkPlayer.exe \
                       players/NetPlayer.fs
