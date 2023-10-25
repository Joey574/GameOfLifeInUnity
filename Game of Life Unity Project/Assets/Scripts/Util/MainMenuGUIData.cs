using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MainMenuGUIData
{
    public Vector2 screenResolution;

    public Rect start, settings, info, exit, exit2, textureWTextBox, textureHTextBox, scrollView, viewRect;
    public Rect firstGame, secondGame, thirdGame, fourthGame, fifthGame;

    public Vector2 buttonSize;
    public Vector2 textBoxSize;

    public Vector2 scroller;

    public void Initialize()
    {
        screenResolution.x = Screen.currentResolution.width;
        screenResolution.y = Screen.currentResolution.height;

        scroller = Vector2.zero;

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

        scrollView.x = 10; scrollView.y = 10; scrollView.width = buttonSize.x; scrollView.height = buttonSize.y * 3;
        viewRect.x = 0; viewRect.y = 0; viewRect.width = 100; viewRect.height = buttonSize.y * 5;

        firstGame.x = 0; firstGame.y = 0; firstGame.width = buttonSize.x; firstGame.height = buttonSize.y;
        secondGame.x = 0; secondGame.y = buttonSize.y; secondGame.width = buttonSize.x; secondGame.height = buttonSize.y;
        thirdGame.x = 0; thirdGame.y = buttonSize.y * 2; thirdGame.width = buttonSize.x; thirdGame.height = buttonSize.y;
        fourthGame.x = 0; fourthGame.y = buttonSize.y * 3; fourthGame.width = buttonSize.x; fourthGame.height = buttonSize.y;
        fifthGame.x = 0; fifthGame.y = buttonSize.y * 4; fifthGame.width = buttonSize.x; fifthGame.height = buttonSize.y;
    }
}
