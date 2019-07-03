using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NesScripts.Controls.PathFind;

public class AI : MonoBehaviour
{
    //wander
    GameMap gameMap;
    public Point _from;
    public Point _to;
    public List<Point> path;

    private void Start()
    {
        gameMap = FindObjectOfType<GameMap>();
    }


    public void MoveTowardsTarget(Point targetPoint)
    {
        _from = new Point((int)transform.position.x, (int)transform.position.y);
        _to = targetPoint;

        path = Pathfinding.FindPath(gameMap.grid, _from, _to);
        GetComponent<Entity>().Move(_to.x, _to.y);
    }

}
