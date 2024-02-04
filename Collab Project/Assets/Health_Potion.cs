using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Health_Potion : Item
{
    public override IEnumerator Use(Character character)
    {
        yield return new WaitForSeconds(1);

        character.health += 10;

        print("Health increased!");

        yield return null;
    }
}
