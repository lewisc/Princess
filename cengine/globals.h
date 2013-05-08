#ifndef PRINCESSGLOBALS
#define PRINCESSGLOBALS
#include "elements.h"

//The index function
int RowCol(int const c, int const r);

//Some useful values when dealing with the board
#define COLS 5
#define ROWS 6
#define INF  1000
#define GAMELENGTH 82

//The board, global variable
extern BoardElements board[30];
extern GameState currentstate;

//exception handling
void exception(char const * const restrict, int const);

#endif
