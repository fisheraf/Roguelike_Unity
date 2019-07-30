using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;

public class Equipment : MonoBehaviour
{
    Engine engine = null;

    public List<Equippable> equipmentList = new List<Equippable>();
    public List<int> eqIDNumber = new List<int>();
    public List<bool> isEquippedList = new List<bool>();

    GameObject panel;
    //[SerializeField] Button[] button = null;

    public Equippable mainHand;
    public Equippable offHand;

    private void Start()
    {
        engine = FindObjectOfType<Engine>();
    }


    public int HPBonus()
    {
        int bonus = 0;

        foreach (Equippable equipment in equipmentList)
        {
            if(equipment.equipped)
            {
                bonus += equipment.hpBonus;
            }
        }

        return bonus;
    }

    public int PowerBonus()
    {
        int bonus = 0;

        foreach (Equippable equipment in equipmentList)
        {
            if (equipment.equipped)
            {
                bonus += equipment.powerBonus;
            }
        }

        return bonus;
    }

    public int DefenseBonus()
    {
        int bonus = 0;

        foreach (Equippable equipment in equipmentList)
        {
            if (equipment.equipped)
            {
                bonus += equipment.defenseBonus;
            }
        }

        return bonus;
    }


    public void SaveEquipped()
    {
        isEquippedList.Clear();
        eqIDNumber.Clear();

        for (int i = 0; i < equipmentList.Count; i++)
        {
            if(equipmentList[i].equipped)
            {
                isEquippedList.Add(true);
            }
            else
            {
                isEquippedList.Add(false);
            }
            if(equipmentList[i].name == "Sword")
            {
                eqIDNumber.Add(101);
            }
            if (equipmentList[i].name == "Shield")
            {
                eqIDNumber.Add(102);
            }
        }


    }

    public void LoadEquipped()
    {     
        for (int i = 0; i < eqIDNumber.Count; i++)
        {
            //Debug.Log(i);
            int x = eqIDNumber[i];
            //equipmentList[i].equipped = isEquippedList[i];



            if(isEquippedList[i])
            {
                //Debug.Log("toggle");
                equipmentList[i].ToggleEquipLoad();
            }

            /*
            if (x == 101)
            {
                //GameObject eqObject = Instantiate(engine.equipmentObject, new Vector3(-10, -10, -1), Quaternion.identity);
                //eqObject.SetActive(true);
                //equipmentList[i].GetComponent<Equippable>().SetEquipment(101);

                //equipmentList.Add(eqObject.GetComponent<Equippable>());
                //eqObject.SetActive(false);                
            }
            if (x == 102)
            {
                //GameObject eqObject = Instantiate(engine.equipmentObject, new Vector3(-10, -10, -1), Quaternion.identity);
                //eqObject.GetComponent<Equippable>().SetEquipment(102);

                //equipmentList.Add(eqObject.GetComponent<Equippable>());
                //eqObject.SetActive(false);
            }
            */
            
        }
    }

}
