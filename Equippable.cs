using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum EquipmentSlot

public class Equippable : MonoBehaviour
{
    public EquipmentSlots.Equipmentslot slot;

    public bool equipped;

    public int hpBonus;
    public int defenseBonus;
    public int powerBonus;

    public void ToggleEquip()
    {
        equipped = !equipped;
    }
}
