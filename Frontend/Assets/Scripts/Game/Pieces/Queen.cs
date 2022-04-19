using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Queen : Piece
{
    // Methods:
    public Queen(char type, Position position)
    {
        this.type = type;
        this.position = position;
        this.isWhite = Char.IsUpper(type);
    }

    public override Dictionary<string, Position> GetLegalMoves(Piece[,] board, string currentMove)
    {
        // Inits:
        Dictionary<string, Position> moves = new Dictionary<string, Position>();
        Dictionary<string, Position> bishopMoves = new Dictionary<string, Position>();
        Dictionary<string, Position> rookMoves = new Dictionary<string, Position>();
        Bishop bishop = new Bishop(type, position);
        Rook rook = new Rook(type, position);

        // Getting the pieces moves:
        bishopMoves = bishop.GetLegalMoves(board, currentMove);
        rookMoves = rook.GetLegalMoves(board, currentMove);

        // Adding the moves to the queen moves:
        foreach (var move in bishopMoves)
            moves.Add(move.Key, move.Value);
        foreach (var move in rookMoves)
            moves.Add(move.Key, move.Value);

        return moves;
    }
}
