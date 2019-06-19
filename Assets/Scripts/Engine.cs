using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Engine : MonoBehaviour
{
    [SerializeField] GameObject player;
 
    enum GameState { PlayerTurn, EnemyTurn }

    GameState gameState = GameState.PlayerTurn;

    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("Player");
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

    private void Movement()
    {
        //up
        if (Input.GetKeyDown(KeyCode.Keypad8))
        {
            player.transform.position += transform.up;
            gameState = GameState.EnemyTurn;
        }
        //down
        if (Input.GetKeyDown(KeyCode.Keypad2))
        {
            player.transform.position -= transform.up;
            gameState = GameState.EnemyTurn;
        }
        //left
        if (Input.GetKeyDown(KeyCode.Keypad4))
        {
            player.transform.position -= transform.right;
            gameState = GameState.EnemyTurn;
        }
        //right
        if (Input.GetKeyDown(KeyCode.Keypad6))
        {
            player.transform.position += transform.right;
            gameState = GameState.EnemyTurn;
        }
        //up left
        if (Input.GetKeyDown(KeyCode.Keypad7))
        {
            player.transform.position += transform.up -transform.right;
            gameState = GameState.EnemyTurn;
        }
        //up right
        if (Input.GetKeyDown(KeyCode.Keypad9))
        {
            player.transform.position += transform.up + transform.right;
            gameState = GameState.EnemyTurn;
        }
        //down left
        if (Input.GetKeyDown(KeyCode.Keypad1))
        {
            player.transform.position -= transform.up + transform.right;
            gameState = GameState.EnemyTurn;
        }
        //down right
        if (Input.GetKeyDown(KeyCode.Keypad3))
        {
            player.transform.position -= transform.up - transform.right;
            gameState = GameState.EnemyTurn;
        }
        //wait
        if (Input.GetKeyDown(KeyCode.Keypad5))
        {            
            gameState = GameState.EnemyTurn;
        } 
    }
}
