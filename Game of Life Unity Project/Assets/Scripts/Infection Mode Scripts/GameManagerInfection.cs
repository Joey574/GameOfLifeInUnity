using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameManagerInfection : GameManagerTemplate
{
    [Header("Infection States")]
    public bool infected;

    protected override void SetCellColor()
    {
        toggleCellState.SetVector("color", liveCell);
    }

    protected override void ToggleDrawState()
    {
        if (alive)
        {
            alive = false;
            infected = true;
        }
        else if (infected)
        {
            infected = false;
        }
        else
        {
            alive = true;
        }

        newColor = alive ? Color.white : infected ? Color.green : Color.black;
    }

}
