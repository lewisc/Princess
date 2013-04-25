fsc=fsharpc --nologo -I:binaries --warn:5
optimize=--optimize --tailcalls+ --crossoptimize+
debug=-g

all : binaries/ZobristKeys.dll binaries/ValidMoves.dll binaries/TypedInput.dll binaries/BoardConstants.dll binaries/Elements.dll binaries 
	

binaries/ZobristKeys.dll : binaries/Elements.dll binaries binaries/BoardConstants.dll ZobristKeys.fs
	$(fsc) $(optimize) -a --out:binaries/ZobristKeys.dll -r:Elements.dll -r:BoardConstants.dll ZobristKeys.fs

binaries/ValidMoves.dll : binaries binaries/TypedInput.dll ValidMoves.fs binaries/Elements.dll binaries/ZobristKeys.dll binaries/BoardConstants.dll binaries/MoveHelpers.dll binaries/MoveGen.dll
	$(fsc) $(optimize) -a --out:binaries/ValidMoves.dll -r:TypedInput.dll -r:Elements.dll -r:ZobristKeys.dll -r:BoardConstants.dll -r:MoveHelpers.dll -r:MoveGen.dll ValidMoves.fs

binaries/TypedInput.dll : binaries TypedInput.fs
	$(fsc) $(optimize) -a --out:binaries/TypedInput.dll TypedInput.fs

binaries/MoveGen.dll : binaries/Elements.dll binaries MoveGen.fs binaries/BoardConstants.dll binaries/ZobristKeys.dll binaries/TypedInput.dll binaries/MoveHelpers.dll
	$(fsc) $(optimize) -a --out:binaries/MoveGen.dll -r:MoveHelpers.dll -r:Elements.dll -r:BoardConstants.dll -r:ZobristKeys.dll -r:TypedInput.dll MoveGen.fs

binaries/MoveHelpers.dll : binaries/Elements.dll binaries MoveHelpers.fs binaries/BoardConstants.dll binaries/ZobristKeys.dll binaries/TypedInput.dll
	$(fsc) $(optimize) -a --out:binaries/MoveHelpers.dll -r:Elements.dll -r:BoardConstants.dll -r:ZobristKeys.dll -r:TypedInput.dll MoveHelpers.fs

binaries/BoardConstants.dll : binaries/Elements.dll binaries BoardConstants.fs
	$(fsc) $(optimize) -a --out:binaries/BoardConstants.dll -r:Elements.dll BoardConstants.fs


binaries/Elements.dll : binaries Elements.fs
	$(fsc) $(optimize) -a --out:binaries/Elements.dll  Elements.fs

binaries :
	mkdir -p binaries
   
clean :
	rm -rf binaries
