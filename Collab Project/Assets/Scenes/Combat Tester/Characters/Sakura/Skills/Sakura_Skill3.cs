using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sakura_Skill3 : Skill
{
    public override bool UseCondition()
    {
        if (owner.currentPhase != Combat_Character.Phase.Main)
            return false;

        if (manaCost > owner.Mana)
            return false;

        return true;
    }

    public override IEnumerator SetUp()
    {
        //yield return CharacterTargeting();

        yield return null;
    }

    public override IEnumerator Execute()
    {
        GetOutcome(chosen_Targets[0].GetComponent<Combat_Character>());

        owner.animationController.Clip("Buff", 1);

        yield return owner.WaitForKeyFrame(0);
    }

    public override IEnumerator Resolve()
    {
        yield return new WaitUntil(() => owner.deck.cardRemoved == true);

        owner.animationController.PlaySpeed(1);

        yield return owner.deck.DrawCards(2, true, true);

        yield return owner.animationController.coroutine;
    }
}
