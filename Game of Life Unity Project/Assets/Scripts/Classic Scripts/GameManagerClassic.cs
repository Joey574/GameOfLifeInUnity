using UnityEngine;

public class GameManagerClassic : GameManagerTemplate
{
    protected override void SetCellColor()
    {
        setCurrentTexture.SetVector("color", liveCell);
        toggleCellState.SetVector("color", liveCell);
    }

    protected override void ToggleDrawState()
    {
        alive = !alive; newColor = alive ? liveCell : Color.black; 
    }
}
