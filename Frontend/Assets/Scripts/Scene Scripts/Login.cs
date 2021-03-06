using UnityEngine;
using UnityEngine.UI;

public class Login : MonoBehaviour
{
    // Fields:
    public InputField usernameInput;
    public InputField passwordInput;
    public Button signInButton;
    public Text signInButtonText;
    public GameObject popupWindow;
    private Communicator communicator;

    // Start is called before the first frame update
    void Start()
    {
        // Inits:
        communicator = Data.instance.communicator;
    }

    // Update is called every frame
    void Update()
    {
        // Checking if fields were filled:
        signInButton.interactable = !(usernameInput.text == "" || passwordInput.text == "");
        signInButtonText.color = !(usernameInput.text == "" || passwordInput.text == "") ? new Color32(31, 194, 18, 255) : new Color32(31, 194, 18, 100);
    }

    /*
     * Sending a login message to the server
     * Input : < None >
     * Output: < None >
     */
    public void SendLoginMessage()
    {
        // Sending the login message:
        communicator.Write(Serializer.SerializeRequest<LoginRequest>(new LoginRequest { Username = usernameInput.text, Password = passwordInput.text }, Serializer.LOGIN_REQUEST));

        // Getting the response:
        string msg = communicator.Read();

        // Condition: error occured
        if (msg[0] == Deserializer.ERROR_RESPONSE)
        {
            // Getting the error message:
            ErrorResponse err = Deserializer.DeserializeResponse<ErrorResponse>(msg);
            popupWindow.GetComponent<PopupWindow>().SetProperties("ERROR", err.Message, new Color32(240, 41, 41, 255), new Color32(240, 41, 41, 255));
            popupWindow.SetActive(true);

            // Resetting the inputs:
            usernameInput.text = "";
            passwordInput.text = "";
            return;
        }

        // Updating the username:
        Data.instance.username = usernameInput.text;
        Data.instance.profileUsername = usernameInput.text;

        // Switching to the menu scene:
        this.GetComponent<SwitchScene>().SwitchSceneByIndex(Data.HOME_SCENE_COUNT);
    }
}