#include "movehelpers.h"
#include "elements.h"
#include "globals.h"
#include "stdbool.h"
#include "stdio.h"

//prints a move
//needs an array of length 13
void moveToString(char * const restrict retval,Move const move)
{
    if(sprintf(retval,"!%d%d-%d%d",move.from.xval,move.from.yval,move.to.xval,move.to.yval) < 0)
    {
        exception("move to string failed", 3);
    }
}
//prints a color
char colorToString(Color const color)
{
    switch(color)
    {
        case white: return 'w';
        case black: return 'b';
        default:
            exception("color to string had an invalid input",4);
            return 'e';
    }
}
//turns a piece into a character
char pieceToString(BoardElements const piece)
{
    switch(piece)
    {
        case wking: return 'K';
        case wqueen: return 'Q';
        case wpawn: return 'P';
        case wrook: return 'R';
        case wknight: return 'N';
        case wbishop: return 'B';
        case bking: return 'k';
        case bqueen: return 'q';
        case bpawn: return 'p';
        case brook: return 'r';
        case bknight: return 'n';
        case bbishop: return 'b';
        case empty: return '.';
        default : exception("pieceToString got an invalid input, boardstate is corrupt", 2);
                  return 'e';
     }
 }
//returns the piececolor of the piece
//or errors out, on error could return black (shouldn't)
Color getColor(BoardElements const piece)
{
    switch(piece)
    {
        case wking: case wqueen: case wpawn:
        case wrook: case wknight: case wbishop:
                return white;

        case bking: case bqueen: case bpawn: 
        case brook: case bknight: case bbishop: 
                return black;

        default : exception("can't retrieve color",1);
                  //to appease the possibility of
                  //this function not returning anything
                  return black;
    }
}
//populates the list with all the pieces on the current board
PieceList getPiecesByColor(Color const color)
{
    //initialize a new movelist all to 0
    //the nested braces are neccessary to make a default element to populate
    //the structs with (which is odd, but whatever)
    PieceList retval = {.count = 0, .pieces={ {.pos={.xval=0,.yval=0},.piece=empty} }};

    int c, r;
    //iterate over the rows and columns 
    for(c=0; c<COLS;++c)
    {
        for(r=0;r<ROWS; ++r)
        {

           //match the color with the respecitve pieces, add them as appropriate
           if(color == white)
           {
               switch(board[RowCol(c,r)]) 
               {
                    case wking: case wqueen: case wpawn:
                    case wrook: case wknight: case wbishop:
                                  retval.pieces[retval.count].pos.xval = c;
                                  retval.pieces[retval.count].pos.yval = r;
                                  retval.pieces[retval.count].piece = board[RowCol(c,r)];
                                  ++retval.count;
                                  break;
                    default : break;
                }
            }
            else
            {
               switch(board[RowCol(c,r)]) 
               {
                    case bking: case bqueen: case bpawn:
                    case brook: case bknight: case bbishop: 
                                  retval.pieces[retval.count].pos.xval = c;
                                  retval.pieces[retval.count].pos.yval = r;
                                  retval.pieces[retval.count].piece = board[RowCol(c,r)];
                                  ++retval.count;
                                  break;
                    default : break;
               }
           }
        }
    }
    return retval;
}

//reverses the color
Color notColor(Color const col)
{
    switch(col)
    {
        case white: return black;
        case black: return white;
        default:
            exception("notcolor sent invalid value", 3);
            return white;
    }
}
//prints the board to a string
//the input should be an array of length 37
void boardToString(char * const restrict retval)
{
    int c, r;
    int index=0;
    //iterate over the rows and columns 
    for(r=0; r<ROWS;++r,++index)
    {
        for(c=0;c<COLS; ++c)
        {
            retval[RowCol(c,r)+index]= pieceToString(board[RowCol(c,r)]);
        }
        retval[RowCol(c,r)+index]='\n';
    }
    retval[37] = 0x0;
}
//determines if the game has ended
bool isterminal()
{
    return (currentstate.playing && (currentstate.movecount < GAMELENGTH));
}
//resets the board to an initial state
void reset()
{
    board[0] = wking;
    board[1] = wqueen;
    board[2] = wbishop;
    board[3] = wknight;
    board[4] = wrook;
    board[5] = wpawn;
    board[6] = wpawn;
    board[7] = wpawn;
    board[8] = wpawn;
    board[9] = wpawn;
    board[10] = empty;
    board[11] = empty;
    board[12] = empty;
    board[13] = empty;
    board[14] = empty;
    board[15] = empty;
    board[16] = empty;
    board[17] = empty;
    board[18] = empty;
    board[19] = empty;
    board[20] = bpawn;
    board[21] = bpawn;
    board[22] = bpawn;
    board[23] = bpawn;
    board[24] = bpawn;
    board[25] = bking;
    board[26] = bqueen;
    board[27] = bbishop;
    board[28] = bknight;
    board[29] = brook;

    currentstate.turn=white;
    currentstate.playing=true;
    currentstate.availablemoves=NULL;
    currentstate.movecount=1;
    currentstate.whitepieces=getPiecesByColor(white);
    currentstate.blackpieces=getPiecesByColor(black);
    currentstate.zobristhash=0;
    currentstate.incrementor=(Score){.whitescore=0,.blackscore=0,.advancementscore=0,.whitepawnscore=0,.blackpawnscore=0};
    currentstate.value=0;
}
