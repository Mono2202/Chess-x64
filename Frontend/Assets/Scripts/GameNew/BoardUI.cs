using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace Chess.Game
{
    public class BoardUI : MonoBehaviour
    {
        // Inputs:
        public PieceTheme pieceTheme;
        public BoardTheme boardTheme;
        public bool showLegalMoves;
        public bool whiteIsBottom;
        public Shader squareShader;
        public Material squareMaterial;

        // Fields:
        MeshRenderer[,] squareRenderers;
        SpriteRenderer[,] squarePieceRenderers;
        Move lastMadeMove;
        MoveGenerator moveGenerator;

        // Constants:
        const float MOVE_ANIMIMATION_DURATION = 0.15f;
        const float PIECE_DEPTH = -0.1f;
        const float PIECE_DRAG_DEPTH = -0.2f;

        /*
         * Awake function
         */
        void Awake()
        {
            // Inits:
            moveGenerator = new MoveGenerator();
        }

         /*
          * Highlighting the legal moves
          * Input : board      - the board
          *         fromSquare - the source square
          * Output: < None >
          */
        public void HighlightLegalMoves(Board board, Coord fromSquare)
        {
            // Condition: show legal moves
            if (showLegalMoves)
            {
                // Getting the legal moves:
                var moves = moveGenerator.GenerateMoves(board);

                // Showing all the legal moves:
                for (int i = 0; i < moves.Count; i++)
                {
                    // Getting the current move:
                    Move move = moves[i];

                    // Condition: the move start square is the from square
                    if (move.StartSquare == BoardRepresentation.IndexFromCoord(fromSquare))
                    {
                        // Getting the coord of the target square:
                        Coord coord = BoardRepresentation.CoordFromIndex(move.TargetSquare);

                        // Setting the square color:
                        SetSquareColour(coord, boardTheme.lightSquares.legal, boardTheme.darkSquares.legal);
                    }
                }
            }
        }

        /*
         * Dragging a piece
         * Input : pieceCoord - the coord
         *         mousePos   - the mouse position
         * Output: < None >
         */
        public void DragPiece(Coord pieceCoord, Vector2 mousePos)
        {
            squarePieceRenderers[pieceCoord.fileIndex, pieceCoord.rankIndex].transform.position = new Vector3(mousePos.x, mousePos.y, PIECE_DRAG_DEPTH);
        }

        /*
         * Resetting the piece's position
         * Input : pieceCoord - the coord
         * Output: < None >
         */
        public void ResetPiecePosition(Coord pieceCoord)
        {
            Vector3 pos = PositionFromCoord(pieceCoord.fileIndex, pieceCoord.rankIndex, PIECE_DEPTH);
            squarePieceRenderers[pieceCoord.fileIndex, pieceCoord.rankIndex].transform.position = pos;
        }

        /*
         * Selecting a square
         * Input : coord - the square coord
         * Output: < None >
         */
        public void SelectSquare(Coord coord)
        {
            SetSquareColour(coord, boardTheme.lightSquares.selected, boardTheme.darkSquares.selected);
        }

        /*
         * Deselecting a square
         * Input : < None >
         * Output: < None >
         */
        public void DeselectSquare()
        {
            ResetSquareColours();
        }

        /*
         * Trying to get the square under the mouse
         * Input : mouseWorld    - the mouse position
         *         selectedCoord - the selected coord
         * Output: true  - square was selected
         *         false - square wasn't selected
         */
        public bool TryGetSquareUnderMouse(Vector2 mouseWorld, out Coord selectedCoord) // TODO: CHANGE TO CANVAS INSTEAD OF CAMERA
        {
            // Inits:
            int file = (int)(mouseWorld.x + 4);
            int rank = (int)(mouseWorld.y + 4);

            // Condition: black POV
            if (!whiteIsBottom)
            {
                file = 7 - file;
                rank = 7 - rank;
            }

            // Getting the selected coord:
            selectedCoord = new Coord(file, rank);

            // Returning whether a square was selected:
            return file >= 0 && file < 8 && rank >= 0 && rank < 8;
        }

        /*
         * Updating the board
         * Input : board - the board
         * Output: < None >
         */
        public void UpdatePosition(Board board)
        {
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    // Getting the current piece from the board:
                    Coord coord = new Coord(file, rank);
                    int piece = board.square[BoardRepresentation.IndexFromCoord(coord.fileIndex, coord.rankIndex)];

                    // Updating the GUI board:
                    squarePieceRenderers[file, rank].sprite = pieceTheme.GetPieceSprite(piece);
                    squarePieceRenderers[file, rank].transform.position = PositionFromCoord(file, rank, PIECE_DEPTH);
                }
            }

        }

        /*
         * Updating the board when move was made
         * Input : board   - the board
         *         move    - the move
         *         animate - whether to animate 
         *                   the move or not
         * Output: < None >
         */
        public void OnMoveMade(Board board, Move move, bool animate = false)
        {
            // Getting the last played move:
            lastMadeMove = move;

            // Condition: animate movement
            if (animate)
            {
                StartCoroutine(AnimateMove(move, board));
            }

            // Condition: updating the board without animation
            else
            {
                UpdatePosition(board);
                ResetSquareColours();
            }
        }

        /*
         * Animating a move on the board
         * Input : move  - the move
         *         board - the board
         * Output: < None >
         */
        IEnumerator AnimateMove(Move move, Board board)
        {
            // Inits:
            float t = 0;
            Coord startCoord = BoardRepresentation.CoordFromIndex(move.StartSquare);
            Coord targetCoord = BoardRepresentation.CoordFromIndex(move.TargetSquare);
            Transform pieceT = squarePieceRenderers[startCoord.fileIndex, startCoord.rankIndex].transform;
            Vector3 startPos = PositionFromCoord(startCoord);
            Vector3 targetPos = PositionFromCoord(targetCoord);
            SetSquareColour(BoardRepresentation.CoordFromIndex(move.StartSquare), boardTheme.lightSquares.moveFromHighlight, boardTheme.darkSquares.moveFromHighlight);

            // Animating the move:
            while (t <= 1)
            {
                yield return null;
                t += Time.deltaTime * 1 / MOVE_ANIMIMATION_DURATION;
                pieceT.position = Vector3.Lerp(startPos, targetPos, t);
            }

            // Updating the board:
            UpdatePosition(board);
            ResetSquareColours();
            pieceT.position = startPos;
        }

        /*
         * Highlighting a certain move
         * Input : move - the move
         * Output: < None >
         */
        void HighlightMove(Move move)
        {
            SetSquareColour(BoardRepresentation.CoordFromIndex(move.StartSquare), boardTheme.lightSquares.moveFromHighlight, boardTheme.darkSquares.moveFromHighlight);
            SetSquareColour(BoardRepresentation.CoordFromIndex(move.TargetSquare), boardTheme.lightSquares.moveToHighlight, boardTheme.darkSquares.moveToHighlight);
        }

        /*
         * Creating the board GUI
         * Input : < None >
         * Output: < None >
         */
        public void CreateBoardUI(bool whiteBottom)
        {
            // Inits:
            whiteIsBottom = whiteBottom;
            squareRenderers = new MeshRenderer[8, 8];
            squarePieceRenderers = new SpriteRenderer[8, 8];

            // Creating the GUI:
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    // Creating a square:
                    Transform square = GameObject.CreatePrimitive(PrimitiveType.Quad).transform;
                    square.name = BoardRepresentation.SquareNameFromCoordinate(file, rank);
                    square.position = PositionFromCoord(file, rank, 0);
                    square.parent = transform;

                    // Setting the renderers:
                    squareRenderers[file, rank] = square.gameObject.GetComponent<MeshRenderer>();
                    squareRenderers[file, rank].material = squareMaterial;

                    // Creating the sprite:
                    SpriteRenderer pieceRenderer = new GameObject("Piece").AddComponent<SpriteRenderer>();
                    pieceRenderer.transform.parent = square;
                    pieceRenderer.transform.position = PositionFromCoord(file, rank, PIECE_DEPTH);
                    pieceRenderer.transform.localScale = Vector3.one * 100 / (2000 / 6f); // TODO: CHANGE 100
                    squarePieceRenderers[file, rank] = pieceRenderer;
                }
            }


            // Resetting the square colors:
            ResetSquareColours();
        }

        /*
         * Resetting the square positions
         * Input : < None>
         * Output: < None >
         */
        void ResetSquarePositions()
        {
            // Resetting the GUI:
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    squareRenderers[file, rank].transform.position = PositionFromCoord(file, rank, 0);
                    squarePieceRenderers[file, rank].transform.position = PositionFromCoord(file, rank, PIECE_DEPTH);
                }
            }

            // Condition: valid move
            if (!lastMadeMove.IsInvalid)
            {
                // Highlighting the last played move:
                HighlightMove(lastMadeMove);
            }
        }

        /*
         * Set board perspective
         * Input : whitePov - true  - white POV
         *                    false - black POV
         * Output: < None >
         */
        public void SetPerspective(bool whitePOV)
        {
            // Setting GUI:
            whiteIsBottom = whitePOV;
            ResetSquarePositions();
        }

        /*
         * Resetting the square colors
         * Input : highlight - whether to highlight or not
         * Output: < None >
         */
        public void ResetSquareColours(bool highlight = true)
        {
            // Setting the default square color:
            for (int rank = 0; rank < 8; rank++)
            {
                for (int file = 0; file < 8; file++)
                {
                    SetSquareColour(new Coord(file, rank), boardTheme.lightSquares.normal, boardTheme.darkSquares.normal);
                }
            }

            // Highlighting the last played move:
            if (highlight)
            {
                if (!lastMadeMove.IsInvalid)
                {
                    HighlightMove(lastMadeMove);
                }
            }
        }

        /*
         * Setting a square color
         * Input : square   - the square to color
         *         lightCol - the light square color
         *         darkCol  - the dark square color
         * Output: < None >
         */
        void SetSquareColour(Coord square, Color lightCol, Color darkCol)
        {
            squareRenderers[square.fileIndex, square.rankIndex].material.color = (square.IsLightSquare()) ? lightCol : darkCol;
        }

        /*
         * Getting a position from coord
         * Input : file  - the file
         *         rank  - the rank
         *         depth - the depth
         * Output: < None >
         */
        public Vector3 PositionFromCoord(int file, int rank, float depth = 0)
        {
            // Condition: white POV
            if (whiteIsBottom)
            {
                return new Vector3(-3.5f + file, -3.5f + rank, depth);
            }

            // Condition: black POV
            return new Vector3(-3.5f + 7 - file, 7 - rank - 3.5f, depth);

        }

        /*
         * Getting a position from coord
         * Input : coord - the coord
         *         depth - the depth
         * Output: < None >
         */
        public Vector3 PositionFromCoord(Coord coord, float depth = 0)
        {
            return PositionFromCoord(coord.fileIndex, coord.rankIndex, depth);
        }
    }
}