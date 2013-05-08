#include "globals.h"
#include "elements.h"
#include "stdbool.h"
#include "stdio.h"
#include "stdlib.h"

//The Board, gets updated, not copied
 BoardElements board [30] = { wking,wqueen,wbishop,wknight,wrook,
                              wpawn,wpawn, wpawn,  wpawn,  wpawn,
                              empty,empty, empty,  empty,  empty,
                              empty,empty, empty,  empty,  empty,
                              bpawn,bpawn, bpawn,  bpawn,  bpawn,
                              bking,bqueen,bbishop,bknight,brook};

//A List of the different things that can
//be in a square, shown to be useful in varia
BoardElements const squares[13] = {wking,wqueen,wbishop,wknight,
                                   wrook,wpawn, empty, bpawn,bking,
                                   bqueen, bbishop, bknight,brook};
GameState currentstate; 

//throws an exception and exits
void exception(char const * const restrict errormessage, int const errnum)
{
    printf("error was: %s\n",errormessage);
    exit(errnum);
}

