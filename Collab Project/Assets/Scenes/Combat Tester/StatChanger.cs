using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StatChanger
{
    public string name;

    public Dictionary<Stats.Stat, int> character_statChanges;

    public Dictionary<Combat_Character.Skill.Stat, int> skill_statChanges;

    public int duration;

    public bool permanent;

    public Combat_Character incrementOnTargetsTurn;       // what turn was it applied (even or odd); will increment at the start of that turn. will need to use "%" modulus
    public int incrementDirection;   // 1 to countUp

    public void AddStatChanger(Dictionary<Stats.Stat, int> statChanges)
    {
        character_statChanges = statChanges;
    }

    public void AddStatChanger(Dictionary<Combat_Character.Skill.Stat, int> statChanges)
    {
        skill_statChanges = statChanges;
    }
}
