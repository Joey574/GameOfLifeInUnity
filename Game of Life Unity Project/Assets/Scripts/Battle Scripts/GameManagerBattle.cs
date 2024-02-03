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

        //if (red)
        //{
        //    red = false;
        //    blue = true;
        //}
        //else if (blue)
        //{
        //    blue = false;
        //}
        //else
        //{
        //    red = true;
        //}

        newColor = blue ? Color.blue : red ? Color.red : Color.black;
    }
}
