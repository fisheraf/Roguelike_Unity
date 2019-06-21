using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Engine : MonoBehaviour
{    
    Entity entity;
    [SerializeField] GameObject player;
    [SerializeField] Tilemap tilemap;
 
    enum GameState { PlayerTurn, EnemyTurn }

    GameState gameState = GameState.PlayerTurn;



    // Start is called before the first frame update
    void Start()
    {
        //CreatePlayer();
        player = GameObject.Find("Player");
        entity = player.GetComponent<Entity>();

        gameState = GameState.PlayerTurn;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameState == GameState.PlayerTurn)
        {
            Movement();
        }

        //enemy turn test
        if (Input.GetKeyDown(KeyCode.Space))
        {
            gameState = GameState.PlayerTurn;
        }
    }

    void CreatePlayer()
    {
        player = Instantiate(player, Vector3.zero, Quaternion.identity);
        entity = player.GetComponent<Entity>();
        entity.entityName = "Player";
        //entity.entitySprite =  //list of sprites?
    }

    private void Movement()
    {
        //own script?
        //Entity.Move(int dx, int dy)
        //down left
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            Move(-1, -1);
            //gameState = GameState.EnemyTurn;
        }
        //down
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            Move(0, -1);
            //gameState = GameState.EnemyTurn;
        }
        //down right
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            Move(1, -1);
            //gameState = GameState.EnemyTurn;
        }
        //left
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            Move(-1, 0);
            //gameState = GameState.EnemyTurn;
        }
        //wait
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {            
            gameState = GameState.EnemyTurn;
        } 
        //right
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            Move(1, 0);
            //gameState = GameState.EnemyTurn;
        }
        //up left
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            Move(-1, 1);
            //gameState = GameState.EnemyTurn;
        }
        //up
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            Move(0, 1);
            //gameState = GameState.EnemyTurn;
        }
        //up right
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            Move(1, 1);
            //gameState = GameState.EnemyTurn;
        }

    }

    void Move(int dx, int dy)
    {        
        if (tilemap.GetTile(new Vector3Int(
                    Mathf.RoundToInt(entity.transform.position.x + dx),
                    Mathf.RoundToInt(entity.transform.position.y + dy),
                    0)).name == "WallTile")
        Debug.Log("wall");
        else
        {
            entity.Move(dx, dy);
            gameState = GameState.EnemyTurn;
        }        
    }
}
