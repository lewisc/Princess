#include "headers/elements.h"
#include "headers/movehelpers.h"
#include "headers/globals.h"

//returns the list of knight moves
MoveList getKnightMoves(Position const pos)
{
    MoveList retval;
    Color const toplay = getColor(board[RowCol(pos.xval,pos.yval)]);
    //move has to be valid, either empty or a valid take
    //+2+1, +1+2, -2-1, -1-2, -1+2, +1-2,-2+1, +2-1
    if(isvalid(pos.xval+2,pos.yval+1) && ((board[RowCol(pos.xval+2,pos.yval+1)] == empty) || (getColor(board[RowCol(pos.xval+2,pos.yval+1)]) != toplay)))
    {
        retval.moves[retval.count].from = pos;
        retval.moves[retval.count].to.xval = pos.xval+2;
        retval.moves[retval.count].to.yval = pos.yval+1;
        retval.count++;
    }
    if(isvalid(pos.xval+1,pos.yval+2) && ((board[RowCol(pos.xval+1,pos.yval+2)] == empty) || (getColor(board[RowCol(pos.xval+1,pos.yval+2)]) != toplay)))
    {
        retval.moves[retval.count].from = pos;
        retval.moves[retval.count].to.xval = pos.xval+1;
        retval.moves[retval.count].to.yval = pos.yval+2;
        retval.count++;
    }
    if(isvalid(pos.xval-2,pos.yval-1) && ((board[RowCol(pos.xval-2,pos.yval-1)] == empty) || (getColor(board[RowCol(pos.xval-2,pos.yval-1)]) != toplay)))
    {
        retval.moves[retval.count].from = pos;
        retval.moves[retval.count].to.xval = pos.xval-2;
        retval.moves[retval.count].to.yval = pos.yval-1;
        retval.count++;
    }
    if(isvalid(pos.xval-1,pos.yval-2) && ((board[RowCol(pos.xval-1,pos.yval-2)] == empty) || (getColor(board[RowCol(pos.xval-1,pos.yval-2)]) != toplay)))
    {
        retval.moves[retval.count].from = pos;
        retval.moves[retval.count].to.xval = pos.xval-1;
        retval.moves[retval.count].to.yval = pos.yval-2;
        retval.count++;
    }
    if(isvalid(pos.xval-1,pos.yval+2) && ((board[RowCol(pos.xval-1,pos.yval+2)] == empty) || (getColor(board[RowCol(pos.xval-1,pos.yval+2)]) != toplay)))
    {
        retval.moves[retval.count].from = pos;
        retval.moves[retval.count].to.xval = pos.xval-1;
        retval.moves[retval.count].to.yval = pos.yval+2;
        retval.count++;
    }
    if(isvalid(pos.xval+1,pos.yval-2) && ((board[RowCol(pos.xval+1,pos.yval-2)] == empty) || (getColor(board[RowCol(pos.xval+1,pos.yval-2)]) != toplay)))
    {
        retval.moves[retval.count].from = pos;
        retval.moves[retval.count].to.xval = pos.xval+1;
        retval.moves[retval.count].to.yval = pos.yval-2;
        retval.count++;
    }
    if(isvalid(pos.xval-2,pos.yval+1) && ((board[RowCol(pos.xval-2,pos.yval+1)] == empty) || (getColor(board[RowCol(pos.xval-2,pos.yval+1)]) != toplay)))
    {
        retval.moves[retval.count].from = pos;
        retval.moves[retval.count].to.xval = pos.xval-2;
        retval.moves[retval.count].to.yval = pos.yval+1;
        retval.count++;
    }
    if(isvalid(pos.xval+2,pos.yval-1) && ((board[RowCol(pos.xval+2,pos.yval-1)] == empty) || (getColor(board[RowCol(pos.xval+2,pos.yval-1)]) != toplay)))
    {
        retval.moves[retval.count].from = pos ;
        retval.moves[retval.count].to.xval = pos.xval+2;
        retval.moves[retval.count].to.yval = pos.yval-1;
        retval.count++;
    }
    return retval;
}


//returns a list of pawn moves on the current board given a particular position
MoveList getPawnMoves(Position pos)
{
    MoveList retval;
    Color const toplay = getColor(board[RowCol(pos.xval,pos.yval)]);
    //going "forward" is different if you are black or white
    int const advance = (toplay == white) ? 1 : -1;
    //attack at advance+1,advance-1, move at advance
    if(isvalid(pos.xval+advance,pos.yval+1) && (!(board[RowCol(pos.xval+advance,pos.yval+1)] == empty) || (getColor(board[RowCol(pos.xval+advance,pos.yval+1)]) != toplay)))
    {
        retval.moves[retval.count].from = pos;
        retval.moves[retval.count].to.xval = pos.xval+advance;
        retval.moves[retval.count].to.yval = pos.yval+1;
        retval.count++;
    }
    if(isvalid(pos.xval+advance,pos.yval-1) && (!(board[RowCol(pos.xval+advance,pos.yval-1)] == empty) || (getColor(board[RowCol(pos.xval+advance,pos.yval-1)]) != toplay)))
    {
        retval.moves[retval.count].from = pos;
        retval.moves[retval.count].to.xval = pos.xval+advance;
        retval.moves[retval.count].to.yval = pos.yval-1;
        retval.count++;
    }
    if(isvalid(pos.xval+advance,pos.yval) && (board[RowCol(pos.xval+advance,pos.yval)] == empty))
    {
        retval.moves[retval.count].from = pos ;
        retval.moves[retval.count].to.xval = pos.xval+advance;
        retval.moves[retval.count].to.yval = pos.yval;
        retval.count++;
    }
    return retval;
}

MoveList getKingMoves(Position const pos)
{
    MoveList retval;
    Color const toplay = getColor(board[RowCol(pos.xval,pos.yval)]);
    //move has to be valid, either empty or a valid take
    //+1+1, +1+0, +0+1, +1-1,-1+1,-1-1, -1+0, +0-1
    if(isvalid(pos.xval+1,pos.yval+1) && ((board[RowCol(pos.xval+1,pos.yval+1)] == empty) || (getColor(board[RowCol(pos.xval+1,pos.yval+1)]) != toplay)))
    {
        retval.moves[retval.count].from = pos ;
        retval.moves[retval.count].to.xval = pos.xval+1;
        retval.moves[retval.count].to.yval = pos.yval+1;
        retval.count++;
    }
    if(isvalid(pos.xval+1,pos.yval) && ((board[RowCol(pos.xval+1,pos.yval)] == empty) || (getColor(board[RowCol(pos.xval+1,pos.yval)]) != toplay)))
    {
        retval.moves[retval.count].from = pos ;
        retval.moves[retval.count].to.xval = pos.xval+1;
        retval.moves[retval.count].to.yval = pos.yval;
        retval.count++;
    }
    if(isvalid(pos.xval,pos.yval+1) && ((board[RowCol(pos.xval,pos.yval+1)] == empty) || (getColor(board[RowCol(pos.xval,pos.yval+1)]) != toplay)))
    {
        retval.moves[retval.count].from = pos ;
        retval.moves[retval.count].to.xval = pos.xval;
        retval.moves[retval.count].to.yval = pos.yval+1;
        retval.count++;
    }
    if(isvalid(pos.xval+1,pos.yval-1) && ((board[RowCol(pos.xval+1,pos.yval-1)] == empty) || (getColor(board[RowCol(pos.xval+1,pos.yval-1)]) != toplay)))
    {
        retval.moves[retval.count].from = pos ;
        retval.moves[retval.count].to.xval = pos.xval+1;
        retval.moves[retval.count].to.yval = pos.yval-1;
        retval.count++;
    }
    if(isvalid(pos.xval-1,pos.yval+1) && ((board[RowCol(pos.xval-1,pos.yval+1)] == empty) || (getColor(board[RowCol(pos.xval-1,pos.yval+1)]) != toplay)))
    {
        retval.moves[retval.count].from = pos ;
        retval.moves[retval.count].to.xval = pos.xval-1;
        retval.moves[retval.count].to.yval = pos.yval+1;
        retval.count++;
    }
    if(isvalid(pos.xval-1,pos.yval-1) && ((board[RowCol(pos.xval-1,pos.yval-1)] == empty) || (getColor(board[RowCol(pos.xval-1,pos.yval-1)]) != toplay)))
    {
        retval.moves[retval.count].from = pos ;
        retval.moves[retval.count].to.xval = pos.xval-1;
        retval.moves[retval.count].to.yval = pos.yval-1;
        retval.count++;
    }
    if(isvalid(pos.xval-1,pos.yval) && ((board[RowCol(pos.xval-1,pos.yval)] == empty) || (getColor(board[RowCol(pos.xval-1,pos.yval)]) != toplay)))
    {
        retval.moves[retval.count].from = pos ;
        retval.moves[retval.count].to.xval = pos.xval-1;
        retval.moves[retval.count].to.yval = pos.yval;
        retval.count++;
    }
    if(isvalid(pos.xval,pos.yval-1) && ((board[RowCol(pos.xval,pos.yval-1)] == empty) || (getColor(board[RowCol(pos.xval,pos.yval-1)]) != toplay)))
    {
        retval.moves[retval.count].from = pos ;
        retval.moves[retval.count].to.xval = pos.xval;
        retval.moves[retval.count].to.yval = pos.yval-1;
        ++retval.count;
    }
    return retval;
}

//returns a list of the moves available to a rook
MoveList getRookMoves(Position const pos)
{

    MoveList retval;
    Color const toplay = getColor(board[RowCol(pos.xval,pos.yval)]);
    int i;
    //scan +x,-x,+y,-y
    for(i=pos.xval+1;i<ROWS;++i)
    {
        if(isvalid(i,pos.yval) && (board[RowCol(i,pos.yval)] == empty))
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = pos.yval;
            ++retval.count;
        }
        else if(getColor(board[RowCol(i,pos.yval)]) != toplay)
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = pos.yval;
            ++retval.count;
            break;
        }
        else
        {
            break;
        }
    }
    for(i=pos.xval-1;i>=0;--i)
    {
        if(isvalid(i,pos.yval) && (board[RowCol(i,pos.yval)] == empty))
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = pos.yval;
            ++retval.count;
        }
        else if(getColor(board[RowCol(i,pos.yval)]) != toplay)
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = pos.yval;
            ++retval.count;
            break;
        }
        else
        {
            break;
        }
    }
    for(i=pos.yval+1;i<ROWS;++i)
    {
        if(isvalid(pos.xval,i) && (board[RowCol(pos.xval,i)] == empty))
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = pos.xval;
            retval.moves[retval.count].to.yval = i;
            ++retval.count;
        }
        else if(getColor(board[RowCol(pos.xval,i)]) != toplay)
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = pos.xval;
            retval.moves[retval.count].to.yval = i;
            ++retval.count;
            break;
        }
        else
        {
            break;
        }
    }
    for(i=pos.yval-1;i>=0;--i)
    {
        if(isvalid(pos.xval,i) && (board[RowCol(pos.xval,i)] == empty))
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = pos.xval;
            retval.moves[retval.count].to.yval = i;
            ++retval.count;
        }
        else if(getColor(board[RowCol(pos.xval,i)]) != toplay)
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = pos.xval;
            retval.moves[retval.count].to.yval = i;
            ++retval.count;
            break;
        }
        else
        {
            break;
        }
    }

    return retval;
}

MoveList getQueenMoves(Position const pos)
{
    MoveList retval;
    Color const toplay = getColor(board[RowCol(pos.xval,pos.yval)]);
    int i ,j;
    //scan +i+0,-i+0, +0+i, +0-i, +i+i, +i-i, -i+i, -i-i
    for(i=pos.xval+1;i<ROWS;++i)
    {
        if(isvalid(i,pos.yval) && (board[RowCol(i,pos.yval)] == empty))
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = pos.yval;
            ++retval.count;
        }
        else if(getColor(board[RowCol(i,pos.yval)]) != toplay)
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = pos.yval;
            ++retval.count;
            break;
        }
        else
        {
            break;
        }
    }
    for(i=pos.xval-1;i>=0;--i)
    {
        if(isvalid(i,pos.yval) && (board[RowCol(i,pos.yval)] == empty))
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = pos.yval;
            ++retval.count;
        }
        else if(getColor(board[RowCol(i,pos.yval)]) != toplay)
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = pos.yval;
            ++retval.count;
            break;
        }
        else
        {
            break;
        }
    }
    for(i=pos.yval+1;i<ROWS;++i)
    {
        if(isvalid(pos.xval,i) && (board[RowCol(pos.xval,i)] == empty))
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = pos.xval;
            retval.moves[retval.count].to.yval = i;
            ++retval.count;
        }
        else if(getColor(board[RowCol(pos.xval,i)]) != toplay)
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = pos.xval;
            retval.moves[retval.count].to.yval = i;
            ++retval.count;
            break;
        }
        else
        {
            break;
        }
    }
    for(i=pos.yval-1;i>=0;--i)
    {
        if(isvalid(pos.xval,i) && (board[RowCol(pos.xval,i)] == empty))
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = pos.xval;
            retval.moves[retval.count].to.yval = i;
            ++retval.count;
        }
        else if(getColor(board[RowCol(pos.xval,i)]) != toplay)
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = pos.xval;
            retval.moves[retval.count].to.yval = i;
            ++retval.count;
            break;
        }
        else
        {
            break;
        }
    }
    for(i=pos.xval+1,j=pos.yval+1;i<ROWS && j<COLS; ++i,++j)
    {
        if(isvalid(i,j) && (board[RowCol(i,j)] == empty))
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = j;
            ++retval.count;
        }
        else if(getColor(board[RowCol(i,j)]) != toplay)
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = j;
            ++retval.count;
            break;
        }
        else
        {
            break;
        }
    }
    for(i=pos.xval+1,j=pos.yval-1;i<ROWS && j>=0; ++i,--j)
    {
        if(isvalid(i,j) && (board[RowCol(i,j)] == empty))
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = j;
            ++retval.count;
        }
        else if(getColor(board[RowCol(i,j)]) != toplay)
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = j;
            ++retval.count;
            break;
        }
        else
        {
            break;
        }
    }
    for(i=pos.xval+1,j=pos.yval-1;i<ROWS && j>=0; ++i,--j)
    {
        if(isvalid(i,j) && (board[RowCol(i,j)] == empty))
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = j;
            ++retval.count;
        }
        else if(getColor(board[RowCol(i,j)]) != toplay)
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = j;
            ++retval.count;
            break;
        }
        else
        {
            break;
        }
    }
    for(i=pos.xval-1,j=pos.yval-1;i<ROWS && j>=0; --i,--j)
    {
        if(isvalid(i,j) && (board[RowCol(i,j)] == empty))
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = j;
            ++retval.count;
        }
        else if(getColor(board[RowCol(i,j)]) != toplay)
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = j;
            ++retval.count;
            break;
        }
        else
        {
            break;
        }
    }
    return retval;
}

MoveList getBishopMoves(Position const pos)
{
    MoveList retval;
    Color const toplay = getColor(board[RowCol(pos.xval,pos.yval)]);
    int i, j;
    //+i+j,+i-j,-i+j, -i-j, nocapture +1+0,+0+1, -1+0,+0-1
    for(i=pos.xval+1,j=pos.yval+1;i<ROWS && j<COLS; ++i,++j)
    {
        if(isvalid(i,j) && (board[RowCol(i,j)] == empty))
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = j;
            ++retval.count;
        }
        else if(getColor(board[RowCol(i,j)]) != toplay)
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = j;
            ++retval.count;
            break;
        }
        else
        {
            break;
        }
    }
    for(i=pos.xval+1,j=pos.yval-1;i<ROWS && j>=0; ++i,--j)
    {
        if(isvalid(i,j) && (board[RowCol(i,j)] == empty))
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = j;
            ++retval.count;
        }
        else if(getColor(board[RowCol(i,j)]) != toplay)
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = j;
            ++retval.count;
            break;
        }
        else
        {
            break;
        }
    }
    for(i=pos.xval+1,j=pos.yval-1;i<ROWS && j>=0; ++i,--j)
    {
        if(isvalid(i,j) && (board[RowCol(i,j)] == empty))
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = j;
            ++retval.count;
        }
        else if(getColor(board[RowCol(i,j)]) != toplay)
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = j;
            ++retval.count;
            break;
        }
        else
        {
            break;
        }
    }
    for(i=pos.xval-1,j=pos.yval-1;i<ROWS && j>=0; --i,--j)
    {
        if(isvalid(i,j) && (board[RowCol(i,j)] == empty))
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = j;
            ++retval.count;
        }
        else if(getColor(board[RowCol(i,j)]) != toplay)
        {
            retval.moves[retval.count].from = pos;
            retval.moves[retval.count].to.xval = i;
            retval.moves[retval.count].to.yval = j;
            ++retval.count;
            break;
        }
        else
        {
            break;
        }
    }
    if(isvalid(pos.xval+1,pos.yval) && (board[RowCol(pos.xval+1,pos.yval)] == empty))
    {
        retval.moves[retval.count].from = pos;
        retval.moves[retval.count].to.xval = pos.xval+1;
        retval.moves[retval.count].to.yval = pos.yval;
        ++retval.count;
    }
    if(isvalid(pos.xval-1,pos.yval) && (board[RowCol(pos.xval-1,pos.yval)] == empty))
    {
        retval.moves[retval.count].from = pos;
        retval.moves[retval.count].to.xval = pos.xval-1;
        retval.moves[retval.count].to.yval = pos.yval;
        ++retval.count;
    }
    if(isvalid(pos.xval,pos.yval+1) && (board[RowCol(pos.xval,pos.yval+1)] == empty))
    {
        retval.moves[retval.count].from = pos;
        retval.moves[retval.count].to.xval = pos.xval;
        retval.moves[retval.count].to.yval = pos.yval+1;
        ++retval.count;
    }
    if(isvalid(pos.xval,pos.yval-1) && (board[RowCol(pos.xval,pos.yval-1)] == empty))
    {
        retval.moves[retval.count].from = pos;
        retval.moves[retval.count].to.xval = pos.xval;
        retval.moves[retval.count].to.yval = pos.yval-1;
        ++retval.count;
    }
    return retval;
}
