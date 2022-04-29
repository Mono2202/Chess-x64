using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Chess
{
    public static class BitBoardUtility
    {
        /*
         * Checks whether the bit board contains a square
         * Input : bitboard - the bitboard
         *         square   - the square
         * Output: true     - square in bitboard
         *         false    - otherwise
         */
        public static bool ContainsSquare(ulong bitboard, int square)
        {
            return ((bitboard >> square) & 1) != 0;
        }
    }
}