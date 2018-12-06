board_all : $(libs)/MoveGen.dll \
            $(libs)/ValidMoves.dll \
            $(libs)/TypedInput.dll \
            $(libs)/Primitives.dll

$(libs)/Primitives.dll : board/Primitives.fs
	$(fsc) $(optimize) -a \
                       --out:$(libs)/Primitives.dll \
                       board/Primitives.fs

$(libs)/TypedInput.dll : board/TypedInput.fs
	$(fsc) $(optimize) -a \
                       --out:$(libs)/TypedInput.dll \
                       board/TypedInput.fs

$(libs)/ValidMoves.dll : $(libs)/TypedInput.dll \
                         $(libs)/Primitives.dll \
                         $(libs)/MoveGen.dll \
                         board/ValidMoves.fs 
	$(fsc) $(optimize) -r:TypedInput.dll \
                       -r:Primitives.dll \
                       -r:MoveGen.dll \
                       -a \
                       --out:$(libs)/ValidMoves.dll \
                       board/ValidMoves.fs

$(libs)/MoveGen.dll : $(libs)/Primitives.dll \
                      $(libs)/TypedInput.dll \
                      board/MoveGen.fs
	$(fsc) $(optimize) -r:Primitives.dll \
                       -r:TypedInput.dll \
                        -a \
                       --out:$(libs)/MoveGen.dll \
                       board/MoveGen.fs
