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
    public Button toggleButton;


    private void Awake()
    {
        toggleButton.onClick.AddListener(() => currentStatus = !currentStatus);
    }

    void Update()
    {
        if (currentStatus)
        {
            cellState.color = Color.white;
        }
        else
        {
            cellState.color = Color.black;
        }
    }
    public void applyState()
    {
        currentStatus = nextStatus;
    }
}
