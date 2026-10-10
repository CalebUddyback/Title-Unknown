using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sakura_Skill4 : Skill
{
    public override bool UseCondition()
    {
        if (Character.currentPhase != Combat_Character.Phase.Main)
            return false;

        if (manaCost > Character.Mana)
            return false;

        if(discard)
            if (Character.deck.hand.Count < 1)
                return false;

        if (Character.deck.hand.Count < 2)
            return false;

        return true;
    }

    public override IEnumerator SetUp()
    {
        yield return Character.deck.DiscardCards();

        //yield return CharacterTargeting();

        yield return null;
    }

    public override IEnumerator Execute()
    {
        GetOutcome(chosen_Targets[0].GetComponent<Combat_Character>());

        Character.animationController.Clip("Buff", 1);

        yield return Character.WaitForKeyFrame(0);
    }

    public override IEnumerator Resolve()
    {
        yield return new WaitUntil(() => Character.deck.cardRemoved == true);

        Character.animationController.PlaySpeed(1);

        Character.AdjustMana(mana, true);

        yield return Character.animationController.coroutine;
    }
}
