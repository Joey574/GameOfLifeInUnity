using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

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
    public int textureWidth = 512;
    public int textureHeight = 512;
    public int simSteps = 15;

    private int coreGroupSize = 16;
    private bool beginSim;
    private bool stepCalled;

    void Awake()
    {
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
        paint = Input.GetMouseButton(0);

        if (Input.GetMouseButtonDown(1)) { alive = !alive; }

        if (Input.GetKeyDown(KeyCode.Q)) { beginSim = !beginSim; }

        if (beginSim && !stepCalled)
        {
            stepCalled = true;
            Invoke(nameof(simStep), (1.0f / simSteps));
        }
    }

    private void simStep()
    {
        setPreTexture.Dispatch(0, lastTexture.width / coreGroupSize, lastTexture.height / coreGroupSize, 1);
        setCurrentTexture.Dispatch(0, currentTexture.width / coreGroupSize, currentTexture.height / coreGroupSize, 1);

        stepCalled = false;
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        toggleCellState.SetBool("paint", paint);
        toggleCellState.SetBool("alive", alive);
        toggleCellState.SetFloat("radius", radius);
        toggleCellState.SetFloat("mousePosX", Input.mousePosition.x);
        toggleCellState.SetFloat("mousePosY", Input.mousePosition.y);

        toggleCellState.Dispatch(0, currentTexture.width / coreGroupSize, currentTexture.height / coreGroupSize, 1);
            
        Graphics.Blit(currentTexture, destination);
    }
}
