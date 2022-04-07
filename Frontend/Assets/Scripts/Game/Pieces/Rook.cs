using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rook : Piece
{
    // Constants:
    private static int[] ROOK_MOVES = { 1, -1 };

    // Methods:
    public Rook(char type, Position position)
    {
        this.type = type;
        this.position = position;
        this.isWhite = Char.IsUpper(type);
    }

    public override Dictionary<string, Position> GetLegalMoves(Piece[,] board, string currentMove)
    {
        // Inits:
        Dictionary<string, Position> moves = new Dictionary<string, Position>();

        // Condition: rook move vertically
        for (int i = 0; i < ROOK_MOVES.Length; i++)
        {
            for (int j = 1; j < Board.BOARD_SIZE; j++)
            {
                // Condition: rook movement is blocked
                if (Position.IsOutOfBounds(position.row + (j * ROOK_MOVES[i]), position.col) ||
                    board[position.row + (j * ROOK_MOVES[i]), position.col] != null)
                {
                    // Condition: enemy piece
                    if (!Position.IsOutOfBounds(position.row + (j * ROOK_MOVES[i]), position.col) &&
                        board[position.row + (j * ROOK_MOVES[i]), position.col] != null &&
                        board[position.row + (j * ROOK_MOVES[i]), position.col].isWhite != isWhite)
                    {
                        moves.Add(Position.MoveToNotation(type, position, new Position(position.row + (j * ROOK_MOVES[i]), position.col), true, board),
                            new Position(position.row + (j * ROOK_MOVES[i]), position.col));
                    }
                    break;
                }

                else
                {
                    moves.Add(Position.MoveToNotation(type, position, new Position(position.row + (j * ROOK_MOVES[i]), position.col), false, board),
                            new Position(position.row + (j * ROOK_MOVES[i]), position.col));
                }
            }
        }

        // Condition: rook move horizontally
        for (int i = 0; i < ROOK_MOVES.Length; i++)
        {
            for (int j = 1; j < Board.BOARD_SIZE; j++)
            {
                // Condition: rook movement is blocked
                if (Position.IsOutOfBounds(position.row, position.col + (j * ROOK_MOVES[i])) ||
                    board[position.row, position.col + (j * ROOK_MOVES[i])] != null)
                {
                    // Condition: enemy piece
                    if (!Position.IsOutOfBounds(position.row, position.col + (j * ROOK_MOVES[i])) &&
                        board[position.row, position.col + (j * ROOK_MOVES[i])] != null &&
                        board[position.row, position.col + (j * ROOK_MOVES[i])].isWhite != isWhite)
                    {
                        moves.Add(Position.MoveToNotation(type, position, new Position(position.row, position.col + (j * ROOK_MOVES[i])), true, board),
                            new Position(position.row, position.col + (j * ROOK_MOVES[i])));
                    }
                    break;
                }

                else
                {
                    moves.Add(Position.MoveToNotation(type, position, new Position(position.row, position.col + (j * ROOK_MOVES[i])), false, board),
                            new Position(position.row, position.col + (j * ROOK_MOVES[i])));
                }
            }
        }

        return moves;
    }
}
