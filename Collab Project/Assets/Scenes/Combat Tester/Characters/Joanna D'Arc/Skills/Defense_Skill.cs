using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense_Skill : Skill
{
    public override bool UseCondition()
    {
        if (Character.currentPhase != Combat_Character.Phase.Main)
            return false;

        if (manaCost > Character.Mana())
            return false;

        if(discard)
            if (Character.cards.hand.Count < 1)
                return false;

        if (Character.blocking)
            return false;

        return true;
    }

    public override IEnumerator SetUp()
    {
        yield return Character.cards.DiscardCards();

        yield return CharacterTargeting();
    }

    public override IEnumerator Execute()
    {
        Character.animationController.Clip(animationName);

        GetOutcome(chosen_Targets[0].GetComponent<Combat_Character>());

        yield return Character.WaitForKeyFrame();

        yield return new WaitUntil(() => Character.cards.cardRemoved == true);

        Character.animationController.Pause();
    }

    public override IEnumerator Resolve()
    {
        Character.animationController.Play();

        Character.blocking = true;

        //yield return null;
        yield return Character.animationController.coroutine;
    }
}
