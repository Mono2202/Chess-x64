using UnityEngine;
using UnityEngine.UI;

public class Register : MonoBehaviour
{
    // Fields:
    public InputField usernameInput;
    public InputField passwordInput;
    public InputField emailInput;
    public Button signUpButton;
    private Communicator communicator;

    // Start is called before the first frame update
    void Start()
    {
        // Inits:
        communicator = Data.instance.communicator;
    }

    void Update()
    {
        // Checking if fields were filled:
        signUpButton.interactable = !(usernameInput.text == "" || passwordInput.text == "" || emailInput.text == "");
    }

    public void SendRegisterMessage()
    {
        // Sending the sign-up request:
        communicator.Write(Serializer.SerializeRequest<SignupRequest>(new SignupRequest { Username = usernameInput.text, Password = passwordInput.text, Email = emailInput.text }, Serializer.SIGNUP_REQUEST));

        // Getting the response:
        string msg = communicator.Read();

        // Condition: error occured
        if (msg[0] == Deserializer.ERROR_RESPONSE)
        {
            // Getting the error message:
            ErrorResponse err = Deserializer.DeserializeResponse<ErrorResponse>(msg);
            //EditorUtility.DisplayDialog("Sign-up Failed", response.Message, "Try Again");
            // TODO: ADD MESSAGE BOX
            return;
        }

        // Switching to the login scene:
        this.GetComponent<SwitchScene>().SwitchSceneByIndex(Data.LOGIN_SCENE_INDEX);
    }
}