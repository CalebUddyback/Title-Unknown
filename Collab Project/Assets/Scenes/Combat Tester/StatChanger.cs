using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class StatChanger
{
    public string name;

    //public Skill linkedSkill;

    public Dictionary<Stats.Stat, int> statChanges;

    public int duration;

    public bool permanent;

    public Combat_Character incrementOnTargetsTurn;       // what turn was it applied (even or odd); will increment at the start of that turn. will need to use "%" modulus
    public int incrementDirection;   // 1 to countUp
}
