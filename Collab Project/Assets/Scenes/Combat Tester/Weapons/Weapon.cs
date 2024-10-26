using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Weapon", menuName = "Weapon")]
public class Weapon : ScriptableObject
{
    public enum Weapon_Type
    {
        sword,
        hammer,
        bow,
        spear,
        gloves,
    }
    public Weapon_Type weapon_Type;

    public string title;
    public int attack;
    public int defense;
    public int accuracy;
    public int critical;
    public int weight;
}

