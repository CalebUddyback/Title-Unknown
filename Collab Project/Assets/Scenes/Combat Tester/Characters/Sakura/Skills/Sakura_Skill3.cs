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

        yield return Character.decks.DrawCards(2, true);

        yield return Character.animationController.coroutine;
    }
}
