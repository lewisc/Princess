fsc=fsharpc --nologo
optimize=--optimize --tailcalls+ --crossoptimize+
debug=-g

../output/ZobristKeys.dll ../output/ValidMoves.dll ../output/TypedInput.dll ../output/BoardConstants.dll ../output/Elements.dll : binaries/ZobristKeys.dll binaries/ValidMoves.dll binaries/TypedInput.dll binaries/BoardConstants.dll binaries/Elements.dll binaries 
	cp binaries/* ../output

binaries/ZobristKeys.dll : binaries/Elements.dll binaries binaries/BoardConstants.dll ZobristKeys.fs
	$(fsc) $(optimize) -a --out:binaries/ZobristKeys.dll -r:binaries/Elements.dll -r:binaries/BoardConstants.dll ZobristKeys.fs

binaries/ValidMoves.dll : binaries binaries/TypedInput.dll ValidMoves.fs binaries/Elements.dll binaries/ZobristKeys.dll binaries/BoardConstants.dll
	$(fsc) $(optimize) -a --out:binaries/ValidMoves.dll -r:binaries/TypedInput.dll -r:binaries/Elements.dll -r:binaries/ZobristKeys.dll -r binaries/BoardConstants.dll ValidMoves.fs

binaries/TypedInput.dll : binaries TypedInput.fs
	$(fsc) $(optimize) -a --out:binaries/TypedInput.dll TypedInput.fs

binaries/BoardConstants.dll : binaries/Elements.dll binaries BoardConstants.fs
	$(fsc) $(optimize) -a --out:binaries/BoardConstants.dll -r:binaries/Elements.dll BoardConstants.fs

binaries/Elements.dll : binaries Elements.fs
	$(fsc) $(optimize) -a --out:binaries/Elements.dll  Elements.fs

binaries :
	mkdir binaries
   
clean :
	rm -rf binaries


