using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Piece
{
    // Fields:
    public char type;
    public Position position;
    public bool isWhite;

    // Methods:
    public abstract Dictionary<string, Position> GetLegalMoves(Piece[,] board, string currentMove);
}
