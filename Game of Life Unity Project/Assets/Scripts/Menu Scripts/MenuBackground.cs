using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using System;

public class MenuBackground : MonoBehaviour
{
    [Header("Public Scripts")]
    public ComputeShader setColor;

    public ComputeShader setCurrentTextureTopLeft;
    public ComputeShader setCurrentTextureTopRight;
    public ComputeShader setCurrentTextureBottomLeft;
    public ComputeShader setCurrentTextureBottomRight;

    public ComputeShader setLastTexture;
    public ComputeShader rleWrite;

    public List<ComputeShader> ClassicBrushes;
    public List<ComputeShader> InfectionBrushes;
    public List<ComputeShader> BattleBrushes;
    public List<ComputeShader> WireworldBrushes;

    public List<ComputeShader> Shapes;

    public RLEDispatch rleDispatch;

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

    [Header("Public Adjustments")]
    public int simSteps = 0;

    [Header("Private Variables")]
    private Vector2 gameSize;
    private Vector2 screen;
    private Rect TopLeft, TopRight, BottomLeft, BottomRight;
    private int threadGroup = 8;
    private int threadDispatchX;
    private int threadDispatchY;

    private bool initialized = false;
    private bool stepCalled = false;

    public RenderTexture before;
    public RenderTexture after;

    void Awake()
    {
        screen.x = Screen.width;
        screen.y = Screen.height;

        gameSize.x = 106;
        gameSize.y = 66;

        rleDispatch = new RLEDispatch();

        initializeTextures();

        initializeLocations();

        initializeKernals();

        threadDispatchX = Mathf.CeilToInt(TopLeft.width / threadGroup);
        threadDispatchY = Mathf.CeilToInt(TopLeft.height / threadGroup);

        initialized = true;

        IEnumerator coroutine = dispatchSquare(127f / simSteps);
        StartCoroutine(coroutine);
    }
   
    private void initializeTextures()
    {
        // Top textures, x, y, width and height
        topLeftCurrent = new RenderTexture((int)gameSize.x, (int)gameSize.y, 0);
        topRightCurrent = new RenderTexture((int)gameSize.x, (int)gameSize.y, 0);
        topLeftLast = new RenderTexture(topLeftCurrent.width, topRightCurrent.height, 0);
        topRightLast = new RenderTexture(topRightCurrent.width, topRightCurrent.height, 0);

        // Bottom textures, x, y, width and height
        bottomLeftCurrent = new RenderTexture((int)gameSize.x, (int)gameSize.y, 0);
        bottomRightCurrent = new RenderTexture((int)gameSize.x, (int)gameSize.y, 0);
        bottomLeftLast = new RenderTexture(bottomLeftCurrent.width, bottomLeftCurrent.height, 0);
        bottomRightLast = new RenderTexture(bottomRightCurrent.width, bottomRightCurrent.height, 0);

        // Enable random write
        topLeftCurrent.enableRandomWrite = true; topRightCurrent.enableRandomWrite = true;
        bottomLeftCurrent.enableRandomWrite = true; bottomRightCurrent.enableRandomWrite = true;
        topLeftLast.enableRandomWrite = true; topRightLast.enableRandomWrite = true;
        bottomLeftLast.enableRandomWrite = true; bottomRightLast.enableRandomWrite = true;

        // Create textures
        topLeftCurrent.Create(); topRightCurrent.Create(); bottomLeftCurrent.Create(); bottomRightCurrent.Create();
        topLeftLast.Create(); topRightLast.Create();bottomLeftLast.Create(); bottomRightLast.Create();

        // Change filter mode
        topLeftCurrent.filterMode = FilterMode.Point; topRightCurrent.filterMode = FilterMode.Point; bottomLeftCurrent.filterMode = FilterMode.Point; bottomRightCurrent.filterMode = FilterMode.Point;
        topLeftLast.filterMode = FilterMode.Point; topRightLast.filterMode = FilterMode.Point; bottomLeftLast.filterMode = FilterMode.Point; bottomRightLast.filterMode = FilterMode.Point;

        // Change antiAliasing
        topLeftCurrent.antiAliasing = 1;topLeftLast.antiAliasing = 1; topRightCurrent.antiAliasing = 1; topRightLast.antiAliasing = 1;
        bottomLeftCurrent.antiAliasing = 1;bottomRightLast.antiAliasing = 1; bottomLeftCurrent.antiAliasing= 1; bottomLeftLast.antiAliasing = 1;

        // Default background texture
        backgroundTexture  = new RenderTexture(8, 8, 1);
        backgroundTexture.enableRandomWrite = true;
        backgroundTexture.Create();
    }

    private void initializeLocations()
    {
        // location and size values of the different games

        // Top left values
        TopLeft.x = 1; TopLeft.y = 1; 
        TopLeft.width = screen.x / 2 - 2;
        TopLeft.height = screen.y / 2 - 2;

        // Top right values
        TopRight.x = screen.x / 2 + 1; TopRight.y = 1; 
        TopRight.width = TopLeft.width; 
        TopRight.height = TopLeft.height;

        // Bottom left values
        BottomLeft.x = 1; BottomLeft.y = screen.y / 2 + 1; 
        BottomLeft.width = TopLeft.width; 
        BottomLeft.height = TopLeft.height;

        // Bottom right values
        BottomRight.x = screen.x / 2; BottomRight.y = screen.y / 2 + 1; 
        BottomRight.width = TopLeft.width; 
        BottomRight.height = TopLeft.height;
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

        WireworldBrushes[0].SetTexture(0, "Result", bottomRightCurrent);
        WireworldBrushes[1].SetTexture(0, "Result", bottomRightCurrent);
        WireworldBrushes[2].SetTexture(0, "Result", bottomRightCurrent);
        WireworldBrushes[3].SetTexture(0, "Result", bottomRightCurrent);

        initializeBackground();
    }

    private void initializeBackground()
    {
        setColor.SetVector("color", new Color(0.1f, 0.1f, 0.1f, 1));
        setColor.SetTexture(0, "Result", backgroundTexture);

        setColor.Dispatch(0, 1, 1, 1);

        initializeClassic();

        initializeInfection();

        initializeWireworld();
    }

    private void initializeClassic()
    {
        // Set background to black first
        setColor.SetTexture(0, "Result", topLeftCurrent);
        setColor.SetVector("color", new Color(0, 0, 0, 1));
        setColor.Dispatch(0, topLeftCurrent.width / threadGroup, topLeftCurrent.height / threadGroup, 1);

        // left to right ships

        rleDispatch.CreateArray("RLEPatterns/1beacon");

        topLeftCurrent = rleDispatch.DispatchKernal(rleWrite, topLeftCurrent, Color.white, new Vector2(30, gameSize.y - 5), new bool2(true, false));

        rleDispatch.CreateArray("RLEPatterns/1beacon");
        topLeftCurrent = rleDispatch.DispatchKernal(rleWrite, topLeftCurrent, Color.white, new Vector2(30, gameSize.y - 25), new bool2(true, false));

        rleDispatch.CreateArray("RLEPatterns/1beacon");
        topLeftCurrent = rleDispatch.DispatchKernal(rleWrite, topLeftCurrent, Color.white, new Vector2(30, gameSize.y - 45), new bool2(true, false));

        // right to left ships

        rleDispatch.CreateArray("RLEPatterns/1beacon");
        topLeftCurrent = rleDispatch.DispatchKernal(rleWrite, topLeftCurrent, Color.white, new Vector2(gameSize.x - 35, gameSize.y - 5), new bool2(false, false));

        rleDispatch.CreateArray("RLEPatterns/1beacon");
        topLeftCurrent = rleDispatch.DispatchKernal(rleWrite, topLeftCurrent, Color.white, new Vector2(gameSize.x - 35, gameSize.y - 25), new bool2(true, false));

        rleDispatch.CreateArray("RLEPatterns/1beacon");
        topLeftCurrent = rleDispatch.DispatchKernal(rleWrite, topLeftCurrent, Color.white, new Vector2(gameSize.x - 35, gameSize.y - 45), new bool2(true, false));
    }

    private void initializeInfection()
    {
        // Set background to black first
        setColor.SetTexture(0, "Result", topRightCurrent);
        setColor.SetVector("color", new Color(0, 0, 0, 1));
        setColor.Dispatch(0, topLeftCurrent.width / threadGroup, topRightCurrent.height / threadGroup, 1);

        // Initialize Space rakes
        rleDispatch.CreateArray("RLEPatterns/1beacon");
        topRightCurrent = rleDispatch.DispatchKernal(rleWrite, topRightCurrent, Color.green, new Vector2(2, gameSize.y / 2 + 10), new bool2(false, true));

        rleDispatch.CreateArray("RLEPatterns/1beacon");
        topRightCurrent = rleDispatch.DispatchKernal(rleWrite, topRightCurrent, Color.green, new Vector2(2, gameSize.y / 2 - 10), new bool2(false, false));

        // Initialize bunnies
        rleDispatch.CreateArray("RLEPatterns/1beacon");

        Unity.Mathematics.Random rn = new Unity.Mathematics.Random();
        rn.state = (uint)DateTime.Now.Ticks;

        int num = rn.NextInt(3, 8);

        for (int i = 0; i < num; i++)
        {
            int x = rn.NextInt((int)gameSize.x / 2, (int)gameSize.x - 10);
            int y = rn.NextInt(10, (int)gameSize.y - 10);

            topRightCurrent = rleDispatch.DispatchKernal(rleWrite, topRightCurrent, Color.white, new Vector2(x, y), new bool2(false, false));
        }
    }

    private void initializeWireworld()
    {
        // Set background to black first
        setColor.SetTexture(0, "Result", bottomRightCurrent);
        setColor.SetVector("color", new Color(0, 0, 0, 1));
        setColor.Dispatch(0, topLeftCurrent.width / threadGroup, topLeftCurrent.height / threadGroup, 1);

        // Set up first 3 Nand gates
        for (int i = -15; i < 16; i = i + 15)
        {
            WireworldBrushes[0].SetFloat("xPos", 15);
            WireworldBrushes[0].SetFloat("yPos", (gameSize.y / 2) + i);
            WireworldBrushes[0].Dispatch(0, 1, 1, 1);
        }

        // Set up 3 top repeaters for the Nand gates
        for (int i = -15; i < 16; i = i + 15)
        {
            WireworldBrushes[1].SetFloat("xPos", 5);
            WireworldBrushes[1].SetFloat("yPos", (gameSize.y / 2) + i + 3);
            WireworldBrushes[1].Dispatch(0, 1, 1, 1);
        }

        // Set up 3 bottom repeaters for the Nand gates
        for (int i = -15; i < 16; i = i + 15)
        {
            WireworldBrushes[1].SetFloat("xPos", 5);
            WireworldBrushes[1].SetFloat("yPos", (gameSize.y / 2) + i - 3);
            WireworldBrushes[1].Dispatch(0, 1, 1, 1);
        }

        // Set length of line for line tool
        WireworldBrushes[2].SetInt("len", 4);

        // Set up lines connecting top repeaters and Nand gates
        for (int i = -15; i < 16; i = i + 15)
        {
            WireworldBrushes[2].SetFloat("xPos", 12);
            WireworldBrushes[2].SetFloat("yPos", (gameSize.y / 2) + i + 3);
            WireworldBrushes[2].Dispatch(0, 1, 1, 1);
        }

        // Set up lines connecting bottom repeaters and Nand gates
        for (int i = -15; i < 16; i = i + 15)
        {
            WireworldBrushes[2].SetFloat("xPos", 12);
            WireworldBrushes[2].SetFloat("yPos", (gameSize.y / 2) + i - 3);
            WireworldBrushes[2].Dispatch(0, 1, 1, 1);
        }

        // Set length of line for line tool
        WireworldBrushes[2].SetInt("len", 8);

        // Set output lines for Nand gates
        for (int i = -15; i < 16; i = i + 15)
        {
            WireworldBrushes[2].SetFloat("xPos", 20);
            WireworldBrushes[2].SetFloat("yPos", (gameSize.y / 2) + i);
            WireworldBrushes[2].Dispatch(0, 1, 1, 1);
        }

        // Set signal to be right
        WireworldBrushes[3].SetBool("left", false);

        // Set signals
        for (int i = -15; i < 16; i = i + 15)
        {
            WireworldBrushes[3].SetFloat("xPos", 10);
            WireworldBrushes[3].SetFloat("yPos", (gameSize.y / 2) + i - 2);
            WireworldBrushes[3].Dispatch(0, 1, 1, 1);
        }

        // Set signals
        for (int i = -15; i < 16; i = i + 15)
        {
            WireworldBrushes[3].SetFloat("xPos", 10);
            WireworldBrushes[3].SetFloat("yPos", (gameSize.y / 2) + i + 4);
            WireworldBrushes[3].Dispatch(0, 1, 1, 1);
        }

        // Set signals to left
        WireworldBrushes[3].SetBool("left", true);

        // Set signals
        for (int i = -15; i < 16; i = i + 15)
        {
            WireworldBrushes[3].SetFloat("xPos", 7);
            WireworldBrushes[3].SetFloat("yPos", (gameSize.y / 2) + i + 2);
            WireworldBrushes[3].Dispatch(0, 1, 1, 1);
        }


        // initialize next set of nand gates
        WireworldBrushes[0].SetFloat("xPos", (gameSize.x / 3));
        WireworldBrushes[0].SetFloat("yPos", (gameSize.y / 2) + 8);
        WireworldBrushes[0].Dispatch(0, 1, 1, 1);

        // Set length of line for line tool
        WireworldBrushes[2].SetInt("len", 3);

        // Set input lines for the second set of Nand gates
        for (int i = 1; i < 15; i = i + 13)
        {
            WireworldBrushes[2].SetFloat("xPos", 28);
            WireworldBrushes[2].SetFloat("yPos", (gameSize.y / 2) + i);
            WireworldBrushes[2].Dispatch(0, 1, 1, 1);
        }

        // Set input lines for the second set of Nand gates
        for (int i = 2; i < 14; i = i + 11)
        {
            WireworldBrushes[2].SetFloat("xPos", 31);
            WireworldBrushes[2].SetFloat("yPos", (gameSize.y / 2) + i);
            WireworldBrushes[2].Dispatch(0, 1, 1, 1);
        }

        // Set length of line for line tool
        WireworldBrushes[2].SetInt("len", 1);

        WireworldBrushes[2].SetFloat("xPos", 34);
        WireworldBrushes[2].SetFloat("yPos", (gameSize.y / 2) + 3);
        WireworldBrushes[2].Dispatch(0, 1, 1, 1);

        WireworldBrushes[2].SetFloat("xPos", 35);
        WireworldBrushes[2].SetFloat("yPos", (gameSize.y / 2) + 12);
        WireworldBrushes[2].Dispatch(0, 1, 1, 1);

        WireworldBrushes[2].SetFloat("xPos", 34);
        WireworldBrushes[2].SetFloat("yPos", (gameSize.y / 2) + 13);
        WireworldBrushes[2].Dispatch(0, 1, 1, 1);

        // Set input lines for the second set of Nand gates
        for (int i = 4; i < 12; i = i + 7)
        {
            WireworldBrushes[2].SetFloat("xPos", 35);
            WireworldBrushes[2].SetFloat("yPos", (gameSize.y / 2) + i);
            WireworldBrushes[2].Dispatch(0, 1, 1, 1);
        }

        // Set input lines for the second set of Nand gates
        WireworldBrushes[2].SetFloat("xPos", 35);
        WireworldBrushes[2].SetFloat("yPos", (gameSize.y / 2) + 5);
        WireworldBrushes[2].Dispatch(0, 1, 1, 1);

        // Set up last Nand Gate
        WireworldBrushes[0].SetFloat("xPos", gameSize.x / 2);
        WireworldBrushes[0].SetFloat("yPos", gameSize.y / 2);
        WireworldBrushes[0].Dispatch(0, 1, 1, 1);

        // Set up input lines for the last Nand gate
        for (int i = 0; i < 11; i++)
        {
            WireworldBrushes[2].SetFloat("xPos", 28 + i);
            WireworldBrushes[2].SetFloat("yPos", 19 + i);
            WireworldBrushes[2].Dispatch(0, 1, 1, 1);
        }

        WireworldBrushes[2].SetInt("len", 15);

        // Set up input lines for the last Nand gate
        WireworldBrushes[2].SetFloat("xPos", gameSize.x / 2 - 14);
        WireworldBrushes[2].SetFloat("yPos", gameSize.y / 2 - 3);
        WireworldBrushes[2].Dispatch(0, 1, 1, 1);

        WireworldBrushes[2].SetInt("len", 10);

        // Set up input lines for the last Nand gate
        WireworldBrushes[2].SetFloat("xPos", gameSize.x / 2 - 9);
        WireworldBrushes[2].SetFloat("yPos", gameSize.y / 2 + 3);
        WireworldBrushes[2].Dispatch(0, 1, 1, 1);

        WireworldBrushes[2].SetInt("len", 1);

        // Set up input lines for the last Nand gate
        for (int i = 0; i < 4; i++)
        {
            WireworldBrushes[2].SetFloat("xPos", 40 + i);
            WireworldBrushes[2].SetFloat("yPos", 40 - i);
            WireworldBrushes[2].Dispatch(0, 1, 1, 1);
        }

        // Set up output lines for the last Nand gate
        WireworldBrushes[2].SetInt("len", 48);

        WireworldBrushes[2].SetFloat("xPos", gameSize.x / 2 + 5);
        WireworldBrushes[2].SetFloat("yPos", gameSize.y / 2);
        WireworldBrushes[2].Dispatch(0, 1, 1, 1);

        // Set up vertical output lines
        WireworldBrushes[2].SetInt("len", 1);
        for (int i = 0; i < 4; i++)
        {
            for (int q = 0; q < 8 + (i * 5); q++)
            {
                WireworldBrushes[2].SetFloat("xPos", gameSize.x / 2 + 18 + (i * 6));
                WireworldBrushes[2].SetFloat("yPos", gameSize.y / 2 + 1 + q);
                WireworldBrushes[2].Dispatch(0, 1, 1, 1);
            }
        }

        // Horizontal output lines
        for (int i = 0; i < 4; i++)
        {
            WireworldBrushes[2].SetInt("len", 23 + (i * 6) - (i * 5));

            WireworldBrushes[2].SetFloat("xPos", gameSize.x / 2 - 5 + (i * 5));
            WireworldBrushes[2].SetFloat("yPos", gameSize.y / 2 + 9 + (i * 5));
            WireworldBrushes[2].Dispatch(0, 1, 1, 1);
        }

        // Set up vertical output lines
        WireworldBrushes[2].SetInt("len", 1);
        for (int i = 0; i < 4; i++)
        {
            for (int q = 0; q < 25; q++)
            {
                WireworldBrushes[2].SetFloat("xPos", gameSize.x / 2 - 6 + (i * 5));
                WireworldBrushes[2].SetFloat("yPos", gameSize.y / 2 + 10 + (i * 5) + q);
                WireworldBrushes[2].Dispatch(0, 1, 1, 1);
            }
        }

        // Set up vertical output lines
        WireworldBrushes[2].SetInt("len", 1);
        for (int i = 0; i < 2; i++)
        {
            for (int q = 0; q < 35; q++)
            {
                WireworldBrushes[2].SetFloat("xPos", gameSize.x / 2 + 42 + (i * 6));
                WireworldBrushes[2].SetFloat("yPos", gameSize.y / 2 + 1 + q);
                WireworldBrushes[2].Dispatch(0, 1, 1, 1);
            }
        }

        for (int i = 0; i < 3; i++)
        {
            for (int q = 0; q < 8 + (i * 5); q++)
            {
                WireworldBrushes[2].SetFloat("xPos", gameSize.x / 2 + 10 + (i * 5));
                WireworldBrushes[2].SetFloat("yPos", gameSize.y / 2 - 1 - q);
                WireworldBrushes[2].Dispatch(0, 1, 1, 1);
            }
        }

        WireworldBrushes[2].SetInt("len", 17);

        for (int i = 0; i < 3; i++)
        {
            WireworldBrushes[2].SetFloat("xPos", gameSize.x / 2 - 7 + (i * 5));
            WireworldBrushes[2].SetFloat("yPos", gameSize.y / 2 - 9 - (i * 5));
            WireworldBrushes[2].Dispatch(0, 1, 1, 1);
        }

        WireworldBrushes[2].SetInt("len", 1);

        for (int i = 0; i < 3; i++)
        {
            for (int q = 0; q < 35; q++)
            {
                WireworldBrushes[2].SetFloat("xPos", gameSize.x / 2 - 8 + (i * 5));
                WireworldBrushes[2].SetFloat("yPos", gameSize.y / 2 - 10 - q - (i * 5));
                WireworldBrushes[2].Dispatch(0, 1, 1, 1);
            }
        }

        for (int i = 0; i < 5; i++)
        {
            for (int q = 0; q < 35; q++)
            {
                WireworldBrushes[2].SetFloat("xPos", gameSize.x / 2 + 27 + (i * 6));
                WireworldBrushes[2].SetFloat("yPos", gameSize.y / 2 - 1 - q);
                WireworldBrushes[2].Dispatch(0, 1, 1, 1);
            }
        }

    }

    private IEnumerator dispatchKernals(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        // Update last
        setLastTexture.SetTexture(0, "PreResult", topLeftLast);
        setLastTexture.SetTexture(0, "Result", topLeftCurrent);
        setLastTexture.Dispatch(0, threadDispatchX, threadDispatchY, 1);

        setLastTexture.SetTexture(0, "PreResult", topRightLast);
        setLastTexture.SetTexture(0, "Result", topRightCurrent);
        setLastTexture.Dispatch(0, threadDispatchX, threadDispatchY, 1);

        setLastTexture.SetTexture(0, "PreResult", bottomLeftLast);
        setLastTexture.SetTexture(0, "Result", bottomLeftCurrent);
        setLastTexture.Dispatch(0, threadDispatchX, threadDispatchY, 1);

        setLastTexture.SetTexture(0, "PreResult", bottomRightLast);
        setLastTexture.SetTexture(0, "Result", bottomRightCurrent);
        setLastTexture.Dispatch(0, threadDispatchX, threadDispatchY, 1);

        // Clasic Gamemode
        setCurrentTextureTopLeft.Dispatch(0, threadDispatchX,
          threadDispatchY, 1);

        setCurrentTextureTopLeft.Dispatch(0, threadDispatchX,
           threadDispatchY, 1);

        // Infection Gamemode
        setCurrentTextureTopRight.Dispatch(0, threadDispatchX,
           threadDispatchY, 1);

        // Battle Gamemode
        setCurrentTextureBottomLeft.Dispatch(0, threadDispatchX,
          threadDispatchY, 1);

        // Wireworld Gamemode
        setCurrentTextureBottomRight.Dispatch(0, threadDispatchX,
           threadDispatchY, 1);

        stepCalled = false;
    }

    private IEnumerator dispatchSquare(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        Unity.Mathematics.Random rn = new Unity.Mathematics.Random();
        rn.state = (uint)DateTime.Now.Ticks;

        Shapes[0].SetTexture(0, "Result", topLeftCurrent);
        Shapes[0].SetVector("color", Color.white);
        Shapes[0].SetInt("radius", 3);

        int num = rn.NextInt(15, 35);

       for (int i = 0; i < num; i++)
       {
            int x = rn.NextInt((int)gameSize.x);
            int y = rn.NextInt((int)gameSize.y);

            Shapes[0].SetFloat("xPos", x);
            Shapes[0].SetFloat("yPos", y);

            Shapes[0].Dispatch(0, 1, 1, 1);
       }
    }

    public void DrawGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, screen.x, screen.y), backgroundTexture, ScaleMode.StretchToFill);

        if (initialized)
        {
            if (!stepCalled)
            {
                stepCalled = true;

                IEnumerator coroutine = dispatchKernals(1.0f / simSteps);
                StartCoroutine(coroutine);
            }

            GUI.DrawTexture(TopLeft, topLeftCurrent, ScaleMode.ScaleAndCrop);
            GUI.DrawTexture(TopRight, topRightCurrent, ScaleMode.ScaleAndCrop);
            GUI.DrawTexture(BottomLeft, bottomLeftCurrent, ScaleMode.ScaleAndCrop);
            GUI.DrawTexture(BottomRight, bottomRightCurrent, ScaleMode.ScaleAndCrop);
        }
    }
}
