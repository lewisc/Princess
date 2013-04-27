#ifndef PRINCESSELEMENTS
#define PRINCESSELEMENTS
#include "stdbool.h"
//the different sides colors
typedef enum
{
    white,
    black
} Color;

//a position on the board
typedef struct
{
    int xval;
    int yval;
} Position;

//a move from a square to another square
typedef struct
{
    Position from;
    Position to;
} Move;

//the different possible elements
//that can be contained in a square
typedef enum 
{
    wking,
    wqueen,
    wrook,
    wknight,
    wpawn,
    wbishop,
    bking,
    bqueen,
    brook,
    bknight,
    bpawn,
    bbishop,
    empty
} BoardElements;

//A score struct, used for the purposes
//of incremental evalution
typedef struct
{
    int whitescore;
    int blackscore;
    int advancementscore;
    int whitepawnscore;
    int blackpawnscore;
} Score;

//everything needed to characterize a piece on a board
typedef struct
{
    Position pos;
    BoardElements piece;
} BoardPiece;

//Lists are done as a static array and a count, which is the number
//of used elments i.e. 0 is 0 used elements, 1 is 1 used elements,
//so for loops are from i = 0 to i< count.

//list of pieces, done this way for efficency
typedef struct
{
   BoardPiece  pieces[20];
   int count;
} PieceList;

//list of moves, done for efficiency
typedef struct
{
    Move moves[80];
    int count;
} MoveList;

typedef struct
{
    Color turn;
    bool playing;
    MoveList AvailableMoves;
    int movecount;
    PieceList whitepieces;
    PieceList blackpieces;
    long int zobristhash;
    Score incrementor;
    int value;
} GameState;

//everything needed to perform an undo
typedef struct
{
    MoveList oldmoves;
    PieceList blackpieces;
    PieceList whitepieces;
    long int oldhash;
    Score oldscore;
} UndoState;
    
#endif
