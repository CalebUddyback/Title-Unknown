using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sakura_Skill2 : Skill
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
        Character.animationController.Play();

        int totalHeal = Random.Range(DamageVariation.x, DamageVariation.y + 1) * CritSuccess;

        foreach (Transform target in chosen_Targets)
        {
            var currentTarget = target.GetComponent<Combat_Character>();

            currentTarget.AdjustHealth(totalHeal, CritSuccess);
        }

        yield return Character.animationController.coroutine;
    }
}
