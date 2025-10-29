using UnityEngine;

[CreateAssetMenu(fileName = "New Character Stats", menuName = "Character Stats")]
public class Character_Stats : ScriptableObject
{
    public int
        max_Health = 100,
        max_Mana = 100;


    public int

        initiative,   // Turn order
        speed,      // Turn amount

        strength,   // Effectivness

        critical,  // Criticals
        luck;       // Dodge

    public enum Stat
    {
        INI,
        SPD,

        STR,

        CRT,
        LCK,
    };
}
