using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName ="New Stats", menuName ="Stats")]
public class Stats : ScriptableObject
{
    [SerializeField]
    int
        maxHealth,
        strength,
        defense,
        magic,
        resistance,
        dexterity,
        speed,
        luck;


    public enum Stat
    {
        MaxHP,

        STR,
        MAG,
        DEX,
        SPD,
        DEF,
        RES,
        LCK,

        PhHit,
        PhAvo,

        MgHit,
        MgAvo,

        Crit,
        CritAvo,
        AS
    };

    [Header("Buffs/Debuffs")]
    public List<StatChanger> statChangers = new List<StatChanger>();

    private Dictionary<Stat, int> GetBaseStats()
    {
        var baseDirectory = new Dictionary<Stat, int>
        {
            {Stat.MaxHP, maxHealth },
            {Stat.STR, strength },
            {Stat.MAG, magic },
            {Stat.DEX, dexterity },
            {Stat.SPD, speed},
            {Stat.DEF, defense},
            {Stat.RES, resistance},
            {Stat.LCK, luck},

            {Stat.PhHit, dexterity},
            {Stat.PhAvo, speed},

            {Stat.MgHit, (dexterity + luck) / 2 },
            {Stat.MgAvo, (speed + luck) / 2 },

            {Stat.Crit, (dexterity + luck) / 2 },
            {Stat.CritAvo, luck },

            {Stat.AS, speed},
        };

        return baseDirectory;
    }

    public Dictionary<Stat, int> GetCurrentStats()
    {
        Dictionary<Stat, int> currentDirectory = GetBaseStats();

        foreach (StatChanger buff in statChangers)
        {
            foreach (KeyValuePair<Stat, int> stat in buff.statChanges)
            {
                currentDirectory[stat.Key] += stat.Value;
            }
        }

        return currentDirectory;
    }

    public Dictionary<Stat, int> GetCurrentStats(Combat_Character.Skill skill)
    {
        Dictionary<Stat, int> currentDirectory = GetCurrentStats();

        currentDirectory[Stat.Crit] += skill.baseInfo[0].critical;
        currentDirectory[Stat.PhHit] += skill.baseInfo[0].accuracy;

        return currentDirectory;
    }

    public Dictionary<Stat, int> GetCombatStats( Combat_Character target)
    {
        Dictionary<Stat, int> ownerDirectory = GetCurrentStats();
        Dictionary<Stat, int> targetDirectory = target.stats.GetCurrentStats();

        ownerDirectory[Stat.Crit] -= targetDirectory[Stat.CritAvo];
        ownerDirectory[Stat.PhHit] -= targetDirectory[Stat.PhAvo];
        //ownerDirectory[Stat.MgHit] -= targetDirectory[Stat.MgAvo];

        return ownerDirectory;
    }


    public Dictionary<Stat, int> CompareStats()
    {
        var newDirectory = GetBaseStats();

        for (int i = 0; i < newDirectory.Count; i++)
        {
            Stat key = newDirectory.ElementAt(i).Key;

            newDirectory[key] = GetCurrentStats()[key] - GetBaseStats()[key];
        }

        return newDirectory;
    }

    /// At the beggining of turn apply buffs to opponent

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


    public bool RNG(int x, int y)
    {
        int max = x + y;

        int roll = UnityEngine.Random.Range(0, max);

        if (roll < x)
            return true;
        else
            return false;
    }


    ///Eventually...
    ///Add weight to fighter. Items, eg. Potions; add weight which slows fighter. decrease weight if item is used
}
