using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class GameManagerInfection : GameManagerTemplate
{
    [Header("Infection States")]
    public bool infected;

    protected override void setCellColor()
    {
    }

    protected override void inputHandler()
    {
        if (Input.mouseScrollDelta.y != 0 && Input.GetKey(KeyCode.LeftShift))
        {
            if (Input.mouseScrollDelta.y > 0)
            {
                radius += radiusInc;
            }
            else
            {
                radius -= radiusInc;
            }

            if (radius <= 0)
            {
                radius = 1;
            }

        }
        else if (Input.mouseScrollDelta.y != 0)
        {
            scale.y = lastScale - (Input.mouseScrollDelta.y * zoomSensitivity);
            scale.x = lastScale - (Input.mouseScrollDelta.y * zoomSensitivity);
        }

        if (Input.GetKey(KeyCode.A))
        {
            offset.x = lastOffset.x - (offsetInc * scale.x) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.D))
        {
            offset.x = lastOffset.x + (offsetInc * scale.x) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.W))
        {
            offset.y = lastOffset.y + (offsetInc * scale.y) * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.S))
        {
            offset.y = lastOffset.y - (offsetInc * scale.y) * Time.deltaTime;
        }

        paint = Input.GetMouseButton(0);

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            StartCoroutine(callMenu());
        }
        if (Input.GetMouseButtonDown(1)) 
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
        }
        if (Input.GetKeyDown(KeyCode.Q)) { beginSim = !beginSim; }
    }

    protected override void OnRenderImage(RenderTexture source, RenderTexture destination)
    {
        if (!menuCalled)
        {
            if (paint)
            {
                float mouseX = Input.mousePosition.x * screenAdjustX * scale.x + (offset.x * textureWidth);
                float mouseY = Input.mousePosition.y * screenAdjustY * scale.y + (offset.y * textureHeight);

                toggleCellState.SetBool("paint", paint);
                toggleCellState.SetBool("alive", alive);
                toggleCellState.SetBool("infected", infected);
                toggleCellState.SetFloat("radius", radius);
                toggleCellState.SetFloat("mousePosX", mouseX);
                toggleCellState.SetFloat("mousePosY", mouseY);

                toggleCellState.Dispatch(0, threadDispatchX, threadDispatchY, 1);

            }
            Graphics.Blit(currentTexture, destination, scale, offset);
        }
    }
}
