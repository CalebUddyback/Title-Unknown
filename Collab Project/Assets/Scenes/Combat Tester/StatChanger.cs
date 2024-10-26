using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StatChanger
{
    public string name;

    public Dictionary<Character_Stats.Stat, int> statChanges;

    public int duration = 1;

    public Combat_Character incrementOnTargetsTurn;       // what turn was it applied (even or odd); will increment at the start of that turn. will need to use "%" modulus
    public int incrementDirection = -1;   // 0 makes the changer Permanent

    public StatChanger()
    {
        statChanges = new Dictionary<Character_Stats.Stat, int>(Enum.GetValues(typeof(Character_Stats.Stat)).Length);
    
        Array values = Enum.GetValues(typeof(Character_Stats.Stat));
    
        foreach (object value in values)
        {
            statChanges.Add((Character_Stats.Stat)value, 0);
        }
    }

    public StatChanger(Dictionary<Character_Stats.Stat, int> changes)
    {
        statChanges = new Dictionary<Character_Stats.Stat, int>(Enum.GetValues(typeof(Character_Stats.Stat)).Length);
    
        Array values = Enum.GetValues(typeof(Character_Stats.Stat));
    
        foreach (object value in values)
        {
            statChanges.Add((Character_Stats.Stat)value, changes.ContainsKey((Character_Stats.Stat)value) ? changes[(Character_Stats.Stat)value] : 0);
        }
    }
}
