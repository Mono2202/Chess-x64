using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Chess.Game
{
    public class GameManagerEditor : MonoBehaviour
    {

        // Inputs:
        public bool loadCustomPosition;
        public string customPosition;

        // Fields:
        public enum Result { Playing, WhiteIsMated, BlackIsMated, Stalemate, Repetition, FiftyMoveRule, InsufficientMaterial }
        public event System.Action onPositionLoaded;
        public event System.Action<Move> onMoveMade;
        Player whitePlayer;
        Player blackPlayer;
        Player playerToMove;
        List<Move> gameMoves;
        BoardUI boardUI;
        Result gameResult;
        public Board board { get; private set; }

        /*
		 * Initializing the GameManager
		 */
        void Start()
        {
            boardUI = FindObjectOfType<BoardUI>();
            gameMoves = new List<Move>();
            board = new Board();
            NewGame();
        }

        /*
         * Update the game each frame
         */
        void Update()
        {
            if (gameResult == Result.Playing)
            {
                playerToMove.Update();
            }
        }

        /*
         * Move was chosen
         */
        void OnMoveChosen(Move move)
        {
            board.MakeMove(move);
            gameMoves.Add(move);
            onMoveMade?.Invoke(move);
            boardUI.OnMoveMade(board, move, false);
            playerToMove = (board.whiteToMove) ? whitePlayer : blackPlayer;
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
    }
}