
connection_all : binaries/IMCSConnection.dll \
                 binaries/ServerConnection.dll

binaries/IMCSConnection.dll : connection/IMCSConnection.fs
	$(fsc) $(optimize) -a \
                       --out:binaries/IMCSConnection.dll \
                       connection/IMCSConnection.fs

binaries/ServerConnection.dll : binaries/Elements.dll \
                                binaries/MoveHelpers.dll \
                                binaries/IMCSConnection.dll \
                                binaries/ValidMoves.dll \
                                binaries/TypedInput.dll \
                                connection/ServerConnection.fs 
	$(fsc) $(optimize) -r:binaries/Elements.dll \
                       -r:binaries/MoveHelpers.dll \
                       -r:binaries/IMCSConnection.dll \
                       -r:binaries/ValidMoves.dll \
                       -r:binaries/TypedInput.dll \
                        -a \
                       --out:binaries/ServerConnection.dll \
                       connection/ServerConnection.fs
