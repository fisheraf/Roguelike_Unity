using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AI : MonoBehaviour
{
    //wander
    GameMap gameMap;
    AStar aStar;
    Pathfinding pathfinding;
    Grid2 grid2;
    Entity entity;
    Entity playerEntity;

    public List<Cell> path;
    public List<Node> path2;


    private void Start()
    {
        gameMap = FindObjectOfType<GameMap>();
        aStar = FindObjectOfType<AStar>();
        pathfinding = FindObjectOfType<Pathfinding>();
        grid2 = FindObjectOfType<Grid2>();
        entity = GetComponent<Entity>();
        //playerEntity = GameObject.Find("Player").GetComponent<Entity>();
    }

    public void MoveTowardsPlayer()
    {
        //MoveTowardsTarget(playerEntity);
    }


    public void MoveTowardsTarget(Entity targetEntity)
    {
        /*
        //Debug.Log(targetEntity.position);
        //Debug.Log(transform.position);
        /*
        Cell targetCell = new Cell(true, targetEntity.position, (int)targetEntity.position.x, (int)targetEntity.position.y);

        Cell _from = new Cell(true, entity.transform.position, (int)entity.transform.position.x, (int)entity.transform.position.y);
        Cell _to = targetCell;
        

        Node targetNode = new Node(true, targetEntity.position, (int)targetEntity.position.x, (int)targetEntity.position.y);
        Node _from = new Node(true, entity.transform.position, (int)entity.transform.position.x, (int)entity.transform.position.y);
        Node _to = targetNode;

        //Debug.Log("from " + _to.gridX + ", " + _to.gridY);

        //Debug.Log("to " + _from.gridX +", " + _from.gridY);

        //aStar.FindPath(_from, _to);
        //path = aStar.aStarPath;

        //pathfinding.FindPath(_from, _to);
        path2 = grid2.path;

        //Debug.Log(path[1].gridX +", " + path[1].gridY);
        //Cell nextCell = path[0];

        //entity.Move(nextCell.gridX, nextCell.gridY);
        */

    }

}
