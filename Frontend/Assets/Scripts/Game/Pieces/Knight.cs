using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Knight : Piece
{
    // Constants:
    private static int[] KNIGHT_MOVES = { 1, -1, 2, -2 };

    // Methods:
    public Knight(char type, Position position)
    {
        this.type = type;
        this.position = position;
        this.isWhite = Char.IsUpper(type);
    }

    public override Dictionary<string, Position> GetLegalMoves(Piece[,] board, string currentMove)
    {
        // Inits:
        Dictionary<string, Position> moves = new Dictionary<string, Position>();

        // Condition: knight move
        for (int i = 0; i < KNIGHT_MOVES.Length; i++)
        {
            for (int j = 0; j < KNIGHT_MOVES.Length; j++)
            {
                if (Math.Abs(KNIGHT_MOVES[i]) != Math.Abs(KNIGHT_MOVES[j]) &&
                    !Position.IsOutOfBounds(position.row + KNIGHT_MOVES[i], position.col + KNIGHT_MOVES[j]))
                {
                    // Condition: can take piece
                    if (board[position.row + KNIGHT_MOVES[i], position.col + KNIGHT_MOVES[j]] != null &&
                        board[position.row + KNIGHT_MOVES[i], position.col + KNIGHT_MOVES[j]].isWhite != isWhite)
                    {
                        moves.Add(Position.MoveToNotation(type, position, new Position(position.row + KNIGHT_MOVES[i], position.col + KNIGHT_MOVES[j]), true, board),
                            new Position(position.row + KNIGHT_MOVES[i], position.col + KNIGHT_MOVES[j]));
                    }
                    
                    // Condition: can move to empty square
                    else if (board[position.row + KNIGHT_MOVES[i], position.col + KNIGHT_MOVES[j]] == null)
                    {
                        Debug.Log(KNIGHT_MOVES[i] + " " + KNIGHT_MOVES[j]);
                        moves.Add(Position.MoveToNotation(type, position, new Position(position.row + KNIGHT_MOVES[i], position.col + KNIGHT_MOVES[j]), false, board),
                            new Position(position.row + KNIGHT_MOVES[i], position.col + KNIGHT_MOVES[j]));
                    }
                }
            }
        }

        return moves;
    }
}
