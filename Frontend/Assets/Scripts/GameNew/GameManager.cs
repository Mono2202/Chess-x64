using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Chess.Game
{
    public class GameManager : MonoBehaviour
    {

        // Inputs:
        public bool loadCustomPosition;
        public string customPosition;

        // Fields:
        public enum Result { Playing, WhiteIsMated, BlackIsMated, Stalemate, Repetition, FiftyMoveRule, InsufficientMaterial, OpponentResigned }
        public event System.Action onPositionLoaded;
        public event System.Action<Move> onMoveMade;
        Player whitePlayer;
        Player blackPlayer;
        Player playerToMove;
        Player currentPlayer;
        List<Move> gameMoves;
        BoardUI boardUI;
        Result gameResult;
        public Board board { get; private set; }
        public Button returnButton;
        public GameObject returnButtonSignResign;
        public GameObject returnButtonSignLeave;
        public Text returnButtonText;
        public GameObject popupWindow;
        public Text whiteUsername;
        public Text blackUsername;
        private Communicator communicator;
        private Communicator listener;
        private Thread tGetMove;
        [HideInInspector] public bool isPlayerWhite;
        [HideInInspector] public bool isCurrentPlayerWhite;
        [HideInInspector] public string playerMove;
        [HideInInspector] public bool gameOver = false;
        [HideInInspector] public bool gotNewMove = false;
        [HideInInspector] public ushort currentMove;
        [HideInInspector] public string status = "";
        [HideInInspector] public Color32 titleColor;
        [HideInInspector] public Color32 textColor;

        /*
		 * Initializing the GameManager
		 */
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
            Data.instance.isWhite = isPlayerWhite;
            isCurrentPlayerWhite = true;

            // Setting the players:
            whiteUsername.text = startingColors[0];
            blackUsername.text = startingColors[2];

            // Starting communication with server about game state:
            tGetMove = new Thread(new ThreadStart(GetMove));
            tGetMove.Start();

            boardUI = FindObjectOfType<BoardUI>();
            boardUI.CreateBoardUI(isPlayerWhite);
            gameMoves = new List<Move>();
            board = new Board();
            NewGame();
        }

        /*
         * Update the game each frame
         */
        void Update()
        {
            // Condition: game isn't over
            if (!gameOver)
            {
                // Condition: opponent resigned
                if (gameResult == Result.OpponentResigned)
                {
                    titleColor = new Color32(8, 171, 0, 255);
                    textColor = new Color32(8, 171, 0, 255);
                    status = "Opponent Resigned";
                    GameOver();
                }

                // Updating the game state:
                gameResult = GetGameState();

                // Condition: game is still in progress
                if (gameResult == Result.Playing && playerToMove == currentPlayer)
                {
                    currentPlayer.Update();
                }

                // Condition: got a new move from the server:
                if (gotNewMove)
                {
                    OnMoveChosen(new Move(currentMove));
                    gotNewMove = false;
                }

                // Condition: opponent resigned
                if ((gameResult == Result.WhiteIsMated && isPlayerWhite) ||
                    (gameResult == Result.BlackIsMated && !isPlayerWhite))
                {
                    titleColor = new Color32(240, 41, 41, 255);
                    textColor = new Color32(240, 41, 41, 255);
                    status = "Game Lost";
                    GameOver();
                }

                else if ((gameResult == Result.WhiteIsMated && !isPlayerWhite) ||
                    (gameResult == Result.BlackIsMated && isPlayerWhite))
                {
                    titleColor = new Color32(8, 171, 0, 255);
                    textColor = new Color32(8, 171, 0, 255);
                    status = "Game Won";
                    GameOver();
                }
            }
        }

        /*
         * Move was chosen
         * Input : move - the move
         * Output: < None >
         */
        void OnMoveChosen(Move move)
        { 
            board.MakeMove(move);
            gameMoves.Add(move);
            onMoveMade?.Invoke(move);
            boardUI.OnMoveMade(board, move, false);
            playerToMove = (board.whiteToMove) ? whitePlayer : blackPlayer;

            if (!gotNewMove)
            {
                // Sending the submit move request:
                communicator.Write(Serializer.SerializeRequest<SubmitMoveRequest>(new SubmitMoveRequest { Move = Convert.ToString(move.Value) + "-" + Convert.ToString(GetGameState()) }, Serializer.SUBMIT_MOVE_REQUEST));

                // Reading the response:
                string msg = communicator.Read();
            }
        }

        /*
         * Creating a new game
         * Input : < None >
         * Output: < None>
         */
        void NewGame()
        {
            // Clearing the game moves:
            gameMoves.Clear();

            // Condition: loading a custom position
            if (loadCustomPosition)
            {
                board.LoadPosition(customPosition);
            }

            // Condition: loading the start position
            else
            {
                board.LoadStartPosition();
            }

            // Updating the GUI:
            onPositionLoaded?.Invoke();
            boardUI.UpdatePosition(board);
            boardUI.ResetSquareColours();

            // Creating the players:
            CreatePlayer(ref whitePlayer);
            CreatePlayer(ref blackPlayer);
            playerToMove = (board.whiteToMove) ? whitePlayer : blackPlayer;
            currentPlayer = (isPlayerWhite) ? whitePlayer : blackPlayer;

            // Setting the current game state:
            gameResult = Result.Playing;
        }

        /*
         * Getting the game state
         * Input : < None >
         * Output: the game state result
         */
        Result GetGameState()
        {
            // Inits:
            MoveGenerator moveGenerator = new MoveGenerator();
            var moves = moveGenerator.GenerateMoves(board);

            // Condition: game is over
            if (moves.Count == 0)
            {
                // Condition: mate
                if (moveGenerator.InCheck())
                {
                    return (board.whiteToMove) ? Result.WhiteIsMated : Result.BlackIsMated;
                }

                // Condition: stalemate
                return Result.Stalemate;
            }

            // Condition: fifty move rule
            if (board.fiftyMoveCounter >= 100)
            {
                return Result.FiftyMoveRule;
            }

            // Condition: threefold repetition
            // TODO

            // Condition: insufficient material
            int numPawns = board.pawns[Board.WHITE_INDEX].Count + board.pawns[Board.BLACK_INDEX].Count;
            int numRooks = board.rooks[Board.WHITE_INDEX].Count + board.rooks[Board.BLACK_INDEX].Count;
            int numQueens = board.queens[Board.WHITE_INDEX].Count + board.queens[Board.BLACK_INDEX].Count;
            int numKnights = board.knights[Board.WHITE_INDEX].Count + board.knights[Board.BLACK_INDEX].Count;
            int numBishops = board.bishops[Board.WHITE_INDEX].Count + board.bishops[Board.BLACK_INDEX].Count;
            if (numPawns + numRooks + numQueens == 0)
            {
                if (numKnights == 1 || numBishops == 1)
                {
                    return Result.InsufficientMaterial;
                }
            }

            // Condition: game is still going
            return Result.Playing;
        }

        /*
         * Creating a player
         * Input : player - the player to create
         * Output: < None >
         */
        void CreatePlayer(ref Player player)
        {
            if (player != null)
            {
                player.onMoveChosen -= OnMoveChosen;
            }

            player = new HumanPlayer(board);
            player.onMoveChosen += OnMoveChosen;
        }

        private void GetMove()
        {
            while (true)
            {
                // Deserializing the response:
                GetRoomStateResponse response = Deserializer.DeserializeResponse<GetRoomStateResponse>(listener.Read());

                // Switching players:
                isCurrentPlayerWhite = !isCurrentPlayerWhite;

                // Condition: opponent left
                if (response.CurrentMove == "OPPONENT LEFT")
                {
                    gameResult = Result.OpponentResigned;
                    return;
                }

                // Updating the current move:
                currentMove = Convert.ToUInt16(response.CurrentMove);

                // Setting the flag:
                gotNewMove = true;
            }
        }

        public void ReturnToMenu()
        {
            // Aborting the thread:
            tGetMove.Abort();

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
            
            // Setting the flag:
            gameOver = true;

            // Assigning the exit button labels:
            returnButtonSignResign.SetActive(false);
            returnButtonSignLeave.SetActive(true);
            returnButtonText.text = "Leave";
        }
    }
}