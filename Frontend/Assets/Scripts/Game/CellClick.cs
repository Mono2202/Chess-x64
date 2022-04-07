using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CellClick : MonoBehaviour
{
    // Fields:
    public int row;
    public int col;
    public Board boardScript;

    public void ShowLegalMoves()
    {
        // Initializing the board colors:
        for (int i = 0; i < Board.BOARD_SIZE; i++)
        {
            for (int j = 0; j < Board.BOARD_SIZE; j++)
            {
                boardScript.guiBoardArr[i, j].GetComponent<Image>().color = (((i + j) % 2 == 0) /*== Data.instance.isWhite*/) ? new Color32(255, 255, 255, 255) : new Color32(65, 65, 65, 255);
            }
        }

        // Showing legal moves:
        if (boardScript.isCurrentPlayerWhite == boardScript.isPlayerWhite &&
            boardScript.boardArr[row, col] != null &&
            boardScript.boardArr[row, col].isWhite == boardScript.isPlayerWhite)
        {
            Dictionary<string, Position> moves = boardScript.boardArr[row, col].GetLegalMoves(boardScript.boardArr, boardScript.currentMove);
            foreach (KeyValuePair<string, Position> move in moves)
            {
                boardScript.guiBoardArr[move.Value.row, move.Value.col].GetComponent<Image>().color = Color.red;
            }

            boardScript.selectedPiece = new Position(row, col);
        }
        
        // Setting the destination:
        else if (boardScript.selectedPiece != null)
        {
            boardScript.selectedDest = new Position(row, col);
        }
    }
}
