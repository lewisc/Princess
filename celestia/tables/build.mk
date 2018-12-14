tables_all : $(libs)/TranspositionTable.dll

$(libs)/TranspositionTable.dll : $(libs)/Primitives.dll \
                                 $(libs)/ZobristHash.dll \
                                 tables/TranspositionTable.fs 
	$(fsc) $(optimize) -r:Primitives.dll \
                       -r:ZobristHash.dll \
                       -a \
                       --out:$(libs)/TranspositionTable.dll \
                       tables/TranspositionTable.fs
