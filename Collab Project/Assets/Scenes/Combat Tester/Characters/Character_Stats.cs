using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "New Character Stats", menuName = "Character Stats")]
public class Character_Stats : ScriptableObject
{
    public int
        max_Health = 100,
        health = 100,
        max_Mana = 100,
        mana = 100;


    public int
        strength,
        defense,
        magic,
        resistance,
        dexterity,
        luck,
        speed;

    public enum Stat
    {
        STR,
        MAG,
        DEX,
        SPD,
        DEF,
        RES,
        LCK,

        ATK,

        PhHit,
        PhAvo,

        MgHit,
        MgAvo,

        Crit,
        CritAvo,

        AS
    };

    //[Header("Buffs/Debuffs")]
    //[SerializeField]
    //private List<StatChanger> statChangers = new List<StatChanger>();
    //
    //private Dictionary<Stat, int> GetCoreStats()
    //{
    //    var baseStats = new Dictionary<Stat, int>
    //    {
    //        {Stat.STR, strength },
    //        {Stat.MAG, magic },
    //        {Stat.DEX, dexterity },
    //        {Stat.SPD, speed},
    //        {Stat.DEF, defense},
    //        {Stat.RES, resistance},
    //        {Stat.LCK, luck},
    //    };
    //
    //    return baseStats;
    //}
    //
    //private Dictionary<Stat, int> GetOffenseStats(Dictionary<Stat, int> core)
    //{
    //
    //    var offenseStats = new Dictionary<Stat, int>()
    //    {
    //        {Stat.ATK, core[Stat.STR]},
    //
    //        {Stat.PhHit, core[Stat.DEX]},
    //        {Stat.PhAvo, core[Stat.SPD]},
    //
    //        {Stat.MgHit, (core[Stat.DEX] + core[Stat.LCK]) / 2 },
    //        {Stat.MgAvo, (core[Stat.SPD] + core[Stat.LCK]) / 2 },
    //
    //        {Stat.Crit, (core[Stat.DEX] + core[Stat.LCK]) / 2 },
    //        {Stat.CritAvo, core[Stat.LCK] },
    //
    //        {Stat.AS, 21 - core[Stat.SPD]},
    //    };
    //
    //    return offenseStats;
    //}
    //
    //private Dictionary<Stat, int> GetBaseStats()
    //{
    //    var baseStats = GetCoreStats();
    //
    //    foreach (var stat in GetOffenseStats(baseStats))
    //        baseStats.Add(stat.Key, stat.Value);
    //
    //    baseStats[Stat.ATK] += weapon.attack;
    //    baseStats[Stat.PhHit] += weapon.accuracy;
    //    baseStats[Stat.Crit] += weapon.critical;
    //
    //    return baseStats;
    //}
    //
    //public Dictionary<Stat, int> GetCurrentStats()
    //{
    //    var currentStats = GetCoreStats();
    //
    //    foreach (var changer in statChangers)
    //    {
    //        for (int i = 0; i < 7; i++)
    //        {
    //            Stat stat = currentStats.ElementAt(i).Key;
    //            currentStats[stat] += changer.statChanges[stat];
    //        }
    //    }
    //
    //    foreach (var stat in GetOffenseStats(currentStats))
    //        currentStats.Add(stat.Key, stat.Value);
    //
    //    foreach (var changer in statChangers)
    //    {
    //        for (int i = 7; i < 15; i++)
    //        {
    //            Stat stat = currentStats.ElementAt(i).Key;
    //            currentStats[stat] += changer.statChanges[stat];
    //        }
    //    }
    //
    //    currentStats[Stat.ATK] += weapon.attack;
    //    currentStats[Stat.PhHit] += weapon.accuracy;
    //    currentStats[Stat.Crit] += weapon.critical;
    //
    //    return currentStats;
    //}
    //
    //
    //public Dictionary<Stat, int> GetCombatStats(Combat_Character.Skill.Skill_Stats skillStats)
    //{
    //    Dictionary<Stat, int> combatStats = GetCurrentStats();
    //
    //    combatStats[Stat.ATK] += skillStats.attack;
    //    combatStats[Stat.PhHit] += skillStats.accuracy;
    //    combatStats[Stat.Crit] += skillStats.critical;
    //
    //    if (skillStats.statChanger != null)
    //        combatStats[Stat.AS] += skillStats.statChanger.statChanges[Stat.AS];
    //
    //    return combatStats;
    //}
    //
    //public Dictionary<Stat, int> GetCombatStats(Combat_Character.Skill.Skill_Stats skillStats, Transform target)
    //{
    //    return GetCombatStats(skillStats, target.GetComponent<Combat_Character>());
    //}
    //
    //public Dictionary<Stat, int> GetCombatStats(Combat_Character.Skill.Skill_Stats skillStats, Combat_Character target)
    //{
    //    Dictionary<Stat, int> ownerStats = GetCombatStats(skillStats);
    //
    //    Dictionary<Stat, int> targetStats = target.character_Stats.GetCurrentStats();
    //
    //    ownerStats[Stat.ATK] = Mathf.Clamp(ownerStats[Stat.ATK] - targetStats[Stat.DEF], 0, 9999);
    //
    //    ownerStats[Stat.PhHit] = Mathf.Clamp(ownerStats[Stat.PhHit] - targetStats[Stat.PhAvo], 0, 100);
    //
    //    ownerStats[Stat.Crit] = Mathf.Clamp(ownerStats[Stat.Crit] - targetStats[Stat.CritAvo], 0, 100);
    //
    //    return ownerStats;
    //}
    //
    //public Color CompareStat(Stat stat, int value, bool reverse)
    //{
    //    int i = 0;
    //
    //    if (GetBaseStats()[stat] < value)
    //        i = 1;
    //
    //    if (GetBaseStats()[stat] > value)
    //        i = -1;
    //
    //    if (reverse)
    //        i *= -1;
    //
    //    if (i == 1)
    //        return Color.blue;
    //    else if (i == -1)
    //        return Color.red;
    //    else
    //        return Color.white;
    //}
    //
    //public void AddStatChanger(StatChanger statChanger)
    //{
    //    statChangers.Add(statChanger);
    //}
    //
    //public void RemoveStatChanger(StatChanger statChanger)
    //{
    //    statChangers.Remove(statChanger);
    //}
    //
    //public void IncrementStatChangers(bool comboState)
    //{
    //    // Statchangers should have individual logic that is called thorugh abstract methods
    //
    //    for (int i = 0; i < statChangers.Count;)
    //    {
    //        statChangers[i].duration += statChangers[i].incrementDirection;
    //
    //        if (statChangers[i].duration <= 0)
    //            statChangers.RemoveAt(i);
    //        else
    //            i++;
    //    }
    //
    //}
    //
    //public void ClearStatChangers()
    //{
    //    statChangers.Clear();
    //}

    ///Eventually...
    ///Add weight to fighter. Items, eg. Potions; add weight which slows fighter. decrease weight if item is used
}
