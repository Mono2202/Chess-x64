using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess.Game
{
    public class HumanPlayer : Player
    {

        // Enums:
        public enum InputState
        {
            None,
            PieceSelected,
            DraggingPiece
        }

        // Fields:
        InputState currentState;
        BoardUI boardUI;
        Camera cam;
        Coord selectedPieceSquare;
        Board board;

        /*
         * HumanPlayer C'tor
         * Input : board - the board
         * Output: < None >
         */
        public HumanPlayer(Board board)
        {
            boardUI = GameObject.FindObjectOfType<BoardUI>();
            cam = Camera.main;
            this.board = board;
        }

        public override void NotifyTurnToMove()
        {

        }

        public override void Update()
        {
            HandleInput();
        }

        /*
         * Handling user input
         * Input : < None >
         * Output: < None >
         */
        void HandleInput()
        {
            // Getting the mouse position:
            Vector2 mousePos = cam.ScreenToWorldPoint(Input.mousePosition);

            // Condition: no input
            if (currentState == InputState.None)
            {
                HandlePieceSelection(mousePos);
            }

            // Condition: dragging piece
            else if (currentState == InputState.DraggingPiece)
            {
                HandleDragMovement(mousePos);
            }

            // Condition: selecting piece
            else if (currentState == InputState.PieceSelected)
            {
                HandlePointAndClickMovement(mousePos);
            }

            // Condition: canceling a piece selection
            if (Input.GetMouseButtonDown(1))
            {
                CancelPieceSelection();
            }
        }

        /*
         * Handling point and click movement
         * Input : mousePos - the mouse position
         * Output: < None >
         */
        void HandlePointAndClickMovement(Vector2 mousePos)
        {
            // Condition: left click was pressed
            if (Input.GetMouseButton(0))
            {
                HandlePiecePlacement(mousePos);
            }
        }

        /*
         * Handling drag movement
         * Input : mousePos - the mouse position
         * Output: < None >
         */
        void HandleDragMovement(Vector2 mousePos)
        {
            // Dragging the piece:
            boardUI.DragPiece(selectedPieceSquare, mousePos);

            // If mouse is released, then try place the piece:
            if (Input.GetMouseButtonUp(0))
            {
                HandlePiecePlacement(mousePos);
            }
        }

        /*
         * Handling piece placement
         * Input : mousePos - the mouse position
         * Output: < None >
         */
        void HandlePiecePlacement(Vector2 mousePos)
        {
            // Inits:
            Coord targetSquare;
            
            // Condition: got a square under mouse
            if (boardUI.TryGetSquareUnderMouse(mousePos, out targetSquare))
            {
                // Condition: selecting the same square
                if (targetSquare.Equals(selectedPieceSquare))
                {
                    // Resetting the piece position:
                    boardUI.ResetPiecePosition(selectedPieceSquare);
                    
                    // Condition: dragging piece 
                    if (currentState == InputState.DraggingPiece)
                    {
                        currentState = InputState.PieceSelected;
                    }

                    // Condition: deselecting the square
                    else
                    {
                        currentState = InputState.None;
                        boardUI.DeselectSquare();
                    }
                }


                else
                {
                    // Getting the target square index:
                    int targetIndex = BoardRepresentation.IndexFromCoord(targetSquare.fileIndex, targetSquare.rankIndex);

                    // Condition: current player's piece
                    if (Piece.IsColour(board.square[targetIndex], board.colourToMove) && board.square[targetIndex] != 0)
                    {
                        // Handling the piece selection:
                        CancelPieceSelection();
                        HandlePieceSelection(mousePos);
                    }

                    // Trying to make a move:
                    else
                    {
                        TryMakeMove(selectedPieceSquare, targetSquare);
                    }
                }
            }

            // Canceling the piece selection:
            else
            {
                CancelPieceSelection();
            }

        }

        /*
         * Canceling piece selection
         * Input : < None >
         * Output: < None >
         */
        void CancelPieceSelection()
        {
            // Condition: the current state isn't none
            if (currentState != InputState.None)
            {
                // Resetting positions:
                currentState = InputState.None;
                boardUI.DeselectSquare();
                boardUI.ResetPiecePosition(selectedPieceSquare);
            }
        }

        /*
         * Trying to make a move
         * Input : startSquare  - the source square
         *         targetSquare - the target square
         * Output: < None >
         */
        void TryMakeMove(Coord startSquare, Coord targetSquare)
        {
            // Inits:
            int startIndex = BoardRepresentation.IndexFromCoord(startSquare);
            int targetIndex = BoardRepresentation.IndexFromCoord(targetSquare);
            bool moveIsLegal = false;
            Move chosenMove = new Move();
            MoveGenerator moveGenerator = new MoveGenerator();
            bool wantsKnightPromotion = Input.GetKey(KeyCode.LeftAlt);

            // Getting the legal moves:
            var legalMoves = moveGenerator.GenerateMoves(board);
            for (int i = 0; i < legalMoves.Count; i++)
            {
                // Current legal move:
                var legalMove = legalMoves[i];

                // Condition: legal move
                if (legalMove.StartSquare == startIndex && legalMove.TargetSquare == targetIndex)
                {
                    // TODO: DOCUMENT
                    if (legalMove.IsPromotion)
                    {
                        if (legalMove.MoveFlag == Move.Flag.PROMOTE_TO_QUEEN && wantsKnightPromotion)
                        {
                            continue;
                        }
                        if (legalMove.MoveFlag != Move.Flag.PROMOTE_TO_QUEEN && !wantsKnightPromotion)
                        {
                            continue;
                        }
                    }

                    // Setting the chosen move:
                    moveIsLegal = true;
                    chosenMove = legalMove;
                    break;
                }
            }

            // Condition: legal move
            if (moveIsLegal)
            {
                // Choosing the move:
                ChoseMove(chosenMove);
                currentState = InputState.None;
            }

            // Canceling the selection:
            else
            {
                CancelPieceSelection();
            }
        }

        /*
         * Handling piece selection
         * Input : mousePos - the mouse position
         * Output: < None >
         */
        void HandlePieceSelection(Vector2 mousePos)
        {
            // Condition: left click was pressed
            if (Input.GetMouseButtonDown(0))
            {
                // Condition: square was pressed
                if (boardUI.TryGetSquareUnderMouse(mousePos, out selectedPieceSquare))
                {
                    // Getting the selected piece index:
                    int index = BoardRepresentation.IndexFromCoord(selectedPieceSquare);

                    // If square contains a piece, select that piece for dragging:
                    if (Piece.IsColour(board.square[index], board.colourToMove))
                    {
                        // Highligting legal moves:
                        boardUI.HighlightLegalMoves(board, selectedPieceSquare);

                        // Selecting the square:
                        boardUI.SelectSquare(selectedPieceSquare);

                        // Updating the current state:
                        currentState = InputState.DraggingPiece;
                    }
                }
            }
        }
    }
}