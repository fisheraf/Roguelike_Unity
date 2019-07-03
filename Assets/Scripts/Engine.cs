using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

using NesScripts.Controls.PathFind;

public class Engine : MonoBehaviour
{   
    [SerializeField] GameObject entityObject;
    Entity entity;
    Entity playerEntity;
    //[SerializeField] GameObject player;
    [SerializeField] Tilemap tilemap;
    GameMap gameMapObj;

    List<GameObject> entities;

    enum GameState { PlayerTurn, EnemyTurn }

    GameState gameState = GameState.PlayerTurn;

    private void Awake()
    {
        //CreateEntity(0, 0, 0, new Color32(0, 135, 255, 255), "Player");
        //CreateEntity()
    }

    // Start is called before the first frame update
    void Start()
    {
        gameMapObj = FindObjectOfType<GameMap>().GetComponent<GameMap>();
        entities = gameMapObj.entities;
        //player = GameObject.Find("Player");
        //entity = player.GetComponent<Entity>();

        //gameState = GameState.PlayerTurn;
    }

    // Update is called once per frame
    void Update()
    {
        if(gameState == GameState.PlayerTurn)
        {
            Movement();
        }

        //enemy turn test
        if(gameState == GameState.EnemyTurn)
        {
            EnemyTurn();
            if (Input.GetKeyDown(KeyCode.Space))
            {
                gameState = GameState.PlayerTurn;
            }
        }
    }

    public void CreateEntity(int x, int y, int z, int spriteNumber, Color32 color, string name, bool fighter = false, bool ai = false)
    {
        entityObject = Instantiate(entityObject, new Vector3(x, y, z), Quaternion.identity);
        entity = entityObject.GetComponent<Entity>();
        SpriteRenderer spriteRenderer = entity.GetComponentInChildren<SpriteRenderer>();
        spriteRenderer.sprite = entity.entitySprites[spriteNumber];
        spriteRenderer.color = color;
        entityObject.name = name;

        gameMapObj.entities.Add(entityObject);

        if(fighter)
        {
            entityObject.AddComponent<Fighter>();
        }
        if (ai)
        {
            
            entityObject.AddComponent<AI>();
        }

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
        playerEntity = GameObject.Find("Player").GetComponent<Entity>();
        Vector3Int targetTile = new Vector3Int(Mathf.RoundToInt(playerEntity.transform.position.x + dx),
                    Mathf.RoundToInt(playerEntity.transform.position.y + dy), 0);

        //List<GameObject> entities = gameMapObj.entities;
        foreach (GameObject entity in entities)
        {
            if(entity.transform.position.x == targetTile.x && entity.transform.position.y == targetTile.y)
            {
                Debug.Log("You tickle the " + entity.name);// + " in the shin");
                gameState = GameState.EnemyTurn;
                return;
            }            
        }
        
        if (tilemap.GetTile(targetTile).name == "WallTile")
        {
            Debug.Log("wall");
        }
        else
        {
            playerEntity.Move(dx, dy);
            gameState = GameState.EnemyTurn;

            //gameMapObj.FOVRecompute();
            gameMapObj.FOV();
            //gameMapObj.GetVisibleCells();
        }        
    }

    void EnemyTurn()
    {
        foreach (GameObject entity in entities)
        {
            if (entity.name != "Player")
            {
                if (entity.GetComponent<AI>() != null)
                {
                    entity.GetComponent<AI>().MoveTowardsTarget(new Point((int)playerEntity.transform.position.x, (int)playerEntity.transform.position.y));
                }

                Debug.Log("The " + entity + " ponders the meaning of life.");
            }
        }
        gameState = GameState.PlayerTurn;
    }
}
