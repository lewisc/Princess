IMCSIO_all : $(libs)/IMCSConnection.dll \
             $(libs)/TypedInput.dll \

$(libs)/TypedInput.dll : IMCSIO/TypedInput.fs
	$(fsc) $(optimize) -a \
                       --out:$(libs)/TypedInput.dll \
                       IMCSIO/TypedInput.fs

$(libs)/IMCSConnection.dll : IMCSIO/IMCSConnection.fs \
                             $(libs)/Primitives.dll	\
                             $(libs)/TypedInput.dll	\
                             $(libs)/GameState.dll 
	$(fsc) $(optimize) -r:Primitives.dll \
                       -r:TypedInput.dll \
                       -r:GameState.dll \
                       -a \
                       --out:$(libs)/IMCSConnection.dll \
                       IMCSIO/IMCSConnection.fs
