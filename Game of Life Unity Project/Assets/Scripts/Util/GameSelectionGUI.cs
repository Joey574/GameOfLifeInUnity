using UnityEngine;

public class GameSelectionGUI : MonoBehaviour
{
    [Header("Passed Values")]
    private GameAttributes gameAttributes;
    private GameValues gameValues;
    private ComputeShader setColor;
    private Rect Location;

    [Header("Private values")]
    Buttons buttons = new Buttons();

    //[Header("GUI Data")]

    private struct Buttons
    {
        public RenderTexture background;
        public Vector2 Desc;
    }

    public void Begin(GameAttributes gameAttributes, Rect Location, ComputeShader setColor)
    {
        initialize();

        this.gameAttributes = gameAttributes;
        this.Location = Location;
        this.setColor = setColor;

        gameValues = GameObject.Find("gameValues").GetComponent<GameValues>();
    }

    private void initialize()
    {
        int w = Screen.width / 10;
        int h = Screen.width / 10;

        buttons.Desc = new Vector2(w, h);

        buttons.background = new RenderTexture(8, 8, 1);
        buttons.background.enableRandomWrite = true;
        buttons.background.Create();
    }

    public GameValues getSettings()
    {
        return gameValues;
    }

    public void OnGUI()
    {
        if (gameAttributes != null)
        {
            GUIStyle style = new GUIStyle(GUI.skin.button);

            GUI.BeginGroup(Location);

            Color backgroundColor = new Color(0.1f, 0.1f, 1.0f, 0.85f);

            setColor.SetVector("color", backgroundColor);
            setColor.SetTexture(0, "Result", buttons.background);

            setColor.Dispatch(0, 1, 1, 1);

            GUI.DrawTexture(new Rect(0, 0, Location.width, Location.height), buttons.background ,ScaleMode.StretchToFill);

            GUI.color = Color.white;

            GUI.Box(new Rect(0,0, buttons.Desc.x, buttons.Desc.y), gameAttributes.Description, style);

            if (gameAttributes.CanCustomizeColor)
            {

            }
            GUI.EndGroup();
        }
    }
}
