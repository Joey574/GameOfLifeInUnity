using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Testing3D : MonoBehaviour
{
    Texture3D texture3D;

    void Awake()
    {
        texture3D = new Texture3D(256, 256, 256, TextureFormat.RGBA32, 0);
        
        texture3D.filterMode = FilterMode.Point;
    }

    void Update()
    {
        
    }

    private void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        Graphics.Blit(texture3D, destination);
    }
}
