board_all : $(libs)/BoardConstants.dll \
            $(libs)/MoveHelpers.dll \
            $(libs)/MoveGen.dll \
            $(libs)/ValidMoves.dll \
            $(libs)/ZobristKeys.dll \
            $(libs)/TypedInput.dll \
            $(libs)/Elements.dll

$(libs)/Elements.dll : board/Elements.fs
	$(fsc) $(optimize) -a \
                       --out:$(libs)/Elements.dll \
                       board/Elements.fs

$(libs)/TypedInput.dll : board/TypedInput.fs
	$(fsc) $(optimize) -a \
                       --out:$(libs)/TypedInput.dll \
                       board/TypedInput.fs

$(libs)/ZobristKeys.dll : $(libs)/Elements.dll \
                          $(libs)/BoardConstants.dll \
                          board/ZobristKeys.fs
	$(fsc) $(optimize) -r:Elements.dll \
                       -r:BoardConstants.dll  \
                       -a \
                       --out:$(libs)/ZobristKeys.dll \
                       board/ZobristKeys.fs

$(libs)/ValidMoves.dll : $(libs)/TypedInput.dll \
                         $(libs)/Elements.dll \
                         $(libs)/ZobristKeys.dll \
                         $(libs)/BoardConstants.dll \
                         $(libs)/MoveHelpers.dll \
                         $(libs)/MoveGen.dll \
                         board/ValidMoves.fs 
	$(fsc) $(optimize) -r:TypedInput.dll \
                       -r:Elements.dll \
                       -r:ZobristKeys.dll \
                       -r:BoardConstants.dll \
                       -r:MoveHelpers.dll \
                       -r:MoveGen.dll \
                       -a \
                       --out:$(libs)/ValidMoves.dll \
                       board/ValidMoves.fs

$(libs)/MoveGen.dll : $(libs)/Elements.dll \
                      $(libs)/BoardConstants.dll \
                      $(libs)/ZobristKeys.dll \
                      $(libs)/TypedInput.dll \
                      $(libs)/MoveHelpers.dll \
                      board/MoveGen.fs
	$(fsc) $(optimize) -r:MoveHelpers.dll \
                       -r:Elements.dll \
                       -r:BoardConstants.dll \
                       -r:ZobristKeys.dll \
                       -r:TypedInput.dll \
                        -a \
                       --out:$(libs)/MoveGen.dll \
                       board/MoveGen.fs

$(libs)/MoveHelpers.dll : $(libs)/Elements.dll \
                          $(libs)/BoardConstants.dll \
                          $(libs)/ZobristKeys.dll \
                          $(libs)/TypedInput.dll\
                          board/MoveHelpers.fs 
	$(fsc) $(optimize) -r:Elements.dll \
                       -r:BoardConstants.dll \
                       -r:ZobristKeys.dll \
                       -r:TypedInput.dll \
                       -a \
                       --out:$(libs)/MoveHelpers.dll \
                       board/MoveHelpers.fs

$(libs)/BoardConstants.dll : $(libs)/Elements.dll \
                             board/BoardConstants.fs
	$(fsc) $(optimize) -r:Elements.dll \
                       -a \
                       --out:$(libs)/BoardConstants.dll \
                       board/BoardConstants.fs
