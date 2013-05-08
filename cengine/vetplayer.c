#include "globals.h"
#include "elements.h"
#include "movecalculation.h"
#include "makemoves.h"
#include "typedinput.h"
#include "movehelpers.h"
#include "stdio.h"

int main(int const argc, char const * const restrict * const restrict argv)
{
    FILE * const restrict inputfile = fopen(argv[1],"r");
    char moveline[30];
    char boardstatestring[40];
//    char movetest[15];
    //has to be initialized
    Move moveread;
    UndoState undo;
    MoveList moves = {.count=0,.moves={{.from={.xval=0,.yval=0},.to={.xval=0,.yval=0}}}};
    //exit if no file passed in
    //
    if(argc != 2)
    {
        exception("need a file input", -1);
    }

    //if we opened the file correctly
    if(inputfile != NULL)
    {
        int current = 0;
        //parse it and apply it while there is a move
        while(fgets(moveline, 29, inputfile) != NULL)
        {
            ++current;
            if(readinput(moveline, &moveread) == true)
            {
                if(currentstate.turn == white)
                {
                    calculateMoves(&currentstate.whitepieces,&moves);
                }
                else if(currentstate.turn == black)
                {
                    calculateMoves(&currentstate.whitepieces,&moves);
                }
                else
                {
                    exception("invalid color",2);
                }
                doupdate(moveread,&undo);
                moves.count = 0;
            }
            else
            {
                if(isterminal())
                {
                    printf("Line: %s was found and was not terminal\n", moveline);
                    printf("boardstate:\n%s\n", (boardToString(boardstatestring),boardstatestring));
                }
                
                    
                reset();
            }
        }
    }

    
    return 0;
}
