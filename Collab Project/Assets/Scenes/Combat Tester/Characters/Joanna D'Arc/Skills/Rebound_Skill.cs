using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rebound_Skill : Skill
{
    public override bool UseCondition()
    {
        if (owner.currentPhase != Combat_Character.Phase.Main)
            return false;

        if (manaCost > owner.Mana)
            return false;

        //if (!Character.blocking)
        //    return false;

        return true;
    }

    public override IEnumerator SetUp()
    {
        //yield return CharacterTargeting();

        yield return null;
    }

    public override IEnumerator Execute()
    {
        owner.animationController.Clip(animationName, 1);

        GetOutcome(chosen_Targets[0].GetComponent<Combat_Character>());

        yield return owner.WaitForKeyFrame(0);
    }

    public override IEnumerator Resolve()
    {
        owner.animationController.PlaySpeed(1);

        owner.AdjustMana(target.mana, true);

        yield return owner.animationController.coroutine;

        owner.animationController.Clip("Block_Set", 1);

        yield return owner.animationController.coroutine;
    }
}
