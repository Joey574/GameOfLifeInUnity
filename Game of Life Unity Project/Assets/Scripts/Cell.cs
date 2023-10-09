using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Cell : MonoBehaviour
{
    public int x;
    public int y;

    public bool currentStatus = false;
    private bool nextStatus;

    private Image cellState;

    void Awake()
    {
        cellState = gameObject.GetComponentInChildren<Image>();
    }
    void Update()
    {
        cellState.enabled = currentStatus;
    }

    public void updateStatus()
    {
        currentStatus = nextStatus;
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
