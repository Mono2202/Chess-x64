using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stats : MonoBehaviour
{
    // Fields:
    private Communicator communicator;
    public List<Text> statsLables;

    // Start is called before the first frame update
    void Start()
    {
        // Inits:
        statsLables[0].text += Data.instance.profileUsername;
        communicator = Data.instance.communicator;
        string[] stats;

        // Sending the stats request:
        communicator.Write(Serializer.SerializeRequest<GetPersonalStatsRequest>(new GetPersonalStatsRequest { Username = Data.instance.profileUsername }, Serializer.GET_PERSONAL_STATS_REQUEST));

        // Deserializing the response:
        GetPersonalStatsResponse response = Deserializer.DeserializeResponse<GetPersonalStatsResponse>(communicator.Read());
        stats = response.Statistics.Split(new string[] { ", " }, StringSplitOptions.None);

        // Assigning the labels:
        for (int i = 1; i < stats.Length + 1; i++)
        {
            statsLables[i].text = stats[i - 1];
        }
    }
}