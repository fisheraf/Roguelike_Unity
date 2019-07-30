using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Equippable : MonoBehaviour
{
    public EquipmentSlots.Equipmentslot slot;
    Inventory inventory = null;
    Engine engine = null;
    Equipment equipment = null;

    public bool equipped;

    public int hpBonus = 0;
    public int defenseBonus = 0;
    public int powerBonus = 0;
    //string slotName = null;

    private void Start()
    {
        inventory = FindObjectOfType<Inventory>();
        engine = FindObjectOfType<Engine>();
        equipment = FindObjectOfType<Equipment>();
    }

    public void ToggleEquip()
    {
        if(slot == EquipmentSlots.Equipmentslot.MainHand)
        {
            if(equipment.mainHand != null)
            {
                equipment.mainHand.equipped = false;
            }
            equipment.mainHand = this;
            equipped = !equipped;
        }

        if (slot == EquipmentSlots.Equipmentslot.OffHand)
        {
            if (equipment.offHand != null)
            {
                equipment.offHand.equipped = false;
            }
            equipment.offHand = this;
            equipped = !equipped;
        }


        //set bonuses from equipment

        inventory.CloseInventory();
        engine.gameState = Engine.GameState.EnemyTurn;
    }

    public void ToggleEquipLoad()
    {
        if(equipment == null)
        {
            equipment = FindObjectOfType<Equipment>();
        }

        if (slot == EquipmentSlots.Equipmentslot.MainHand)
        {
            if (equipment.mainHand != null)
            {
                equipment.mainHand.equipped = false;
            }
            equipment.mainHand = this;
            equipped = !equipped;
        }

        if (slot == EquipmentSlots.Equipmentslot.OffHand)
        {
            if (equipment.offHand != null)
            {
                equipment.offHand.equipped = false;
            }
            equipment.offHand = this;
            equipped = !equipped;
        }
    }

    public void SetEquipment(int eNumber)
    {
        if(eNumber == 101)
        {
            gameObject.name = "Sword";
            hpBonus = 0;
            powerBonus = 3;
            defenseBonus = 0;
            //slotName = "main hand";
            slot = EquipmentSlots.Equipmentslot.MainHand;
        }
        else if(eNumber == 102)
        {
            gameObject.name = "Shield";
            hpBonus = 0;
            powerBonus = 0;
            defenseBonus = 1;
            //slotName = "off hand";
            slot = EquipmentSlots.Equipmentslot.OffHand;
        }
    }
}
