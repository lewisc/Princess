connection_all : $(libs)/IMCSConnection.dll \
                 $(libs)/ServerConnection.dll

$(libs)/IMCSConnection.dll : connection/IMCSConnection.fs
	$(fsc) $(optimize) -a \
                       --out:$(libs)/IMCSConnection.dll \
                       connection/IMCSConnection.fs

$(libs)/ServerConnection.dll : $(libs)/Primitives.dll \
                               $(libs)/MoveHelpers.dll \
                               $(libs)/IMCSConnection.dll \
                               $(libs)/ValidMoves.dll \
                               $(libs)/TypedInput.dll \
                               connection/ServerConnection.fs 
	$(fsc) $(optimize) -r:Primitives.dll \
                       -r:MoveHelpers.dll \
                       -r:IMCSConnection.dll \
                       -r:ValidMoves.dll \
                       -r:TypedInput.dll \
                       -a \
                       --out:$(libs)/ServerConnection.dll \
                       connection/ServerConnection.fs
