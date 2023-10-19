using System.Collections;
using System.Collections.Generic;
using Unity.Jobs;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UIElements;

public class GameManagerGPU : MonoBehaviour
{

    [Header("Gameobjects")]
    public ComputeShader setCurrentTexture;
    public ComputeShader setPreTexture;
    public ComputeShader toggleCellState;

    public RenderTexture currentTexture;
    public RenderTexture lastTexture;

    [Header("Player Interactions")]
    public bool paint;
    public bool alive;

    public float radius;

    [Header("Game Data")]
    public int simSteps;   

    [Header("Private states")]
    private int textureWidth = 3840;
    private int textureHeight = 2160;
    private float screenAdjustX;
    private float screenAdjustY;

    private int coreGroupSize = 16;
    private bool beginSim;
    private bool stepCalled;

    public Vector2 scale;
    private Vector2 offset;

    private float lastScale = 1;
    private Vector2 lastOffset;

    private float mouseSensitivity = 0.1f;

    private IEnumerator coroutine;

    void Awake()
    {
        lastOffset.x = 0;
        lastOffset.y = 0;

        screenAdjustX = (float)textureWidth / (float)Screen.currentResolution.width;
        screenAdjustY = (float)textureHeight / (float) Screen.currentResolution.height;

        currentTexture = new RenderTexture(textureWidth, textureHeight, 0);
        currentTexture.enableRandomWrite = true;
        currentTexture.Create();

        lastTexture = new RenderTexture(textureWidth, textureHeight, 0);
        lastTexture.enableRandomWrite = true;
        lastTexture.Create();

        setCurrentTexture.SetTexture(0, "PreResult", lastTexture);
        setCurrentTexture.SetTexture(0, "Result", currentTexture);

        setPreTexture.SetTexture(0, "PreResult", lastTexture);
        setPreTexture.SetTexture(0, "Result", currentTexture);

        toggleCellState.SetTexture(0, "Result", currentTexture);
    }

    void Update()
    {
        scale.y = lastScale - (Input.mouseScrollDelta.y * mouseSensitivity);
        scale.y = Mathf.Clamp(scale.y, 0.5f, 1.0f);
        scale.x = scale.y;

        if (Input.GetKey(KeyCode.A))
        {
            offset.x = lastOffset.x - 0.01f;
        }

        if (Input.GetKey(KeyCode.D))
        {
            offset.x = lastOffset.x + 0.01f;
        }

        if (Input.GetKey(KeyCode.W))
        {
            offset.y = lastOffset.y + 0.01f;
        }

        if (Input.GetKey(KeyCode.S))
        {
            offset.y = lastOffset.y - 0.01f;
        }

        //offset.x = Mathf.Clamp(offset.x, (-textureWidth / 2), (textureWidth / 2));
        //offset.y = Mathf.Clamp(offset.y, (-textureHeight / 2), (textureHeight / 2));

        paint = Input.GetMouseButton(0);

        if (Input.GetKey(KeyCode.Escape)) { Application.Quit(); }
        if (Input.GetMouseButtonDown(1)) { alive = !alive; }
        if (Input.GetKeyDown(KeyCode.Q)) { beginSim = !beginSim; }

        if (beginSim && !stepCalled)
        {
            stepCalled = true;
            simStep();
        }

        lastScale = scale.y;
        lastOffset.x = offset.x;
        lastOffset.y = offset.y;
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

    private void OnGUI()
    {
        simSteps = (int)GUI.VerticalSlider(new Rect(25, (textureHeight / 2), 150, 300), simSteps, 1000.0f, 1.0f);
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        float mouseX = (Input.mousePosition.x + (offset.x * screenAdjustX)) * screenAdjustX;
        float mouseY = (Input.mousePosition.y + (offset.y * screenAdjustY)) * screenAdjustY;

        toggleCellState.SetBool("paint", paint);
        toggleCellState.SetBool("alive", alive);
        toggleCellState.SetFloat("radius", radius);
        toggleCellState.SetFloat("mousePosX", mouseX);
        toggleCellState.SetFloat("mousePosY", mouseY);

        toggleCellState.Dispatch(0, currentTexture.width / coreGroupSize, currentTexture.height / coreGroupSize, 1);
            
        Graphics.Blit(currentTexture, destination, scale, offset);
    }
}
