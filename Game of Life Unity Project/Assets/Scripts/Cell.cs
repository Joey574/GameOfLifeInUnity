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

    private Image cellState;

    void Awake()
    {
        cellState = gameObject.GetComponentInChildren<Image>();
    }

    public void applyTexture()
    {
        cellState.enabled = currentStatus;
    }
}
