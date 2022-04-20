using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopupWindow : MonoBehaviour
{
    // Fields:
    public GameObject popupWindow;
    public Text titleLabel;
    public Text textLabel;

    public void SetProperties(string title, string text, Color32 titleColor, Color32 textColor)
    {
        // Setting the lables:
        titleLabel.text = title;
        textLabel.text = text;

        // Setting the colors:
        titleLabel.color = titleColor;
        textLabel.color = textColor;
    }

    public void ClosePopup()
    {
        popupWindow.SetActive(false);
    }
}
