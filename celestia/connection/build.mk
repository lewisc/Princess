connection_all : $(libs)/IMCSConnection.dll \
                 $(libs)/TypedInput.dll \
                 $(libs)/ServerConnection.dll

$(libs)/TypedInput.dll : connection/TypedInput.fs
	$(fsc) $(optimize) -a \
                       --out:$(libs)/TypedInput.dll \
                       connection/TypedInput.fs

$(libs)/IMCSConnection.dll : connection/IMCSConnection.fs
	$(fsc) $(optimize) -a \
                       --out:$(libs)/IMCSConnection.dll \
                       connection/IMCSConnection.fs

$(libs)/ServerConnection.dll : $(libs)/Primitives.dll \
                               $(libs)/IMCSConnection.dll \
                               $(libs)/TypedInput.dll \
                               $(libs)/GameState.dll \
                               connection/ServerConnection.fs 
	$(fsc) $(optimize) -r:Primitives.dll \
                       -r:IMCSConnection.dll \
                       -r:TypedInput.dll \
                       -r:GameState.dll \
                       -a \
                       --out:$(libs)/ServerConnection.dll \
                       connection/ServerConnection.fs
