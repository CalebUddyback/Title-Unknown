using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense_Skill : Card
{
    public override bool UseCondition()
    {
        if (Character.currentPhase != Combat_Character.Phase.Main)
            return false;

        if (stats.mana > Character.Mana)
            return false;

        return true;
    }

    public override IEnumerator SetUp()
    {
        yield return Character.hand.DiscardCards(1);

        yield return CharacterTargeting();
    }

    public override IEnumerator Action()
    {
        Character.animationController.Clip("Block_Set");

        GetOutcome(stats, chosen_Targets[0].GetComponent<Combat_Character>());

        Character.blocking = true;

        yield return new WaitUntil(() => Character.hand.cardRemoved == true);

        yield return Character.animationController.coroutine;
    }
}
