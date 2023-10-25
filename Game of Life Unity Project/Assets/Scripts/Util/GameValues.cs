using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class GameValues : MonoBehaviour
{
    public Vector2 gameBoardSize;
    public Color liveCell;

    public bool rand;
    public float randFil;

    private void Awake()
    {
        gameBoardSize.x = Screen.currentResolution.width;
        gameBoardSize.y = Screen.currentResolution.height;

        liveCell = Color.white;

        DontDestroyOnLoad(gameObject);
    }
}
