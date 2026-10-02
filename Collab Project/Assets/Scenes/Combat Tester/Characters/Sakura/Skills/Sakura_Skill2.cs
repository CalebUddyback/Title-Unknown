using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sakura_Skill2 : Skill
{
    public override bool UseCondition()
    {
        return base.UseCondition();
    }

    public override IEnumerator SetUp()
    {
        //yield return CharacterTargeting();

        yield return null;
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

        int totalHeal = health * CritSuccess;

        foreach (Combat_Character target in chosen_Targets)
        {
            var currentTarget = target;

            currentTarget.AdjustHealth(null, totalHeal, "", CritSuccess);
        }

        yield return Character.animationController.coroutine;
    }
}
