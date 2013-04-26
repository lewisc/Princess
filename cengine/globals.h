#ifndef PRINCESSGLOBALS
#define PRINCESSGLOBALS

//The index macro
#define RowCol(x,y) (x*5)+y

//The Fetch move value x/y from the moves list
#define fetchFromX(m) 2*m
#define fetchFromY(m) 2*m+1
#define fetchToX(m) 2*m+2
#define fetchToY(m) 2*m+3

//Some useful values when dealing with the board
#define cols 5
#define rows 6
#define maxCol 4
#define maxRow 5
#define inf 1000

//The board, global variable
extern char board[30];
extern char const * const squares;

//The move list
extern int moves[2600];
extern int lastValidMove;

#endif
