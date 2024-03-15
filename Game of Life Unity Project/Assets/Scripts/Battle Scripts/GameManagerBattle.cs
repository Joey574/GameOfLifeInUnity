using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameManagerBattle : GameManagerTemplate
{
    [Header("Battle Info")]
    public bool blue;
    public bool red;

    protected override void SetCellColor()
    {
    }

    protected override void ToggleDrawState()
    {
        red = !red && !blue ? true : false;
        blue = !red && !blue ? true : false;

        newColor = blue ? Color.blue : red ? Color.red : Color.black;
    }
}
