using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManagerBattle : GameManagerTemplate
{
    [Header("Battle Info")]
    public bool blue;
    public bool red;

    protected override void inputHandler()
    {
        throw new System.NotImplementedException();
    }

    protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        throw new System.NotImplementedException();
    }
}