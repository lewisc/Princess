tables_all : $(libs)/TranspositionTable.dll

$(libs)/TranspositionTable.dll : $(libs)/Elements.dll \
                                 $(libs)/ValidMoves.dll \
                                 $(libs)/ZobristKeys.dll \
                                 tables/TranspositionTable.fs 
	$(fsc) $(optimize) -r:Elements.dll \
                       -r:ValidMoves.dll \
                       -r:ZobristKeys.dll \
                       -a \
                       --out:$(libs)/TranspositionTable.dll \
                       tables/TranspositionTable.fs
