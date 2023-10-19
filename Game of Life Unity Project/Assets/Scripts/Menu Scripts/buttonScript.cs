using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class buttonScript : MonoBehaviour
{
    [Header("Button")]
    public Button startButton, exitButton;

    [Header("Button Position")]
    public int startX, startY, exitX, exitY;

    // Start is called before the first frame update
    void Start()
    {
        startButton.onClick.AddListener(startGame);
        exitButton.onClick.AddListener(exitGame);
    }

    private void Update()
    {
        
    }

    void startGame()
    {
        SceneManager.LoadScene("Game GPU", LoadSceneMode.Single);
        Debug.Log("Start game");
    }

    void exitGame()
    {
        Application.Quit();
        Debug.Log("Exit game");
    }

    void setPos()
    {

    }

}
