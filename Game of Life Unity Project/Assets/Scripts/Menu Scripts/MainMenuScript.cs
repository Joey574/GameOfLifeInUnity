using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.UIElements;

public class MainMenuScript : MonoBehaviour
{
    private string x = "";
    private string y = "";

    private bool inSettings;
    private bool inStart;
    private bool inInfo;

    private GameValues gameValues;

    private MainMenuGUIData gui;

    private void Awake()
    {
        gui = new MainMenuGUIData();
        gui.Initialize();

        gameValues = GameObject.Find("gameValues").GetComponent<GameValues>();
    }

    private void OnGUI()
    {
        GUIStyle style = new GUIStyle(GUI.skin.button);
        GUIStyle styleText = new GUIStyle(GUI.skin.textField);
        style.fontSize = 28;
        styleText.fontSize = 28;

        GUI.BeginGroup(new Rect((gui.screenResolution.x / 2) - (gui.buttonSize.x / 2),
            (gui.screenResolution.y / 2) - (gui.buttonSize.y * 2),
            gui.screenResolution.x,
            (gui.screenResolution.y)));

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
        if (GUI.Button(gui.start, "Start", style)) { inStart = true; }
        if (GUI.Button(gui.settings, "Settings", style)) { inSettings = true; }
        if (GUI.Button(gui.info, "Info", style)) { inInfo = true; }
        if (GUI.Button(gui.exit, "Exit", style)) { Application.Quit(); }
    }

    private void settingsMenu(GUIStyle styleText, GUIStyle style)
    {
        x = GUI.TextField(gui.textureWTextBox, x, styleText);
        y = GUI.TextField(gui.textureHTextBox, y, styleText);

        try
        {
            gameValues.gameBoardSize.x = Int32.Parse(x);
            gameValues.gameBoardSize.y = Int32.Parse(y);
        }
        catch
        {
            x = gui.screenResolution.x.ToString();
            y = gui.screenResolution.y.ToString();
        }

        if (GUI.Button(gui.exit, "Back", style)) { inSettings = false; }
    }


    private void infoMenu()
    {

    }


    private void startMenu(GUIStyle styleText, GUIStyle style)
    {
        gui.scroller = GUI.BeginScrollView(gui.scrollView, gui.scroller, gui.viewRect, true, true);

        string game = null;

        GUI.Button(new Rect(10, 10, 10, 10), "TestFFS", style);

        if (GUI.Button(gui.firstGame, "Classic", style)) { }

        if (game != null)
        {
            startGame(game);
        }

        GUI.EndScrollView();
    }

    private void startGame(string gameMode)
    {
        SceneManager.LoadScene(gameMode, LoadSceneMode.Single);
    }
}  
