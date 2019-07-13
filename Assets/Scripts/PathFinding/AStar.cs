using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.Diagnostics;
using Debug = UnityEngine.Debug;

public class AStar : MonoBehaviour
{
    //credit to Sebastian Lague Tutorial
    public Stopwatch timer;


    public Transform start, target;

    GameMap gameMap;
    public List<Cell> aStarPath = new List<Cell>();
    public List<Vector2> aPath = new List<Vector2>();

    void Awake()
    {
        gameMap = FindObjectOfType<GameMap>();
    }

    public List<Cell> openSet;
    

    public void FindPath(Cell startCell, Cell targetCell)
    {
        timer = new Stopwatch();
        timer.Start();

        //Debug.Log(startPosition);
        //Debug.Log(targetPosition);

        //Cell startCell = gameMap.CellFromWorldPoint(startPosition);
        //Cell targetCell = gameMap.CellFromWorldPoint(targetPosition);

        Debug.Log("Astar" + startCell.gridX + "," + startCell.gridY);
        Debug.Log("to" + targetCell.gridX + "," + targetCell.gridY);

        openSet.Clear();
        openSet = new List<Cell>();
        HashSet<Cell> closedSet = new HashSet<Cell>();
        openSet.Add(startCell);

        while (openSet.Count > 0)
        {
            Debug.Log(openSet[0].gridX + "," + openSet[0].gridY);
            Cell cell = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
            {
                if(openSet[i].fCost < cell.fCost || openSet[i].fCost == cell.fCost)
                {
                    if(openSet[i].hCost < cell.hCost)
                    {
                        cell = openSet[i];
                    }
                }
            }

            openSet.Remove(cell);
            closedSet.Add(cell);

            if (cell.gridX == targetCell.gridX && cell.gridY == targetCell.gridY)
            {
                Debug.Log("Path Found");
                RetracePath(startCell,targetCell);
                return;
            }


            foreach (Cell neighbor in gameMap.GetNeighbors(cell))
            {
                //Debug.Log(neighbor.walkable);
                if(!neighbor.walkable || closedSet.Contains(neighbor))
                {
                    continue;
                }

                int newCostToNeighbor = cell.gCost + GetDistance(cell, neighbor);
                if(newCostToNeighbor < cell.gCost || !openSet.Contains(neighbor))
                {
                    neighbor.gCost = newCostToNeighbor;
                    neighbor.hCost = GetDistance(neighbor, targetCell);
                    neighbor.parent = cell;

                    if (!openSet.Contains(neighbor))
                        openSet.Add(neighbor);
                }
            }
        }

        timer.Stop();
        Debug.Log("Running A* took:" + timer.Elapsed);
        //set random direction?
        //return null;
    }

    void RetracePath(Cell startCell, Cell endCell)
    {
        Debug.Log("target found");
        List<Cell> path = new List<Cell>();
        Cell currentCell = endCell;

        while(currentCell != startCell)
        {
            path.Add(currentCell);
            //aPath.Add(new Vector2(currentCell.gridX, currentCell.gridY));
            currentCell = currentCell.parent;            
        }
        path.Reverse();


        aStarPath = path;
        gameMap.path = path;
    }

    int GetDistance(Cell cellA, Cell cellB)
    {
        int distanceX = Mathf.Abs(cellA.gridX - cellB.gridX);
        int distanceY = Mathf.Abs(cellA.gridY - cellB.gridY);

        if(distanceX > distanceY)
        {
            return 14 * distanceY + 10 * (distanceX - distanceY);
        }
        else
        {
            return 14 * distanceX + 10 * (distanceY - distanceX);
        }
    }
}
