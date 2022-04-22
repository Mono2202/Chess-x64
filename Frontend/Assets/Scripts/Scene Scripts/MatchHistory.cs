using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchHistory : MonoBehaviour
{
    // Fields:
    private Communicator communicator;
    public GameObject content;
    public GameObject matchPrefab;

    // Start is called before the first frame update
    void Start()
    {
        // Inits:
        communicator = Data.instance.communicator;
        string[] games;

        // Sending the games request:
        communicator.Write(Serializer.SerializeRequest<GetMatchHistoryRequest>(new GetMatchHistoryRequest { Username = Data.instance.username }, Serializer.GET_MATCH_HISTORY_REQUEST));

        // Deserializing the response:
        GetMatchHistoryResponse response = Deserializer.DeserializeResponse<GetMatchHistoryResponse>(communicator.Read());
        games = response.Games.Split(new string[] { "@@" }, StringSplitOptions.None);

        // Assigning the labels:
        for (int i = 0; i < games.Length; i++)
        {
            if (games[i] != "")
            {
                // Inits:
                string[] currentGame = games[i].Split(new string[] { "&&&" }, StringSplitOptions.None);

                // Creating the match prefab:
                GameObject currentMatch = Instantiate(matchPrefab);
                currentMatch.transform.SetParent(content.transform, false);
                currentMatch.transform.localPosition = new Vector3(0, 140 - i * 100, 0);
                currentMatch.GetComponent<MatchPrefab>().SetPrefab(currentGame[0],
                    currentGame[1], currentGame[2], currentGame[3], currentGame[4]);
            }
        }
    }
}