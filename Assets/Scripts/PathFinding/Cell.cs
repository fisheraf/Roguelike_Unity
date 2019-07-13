using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cell 
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX;
    public int gridY;

    public int gCost;
    public int hCost;
    public Cell parent;

    public bool cellSeen;
    public bool cellVisibile;
    public bool cellIsWall;

    public bool hasEntity;



    public Cell(bool _walkable, Vector3 _worldPosition, int _gridX, int _gridY)
    {
        walkable = _walkable;
        worldPosition = _worldPosition;
        gridX = _gridX;
        gridY = _gridY;
    }

    public int fCost
    {
        get { return gCost + hCost; }
    }
}
