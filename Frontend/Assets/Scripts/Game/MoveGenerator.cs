namespace Chess
{
    using System.Collections.Generic;
    using static PrecomputedMoveData;
    using static BoardRepresentation;

    public class MoveGenerator
    {
        // Fields:
        public enum PromotionMode { All, QueenOnly, QueenAndKnight }

        public PromotionMode promotionsToGenerate = PromotionMode.All;

        List<Move> moves;
        bool isWhiteToMove;
        int friendlyColour;
        int opponentColour;
        int friendlyKingSquare;
        int friendlyColourIndex;
        int opponentColourIndex;

        bool inCheck;
        bool inDoubleCheck;
        bool pinsExistInPosition;
        ulong checkRayBitmask;
        ulong pinRayBitmask;
        ulong opponentKnightAttacks;
        ulong opponentAttackMapNoPawns;
        public ulong opponentAttackMap;
        public ulong opponentPawnAttackMap;
        ulong opponentSlidingAttackMap;

        bool genQuiets;
        Board board;

        /*
         * Generates list of legal moves in current position
         * Input : board             - the board
         *         includeQuietMoves - moves that are not a capture
         *                             check or immediate threat
         * Output: moves             - the move list
         */
        public List<Move> GenerateMoves(Board board, bool includeQuietMoves = true)
        {
            // Inits:
            this.board = board;
            genQuiets = includeQuietMoves;
            Init();

            // Calculating moves:
            CalculateAttackData();
            GenerateKingMoves();

            // Only king moves are valid in a double check position, so can return early.
            if (inDoubleCheck)
            {
                return moves;
            }

            // Calculating more moves:
            GenerateSlidingMoves();
            GenerateKnightMoves();
            GeneratePawnMoves();

            return moves;
        }
        public bool InCheck()
        {
            return inCheck;
        }

        /*
         * Initializing the move generator
         * Input : < None >
         * Output: < None >
         */
        void Init()
        {
            // Inits:
            moves = new List<Move>(64);
            inCheck = false;
            inDoubleCheck = false;
            pinsExistInPosition = false;
            checkRayBitmask = 0;
            pinRayBitmask = 0;

            isWhiteToMove = board.colourToMove == Piece.WHITE;
            friendlyColour = board.colourToMove;
            opponentColour = board.opponentColour;
            friendlyKingSquare = board.kingSquare[board.colourToMoveIndex];
            friendlyColourIndex = (board.whiteToMove) ? Board.WHITE_INDEX : Board.BLACK_INDEX;
            opponentColourIndex = 1 - friendlyColourIndex;
        }

        /*
         * Generating king moves
         * Input : < None >
         * Output: < None >
         */
        void GenerateKingMoves()
        {
            for (int i = 0; i < kingMoves[friendlyKingSquare].Length; i++)
            {
                int targetSquare = kingMoves[friendlyKingSquare][i];
                int pieceOnTargetSquare = board.square[targetSquare];

                // Skip squares occupied by friendly pieces:
                if (Piece.IsColour(pieceOnTargetSquare, friendlyColour))
                {
                    continue;
                }

                bool isCapture = Piece.IsColour(pieceOnTargetSquare, opponentColour);
                if (!isCapture)
                {
                    // Condition: king can't move to square marked as under enemy control, unless he is capturing that piece
                    if (!genQuiets || SquareIsInCheckRay(targetSquare))
                    {
                        continue;
                    }
                }

                // Condition: safe for king to move to this square
                if (!SquareIsAttacked(targetSquare))
                {
                    moves.Add(new Move(friendlyKingSquare, targetSquare));

                    // Castling:
                    if (!inCheck && !isCapture)
                    {
                        // Castle kingside
                        if ((targetSquare == f1 || targetSquare == f8) && HasKingsideCastleRight)
                        {
                            int castleKingsideSquare = targetSquare + 1;
                            if (board.square[castleKingsideSquare] == Piece.NONE)
                            {
                                if (!SquareIsAttacked(castleKingsideSquare))
                                {
                                    moves.Add(new Move(friendlyKingSquare, castleKingsideSquare, Move.Flag.CASTLING));
                                }
                            }
                        }

                        // Castle queenside
                        else if ((targetSquare == d1 || targetSquare == d8) && HasQueensideCastleRight)
                        {
                            int castleQueensideSquare = targetSquare - 1;
                            if (board.square[castleQueensideSquare] == Piece.NONE && board.square[castleQueensideSquare - 1] == Piece.NONE)
                            {
                                if (!SquareIsAttacked(castleQueensideSquare))
                                {
                                    moves.Add(new Move(friendlyKingSquare, castleQueensideSquare, Move.Flag.CASTLING));
                                }
                            }
                        }
                    }
                }
            }
        }

        /*
         * Generating sliding moves
         * Input : < None >
         * Output: < None >
         */
        void GenerateSlidingMoves()
        {
            // Generating rook moves:
            PieceList rooks = board.rooks[friendlyColourIndex];
            for (int i = 0; i < rooks.Count; i++)
            {
                GenerateSlidingPieceMoves(rooks[i], 0, 4);
            }

            // Generating bishop moves:
            PieceList bishops = board.bishops[friendlyColourIndex];
            for (int i = 0; i < bishops.Count; i++)
            {
                GenerateSlidingPieceMoves(bishops[i], 4, 8);
            }

            // Generating queen moves:
            PieceList queens = board.queens[friendlyColourIndex];
            for (int i = 0; i < queens.Count; i++)
            {
                GenerateSlidingPieceMoves(queens[i], 0, 8);
            }
        }

        /*
         * Generating sliding moves
         * Input : startSquare   - the source square
         *         startDirIndex - the starting direction
         *         endDirIndex   - the ending direction
         * Output: < None >
         */
        void GenerateSlidingPieceMoves(int startSquare, int startDirIndex, int endDirIndex)
        {
            // Inits:
            bool isPinned = IsPinned(startSquare);

            // Condition: if this piece is pinned, and the king is in check, this piece cannot move
            if (inCheck && isPinned)
            {
                return;
            }

            for (int directionIndex = startDirIndex; directionIndex < endDirIndex; directionIndex++)
            {
                int currentDirOffset = directionOffsets[directionIndex];

                // Condition: if pinned, this piece can only move along the ray towards/away from the friendly king, so skip other directions:
                if (isPinned && !IsMovingAlongRay(currentDirOffset, friendlyKingSquare, startSquare))
                {
                    continue;
                }

                for (int n = 0; n < numSquaresToEdge[startSquare][directionIndex]; n++)
                {
                    int targetSquare = startSquare + currentDirOffset * (n + 1);
                    int targetSquarePiece = board.square[targetSquare];

                    // Condition: blocked by friendly piece, so stop looking in this direction
                    if (Piece.IsColour(targetSquarePiece, friendlyColour))
                    {
                        break;
                    }

                    bool isCapture = targetSquarePiece != Piece.NONE;
                    bool movePreventsCheck = SquareIsInCheckRay(targetSquare);

                    if (movePreventsCheck || !inCheck)
                    {
                        if (genQuiets || isCapture)
                        {
                            moves.Add(new Move(startSquare, targetSquare));
                        }
                    }

                    // Condition: if square not empty, can't move any further in this direction
                    //            also, if this move blocked a check, further moves won't block the check
                    if (isCapture || movePreventsCheck)
                    {
                        break;
                    }
                }
            }
        }

        /*
         * Generating king moves
         * Input : < None >
         * Output: < None >
         */
        void GenerateKnightMoves()
        {
            // Inits:
            PieceList myKnights = board.knights[friendlyColourIndex];

            for (int i = 0; i < myKnights.Count; i++)
            {
                int startSquare = myKnights[i];

                // Condition: knight cannot move if it is pinned:
                if (IsPinned(startSquare))
                {
                    continue;
                }

                for (int knightMoveIndex = 0; knightMoveIndex < knightMoves[startSquare].Length; knightMoveIndex++)
                {
                    int targetSquare = knightMoves[startSquare][knightMoveIndex];
                    int targetSquarePiece = board.square[targetSquare];
                    bool isCapture = Piece.IsColour(targetSquarePiece, opponentColour);
                    if (genQuiets || isCapture)
                    {
                        // Condition: skip if square contains friendly piece, or if in check and knight is not interposing/capturing checking piece
                        if (Piece.IsColour(targetSquarePiece, friendlyColour) || (inCheck && !SquareIsInCheckRay(targetSquare)))
                        {
                            continue;
                        }
                        moves.Add(new Move(startSquare, targetSquare));
                    }
                }
            }
        }

        /*
         * Generating pawn moves
         * Input : < None >
         * Output: < None >
         */
        void GeneratePawnMoves()
        {
            // Inits:
            PieceList myPawns = board.pawns[friendlyColourIndex];
            int pawnOffset = (friendlyColour == Piece.WHITE) ? 8 : -8;
            int startRank = (board.whiteToMove) ? 1 : 6;
            int finalRankBeforePromotion = (board.whiteToMove) ? 6 : 1;
            int enPassantFile = ((int)(board.currentGameState >> 4) & 15) - 1;
            int enPassantSquare = -1;

            if (enPassantFile != -1)
            {
                enPassantSquare = 8 * ((board.whiteToMove) ? 5 : 2) + enPassantFile;
            }

            for (int i = 0; i < myPawns.Count; i++)
            {
                int startSquare = myPawns[i];
                int rank = RankIndex(startSquare);
                bool oneStepFromPromotion = rank == finalRankBeforePromotion;

                if (genQuiets)
                {

                    int squareOneForward = startSquare + pawnOffset;

                    // Condition: square ahead of pawn is empty, forward moves
                    if (board.square[squareOneForward] == Piece.NONE)
                    {
                        // Condition: pawn not pinned, or is moving along line of pin
                        if (!IsPinned(startSquare) || IsMovingAlongRay(pawnOffset, startSquare, friendlyKingSquare))
                        {
                            // Condition: not in check, or pawn is interposing checking piece
                            if (!inCheck || SquareIsInCheckRay(squareOneForward))
                            {
                                if (oneStepFromPromotion)
                                {
                                    MakePromotionMoves(startSquare, squareOneForward);
                                }

                                else
                                {
                                    moves.Add(new Move(startSquare, squareOneForward));
                                }
                            }

                            // Condition: is on starting square (so can move two forward if not blocked)
                            if (rank == startRank)
                            {
                                int squareTwoForward = squareOneForward + pawnOffset;
                                if (board.square[squareTwoForward] == Piece.NONE)
                                {
                                    // Condition: not in check, or pawn is interposing checking piece
                                    if (!inCheck || SquareIsInCheckRay(squareTwoForward))
                                    {
                                        moves.Add(new Move(startSquare, squareTwoForward, Move.Flag.PAWN_TWO_FORWARD));
                                    }
                                }
                            }
                        }
                    }
                }

                // Pawn Captures:
                for (int j = 0; j < 2; j++)
                {
                    // Condition: check if square exists diagonal to pawn
                    if (numSquaresToEdge[startSquare][pawnAttackDirections[friendlyColourIndex][j]] > 0)
                    {
                        // Move in direction friendly pawns attack to get square from which enemy pawn would attack:
                        int pawnCaptureDir = directionOffsets[pawnAttackDirections[friendlyColourIndex][j]];
                        int targetSquare = startSquare + pawnCaptureDir;
                        int targetPiece = board.square[targetSquare];

                        // Condition: if piece is pinned, and the square it wants to move to is not on same line as the pin, then skip this direction
                        if (IsPinned(startSquare) && !IsMovingAlongRay(pawnCaptureDir, friendlyKingSquare, startSquare))
                        {
                            continue;
                        }

                        // Condition: regular capture
                        if (Piece.IsColour(targetPiece, opponentColour))
                        {
                            // Condition: if in check, and piece is not capturing/interposing the checking piece, then skip to next square
                            if (inCheck && !SquareIsInCheckRay(targetSquare))
                            {
                                continue;
                            }

                            if (oneStepFromPromotion)
                            {
                                MakePromotionMoves(startSquare, targetSquare);
                            }

                            else
                            {
                                moves.Add(new Move(startSquare, targetSquare));
                            }
                        }

                        // Condition: capture en-passant
                        if (targetSquare == enPassantSquare)
                        {
                            int epCapturedPawnSquare = targetSquare + ((board.whiteToMove) ? -8 : 8);

                            if (!InCheckAfterEnPassant(startSquare, targetSquare, epCapturedPawnSquare))
                            {
                                moves.Add(new Move(startSquare, targetSquare, Move.Flag.EN_PASSANT_CAPTURE));
                            }
                        }
                    }
                }
            }
        }

        /*
         * Generating promotion moves
         * Input : < None >
         * Output: < None >
         */
        void MakePromotionMoves(int fromSquare, int toSquare)
        {
            // Adding promotion moves:
            moves.Add(new Move(fromSquare, toSquare, Move.Flag.PROMOTE_TO_QUEEN));

            if (promotionsToGenerate == PromotionMode.All)
            {
                moves.Add(new Move(fromSquare, toSquare, Move.Flag.PROMOTE_TO_KNIGHT));
                moves.Add(new Move(fromSquare, toSquare, Move.Flag.PROMOTE_TO_ROOK));
                moves.Add(new Move(fromSquare, toSquare, Move.Flag.PROMOTE_TO_BISHOP));
            }

            else if (promotionsToGenerate == PromotionMode.QueenAndKnight)
            {
                moves.Add(new Move(fromSquare, toSquare, Move.Flag.PROMOTE_TO_KNIGHT));
            }

        }

        /*
         * Checking whether piece is moving along a ray
         * Input : rayDir       - the ray direction
         *         startSquare  - the start square
         *         targetSquare - the target square
         * Output: true / false
         */
        bool IsMovingAlongRay(int rayDir, int startSquare, int targetSquare)
        {
            int moveDir = directionLookup[targetSquare - startSquare + 63];
            return (rayDir == moveDir || -rayDir == moveDir);
        }

        /*
         * Checking whether piece is pinned
         * Input : square - the square
         * Output: true / false
         */
        bool IsPinned(int square)
        {
            return pinsExistInPosition && ((pinRayBitmask >> square) & 1) != 0;
        }

        /*
         * Checking whether square is in check ray
         * Input : square - the square
         * Output: true / false
         */
        bool SquareIsInCheckRay(int square)
        {
            return inCheck && ((checkRayBitmask >> square) & 1) != 0;
        }

        bool HasKingsideCastleRight
        {
            get
            {
                int mask = (board.whiteToMove) ? 1 : 4;
                return (board.currentGameState & mask) != 0;
            }
        }

        bool HasQueensideCastleRight
        {
            get
            {
                int mask = (board.whiteToMove) ? 2 : 8;
                return (board.currentGameState & mask) != 0;
            }
        }

        /*
         * Generating sliding attack map
         * Input : < None >
         * Output: < None >
         */
        void GenSlidingAttackMap()
        {
            // Inits:
            opponentSlidingAttackMap = 0;

            // Enemy rooks:
            PieceList enemyRooks = board.rooks[opponentColourIndex];
            for (int i = 0; i < enemyRooks.Count; i++)
            {
                UpdateSlidingAttackPiece(enemyRooks[i], 0, 4);
            }

            // Enemy queens:
            PieceList enemyQueens = board.queens[opponentColourIndex];
            for (int i = 0; i < enemyQueens.Count; i++)
            {
                UpdateSlidingAttackPiece(enemyQueens[i], 0, 8);
            }

            // Enemy bishop:
            PieceList enemyBishops = board.bishops[opponentColourIndex];
            for (int i = 0; i < enemyBishops.Count; i++)
            {
                UpdateSlidingAttackPiece(enemyBishops[i], 4, 8);
            }
        }

        /*
         * Updating sliding attack piece
         * Input : startSquare   - the start square
         *         startDirIndex - the start direction
         *         endDirIndex   - the end direction
         * Output: < None >
         */
        void UpdateSlidingAttackPiece(int startSquare, int startDirIndex, int endDirIndex)
        {

            for (int directionIndex = startDirIndex; directionIndex < endDirIndex; directionIndex++)
            {
                int currentDirOffset = directionOffsets[directionIndex];

                for (int n = 0; n < numSquaresToEdge[startSquare][directionIndex]; n++)
                {
                    // Inits:
                    int targetSquare = startSquare + currentDirOffset * (n + 1);
                    int targetSquarePiece = board.square[targetSquare];
                    opponentSlidingAttackMap |= 1ul << targetSquare;

                    if (targetSquare != friendlyKingSquare)
                    {
                        if (targetSquarePiece != Piece.NONE)
                        {
                            break;
                        }
                    }
                }
            }
        }

        void CalculateAttackData()
        {
            // Generating the sliding attack map:
            GenSlidingAttackMap();

            // Search squares in all directions around friendly king for checks/pins by enemy sliding pieces (queen, rook, bishop):
            int startDirIndex = 0;
            int endDirIndex = 8;

            if (board.queens[opponentColourIndex].Count == 0)
            {
                startDirIndex = (board.rooks[opponentColourIndex].Count > 0) ? 0 : 4;
                endDirIndex = (board.bishops[opponentColourIndex].Count > 0) ? 8 : 4;
            }

            for (int dir = startDirIndex; dir < endDirIndex; dir++)
            {
                bool isDiagonal = dir > 3;

                int n = numSquaresToEdge[friendlyKingSquare][dir];
                int directionOffset = directionOffsets[dir];
                bool isFriendlyPieceAlongRay = false;
                ulong rayMask = 0;

                for (int i = 0; i < n; i++)
                {
                    int squareIndex = friendlyKingSquare + directionOffset * (i + 1);
                    rayMask |= 1ul << squareIndex;
                    int piece = board.square[squareIndex];

                    // Condition: this square contains a piece
                    if (piece != Piece.NONE)
                    {
                        if (Piece.IsColour(piece, friendlyColour))
                        {
                            // Condition: first friendly piece we have come across in this direction, so it might be pinned
                            if (!isFriendlyPieceAlongRay)
                            {
                                isFriendlyPieceAlongRay = true;
                            }

                            // Condition: this is the second friendly piece we've found in this direction, therefore pin is not possible
                            else
                            {
                                break;
                            }
                        }

                        // Condition: this square contains an enemy piece
                        else
                        {
                            int pieceType = Piece.PieceType(piece);

                            // Condition: check if piece is in bitmask of pieces able to move in current direction
                            if (isDiagonal && Piece.IsBishopOrQueen(pieceType) || !isDiagonal && Piece.IsRookOrQueen(pieceType))
                            {
                                // Condition: friendly piece blocks the check, so this is a pin
                                if (isFriendlyPieceAlongRay)
                                {
                                    pinsExistInPosition = true;
                                    pinRayBitmask |= rayMask;
                                }

                                // Condition: no friendly piece blocking the attack, so this is a check
                                else
                                {
                                    checkRayBitmask |= rayMask;

                                    // If already in check, then this is double check:
                                    inDoubleCheck = inCheck;
                                    inCheck = true;
                                }
                                break;
                            }

                            else
                            {
                                // This enemy piece is not able to move in the current direction, and so is blocking any checks/pins:
                                break;
                            }
                        }
                    }
                }

                // Condition: stop searching for pins if in double check, as the king is the only piece able to move in that case anyway:
                if (inDoubleCheck)
                {
                    break;
                }

            }

            // Knight attacks:
            PieceList opponentKnights = board.knights[opponentColourIndex];
            opponentKnightAttacks = 0;
            bool isKnightCheck = false;

            for (int knightIndex = 0; knightIndex < opponentKnights.Count; knightIndex++)
            {
                int startSquare = opponentKnights[knightIndex];
                opponentKnightAttacks |= knightAttackBitboards[startSquare];

                if (!isKnightCheck && BitBoardUtility.ContainsSquare(opponentKnightAttacks, friendlyKingSquare))
                {
                    isKnightCheck = true;

                    // If already in check, then this is double check:
                    inDoubleCheck = inCheck; 
                    inCheck = true;
                    checkRayBitmask |= 1ul << startSquare;
                }
            }

            // Pawn attacks:
            PieceList opponentPawns = board.pawns[opponentColourIndex];
            opponentPawnAttackMap = 0;
            bool isPawnCheck = false;

            for (int pawnIndex = 0; pawnIndex < opponentPawns.Count; pawnIndex++)
            {
                int pawnSquare = opponentPawns[pawnIndex];
                ulong pawnAttacks = pawnAttackBitboards[pawnSquare][opponentColourIndex];
                opponentPawnAttackMap |= pawnAttacks;

                if (!isPawnCheck && BitBoardUtility.ContainsSquare(pawnAttacks, friendlyKingSquare))
                {
                    isPawnCheck = true;

                    // If already in check, then this is double check:
                    inDoubleCheck = inCheck; 
                    inCheck = true;
                    checkRayBitmask |= 1ul << pawnSquare;
                }
            }

            int enemyKingSquare = board.kingSquare[opponentColourIndex];

            opponentAttackMapNoPawns = opponentSlidingAttackMap | opponentKnightAttacks | kingAttackBitboards[enemyKingSquare];
            opponentAttackMap = opponentAttackMapNoPawns | opponentPawnAttackMap;
        }

        /*
         * Checking whether square is attacked
         * Input : square - the square
         * Output: true / false
         */
        bool SquareIsAttacked(int square)
        {
            return BitBoardUtility.ContainsSquare(opponentAttackMap, square);
        }

        /*
         * Checking whether after en-passant check occures
         * Input : startSquare          - the source square
         *         targetSquare         - the target square
         *         epCapturedPawnSquare - the captured square
         * Output: true / false
         */
        bool InCheckAfterEnPassant(int startSquare, int targetSquare, int epCapturedPawnSquare)
        {
            // Update board to reflect en-passant capture:
            board.square[targetSquare] = board.square[startSquare];
            board.square[startSquare] = Piece.NONE;
            board.square[epCapturedPawnSquare] = Piece.NONE;

            bool inCheckAfterEpCapture = false;
            if (SquareAttackedAfterEPCapture(epCapturedPawnSquare, startSquare))
            {
                inCheckAfterEpCapture = true;
            }

            // Undo change to board:
            board.square[targetSquare] = Piece.NONE;
            board.square[startSquare] = Piece.PAWN | friendlyColour;
            board.square[epCapturedPawnSquare] = Piece.PAWN | opponentColour;
            return inCheckAfterEpCapture;
        }

        /*
         * Checking if a square is attacked after capturing
         * with en-passant
         * Input : epCaptureSquare          - the capture square
         *         capturingPawnStartSquare - the start square
         * Output: true / false
         */
        bool SquareAttackedAfterEPCapture(int epCaptureSquare, int capturingPawnStartSquare)
        {
            if (BitBoardUtility.ContainsSquare(opponentAttackMapNoPawns, friendlyKingSquare))
            {
                return true;
            }

            // Loop through the horizontal direction towards ep capture to see if any enemy piece now attacks king:
            int dirIndex = (epCaptureSquare < friendlyKingSquare) ? 2 : 3;
            for (int i = 0; i < numSquaresToEdge[friendlyKingSquare][dirIndex]; i++)
            {
                int squareIndex = friendlyKingSquare + directionOffsets[dirIndex] * (i + 1);
                int piece = board.square[squareIndex];
                if (piece != Piece.NONE)
                {
                    // Condition: friendly piece is blocking view of this square from the enemy.
                    if (Piece.IsColour(piece, friendlyColour))
                    {
                        break;
                    }

                    // Condition: this square contains an enemy piece
                    else
                    {
                        if (Piece.IsRookOrQueen(piece))
                        {
                            return true;
                        }

                        else
                        {
                            // This piece is not able to move in the current direction, and is therefore blocking any checks along this line:
                            break;
                        }
                    }
                }
            }

            // Check if enemy pawn is controlling this square (can't use pawn attack bitboard, because pawn has been captured):
            for (int i = 0; i < 2; i++)
            {
                // Condition: check if square exists diagonal to friendly king from which enemy pawn could be attacking it
                if (numSquaresToEdge[friendlyKingSquare][pawnAttackDirections[friendlyColourIndex][i]] > 0)
                {
                    // Move in direction friendly pawns attack to get square from which enemy pawn would attack:
                    int piece = board.square[friendlyKingSquare + directionOffsets[pawnAttackDirections[friendlyColourIndex][i]]];
                    
                    // Condition: enemy pawn
                    if (piece == (Piece.PAWN | opponentColour))
                    {
                        return true;
                    }
                }
            }

            return false;
        }
    }

}