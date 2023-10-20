using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ESCMenu : MonoBehaviour
{
    public bool render = false;

    private int buttonWidth;
    private int buttonHeight;

    private GameManagerGPU gameManagerGPU;

    public void begin(GameManagerGPU gameManagerGPU)
    {
        buttonHeight = Screen.currentResolution.height / 16;
        buttonWidth = Screen.currentResolution.width / 8;

        this.gameManagerGPU = gameManagerGPU;
        render = true;
    }

    public void OnGUI() 
    {
        if (render)
        {
            float w = (Screen.currentResolution.width / 2) - (buttonWidth / 2);
            float h = Screen.currentResolution.height / 2;

            GUI.color = Color.white;
            GUI.backgroundColor = Color.grey;

            if (GUI.Button(new Rect(w, h - buttonHeight - 1, buttonWidth, buttonHeight), "Resume"))
            {
                gameManagerGPU.setMenuCalled(false);
                render = false;
            }

            GUI.Button(new Rect(w, h, buttonWidth, buttonHeight), "Settings");

            if (GUI.Button(new Rect(w, h + buttonHeight + 1, buttonWidth, buttonHeight), "Exit to Menu"))
            {
                SceneManager.LoadScene("Menu", LoadSceneMode.Single);
            }
            if (GUI.Button(new Rect(w, h + (2 * (buttonHeight + 1)), buttonWidth, buttonHeight), "Exit Game"))
            {
                Application.Quit();
            }
        }
    }
}
