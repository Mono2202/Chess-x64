using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupWindow : MonoBehaviour
{
    // Inputs:
    public GameObject popupWindow;
    public Text titleLabel;
    public Text textLabel;

    /*
     * Setting the popup window's properties
     * Input : title      - the popup's title
     *         text       - the popup's text
     *         titleColor - the title's color
     *         textColor  - the text's color
     */
    public void SetProperties(string title, string text, Color32 titleColor, Color32 textColor)
    {
        // Setting the lables:
        titleLabel.text = title;
        textLabel.text = text;

        // Setting the colors:
        titleLabel.color = titleColor;
        textLabel.color = textColor;
    }

    /*
     * Closing the popup window
     * Input : < None >
     * Output: < None >
     */
    public void ClosePopup()
    {
        popupWindow.SetActive(false);
    }
}
