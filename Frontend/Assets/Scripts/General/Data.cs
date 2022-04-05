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
    public const int REGISTER_SCENE_COUNT = 2;
    public const int MENU_SCENE_COUNT = 3;
    public const int SEARCHING_SCENE_COUNT = 4;
    public const int GAME_SCENE_COUNT = 5;

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
        SceneManager.LoadScene(LOGIN_SCENE_INDEX);
    }
}
