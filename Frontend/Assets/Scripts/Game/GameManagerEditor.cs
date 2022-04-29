using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

namespace Chess.Game
{
    public class GameManagerEditor : MonoBehaviour
    {

        // Inputs:
        public bool loadCustomPosition;
        public string customPosition;
        public InputField inputFEN;
        public Button inputFENButton;
        public Button resetButton;
        public Text inputFENButtonText;
        public Text bestMoveLabel;

        // Fields:
        public enum Result { Playing, WhiteIsMated, BlackIsMated, Stalemate, Repetition, FiftyMoveRule, InsufficientMaterial }
        public event System.Action onPositionLoaded;
        public event System.Action<Move> onMoveMade;
        Player whitePlayer;
        Player blackPlayer;
        Player playerToMove;
        List<Move> gameMoves;
        BoardUI boardUI;
        string currentBestMove;
        public Board board { get; private set; }

        /*
		 * Initializing the GameManager
		 */
        void Start()
        {
            boardUI = FindObjectOfType<BoardUI>();
            boardUI.CreateBoardUI(true);
            gameMoves = new List<Move>();
            board = new Board();
            NewGame();
        }

        /*
         * Update the game each frame
         */
        void Update()
        {
            // Condition: new best move
            if (bestMoveLabel.text != currentBestMove)
            {
                bestMoveLabel.text = currentBestMove;
            }

            // Checking if fields were filled:
            inputFENButton.interactable = !(inputFEN.text == "");
            inputFENButtonText.color = !(inputFEN.text == "") ? new Color32(31, 194, 18, 255) : new Color32(31, 194, 18, 100);
            
            // Updating moves:
            playerToMove.Update();
        }

        /*
         * Move was chosen
         */
        void OnMoveChosen(Move move)
        {
            // Updating game and board:
            board.MakeMove(move);
            gameMoves.Add(move);
            onMoveMade?.Invoke(move);
            boardUI.OnMoveMade(board, move, false);
            playerToMove = (board.whiteToMove) ? whitePlayer : blackPlayer;

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
                try
                {
                    board.LoadPosition(customPosition);
                }

                // Condition: loading start position:
                catch
                {
                    board.LoadStartPosition();
                }
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

        /*
         * Loading board with certain FEN
         * Input : < None >
         * Output: < None >
         */
        public void LoadFEN()
        {
            loadCustomPosition = true;
            customPosition = inputFEN.text;
            inputFEN.text = "";
            NewGame();
        }

        /*
         * Resetting the board to a default position
         * Input : < None >
         * Output: < None >
         */
        public void ResetBoard()
        {
            loadCustomPosition = false;
            NewGame();
        }

        /*
         * Using Stockfish to get the best move
         * Input : < None >
         * Output: < None >
         */
        private void UseEngine()
        {
            // Getting the current FEN:
            string fenString = FenUtility.CurrentFen(board);

            // Execute stockfish:
            var p = new System.Diagnostics.Process();
            p.StartInfo.FileName = Application.streamingAssetsPath + "/Stockfish.exe";
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardInput = true;
            p.StartInfo.RedirectStandardOutput = true;
            p.StartInfo.CreateNoWindow = true;
            p.Start();

            // Setting the position:
            string setupString = "position fen " + fenString;
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
            currentBestMove = bestMove.Substring(9, 4);

            // Closing the process: 
            p.Close();
        }
    }
}