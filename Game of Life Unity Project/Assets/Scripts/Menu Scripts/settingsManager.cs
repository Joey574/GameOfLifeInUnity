using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class settingsManager : MonoBehaviour
{
    [Header("Simulation Size")]
    public int width = 100;
    public int height = 100;
    
    void Awake()
    {
        
    }

    public int getWidth()
    {
        return width;
    }

    public int getHeight()
    {
        return height;
    }
}
