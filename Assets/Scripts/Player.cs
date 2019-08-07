using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    GameMap gameMap;
    Engine engine;

    public List<GameObject> entities;

    float timer;
    bool attacking;

    void Awake()
    {
        gameMap = FindObjectOfType<GameMap>();
        engine = FindObjectOfType<Engine>();

        entities = gameMap.entities;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Movement()
    {
        timer += Time.deltaTime;

        if(!Input.anyKey)
        {
            attacking = false;
        }

        if(timer > .2f && !attacking)
        {            
            //down left
            if (Input.GetKey(KeyCode.Keypad1))
            {
                Move(-1, -1);
            }
            //down
            if (Input.GetKey(KeyCode.Keypad2))
            {
                Move(0, -1);
            }
            //down right
            if (Input.GetKey(KeyCode.Keypad3))
            {
                Move(1, -1);
            }
            //left
            if (Input.GetKey(KeyCode.Keypad4))
            {
                Move(-1, 0);
            }
            //wait
            if (Input.GetKeyDown(KeyCode.Keypad5))
            {
                engine.gameState = Engine.GameState.EnemyTurn;
            }
            //right
            if (Input.GetKey(KeyCode.Keypad6))
            {
                Move(1, 0);
            }
            //up left
            if (Input.GetKey(KeyCode.Keypad7))
            {
                Move(-1, 1);
            }
            //up
            if (Input.GetKey(KeyCode.Keypad8))
            {
                Move(0, 1);
            }
            //up right
            if (Input.GetKey(KeyCode.Keypad9))
            {
                Move(1, 1);
            }
        }
    }

    void Move(int dx, int dy)
    {
        Vector2Int target = new Vector2Int(Mathf.RoundToInt(transform.position.x + dx),
                    Mathf.RoundToInt(transform.position.y + dy));

        //List<GameObject> entities = gameMapObj.entities;
        foreach (GameObject entity in entities)
        {
            if (entity.transform.position.x == target.x && entity.transform.position.y == target.y)
            {
                //Debug.Log("You tickle the " + entity.name);// + " in the shin");
                GetComponent<Fighter>().Attack(entity.GetComponent<Fighter>());
                attacking = true;
                if (engine.gameState == Engine.GameState.PlayerTurn)
                {
                    engine.gameState = Engine.GameState.EnemyTurn;
                }
                return;
            }
        }

        if (gameMap.tileIsWall[target.x, target.y])
        {
            Debug.Log("wall");
        }
        else
        {
            MovePlayer(dx, dy);
            timer = 0f;
            attacking = false;
            engine.gameState = Engine.GameState.EnemyTurn;

            gameMap.FOV();
        }        
    }


    public void MovePlayer(int dx, int dy)
    {
        //following keypad movement (1-9 order this time)
        //down left
        if (dx == -1 && dy == -1)
        {
            gameObject.transform.position -= transform.up + transform.right;
            //position += dx & dy
        }
        //down
        if (dx == 0 && dy == -1)
        {
            gameObject.transform.position -= transform.up;
        }
        //down right
        if (dx == 1 && dy == -1)
        {
            gameObject.transform.position -= transform.up - transform.right;
        }
        //left
        if (dx == -1 && dy == 0)
        {
            gameObject.transform.position -= transform.right;
        }
        //wait
        if (dx == 0 && dy == 0)
        {
            //skip turn
        }
        //right
        if (dx == 1 && dy == 0)
        {
            gameObject.transform.position += transform.right;
        }
        //up left
        if (dx == -1 && dy == 1)
        {
            gameObject.transform.position += transform.up - transform.right;
        }
        //up
        if (dx == 0 && dy == 1)
        {
            gameObject.transform.position += transform.up;
        }
        //up right
        if (dx == 1 && dy == 1)
        {
            gameObject.transform.position += transform.up + transform.right;
        }
    }
}
