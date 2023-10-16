using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;
using static UnityEngine.RuleTile.TilingRuleOutput;
using Unity.Jobs;
using Unity.Collections;

public class gameManagerScript : MonoBehaviour
{
    [Header("Gameobjects")]
    public GameObject gameManager;
    public GameObject canvas;
    public GameObject background;
    public Button startButton;
    public Button stopButton;

    private RectTransform canvasSize;

    [Header("Cell Info")]
    public GameObject cellPrefab;
    public int cellWidth = 10;
    public int cellHeight = 10;
    public float yOffset;

    [Header("Button Adjustments")]
    public float xButtonOffset;
    public float yButtonOffset;
    public float xStopButtonOffset;
    public float yStopButtonOffset;

    [Header("Sim info")]
    public bool randStart;
    public float fillValue;
    public float simSteps;
    public int generation = 0;
    public bool gpuCompute;

    private int Xcount;
    private int Ycount;
    private int count = 0;

    private List<GameObject> row;
    private List<List<GameObject>> cells;
    private Hashtable neighbors;

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
        neighbors = new Hashtable();

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
                t.x = x; t.y = y; t.index = count;
                count++;

                if (randStart)
                {
                    t.nextStatus = randVal();
                } 
                else
                {
                    t.currentStatus = false;
                }

                t.applyState();

                cells[y][x] = temp;
            }
        }      

        startButton.onClick.AddListener(() => beginSim = true);
        stopButton.onClick.AddListener(() => beginSim = false);

        setNeighbors();

        Debug.Log("Cells: " + (Xcount * Ycount));
    }

    private void setNeighbors()
    {
        int i = 0;

        for (int y = 0; y < Ycount; y++)
        {
            for (int x = 0; x < Xcount; x++, i++)
            {
                List<Cell> temp = new List<Cell>();

                try { temp.Add(cells[y][x - 1].GetComponent<Cell>()); } catch { }
                try { temp.Add(cells[y][x + 1].GetComponent<Cell>()); } catch { }
                try { temp.Add(cells[y - 1][x].GetComponent<Cell>()); } catch { }
                try { temp.Add(cells[y + 1][x].GetComponent<Cell>()); } catch { }
                try { temp.Add(cells[y + 1][x - 1].GetComponent<Cell>()); } catch { }
                try { temp.Add(cells[y + 1][x + 1].GetComponent<Cell>()); } catch { }
                try { temp.Add(cells[y - 1][x - 1].GetComponent<Cell>()); } catch { }
                try { temp.Add(cells[y - 1][x + 1].GetComponent<Cell>()); } catch { }

                neighbors.Add(i, temp);
            }
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) { beginSim = !beginSim; }

        if (beginSim && !stepCalled)
        {
            stepCalled = true;
            Invoke(nameof(simStep), (1.0f / simSteps));
        }

    }

    private void gpuSimStep()
    {

    }

    private void simStep()
    {
        long start = System.DateTime.Now.Ticks;

        generation++;

        calculateCells();

        updateCells();

        long end = System.DateTime.Now.Ticks;

        Debug.Log("Step time: " + ((end - start) / 1000) + " microseconds");

        stepCalled = false;
    }

    private void calculateCells()
    {
        for (int y = 0; y < Ycount; y++)
        {
            for (int x = 0; x < Xcount; x++)
            {
                Cell cell = cells[y][x].GetComponent<Cell>();
                int i = getNeighbors(cell.index);

                switch (i)
                {
                    case 2:
                        cell.nextStatus = cell.currentStatus;
                        break;
                    case 3:
                        cell.nextStatus = true;
                        break;
                    default:
                        cell.nextStatus = false;
                        break;
                }
            }
        }
    }

    private void updateCells()
    {
        foreach (var cell in cells)
        {
            foreach (GameObject c in cell)
            {
                c.GetComponent<Cell>().applyState();
            }
        }
    }

    public int getNeighbors(int i)
    {
        int output = 0;

        List<Cell> temp = new List<Cell>();
        temp = (List<Cell>) neighbors[i];

        for (int x = 0; x < temp.Count() && output < 4; x++)
        {
            if (temp[x].currentStatus) { output++; }
        }

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
        stopButton.transform.localPosition = new Vector3(-(canvasSize.rect.width / 2) + xStopButtonOffset,
            (canvasSize.rect.height / 2) - yStopButtonOffset, 0);
    }

    private bool randVal()
    {
        return (Random.Range(0f, 1.0f) < fillValue);
    }

}

/*
First we check if simStep has been called already, if not, call it

    SIMSTEP:
Get system time

calculateCells

updateCells

output time

    CALCULATE CELLS:


 */
