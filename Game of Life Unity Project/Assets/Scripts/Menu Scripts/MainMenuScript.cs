using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UIElements;

public class MainMenuScript : MonoBehaviour
{
    [Header("Button")]
    private Rect start, settings, info, exit, exit2, textureWTextBox, textureHTextBox;
    private Vector2 buttonSize;
    private Vector2 textBoxSize;

    [Header("Private data")]
    private Vector2 screenResolution;

    private string x = "";
    private string y = "";

    private bool inSettings;
    private bool inStart;
    private bool inInfo;

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
        info.width = buttonSize.x; info.height = buttonSize.y;
        exit.width = buttonSize.x; exit.height = buttonSize.y;
        exit2.width = buttonSize.x; exit2.height = buttonSize.y;

        textureWTextBox.width = textBoxSize.x; textureWTextBox.height = textBoxSize.y;
        textureHTextBox.width = textBoxSize.x; textureHTextBox.height = textBoxSize.y;

        settings.y = buttonSize.y;
        info.y = 2 * buttonSize.y;
        exit.y = 3 * buttonSize.y;

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
            (buttonSize.y * 4)));

        if (!inSettings && !inStart && !inInfo)
        {
            mainMenu(style);
        }
        else if (inSettings)
        {
            settingsMenu(styleText, style);
        }
        else if (inInfo)
        {
            infoMenu();
        }
        else
        {
            startMenu(styleText, style);
        }

        GUI.EndGroup();
    }

    private void mainMenu(GUIStyle style)
    {
        if (GUI.Button(start, "Start", style)) { inStart = true; }
        if (GUI.Button(settings, "Settings", style)) { inSettings = true; }
        if (GUI.Button(info, "Info", style)) { inInfo = true; }
        if (GUI.Button(exit, "Exit", style)) { Application.Quit(); }
    }

    private void settingsMenu(GUIStyle styleText, GUIStyle style)
    {
        x = GUI.TextField(textureWTextBox, x, styleText);
        y = GUI.TextField(textureHTextBox, y, styleText);

        try
        {
            gameValues.gameBoardSize.x = Int32.Parse(x);
            gameValues.gameBoardSize.y = Int32.Parse(y);
        }
        catch
        {
            x = screenResolution.x.ToString();
            y = screenResolution.y.ToString();
        }

        if (GUI.Button(exit, "Back", style)) { inSettings = false; }
    }


    private void infoMenu()
    {

    }


    private void startMenu(GUIStyle styleText, GUIStyle style)
    {
<<<<<<< Updated upstream
        ScrollView scrollView = new ScrollView();

=======
>>>>>>> Stashed changes
        startGame("Classic Mode");
    }

    private void startGame(string gameMode)
    {
        SceneManager.LoadScene(gameMode, LoadSceneMode.Single);
    }
}  
