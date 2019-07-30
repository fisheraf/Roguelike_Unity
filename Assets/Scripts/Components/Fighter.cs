using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;


public class Fighter : MonoBehaviour
{
    public int MaxHP;
    int baseMaxHP;
    public int bonusMaxHP;
    public int HP;

    public int defense => baseDefense + bonusDefense;
    public int baseDefense;
    public int bonusDefense;

    public int power => basePower + bonusPower;
    public int basePower;
    public int bonusPower;

    public int XP;

    UIManager uiManager;

    private void Awake()
    {
        uiManager = FindObjectOfType<UIManager>();
    }

    


    public void takeDamage(int damage)
    {
        HP -= damage;
        if(name == "Player")
        {
            //uiManager.SetHealthText(HP);
            uiManager.SetUIText();
        }

        if(HP <= 0)
        {
            if(name != "Player")
            {
                uiManager.NewMessage("The " + this.GetComponent<Entity>().name + " dies.");
                FindObjectOfType<Player>().GetComponent<Level>().addXP(XP);
                uiManager.NewMessage("You gain " + XP + " experience.");
                GetComponent<Entity>().KillEntity();
            }
            else if(name == "Player")
            {
                Debug.Log("Player Dead");
                //end game
            }
      
        }
    }

    public void attack(Fighter target)
    {
        int damage = power - target.defense;

        if (damage > 0)
        {
            //Debug.Log(target + " takes " + damage + " damage from " + this.GetComponent<Entity>().name + ".");
            uiManager.NewMessage(target.name + " " + "takes<size=200%><voffset=-.2em><#FF6C11> " +  damage + "</size></voffset></color> damage from " + this.GetComponent<Entity>().name + ".");
            //add kills message
            target.takeDamage(damage);
        }
        else
        {
            //Debug.Log(this.GetComponent<Entity>().name + " attacks " + target + " but deals no damage.");
            uiManager.NewMessage(this.GetComponent<Entity>().name + " attacks " + target.name + " but deals no damage.");
        }
    }

    public void heal(int amount)
    {
        HP = HP + amount;
        if (HP > MaxHP)
        {
            HP = MaxHP;
        }
        //uiManager.SetHealthText(HP);
        uiManager.SetUIText();
    }
}
