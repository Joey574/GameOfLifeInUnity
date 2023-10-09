using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class gameManagerScript : MonoBehaviour
{

    [Header("Gameobjects")]
    public GameObject gameManager;
    public GameObject cellPrefab;
    public GameObject canvas;
    public GameObject background;
    public Button startButton;

    [Header("Cell Info")]
    public int cellWidth = 10;
    public int cellHeight = 10;
    public float yOffset;

    [Header("Button Adjustments")]
    public float xButtonOffset;
    public float yButtonOffset;

    [Header("Sim info")]
    public float simSteps;
    public int generation = 0;

    private settingsManager settings;
    private int width;
    private int height;

    private float xStart;
    private float yStart;

    private List<GameObject> cells;
    private GameObject temp;

    private RectTransform canvasSize;

    private float xAdjust;
    private float yAdjust;

    private int xCount = -1;
    private int yCount;

    private int maxX;
    private int maxY;

    private Cell t;

    private bool beginSim = false;

    void Awake()
    {
        getCanvasInfo();
        setLocations();

        xStart = -(canvasSize.rect.width / 2);
        yStart = -(canvasSize.rect.height / 2);

        cells = new List<GameObject>();

        xAdjust = (canvasSize.rect.width / 2);
        yAdjust = (canvasSize.rect.height / 2);

        for (float i = yStart; i < ((canvasSize.rect.height / 2) - yOffset); i = i + cellHeight)
        {
            if (xCount != -1)
            {
                maxX = xCount - 1;
            }
            xCount = 0;
            for (float x = xStart; x < (canvasSize.rect.width / 2); x = x + cellWidth)
            {
                temp = Instantiate(cellPrefab, new Vector3(x + xAdjust, i + yAdjust, 0), Quaternion.identity);
                temp.transform.SetParent(canvas.transform);
                t = temp.GetComponent<Cell>();
                t.x = xCount; t.y = yCount;
                cells.Add(temp);

                xCount++;
            }
            yCount++;
        }
        maxY = yCount - 1;
        Debug.Log("Cells: " + cells.Count);
    }

    void Update()
    {
        startButton.onClick.AddListener(startSim);

        if (beginSim)
        {
            Invoke(nameof(simStep), (1.0f));
        }
    }

    private void startSim()
    {
        beginSim = true;
    }

    private void simStep()
    {
        generation++;

        for (int i = 0; i < cells.Count(); i++)
        {
            int neighbors = 0;
            try
            {
                if (cells[i - 1] != null && cells[i - 1].GetComponent<Cell>().getStatus())
                {
                    neighbors++;
                }
            } catch { }

            try
            {
                if (cells[i + 1] != null && cells[i - 1].GetComponent<Cell>().getStatus())
                {
                    neighbors++;
                }
            } catch { }

            try
            {
                if (cells[i - xCount] != null && cells[i - 1].GetComponent<Cell>().getStatus())
                {
                    neighbors++;
                }
            }
            catch { }

            try
            {
                if (cells[i + xCount] != null && cells[i - 1].GetComponent<Cell>().getStatus())
                {
                    neighbors++;
                }
            }
            catch { }

            try
            {
                if (cells[i - xCount + 1] != null && cells[i - 1].GetComponent<Cell>().getStatus())
                {
                    neighbors++;
                }
            }
            catch { }

            try
            {
                if (cells[i - xCount - 1] != null && cells[i - 1].GetComponent<Cell>().getStatus())
                {
                    neighbors++;
                }
            }
            catch { }

            try
            {
                if (cells[i + xCount - 1] != null && cells[i - 1].GetComponent<Cell>().getStatus())
                {
                    neighbors++;
                }
            }
            catch { }

            try
            {
                if (cells[i + xCount + 1] != null && cells[i - 1].GetComponent<Cell>().getStatus())
                {
                    neighbors++;
                }
            }
            catch { }

            if (neighbors == 2) 
            {
                cells[i].GetComponent<Cell>().setNext(cells[i].GetComponent<Cell>().getStatus());
            } else if (neighbors == 3)
            {
                cells[i].GetComponent<Cell>().setNext(true);
            } else
            {
                cells[i].GetComponent<Cell>().setNext(false);
            }
        }

        for (int i = 0; i < cells.Count(); i++)
        {
            cells[i].GetComponent<Cell>().updateStatus();
        }
    }

    private void getCanvasInfo()
    {
        canvasSize = canvas.GetComponent<RectTransform>();
    }

    private void setLocations()
    {
        background.transform.localPosition = new Vector3(0, 0, 0);
        startButton.transform.localPosition = new Vector3(-(canvasSize.rect.width / 2) + xButtonOffset,
            (canvasSize.rect.height / 2) - yButtonOffset, 0);
    }

}