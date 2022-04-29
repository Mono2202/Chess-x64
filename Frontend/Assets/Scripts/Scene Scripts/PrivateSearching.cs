using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class PrivateSearching : MonoBehaviour
{
    // Fields:
    public Text roomIdText;
    private Communicator communicator;

    // Start is called before the first frame update
    void Start()
    {
        // Inits:
        communicator = Data.instance.communicator;

        // Sending the create room request:
        communicator.Write(Serializer.SerializeRequest<CreateRoomRequest>(new CreateRoomRequest { GameMode = "P" }, Serializer.CREATE_ROOM_REQUEST));

        // Deserializing the response:
        CreateRoomResponse response = Deserializer.DeserializeResponse<CreateRoomResponse>(communicator.Read());

        // Setting the room ID:
        roomIdText.text = response.RoomID.ToString();

        // Waiting for another player to join:
        StartCoroutine(WaitForPlayer());
    }

    /*
     * Waiting for a player to join the room
     * Input : < None >
     * Output: < None >
     */
    private IEnumerator WaitForPlayer()
    {
        while (true)
        {
            // Waiting for half a second:
            yield return new WaitForSeconds(Data.DELAY_TIME);

            // Sending the get room state request:
            communicator.Write(Serializer.SerializeRequest<GetRoomStateRequest>(new GetRoomStateRequest { }, Serializer.GET_ROOM_STATE_REQUEST));

            // Deserializing the response:
            GetRoomStateResponse response = Deserializer.DeserializeResponse<GetRoomStateResponse>(communicator.Read());

            // Condition: game has started
            if (response.IsActive)
            {
                // Switching to the game scene:
                this.GetComponent<SwitchScene>().SwitchSceneByIndex(Data.GAME_SCENE_COUNT);
            }
        }
    }

    /*
     * Copying the room code to the user's clipboard
     * Input : < None >
     * Output: < None >
     */
    public void CopyToClipboard()
    {
        // Copying the room code to the clipboard:
        TextEditor te = new TextEditor();
        te.text = roomIdText.text;
        te.SelectAll();
        te.Copy();
    }

    /*
     * Returning to menu screen
     * Input : < None >
     * Output: < None >
     */
    public void ReturnToMenu()
    {
        // Sending the leave room request:
        communicator.Write(Serializer.SerializeRequest<LeaveRoomRequest>(new LeaveRoomRequest { }, Serializer.LEAVE_ROOM_REQUEST));

        // Reading the message:
        string msg = communicator.Read();

        // Switching to the menu scene:
        this.GetComponent<SwitchScene>().SwitchSceneByIndex(Data.PLAY_SCENE_COUNT);
    }
}