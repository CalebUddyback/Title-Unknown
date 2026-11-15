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

        owner.animationController.Clip("Buff", 1);

        yield return owner.WaitForKeyFrame(0);
    }

    public override IEnumerator Resolve()
    {
        owner.animationController.PlaySpeed(1);

        int totalHeal = target.health * CritSuccess;

        foreach (Combat_Character target in chosen_Targets)
        {
            var currentTarget = target;

            currentTarget.AdjustHealth(null, totalHeal, "", CritSuccess);
        }

        yield return owner.animationController.coroutine;
    }
}
