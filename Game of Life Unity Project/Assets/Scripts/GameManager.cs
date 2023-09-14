using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public int numberOfTiles;

    void Start()
    {
        for (int i = 0; i < numberOfTiles; i++)
        {
            GameObject.Find("Square(" + i + ")").GetComponent<DotBehavior>().setDotNumber(numberOfTiles);

        }
    }
}
