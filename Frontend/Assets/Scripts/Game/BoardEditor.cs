using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardEditor : MonoBehaviour
{
    // Fields:
    public GameObject cell;
    public Transform canvas;
    public List<Sprite> chessSprites = new List<Sprite>();
    [HideInInspector] public GameObject[,] guiBoardArr = new GameObject[BOARD_SIZE, BOARD_SIZE];
    [HideInInspector] public Piece[,] boardArr = new Piece[BOARD_SIZE, BOARD_SIZE];
    [HideInInspector] public Position selectedPiece;
    [HideInInspector] public Position selectedDest;

    // Constants:
    public const int BOARD_SIZE = 8;
    public static char[,] INITIAL_BOARD =
    {
        { 'r', 'n', 'b', 'q', 'k', 'b', 'n', 'r' },
        { 'p', 'p', 'p', 'p', 'p', 'p', 'p', 'p' },
        { '#', '#', '#', '#', '#', '#', '#', '#' },
        { '#', '#', '#', '#', '#', '#', '#', '#' },
        { '#', '#', '#', '#', '#', '#', '#', '#' },
        { '#', '#', '#', '#', '#', '#', '#', '#' },
        { 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P' },
        { 'R', 'N', 'B', 'Q', 'K', 'B', 'N', 'R' }
    };

    public const int W_PAWN_SPRITE_INDEX = 0;
    public const int W_KNIGHT_SPRITE_INDEX = 1;
    public const int W_BISHOP_SPRITE_INDEX = 2;
    public const int W_ROOK_SPRITE_INDEX = 3;
    public const int W_QUEEN_SPRITE_INDEX = 4;
    public const int W_KING_SPRITE_INDEX = 5;
    public const int B_PAWN_SPRITE_INDEX = 6;
    public const int B_KNIGHT_SPRITE_INDEX = 7;
    public const int B_BISHOP_SPRITE_INDEX = 8;
    public const int B_ROOK_SPRITE_INDEX = 9;
    public const int B_QUEEN_SPRITE_INDEX = 10;
    public const int B_KING_SPRITE_INDEX = 11;

    public static Dictionary<char, int> charToSpriteIndex = new Dictionary<char, int>
    {
        { 'P', W_PAWN_SPRITE_INDEX },
        { 'N', W_KNIGHT_SPRITE_INDEX },
        { 'B', W_BISHOP_SPRITE_INDEX },
        { 'R', W_ROOK_SPRITE_INDEX },
        { 'Q', W_QUEEN_SPRITE_INDEX },
        { 'K', W_KING_SPRITE_INDEX },
        { 'p', B_PAWN_SPRITE_INDEX },
        { 'n', B_KNIGHT_SPRITE_INDEX },
        { 'b', B_BISHOP_SPRITE_INDEX },
        { 'r', B_ROOK_SPRITE_INDEX },
        { 'q', B_QUEEN_SPRITE_INDEX },
        { 'k', B_KING_SPRITE_INDEX }
    };

    // Start is called before the first frame update
    void Start()
    {
        // Creating the chess board:
        for (int i = 0; i < BOARD_SIZE; i++)
        {
            for (int j = 0; j < BOARD_SIZE; j++)
            {
                // Adding the chess piece object to the array:
                boardArr[i, j] = CreatePiece(INITIAL_BOARD[i, j], new Position(i, j));

                // Adding the cell to the GUI board array:
                guiBoardArr[i, j] = CreateCell(i, j, INITIAL_BOARD[i, j]);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        // Condition: move was played
        if (selectedPiece != null && selectedDest != null)
        {
            // Updating the board:
            UpdateBoard(selectedPiece, selectedDest);

            // Resetting the properties:
            selectedPiece = null;
            selectedDest = null;
        }
    }

    private Piece CreatePiece(char type, Position position)
    {
        switch (Char.ToUpper(type))
        {
            case 'P': return new Pawn(type, position);
            case 'N': return new Knight(type, position);
            case 'B': return new Bishop(type, position);
            case 'R': return new Rook(type, position);
            case 'Q': return new Queen(type, position);
            case 'K': return new King(type, position);
            default: return null;
        }
    }

    private GameObject CreateCell(int i, int j, char type)
    {
        // Creating the cell game object:
        GameObject currentCell = Instantiate(cell);

        // Changing the cell's color: TODO
        //currentCell.GetComponent<Image>().color = (((i + j) % 2 == 0) /*== Data.instance.isWhite*/) ? new Color32(65, 65, 65, 255) : new Color32(8, 171, 0, 255);
        currentCell.GetComponent<Image>().color = (((i + j) % 2 == 0) /*== Data.instance.isWhite*/) ? new Color32(255, 255, 255, 255) : new Color32(65, 65, 65, 255);

        // Changing the cell's parent:
        currentCell.transform.SetParent(canvas, false);

        // Positioning the cell:
        currentCell.transform.localPosition = new Vector3(-350 + j * 100, 350 - i * 100, 0);

        // Creating the chess sprite game object:
        GameObject chessSprite = new GameObject();
        chessSprite.AddComponent<Image>();

        // Condition: piece in current cell
        if (type != '#')
        {
            // Assigning the sprite to the game object:
            chessSprite.GetComponent<Image>().sprite = chessSprites[charToSpriteIndex[type]];
        }

        else
        {
            // Hiding the empty sprite:
            chessSprite.GetComponent<Image>().enabled = false;
        }

        // Assigning the sprite to the cell:
        chessSprite.transform.SetParent(currentCell.transform, false);

        // Adding option to click on cell:
        currentCell.GetComponent<FreeCellClick>().row = i;
        currentCell.GetComponent<FreeCellClick>().col = j;
        currentCell.GetComponent<FreeCellClick>().boardScript = this;

        return currentCell;
    }

    private void UpdateBoard(Position src, Position dst)
    {
        // Changing the GUI:
        Destroy(guiBoardArr[dst.row, dst.col]);
        guiBoardArr[dst.row, dst.col] = CreateCell(dst.row, dst.col,
            boardArr[src.row, src.col].type);
        Destroy(guiBoardArr[src.row, src.col]);
        guiBoardArr[src.row, src.col] = CreateCell(src.row, src.col, '#');

        // Changing the board array:
        boardArr[dst.row, dst.col] = boardArr[src.row, src.col];
        boardArr[dst.row, dst.col].position = new Position(dst.row, dst.col);
        boardArr[src.row, src.col] = null;
    }

    public void ReturnToMenu()
    {
        // Switching to the menu scene:
        this.GetComponent<SwitchScene>().SwitchSceneByIndex(Data.MENU_SCENE_COUNT);
    }
}