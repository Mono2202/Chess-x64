using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bishop : Piece
{
    // Constants:
    private static int[] BISHOP_MOVES = { 1, -1 };

    // Methods:
    public Bishop(char type, Position position)
    {
        this.type = type;
        this.position = position;
        this.isWhite = Char.IsUpper(type);
    }

    public override Dictionary<string, Position> GetLegalMoves(Piece[,] board, string currentMove)
    {
        // Inits:
        Dictionary<string, Position> moves = new Dictionary<string, Position>();

        // Condition: bishop move
        for (int i = 0; i < BISHOP_MOVES.Length; i++)
        {
            for (int j = 0; j < BISHOP_MOVES.Length; j++)
            {
                for (int k = 1; k < Board.BOARD_SIZE; k++)
                {
                    // Condition: bishop movement is blocked
                    if (Position.IsOutOfBounds(position.row + (k * BISHOP_MOVES[i]), position.col + (k * BISHOP_MOVES[j])) ||
                        board[position.row + (k * BISHOP_MOVES[i]), position.col + (k * BISHOP_MOVES[j])] != null)
                    {
                        // Condition: enemy piece
                        if (!Position.IsOutOfBounds(position.row + (k * BISHOP_MOVES[i]), position.col + (k * BISHOP_MOVES[j])) &&
                            board[position.row + (k * BISHOP_MOVES[i]), position.col + (k * BISHOP_MOVES[j])] != null &&
                            board[position.row + (k * BISHOP_MOVES[i]), position.col + (k * BISHOP_MOVES[j])].isWhite != isWhite)
                        {
                            moves.Add(Position.MoveToNotation(type, position, new Position(position.row + (k * BISHOP_MOVES[i]), position.col + (k * BISHOP_MOVES[j])), true, board),
                                new Position(position.row + (k * BISHOP_MOVES[i]), position.col + (k * BISHOP_MOVES[j])));
                        }
                        break;
                    }

                    else
                    {
                        moves.Add(Position.MoveToNotation(type, position, new Position(position.row + (k * BISHOP_MOVES[i]), position.col + (k * BISHOP_MOVES[j])), false, board),
                                new Position(position.row + (k * BISHOP_MOVES[i]), position.col + (k * BISHOP_MOVES[j])));
                    }
                }
            }
        }
        
        return moves;
    }
}
