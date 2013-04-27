CC=gcc -Wall -Wextra -Werror -O4 -I headers -L binaries
debug=-g

all  : binaries/globals.o binaries/movehelpers.o

binaries/movehelpers.o : binaries movehelpers.c movehelpers.h binaries/globals.o
	$(CC) -o binaries/movehelpers.o -c -I binaries -L binaries  movehelpers.c
	cp movehelpers.h headers

binaries/globals.o : binaries globals.c globals.h
	$(CC) -o binaries/globals.o  -c globals.c
	cp globals.h headers
	cp elements.h headers

binaries headers : 
	mkdir -p binaries
	mkdir -p headers

clean : 
	rm -rf binaries
	rm -rf headers

