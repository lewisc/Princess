fsc=fsharpc --nologo -I:binaries --warn:5
optimize=--optimize --tailcalls+ --crossoptimize+
debug=-g

all : binaries/ZobristKeys.dll binaries/ValidMoves.dll binaries/TypedInput.dll binaries/BoardConstants.dll binaries/Elements.dll binaries 
	

binaries/ZobristKeys.dll : binaries/Elements.dll binaries binaries/BoardConstants.dll ZobristKeys.fs
	$(fsc) $(optimize) -a --out:binaries/ZobristKeys.dll -r:Elements.dll -r:BoardConstants.dll ZobristKeys.fs

binaries/ValidMoves.dll : binaries binaries/TypedInput.dll ValidMoves.fs binaries/Elements.dll binaries/ZobristKeys.dll binaries/BoardConstants.dll
	$(fsc) $(optimize) -a --out:binaries/ValidMoves.dll -r:TypedInput.dll -r:Elements.dll -r:ZobristKeys.dll -r BoardConstants.dll ValidMoves.fs

binaries/TypedInput.dll : binaries TypedInput.fs
	$(fsc) $(optimize) -a --out:binaries/TypedInput.dll TypedInput.fs

binaries/BoardConstants.dll : binaries/Elements.dll binaries BoardConstants.fs
	$(fsc) $(optimize) -a --out:binaries/BoardConstants.dll -r:Elements.dll BoardConstants.fs

binaries/Elements.dll : binaries Elements.fs
	$(fsc) $(optimize) -a --out:binaries/Elements.dll  Elements.fs

binaries :
	mkdir -p binaries
   
clean :
	rm -rf binaries
