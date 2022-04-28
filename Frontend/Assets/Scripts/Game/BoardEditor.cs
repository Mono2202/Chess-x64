using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class BoardEditor : MonoBehaviour
{
    // Fields:
    public GameObject cell;
    public Transform canvas;
    public InputField turnInput;
    public Text bestMoveText;
    public InputField addPieceInput;
    public List<Sprite> chessSprites = new List<Sprite>();
    [HideInInspector] public GameObject[,] guiBoardArr = new GameObject[BOARD_SIZE, BOARD_SIZE];
    [HideInInspector] public Piece[,] boardArr = new Piece[BOARD_SIZE, BOARD_SIZE];
    [HideInInspector] public Position selectedPiece;
    [HideInInspector] public Position selectedDest;
    [HideInInspector] public bool turnChanged;
    [HideInInspector] public string currentTurn;
    [HideInInspector] public string currentMove = "";
    [HideInInspector] public string currentNewPiece = "";

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

        // Updating the current player:
        turnInput.text = "w";
        currentTurn = turnInput.text;
    }

    // Update is called once per frame
    void Update()
    {
        // Condition: different turn
        if (currentTurn != turnInput.text)
        {
            turnChanged = true;
        }

        // Setting the current turn:
        currentTurn = turnInput.text;

        // Condition: move was played
        if (selectedPiece != null && selectedDest != null)
        {
            // Updating the board:
            UpdateBoard(selectedPiece, selectedDest);

            // Resetting the properties:
            selectedPiece = null;
            selectedDest = null;

            // Updating the current player:
            turnInput.text = (turnInput.text == "w") ? "b" : "w";
            currentTurn = turnInput.text;

            // Using the engine:
            try
            {
                Thread tEngine = new Thread(new ThreadStart(UseEngine));
                tEngine.Start();
            }

            catch
            {
                return;
            }
        }

        // Condition: a turn has changed
        if (turnChanged)
        {
            try
            {
                Thread tEngine = new Thread(new ThreadStart(UseEngine));
                tEngine.Start();
            }

            catch
            {
                return;
            }
        }

        // Setting the flag:
        turnChanged = false;

        // Setting the best move:
        bestMoveText.text = currentMove;

        // Condition: new piece
        if (addPieceInput.text.Length == 3)
        {
            currentNewPiece = addPieceInput.text;
            addPieceInput.text = "";
            AddPiece();
        }
    }

    private void UseEngine()
    {
        // Inits:
        string boardFEN = "";
        int count = 0;

        // Turning the board to FEN:
        for (int i = 0; i < BOARD_SIZE; i++)
        {
            for (int j = 0; j < BOARD_SIZE; j++)
            {
                // Condition: piece in current square
                if (boardArr[i, j] != null)
                {
                    if (count > 0)
                        boardFEN += Convert.ToString(count) + boardArr[i, j].type;
                    else
                        boardFEN += boardArr[i, j].type;
                    count = 0;
                }

                // Condition: empty square
                else
                {
                    count++;
                }
            }

            // Condition: more empty squares
            if (count > 0)
            {
                boardFEN += Convert.ToString(count);
            }

            // Resetting the counter:
            count = 0;
            boardFEN += "/";
        }

        // Updating the FEN:
        boardFEN = boardFEN.Remove(boardFEN.Length - 1) + " ";
        boardFEN += (turnInput.text == "") ? "w" : turnInput.text; // TODO: CHECK IF VALID

        // Execute stockfish:
        var p = new System.Diagnostics.Process();
        p.StartInfo.FileName = Application.streamingAssetsPath + "/Stockfish.exe";
        p.StartInfo.UseShellExecute = false;
        p.StartInfo.RedirectStandardInput = true;
        p.StartInfo.RedirectStandardOutput = true;
        p.StartInfo.CreateNoWindow = true;
        p.Start();

        // Setting the position:
        string setupString = "position fen " + boardFEN + " KQkq - 0 1";
        p.StandardInput.WriteLine(setupString);

        // Calculating:
        string processString = "go depth 5";
        p.StandardInput.WriteLine(processString);

        // Getting the best move in the current position:
        string bestMove = p.StandardOutput.ReadLine();
        while (!bestMove.Contains("bestmove"))
        {
            bestMove = p.StandardOutput.ReadLine();
        }
        currentMove = bestMove.Substring(9, 4);

        // Closing the process: 
        p.Close();
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
        currentCell.GetComponent<Image>().color = (((i + j) % 2 == 0) /*== Data.instance.isWhite*/) ? Data.instance.whiteSquareColor : Data.instance.blackSquareColor;

        // Changing the cell's parent:
        currentCell.transform.SetParent(canvas, false);

        // Positioning the cell:
        currentCell.transform.localPosition = new Vector3(-350 / 2 + j * 50, 350 / 2 - i * 50, 0);

        // Creating the chess sprite game object:
        GameObject chessSprite = new GameObject();
        chessSprite.AddComponent<Image>();

        // Condition: piece in current cell
        if (type != '#')
        {
            // Assigning the sprite to the game object:
            chessSprite.GetComponent<Image>().sprite = chessSprites[charToSpriteIndex[type]];
            chessSprite.GetComponent<Image>().rectTransform.sizeDelta = new Vector2(45, 45);
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
        this.GetComponent<SwitchScene>().SwitchSceneByIndex(Data.HOME_SCENE_COUNT);
    }

    public void AddPiece()
    {
        try
        {
            // Setting the position
            Position pos = new Position(currentNewPiece[2] - '1', currentNewPiece[1] - 'a');

            // Changing the GUI:
            Destroy(guiBoardArr[pos.row, pos.col]);
            guiBoardArr[pos.row, pos.col] = CreateCell(pos.row, pos.col,
                currentNewPiece[0]);

            // Changing the board array:
            boardArr[pos.row, pos.col] = CreatePiece(currentNewPiece[0], pos);

            // Using the engine:
            try
            {
                Thread tEngine = new Thread(new ThreadStart(UseEngine));
                tEngine.Start();
            }

            catch
            {
                return;
            }
        }

        catch
        {
            return; // TODO: ADD POPUP ERROR
        }
    }
}
