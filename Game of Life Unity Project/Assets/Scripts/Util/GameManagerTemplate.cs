using System.Collections;
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

    public float radius;

    [Header("Game Data")]
    public int simSteps = 0;
    protected Color liveCell;

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

    private bool shouldQuit = false;

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
    protected GameValues gameValues;

    protected IEnumerator coroutine;

    protected abstract void setCellColor();

    protected abstract void inputHandler();

    protected abstract void OnRenderImage(RenderTexture source, RenderTexture destination);

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

        handleAdjustmentsThread = new Thread(() => handleAdjustements());
        handleAdjustmentsThread.Start();

        threadDispatchX = Mathf.CeilToInt((float)currentTexture.width / (float)threadGroupSize);
        threadDispatchY = Mathf.CeilToInt((float)currentTexture.height / (float)threadGroupSize);

        setCellColor();

        Destroy(GameObject.Find("gameValues"));
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
        escMenu.begin(gameObject.GetComponent<GameManagerTemplate>(), currentTexture, scale, offset, setColor, simSteps);
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
