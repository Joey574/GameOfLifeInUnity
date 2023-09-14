using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DotBehavior : MonoBehaviour
{

    [Header("Game Data")]
    public int numberOfTiles;

    [Header("Location Data")]
    public int x;
    public int y;

    private int neighborsAlive;
    private bool alive;

    private List<DotBehavior> neighbors;

    void setNeighbors()
    {
        DotBehavior temp;
        for (int i = 0; i < numberOfTiles; i++)
        {
            temp = GameObject.Find("Square(" + i + ")").GetComponent<DotBehavior>();
            if ((temp.getX() == (x + 1)) && (temp.getY() == (y + 1)))
            {
                neighbors.Add(temp);
            }
            else if ((temp.getX() == (x + 1)) && (temp.getY() == y))
            {
                neighbors.Add(temp);
            }
            else if ((temp.getX() == (x + 1)) && (temp.getY() == (y - 1)))
            {
                neighbors.Add(temp);
            }
            else if ((temp.getX() == (x - 1)) && (temp.getY() == (y + 1)))
            {
                neighbors.Add(temp);
            }
            else if ((temp.getX() == (x - 1)) && (temp.getY() == y))
            {
                neighbors.Add(temp);
            }
            else if ((temp.getX() == (x - 1)) && (temp.getY() == (y - 1)))
            {
                neighbors.Add(temp);
            }
            else if ((temp.getX() == x) && (temp.getY() == (y + 1)))
            {
                neighbors.Add(temp);
            }
            else if ((temp.getX() == x) && (temp.getY() == (y - 1)))
            {
                neighbors.Add(temp);
            }

        }
    }

    void Update()
    {
        neighbors.ForEach((v) =>
        {
            if (v.getStatus())
            {
                neighborsAlive++;
            }
        });

        if (alive && (neighborsAlive == 2 || neighborsAlive == 3))
        {
            alive = true;
        }
        else if (!alive && neighborsAlive == 3)
        {
            alive = true;
        }
        else
        {
            alive = false;
        }

        if (alive)
        {
            GameObject.
        }
    }

    public int getX()
    {
        return x;
    }

    public int getY()
    {
        return y;
    }

    public bool getStatus()
    {
        return alive;
    }

    public void setDotNumber(int input)
    {
        numberOfTiles = input;
        setNeighbors();
    }
}
