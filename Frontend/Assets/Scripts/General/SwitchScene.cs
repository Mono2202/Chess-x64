using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchScene : MonoBehaviour
{
    /*
     * Switching scene by index
     * Input : index - the scene's index
     * Output: < None >
     */
    public void SwitchSceneByIndex(int index)
    {
        SceneManager.LoadScene(index);
    }

    /*
     * Exiting from the program
     * Input : < None >
     * Output: < None >
     */
    public void ExitGame()
    {
        Application.Quit();
    }

    /*
     * Switching to the personal profile scene
     * Input : < None >
     * Output: < None >
     */
    public void SwitchToPersonalProfile()
    {
        Data.instance.profileUsername = Data.instance.username;
        SceneManager.LoadScene(Data.PROFILE_SCENE_COUNT);
    }
}
