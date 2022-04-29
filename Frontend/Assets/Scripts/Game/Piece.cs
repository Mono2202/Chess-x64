namespace Chess
{
    public static class Piece
    {
        // Constants:
        public const int NONE = 0;
        public const int KING = 1;
        public const int PAWN = 2;
        public const int KNIGHT = 3;
        public const int BISHOP = 5;
        public const int ROOK = 6;
        public const int QUEEN = 7;

        public const int WHITE = 8;
        public const int BLACK = 16;

        const int TYPE_MASK = 0b00111;
        const int BLACK_MASK = 0b10000;
        const int WHITE_MASK = 0b01000;
        const int COLOR_MASK = WHITE_MASK | BLACK_MASK;

        public static bool IsColour(int piece, int colour)
        {
            return (piece & COLOR_MASK) == colour;
        }

        public static int Colour(int piece)
        {
            return piece & COLOR_MASK;
        }

        public static int PieceType(int piece)
        {
            return piece & TYPE_MASK;
        }

        public static bool IsRookOrQueen(int piece)
        {
            return (piece & 0b110) == 0b110;
        }

        public static bool IsBishopOrQueen(int piece)
        {
            return (piece & 0b101) == 0b101;
        }

        public static bool IsSlidingPiece(int piece)
        {
            return (piece & 0b100) != 0;
        }
    }
}