using System.Collections;
using UnityEngine;

public class Searching : MonoBehaviour
{
    // Fields:
    private Communicator communicator;

    // Start is called before the first frame update
    void Start()
    {
        // Inits:
        communicator = Data.instance.communicator;

        // Sending the search ELO room request:
        communicator.Write(Serializer.SerializeRequest<SearchEloRoomRequest>(new SearchEloRoomRequest { }, Serializer.SEARCH_ELO_ROOM_REQUEST));

        // Deserializing the response:
        SearchEloRoomResponse response = Deserializer.DeserializeResponse<SearchEloRoomResponse>(communicator.Read());

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
            // Sending the request:
            communicator.Write(Serializer.SerializeRequest<CreateRoomRequest>(new CreateRoomRequest { GameMode = "E" }, Serializer.CREATE_ROOM_REQUEST));

            // Reading the response:
            string msg = communicator.Read();

            // Waiting for another player to join:
            StartCoroutine(WaitForPlayer());
        }
    }

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
}