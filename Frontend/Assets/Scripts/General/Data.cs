using System;
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
    public Color32 whiteSquareColor = new Color32(255, 255, 255, 255);
    public Color32 blackSquareColor = new Color32(65, 65, 65, 255);

    // Constants:
    public const int LOGIN_SCENE_INDEX = 1;
    public const int REGISTER_SCENE_COUNT = 2;
    public const int MENU_SCENE_COUNT = 3;
    public const int SEARCHING_SCENE_COUNT = 4;
    public const int GAME_SCENE_COUNT = 5;
    public const int STATS_SCENE_COUNT = 6;
    public const int BOARD_EDITOR_SCENE_COUNT = 7;
    public const int LOADING_SCENE_COUNT = 8;
    public const int PLAY_SCENE_COUNT = 9;
    public const int PRIVATE_SEARCHING_SCENE_COUNT = 10;

    public const float DELAY_TIME = 0.5f;

    // Methods:
    private void Awake()
    {
        // Creating the instance:
        instance = this;

        // Creating the communicator object:
        instance.communicator = new Communicator();

        // Condition: not first session
        if (false) // TODO: CHANGE TO DB SETTINGS
        {
            LoadUserPrefrences();
        }

        // Condition: first session
        else
        {
            ResetSquareColors();
        }

        // Keeping the prefab loaded at all times:
        DontDestroyOnLoad(gameObject);

        // Switching to the login scene:
        SceneManager.LoadScene(LOGIN_SCENE_INDEX);
    }

    void OnApplicationQuit()
    {
        instance.communicator.Write("EXIT");
    }

    public void LoadUserPrefrences()
    {
    }

    public void ResetSquareColors()
    {
        // Setting the square colors:
        instance.whiteSquareColor = new Color32(255, 255, 255, 255);
        instance.blackSquareColor = new Color32(65, 65, 65, 255);
    }
}
