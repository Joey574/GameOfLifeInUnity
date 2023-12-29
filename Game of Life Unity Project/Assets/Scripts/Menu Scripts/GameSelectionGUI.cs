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
        int w = Screen.width / 10;
        int h = Screen.width / 10;

        buttons.Desc = new Vector2(w, h);
    }

    public void DrawGUI(GameAttributes gameAttributes)
    {
        if (gameAttributes != null)
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);

            Color backgroundColor = new Color(0.1f, 0.1f, 1.0f, 1.0f);

            setColor.SetVector("color", backgroundColor);
            setColor.SetTexture(0, "Result", buttons.background);

            setColor.Dispatch(0, 1, 1, 1);

            GUI.DrawTexture(new Rect(0,0, 200, 200), buttons.background ,ScaleMode.StretchToFill);

            GUI.color = Color.white;

            GUI.Box(new Rect(0,0, buttons.Desc.x, buttons.Desc.y), gameAttributes.Description, style);

            if (gameAttributes.CanCustomizeColor)
            {

            }
        }
    }
}
