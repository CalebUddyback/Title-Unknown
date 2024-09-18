using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName ="New Character Stats", menuName ="Character Stats")]
public class Character_Stats : ScriptableObject
{
    [SerializeField]
    int
        health,
        strength,
        defense,
        magic,
        resistance,
        dexterity,
        luck;

    [SerializeField]
    float
        speed;

    public Weapon weapon;

    public enum Stat
    {
        HP,

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

    [Header("Buffs/Debuffs")]
    [SerializeField]
    private List<StatChanger> statChangers = new List<StatChanger>();

    private Dictionary<Stat, float> GetCoreStats()
    {
        var baseStats = new Dictionary<Stat, float>
        {
            {Stat.HP, health },
            {Stat.STR, strength },
            {Stat.MAG, magic },
            {Stat.DEX, dexterity },
            {Stat.SPD, speed},
            {Stat.DEF, defense},
            {Stat.RES, resistance},
            {Stat.LCK, luck},
        };

        return baseStats;
    }

    private Dictionary<Stat, float> GetOffenseStats(Dictionary<Stat, float> core)
    {

        var offenseStats = new Dictionary<Stat, float>()
        {
            {Stat.ATK, core[Stat.STR]},

            {Stat.PhHit, core[Stat.DEX]},
            {Stat.PhAvo, core[Stat.SPD]},

            {Stat.MgHit, (core[Stat.DEX] + core[Stat.LCK]) / 2 },
            {Stat.MgAvo, (core[Stat.SPD] + core[Stat.LCK]) / 2 },

            {Stat.Crit, (core[Stat.DEX] + core[Stat.LCK]) / 2 },
            {Stat.CritAvo, core[Stat.LCK] },

            {Stat.AS, 3 - (core[Stat.SPD]/10)},
        };

        return offenseStats;
    }

    private Dictionary<Stat, float> GetBaseStats()
    {
        var baseStats = GetCoreStats();

        foreach (var stat in GetOffenseStats(baseStats))
            baseStats.Add(stat.Key, stat.Value);

        baseStats[Stat.ATK] += weapon.attack;
        baseStats[Stat.PhHit] += weapon.accuracy;
        baseStats[Stat.Crit] += weapon.critical;

        return baseStats;
    }

    public Dictionary<Stat, float> GetCurrentStats()
    {
        Dictionary<Stat, float> currentStats = GetCoreStats();

        foreach (StatChanger changer in statChangers)
        {
            foreach (KeyValuePair<Stat, float> stat in changer.statChanges)
            {
                if (currentStats.ContainsKey(stat.Key))
                    currentStats[stat.Key] += stat.Value;
            }
        }

        Dictionary<Stat, float> offenseStats = GetOffenseStats(currentStats);

        foreach (StatChanger changer in statChangers)
        {
            foreach (KeyValuePair<Stat, float> stat in changer.statChanges)
            {
                if (offenseStats.ContainsKey(stat.Key))
                    offenseStats[stat.Key] += stat.Value;
            }
        }

        foreach (var stat in offenseStats)
            currentStats.Add(stat.Key, stat.Value);

        currentStats[Stat.ATK] += weapon.attack;
        currentStats[Stat.PhHit] += weapon.accuracy;
        currentStats[Stat.Crit] += weapon.critical;

        return currentStats;
    }


    public Dictionary<Stat, float> GetCombatStats(Combat_Character.Skill.Skill_Stats skillStats)
    {
        Dictionary<Stat, float> combatStats = GetCurrentStats();

        combatStats[Stat.ATK] += skillStats.attack;
        combatStats[Stat.PhHit] += skillStats.accuracy;
        combatStats[Stat.Crit] += skillStats.critical;

        if (skillStats.statChanger.GetStat(Stat.AS))
            combatStats[Stat.AS] += skillStats.statChanger.statChanges[Stat.AS];

        return combatStats;
    }

    public Dictionary<Stat, float> GetCombatStats(Combat_Character.Skill.Skill_Stats skillStats, Transform target)
    {
        return GetCombatStats(skillStats, target.GetComponent<Combat_Character>());
    }

    public Dictionary<Stat, float> GetCombatStats(Combat_Character.Skill.Skill_Stats skillStats, Combat_Character target)
    {
        Dictionary<Stat, float> ownerStats = GetCombatStats(skillStats);

        Dictionary<Stat, float> targetStats = target.character_Stats.GetCurrentStats();

        ownerStats[Stat.ATK] = Mathf.Clamp(ownerStats[Stat.ATK] - targetStats[Stat.DEF], 0, 9999);

        ownerStats[Stat.PhHit] = Mathf.Clamp(ownerStats[Stat.PhHit] - targetStats[Stat.PhAvo], 0, 100);

        ownerStats[Stat.Crit] = Mathf.Clamp(ownerStats[Stat.Crit] - targetStats[Stat.CritAvo], 0, 100);

        return ownerStats;
    }

    public int CompareStat(Stat stat, float value)
    {
        if (GetBaseStats()[stat] < value)
            return 1;
        else if (GetBaseStats()[stat] > value)
            return -1;
        else
            return 0;
    }

    public Dictionary<Stat, float> CompareAllStats()
    {
        var newDirectory = GetBaseStats();

        for (int i = 0; i < newDirectory.Count; i++)
        {
            Stat key = newDirectory.ElementAt(i).Key;

            newDirectory[key] = GetCurrentStats()[key] - GetBaseStats()[key];
        }

        return newDirectory;
    }


    public void AddStatChanger(StatChanger statChanger)
    {
        statChangers.Add(statChanger);
    }

    public void RemoveStatChanger(StatChanger statChanger)
    {
        statChangers.Remove(statChanger);
    }

    public void IncrementStatChangers()
    {
        for (int i = 0; i < statChangers.Count;)
        {
            if (statChangers[i].duration <= 0)
            {
                statChangers.RemoveAt(i);
            }
            else
            {
                statChangers[i].duration += statChangers[i].incrementDirection;
                i++;
            }
        }
    }

    public void ClearStatChangers()
    {
        statChangers.Clear();
    }

    ///Eventually...
    ///Add weight to fighter. Items, eg. Potions; add weight which slows fighter. decrease weight if item is used
}
