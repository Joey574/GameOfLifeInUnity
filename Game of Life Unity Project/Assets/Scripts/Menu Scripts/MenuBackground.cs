using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
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

    void Awake()
    {
        screen.x = Screen.currentResolution.width;
        screen.y = Screen.currentResolution.height;

        gameSize.x = 106;
        gameSize.y = 66;

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

        topLeftCurrent.antiAliasing = 1;topLeftLast.antiAliasing = 1; topRightCurrent.antiAliasing = 1; topRightLast.antiAliasing = 1;
        bottomLeftCurrent.antiAliasing = 1;bottomRightLast.antiAliasing = 1; bottomLeftCurrent.antiAliasing= 1; bottomLeftLast.antiAliasing = 1;

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

        Brushes[0].SetTexture(0, "Result", bottomRightCurrent);
        Brushes[1].SetTexture(0, "Result", bottomRightCurrent);
        Brushes[2].SetTexture(0, "Result", bottomRightCurrent);
        Brushes[3].SetTexture(0, "Result", bottomRightCurrent);

        Brushes[4].SetTexture(0, "Result", topLeftCurrent);
        
        Brushes[4].SetVector("color", Color.white);

        initializeBackground();
    }

    private void initializeBackground()
    {
        setColor.SetVector("color", new Color(0.1f, 0.1f, 0.1f, 1));
        setColor.SetTexture(0, "Result", backgroundTexture);

        setColor.Dispatch(0, 1, 1, 1);

        initializeWireworld();

        initializeClassic();
    }

    private void initializeClassic()
    {
        setColor.SetTexture(0, "Result", topLeftCurrent);
        setColor.SetVector("color", new Color(0,0,0, 1));
        setColor.Dispatch(0, topLeftCurrent.width / threadGroup, topLeftCurrent.height / threadGroup, 1);

        Brushes[4].SetFloat("xPos", gameSize.x / 2 - 40);
        Brushes[4].SetFloat("yPos", gameSize.y / 2 - 20);

        Brushes[4].Dispatch(0, 1, 1, 1);
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
            Brushes[0].SetFloat("xPos", 15);
            Brushes[0].SetFloat("yPos", (gameSize.y / 2) + i);
            Brushes[0].Dispatch(0, 1, 1, 1);
        }

        // Set up 3 top repeaters for the Nand gates
        for (int i = -15; i < 16; i = i + 15)
        {
            Brushes[1].SetFloat("xPos", 5);
            Brushes[1].SetFloat("yPos", (gameSize.y / 2) + i + 3);
            Brushes[1].Dispatch(0, 1, 1, 1);
        }

        // Set up 3 bottom repeaters for the Nand gates
        for (int i = -15; i < 16; i = i + 15)
        {
            Brushes[1].SetFloat("xPos", 5);
            Brushes[1].SetFloat("yPos", (gameSize.y / 2) + i - 3);
            Brushes[1].Dispatch(0, 1, 1, 1);
        }

        // Set length of line for line tool
        Brushes[2].SetInt("len", 4);

        // Set up lines connecting top repeaters and Nand gates
        for (int i = -15; i < 16; i = i + 15)
        {
            Brushes[2].SetFloat("xPos", 12);
            Brushes[2].SetFloat("yPos", (gameSize.y / 2) + i + 3);
            Brushes[2].Dispatch(0, 1, 1, 1);
        }

        // Set up lines connecting bottom repeaters and Nand gates
        for (int i = -15; i < 16; i = i + 15)
        {
            Brushes[2].SetFloat("xPos", 12);
            Brushes[2].SetFloat("yPos", (gameSize.y / 2) + i - 3);
            Brushes[2].Dispatch(0, 1, 1, 1);
        }

        // Set length of line for line tool
        Brushes[2].SetInt("len", 8);

        // Set output lines for Nand gates
        for (int i = -15; i < 16; i = i + 15)
        {
            Brushes[2].SetFloat("xPos", 20);
            Brushes[2].SetFloat("yPos", (gameSize.y / 2) + i);
            Brushes[2].Dispatch(0, 1, 1, 1);
        }

        // Set signal to be right
        Brushes[3].SetBool("left", false);

        // Set signals
        for (int i = -15; i < 16; i = i + 15)
        {
            Brushes[3].SetFloat("xPos", 10);
            Brushes[3].SetFloat("yPos", (gameSize.y / 2) + i - 2);
            Brushes[3].Dispatch(0, 1, 1, 1);
        }

        // Set signals
        for (int i = -15; i < 16; i = i + 15)
        {
            Brushes[3].SetFloat("xPos", 10);
            Brushes[3].SetFloat("yPos", (gameSize.y / 2) + i + 4);
            Brushes[3].Dispatch(0, 1, 1, 1);
        }

        // Set signals to left
        Brushes[3].SetBool("left", true);

        // Set signals
        for (int i = -15; i < 16; i = i + 15)
        {
            Brushes[3].SetFloat("xPos", 7);
            Brushes[3].SetFloat("yPos", (gameSize.y / 2) + i + 2);
            Brushes[3].Dispatch(0, 1, 1, 1);
        }


        // initialize next set of nand gates
        Brushes[0].SetFloat("xPos", (gameSize.x / 3));
        Brushes[0].SetFloat("yPos", (gameSize.y / 2) + 8);
        Brushes[0].Dispatch(0, 1, 1, 1);

        // Set length of line for line tool
        Brushes[2].SetInt("len", 3);

        // Set input lines for the second set of Nand gates
        for (int i = 1; i < 15; i = i + 13)
        {
            Brushes[2].SetFloat("xPos", 28);
            Brushes[2].SetFloat("yPos", (gameSize.y / 2) + i);
            Brushes[2].Dispatch(0, 1, 1, 1);
        }

        // Set input lines for the second set of Nand gates
        for (int i = 2; i < 14; i = i + 11)
        {
            Brushes[2].SetFloat("xPos", 31);
            Brushes[2].SetFloat("yPos", (gameSize.y / 2) + i);
            Brushes[2].Dispatch(0, 1, 1, 1);
        }

        // Set length of line for line tool
        Brushes[2].SetInt("len", 1);

        Brushes[2].SetFloat("xPos", 34);
        Brushes[2].SetFloat("yPos", (gameSize.y / 2) + 3);
        Brushes[2].Dispatch(0, 1, 1, 1);

        Brushes[2].SetFloat("xPos", 35);
        Brushes[2].SetFloat("yPos", (gameSize.y / 2) + 12);
        Brushes[2].Dispatch(0, 1, 1, 1);

        Brushes[2].SetFloat("xPos", 34);
        Brushes[2].SetFloat("yPos", (gameSize.y / 2) + 13);
        Brushes[2].Dispatch(0, 1, 1, 1);

        // Set input lines for the second set of Nand gates
        for (int i = 4; i < 12; i = i + 7)
        {
            Brushes[2].SetFloat("xPos", 35);
            Brushes[2].SetFloat("yPos", (gameSize.y / 2) + i);
            Brushes[2].Dispatch(0, 1, 1, 1);
        }

        // Set input lines for the second set of Nand gates
        Brushes[2].SetFloat("xPos", 35);
        Brushes[2].SetFloat("yPos", (gameSize.y / 2) + 5);
        Brushes[2].Dispatch(0, 1, 1, 1);

        // Set up last Nand Gate
        Brushes[0].SetFloat("xPos", gameSize.x / 2);
        Brushes[0].SetFloat("yPos", gameSize.y / 2);
        Brushes[0].Dispatch(0, 1, 1, 1);

        // Set up input lines for the last Nand gate
        for (int i = 0; i < 11; i++)
        {
            Brushes[2].SetFloat("xPos", 28 + i);
            Brushes[2].SetFloat("yPos", 19 + i);
            Brushes[2].Dispatch(0, 1, 1, 1);
        }

        Brushes[2].SetInt("len", 15);

        // Set up input lines for the last Nand gate
        Brushes[2].SetFloat("xPos", gameSize.x / 2 - 14);
        Brushes[2].SetFloat("yPos", gameSize.y / 2 - 3);
        Brushes[2].Dispatch(0, 1, 1, 1);

        Brushes[2].SetInt("len", 10);

        // Set up input lines for the last Nand gate
        Brushes[2].SetFloat("xPos", gameSize.x / 2 - 9);
        Brushes[2].SetFloat("yPos", gameSize.y / 2 + 3);
        Brushes[2].Dispatch(0, 1, 1, 1);

        Brushes[2].SetInt("len", 1);

        // Set up input lines for the last Nand gate
        for (int i = 0; i < 4; i++)
        {
            Brushes[2].SetFloat("xPos", 40 + i);
            Brushes[2].SetFloat("yPos", 40 - i);
            Brushes[2].Dispatch(0, 1, 1, 1);
        }

        // Set up output lines for the last Nand gate
        Brushes[2].SetInt("len", 48);

        Brushes[2].SetFloat("xPos", gameSize.x / 2 + 5);
        Brushes[2].SetFloat("yPos", gameSize.y / 2);
        Brushes[2].Dispatch(0, 1, 1, 1);

        // Set up vertical output lines
        Brushes[2].SetInt("len", 1);
        for (int i = 0; i < 4; i++)
        {
            for (int q = 0; q < 8 + (i * 5); q++)
            {
                Brushes[2].SetFloat("xPos", gameSize.x / 2 + 18 + (i * 6));
                Brushes[2].SetFloat("yPos", gameSize.y / 2 + 1 + q);
                Brushes[2].Dispatch(0, 1, 1, 1);
            }
        }

        // Horizontal output lines
        for (int i = 0; i < 4; i++)
        {
            Brushes[2].SetInt("len", 23 + (i * 6) - (i * 5));

            Brushes[2].SetFloat("xPos", gameSize.x / 2 - 5 + (i * 5));
            Brushes[2].SetFloat("yPos", gameSize.y / 2 + 9 + (i * 5));
            Brushes[2].Dispatch(0, 1, 1, 1);
        }

        // Set up vertical output lines
        Brushes[2].SetInt("len", 1);
        for (int i = 0; i < 4; i++)
        {
            for (int q = 0; q < 25; q++)
            {
                Brushes[2].SetFloat("xPos", gameSize.x / 2 - 6 + (i * 5));
                Brushes[2].SetFloat("yPos", gameSize.y / 2 + 10 + (i * 5) + q);
                Brushes[2].Dispatch(0, 1, 1, 1);
            }
        }

        // Set up vertical output lines
        Brushes[2].SetInt("len", 1);
        for (int i = 0; i < 2; i++)
        {
            for (int q = 0; q < 35; q++)
            {
                Brushes[2].SetFloat("xPos", gameSize.x / 2 + 42 + (i * 6));
                Brushes[2].SetFloat("yPos", gameSize.y / 2 + 1 + q);
                Brushes[2].Dispatch(0, 1, 1, 1);
            }
        }

        for (int i = 0; i < 3; i++)
        {
            for (int q = 0; q < 8 + (i * 5); q++)
            {
                Brushes[2].SetFloat("xPos", gameSize.x / 2 + 10 + (i * 5));
                Brushes[2].SetFloat("yPos", gameSize.y / 2 - 1 - q);
                Brushes[2].Dispatch(0, 1, 1, 1);
            }
        }

        Brushes[2].SetInt("len", 17);

        for (int i = 0; i < 3; i++)
        {
            Brushes[2].SetFloat("xPos", gameSize.x / 2 - 7 + (i * 5));
            Brushes[2].SetFloat("yPos", gameSize.y / 2 - 9 - (i * 5));
            Brushes[2].Dispatch(0, 1, 1, 1);
        }

        Brushes[2].SetInt("len", 1);

        for (int i = 0; i < 3; i++)
        {
            for (int q = 0; q < 35; q++)
            {
                Brushes[2].SetFloat("xPos", gameSize.x / 2 - 8 + (i * 5));
                Brushes[2].SetFloat("yPos", gameSize.y / 2 - 10 - q - (i * 5));
                Brushes[2].Dispatch(0, 1, 1, 1);
            }
        }

        for (int i = 0; i < 5; i++)
        {
            for (int q = 0; q < 35; q++)
            {
                Brushes[2].SetFloat("xPos", gameSize.x / 2 + 27 + (i * 6));
                Brushes[2].SetFloat("yPos", gameSize.y / 2 - 1 - q);
                Brushes[2].Dispatch(0, 1, 1, 1);
            }
        }

    }

    private IEnumerator dispatchKernals(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

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

        stepCalled = false;
    }

    public void DrawGUI()
    {
        GUI.DrawTexture(new Rect(0, 0, screen.x, screen.y), backgroundTexture, ScaleMode.StretchToFill);

        if (initialized)
        {
            if (!stepCalled)
            {
                stepCalled = true;
                if (simSteps != 0)
                {
                    IEnumerator coroutine = dispatchKernals(1.0f / simSteps);
                    StartCoroutine(coroutine);
                }
            }

            GUI.DrawTexture(TopLeft, topLeftCurrent, ScaleMode.ScaleToFit);
            GUI.DrawTexture(TopRight, topRightCurrent, ScaleMode.ScaleToFit);
            GUI.DrawTexture(BottomLeft, bottomLeftCurrent, ScaleMode.ScaleToFit);
            GUI.DrawTexture(BottomRight, bottomRightCurrent, ScaleMode.ScaleToFit);
        }
    }
}
