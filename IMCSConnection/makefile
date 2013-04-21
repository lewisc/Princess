fsc=fsharpc --nologo
optimize=--optimize --tailcalls+ --crossoptimize+
debug=-g

../output/IMCSConnection.dll ../output/ServerConnection.dll  : binaries/IMCSConnection.dll binaries/ServerConnection.dll
	cp binaries/* ../output

binaries/IMCSConnection.dll : binaries IMCSConnection.fs
	$(fsc) $(optimize) -a --out:binaries/IMCSConnection.dll IMCSConnection.fs

binaries/ServerConnection.dll : binaries ServerConnection.fs binaries/IMCSConnection.dll ../output/Elements.dll ../output/ValidMoves.dll ../output/TypedInput.dll
	$(fsc) $(optimize) -a --out:binaries/ServerConnection.dll -r:../output/Elements.dll  -r:binaries/IMCSConnection.dll -r:../output/ValidMoves.dll -r:../output/TypedInput.dll ServerConnection.fs

binaries :
	mkdir binaries
   


