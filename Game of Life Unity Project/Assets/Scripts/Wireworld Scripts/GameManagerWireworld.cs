using UnityEngine;

public class GameManagerWireworld : GameManagerTemplate
{
    [Header("Wireworld Data")]
    public bool conduit;
    public bool heads;
    public bool tails;

    protected override void SetCellColor()
    {
    }

    protected override void ToggleDrawState()
    {
        if (conduit)
        {
            conduit = false;
            heads = true;
        }
        else if (heads)
        {
            heads = false;
            tails = true;
        }
        else if (tails)
        {
            tails = false;
        }
        else
        {
            conduit = true;
        }

        newColor = conduit ? new Color(1,1,0,1) : heads ? Color.red : tails ? Color.blue : Color.black;
    }
}
