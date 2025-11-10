using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sakura_Skill3 : Skill
{
    public override bool UseCondition()
    {
        if (Character.currentPhase != Combat_Character.Phase.Main)
            return false;

        if (manaCost > Character.Mana())
            return false;

        return true;
    }

    public override IEnumerator SetUp()
    {
        yield return CharacterTargeting();
    }

    public override IEnumerator Action()
    {
        Character.animationController.Clip("Buff");

        yield return Character.WaitForKeyFrame();

        // Should try to push thorough ApplyOutcome eventually


        GetOutcome(intervals, chosen_Targets[0].GetComponent<Combat_Character>());

        yield return new WaitUntil(() => Character.hand.cardRemoved == true);

        yield return Character.hand.DrawCards(2, true);

        yield return Character.animationController.coroutine;
    }
}
