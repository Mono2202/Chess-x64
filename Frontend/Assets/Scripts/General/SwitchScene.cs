using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    public void SwitchSceneByIndex(int index)
    {
        SceneManager.LoadScene(index);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    public void SwitchToPersonalProfile()
    {
        Data.instance.profileUsername = Data.instance.username;
        SceneManager.LoadScene(Data.PROFILE_SCENE_COUNT);
    }
}
