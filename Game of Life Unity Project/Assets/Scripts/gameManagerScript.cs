using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using static UnityEngine.RuleTile.TilingRuleOutput;

public class gameManagerScript : MonoBehaviour
{
    [Header("Gameobjects")]
    public GameObject gameManager;
    public GameObject canvas;
    public GameObject background;
    public Button startButton;

    private RectTransform canvasSize;

    [Header("Cell Info")]
    public GameObject cellPrefab;
    public int cellWidth = 10;
    public int cellHeight = 10;
    public float yOffset;

    [Header("Button prefab")]
    public Button button;

    [Header("Button Adjustments")]
    public float xButtonOffset;
    public float yButtonOffset;

    [Header("Sim info")]
    public float simSteps;
    public int generation = 0;

    private int Xcount;
    private int Ycount;

    private List<GameObject> row;
    private List<List<GameObject>> cells;

    private bool beginSim = false;
    private bool stepCalled = false;

    private int same = 0;
    private int died = 0;
    private int born = 0;

    void Awake()
    {
        getCanvasInfo();
        setLocations();

        Xcount = (int) (canvasSize.rect.width / cellWidth) + 1;
        Ycount = (int) ((canvasSize.rect.height / cellHeight) - (yOffset / cellHeight));

        cells = new List<List<GameObject>>();

        for (int y = 0; y < Ycount; y++)
        {
            row = new List<GameObject>();
            for (int x = 0; x < Xcount; x++)
            {
                GameObject temp = cellPrefab;
                row.Add(temp);
            }
            cells.Add(row);
        }

        Debug.Log("Xcount: " + Xcount);
        Debug.Log("Ycount: " + Ycount);

        for (int y = 0; y < Ycount; y++)
        {
            for (int x = 0; x < Xcount; x++)
            {
                GameObject temp = Instantiate(cellPrefab, new Vector3((x * cellWidth) + 5, (y * cellHeight) + 5, 0), Quaternion.identity);
                temp.transform.SetParent(canvasSize.transform, true);
                Cell t = temp.GetComponent<Cell>();
                t.x = x; t.y = y;
                t.currentStatus = randVal();

                if (t.currentStatus)
                {
                    t.born();
                }
                else
                {
                    t.kill();
                }

                cells[y][x] = temp;
            }
        }      

        startButton.onClick.AddListener(() => beginSim = true);

        Debug.Log("Cells: " + (Xcount * Ycount));
    }

    private bool randVal()
    {
        return (Random.Range(0f, 1.0f) > 0.7f);
    }

    void Update()
    {
        if (beginSim && !stepCalled)
        {
            stepCalled = true;
            Invoke(nameof(simStep), (1.0f / simSteps));
        }
    }

    private void simStep()
    {
        generation++;

        calculateCells();

        updateCells();

        //Debug.Log("Born: " + born + " Same: " + same + " Died: " + died);

        stepCalled = false;
    }

    private void calculateCells()
    {
        born = 0;
        died = 0;
        same = 0;

        for (int y = 0; y < Ycount; y++)
        {
            for (int x = 0; x < Xcount; x++)
            {
                int i = getNeighbors(x, y);
                Cell cell = cells[y][x].GetComponent<Cell>();

                if (i < 2 || i >= 4)
                {
                    died++;
                    cell.nextStatus = false;
                }
                else if (i == 2)
                {
                    same++;
                    cell.nextStatus = cell.currentStatus;
                }
                else if (i == 3)
                {
                    born++;
                    cell.nextStatus = true;
                }
            }
        }
    }

    private void updateCells()
    {
        for (int y = 0; y < Ycount; y++)
        {
            for (int x =  0; x < Xcount; x++)
            {
                Cell c = cells[y][x].GetComponent<Cell>();

                if (c.nextStatus)
                {
                    c.born();
                }
                else
                {
                    c.kill();
                }
            }
        }

    }
    private int getNeighbors(int x, int y)
    {
        int output = 0;

        try { if (cells[y - 1][x].GetComponent<Cell>().currentStatus) { output++; } } catch { }
        try { if (cells[y + 1][x].GetComponent<Cell>().currentStatus) { output++; } } catch { }
        try { if (cells[y - 1][x - 1].GetComponent<Cell>().currentStatus) { output++; } } catch { }
        try { if (cells[y - 1][x + 1].GetComponent<Cell>().currentStatus) { output++; } } catch { }
        try { if (cells[y + 1][x - 1].GetComponent<Cell>().currentStatus) { output++; } } catch { }
        try { if (cells[y + 1][x + 1].GetComponent<Cell>().currentStatus) { output++; } } catch { }
        try { if (cells[y][x - 1].GetComponent<Cell>().currentStatus) { output++; } } catch { }
        try { if (cells[y][x + 1].GetComponent<Cell>().currentStatus) { output++; } } catch { }

        return output;
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