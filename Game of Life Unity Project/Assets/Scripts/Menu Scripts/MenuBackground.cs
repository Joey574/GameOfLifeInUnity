using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuBackground : MonoBehaviour
{
    [Header("Public Scripts")]
    public ComputeShader setPreTexture;
    public ComputeShader setColor;

    public ComputeShader setCurrentTextureTopLeft;
    public ComputeShader setCurrentTextureTopRight;
    public ComputeShader setCurrentTextureBottomLeft;
    public ComputeShader setCurrentTextureBottomRight;

    public List<ComputeShader> Brushes;

    [Header("Render Textures")]
    private RenderTexture topLeftCurrent;
    private RenderTexture topRightCurrent;
    private RenderTexture bottomLeftCurrent;
    private RenderTexture bottomRightCurrent;

    private RenderTexture topLeftLast;
    private RenderTexture topRightLast;
    private RenderTexture bottomLeftLast;
    private RenderTexture bottomRightLast;

    private RenderTexture backgroundTexture;

    [Header("Private Variables")]
    private Vector2 gameSize;
    private Vector2 screen;
    private Rect TopLeft, TopRight, BottomLeft, BottomRight;
    private int threadGroup = 8;
    private int threadDispatchX;
    private int threadDispatchY;


    private bool initialized = false;
    private bool backgroundDrawn = false;

    void Awake()
    {
        screen.x = Screen.currentResolution.width;
        screen.y = Screen.currentResolution.height;

        gameSize.x = screen.x / 8;
        gameSize.y = screen.y / 8;

        initializeTextures();

        initializeLocations();

        initializeKernals();

        threadDispatchX = Mathf.CeilToInt(TopLeft.width / threadGroup);
        threadDispatchY = Mathf.CeilToInt(TopLeft.height / threadGroup);

        initialized = true;
    }

    private void initializeTextures()
    {
        topLeftCurrent = new RenderTexture((int)gameSize.x, (int)gameSize.y, 0);
        topRightCurrent = new RenderTexture((int)gameSize.x, (int)gameSize.y, 0);
        topLeftLast = new RenderTexture(topLeftCurrent.width, topRightCurrent.height, 0);
        topRightLast = new RenderTexture(topRightCurrent.width, topRightCurrent.height, 0);

        bottomLeftCurrent = new RenderTexture((int)gameSize.x, (int)gameSize.y, 0);
        bottomRightCurrent = new RenderTexture((int)gameSize.x, (int)gameSize.y, 0);
        bottomLeftLast = new RenderTexture(bottomLeftCurrent.width, bottomLeftCurrent.height, 0);
        bottomRightLast = new RenderTexture(bottomRightCurrent.width, bottomRightCurrent.height, 0);

        topLeftCurrent.enableRandomWrite = true; topRightCurrent.enableRandomWrite = true;
        bottomLeftCurrent.enableRandomWrite = true; bottomRightCurrent.enableRandomWrite = true;
        topLeftLast.enableRandomWrite = true; topRightLast.enableRandomWrite = true;
        bottomLeftLast.enableRandomWrite = true; bottomRightLast.enableRandomWrite = true;

        topLeftCurrent.Create(); topRightCurrent.Create(); bottomLeftCurrent.Create(); bottomRightCurrent.Create();
        topLeftLast.Create(); topRightLast.Create();bottomLeftLast.Create(); bottomRightLast.Create();

        topLeftCurrent.filterMode = FilterMode.Point; topRightCurrent.filterMode = FilterMode.Point; bottomLeftCurrent.filterMode = FilterMode.Point; bottomRightCurrent.filterMode = FilterMode.Point;
        topLeftLast.filterMode = FilterMode.Point; topRightLast.filterMode = FilterMode.Point; bottomLeftLast.filterMode = FilterMode.Point; bottomRightLast.filterMode = FilterMode.Point;

        backgroundTexture  = new RenderTexture(8, 8, 1);
        backgroundTexture.enableRandomWrite = true;
        backgroundTexture.Create();

        setColor.SetVector("color", new Color(0.1f, 0.1f, 0.1f, 1));
        setColor.SetTexture(0, "Result", backgroundTexture);

        setColor.Dispatch(0, 1, 1, 1);

        Brushes[0].SetTexture(0, "Result", bottomRightCurrent);
    }

    private void initializeLocations()
    {
        TopLeft.x = 0; TopLeft.y = 0; TopLeft.width = screen.x / 2 - 1; TopLeft.height = screen.y / 2 - 1;
        TopRight.x = screen.x / 2; TopRight.y = 0; TopRight.width = TopLeft.width; TopRight.height = TopLeft.height;

        BottomLeft.x = 0; BottomLeft.y = screen.y / 2; BottomLeft.width = TopLeft.width; BottomLeft.height = TopLeft.height;
        BottomRight.x = screen.x / 2; BottomRight.y = screen.y / 2; BottomRight.width = TopLeft.width; BottomRight.height = TopLeft.height;
    }

    private void initializeKernals()
    {
        setCurrentTextureTopLeft.SetTexture(0, "Result", topLeftCurrent);
        setCurrentTextureTopLeft.SetTexture(0, "PreResult", topLeftLast);
        setCurrentTextureTopLeft.SetVector("color", Color.white);

        setCurrentTextureTopRight.SetTexture(0, "Result", topRightCurrent);
        setCurrentTextureTopRight.SetTexture(0, "PreResult", topRightLast);

        setCurrentTextureBottomLeft.SetTexture(0, "Result", bottomLeftCurrent);
        setCurrentTextureBottomLeft.SetTexture(0, "PreResult", bottomLeftLast);

        setCurrentTextureBottomRight.SetTexture(0, "Result", bottomRightCurrent);
        setCurrentTextureBottomRight.SetTexture(0, "PreResult", bottomRightLast);
    }

    private void dispatchKernals()
    {
        setPreTexture.SetTexture(0, "Result", topLeftCurrent);
        setPreTexture.SetTexture(0, "PreResult", topLeftLast);

        setPreTexture.Dispatch(0, threadDispatchX,
           threadDispatchY, 1);

        setCurrentTextureTopLeft.Dispatch(0, threadDispatchX,
           threadDispatchY, 1);

        setPreTexture.SetTexture(0, "Result", topRightCurrent);
        setPreTexture.SetTexture(0, "PreResult", topRightLast);

        setPreTexture.Dispatch(0, threadDispatchX,
           threadDispatchY, 1);

        setCurrentTextureTopRight.Dispatch(0, threadDispatchX,
           threadDispatchY, 1);

        setPreTexture.SetTexture(0, "Result", bottomLeftCurrent);
        setPreTexture.SetTexture(0, "PreResult", bottomLeftLast);

        setPreTexture.Dispatch(0, threadDispatchX,
           threadDispatchY, 1);

        setCurrentTextureBottomLeft.Dispatch(0, threadDispatchX,
          threadDispatchY, 1);

        setPreTexture.SetTexture(0, "Result", bottomRightCurrent);
        setPreTexture.SetTexture(0, "PreResult", bottomRightLast);

        setPreTexture.Dispatch(0, threadDispatchX,
           threadDispatchY, 1);

        setCurrentTextureBottomRight.Dispatch(0, threadDispatchX,
           threadDispatchY, 1);

        Brushes[0].SetFloat("x", gameSize.x);
        Brushes[0].SetFloat("y", gameSize.y);
        Brushes[0].Dispatch(0, 1, 1, 1);
    }

    public void DrawGUI()
    {
        if (!backgroundDrawn)
        {
            GUI.DrawTexture(new Rect(0, 0, screen.x, screen.y), backgroundTexture, ScaleMode.StretchToFill);
            backgroundDrawn = true;
        }

        if (initialized)
        {
            dispatchKernals();

            GUI.DrawTexture(TopLeft, topLeftCurrent, ScaleMode.ScaleToFit);
            GUI.DrawTexture(TopRight, topRightCurrent, ScaleMode.ScaleToFit);
            GUI.DrawTexture(BottomLeft, bottomLeftCurrent, ScaleMode.ScaleToFit);
            GUI.DrawTexture(BottomRight, bottomRightCurrent, ScaleMode.ScaleToFit);
        }
    }
}
