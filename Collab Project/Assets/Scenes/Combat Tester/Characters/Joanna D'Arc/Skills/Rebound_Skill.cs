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

    public override IEnumerator Execute()
    {
        Character.animationController.Clip(animationName);

        GetOutcome(chosen_Targets[0].GetComponent<Combat_Character>());

        yield return Character.WaitForKeyFrame();

        Character.animationController.Pause();
    }

    public override IEnumerator Resolve()
    {
        Character.animationController.Play();

        Character.AdjustMana(mana, true);

        yield return Character.animationController.coroutine;

        Character.animationController.Clip("Block_Set");

        yield return Character.animationController.coroutine;
    }
}
