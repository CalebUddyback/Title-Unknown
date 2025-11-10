using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sakura_Skill4 : Skill
{
    public override bool UseCondition()
    {
        if (Character.currentPhase != Combat_Character.Phase.Main)
            return false;

        if (manaCost > Character.Mana())
            return false;

        if (Character.hand.hand.Count < discards + 1)
            return false;

        if (Character.hand.hand.Count < 2)
            return false;

        return true;
    }

    public override IEnumerator SetUp()
    {
        yield return Character.hand.DiscardCards(discards);

        yield return CharacterTargeting();
    }

    public override IEnumerator Action()
    {
        Character.animationController.Clip("Buff");

        yield return Character.WaitForKeyFrame();

        Character.Mana(mana, true);

        GetOutcome(intervals, chosen_Targets[0].GetComponent<Combat_Character>());

        yield return new WaitUntil(() => Character.hand.cardRemoved == true);

        yield return Character.animationController.coroutine;
    }
}
