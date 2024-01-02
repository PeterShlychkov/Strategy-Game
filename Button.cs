using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Button : MonoBehaviour
{
    private int x;
    private int y;

    void Start()
    {

    }
    void Update()
    {

    }

    public void setCoord(int setX, int setY)
    {
        x = setX;
        y = setY;
    }
    public int getX()
    {
        return x;
    }

    public int getY()
    {
        return y;
    }
}
