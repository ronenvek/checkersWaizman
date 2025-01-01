public class Game
{
    //board is stored in 3 unsigned longs, each bit represents a square, i.e. the first bit is bottom left, and the last bit is top right
    //if the kings overlap with one of the 
    public ulong top;
    public ulong bottom;
    public ulong kings;

    public int turn = 0;
    public bool mustEat = true;

    public bool gameOver = false;
    public bool winner;

    public ulong[] legalMoves; 
    //each location is for a different legal moves, if a bit is set to 1 then the piece there can do the move specified below
    //i can make it only be of size 8, might do that later on

    // 0 -> bottom eat 18 up
    // 1 -> bottom eat 14 up
    // 2 -> bottom eat 18 down kings
    // 3 -> bottom eat 14 down kings

    // 4 -> top eat 18 down
    // 5 -> top eat 14 down
    // 6 -> top eat 18 up kings
    // 7 -> top eat 14 up kings


    // 8 -> bottom move 9 up
    // 9 -> bottom move 7 up
    // 10 -> bottom move 9 down kings
    // 11 -> bottom move 7 down kings

    // 12 -> top move 9 down
    // 13 -> top move 7 down
    // 14 -> top move 9 up kings
    // 15 -> top move 7 up kings
}