using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Settings : MonoBehaviour
{
    // Fields:
    public Button changeWhite;
    public Text changeWhiteText;
    public Button changeBlack;
    public Text changeBlackText;
    public InputField rWhite;
    public InputField gWhite;
    public InputField bWhite;
    public InputField aWhite;
    public InputField rBlack;
    public InputField gBlack;
    public InputField bBlack;
    public InputField aBlack;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Checking if white square fields were filled:
        changeWhite.interactable = !(rWhite.text == "" || gWhite.text == "" || bWhite.text == "" || aWhite.text == "");
        changeWhiteText.color = !(rWhite.text == "" || gWhite.text == "" || bWhite.text == "" || aWhite.text == "") ? 
            new Color32(31, 194, 18, 255) : new Color32(31, 194, 18, 100);

        // Checking if black square fields were filled:
        changeBlack.interactable = !(rBlack.text == "" || gBlack.text == "" || bBlack.text == "" || aBlack.text == "");
        changeBlackText.color = !(rBlack.text == "" || gBlack.text == "" || bBlack.text == "" || aBlack.text == "") ?
            new Color32(31, 194, 18, 255) : new Color32(31, 194, 18, 100);
    }

    public void ChangeColorWhite()
    {
        // Changing the white colored squares:
        Data.instance.whiteSquareColor = new Color32(Convert.ToByte(rWhite.text),
            Convert.ToByte(gWhite.text), Convert.ToByte(bWhite.text), Convert.ToByte(aWhite.text));
    }

    public void ChangeColorBlack()
    {
        // Changing the black colored squares:
        Data.instance.blackSquareColor = new Color32(Convert.ToByte(rBlack.text),
            Convert.ToByte(gBlack.text), Convert.ToByte(bBlack.text), Convert.ToByte(aBlack.text));
    }
}
