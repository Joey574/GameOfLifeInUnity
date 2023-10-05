using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public int x;
    public int y;

    private bool currentStatus = false;
    private bool nextStatus;

    private Image cellState;

    void Awake()
    {
        cellState = gameObject.GetComponentInChildren<Image>();
    }
    void Update()
    {
        if (currentStatus)
        {
            cellState.enabled = false;
        }
        else
        {
            cellState.enabled = true;

        }
    }

    public bool getStatus()
    {
        return currentStatus;
    }

    public void setNext(bool next)
    {
        nextStatus = next;
    }

}
