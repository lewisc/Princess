fsc=fsharpc --nologo --warn:5 -I:binaries
optimize=--optimize --tailcalls+ --crossoptimize+
debug=-g

all  : binaries/DepthFirstSearchRegression.dll binaries/MoveRegression.dll binaries/TestTypes.dll binaries/RegressionRunner.exe

binaries/MoveRegression.dll : binaries binaries/TestTypes.dll MoveRegression.fs
	$(fsc) $(optimize) -a -r:Elements.dll -r:ValidMoves.dll -r:TestTypes.dll --out:binaries/MoveRegression.dll -r:TypedInput.dll  -r:Resources.dll MoveRegression.fs

binaries/TestTypes.dll : binaries TestTypes.fs
	$(fsc) $(optimize) -a --out:binaries/TestTypes.dll  TestTypes.fs

binaries/RegressionRunner.exe : binaries RegressionRunner.fs binaries/TestTypes.dll binaries/MoveRegression.dll binaries/DepthFirstSearchRegression.dll 
	$(fsc) $(optimize) --standalone --out:binaries/RegressionRunner.exe -r:IMCSConnection.dll  -r:Elements.dll -r:ValidMoves.dll -r:AlphaBeta.dll -r:DepthFirstSearch.dll -r:Heuristics.dll -r:MTDF.dll -r:TranspositionTable.dll -r:AlphaBetaID.dll -r:ServerConnection.dll -r:TestTypes.dll -r:MoveRegression.dll -r:TypedInput.dll -r:DepthFirstSearchRegression.dll -r:BoardConstants.dll -r:Resources.dll -r:ZobristKeys.dll RegressionRunner.fs 

binaries/DepthFirstSearchRegression.dll : binaries DepthFirstSearch.fs binaries/TestTypes.dll
	$(fsc) $(optimize) -a --out:binaries/DepthFirstSearchRegression.dll -r:IMCSConnection.dll  -r:Elements.dll -r:ValidMoves.dll -r:AlphaBeta.dll -r:DepthFirstSearch.dll -r:Heuristics.dll -r:MTDF.dll -r:TranspositionTable.dll -r:AlphaBetaID.dll -r:ServerConnection.dll -r:TestTypes.dll DepthFirstSearch.fs

binaries : players/binaries config/binaries
	mkdir binaries
	cp players/binaries/* binaries
	cp config/binaries/* binaries

players/binaries :
	cd players && $(MAKE)

config/players:
	cd players &&  $(MAKE)


clean : cleanconf cleanplayers
	rm -rf binaries

cleanconf :
	cd config && $(MAKE) clean
	

cleanplayers : 
	cd playersection && $(MAKE) clean
