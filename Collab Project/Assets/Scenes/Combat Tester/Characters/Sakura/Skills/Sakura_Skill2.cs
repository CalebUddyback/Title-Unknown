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

    public override IEnumerator Action()
    {
        Character.animationController.Clip("Buff");

        yield return Character.WaitForKeyFrame();

        // Should try to push thorough ApplyOutcome eventually


        GetOutcome(intervals, chosen_Targets[0].GetComponent<Combat_Character>());

        //CritSuccess = 3;

        int totalHeal = Random.Range(intervals.DamageVariation.x, intervals.DamageVariation.y + 1) * CritSuccess;

        foreach (Transform target in chosen_Targets)
        {
            var currentTarget = target.GetComponent<Combat_Character>();

            currentTarget.Health(totalHeal, CritSuccess);

            //Character.Mana += skill_Stats[0].mana;

            //Instantiate(character.outcome_Bubble_Prefab, character.outcome_Bubble_Pos.position, Quaternion.identity).Input(totalHeal);
        }

        //Character.SetSkill(this, image);

        yield return Character.animationController.coroutine;
    }
}
