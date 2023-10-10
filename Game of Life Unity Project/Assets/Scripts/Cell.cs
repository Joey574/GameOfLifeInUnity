using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    [Header("Location")]
    public int x;
    public int y;

    public bool currentStatus;
    public bool nextStatus;

    public Image cellState;

    public void kill()
    {
        cellState.color = Color.black;
        currentStatus = false;
    }

    public void born()
    {
        cellState.color = Color.white;
        currentStatus = true;
    }
}
