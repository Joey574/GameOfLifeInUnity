using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI

public class buttonScript : MonoBehaviour
{
    public Button startButton, exitButton;

    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(startGame);
        exitButton.onClick.AddListener(exitGame);
    }

    void startGame()
    {
        Debug.Log("Start game");
    }

    void exitGame()
    {
        Debug.Log("Exit game");
    }

}
