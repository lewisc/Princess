board_all : $(libs)/MoveGeneration.dll \
            $(libs)/ZobristHash.dll \
            $(libs)/GameState.dll \
            $(libs)/Primitives.dll

$(libs)/Primitives.dll : board/Primitives.fs
	$(fsc) $(optimize) -a \
                       --out:$(libs)/Primitives.dll \
                       board/Primitives.fs

$(libs)/ZobristHash.dll : $(libs)/Primitives.dll \
                          board/ZobristHash.fs
	$(fsc) $(optimize) -r:Primitives.dll \
                       -a \
                       --out:$(libs)/ZobristHash.dll \
                       board/ZobristHash.fs

$(libs)/MoveGeneration.dll : $(libs)/Primitives.dll \
                      $(libs)/ZobristHash.dll \
                      $(libs)/FoldBreak.dll \
                      board/MoveGeneration.fs
	$(fsc) $(optimize) -r:Primitives.dll \
                       -r:ZobristHash.dll \
                       -r:FoldBreak.dll \
                       -a \
                       --out:$(libs)/MoveGeneration.dll \
                       board/MoveGeneration.fs

$(libs)/GameState.dll : $(libs)/Primitives.dll \
                        $(libs)/ZobristHash.dll \
                        $(libs)/MoveGeneration.dll \
                       board/GameState.fs
	$(fsc) $(optimize) -r:Primitives.dll \
                       -r:ZobristHash.dll \
                       -r:MoveGeneration.dll \
                       -a \
                       --out:$(libs)/GameState.dll \
                       board/GameState.fs
