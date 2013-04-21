fsc=fsharpc --nologo -I:binaries --warn:5
optimize=--optimize --tailcalls+ --crossoptimize+
debug=-g

all : binaries/TranspositionTable.dll
	

binaries/TranspositionTable.dll  : binaries TranspositionTable.fs 
	$(fsc) $(optimize) --out:binaries/TranspositionTable.dll -a -r:Elements.dll -r:ValidMoves.dll -r:ZobristKeys.dll TranspositionTable.fs

binaries : board/binaries
	mkdir -p binaries
	cp board/binaries/* binaries

board/binaries :
	cd board && $(MAKE)

clean : cleanboard
	rm -rf binaries

cleanboard :
	cd board && $(MAKE) clean
