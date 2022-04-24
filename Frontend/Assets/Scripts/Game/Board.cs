using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Board : MonoBehaviour
{
    // Fields:
    private Communicator communicator;
    private Communicator listener;
    public GameObject cell;
    public Transform canvas;
    public List<Sprite> chessSprites = new List<Sprite>();
    public Button returnButton;
    public GameObject returnButtonSignResign;
    public GameObject returnButtonSignLeave;
    public Text returnButtonText;
    public GameObject popupWindow;
    public Text whiteUsername;
    public Text blackUsername;
    [HideInInspector] public GameObject[,] guiBoardArr = new GameObject[BOARD_SIZE, BOARD_SIZE];
    [HideInInspector] public Piece[,] boardArr = new Piece[BOARD_SIZE, BOARD_SIZE];
    [HideInInspector] public string currentMove;
    [HideInInspector] public Position selectedPiece;
    [HideInInspector] public Position selectedDest;
    [HideInInspector] public bool isPlayerWhite;
    [HideInInspector] public bool isCurrentPlayerWhite;
    [HideInInspector] public string playerMove;
    [HideInInspector] public bool gameOver = false;
    [HideInInspector] public bool gameOverBoard = false;
    [HideInInspector] public bool updateBoardFlag = false;
    [HideInInspector] public string status = "";
    [HideInInspector] public Color32 titleColor;
    [HideInInspector] public Color32 textColor;
    [HideInInspector] public Thread tSubmitMove;
    [HideInInspector] public Thread tGetState;

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
        // Inits:
        communicator = Data.instance.communicator;
        listener = Data.instance.listener;
        listener.m_socket.Flush();

        // Sending the get room state request:
        communicator.Write(Serializer.SerializeRequest<GetRoomStateRequest>(new GetRoomStateRequest { }, Serializer.GET_ROOM_STATE_REQUEST));

        // Deserializing the response:
        GetRoomStateResponse response = Deserializer.DeserializeResponse<GetRoomStateResponse>(communicator.Read());
        string[] startingColors = response.CurrentMove.Split(new string[] { "&&&" }, StringSplitOptions.None);

        // Setting the board's fields:
        isPlayerWhite = (startingColors[0] == Data.instance.username) ? true : false;
        currentMove = response.CurrentMove;
        isCurrentPlayerWhite = true;
        playerMove = "";

        // Setting the players:
        whiteUsername.text = startingColors[0];
        blackUsername.text = startingColors[2];

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

        // Starting communication with server about game state:
        tSubmitMove = new Thread(new ThreadStart(SubmitMove));
        tGetState = new Thread(new ThreadStart(GetState));
        tSubmitMove.Start();
        tGetState.Start();
    }

    // Update is called once per frame
    void Update()
    {
        // Condition: move was played
        if (selectedPiece != null && selectedDest != null && playerMove == "" && !gameOverBoard)
        {
            if (boardArr[selectedPiece.row, selectedPiece.col].GetLegalMoves(boardArr, currentMove).Any(move => (move.Value != null && move.Value.row == selectedDest.row && move.Value.col == selectedDest.col)))
            {
                // Changing the player move:
                playerMove = Position.MoveToNotation(boardArr[selectedPiece.row, selectedPiece.col].type, selectedPiece, selectedDest, false, boardArr);

                // Updating the current move:
                currentMove = playerMove;

                // Updating the board:
                UpdateBoard(selectedPiece, selectedDest);
            }

            // Resetting the properties:
            selectedPiece = null;
            selectedDest = null;
        }

        // Condition: board needs to be updated
        if (updateBoardFlag)
        {
            // Resetting the flag:
            updateBoardFlag = false;

            // Updating the board:
            UpdateBoard(new Position(currentMove[2] - '1', currentMove[1] - 'a'),
                new Position(currentMove[4] - '1', currentMove[3] - 'a'));
        }

        // Condition: game is over
        if (gameOver)
        {
            gameOver = false;
            GameOver();
        }
    }

    private void SubmitMove()
    {
        while (true)
        {
            // Condition: current player playes
            if (isCurrentPlayerWhite == isPlayerWhite && playerMove != "")
            {
                // Sending the submit move request:
                communicator.Write(Serializer.SerializeRequest<SubmitMoveRequest>(new SubmitMoveRequest { Move = playerMove }, Serializer.SUBMIT_MOVE_REQUEST));

                // Reading the response:
                string msg = communicator.Read();

                // Condition: game ended with win
                if (playerMove.Contains("#"))
                {
                    gameOver = true;
                    gameOverBoard = true;
                    status = "Win";
                    titleColor = new Color32(8, 171, 0, 255);
                    textColor = new Color32(8, 171, 0, 255);
                    return;
                }

                // Condition: game ended with tie
                else if (playerMove.Contains("%"))
                {
                    gameOver = true;
                    gameOverBoard = true;
                    status = "Tie";
                    titleColor = new Color32(8, 171, 0, 255);
                    textColor = new Color32(8, 171, 0, 255);
                    return;
                }

                // Resetting the player's move:
                playerMove = "";

                // Switching players:
                isCurrentPlayerWhite = !isCurrentPlayerWhite;
            }
        }
    }

    private void GetState()
    {
        while (true)
        {
            // Deserializing the response:
            GetRoomStateResponse response = Deserializer.DeserializeResponse<GetRoomStateResponse>(listener.Read());
            print(response.CurrentMove);
            // Switching players:
            isCurrentPlayerWhite = !isCurrentPlayerWhite;

            // Updating the current move:
            currentMove = response.CurrentMove;

            // Condition: game ended with lose
            if (currentMove.Contains("#"))
            {
                gameOver = true;
                gameOverBoard = true;
                status = "Lose";
                titleColor = new Color32(240, 41, 41, 255);
                textColor = new Color32(240, 41, 41, 255);
                return;
            }

            // Condition: game ended with tie
            else if (currentMove.Contains("%"))
            {
                gameOver = true;
                gameOverBoard = true;
                status = "Tie";
                titleColor = new Color32(8, 171, 0, 255);
                textColor = new Color32(8, 171, 0, 255);
                return;
            }

            // Condition: game ended with win
            else if (currentMove.Contains("OPPONENT LEFT"))
            {
                gameOver = true;
                gameOverBoard = true;
                status = "Win";
                titleColor = new Color32(8, 171, 0, 255);
                textColor = new Color32(8, 171, 0, 255);
                return;
            }

            // Setting the update board flag:
            updateBoardFlag = true;
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
        currentCell.GetComponent<Image>().color = (((i + j) % 2 == 0) /*== Data.instance.isWhite*/) ? Data.instance.whiteSquareColor : Data.instance.blackSquareColor;

        // Changing the cell's parent:
        currentCell.transform.SetParent(canvas, false);

        // Positioning the cell:
        currentCell.transform.localPosition = new Vector3(-350 / 2 + j * 50, 350 / 2 - i * 50, 0); // TODO: TURN TO CONSTANTS

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
        currentCell.GetComponent<CellClick>().row = i;
        currentCell.GetComponent<CellClick>().col = j;
        currentCell.GetComponent<CellClick>().boardScript = this;

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
        // Aborting the threads:
        tSubmitMove.Abort();
        tGetState.Abort();

        // Sending the leave room request:
        communicator.Write(Serializer.SerializeRequest<LeaveRoomRequest>(new LeaveRoomRequest { }, Serializer.LEAVE_ROOM_REQUEST));

        // Reading the message:
        string msg = communicator.Read();

        // Switching to the menu scene:
        this.GetComponent<SwitchScene>().SwitchSceneByIndex(Data.MENU_SCENE_COUNT);
    }

    private void GameOver()
    {
        // Opening the popup window:
        popupWindow.GetComponent<PopupWindow>().SetProperties("Results", status, titleColor, textColor);
        popupWindow.SetActive(true);

        // Assigning the exit button labels:
        returnButtonSignResign.SetActive(false);
        returnButtonSignLeave.SetActive(true);
        returnButtonText.text = "Leave";
    }
}