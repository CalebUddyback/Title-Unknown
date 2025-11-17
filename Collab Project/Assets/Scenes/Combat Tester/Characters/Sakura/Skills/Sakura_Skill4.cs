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

        if(discard)
            if (Character.decks.hand.Count < 1)
                return false;

        if (Character.decks.hand.Count < 2)
            return false;

        return true;
    }

    public override IEnumerator SetUp()
    {
        yield return Character.decks.DiscardCards();

        yield return CharacterTargeting();
    }

    public override IEnumerator Execute()
    {
        GetOutcome(chosen_Targets[0].GetComponent<Combat_Character>());

        Character.animationController.Clip("Buff");

        yield return Character.WaitForKeyFrame();

        Character.animationController.Pause();
    }

    public override IEnumerator Resolve()
    {
        yield return new WaitUntil(() => Character.decks.cardRemoved == true);

        Character.animationController.Play();

        Character.AdjustMana(mana, true);

        yield return Character.animationController.coroutine;
    }
}
