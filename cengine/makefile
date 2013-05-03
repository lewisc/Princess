CC=gcc-4.8 -std=c99 -Wpedantic -Wall -Wextra -Werror -O3 -I headers -flto -L binaries
debug=-g

all  : binaries/globals.o binaries/movehelpers.o binaries/movecalculation.o binaries/typedinput.o binaries/makemoves.o exe/vetplayer

exe/vetplayer : binaries vetplayer.c elements.h globals.h binaries/globals.o binaries/makemoves.o
	$(CC) -o exe/vetplayer -I binaries -L binaries  vetplayer.c

binaries/makemoves.o : binaries makemoves.c makemoves.h elements.h globals.h binaries/globals.o
	$(CC) -o binaries/makemoves.o -c -I binaries -L binaries  makemoves.c
	cp makemoves.h headers

binaries/typedinput.o : binaries typedinput.c typedinput.h elements.h binaries/globals.o
	$(CC) -o binaries/typedinput.o -c -I binaries -L binaries  typedinput.c
	cp typedinput.h headers

binaries/movecalculation.o : binaries movecalculation.c movecalculation.h binaries/movehelpers.o elements.h
	$(CC) -o binaries/movecalculation.o -c -I binaries -L binaries  movecalculation.c
	cp movecalculation.h headers

binaries/movehelpers.o : binaries movehelpers.c movehelpers.h binaries/globals.o elements.h
	$(CC) -o binaries/movehelpers.o -c -I binaries -L binaries  movehelpers.c
	cp movehelpers.h headers

binaries/globals.o : binaries globals.c globals.h elements.h
	$(CC) -o binaries/globals.o  -c globals.c
	cp globals.h headers
	cp elements.h headers

binaries headers : 
	mkdir -p binaries
	mkdir -p headers
	mkdir -p exe

clean : 
	rm -rf binaries
	rm -rf headers

