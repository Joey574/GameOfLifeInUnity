using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

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
        SceneManager.LoadScene("Game", LoadSceneMode.Single);
        Debug.Log("Start game");
    }

    void exitGame()
    {
        Application.Quit();
        Debug.Log("Exit game");
    }

}
