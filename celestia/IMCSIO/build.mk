IMCSIO_all : $(libs)/IMCSConnection.dll \
                 $(libs)/TypedInput.dll \
                 $(libs)/ServerConnection.dll

$(libs)/TypedInput.dll : IMCSIO/TypedInput.fs
	$(fsc) $(optimize) -a \
                       --out:$(libs)/TypedInput.dll \
                       IMCSIO/TypedInput.fs

$(libs)/IMCSConnection.dll : IMCSIO/IMCSConnection.fs
	$(fsc) $(optimize) -a \
                       --out:$(libs)/IMCSConnection.dll \
                       IMCSIO/IMCSConnection.fs

$(libs)/ServerConnection.dll : $(libs)/Primitives.dll \
                               $(libs)/IMCSConnection.dll \
                               $(libs)/TypedInput.dll \
                               $(libs)/GameState.dll \
                               IMCSIO/ServerConnection.fs 
	$(fsc) $(optimize) -r:Primitives.dll \
                       -r:IMCSConnection.dll \
                       -r:TypedInput.dll \
                       -r:GameState.dll \
                       -a \
                       --out:$(libs)/ServerConnection.dll \
                       IMCSIO/ServerConnection.fs
