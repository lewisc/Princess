#include "stdlib.h"
#include "makemoves.h"
#include "elements.h"
#include "globals.h"
#include "movehelpers.h"

static void removePieceFromList(Color const color, Position const position)
{
    int i;
    switch(color)
    {
        case white :
            for(i = 0; i < currentstate.whitepieces.count; ++i)
            {
                if(currentstate.whitepieces.pieces[i].pos.xval == position.xval && currentstate.whitepieces.pieces[i].pos.yval == position.yval)
                {
                    //replace the deleted value with the last active value
                    //there's a problem if count = 0. that would imply removing a piece from an empty board
                    currentstate.whitepieces.pieces[i] = currentstate.whitepieces.pieces[currentstate.whitepieces.count-1]; 
                    --currentstate.whitepieces.count;
                    break;
                }
            }
            break;
        case black :
            for(i = 0; i < currentstate.blackpieces.count; ++i)
            {
                if(currentstate.blackpieces.pieces[i].pos.xval == position.xval && currentstate.blackpieces.pieces[i].pos.yval == position.yval)
                {
                    //replace the deleted value with the last active value
                    //there's a problem if count = 0. that would imply removing a piece from an empty board
                    currentstate.blackpieces.pieces[i] = currentstate.blackpieces.pieces[currentstate.blackpieces.count-1]; 
                    currentstate.blackpieces.count--;
                    break;
                }
            }
            break;
    }
}

//perform the update of applying the move to the board, and store the undo info in the undo struct
void doupdate(Move const move, UndoState * const restrict undo)
{
    //store the undo info
    undo->oldmoves = currentstate.availablemoves;
    undo->oldhash = currentstate.zobristhash;
    undo->oldvalue = currentstate.value;

    //TODO:may need to make a few of these things pointers, determine later
    undo->oldscore = currentstate.incrementor;

    //apply the update state
    currentstate.availablemoves = NULL;
    currentstate.turn = notColor(currentstate.turn);
    ++currentstate.movecount;
    //get the capture piece and the move piece
    BoardElements movepiece = board[RowCol(move.from.xval,move.from.yval)];
    BoardElements cappiece = board[RowCol(move.to.xval,move.to.yval)];

    //record what piece was capped where
    undo->cappedpiece.piece = cappiece;
    undo->cappedpiece.pos = move.to;

    if(cappiece == wking || cappiece == bking ||  isterminal())
    {
        currentstate.playing = false;
    }
    //capture the piece if necessary
    if(cappiece != empty)
    {
        removePieceFromList(getColor(cappiece), move.to);
    }
    //remove the piece that was advanced
    removePieceFromList(getColor(movepiece), move.from);

    //from square is empty
    board[RowCol(move.from.xval,move.from.yval)] = empty;
    //promote to queen or move piece
    //and add the new piece into the list
    if(movepiece == wpawn && move.to.yval == (ROWS-1))
    {
        board[RowCol(move.to.xval,move.to.yval)] = wqueen;
        currentstate.whitepieces.pieces[currentstate.whitepieces.count].piece = wqueen;
        currentstate.whitepieces.pieces[currentstate.whitepieces.count].pos.xval = move.to.xval;
        currentstate.whitepieces.pieces[currentstate.whitepieces.count].pos.yval = move.to.yval;
        ++currentstate.whitepieces.count;
    }
    else if(movepiece == bpawn && move.to.yval == 0)
    {
        board[RowCol(move.to.xval,move.to.yval)] = bqueen;
        currentstate.blackpieces.pieces[currentstate.blackpieces.count].piece = bqueen;
        currentstate.blackpieces.pieces[currentstate.blackpieces.count].pos.xval = move.to.xval;
        currentstate.blackpieces.pieces[currentstate.blackpieces.count].pos.yval = move.to.yval;
        ++currentstate.blackpieces.count;
    }
    else
    {
        board[RowCol(move.to.xval,move.to.yval)] = movepiece;
        switch(getColor(movepiece))
        {
            case white: currentstate.whitepieces.pieces[currentstate.whitepieces.count].piece = wqueen;
                        currentstate.whitepieces.pieces[currentstate.whitepieces.count].pos.xval = move.to.xval;
                        currentstate.whitepieces.pieces[currentstate.whitepieces.count].pos.yval = move.to.yval;
                        ++currentstate.whitepieces.count;
                    break;
            case black: currentstate.blackpieces.pieces[currentstate.blackpieces.count].piece = bqueen;
                        currentstate.blackpieces.pieces[currentstate.blackpieces.count].pos.xval = move.to.xval;
                        currentstate.blackpieces.pieces[currentstate.blackpieces.count].pos.yval = move.to.yval;
                        ++currentstate.blackpieces.count;
                    break;
        }
    }

    //update the zobrist
    currentstate.zobristhash = 0;

    //this section of code needs to know about the incrementor
    //
    //currentstate.incrementor;
    //currentstate.value;
}

//undoes an applied move
void undoupdate(UndoState const * const restrict undo)
{
    //unfold all the direct elements of the undo struct
    currentstate.availablemoves = undo->oldmoves;
    currentstate.zobristhash = undo->oldhash;
    currentstate.value = undo->oldvalue;
    currentstate.incrementor = undo->oldscore;
    currentstate.turn = notColor(currentstate.turn);
    --currentstate.movecount;
    currentstate.playing = true;
    //apply the undo piece updates
    board[RowCol(undo->frompiece.pos.xval,undo->frompiece.pos.yval)] = undo->frompiece.piece;
    board[RowCol(undo->cappedpiece.pos.xval,undo->cappedpiece.pos.yval)] = undo->cappedpiece.piece;

    //if the capped piece wasn't an empty, add it back in to the appropriate list of pieces
    if(undo->cappedpiece.piece != empty)
    {
        switch(getColor(undo->cappedpiece.piece))
        {
            case white: currentstate.whitepieces.pieces[currentstate.whitepieces.count] = undo->cappedpiece;
                        ++currentstate.whitepieces.count;
                    break;
            case black: currentstate.blackpieces.pieces[currentstate.blackpieces.count] = undo->cappedpiece;
                        ++currentstate.blackpieces.count;
                    break;
        }
    }
}
