using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Mathematics;
using UnityEngine;

public class RLEDispatch
{
    [Header("Data")]
    private string fileName;
    private List<int> rleArray = new List<int>();

    public void CreateArray(string fileName)
   {
        this.fileName = fileName;

        // clear array
        rleArray = new List<int>();

        readFile();
   }

    public RenderTexture DispatchKernal(ComputeShader rleWrite, RenderTexture renderTexture, Color color, Vector2 loc, bool2 lrud)
    {
        rleWrite.SetTexture(0, "Result", renderTexture);
        rleWrite.SetVector("color", color);

        rleWrite.SetFloat("xLoc", loc.x);
        rleWrite.SetFloat("yLoc", loc.y);

        rleWrite.SetBool("lr", lrud.x);
        rleWrite.SetBool("ud", lrud.y);

        ComputeBuffer buffer = new ComputeBuffer(rleArray.Count, sizeof(int));
        buffer.SetData(rleArray);


        int[] arr = new int[rleArray.Count];
        buffer.GetData(arr);

        for (int i = 0; i < rleArray.Count; i++)
        {
            Debug.Log("Source: " + rleArray[i] + " Buffer: " + arr[i]);
        }

        rleWrite.SetBuffer(0, "rle", buffer);

        rleWrite.SetInts("rle", rleArray.ToArray());
        rleWrite.SetInt("len", rleArray.Count);

        rleWrite.Dispatch(0, 1, 1, 1);

        buffer.Release();

        return renderTexture;
    }


    // ARRAY CREATION
    private void readFile()
    {
        TextAsset textAsset = Resources.Load<TextAsset>(fileName);

        string rle = textAsset.text;

        // Remove pattern description
        while (!rle.StartsWith('x') && !rle.StartsWith('X'))
        {
            rle = rle.Remove(0, rle.IndexOf("\n") + 1);
        }

        // adjust
        rle = rle.Remove(0, rle.IndexOf("\n"));

        int count = -1;

        for (int i = 0; i < rle.Length; i++)
        {
            if (rle[i] == 'b')
            {
                count = addCount(count);
                rleArray.Add(0);
            }
            else if (rle[i] == 'o')
            {
                count = addCount(count);
                rleArray.Add(-1);
            }
            else if (rle[i] == '$')
            {
                count = addCount(count);
                rleArray.Add(-2);
            }
            else if (rle[i] >= '0' && rle[i] <= '9')
            {
                count = adjustCount(count, rle[i] - '0');
            }
            else if (rle[i] == '!')
            {
                break;
            }
        }
    }

    private int adjustCount(int count, int val)
    {
        if (count < 0)
        {
            count = val;
        }
        else if (count > 0)
        {
            count *= 10;
            count += val;
        }

        return count;
    } 

    private int addCount(int count)
    {
        if (count < 1)
        {
            rleArray.Add(1);
        } 
        else
        {
            rleArray.Add(count);
        }
        return -1;
    }
}
