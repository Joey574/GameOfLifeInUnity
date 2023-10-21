using Mono.Cecil;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManagerClassic : MonoBehaviour
{

    [Header("Gameobjects")]
    public ComputeShader setCurrentTexture;
    public ComputeShader setPreTexture;
    public ComputeShader toggleCellState;
    public ComputeShader setColor;

    public RenderTexture currentTexture;
    public RenderTexture lastTexture;

    [Header("Player Interactions")]
    public bool paint;
    public bool alive;

    public float radius;

    [Header("Game Data")]
    public int simSteps;   

    [Header("Private states")]
    private int textureWidth;
    private int textureHeight;
    private float screenAdjustX;
    private float screenAdjustY;

    private int coreGroupSize = 8;
    private bool beginSim;
    private bool stepCalled;
    private bool menuCalled = false;
    private bool shouldUpdate;

    private Vector2 scale;
    private Vector2 offset;

    private float lastScale = 1;
    private Vector2 lastOffset;

    private float offsetInc = 0.005f;
    private float zoomSensitivity = 0.05f;
    private float radiusInc = 5;

    private float zoomMin = 0.01f;
    private float zoomMax = 1.0f;

    private Thread handleAdjustmentsThread;
    private ESCMenu escMenu;
    private GameValues gameValues;

    private IEnumerator coroutine;

    void Awake()
    {
        gameValues = GameObject.Find("gameValues").GetComponent<GameValues>();

        textureWidth = (int) gameValues.gameBoardSize.x;
        textureHeight = (int) gameValues.gameBoardSize.y;

        lastOffset.x = 0;
        lastOffset.y = 0;

        scale.x = 1;
        scale.y = 1;

        screenAdjustX = (float)textureWidth / (float)Screen.currentResolution.width;
        screenAdjustY = (float)textureHeight / (float) Screen.currentResolution.height;

        currentTexture = new RenderTexture(textureWidth, textureHeight, 0);
        currentTexture.enableRandomWrite = true;
        currentTexture.Create();

        currentTexture.filterMode = FilterMode.Point;

        lastTexture = new RenderTexture(textureWidth, textureHeight, 0);
        lastTexture.enableRandomWrite = true;
        lastTexture.Create();

        setCurrentTexture.SetTexture(0, "PreResult", lastTexture);
        setCurrentTexture.SetTexture(0, "Result", currentTexture);

        setPreTexture.SetTexture(0, "PreResult", lastTexture);
        setPreTexture.SetTexture(0, "Result", currentTexture);

        toggleCellState.SetTexture(0, "Result", currentTexture);

        handleAdjustmentsThread = new Thread(() => handleAdjustements());
        handleAdjustmentsThread.Start();

        Destroy(GameObject.Find("gameValues"));
    }

    void Update()
    {
        if (!menuCalled)
        {
            inputHandler();

            shouldUpdate = true;

            if (beginSim && !stepCalled)
            {
                stepCalled = true;
                simStep();
            }
        }
    }

    private void inputHandler()
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
            offset.x = lastOffset.x - (offsetInc * scale.x);
        }
        if (Input.GetKey(KeyCode.D))
        {
            offset.x = lastOffset.x + (offsetInc * scale.x);
        }
        if (Input.GetKey(KeyCode.W))
        {
            offset.y = lastOffset.y + (offsetInc * scale.y);
        }
        if (Input.GetKey(KeyCode.S))
        {
            offset.y = lastOffset.y - (offsetInc * scale.y);
        }

        paint = Input.GetMouseButton(0);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(callMenu());
        }
        if (Input.GetMouseButtonDown(1)) { alive = !alive; }
        if (Input.GetKeyDown(KeyCode.Q)) { beginSim = !beginSim; }
    }

    private void simStep()
    {
        coroutine = dispatchKernals(1.0f / simSteps);
        StartCoroutine(coroutine);
    }

    private IEnumerator dispatchKernals(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        setPreTexture.Dispatch(0, lastTexture.width / coreGroupSize, 
            lastTexture.height / coreGroupSize, 1);

        setCurrentTexture.Dispatch(0, currentTexture.width / coreGroupSize, 
            currentTexture.height / coreGroupSize, 1);

        stepCalled = false;
    }

    private IEnumerator callMenu() 
    {
        yield return new WaitForSeconds(0);

        escMenu = gameObject.AddComponent<ESCMenu>();

        beginSim = false;
        menuCalled = true;
        escMenu.begin(gameObject.GetComponent<GameManagerClassic>(), currentTexture, scale, offset, setColor, simSteps);
    }

    private void handleAdjustements()
    {
        while(true)
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

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!menuCalled)
        {
            if(paint)
            {
                float mouseX = Input.mousePosition.x * screenAdjustX * scale.x + (offset.x * textureWidth);
                float mouseY = Input.mousePosition.y * screenAdjustY * scale.y + (offset.y * textureHeight);

                toggleCellState.SetBool("paint", paint);
                toggleCellState.SetBool("alive", alive);
                toggleCellState.SetFloat("radius", radius);
                toggleCellState.SetFloat("mousePosX", mouseX);
                toggleCellState.SetFloat("mousePosY", mouseY);

                toggleCellState.Dispatch(0, currentTexture.width / coreGroupSize, currentTexture.height / coreGroupSize, 1);

            }
            Graphics.Blit(currentTexture, destination, scale, offset);
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
}
