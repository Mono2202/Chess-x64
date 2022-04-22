using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchPrefab : MonoBehaviour
{
    // Fields:
    public Text whiteUsernameText;
    public Text blackUsernameText;
    public Text gameText;
    public Text scoreText;
    public Text dateText;

    public void SetPrefab(string whiteUsername, string blackUsername, string game,
        string wonUsername, string date)
    {
        // Setting the labels:
        whiteUsernameText.text = whiteUsername;
        blackUsernameText.text = blackUsername;
        gameText.text = game;
        dateText.text = date;

        // Condition: tie
        if (wonUsername == "!TIE!")
        {
            scoreText.text = "1/2 - 1/2";
        }

        // Condition: white won
        else
        {
            scoreText.text = (wonUsername == whiteUsername) ? "1 - 0" : "0 - 1";
        }
    }
}
