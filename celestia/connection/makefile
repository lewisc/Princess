fsc=fsharpc --nologo -I:binaries --warn:5
optimize=--optimize --tailcalls+ --crossoptimize+
debug=-g

all : binaries/IMCSConnection.dll binaries/ServerConnection.dll

binaries/IMCSConnection.dll : binaries IMCSConnection.fs
	$(fsc) $(optimize) -a --out:binaries/IMCSConnection.dll IMCSConnection.fs

binaries/ServerConnection.dll : binaries ServerConnection.fs binaries/IMCSConnection.dll
	$(fsc) $(optimize) -a --out:binaries/ServerConnection.dll -r:Elements.dll -r:MoveHelpers.dll  -r:IMCSConnection.dll -r:ValidMoves.dll -r:TypedInput.dll ServerConnection.fs

binaries : 
	mkdir -p binaries

clean : cleansearch
	rm -rf binaries

cleansearch :
	cd search && $(MAKE) clean

   


