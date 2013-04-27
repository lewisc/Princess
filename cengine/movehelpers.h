#ifndef PRINCESSMOVEHELPERS
#define PRINCESSMOVEHELPERS
#include "elements.h"

//prints a move
//needs an array of length 13
void moveToString(char * const, Move const);
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
void boardToString(char * const retval);

#endif
