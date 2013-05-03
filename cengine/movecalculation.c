#include "elements.h"
#include "movehelpers.h"
#include "globals.h"
#include "movecalculation.h"

//helper function to update the move list
static inline void updateMoveList(MoveList * const restrict retval, Position const pos, int const xdelta, int const ydelta)
{
        retval->moves[retval->count].from = pos;
        retval->moves[retval->count].to.xval = pos.xval+xdelta;
        retval->moves[retval->count].to.yval = pos.yval+ydelta;
        ++retval->count;
}

//returns true if the position is takeable or empty by the color
static inline bool emptyOrTake(Position const pos, Color const toplay)
{
    if(isvalid(pos.xval+2,pos.yval-1) && ((board[RowCol(pos.xval+2,pos.yval-1)] == empty) || (getColor(board[RowCol(pos.xval+2,pos.yval-1)]) != toplay)))
    {
        return true;
    }
    else
    {
        return false;
    }
}

//updates the list if the position is takeable, returns true if you should continue, false otherwise
static inline bool continueOrUpdate(MoveList * const restrict retval, Position const pos, Color const toplay, int const i, int const j)
{
        if(isvalid(pos.xval+i,pos.yval+j) && (board[RowCol(pos.xval+i,pos.yval+j)] == empty))
        {
            updateMoveList(retval, pos, i, j);
            return true;
        }
        else if(getColor(board[RowCol(pos.xval+i,pos.yval+j)]) != toplay)
        {
            updateMoveList(retval, pos, i, j);
            return false;
        }
        else
        {
            return false;
        }
}


//returns the list of knight moves
static void getKnightMoves(Position const pos, MoveList * const restrict retval)
{
    Color const toplay = getColor(board[RowCol(pos.xval,pos.yval)]);
    //move has to be valid, either empty or a valid take
    //+2+1, +1+2, -2-1, -1-2, -1+2, +1-2,-2+1, +2-1
    if(emptyOrTake(pos,toplay))
    {
        updateMoveList(retval, pos, 2, 1);
    }
    if(emptyOrTake(pos,toplay))
    {
        updateMoveList(retval, pos, 1, 2);
    }
    if(emptyOrTake(pos,toplay))
    {
        updateMoveList(retval, pos, -2, -1);
    }
    if(emptyOrTake(pos,toplay))
    {
        updateMoveList(retval, pos, -1, -2);
    }
    if(emptyOrTake(pos,toplay))
    {
        updateMoveList(retval, pos, -1, 2);
    }
    if(emptyOrTake(pos,toplay))
    {
        updateMoveList(retval, pos, 1, -2);
    }
    if(emptyOrTake(pos,toplay))
    {
        updateMoveList(retval, pos, -2, 1);
    }
    if(emptyOrTake(pos,toplay))
    {
        updateMoveList(retval, pos, 2, -1);
    }
}

//returns a list of pawn moves on the current board given a particular position
static void getPawnMoves(Position const pos, MoveList * const restrict retval)
{
    Color const toplay = getColor(board[RowCol(pos.xval,pos.yval)]);
    //going "forward" is different if you are black or white
    int const advance = (toplay == white) ? 1 : -1;
    //attack at advance+1,advance-1, move at advance
    if(isvalid(pos.xval+advance,pos.yval+1) && (!(board[RowCol(pos.xval+advance,pos.yval+1)] == empty) || (getColor(board[RowCol(pos.xval+advance,pos.yval+1)]) != toplay)))
    {
        updateMoveList(retval, pos, advance, -1);
    }
    if(isvalid(pos.xval+advance,pos.yval-1) && (!(board[RowCol(pos.xval+advance,pos.yval-1)] == empty) || (getColor(board[RowCol(pos.xval+advance,pos.yval-1)]) != toplay)))
    {
        updateMoveList(retval, pos, advance, -1);
    }
    if(isvalid(pos.xval+advance,pos.yval) && (board[RowCol(pos.xval+advance,pos.yval)] == empty))
    {
        updateMoveList(retval, pos, advance, 0);
    }
}

static void getKingMoves(Position const pos, MoveList * const restrict retval)
{
    Color const toplay = getColor(board[RowCol(pos.xval,pos.yval)]);
    //move has to be valid, either empty or a valid take
    //+1+1, +1+0, +0+1, +1-1,-1+1,-1-1, -1+0, +0-1
    if(emptyOrTake(pos,toplay))
    {
        updateMoveList(retval, pos, 1, 1);
    }
    if(emptyOrTake(pos,toplay))
    {
        updateMoveList(retval, pos, 1, 0);
    }
    if(emptyOrTake(pos,toplay))
    {
        updateMoveList(retval, pos, 0, 1);
    }
    if(emptyOrTake(pos,toplay))
    {
        updateMoveList(retval, pos, 1, -1);
    }
    if(emptyOrTake(pos,toplay))
    {
        updateMoveList(retval, pos, -1, 1);
    }
    if(emptyOrTake(pos,toplay))
    {
        updateMoveList(retval, pos, -1, -1);
    }
    if(emptyOrTake(pos,toplay))
    {
        updateMoveList(retval, pos, -1, 0);
    }
    if(emptyOrTake(pos,toplay))
    {
        updateMoveList(retval, pos, 0, -1);
    }
}

//returns a list of the moves available to a rook
static void getRookMoves(Position const pos, MoveList * const restrict retval)
{
    Color const toplay = getColor(board[RowCol(pos.xval,pos.yval)]);
    int i;
    //scan +x,-x,+y,-y
    for(i=1;(pos.xval+i)<ROWS;++i)
    {
        if(continueOrUpdate(retval, pos, toplay, i, 0) == false)
        {
            break;
        }
    }
    for(i=-1;(pos.xval+i)>=0;--i)
    {
        if(continueOrUpdate(retval, pos, toplay, i, 0) == false)
        {
            break;
        }
    }
    for(i=1;(pos.yval+i)<COLS;++i)
    {
        if(continueOrUpdate(retval, pos, toplay, 0, i) == false)
        {
            break;
        }
    }
    for(i=-1;(pos.yval+i)>=0;--i)
    {
        if(continueOrUpdate(retval, pos, toplay, 0, i) == false)
        {
            break;
        }
    }
}

static void getQueenMoves(Position const pos, MoveList * const restrict retval)
{
    Color const toplay = getColor(board[RowCol(pos.xval,pos.yval)]);
    int i ,j;
    //scan +i+0,-i+0, +0+i, +0-i, +i+i, +i-i, -i+i, -i-i
    for(i=1;(pos.xval+i)<ROWS;++i)
    {
        if(continueOrUpdate(retval, pos, toplay, i, 0) == false)
        {
            break;
        }
    }
    for(i=-1;(pos.xval+i)>=0;--i)
    {
        if(continueOrUpdate(retval, pos, toplay, i, 0) == false)
        {
            break;
        }
    }
    for(i=1;(pos.yval+i)<COLS;++i)
    {
        if(continueOrUpdate(retval, pos, toplay, 0, i) == false)
        {
            break;
        }
    }
    for(i=pos.yval-1;i>=0;--i)
    {
        if(continueOrUpdate(retval, pos, toplay, 0, i) == false)
        {
            break;
        }
    }
    for(i=1,j=1;(pos.xval+i)<ROWS && (pos.yval+j)<COLS; ++i,++j)
    {
        if(continueOrUpdate(retval, pos, toplay, i, j) == false)
        {
            break;
        }
    }
    for(i=1,j=-1;(pos.xval+i)<ROWS && (pos.yval+j)>=0; ++i,--j)
    {
        if(continueOrUpdate(retval, pos, toplay, i, j) == false)
        {
            break;
        }
    }
    for(i=1,j=-1;(pos.xval+i)<ROWS && (pos.yval+j)>=0; ++i,--j)
    {
        if(continueOrUpdate(retval, pos, toplay, i, j) == false)
        {
            break;
        }
    }
    for(i=-1,j=-1;(pos.xval+i)<ROWS && (pos.yval+j)>=0; --i,--j)
    {
        if(continueOrUpdate(retval, pos, toplay, i, j) == false)
        {
            break;
        }
    }
}

static void getBishopMoves(Position const pos, MoveList * const restrict retval)
{
    Color const toplay = getColor(board[RowCol(pos.xval,pos.yval)]);
    int i, j;
    //+i+j,+i-j,-i+j, -i-j, nocapture +1+0,+0+1, -1+0,+0-1
    for(i=1,j=1;(pos.xval+i)<ROWS && (pos.yval+j)<COLS; ++i,++j)
    {
        if(continueOrUpdate(retval, pos, toplay, i, j) == false)
        {
            break;
        }
    }
    for(i=1,j=-1;(pos.xval+i)<ROWS && (pos.yval+j)>=0; ++i,--j)
    {
        if(continueOrUpdate(retval, pos, toplay, i, j) == false)
        {
            break;
        }
    }
    for(i=1,j=-1;(pos.xval+i)<ROWS && (pos.yval+j)>=0; ++i,--j)
    {
        if(continueOrUpdate(retval, pos, toplay, i, j) == false)
        {
            break;
        }
    }
    for(i=-1,j=-1;(pos.xval+i)<ROWS && (pos.yval+j)>=0; --i,--j)
    {
        if(continueOrUpdate(retval, pos, toplay, i, j) == false)
        {
            break;
        }

    }
    if(isvalid(pos.xval+1,pos.yval) && (board[RowCol(pos.xval+1,pos.yval)] == empty))
    {
        updateMoveList(retval, pos, 1, 0);
    }
    if(isvalid(pos.xval-1,pos.yval) && (board[RowCol(pos.xval-1,pos.yval)] == empty))
    {
        updateMoveList(retval, pos, -1, 0);
    }
    if(isvalid(pos.xval,pos.yval+1) && (board[RowCol(pos.xval,pos.yval+1)] == empty))
    {
        updateMoveList(retval, pos, 0, -1);
    }
    if(isvalid(pos.xval,pos.yval-1) && (board[RowCol(pos.xval,pos.yval-1)] == empty))
    {
        updateMoveList(retval, pos, 0, -1);
    }
}

void calculateMoves(PieceList const * const restrict pieces, MoveList * const restrict retval)
{
    int i;
    for(i=0;i<pieces->count;++i)
    {
        switch(pieces->pieces[i].piece)
        {
            case wking: case bking:
                getKingMoves(pieces->pieces[i].pos,retval);
                break;
            case wrook: case brook:
                getRookMoves(pieces->pieces[i].pos,retval);
                break;
            case wbishop: case bbishop:
                getBishopMoves(pieces->pieces[i].pos,retval);
                break;
            case wpawn: case bpawn:
                getPawnMoves(pieces->pieces[i].pos,retval);
                break;
            case wqueen: case bqueen:
                getQueenMoves(pieces->pieces[i].pos,retval);
                break;
            case wknight: case bknight:
                getKnightMoves(pieces->pieces[i].pos,retval);
                break;
            case empty:
                exception("calculate moves encountered an empty square", 3);
                break;
            default:
                exception("calculate moves encountered a non-existent piece",3);
                break;
        }
    }
}
