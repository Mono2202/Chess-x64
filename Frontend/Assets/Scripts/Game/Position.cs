using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Position
{
    // Fields:
    public int row;
    public int col;

    // Methods:
    public Position(int row, int col)
    {
        this.row = row;
        this.col = col;
    }

    /*
     * Checks if a board position is out of bounds
     * Input : position - the board position
     * Output: true     - the position is out of bounds
     *         false    - otherwise
     */
    public static bool IsOutOfBounds(int row, int col)
    {
        return row < 0 || row >= Board.BOARD_SIZE || 
            col < 0 || col >= Board.BOARD_SIZE;
    }

    /*
     * Converts a move to a notation
     * Input : type  - the piece's type
     *         src   - the source position
     *         dest  - the dest position
     *         taken - a piece was taken
     *         board - the chess board
     *         
     * Output: true  - the position is out of bounds
     *         false - otherwise
     */
    public static string MoveToNotation(char type, Position src, Position dst, bool taken, Piece[,] board)
    {
        return Char.ToUpper(type) + ColToLetter(src.col) + RowToNumber(src.row) + /*(taken ? "x" : "") +*/
            ColToLetter(dst.col) + RowToNumber(dst.row); //+ AddSpecialNotation(); TODO
    }

    /*public static string AddSpecialNotation(Position[,] board)
    {
        if (IsMate(board))
            return "#";

        if (IsCheck(board))
            return "+";

        return "";
    }*/

    private static string ColToLetter(int col)
    {
        return ((char)(col + 'a')).ToString();
    }
    private static string RowToNumber(int row)
    {
        return ((char)(row + '1')).ToString();
    }
}
