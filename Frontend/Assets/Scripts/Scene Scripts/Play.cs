using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Play : MonoBehaviour
{
    // Fields:
    public InputField roomCodeInput;
    public Button joinRoomButton;
    public Text joinRoomButtonText;
    private Communicator communicator;

    // Start is called before the first frame update
    void Start()
    {
        // Inits:
        communicator = Data.instance.communicator;
    }

    // Update is called once per frame
    void Update()
    {
        // Checking if fields were filled:
        joinRoomButton.interactable = !(roomCodeInput.text == "");
        joinRoomButtonText.color = !(roomCodeInput.text == "") ? new Color32(31, 194, 18, 255) : new Color32(31, 194, 18, 100);
    }

    public void JoinPrivateRoom()
    {
        print(roomCodeInput.text);

        // Sending the search private room request:
        communicator.Write(Serializer.SerializeRequest<SearchPrivateRoomRequest>(new SearchPrivateRoomRequest { RoomCode = roomCodeInput.text }, Serializer.SEARCH_PRIVATE_ROOM_REQUEST));

        // Deserializing the response:
        SearchPrivateRoomResponse response = Deserializer.DeserializeResponse<SearchPrivateRoomResponse>(communicator.Read());

        print("response: " + response.RoomID);

        // Condition: room found
        if (response.RoomID != 0)
        {
            // Sending the join room request:
            communicator.Write(Serializer.SerializeRequest<JoinRoomRequest>(new JoinRoomRequest { RoomID = response.RoomID }, Serializer.JOIN_ROOM_REQUEST));

            // Reading the response:
            string msg = communicator.Read();

            // Switching to the game scene:
            this.GetComponent<SwitchScene>().SwitchSceneByIndex(Data.GAME_SCENE_COUNT);
        }

        // Condition: room not found, creating a new room
        else
        {
            return; // TODO: ADD POPUP
        }
    }
}
