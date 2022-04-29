using UnityEngine;

namespace Chess.Game
{
    public class BoardTheme : ScriptableObject
    {
        // Inputs:
        public SquareColours lightSquares;
        public SquareColours darkSquares;

        // Structs:
        [System.Serializable]
        public struct SquareColours
        {
            public Color normal;
            public Color legal;
            public Color selected;
            public Color moveFromHighlight;
            public Color moveToHighlight;
        }
    }
}