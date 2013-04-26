CC=gcc -Wall -Wextra -Werror -O4
debug=-g

all  : binaries/global.o

binaries/global.o : binaries globals.c globals.h
	$(CC) -o binaries/global.o  -c globals.c

binaries : 
	mkdir -p binaries

clean : 
	rm -rf binaries
