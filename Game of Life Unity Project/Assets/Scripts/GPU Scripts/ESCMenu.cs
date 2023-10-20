using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ESCMenu : MonoBehaviour
{
    public bool render = false;

    private int buttonWidth = 200;
    private int buttonHeight = 50;

    public void begin()
    {
        render = true;
    }

    public void OnGUI() 
    {
        float w = (Screen.currentResolution.width / 2) - (buttonWidth / 2);
        float h = Screen.currentResolution.height / 2;

        if (render)
        {
            GUI.color = Color.gray;
            GUI.Button(new Rect(w, h, buttonWidth, buttonHeight), "Resume");
            GUI.Button(new Rect(w, h, buttonWidth, buttonHeight), "Settings");
            GUI.Button(new Rect(w, h, buttonWidth, buttonHeight), "Exit to Menu");
            GUI.Button(new Rect(w, h, buttonWidth, buttonHeight), "Exit Game");
        }
    }
}
