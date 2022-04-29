using UnityEngine;

namespace Chess.Game
{
    public class PieceTheme : ScriptableObject
    {
        // Inputs:
        public PieceSprites whitePieces;
        public PieceSprites blackPieces;

        /*
         * Getting piece sprite
         * Input : piece - the piece to get
         * Output: the piece sprite
         */
        public Sprite GetPieceSprite(int piece)
        {
            // Getting the correct piece colors:
            PieceSprites pieceSprites = Piece.IsColour(piece, Piece.WHITE) ? whitePieces : blackPieces;

            // Getting the correct piece sprite:
            switch (Piece.PieceType(piece))
            {
                case Piece.PAWN:   return pieceSprites.pawn;
                case Piece.ROOK:   return pieceSprites.rook;
                case Piece.KNIGHT: return pieceSprites.knight;
                case Piece.BISHOP: return pieceSprites.bishop;
                case Piece.QUEEN:  return pieceSprites.queen;
                case Piece.KING:   return pieceSprites.king;
                default:           return null;
            }
        }

        // PieceSprites Class:
        [System.Serializable]
        public class PieceSprites
        {
            // Inputs:
            public Sprite pawn, rook, knight, bishop, queen, king;

            /*
             * Getting the class's fields as an array
             * Input : < None >
             * Output: < None >
             */
            public Sprite this[int i]
            {
                get
                {
                    return new Sprite[] { pawn, rook, knight, bishop, queen, king }[i];
                }
            }
        }
    }
}