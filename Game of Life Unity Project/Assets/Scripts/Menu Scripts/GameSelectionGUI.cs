using UnityEngine;

public class GameSelectionGUI : MonoBehaviour
{
    [Header("Compute Shaders")]
    public ComputeShader setColor;

    [Header("Public Scripts")]
    public GameValues gameValues;

    [Header("Private values")]
    Buttons buttons = new Buttons();

    private struct Buttons
    {
        public RenderTexture background;
        public Vector2 Desc;
    }

    public void Awake()
    {
        Initialize();

        buttons.background = new RenderTexture(8, 8, 1);
        buttons.background.enableRandomWrite = true;
        buttons.background.Create();
    }

    private void Initialize()
    {
        int w = Screen.width / 5;
        int h = Screen.height / 10;

        buttons.Desc = new Vector2(w, h);
    }

    public void DrawGUI(GameAttributes gameAttributes, MainMenuGUIData gui)
    {
        if (gameAttributes != null)
        {
            GUIStyle style = new GUIStyle(GUI.skin.label);
            style.fontSize = 18;

            Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 0.85f);

            setColor.SetVector("color", backgroundColor);
            setColor.SetTexture(0, "Result", buttons.background);

            setColor.Dispatch(0, 1, 1, 1);

            GUI.DrawTexture(new Rect(gui.firstGame.width + 10, 0, gui.firstGame.width * 1.5f, gui.firstGame.height * 5), buttons.background ,ScaleMode.StretchToFill);

            GUI.color = Color.white;

            GUI.Label(new Rect(gui.firstGame.width + 11, gui.firstGame.height * 2, (gui.firstGame.width * 1.5f) - 2, buttons.Desc.y), gameAttributes.Description, style);

            if (gameAttributes.CanCustomizeColor)
            {

            }
        }
    }
}
