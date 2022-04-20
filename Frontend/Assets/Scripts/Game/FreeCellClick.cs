using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FreeCellClick : MonoBehaviour
{
    // Fields:
    public int row;
    public int col;
    public BoardEditor boardScript;

    public void CellClick()
    {
        // Initializing the board colors:
        for (int i = 0; i < Board.BOARD_SIZE; i++)
        {
            for (int j = 0; j < Board.BOARD_SIZE; j++)
            {
                boardScript.guiBoardArr[i, j].GetComponent<Image>().color = (((i + j) % 2 == 0) /*== Data.instance.isWhite*/) ? new Color32(255, 255, 255, 255) : new Color32(65, 65, 65, 255);
            }
        }

        // Choosing a piece:
        if (boardScript.boardArr[row, col] != null &&
            boardScript.selectedPiece == null)
        {
            boardScript.selectedPiece = new Position(row, col);
            boardScript.guiBoardArr[row, col].GetComponent<Image>().color = Color.red;
        }
        
        // Setting the destination:
        else if (boardScript.selectedPiece != null)
        {
            boardScript.selectedDest = new Position(row, col);
        }
    }
}
