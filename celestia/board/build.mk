board_all : binaries/BoardConstants.dll \
            binaries/MoveHelpers.dll \
            binaries/MoveGen.dll \
            binaries/ValidMoves.dll \
            binaries/ZobristKeys.dll \
            binaries/TypedInput.dll \
            binaries/Elements.dll

binaries/Elements.dll : board/Elements.fs
	$(fsc) $(optimize) -a \
                       --out:binaries/Elements.dll \
                       board/Elements.fs

binaries/TypedInput.dll : board/TypedInput.fs
	$(fsc) $(optimize) -a \
                       --out:binaries/TypedInput.dll \
                       board/TypedInput.fs

binaries/ZobristKeys.dll : binaries/Elements.dll \
                           binaries/BoardConstants.dll \
                           board/ZobristKeys.fs
	$(fsc) $(optimize) -r:binaries/Elements.dll \
                       -r:binaries/BoardConstants.dll  \
                       -a \
                       --out:binaries/ZobristKeys.dll \
                       board/ZobristKeys.fs

binaries/ValidMoves.dll : binaries/TypedInput.dll \
                          binaries/Elements.dll \
                          binaries/ZobristKeys.dll \
                          binaries/BoardConstants.dll \
                          binaries/MoveHelpers.dll \
                          binaries/MoveGen.dll \
                          board/ValidMoves.fs 
	$(fsc) $(optimize) -r:TypedInput.dll \
                       -r:Elements.dll \
                       -r:ZobristKeys.dll \
                       -r:BoardConstants.dll \
                       -r:MoveHelpers.dll \
                       -r:MoveGen.dll \
                       -a \
                       --out:binaries/ValidMoves.dll \
                       board/ValidMoves.fs

binaries/MoveGen.dll : binaries/Elements.dll \
                       binaries/BoardConstants.dll \
                       binaries/ZobristKeys.dll \
                       binaries/TypedInput.dll \
                       binaries/MoveHelpers.dll \
                       board/MoveGen.fs
	$(fsc) $(optimize) -r:MoveHelpers.dll \
                       -r:Elements.dll \
                       -r:BoardConstants.dll \
                       -r:ZobristKeys.dll \
                       -r:TypedInput.dll \
                        -a \
                       --out:binaries/MoveGen.dll \
                       board/MoveGen.fs

binaries/MoveHelpers.dll : binaries/Elements.dll \
                           binaries/BoardConstants.dll \
                           binaries/ZobristKeys.dll \
                           binaries/TypedInput.dll\
                           board/MoveHelpers.fs 
	$(fsc) $(optimize) -r:binaries/Elements.dll \
                       -r:binaries/BoardConstants.dll \
                       -r:binaries/ZobristKeys.dll \
                       -r:binaries/TypedInput.dll \
                       -a \
                       --out:binaries/MoveHelpers.dll \
                       board/MoveHelpers.fs

binaries/BoardConstants.dll : binaries/Elements.dll \
                              board/BoardConstants.fs
	$(fsc) $(optimize) -r:binaries/Elements.dll \
                       -a \
                       --out:binaries/BoardConstants.dll \
                       board/BoardConstants.fs
