using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public static void getStartingBoard(ref Game game)
    {
        game.top = 6172839697753047040;
        game.bottom = 11163050;
    }

    public static char getPiece(int i, Game game) 
    {
        ulong m = toBin(i); //get binary representation of position
        if ((game.top & m) != 0) //check if its top
            return 't';
        if ((game.bottom & m) != 0) //check if its bottom
            return 'b';
        return 'e'; //none so return empty
    }

    public static void setPiece(int i, char c, ref Game game) 
    {
        ulong m = toBin(i); //get binary representation of position
        if (c == 't')
            game.top |= m; //set the bit at the location to 1
        if (c == 'b')
            game.bottom |= m;
        if (c == 'e') { 
            game.bottom &= (~m); //remove bit from both sides
            game.top &= (~m);
        }
    }

    public static void updateLegalMoves(ref Game game)
    {
        ulong[] moves = new ulong[16]; //create empty legal moves array

        ulong empty = ~(game.bottom | game.top); //all empty places

        ulong bkings = game.kings & game.bottom; //bottom kings
        ulong tkings = game.kings & game.top; //top kings

        if (game.turn % 2 == 0) //bottom turn so only get his legal moves
        {
            moves[0] = (game.bottom & 0x3F3F3F3F3F3F3F3F) & (game.top >> 9) & (empty >> 18);
            moves[1] = (game.bottom & 0xFCFCFCFCFCFCFCFC) & (game.top >> 7) & (empty >> 14);
            moves[2] = (bkings & 0xFCFCFCFCFCFCFCFC) & (game.bottom << 9) & (empty << 18);
            moves[3] = (bkings & 0x3F3F3F3F3F3F3F3F) & (game.bottom << 7) & (empty << 14);

            if (!game.mustEat || Mathf.Max(moves[0], moves[1], moves[2], moves[3]) == 0)
            {
                moves[8] = ((game.bottom & 0x7F7F7F7F7F7F7F7F)) & (empty >> 9);
                moves[9] = ((game.bottom & 0xFEFEFEFEFEFEFEFE)) & (empty >> 7);
                moves[10] = ((bkings & 0xFEFEFEFEFEFEFEFE)) & (empty << 9);
                moves[11] = ((bkings & 0x7F7F7F7F7F7F7F7F)) & (empty << 7);
            }
        }
        
        else{
            moves[4] = (game.top & 0xFCFCFCFCFCFCFCFC) & (game.bottom << 9) & (empty << 18);
            moves[5] = (game.top & 0x3F3F3F3F3F3F3F3F) & (game.bottom << 7) & (empty << 14);
            moves[6] = (tkings & 0x3F3F3F3F3F3F3F3F) & (game.top >> 9) & (empty >> 18);
            moves[7] = (tkings & 0xFCFCFCFCFCFCFCFC) & (game.top >> 7) & (empty >> 14);

            if (!game.mustEat || Mathf.Max(moves[4], moves[5], moves[6], moves[7]) == 0)
            {
                moves[12] = ((game.top & 0xFEFEFEFEFEFEFEFE)) & (empty << 9);
                moves[13] = ((game.top & 0x7F7F7F7F7F7F7F7F)) & (empty << 7);
                moves[14] = ((tkings & 0x7F7F7F7F7F7F7F7F)) & (empty >> 9);
                moves[15] = ((tkings & 0xFEFEFEFEFEFEFEFE)) & (empty >> 7);
            }
        }
        game.legalMoves = moves;
    }

    public static bool doubleCapture(int pos,  ref Game game) //update legal moves for double capture and return true if possible
    {

        ulong[] moves = new ulong[16];

        ulong empty = ~(game.bottom | game.top);
        ulong capture = toBin(pos);
        ulong king = game.kings & capture;

        moves[0] = (game.bottom & capture & 0x3F3F3F3F3F3F3F3F) & (game.top >> 9) & (empty >> 18);
        moves[1] = (game.bottom & capture & 0xFCFCFCFCFCFCFCFC) & (game.top >> 7) & (empty >> 14);

        moves[4] = (game.top & capture & 0xFCFCFCFCFCFCFCFC) & (game.bottom << 9) & (empty << 18);
        moves[5] = (game.top & capture & 0x3F3F3F3F3F3F3F3F) & (game.bottom << 7) & (empty << 14);

        if (king != 0)
        {
            moves[2] = (game.bottom & capture & 0xFCFCFCFCFCFCFCFC) & (game.bottom << 9) & (empty << 18);
            moves[3] = (game.bottom & capture & 0x3F3F3F3F3F3F3F3F) & (game.bottom << 7) & (empty << 14);

            moves[6] = (game.top & capture & 0x3F3F3F3F3F3F3F3F) & (game.top >> 9) & (empty >> 18);
            moves[7] = (game.top & capture & 0xFCFCFCFCFCFCFCFC) & (game.top >> 7) & (empty >> 14);
        }

        if (Mathf.Max(moves[0], moves[1], moves[2], moves[3], moves[4], moves[5], moves[6], moves[7]) != 0)
        {
            for (int i = 8; i < 16; i++)
                moves[i] = 0;
            game.legalMoves = moves;
            return true;
        }
        return false;


    }


    public static void makeMove(int i, int j, ref Game game) //make a move (checks if its legal)
    {
        moveChecker(ref game, i, j, 0, 18, 0);
        moveChecker(ref game, i, j, 1, 14, 0);
        moveChecker(ref game, i, j, 2, -18, 0);
        moveChecker(ref game, i, j, 3, -14, 0);

        moveChecker(ref game, i, j, 4, -18, 1);
        moveChecker(ref game, i, j, 5, -14, 1);
        moveChecker(ref game, i, j, 6, 18, 1);
        moveChecker(ref game, i, j, 7, 14, 1);

        moveChecker(ref game, i, j, 8, 9, 0);
        moveChecker(ref game, i, j, 9, 7, 0);
        moveChecker(ref game, i, j, 10, -9, 0);
        moveChecker(ref game, i, j, 11, -7, 0);

        moveChecker(ref game, i, j, 12, -9, 1);
        moveChecker(ref game, i, j, 13, -7, 1);
        moveChecker(ref game, i, j, 14, 9, 1);
        moveChecker(ref game, i, j, 15, 7, 1);

    }

    private static void moveChecker(ref Game game, int i, int j, int loc, int offset, int turn) 
    {
        if (isMovesArrayIndex(i, loc, game) && i + offset == j && game.turn % 2 == turn) //move is legal
            movePiece(i, j, offset >= 14 || offset <= -14, ref game);
    }

    public static bool isMovesArrayIndex(int i, int j, Game game) //check if piece has a legal move
    {
        ulong loc = toBin(i);
        return (game.legalMoves[j] & loc) != 0;
    }

    public static void movePiece(int i, int j, bool eat, ref Game game)
    {
        setPiece(j, getPiece(i, game), ref game); //move piece
        setPiece(i, 'e', ref game); //remove old piece
        
        if (eat)
        {
            setPiece((j - i) / 2 + i, 'e', ref game); //remove eaten piece
            if (doubleCapture(j, ref game)) //check double capture
                return; //return since we dont want to update the turn (give another turn to eat, and dont update legal moves)
         
        }
        game.turn += 1;
        updateLegalMoves(ref game);
    }

    public static bool isEmpty(int i, Game game) //check if place is empty
    {
        return getPiece(i, game) == 'e';
    }

    public static ulong toBin(int i) //convert num to binary board representation 
    {
        ulong m = 1;
        m <<= i;
        return m;
    }
}
