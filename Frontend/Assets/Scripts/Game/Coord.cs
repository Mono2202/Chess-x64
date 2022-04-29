using System;
namespace Chess
{
    public struct Coord : IComparable<Coord>
    {
        // Fields:
        public readonly int fileIndex;
        public readonly int rankIndex;

        /*
         * C'tor
         */
        public Coord(int fileIndex, int rankIndex)
        {
            this.fileIndex = fileIndex;
            this.rankIndex = rankIndex;
        }

        /*
         * Checking what is the color of the current square
         * Input : < None >
         * Output: true  - light square
         *         false - dark square
         */
        public bool IsLightSquare()
        {
            return (fileIndex + rankIndex) % 2 != 0;
        }

        /*
         * Comparing 2 coords
         * Input : other - the other coord
         * Output: 0     - same coord
         *         1     - different coord
         */
        public int CompareTo(Coord other)
        {
            return (fileIndex == other.fileIndex && rankIndex == other.rankIndex) ? 0 : 1;
        }
    }
}