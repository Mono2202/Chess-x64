namespace Chess
{
    using System.Collections.Generic;

    public class Board
    {
        // Constants:
        public const int WHITE_INDEX = 0;
        public const int BLACK_INDEX = 1;
        const uint WHITE_CASTLE_KINGSIDE_MASK = 0b1111111111111110;
        const uint WHITE_CASTLE_QUEENSIDE_MASK = 0b1111111111111101;
        const uint BLACK_CASTLE_KINGSIDE_MASK = 0b1111111111111011;
        const uint BLACK_CASTLE_QUEENSIDE_MASK = 0b1111111111110111;
        const uint WHITE_CASTLE_MASK = WHITE_CASTLE_KINGSIDE_MASK & WHITE_CASTLE_QUEENSIDE_MASK;
        const uint BLACK_CASTLE_MASK = BLACK_CASTLE_KINGSIDE_MASK & BLACK_CASTLE_QUEENSIDE_MASK;

        // Fields:
        public int[] square;            // Stored piece codes; Piece code is defined as piecetype | colour code
        public bool whiteToMove;
        public int colourToMove;
        public int opponentColour;
        public int colourToMoveIndex;

        Stack<uint> gameStateHistory;     // Bits 0-3   : store white and black kingside/queenside castling legality
                                          // Bits 4-7   : store file of en-passant square
                                          // Bits 8-13  : captured piece
                                          // Bits 14-...: fifty move counter
        public uint currentGameState;     // The current game state
        public int plyCount;              // Total plies played in game
        public int fiftyMoveCounter;      // Total plies since last pawn move or capture
        public int[] kingSquare;          // Square of white and black king

        public PieceList[] rooks;         //
        public PieceList[] bishops;       //
        public PieceList[] queens;        // Piece 
        public PieceList[] knights;       //     Lists
        public PieceList[] pawns;         //
        public PieceList[] allPieceLists; //

        /*
         * Get piece list
         * Input : pieceType - the piece type
         *         colorIndex - the color index
         * Output: the piece list
         */
        PieceList GetPieceList(int pieceType, int colourIndex)
        {
            return allPieceLists[colourIndex * 8 + pieceType];
        }

        /*
         * Making a move
         * Input : move - the move
         * Output: < None >
         */
        public void MakeMove(Move move)
        {
            // Inits:
            uint oldEnPassantFile = (currentGameState >> 4) & 15;
            uint originalCastleState = currentGameState & 15;
            uint newCastleState = originalCastleState;
            currentGameState = 0;

            int opponentColourIndex = 1 - colourToMoveIndex;
            int moveFrom = move.StartSquare;
            int moveTo = move.TargetSquare;

            int capturedPieceType = Piece.PieceType(square[moveTo]);
            currentGameState |= (ushort)(capturedPieceType << 8);
            int movePiece = square[moveFrom];
            int pieceOnTargetSquare = movePiece;
            int movePieceType = Piece.PieceType(movePiece);

            int moveFlag = move.MoveFlag;
            bool isPromotion = move.IsPromotion;
            bool isEnPassant = moveFlag == Move.Flag.EN_PASSANT_CAPTURE;

            // Handle captures:
            if (capturedPieceType != 0 && !isEnPassant)
            {
                GetPieceList(capturedPieceType, opponentColourIndex).RemovePieceAtSquare(moveTo);
            }

            // Condition: king moved
            if (movePieceType == Piece.KING)
            {
                kingSquare[colourToMoveIndex] = moveTo;

                // Keeping castle state:
                newCastleState &= (whiteToMove) ? WHITE_CASTLE_MASK : BLACK_CASTLE_MASK;
            }

            // Condition: other piece moved
            else
            {
                GetPieceList(movePieceType, colourToMoveIndex).MovePiece(moveFrom, moveTo);
            }

            // Handle promotion:
            if (isPromotion)
            {
                // Inits:
                int promoteType = 0;

                // Getting the correct promote type:
                switch (moveFlag)
                {
                    case Move.Flag.PROMOTE_TO_QUEEN:
                        promoteType = Piece.QUEEN;
                        queens[colourToMoveIndex].AddPieceAtSquare(moveTo);
                        break;

                    case Move.Flag.PROMOTE_TO_ROOK:
                        promoteType = Piece.ROOK;
                        rooks[colourToMoveIndex].AddPieceAtSquare(moveTo);
                        break;

                    case Move.Flag.PROMOTE_TO_BISHOP:
                        promoteType = Piece.BISHOP;
                        bishops[colourToMoveIndex].AddPieceAtSquare(moveTo);
                        break;

                    case Move.Flag.PROMOTE_TO_KNIGHT:
                        promoteType = Piece.KNIGHT;
                        knights[colourToMoveIndex].AddPieceAtSquare(moveTo);
                        break;
                }

                // Setting the promoted piece:
                pieceOnTargetSquare = promoteType | colourToMove;

                // Removing the promoted pawn from the square:
                pawns[colourToMoveIndex].RemovePieceAtSquare(moveTo);
            }

            else
            {
                // Handle other special moves:
                switch (moveFlag)
                {
                    // Condition: handle en-passant
                    case Move.Flag.EN_PASSANT_CAPTURE:

                        // Choosing the target square:
                        int enPassantPawnSquare = moveTo + ((colourToMove == Piece.WHITE) ? -8 : 8);

                        // Setting the en-passant square:
                        currentGameState |= (ushort)(square[enPassantPawnSquare] << 8);

                        // Clearing the en-passant square:
                        square[enPassantPawnSquare] = 0;
                        pawns[opponentColourIndex].RemovePieceAtSquare(enPassantPawnSquare);
                        break;

                    // Condition: handle castling
                    case Move.Flag.CASTLING:

                        // Checking which side to castle:
                        bool kingside = moveTo == BoardRepresentation.g1 || moveTo == BoardRepresentation.g8;
                        int castlingRookFromIndex = (kingside) ? moveTo + 1 : moveTo - 2;
                        int castlingRookToIndex = (kingside) ? moveTo - 1 : moveTo + 1;

                        // Updating the board:
                        square[castlingRookFromIndex] = Piece.NONE;
                        square[castlingRookToIndex] = Piece.ROOK | colourToMove;
                        rooks[colourToMoveIndex].MovePiece(castlingRookFromIndex, castlingRookToIndex);
                        break;
                }
            }

            // Update the board representation:
            square[moveTo] = pieceOnTargetSquare;
            square[moveFrom] = 0;

            // Pawn has moved two forwards, mark file with en-passant flag:
            if (moveFlag == Move.Flag.PAWN_TWO_FORWARD)
            {
                int file = BoardRepresentation.FileIndex(moveFrom) + 1;
                currentGameState |= (ushort)(file << 4);
            }

            // Piece moving to/from rook square removes castling right for that side:
            if (originalCastleState != 0)
            {
                if (moveTo == BoardRepresentation.h1 || moveFrom == BoardRepresentation.h1)
                {
                    newCastleState &= WHITE_CASTLE_KINGSIDE_MASK;
                }
                else if (moveTo == BoardRepresentation.a1 || moveFrom == BoardRepresentation.a1)
                {
                    newCastleState &= WHITE_CASTLE_QUEENSIDE_MASK;
                }
                if (moveTo == BoardRepresentation.h8 || moveFrom == BoardRepresentation.h8)
                {
                    newCastleState &= BLACK_CASTLE_KINGSIDE_MASK;
                }
                else if (moveTo == BoardRepresentation.a8 || moveFrom == BoardRepresentation.a8)
                {
                    newCastleState &= BLACK_CASTLE_QUEENSIDE_MASK;
                }
            }

            // Updating the current game state:
            currentGameState |= newCastleState;
            currentGameState |= (uint)fiftyMoveCounter << 14;

            // Pushing the current game state to the game state history:
            gameStateHistory.Push(currentGameState);

            // Change side to move:
            whiteToMove = !whiteToMove;
            colourToMove = (whiteToMove) ? Piece.WHITE : Piece.BLACK;
            opponentColour = (whiteToMove) ? Piece.BLACK : Piece.WHITE;
            colourToMoveIndex = 1 - colourToMoveIndex;

            // Raising move counters:
            plyCount++;
            fiftyMoveCounter++;
        }

        /*
         * Loading the start position
         * Input : < None >
         * Output: < None >
         */
        public void LoadStartPosition()
        {
            LoadPosition(FenUtility.START_FEN);
        }

        /*
         * Loading custom position from FEN string
         * Input : fen - the FEN string
         * Output: < None >
         */
        public void LoadPosition(string fen)
        {
            // Inits:
            Initialize();
            var loadedPosition = FenUtility.PositionFromFen(fen);

            // Load pieces into board array and piece lists:
            for (int squareIndex = 0; squareIndex < 64; squareIndex++)
            {
                // Inits:
                int piece = loadedPosition.squares[squareIndex];
                square[squareIndex] = piece;

                // Condition: piece exists
                if (piece != Piece.NONE)
                {
                    // Getting the piece type and color:
                    int pieceType = Piece.PieceType(piece);
                    int pieceColourIndex = (Piece.IsColour(piece, Piece.WHITE)) ? WHITE_INDEX : BLACK_INDEX;

                    // Condition: piece is a sliding piece
                    if (Piece.IsSlidingPiece(piece))
                    {
                        // Condition: piece is a queen
                        if (pieceType == Piece.QUEEN)
                        {
                            queens[pieceColourIndex].AddPieceAtSquare(squareIndex);
                        }

                        // Condition: piece is a rook
                        else if (pieceType == Piece.ROOK)
                        {
                            rooks[pieceColourIndex].AddPieceAtSquare(squareIndex);
                        }

                        // Condition: piece is a bishop
                        else if (pieceType == Piece.BISHOP)
                        {
                            bishops[pieceColourIndex].AddPieceAtSquare(squareIndex);
                        }
                    }

                    // Condition: piece is a knight
                    else if (pieceType == Piece.KNIGHT)
                    {
                        knights[pieceColourIndex].AddPieceAtSquare(squareIndex);
                    }

                    // Condition: piece is a pawn
                    else if (pieceType == Piece.PAWN)
                    {
                        pawns[pieceColourIndex].AddPieceAtSquare(squareIndex);
                    }

                    // Condition: piece is a king
                    else if (pieceType == Piece.KING)
                    {
                        kingSquare[pieceColourIndex] = squareIndex;
                    }
                }
            }

            // Setting color to move:
            whiteToMove = loadedPosition.whiteToMove;
            colourToMove = (whiteToMove) ? Piece.WHITE : Piece.BLACK;
            opponentColour = (whiteToMove) ? Piece.BLACK : Piece.WHITE;
            colourToMoveIndex = (whiteToMove) ? 0 : 1;

            // Creating the gamestate:
            int whiteCastle = ((loadedPosition.whiteCastleKingside) ? 1 << 0 : 0) | ((loadedPosition.whiteCastleQueenside) ? 1 << 1 : 0);
            int blackCastle = ((loadedPosition.blackCastleKingside) ? 1 << 2 : 0) | ((loadedPosition.blackCastleQueenside) ? 1 << 3 : 0);
            int enPassantState = loadedPosition.enPassantFile << 4;
            ushort initialGameState = (ushort)(whiteCastle | blackCastle | enPassantState);
            currentGameState = initialGameState;

            // Adding to the game history:
            gameStateHistory.Push(initialGameState);

            // Setting the plies count:
            plyCount = loadedPosition.plyCount;
        }

        /*
         * Initialize board
         * Input : < None >
         * Output: < None >
         */
        void Initialize()
        {
            // Inits:
            square = new int[64];
            kingSquare = new int[2];
            gameStateHistory = new Stack<uint>();
            plyCount = 0;
            fiftyMoveCounter = 0;

            knights = new PieceList[] { new PieceList(10), new PieceList(10) };
            pawns = new PieceList[] { new PieceList(8), new PieceList(8) };
            rooks = new PieceList[] { new PieceList(10), new PieceList(10) };
            bishops = new PieceList[] { new PieceList(10), new PieceList(10) };
            queens = new PieceList[] { new PieceList(9), new PieceList(9) };
            PieceList emptyList = new PieceList(0);

            allPieceLists = new PieceList[] {
                emptyList,
                emptyList,
                pawns[WHITE_INDEX],
                knights[WHITE_INDEX],
                emptyList,
                bishops[WHITE_INDEX],
                rooks[WHITE_INDEX],
                queens[WHITE_INDEX],
                emptyList,
                emptyList,
                pawns[BLACK_INDEX],
                knights[BLACK_INDEX],
                emptyList,
                bishops[BLACK_INDEX],
                rooks[BLACK_INDEX],
                queens[BLACK_INDEX],
            };
        }
    }
}