using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Castle_Node : Node_Effect
{
    public override IEnumerator LandOnEffect(Character character)
    {
        //print("CASTLE");

        //character.animations.Clip("Idle " + character.IsFacing);

        character.animations.Clip("Idle Down");

        if (character.inventory.Count > 0)
        {

            yield return new WaitForSeconds(1f);

            while (character.inventory.Count > 0)
            {
                character.inventory.RemoveAt(0);
                character.gold += 1;

                yield return new WaitForSeconds(0.3f);
            }

        }

        //print("Items sold");

        yield return null;
    }

    public override IEnumerator PassEffect(Character character)
    {
        yield return StartCoroutine(LandOnEffect(character));
    }
}
