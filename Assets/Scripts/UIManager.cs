using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class UIManager : MonoBehaviour
{
    public Fighter playerFighter = null;


    [SerializeField] TextMeshProUGUI healthText = null;
    [SerializeField] TextMeshProUGUI powerText = null;
    [SerializeField] TextMeshProUGUI defenseText = null;


    [SerializeField] TextMeshProUGUI[] messages = new TextMeshProUGUI[5];
    [SerializeField] TextMeshProUGUI monster = null;

    [SerializeField] TextMeshProUGUI dungeonLevel = null;
    [SerializeField] TextMeshProUGUI turnsTaken = null;

    [SerializeField] Image damageImage = null;


    public void SetPlayer()
    {
        if (playerFighter == null)
        {
            playerFighter = GameObject.FindObjectOfType<Player>().GetComponent<Fighter>();

            SetUIText();
        }
    }



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

    public void TurnsTakenUpdate(int turns)
    {
        turnsTaken.text = "Turns  Taken: " + turns;
    }

    public void DamageFlash()
    {
        //damageImage.DOFade(.2f, .1f);
        damageImage.color = new Color(1, 0, 0, .2f);
        damageImage.DOFade(0f, .2f);

    }
}
