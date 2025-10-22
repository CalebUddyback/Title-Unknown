using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sakura_Skill2 : Card
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
        yield return CharacterTargeting();
    }

    public override IEnumerator Action()
    {
        Character.animationController.Clip("Sakura Buff");

        yield return Character.WaitForKeyFrame();

        // Should try to push thorough ApplyOutcome eventually


        GetOutcome(stats, chosen_Targets[0].GetComponent<Combat_Character>());

        //CritSuccess = 3;

        int totalHeal = Random.Range(stats.DamageVariation.x, stats.DamageVariation.y + 1) * CritSuccess;

        foreach (Transform target in chosen_Targets)
        {
            var currentTarget = target.GetComponent<Combat_Character>();

            currentTarget.Health += totalHeal;

            //Character.Mana += skill_Stats[0].mana;

            Instantiate(currentTarget.outcome_Bubble_Prefab, Character.TurnController.mainCamera.UIPosition(currentTarget.outcome_Bubble_Pos.position), Quaternion.identity, Character.TurnController.damage_Bubbles).Input(totalHeal);

            //Instantiate(character.outcome_Bubble_Prefab, character.outcome_Bubble_Pos.position, Quaternion.identity).Input(totalHeal);
        }

        //Character.SetSkill(this, image);

        yield return Character.animationController.coroutine;
    }
}
