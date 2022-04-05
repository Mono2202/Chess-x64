using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Data : MonoBehaviour
{
    // Fields:
    public static Data instance;
    public Communicator communicator;
    public string username;

    // Constants:
    public const int LOGIN_SCENE_INDEX = 1;

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
