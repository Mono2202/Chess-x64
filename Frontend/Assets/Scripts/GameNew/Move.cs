namespace Chess
{
    /* 
    Bits 0-5  : from square (0 to 63)
    Bits 6-11 : to square (0 to 63)
    Bits 12-15: flag
    */
    public readonly struct Move
    {
        // Fields:
        readonly ushort moveValue;

        // Structs:
        public readonly struct Flag
        {
            public const int NONE = 0;
            public const int EN_PASSANT_CAPTURE = 1;
            public const int CASTLING = 2;
            public const int PROMOTE_TO_QUEEN = 3;
            public const int PROMOTE_TO_KNIGHT = 4;
            public const int PROMOTE_TO_ROOK = 5;
            public const int PROMOTE_TO_BISHOP = 6;
            public const int PAWN_TWO_FORWARD = 7;
        }

        // Constants:
        const ushort START_SQUARE_MASK = 0b0000000000111111;
        const ushort TARGET_SQUARE_MASK = 0b0000111111000000;
        const ushort FLAG_MASK = 0b1111000000000000;

        /*
         * Move C'tor
         * Input : moveValue - the move value
         * Output: < None >
         */
        public Move(ushort moveValue)
        {
            this.moveValue = moveValue;
        }

        /*
         * Move C'tor
         * Input : startSquare  - the source square
         *         targetSquare - the destination square
         * Output: < None >
         */
        public Move(int startSquare, int targetSquare)
        {
            moveValue = (ushort)(startSquare | targetSquare << 6);
        }

        /*
         * Move C'tor
         * Input : startSquare  - the source square
         *         targetSquare - the destination square
         *         flag         - a flag
         * Output: < None >
         */
        public Move(int startSquare, int targetSquare, int flag)
        {
            moveValue = (ushort)(startSquare | targetSquare << 6 | flag << 12);
        }

        public int StartSquare
        {
            get
            {
                return moveValue & START_SQUARE_MASK;
            }
        }

        public int TargetSquare
        {
            get
            {
                return (moveValue & TARGET_SQUARE_MASK) >> 6;
            }
        }

        public bool IsPromotion
        {
            get
            {
                int flag = MoveFlag;
                return flag == Flag.PROMOTE_TO_QUEEN || flag == Flag.PROMOTE_TO_ROOK || flag == Flag.PROMOTE_TO_KNIGHT || flag == Flag.PROMOTE_TO_BISHOP;
            }
        }

        public int MoveFlag
        {
            get
            {
                return moveValue >> 12;
            }
        }

        public int PromotionPieceType
        {
            get
            {
                switch (MoveFlag)
                {
                    case Flag.PROMOTE_TO_ROOK:
                        return Piece.ROOK;
                    case Flag.PROMOTE_TO_KNIGHT:
                        return Piece.KNIGHT;
                    case Flag.PROMOTE_TO_BISHOP:
                        return Piece.BISHOP;
                    case Flag.PROMOTE_TO_QUEEN:
                        return Piece.QUEEN;
                    default:
                        return Piece.NONE;
                }
            }
        }

        public static Move InvalidMove
        {
            get
            {
                return new Move(0);
            }
        }

        public static bool SameMove(Move a, Move b)
        {
            return a.moveValue == b.moveValue;
        }

        public ushort Value
        {
            get
            {
                return moveValue;
            }
        }

        public bool IsInvalid
        {
            get
            {
                return moveValue == 0;
            }
        }

        public string Name
        {
            get
            {
                return BoardRepresentation.SquareNameFromIndex(StartSquare) + "-" + BoardRepresentation.SquareNameFromIndex(TargetSquare);
            }
        }
    }
}
