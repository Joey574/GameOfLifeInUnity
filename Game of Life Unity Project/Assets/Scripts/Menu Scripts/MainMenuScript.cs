using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;

public class MainMenuScript : MonoBehaviour
{
    [Header("Button")]
    private Rect start, settings, exit, textureWTextBox, textureHTextBox;
    private Vector2 buttonSize;
    private Vector2 textBoxSize;

    [Header("Private data")]
    private Vector2 screenResolution;

    private string x = "3840";
    private string y = "2160";

    private bool inSettings;

    private GameValues gameValues;

    private void Awake()
    {
        screenResolution.x = Screen.currentResolution.width;
        screenResolution.y = Screen.currentResolution.height;

        buttonSize.x = screenResolution.x / 5;
        buttonSize.y = screenResolution.y / 15;

        textBoxSize.x = screenResolution.x / 5;
        textBoxSize.y = screenResolution.y / 15;

        start.width = buttonSize.x; start.height = buttonSize.y;
        settings.width = buttonSize.x; settings.height = buttonSize.y;
        exit.width = buttonSize.x; exit.height = buttonSize.y;

        textureWTextBox.width = textBoxSize.x; textureWTextBox.height = textBoxSize.y;
        textureHTextBox.width = textBoxSize.x; textureHTextBox.height = textBoxSize.y;

        settings.y = buttonSize.y;
        exit.y = 2 * buttonSize.y;

        textureHTextBox.y = textBoxSize.y;

        gameValues = GameObject.Find("gameValues").GetComponent<GameValues>();
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);
        GUIStyle styleText = new GUIStyle(GUI.skin.textField);
        style.fontSize = 28;
        styleText.fontSize = 28;

        GUI.BeginGroup(new Rect((screenResolution.x / 2) - (buttonSize.x / 2),
            (screenResolution.y / 2) - (buttonSize.y * 2),
            buttonSize.x,
            (buttonSize.y * 3)));

        if (!inSettings)
        {
            if (GUI.Button(start, "Start", style)) { startGame(); }
            if (GUI.Button(settings, "Settings", style)) { inSettings = true; }
            if (GUI.Button(exit, "Exit", style)) { Application.Quit(); }

        }
        else
        {
            x = GUI.TextField(textureWTextBox, x, styleText);
            y = GUI.TextField(textureHTextBox, y, styleText);

            try
            {
                gameValues.gameBoardSize.x = Int32.Parse(x);
                gameValues.gameBoardSize.y = Int32.Parse(y);
            } catch 
            {
                x = "3840";
                y = "2160";
            }

            if (GUI.Button(exit, "Back", style)) { inSettings = false; }
        }

        GUI.EndGroup();
    }

    private void startGame()
    {
        SceneManager.LoadScene("Classic Mode", LoadSceneMode.Single);
    }

}
