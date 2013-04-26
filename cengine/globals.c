#include "globals.h"

//The Board, get's updated, not copied
char board [30] = 
        { 'K','Q','B','N','R',
          'P','P','P','P','P',
          '.','.','.','.','.',
          '.','.','.','.','.',
          'p','p','p','p','p',
          'r','n','b','q','k'};

//A List of the different things that can
//be in a square, shown to be useful in varia
char const * const squares = "KQBNRPprnbqk.";

int moves[2600];

int lastValidMove = -1;
