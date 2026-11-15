using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sakura_Skill4 : Skill
{
    public override bool UseCondition()
    {
        if (owner.currentPhase != Combat_Character.Phase.Main)
            return false;

        if (manaCost > owner.Mana)
            return false;

        if(discard)
            if (owner.deck.hand.Count < 1)
                return false;

        if (owner.deck.hand.Count < 2)
            return false;

        return true;
    }

    public override IEnumerator SetUp()
    {
        yield return owner.deck.DiscardCards();

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

        owner.AdjustMana(target.mana, true);

        yield return owner.animationController.coroutine;
    }
}
