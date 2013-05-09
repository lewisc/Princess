#include "globals.h"
#include "elements.h"
#include "movecalculation.h"
#include "makemoves.h"
#include "typedinput.h"
#include "movehelpers.h"
#include "stdio.h"
#include "stdlib.h"
#include "time.h"

int main()
{
    //char movetest[15];
    //has to be initialized
    Move moveread;
    char moveout[20];
//    char boardstatestring[45];
    UndoState undo;
    MoveList moves = {.count=0,.moves={{.from={.xval=0,.yval=0},.to={.xval=0,.yval=0}}}};
    srand(time(NULL));
    int i;

    reset();
    for(i = 0; i<10000;i++)
    {
       reset(); 
       printf("=Game %d\n", i);

        while(!isterminal())
        {
            if(currentstate.turn == white)
            {
                calculateMoves(&currentstate.whitepieces,&moves);
            }
            else if(currentstate.turn == black)
            {
                calculateMoves(&currentstate.blackpieces,&moves);
            }
            else
            {
                exception("invalid color",2);
            }

            currentstate.availablemoves = &moves;
            moveread = currentstate.availablemoves->moves[rand()%currentstate.availablemoves->count];
            
//            printf("board is:\n");
//            printf("boardstate:\n%s\n", (boardToString(boardstatestring),boardstatestring));
            doupdate(moveread,&undo);
            printf("%s\n", (moveToString(moveout,moveread),moveout));
            moves.count = 0;
        }
    }
    
    return 0;
}
