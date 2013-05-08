#ifndef PRINCESSDOUNDO
#define PRINCESSDOUNDO
#include "elements.h"

//perform the update of applying the move to the board, and store the undo info in the undo struct
void doupdate(Move const, UndoState * const restrict);
//undoes an applied move
void undoupdate(UndoState const * const restrict);

#endif
