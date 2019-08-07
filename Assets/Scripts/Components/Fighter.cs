using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

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
       


    public void TakeDamage(int damage)
    {
        HP -= damage;
        if(name == "Player")
        {
            uiManager.SetUIText();
            if(damage > 0)
            {
                uiManager.DamageFlash();
            }
        }

        if(HP <= 0)
        {
            if(name != "Player")
            {
                uiManager.NewMessage("The " + this.GetComponent<Entity>().name + " dies.");
                FindObjectOfType<Player>().GetComponent<Level>().addXP(XP);
                uiManager.NewMessage("You gain<size=200%><voffset=-.2em><#F6019D> " + XP + "</size></voffset></color> experience.");
                GetComponent<Entity>().KillEntity();
            }
            else if(name == "Player")
            {
                Debug.Log("Player Dead");
                //end game
            }
      
        }
    }

    public void Attack(Fighter target)
    {
        int damage = power - target.defense;

        if (damage > 0)
        {
            //Debug.Log(target + " takes " + damage + " damage from " + this.GetComponent<Entity>().name + ".");
            uiManager.NewMessage(target.name + " " + "takes<size=200%><voffset=-.2em><#FF6C11> " +  damage + "</size></voffset></color> damage from " + this.GetComponent<Entity>().name + ".");
            //add kills message
            target.TakeDamage(damage);
        }
        else
        {
            //Debug.Log(this.GetComponent<Entity>().name + " attacks " + target + " but deals no damage.");
            uiManager.NewMessage(this.GetComponent<Entity>().name + " attacks " + target.name + " but deals no damage.");
        }
    }

    public void Heal(int amount)
    {
        HP = HP + amount;
        if (HP > MaxHP)
        {
            HP = MaxHP;
        }
        uiManager.SetUIText();
    }


    public List<HoT> HoTs = new List<HoT>();

    public void NanoHeal(int amount, int duration)
    {
        HoTs.Add(new HoT(amount, duration));
    }

    public void UpdateHoTs()
    {
        if (!HoTs.Any())
        {
            return;
        }
        for (int i = 0; i < HoTs.Capacity; i++)
        {
            if(HoTs[i].duration > 0)
            {
                HoTs[i].duration--;
                Heal(HoTs[i].amount);
                return;
            }
            else
            {
                HoTs.Remove(HoTs[i]);
                return;
            }
        }
    }
}
