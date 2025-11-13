using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parry_Skill : Skill
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
        Character.enemyTransform = Character.TurnController.characterTurn.transform;

        Character.animationController.Play();

        Character.animationController.Clip(animationName);

        GetOutcome(Character.TurnController.characterTurn);

        yield return Character.WaitForKeyFrame();

        Character.animationController.Pause();
    }

    public override IEnumerator Resolve()
    {
        Character.TurnController.characterTurn.cards.resolveStack.Peek().negated = true;

        Coroutine outcome = Character.StartCoroutine(Character.ApplyOutcome(this));

        yield return outcome;
    }
}
