namespace Chess
{
    using System.Collections.Generic;

    public static class FenUtility
    {
        // Constants:
        static Dictionary<char, int> pieceTypeFromSymbol = new Dictionary<char, int>()
        {
            ['k'] = Piece.KING,
            ['p'] = Piece.PAWN,
            ['n'] = Piece.KNIGHT,
            ['b'] = Piece.BISHOP,
            ['r'] = Piece.ROOK,
            ['q'] = Piece.QUEEN
        };
        public const string START_FEN = "rnbqkbnr/pppppppp/8/8/8/8/PPPPPPPP/RNBQKBNR w KQkq - 0 1";

        /*
         * Load position from FEN
         * Input : fen                - the FEN string
         * Output: loadedPositionInfo - the loaded position
         */
        public static LoadedPositionInfo PositionFromFen(string fen)
        {
            // Inits:
            LoadedPositionInfo loadedPositionInfo = new LoadedPositionInfo();
            string[] sections = fen.Split(' ');
            int file = 0;
            int rank = 7;

            // Converting FEN to pieces:
            foreach (char symbol in sections[0])
            {
                // Condition: next rank
                if (symbol == '/')
                {
                    file = 0;
                    rank--;
                }

                else
                {
                    // Condition: empty spaces
                    if (char.IsDigit(symbol))
                    {
                        file += (int)char.GetNumericValue(symbol);
                    }

                    else
                    {
                        // Adding the current piece:
                        int pieceColour = (char.IsUpper(symbol)) ? Piece.WHITE : Piece.BLACK;
                        int pieceType = pieceTypeFromSymbol[char.ToLower(symbol)];
                        loadedPositionInfo.squares[rank * 8 + file] = pieceType | pieceColour;
                        file++;
                    }
                }
            }

            // Getting the player to move:
            loadedPositionInfo.whiteToMove = (sections[1] == "w");

            // Setting castling rights:
            string castlingRights = (sections.Length > 2) ? sections[2] : "KQkq";
            loadedPositionInfo.whiteCastleKingside = castlingRights.Contains("K");
            loadedPositionInfo.whiteCastleQueenside = castlingRights.Contains("Q");
            loadedPositionInfo.blackCastleKingside = castlingRights.Contains("k");
            loadedPositionInfo.blackCastleQueenside = castlingRights.Contains("q");

            // Condition: en-passant available
            if (sections.Length > 3)
            {
                string enPassantFileName = sections[3][0].ToString();
                if (BoardRepresentation.FILE_NAMES.Contains(enPassantFileName))
                {
                    loadedPositionInfo.enPassantFile = BoardRepresentation.FILE_NAMES.IndexOf(enPassantFileName) + 1;
                }
            }

            // Getting the plies count:
            if (sections.Length > 4)
            {
                int.TryParse(sections[4], out loadedPositionInfo.plyCount);
            }

            return loadedPositionInfo;
        }

        /*
         * Getting FEN string from board
         * Input : board - the board
         * Output: fen - the FEN string
         */
        public static string CurrentFen(Board board) // TODO: DOCUMENT
        {
            // Inits:
            string fen = "";

            for (int rank = 7; rank >= 0; rank--)
            {
                int numEmptyFiles = 0;
                for (int file = 0; file < 8; file++)
                {
                    int i = rank * 8 + file;
                    int piece = board.square[i];
                    if (piece != 0)
                    {
                        if (numEmptyFiles != 0)
                        {
                            fen += numEmptyFiles;
                            numEmptyFiles = 0;
                        }

                        bool isBlack = Piece.IsColour(piece, Piece.BLACK);
                        int pieceType = Piece.PieceType(piece);
                        char pieceChar = ' ';

                        switch (pieceType)
                        {
                            case Piece.ROOK:
                                pieceChar = 'R';
                                break;
                            case Piece.KNIGHT:
                                pieceChar = 'N';
                                break;
                            case Piece.BISHOP:
                                pieceChar = 'B';
                                break;
                            case Piece.QUEEN:
                                pieceChar = 'Q';
                                break;
                            case Piece.KING:
                                pieceChar = 'K';
                                break;
                            case Piece.PAWN:
                                pieceChar = 'P';
                                break;
                        }
                        fen += (isBlack) ? pieceChar.ToString().ToLower() : pieceChar.ToString();
                    }
                    else
                    {
                        numEmptyFiles++;
                    }

                }
                if (numEmptyFiles != 0)
                {
                    fen += numEmptyFiles;
                }
                if (rank != 0)
                {
                    fen += '/';
                }
            }

            // Side to move
            fen += ' ';
            fen += (board.whiteToMove) ? 'w' : 'b';

            // Castling
            bool whiteKingside = (board.currentGameState & 1) == 1;
            bool whiteQueenside = (board.currentGameState >> 1 & 1) == 1;
            bool blackKingside = (board.currentGameState >> 2 & 1) == 1;
            bool blackQueenside = (board.currentGameState >> 3 & 1) == 1;
            fen += ' ';
            fen += (whiteKingside) ? "K" : "";
            fen += (whiteQueenside) ? "Q" : "";
            fen += (blackKingside) ? "k" : "";
            fen += (blackQueenside) ? "q" : "";
            fen += ((board.currentGameState & 15) == 0) ? "-" : "";

            // En-passant
            fen += ' ';
            int epFile = (int)(board.currentGameState >> 4) & 15;
            if (epFile == 0)
            {
                fen += '-';
            }
            else
            {
                string fileName = BoardRepresentation.FILE_NAMES[epFile - 1].ToString();
                int epRank = (board.whiteToMove) ? 6 : 3;
                fen += fileName + epRank;
            }

            // 50 move counter
            fen += ' ';
            fen += board.fiftyMoveCounter;

            // Full-move count (should be one at start, and increase after each move by black)
            fen += ' ';
            fen += (board.plyCount / 2) + 1;

            return fen;
        }

        // LoadedPositionInfo Class:
        public class LoadedPositionInfo
        {
            // Fields:
            public int[] squares;
            public bool whiteCastleKingside;
            public bool whiteCastleQueenside;
            public bool blackCastleKingside;
            public bool blackCastleQueenside;
            public int enPassantFile;
            public bool whiteToMove;
            public int plyCount;

            public LoadedPositionInfo()
            {
                squares = new int[64];
            }
        }
    }
}