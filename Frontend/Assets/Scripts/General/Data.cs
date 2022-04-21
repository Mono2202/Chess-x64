using System.Collections.Generic;
using System.Runtime.InteropServices;
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
    public const int STATS_SCENE_COUNT = 6;
    public const int BOARD_EDITOR_SCENE_COUNT = 7;
    public const int LOADING_SCENE_COUNT = 8;

    public const float DELAY_TIME = 0.5f;

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
        SceneManager.LoadScene(LOADING_SCENE_COUNT);
    }

    void OnApplicationQuit()
    {
        instance.communicator.Write("EXIT");
    }
}
