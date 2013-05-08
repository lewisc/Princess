#ifndef PRINCESSMOVEHELPERS
#define PRINCESSMOVEHELPERS
#include "elements.h"

#define isvalid(x,y) (x >=0 && x < COLS && y >= 0 && y < ROWS)

//prints a move
//needs an array of length 13
void moveToString(char * const restrict, Move const);
//prints a color
char colorToString(Color const color);
//turns a piece into a character
char pieceToString(BoardElements const piece);
//returns the piececolor of the piece
//or errors out, on error could return black (shouldn't)
Color getColor(BoardElements const piece);
//populates the list with all the pieces on the current board
PieceList getPiecesByColor(Color const color);
//determines if the game has ended
bool isterminal();
//prints the board to a string
//the input should be an array of length 31
void boardToString(char * const restrict retval);
//reverses the color
Color notColor(Color const);
//resets the board state
void reset();

#endif
