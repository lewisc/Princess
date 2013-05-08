#include "regex.h"
#include "globals.h"
#include "stdbool.h"
#include "typedinput.h"
#include "stdlib.h"
#include "stdio.h"

//the regular expression and a boolean declaring whether
//it exists yet or not
static regex_t move;
static bool enabled = false;
//this cleans up the regular expression
void cleanup()
{
    if(enabled == true)
    {
        regfree(&move);
        enabled = false;
    }
}
static int stringToInt(char const c)
{
    switch(c)
    {
        case 'a': return 0;
        case 'b': return 1;
        case 'c': return 2;
        case 'd': return 3;
        case 'e': return 4;

        case '1': return 0;
        case '2': return 1;
        case '3': return 2;
        case '4': return 3;
        case '5': return 4;
        case '6': return 5;
        default :
            exception("invalid input character",2);
            return -1;
    }
}

//parse a string as a move and if it parses return true, otherwise return false
//populate the input value retval with the contents of the move
bool readinput(char const * const restrict movestring, Move * const restrict retval)
{
    if(enabled == false)
    {
        //extended posix regular expressions
        //so we don't have to escape escaped escape characters
        int compiled = regcomp(&move, "!\\s?([a-e])([1-6])-([a-e])([1-6])",REG_EXTENDED);
        if(compiled)
        {
            exception("failed to compile move regex", 1);        
        }
        else
        {
            atexit(&cleanup);
            enabled = true;
        }
   }
   regmatch_t matches[5];
   int exec = regexec(&move,movestring,5,matches,0); 
   if(exec)
   {
        //failed to match or other error
        return false;
   }
   else
   {
      
       retval->from.xval = stringToInt(movestring[matches[1].rm_so]);
       retval->from.yval = stringToInt(movestring[matches[2].rm_so]);
       retval->to.xval = stringToInt(movestring[matches[3].rm_so]);
       retval->to.yval = stringToInt(movestring[matches[4].rm_so]);
       return true;
   }
}
