using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ESCMenu : MonoBehaviour
{
    private ComputeShader setColor;

    public bool render = false;
    private bool settings = false;
    private bool updated = false;

    private Texture current;

    private Vector2 scale;
    private Vector2 offset;

    private int buttonWidth;
    private int buttonHeight;

    private int settingsWidth;
    private int settingsHeight;

    private float w;
    private float h;

    private int simSteps;

    private buttonPos buttons;

    private struct buttonPos
    {
        public Rect fistPos;
        public Rect secondPos;
        public Rect thirdPos;
        public Rect fourthPos;
        public Rect backgroundPos;

        public Rect simStep;
        public Rect simStepSlider;
        public Rect simStepBox;

        public RenderTexture background;

        public GUIContent simStepContent;
    }

    private GameManagerGPU gameManagerGPU;

    private void initialize ()
    {
        buttons = new buttonPos();

        buttonHeight = Screen.currentResolution.height / 16;
        buttonWidth = Screen.currentResolution.width / 8;

        settingsHeight = Screen.currentResolution.height / 40;
        settingsWidth = Screen.currentResolution.width / 8;

        buttons.simStepContent = new GUIContent();

        w = (Screen.currentResolution.width / 2) - (buttonWidth / 2);
        h = Screen.currentResolution.height / 2;

        buttons.fistPos.x = w;
        buttons.secondPos.x = w;
        buttons.thirdPos.x = w;
        buttons.fourthPos.x = w;

        buttons.simStep.x = w;
        buttons.simStepSlider.x = w;
        buttons.simStepBox.x = w + (settingsWidth - (settingsWidth / 5));

        buttons.fistPos.width = buttonWidth;
        buttons.secondPos.width = buttonWidth;
        buttons.thirdPos.width = buttonWidth;
        buttons.fourthPos.width = buttonWidth;
        buttons.fistPos.height = buttonHeight;
        buttons.secondPos.height = buttonHeight;
        buttons.thirdPos.height = buttonHeight;
        buttons.fourthPos.height = buttonHeight;

        buttons.simStep.width = settingsWidth;
        buttons.simStepSlider.width = settingsWidth - (settingsWidth / 5) - 5;
        buttons.simStepBox.width = (settingsWidth / 5);
        buttons.simStep.height = settingsHeight;
        buttons.simStepSlider.height = settingsHeight;
        buttons.simStepBox.height = settingsHeight / 1.5f;

        buttons.fistPos.y = h - (2 * (buttonHeight - 1));
        buttons.secondPos.y = buttons.fistPos.y + buttonHeight + 1;
        buttons.thirdPos.y = buttons.secondPos.y + buttonHeight + 1;
        buttons.fourthPos.y = buttons.thirdPos.y + buttonHeight + 1;

        buttons.simStep.y = h - (2 * (buttonHeight - 1));
        buttons.simStepSlider.y = buttons.simStep.y + settingsHeight + 5;
        buttons.simStepBox.y = buttons.simStepSlider.y;

        buttons.backgroundPos.x = buttons.fistPos.x - 15;
        buttons.backgroundPos.y = buttons.fistPos.y - 15;
        buttons.backgroundPos.width = buttonWidth + 30;
        buttons.backgroundPos.height = ((buttonHeight + 1) * 4) + 30;

        buttons.background = new RenderTexture(8, 8, 1);
        buttons.background.enableRandomWrite = true;
        buttons.background.Create();

        Color backgroundColor = new Color(1, 1, 1, 0.1f);

        setColor.SetVector("color", backgroundColor);
        setColor.SetTexture(0, "Result", buttons.background);

        setColor.Dispatch(0, 1, 1, 1);

        buttons.simStepContent.tooltip = "The number of times the board is updated per second, your fps limits how high this value will effectively be.";
        buttons.simStepContent.text = "Sim steps / second";
    }

    public void begin(GameManagerGPU gameManagerGPU, Texture current, Vector2 scale, Vector2 offset, ComputeShader setColor, int simSteps)
    {
        this.setColor = setColor;

        this.gameManagerGPU = gameManagerGPU;
        this.current = current;

        this.simSteps = simSteps; 

        this.scale = scale;
        this.offset = offset;

        initialize();

        render = true;
    }

    public void OnGUI() 
    {
        if (render)
        {
            GUI.color = Color.white;
            GUI.backgroundColor = new Color(256, 0, 0);
            GUI.contentColor = Color.white;

            GUI.DrawTexture(buttons.backgroundPos, buttons.background, ScaleMode.StretchToFill);

            GUI.Label(new Rect(100, 100, 500, 400), GUI.tooltip);

            if (settings)
            {
                string simString;

                if (GUI.Button(buttons.fourthPos, "Back")) { settings = false; updated = false; }

                GUI.Box(buttons.simStep, buttons.simStepContent);

                simSteps = (int) GUI.HorizontalSlider(buttons.simStepSlider, simSteps, 1, 1000);

                simString = GUI.TextField(buttons.simStepBox, simSteps.ToString());

                try
                {
                    simSteps = Int32.Parse(simString);
                } catch { }

                Mathf.Clamp(simSteps, 1, 1000);
            }
            else
            {
                if (GUI.Button(buttons.fistPos, "Resume"))
                {
                    gameManagerGPU.setSimSteps(simSteps);

                    gameManagerGPU.setMenuCalled(false);
                    render = false;
                    Destroy(gameObject.GetComponent<ESCMenu>());
                }
                if (GUI.Button(buttons.secondPos, "Settings"))
                {
                    settings = true;
                    updated = false;
                }
                if (GUI.Button(buttons.thirdPos, "Exit to Menu"))
                {
                    SceneManager.LoadScene("Menu", LoadSceneMode.Single);
                }
                if (GUI.Button(buttons.fourthPos, "Exit Game"))
                {
                    Application.Quit();
                }
            }         
        }
    }

    public void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!updated)
        {
            Graphics.Blit(current, destination, scale, offset);
            updated = true;
        }
    }
}
