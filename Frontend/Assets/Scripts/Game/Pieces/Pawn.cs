using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pawn : Piece
{
    // Fields:
    private int moveDirection;
    private int startingRow;
    private int enPassantRow;

    // Constants:
    private const int PAWN_MOVE = 1;
    private const int STARTING_W_PAWN_ROW = 6;
    private const int STARTING_B_PAWN_ROW = 1;
    private const int EN_PASSANT_W_PAWN_ROW = 3;
    private const int EN_PASSANT_B_PAWN_ROW = 4;

    // Methods:
    public Pawn(char type, Position position)
    {
        this.type = type;
        this.position = position;
        this.isWhite = Char.IsUpper(type);
        this.moveDirection = isWhite ? -PAWN_MOVE : PAWN_MOVE;
        this.startingRow = isWhite ? STARTING_W_PAWN_ROW : STARTING_B_PAWN_ROW;
        this.enPassantRow = isWhite ? EN_PASSANT_W_PAWN_ROW : EN_PASSANT_B_PAWN_ROW;
    }

    public override Dictionary<string, Position> GetLegalMoves(Piece[,] board, string currentMove)
    {
        // Inits:
        Dictionary<string, Position> moves = new Dictionary<string, Position>();

        // Condition: first pawn move
        if (position.row == startingRow &&
            board[position.row + moveDirection, position.col] == null &&
            board[position.row + 2 * moveDirection, position.col] == null)
        {
            moves.Add(Position.MoveToNotation(type, position, new Position(position.row + 2 * moveDirection, position.col), false, board),
                new Position(position.row + 2 * moveDirection, position.col));
        }

        // Condition: one square move
        if (!Position.IsOutOfBounds(position.row + moveDirection, position.col) &&
            board[position.row + moveDirection, position.col] == null)
        {
            moves.Add(Position.MoveToNotation(type, position, new Position(position.row + moveDirection, position.col), false, board), 
                new Position(position.row + moveDirection, position.col));
        }

        // Condition: can take piece from right
        if (!Position.IsOutOfBounds(position.row + moveDirection, position.col + moveDirection) &&
            board[position.row + moveDirection, position.col + moveDirection] != null &&
            board[position.row + moveDirection, position.col + moveDirection].isWhite != isWhite)
        {
            moves.Add(Position.MoveToNotation(type, position, new Position(position.row + moveDirection, position.col + moveDirection), true, board),
                new Position(position.row + moveDirection, position.col + moveDirection));
        }

        // Condition: can take piece from left
        if (!Position.IsOutOfBounds(position.row + moveDirection, position.col - moveDirection) &&
            board[position.row + moveDirection, position.col - moveDirection] != null &&
            board[position.row + moveDirection, position.col - moveDirection].isWhite != isWhite)
        {
            moves.Add(Position.MoveToNotation(type, position, new Position(position.row + moveDirection, position.col - moveDirection), true, board),
                new Position(position.row + moveDirection, position.col - moveDirection));
        }
        Debug.Log(currentMove);
        // Condition: can en passant
        if (position.row == enPassantRow &&
            Math.Abs(currentMove[2] - currentMove[4]) == 2 &&
            currentMove[0] == 'P') // TODO: CHANGE TO FUNCTION THAT PARSES currentMove to Position
        {
            moves.Add(Position.MoveToNotation(type, position, new Position(position.row + moveDirection, currentMove[1] - 'a'/*TODO*/), true, board),
                new Position(position.row + moveDirection, currentMove[1] - 'a'/*TODO*/));
        }

        return moves;
    }
}
