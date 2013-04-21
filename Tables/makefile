fsc=fsharpc --nologo
optimize=--optimize --tailcalls+ --crossoptimize+
debug=-g

../output/TranspositionTable.dll : binaries/TranspositionTable.dll
	cp binaries/* ../output

binaries/TranspositionTable.dll  : binaries TranspositionTable.fs ../output/Elements.dll ../output/ValidMoves.dll  ../output/ZobristKeys.dll
	$(fsc) $(optimize) --out:binaries/TranspositionTable.dll -a -r:../output/Elements.dll -r:../output/ValidMoves.dll -r:../output/ZobristKeys.dll TranspositionTable.fs

binaries :
	mkdir binaries

clean :
	rm -rf binaries
   


