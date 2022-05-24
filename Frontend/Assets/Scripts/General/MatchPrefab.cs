using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MatchPrefab : MonoBehaviour
{
    // Inputs:
    public Text whiteUsernameText;
    public Text blackUsernameText;
    public Text gameText;
    public Text scoreText;
    public Text dateText;

    /*
     * Setting the prefab's properties
     * Input : whiteUsername - the white player's username
     *         blackUsername - the black player's username
     *         wonUsername   - the winning player's username
     *         date          - the date the game took place
     * Output: < None >
     */
    public void SetPrefab(string whiteUsername, string blackUsername,
        string wonUsername, string date)
    {
        // Setting the labels:
        whiteUsernameText.text = whiteUsername;
        blackUsernameText.text = blackUsername;
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
