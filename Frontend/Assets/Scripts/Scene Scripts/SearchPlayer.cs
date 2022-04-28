using UnityEngine;
using UnityEngine.UI;

public class SearchPlayer : MonoBehaviour
{
    // Fields:
    public InputField usernameInput;
    public Button searchButton;
    public Text searchButtonText;
    public GameObject popupWindow;
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
        searchButton.interactable = !(usernameInput.text == "");
        searchButtonText.color = !(usernameInput.text == "") ? new Color32(31, 194, 18, 255) : new Color32(31, 194, 18, 100);
    }

    public void GetProfile()
    {
        // Sending the stats request:
        communicator.Write(Serializer.SerializeRequest<GetPersonalStatsRequest>(new GetPersonalStatsRequest { Username = usernameInput.text }, Serializer.GET_PERSONAL_STATS_REQUEST));

        // Getting the response:
        string msg = communicator.Read();

        // Condition: error occured
        if (msg[0] == Deserializer.ERROR_RESPONSE)
        {
            // Getting the error message:
            ErrorResponse err = Deserializer.DeserializeResponse<ErrorResponse>(msg);
            popupWindow.GetComponent<PopupWindow>().SetProperties("ERROR", err.Message, new Color32(240, 41, 41, 255), new Color32(240, 41, 41, 255));
            popupWindow.SetActive(true);
            return;
        }

        // Updating the username:
        Data.instance.profileUsername = usernameInput.text;

        // Switching to the profile scene:
        this.GetComponent<SwitchScene>().SwitchSceneByIndex(Data.PROFILE_SCENE_COUNT);
    }
}