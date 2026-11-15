using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Default_Skill: Skill
{
    public override bool UseCondition()
    {
        return base.UseCondition();
    }

    public override IEnumerator SetUp()   // parameter can be changed to submenu_controller_2
    {

        //character.TurnController.descriptionBox.ATK_Num.text = skill_Stats.attack.ToString();
        //character.TurnController.descriptionBox.HIT_Num.text = skill_Stats.accuracy.ToString();
        //character.TurnController.descriptionBox.CRT_Num.text = skill_Stats.critical.ToString();

        //yield return CharacterTargeting();

        yield return null;
    }

    public override IEnumerator Execute()
    {
 
        owner.enemyTransform = chosen_Targets[0].transform;

        GetOutcome(chosen_Targets[0].GetComponent<Combat_Character>());

        /*CAMERA CONTROL*/

        //character.mCamera.BlackOut(0.9f, 0.5f);

        Vector3 camTargetPos = new Vector3(chosen_Targets[0].transform.position.x, 0.45f, chosen_Targets[0].transform.position.z - 2f);

        yield return owner.TurnController.mainCamera.MovingTo(camTargetPos, 0.5f);

        stage = Stage.Moving;

        yield return owner.MoveInRange(-(distance));

        owner.animationController.Clip(animationName, 1f);

        yield return owner.WaitForKeyFrame(0);

        stage = Stage.Impact;

        CoroutineWithData cwd = new CoroutineWithData(owner, owner.TurnController.Reactions());
        yield return cwd.coroutine;
    }

    public override IEnumerator Resolve()
    {

        switch (type)
        {
            case Type.Offensive:

                yield return owner.StartCoroutine(owner.ApplyOutcome(this));

                break;

            case Type.Spell:

                //yield return new WaitForSeconds(ImpactDelay); // Impact Delay

                owner.animationController.PlaySpeed(1);

                yield return owner.WaitForKeyFrame(0);

                if (target.defense != 0)
                    chosen_Targets[0].AdjustDefense(null, target.defense, "", 1);

                if (target.mana != 0)
                    chosen_Targets[0].AdjustMana(target.mana, true);

                if (target.health != 0)
                    chosen_Targets[0].AdjustHealth(null, target.health, "", 1);

                owner.animationController.PlaySpeed(1);

                yield return owner.animationController.coroutine;

                owner.animationController.Clip("Idle", 1);

                yield return TurnController.mainCamera.Reset(0.2f);

                break;
        }
    }
}
