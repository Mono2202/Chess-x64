using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class King : Piece
{
    // Constants:
    private static int[] KING_MOVES = { 1, -1, 0 };

    // Methods:
    public King(char type, Position position)
    {
        this.type = type;
        this.position = position;
        this.isWhite = Char.IsUpper(type);
    }

    public override Dictionary<string, Position> GetLegalMoves(Piece[,] board, string currentMove)
    {
        // Inits:
        Dictionary<string, Position> moves = new Dictionary<string, Position>();

        // Condition: king move
        for (int i = 0; i < KING_MOVES.Length; i++)
        {
            for (int j = 0; j < KING_MOVES.Length; j++)
            {
                // Condition: king stuck in place
                if (KING_MOVES[i] == 0 && KING_MOVES[j] == 0)
                    break;

                // Condition: king movement is blocked
                if (Position.IsOutOfBounds(position.row + KING_MOVES[i], position.col + KING_MOVES[j]) ||
                    board[position.row + KING_MOVES[i], position.col + KING_MOVES[j]] != null)
                {
                    // Condition: enemy piece
                    if (!Position.IsOutOfBounds(position.row + KING_MOVES[i], position.col + KING_MOVES[j]) &&
                        board[position.row + KING_MOVES[i], position.col + KING_MOVES[j]] != null &&
                        board[position.row + KING_MOVES[i], position.col + KING_MOVES[j]].isWhite != isWhite)
                    {
                        moves.Add(Position.MoveToNotation(type, position, new Position(position.row + KING_MOVES[i], position.col + KING_MOVES[j]), true, board),
                            new Position(position.row + KING_MOVES[i], position.col + KING_MOVES[j]));
                    }
                    continue;
                }

                else
                {
                    moves.Add(Position.MoveToNotation(type, position, new Position(position.row + KING_MOVES[i], position.col + KING_MOVES[j]), false, board),
                            new Position(position.row + KING_MOVES[i], position.col + KING_MOVES[j]));
                }
            }
        }

        return moves;
    }
}
