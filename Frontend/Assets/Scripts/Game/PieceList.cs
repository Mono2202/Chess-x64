public class PieceList
{
    // Fields:
    public int[] occupiedSquares;
    int[] map;
    int numPieces;

    /*
     * PieceList C'tor
     * Input : maxPieceCount - the maximum piece count
     * Output: < None >
     */
    public PieceList(int maxPieceCount = 16)
    {
        occupiedSquares = new int[maxPieceCount];
        map = new int[64];
        numPieces = 0;
    }

    /*
     * Getting the amount of pieces
     * Input : < None >
     * Output: numPieces - the amount of pieces
     */
    public int Count
    {
        get
        {
            return numPieces;
        }
    }

    /*
     * Adding a piece to a specific square
     * Input : square - the square
     * Output: < None >
     */
    public void AddPieceAtSquare(int square)
    {
        occupiedSquares[numPieces] = square;
        map[square] = numPieces;
        numPieces++;
    }

    /*
     * Removing a piece from a specific square
     * Input : square - the square
     * Output: < None >
     */
    public void RemovePieceAtSquare(int square)
    {
        // Get the index of this element in the occupiedSquares array:
        int pieceIndex = map[square];

        // Move last element in array to the place of the removed element:
        occupiedSquares[pieceIndex] = occupiedSquares[numPieces - 1];

        // Update map to point to the moved element's new location in the array:
        map[occupiedSquares[pieceIndex]] = pieceIndex;

        // Lowering the counter:
        numPieces--;
    }

    /*
     * Moving a piece
     * Input : startSquare  - the source square
     *         targetSquare - the destination square
     * Output: < None >
     */
    public void MovePiece(int startSquare, int targetSquare)
    {
        // Get the index of this element in the occupiedSquares array:
        int pieceIndex = map[startSquare]; // TODO: map[startSquare] = 0?

        // Switching location:
        occupiedSquares[pieceIndex] = targetSquare;
        map[targetSquare] = pieceIndex;
    }

    public int this[int index] => occupiedSquares[index];

}