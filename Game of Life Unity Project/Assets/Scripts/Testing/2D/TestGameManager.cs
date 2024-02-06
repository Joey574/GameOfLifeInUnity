using System.Collections;
using System.Threading;
using UnityEngine;
using UnityEngine.UIElements;

public class TestGameManager : MonoBehaviour
{
    [Header("Public Scripts")]
    public ComputeShader setCurrentTexture;
    public ComputeShader toggleCellState;
    public ComputeShader setColor;
    public ComputeShader randFill;

    private RenderTexture currentTexture;
    private RenderTexture lastTexture;
    protected bool current = true;

    [Header("Player Interactions")]
    public bool paint;
    public bool alive;

    public float radius;

    [Header("Game Data")]
    public int textureWidth;
    public int textureHeight;
    public int simSteps = 0;
    protected Color liveCell;

    [Header("Private states")]

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

    private bool shouldQuit = false;

    int times = 8;

    [Header("GUI Adjustments")]
    protected Vector2 scale;
    protected Vector2 offset;

    protected float lastScale = 1;
    protected Vector2 lastOffset;

    protected float offsetInc = 0.5f;
    protected float zoomSensitivity = 0.05f;
    protected float radiusInc = 5;

    protected float zoomMin = 0.01f;
    protected float zoomMax = 1.0f;

    [Header("Multithreading Values")]
    protected Thread handleAdjustmentsThread;
    protected ESCMenu escMenu;

    protected IEnumerator coroutine;

    protected void inputHandler()
    {
        if (Input.mouseScrollDelta.y != 0 && Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                radius += radiusInc;
            }
            else
            {
                radius -= radiusInc;
            }

            if (radius <= 0)
            {
                radius = 1;
            }

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
            StartCoroutine(callMenu());
        }
        if (Input.GetMouseButtonDown(1)) { alive = !alive; Color color = alive ? liveCell : Color.black; toggleCellState.SetVector("color", color); }
        if (Input.GetKeyDown(KeyCode.Q)) { beginSim = !beginSim; }
    }

    protected void setCellColor()
    {
        setCurrentTexture.SetVector("color", liveCell);
        toggleCellState.SetVector("color", liveCell);
        randFill.SetVector("color", liveCell);
    }

    protected void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!menuCalled)
        {
            if (paint)
            {
                float mouseX = Input.mousePosition.x * screenAdjustX * scale.x + (offset.x * textureWidth);
                float mouseY = Input.mousePosition.y * screenAdjustY * scale.y + (offset.y * textureHeight);

                if (current)
                {
                    toggleCellState.SetTexture(0, "Result", currentTexture);
                }
                else
                {
                    toggleCellState.SetTexture(0, "Result", lastTexture);
                }

                toggleCellState.SetInt("radius", (int)radius);
                toggleCellState.SetFloat("xPos", mouseX);
                toggleCellState.SetFloat("yPos", mouseY);

                toggleCellState.Dispatch(0, 1, 1, 1);
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
    }

    void Awake()
    {
        liveCell = Color.white;

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

        setCurrentTexture.SetTexture(0, "PreResult", lastTexture);
        setCurrentTexture.SetTexture(0, "Result", currentTexture);

        randFill.SetTexture(0, "Result", currentTexture);

        handleAdjustmentsThread = new Thread(() => handleAdjustements());
        handleAdjustmentsThread.Start();

        threadDispatchX = (Mathf.CeilToInt((float)currentTexture.width / (float)threadGroupSize)) / times;
        threadDispatchY = Mathf.CeilToInt((float)currentTexture.height / (float)threadGroupSize);

        setCurrentTexture.SetInt("scale", threadDispatchX * threadGroupSize);
        setCurrentTexture.SetInt("times", times);

        setCellColor();

        randFill.Dispatch(0, threadDispatchX * times, threadDispatchY, 1);

        toggleCellState.SetTexture(0, "Result", lastTexture);
        toggleCellState.SetTexture(0, "PreResult", lastTexture);

        beginSim = true;
    }

    void Update()
    {
        if (!menuCalled)
        {
            inputHandler();

            shouldUpdate = true;

            if (beginSim && !stepCalled && simSteps > 0)
            {
                stepCalled = true;
                current = !current;

                simStep();
            }
        }
    }

    protected void simStep()
    {
        coroutine = dispatchKernals(1.0f / simSteps);
        StartCoroutine(coroutine);
    }

    protected IEnumerator dispatchKernals(float waitTime)
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

    protected IEnumerator callMenu()
    {
        yield return new WaitForSeconds(0);

        escMenu = gameObject.AddComponent<ESCMenu>();

        beginSim = false;
        menuCalled = true;
        escMenu.begin(gameObject.GetComponent<GameManagerTemplate>(), setColor, simSteps);
    }

    protected void handleAdjustements()
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

    public void setMenuCalled(bool menuCalled)
    {
        this.menuCalled = menuCalled;
    }

    public void setSimSteps(int simSteps)
    {
        this.simSteps = simSteps;
    }

    private void OnDestroy()
    {
        shouldQuit = true;
        handleAdjustmentsThread.Join();
    }
}