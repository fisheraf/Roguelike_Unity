using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour
{
    Engine engine;

    // Start is called before the first frame update
    void Start()
    {
        engine = FindObjectOfType<Engine>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void HideMenu()
    {
        gameObject.SetActive(false);
        engine.gameState = Engine.GameState.PlayerTurn;
    }
}
