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
    private int textureWidth = 768;
    private int textureHeight = 768;

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

        if (Input.GetMouseButtonDown(1))
        {
            alive = !alive;
        }
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        long start = System.DateTime.Now.Ticks;

        toggleCellState.SetBool("paint", paint);
        toggleCellState.SetBool("alive", alive);
        toggleCellState.SetFloat("radius", radius);

        toggleCellState.SetFloat("mousePosX", Input.mousePosition.x);
        toggleCellState.SetFloat("mousePosY", Input.mousePosition.y);

        setCurrentTexture.Dispatch(0, currentTexture.width / 8, currentTexture.height / 8, 1);

        toggleCellState.Dispatch(0, currentTexture.width / 8, currentTexture.height / 8, 1);

        setPreTexture.Dispatch(0, lastTexture.width / 8, lastTexture.height / 8, 1);

        Graphics.Blit(currentTexture, destination);
    }
}
