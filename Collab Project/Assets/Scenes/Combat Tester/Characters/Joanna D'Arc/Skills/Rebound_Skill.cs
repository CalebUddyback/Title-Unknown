using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rebound_Skill : Skill
{
    public override bool UseCondition()
    {
        if (Character.currentPhase != Combat_Character.Phase.Main)
            return false;

        if (manaCost > Character.Mana())
            return false;

        if (!Character.blocking)
            return false;

        return true;
    }

    public override IEnumerator SetUp()
    {
        yield return CharacterTargeting();
    }

    public override IEnumerator Action()
    {
        int manaAmount = mana;

        Character.animationController.Clip(animationName);

        yield return Character.WaitForKeyFrame();

        Character.Mana(manaAmount, true);

        GetOutcome(intervals, chosen_Targets[0].GetComponent<Combat_Character>());

        //yield return new WaitUntil(() => Character.hand.cardRemoved == true);

        yield return Character.animationController.coroutine;

        Character.animationController.Clip("Block_Set");
    }
}
