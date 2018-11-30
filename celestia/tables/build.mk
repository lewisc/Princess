tables_all : binaries/TranspositionTable.dll

binaries/TranspositionTable.dll : binaries/Elements.dll \
                                   binaries/ValidMoves.dll \
                                   binaries/ZobristKeys.dll \
                                   tables/TranspositionTable.fs 
	$(fsc) $(optimize) -r:binaries/Elements.dll \
                       -r:binaries/ValidMoves.dll \
                       -r:binaries/ZobristKeys.dll \
                       -a \
                       --out:binaries/TranspositionTable.dll \
                       tables/TranspositionTable.fs
