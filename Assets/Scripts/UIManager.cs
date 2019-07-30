using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class UIManager : MonoBehaviour
{
    public Fighter playerFighter = null;


    [SerializeField] TextMeshProUGUI healthText = null;
    int playerMaxHP;

    [SerializeField] TextMeshProUGUI powerText = null;
    [SerializeField] TextMeshProUGUI defenseText = null;


    [SerializeField] TextMeshProUGUI[] messages = new TextMeshProUGUI[5];
    [SerializeField] TextMeshProUGUI monster = null;

    [SerializeField] TextMeshProUGUI dungeonLevel = null;

    private void Start()
    {

    }

    private void Update()
    {
        /*
        if (playerFighter == null)
        {
            playerFighter = GameObject.FindObjectOfType<Player>().GetComponent<Fighter>();

            int health = playerFighter.MaxHP;
            SetHealthText(health);
        }
        */
    }

    public void SetPlayer()
    {
        if (playerFighter == null)
        {
            playerFighter = GameObject.FindObjectOfType<Player>().GetComponent<Fighter>();

            //int health = playerFighter.MaxHP;
            //SetHealthText(health);
            SetUIText();
        }
    }

    /*
    public void SetHealthText(int currentHealth)
    {
        playerMaxHP = playerFighter.MaxHP;

        string healthString = "HP:" + playerMaxHP + "/" + currentHealth;

        healthText.text = healthString;
    }

    public void SetPowerText()
    {
        string powerString = "Power:" + playerFighter.power;
        powerText.text = powerString;
    }

    public void SetDefenseText()
    {
        string defenseString = "Defense:" + playerFighter.defense;
        defenseText.text = defenseString;
    }
    */

    public void SetUIText()
    {
        string healthString = "HP:" + playerFighter.HP + "/" + playerFighter.MaxHP;
        healthText.text = healthString;

        string powerString = "Power:" + playerFighter.power;
        powerText.text = powerString;

        string defenseString = "Defense:" + playerFighter.defense;
        defenseText.text = defenseString;
    }



    public void NewMessage(string message)
    {
        for (int i = messages.Length -1; i > 0; i--)
        {
            messages[i].text = messages[i - 1].text;
        }
        messages[0].text = message;
    }

    public void MouseOverText(string text)
    {
        monster.text = text;
    }

    public void DungeonLevelUpdate(int level)
    {
        dungeonLevel.text = "Dungeon Level: " + level;
    }
}
