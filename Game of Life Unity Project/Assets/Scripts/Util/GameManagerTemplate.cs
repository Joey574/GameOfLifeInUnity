using System;
using System.Collections;
using System.Drawing;
using System.Threading;
using UnityEngine;

public abstract class GameManagerTemplate : MonoBehaviour
{
    [Header("Public Scripts")]
    public ComputeShader setCurrentTexture;
    public ComputeShader toggleCellState;
    public ComputeShader setColor;

    public RenderTexture currentTexture;
    public RenderTexture lastTexture;
    protected bool current = true;

    [Header("Player Interactions")]
    public bool paint;
    public bool alive;

    public int radius;

    [Header("Game Data")]
    public int simSteps = 0;
    protected UnityEngine.Color liveCell;

    [Header("Private states")]
    protected int textureWidth;
    protected int textureHeight;
    protected float screenAdjustX;
    protected float screenAdjustY;

    [Header("Kernal Values")]
    protected int threadGroupSize = 8;
    protected int threadDispatchX;
    protected int threadDispatchY;

    [Header("Internal States")]
    protected bool beginSim;
    protected bool stepCalled;
    protected bool shouldUpdate;
    protected bool menuCalled = false;
    protected UnityEngine.Color newColor;

    private bool shouldQuit = false;

    [Header("GUI Adjustments")]
    protected Vector2 scale;
    protected Vector2 offset;

    protected float lastScale = 1;
    protected Vector2 lastOffset;

    protected float offsetInc = 0.5f;
    protected float zoomSensitivity = 0.05f;
    protected int radiusInc = 5;

    protected float zoomMin = 0.01f;
    protected float zoomMax = 1.0f;

    [Header("Multithreading Values")]
    protected Thread handleAdjustmentsThread;
    protected ESCMenu escMenu;
    protected GameValues gameValues;

    protected IEnumerator coroutine;

    protected abstract void SetCellColor();

    protected abstract void ToggleDrawState();

    void Awake()
    {
        gameValues = GameObject.Find("gameValues").GetComponent<GameValues>();

        textureWidth = (int)gameValues.gameBoardSize.x;
        textureHeight = (int)gameValues.gameBoardSize.y;

        liveCell = gameValues.liveCell;

        lastOffset.x = 0;
        lastOffset.y = 0;

        scale.x = 1;
        scale.y = 1;

        screenAdjustX = (float)textureWidth / (float)Screen.currentResolution.width;
        screenAdjustY = (float)textureHeight / (float)Screen.currentResolution.height;

        currentTexture = new RenderTexture(textureWidth, textureHeight, 0);
        currentTexture.enableRandomWrite = true;
        currentTexture.Create();

        lastTexture = new RenderTexture(textureWidth, textureHeight, 0);
        lastTexture.enableRandomWrite = true;
        lastTexture.Create();

        currentTexture.filterMode = FilterMode.Point;
        lastTexture.filterMode = FilterMode.Point;

        currentTexture.antiAliasing = 1;
        lastTexture.antiAliasing = 1;

        handleAdjustmentsThread = new Thread(() => HandleAdjustements());
        handleAdjustmentsThread.Start();

        threadDispatchX = Mathf.CeilToInt((float)currentTexture.width / (float)threadGroupSize);
        threadDispatchY = Mathf.CeilToInt((float)currentTexture.height / (float)threadGroupSize);

        SetCellColor();

        toggleCellState.SetInt("radius", radius);

        toggleCellState.SetTexture(0, "Result", currentTexture);
        toggleCellState.SetTexture(0, "PreResult", lastTexture);

        Destroy(GameObject.Find("gameValues"));
    }

    void Update()
    {
        if (!menuCalled)
        {
            InputHandler();

            shouldUpdate = true;

            if (beginSim && !stepCalled && simSteps > 0)
            {
                stepCalled = true;
                current = !current;

                SimStep();
            }
        }
    }

    protected void SimStep()
    {
        coroutine = DispatchKernals(1.0f / simSteps);
        StartCoroutine(coroutine);
    }

    protected IEnumerator DispatchKernals(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (current)
        {
            setCurrentTexture.SetTexture(0, "PreResult", lastTexture);
            setCurrentTexture.SetTexture(0, "Result", currentTexture);
        }
        else
        {
            setCurrentTexture.SetTexture(0, "PreResult", currentTexture);
            setCurrentTexture.SetTexture(0, "Result", lastTexture);
        }
        
        setCurrentTexture.Dispatch(0, threadDispatchX,
            threadDispatchY, 1);

        stepCalled = false;  
    }

    protected IEnumerator CallMenu()
    {
        yield return new WaitForSeconds(0);

        escMenu = gameObject.AddComponent<ESCMenu>();

        beginSim = false;
        menuCalled = true;
        escMenu.begin(gameObject.GetComponent<GameManagerTemplate>(), setColor, simSteps);
    }

    protected void HandleAdjustements()
    {
        while (!shouldQuit)
        {
            if (shouldUpdate)
            {
                scale.x = Mathf.Clamp(scale.x, zoomMin, zoomMax);
                scale.y = Mathf.Clamp(scale.y, zoomMin, zoomMax);

                if (scale.y != lastScale)
                {
                    offset.x += (lastScale - scale.x) / 2;
                    offset.y += (lastScale - scale.y) / 2;
                }

                offset.x = Mathf.Clamp(offset.x, 0, (-scale.x + 1));
                offset.y = Mathf.Clamp(offset.y, 0, (-scale.y + 1));

                lastScale = scale.y;
                lastOffset.x = offset.x;
                lastOffset.y = offset.y;

                shouldUpdate = false;
            }
        }
    }

    protected void InputHandler()
    {
        if (Input.mouseScrollDelta.y != 0 && Input.GetKey(KeyCode.LeftShift))
        {
            radius = Input.mouseScrollDelta.y > 0 ? radius += radiusInc : radius -= radiusInc;
            radius = Mathf.Clamp(radius, 0, radius + 1);

            toggleCellState.SetInt("radius", radius);
        }
        else if (Input.mouseScrollDelta.y != 0)
        {
            scale.y = lastScale - (Input.mouseScrollDelta.y * zoomSensitivity);
            scale.x = lastScale - (Input.mouseScrollDelta.y * zoomSensitivity);
        }

        if (Input.GetKey(KeyCode.A))
        {
            offset.x = lastOffset.x - (offsetInc * scale.x) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            offset.x = lastOffset.x + (offsetInc * scale.x) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W))
        {
            offset.y = lastOffset.y + (offsetInc * scale.y) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            offset.y = lastOffset.y - (offsetInc * scale.y) * Time.deltaTime;
        }

        paint = Input.GetMouseButton(0);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(CallMenu());
        }

        if (Input.GetKeyDown(KeyCode.Q)) { beginSim = !beginSim; }

        if (Input.GetMouseButtonDown(1))
        {
            ToggleDrawState();
            toggleCellState.SetVector("color", newColor);
        }
    }

    protected void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!menuCalled)
        {
            if (paint)
            {
                float mouseX = Input.mousePosition.x * screenAdjustX * scale.x + (offset.x * textureWidth);
                float mouseY = Input.mousePosition.y * screenAdjustY * scale.y + (offset.y * textureHeight);

                toggleCellState.SetInt("xPos", (int)mouseX);
                toggleCellState.SetInt("yPos", (int)mouseY);

                toggleCellState.Dispatch(0, 1, 1, 1);              
            }
        }
        if (current)
        {
            Graphics.Blit(currentTexture, destination, scale, offset);
        }
        else
        {
            Graphics.Blit(lastTexture, destination, scale, offset);
        }
    }

    public void SetMenuCalled(bool menuCalled)
    {
        this.menuCalled = menuCalled;
    }

    public void SetSimSteps(int simSteps)
    {
        this.simSteps = simSteps;
    }

    private void OnDestroy()
    {
        shouldQuit = true;
        handleAdjustmentsThread.Join();
    }
}
