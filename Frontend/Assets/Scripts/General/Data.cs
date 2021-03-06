using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Data : MonoBehaviour
{
    // Fields:
    public static Data instance;
    public Communicator communicator;
    public Communicator listener;
    public string username;
    public string profileUsername;
    public bool isWhite = true;
    public Color32 whiteSquareColor = new Color32(255, 255, 255, 255);
    public Color32 blackSquareColor = new Color32(65, 65, 65, 255);

    // Constants:
    public const int LOGIN_SCENE_COUNT = 1;
    public const int REGISTER_SCENE_COUNT = 2;
    public const int HOME_SCENE_COUNT = 3;
    public const int PLAY_SCENE_COUNT = 4;
    public const int PRIVATE_ROOM_SCENE_COUNT = 5;
    public const int SEARCHING_FOR_GAME_SCENE_COUNT = 6;
    public const int GAME_SCENE_COUNT = 7;
    public const int BOARD_EDITOR_SCENE_COUNT = 8;
    public const int PROFILE_SCENE_COUNT = 9;
    public const int MATCH_HISTORY_SCENE_COUNT = 10;
    public const int SEARCH_PLAYER_SCENE_COUNT = 11;

    public const float DELAY_TIME = 0.5f;

    private const int IP_INDEX = 0;
    private const int PORT_INDEX = 1;
    private const int LISTENER_PORT_INDEX = 2;

    
    /*
     * Runs when object is created
     */
    private void Awake()
    {
        // Inits:
        string ip = "";
        int port = 0;
        int listenerPort = 0;

        // Getting the server settings:
        ReadConfig(ref ip, ref port, ref listenerPort);

        // Creating the instance:
        instance = this;

        // Creating the communicator object:
        instance.communicator = new Communicator(ip, port);
        instance.listener = new Communicator(ip, listenerPort);

        // Keeping the prefab loaded at all times:
        DontDestroyOnLoad(gameObject);

        // Switching to the login scene:
        SceneManager.LoadScene(LOGIN_SCENE_COUNT);
    }

    /*
     * Runs when program quits
     */
    void OnApplicationQuit()
    {
        // Sending exit message to server:
        instance.communicator.Write("EXIT");
    }

    /*
     * Reading the config file
     * Input : ip           - the refrence to the ip string
     *         port         - the refrence to the port string
     *         listenerPort - the refrence to the listenerPort string
     * Output: < None >
     */
    private void ReadConfig(ref string ip, ref int port, ref int listenerPort)
    {
        // Inits:
        string configFilePath = Application.streamingAssetsPath + "/config.txt";
        string[] configFileLines = File.ReadAllLines(configFilePath);

        // Setting the fields:
        ip = configFileLines[IP_INDEX].Substring(4);
        port = int.Parse(configFileLines[PORT_INDEX].Substring(6));
        listenerPort = int.Parse(configFileLines[LISTENER_PORT_INDEX].Substring(14));
    }
}
