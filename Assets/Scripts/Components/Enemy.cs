using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Enemy", menuName = "Enemy", order = 51)]
public class Enemy : ScriptableObject
{
    public int MaxHP;
    public int HP;

    public int defense;
    public int power;

    public Sprite sprite;
    public Color32 color;

    new public string name;
}
