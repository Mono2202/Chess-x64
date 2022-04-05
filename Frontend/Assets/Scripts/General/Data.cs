using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Data : MonoBehaviour
{
    // Fields:
    public static Data instance;
    public Communicator communicator;
    public string username;
    public bool isWhite = true;

    // Constants:
    public const int LOGIN_SCENE_INDEX = 1;
    public const int REGISTER_SCENE_COUNT = 2;
    public const int MENU_SCENE_COUNT = 3;
    public const int SEARCHING_SCENE_COUNT = 4;
    public const int GAME_SCENE_COUNT = 5;

    public const float DELAY_TIME = 0.5f;

    public const int BOARD_SIZE = 8;
    public static char[,] INITIAL_BOARD =
    {
        { 'r', 'n', 'b', 'q', 'k', 'b', 'n', 'r' },
        { 'p', 'p', 'p', 'p', 'p', 'p', 'p', 'p' },
        { '#', '#', '#', '#', '#', '#', '#', '#' },
        { '#', '#', '#', '#', '#', '#', '#', '#' },
        { '#', '#', '#', '#', '#', '#', '#', '#' },
        { '#', '#', '#', '#', '#', '#', '#', '#' },
        { 'P', 'P', 'P', 'P', 'P', 'P', 'P', 'P' },
        { 'R', 'N', 'B', 'Q', 'K', 'B', 'N', 'R' }
    };

    public const int W_PAWN_SPRITE_INDEX = 0;
    public const int W_KNIGHT_SPRITE_INDEX = 1;
    public const int W_BISHOP_SPRITE_INDEX = 2;
    public const int W_ROOK_SPRITE_INDEX = 3;
    public const int W_QUEEN_SPRITE_INDEX = 4;
    public const int W_KING_SPRITE_INDEX = 5;
    public const int B_PAWN_SPRITE_INDEX = 6;
    public const int B_KNIGHT_SPRITE_INDEX = 7;
    public const int B_BISHOP_SPRITE_INDEX = 8;
    public const int B_ROOK_SPRITE_INDEX = 9;
    public const int B_QUEEN_SPRITE_INDEX = 10;
    public const int B_KING_SPRITE_INDEX = 11;

    public Dictionary<char, int> charToSpriteIndex = new Dictionary<char, int>
    {
        { 'P', W_PAWN_SPRITE_INDEX },
        { 'N', W_KNIGHT_SPRITE_INDEX },
        { 'B', W_BISHOP_SPRITE_INDEX },
        { 'R', W_ROOK_SPRITE_INDEX },
        { 'Q', W_QUEEN_SPRITE_INDEX },
        { 'K', W_KING_SPRITE_INDEX },
        { 'p', B_PAWN_SPRITE_INDEX },
        { 'n', B_KNIGHT_SPRITE_INDEX },
        { 'b', B_BISHOP_SPRITE_INDEX },
        { 'r', B_ROOK_SPRITE_INDEX },
        { 'q', B_QUEEN_SPRITE_INDEX },
        { 'k', B_KING_SPRITE_INDEX }
    };

    // Methods:
    private void Awake()
    {
        // Creating the instance:
        instance = this;

        // Creating the communicator object:
        instance.communicator = new Communicator();

        // Keeping the prefab loaded at all times:
        DontDestroyOnLoad(gameObject);

        // Switching to the login scene:
        SceneManager.LoadScene(LOGIN_SCENE_INDEX);
    }
}
