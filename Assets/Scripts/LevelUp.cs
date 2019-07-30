using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;


public class LevelUp : MonoBehaviour
{

    Fighter playerFighter = null;
    Engine engine = null;
    [SerializeField] GameObject panel = null;
    [SerializeField] Button[] button = null;

    Engine.GameState lastGameState;

    private void Start()
    {
        //playerFighter = FindObjectOfType<Player>().GetComponent<Fighter>();
        engine = FindObjectOfType<Engine>();
        panel.SetActive(false);
    }

    public void SetPlayer()
    {
        if (playerFighter == null)
        {
            playerFighter = FindObjectOfType<Player>().GetComponent<Fighter>();
        }
    }

    private void Update()
    {
        if(engine.gameState == Engine.GameState.LevelUp)
        {
            if(Input.GetKeyDown(KeyCode.Escape))
            {
                CloseMenu();
            }
        }
    }

    public void OpenMenu()
    {
        //lastGameState = engine.gameState;
        engine.gameState = Engine.GameState.LevelUp;
        Debug.Log("level up state");
        panel.SetActive(true);

        EventSystem.current.firstSelectedGameObject = button[0].gameObject;
        EventSystem.current.SetSelectedGameObject(button[0].gameObject);
    }

    public void CloseMenu()
    {
        engine.gameState = Engine.GameState.EnemyTurn;
        //engine.gameState = Engine.GameState.LevelUp;
        panel.SetActive(false);
    }

    public void StatIncrease(int choice)
    {
        if(choice == 0)
        {
            playerFighter.MaxHP += 20;
            playerFighter.HP += 20;
        }
        else if(choice == 1)
        {
            playerFighter.basePower += 1;
        }
        else if(choice == 2)
        {
            playerFighter.baseDefense += 1;
        }

        FindObjectOfType<UIManager>().SetUIText();
        panel.SetActive(false);
        engine.gameState = Engine.GameState.EnemyTurn;
        //engine.gameState = lastGameState;
    }

}