fsc=fsharpc --nologo -I:binaries --warn:5
optimize=--optimize --tailcalls+ --crossoptimize+
debug=-g

all : binaries/TranspositionTable.dll
	

binaries/TranspositionTable.dll  : binaries TranspositionTable.fs 
	$(fsc) $(optimize) --out:binaries/TranspositionTable.dll -a -r:Elements.dll -r:ValidMoves.dll -r:ZobristKeys.dll TranspositionTable.fs

binaries : 
	mkdir -p binaries

clean :
	rm -rf binaries
