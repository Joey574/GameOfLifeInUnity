using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class gameManagerScript : MonoBehaviour
{

    [Header("Gameobjects")]
    public GameObject gameManager;
    public GameObject cellPrefab;
    public GameObject canvas;

    [Header("Cell Info")]
    public int cellWidth = 10;
    public int cellHeight = 10;

    private settingsManager settings;
    private int width;
    private int height;

    private float xStart;
    private float yStart;

    public float yOffset;

    private List<GameObject> cells;
    private GameObject temp;

    private RectTransform canvasSize;

    private float xAdjust;
    private float yAdjust;

    void Awake()
    {
        getCanvasInfo();

        xStart = -(canvasSize.rect.width / 2);
        yStart = -(canvasSize.rect.height / 2);

        cells = new List<GameObject>();

        settings = gameManager.GetComponent<settingsManager>();

        xAdjust = (canvasSize.rect.width / 2);
        yAdjust = (canvasSize.rect.height / 2);

        //height = settings.getHeight();
        //width = settings.getWidth();

        for (float i = yStart; i < ((canvasSize.rect.height / 2) - yOffset); i = i + cellHeight)
        {
            for (float x = xStart; x < (canvasSize.rect.width / 2); x = x + cellWidth)
            {
                temp = Instantiate(cellPrefab, new Vector3(x + xAdjust, i + yAdjust, 0), Quaternion.identity);
                temp.transform.SetParent(canvas.transform);
                cells.Add(temp);
            }
        }
    }

    private void getCanvasInfo()
    {
        canvasSize = canvas.GetComponent<RectTransform>();
    }

}