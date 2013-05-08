CC=gcc-4.8 -std=c99 -Wpedantic -Wall -Wextra -Werror -O3 -I headers -flto -L binaries
CCTEST=gcc-4.8 -std=c99 -Wpedantic -Wall -Wextra -Werror -g -I headers -L binaries
CCWH=gcc-4.8 -std=c99 -Wpedantic -Wall -Wextra -Werror -O3 -I headers -fwhole-program  -L binaries
debug=-g

all  : binaries/globals.o binaries/movehelpers.o binaries/movecalculation.o binaries/typedinput.o binaries/makemoves.o exe/vetplayer

debug : CC=$(CCTEST)
debug : all

test : all
	./exe/vetplayer testdata.txt
	

exe/vetplayer : binaries vetplayer.c elements.h globals.h binaries/globals.o binaries/makemoves.o binaries/typedinput.o
	$(CC) -o exe/vetplayer -flto-partition=none  -fwhole-program binaries/globals.o binaries/makemoves.o binaries/typedinput.o binaries/movehelpers.o binaries/movecalculation.o vetplayer.c

binaries/makemoves.o : binaries makemoves.c makemoves.h elements.h globals.h binaries/globals.o
	$(CC) -o binaries/makemoves.o -c -I binaries -L binaries  makemoves.c
	cp makemoves.h headers

binaries/typedinput.o : binaries typedinput.c typedinput.h elements.h binaries/globals.o
	$(CC) -o binaries/typedinput.o -c typedinput.c
	cp typedinput.h headers

binaries/movecalculation.o : binaries movecalculation.c movecalculation.h binaries/movehelpers.o elements.h
	$(CC) -o binaries/movecalculation.o -c  movecalculation.c
	cp movecalculation.h headers

binaries/movehelpers.o : binaries movehelpers.c movehelpers.h binaries/globals.o elements.h
	$(CC) -o binaries/movehelpers.o -c  movehelpers.c
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
	rm -rf exe

