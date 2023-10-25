using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameValues : MonoBehaviour
{
    public Vector2 gameBoardSize;
<<<<<<< Updated upstream
    public Color liveCell;
=======
    public Color livePixel;
    public bool rand;
    public float randFil;
>>>>>>> Stashed changes

    private void Awake()
    {
        gameBoardSize.x = Screen.currentResolution.width;
        gameBoardSize.y = Screen.currentResolution.height;

        DontDestroyOnLoad(gameObject);
    }
}
