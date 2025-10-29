using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sakura_Skill1: Card
{
    public override bool UseCondition()
    {
        if (Character.currentPhase != Combat_Character.Phase.Main)
            return false;

        if (stats.mana > Character.Mana)
            return false;

        return true;
    }

    public override IEnumerator SetUp()   // parameter can be changed to submenu_controller_2
    {

        //character.TurnController.descriptionBox.ATK_Num.text = skill_Stats.attack.ToString();
        //character.TurnController.descriptionBox.HIT_Num.text = skill_Stats.accuracy.ToString();
        //character.TurnController.descriptionBox.CRT_Num.text = skill_Stats.critical.ToString();

        yield return CharacterTargeting();
    }

    public override IEnumerator Action()
    {

        Character.enemyTransform = chosen_Targets[0];

        /*CAMERA CONTROL*/

        //character.mCamera.BlackOut(0.9f, 0.5f);

        Vector3 camTargetPos = new Vector3(chosen_Targets[0].position.x, 0.45f, chosen_Targets[0].position.z - 2f);

        yield return Character.TurnController.mainCamera.MovingTo(camTargetPos, 0.5f);

        yield return Character.MoveInRange(new Vector3(-distance, 0, 0));

        Character.animationController.Clip(animationName);

        yield return Character.WaitForKeyFrame();

        //yield return character.TurnController.Reactions(Turn_Controller.Stage.IMPACT, info[0]);

        CoroutineWithData cwd = new CoroutineWithData(Character, Character.TurnController.Reactions(Turn_Controller.Stage.IMPACT));
        yield return cwd.coroutine;

        GetOutcome(stats, chosen_Targets[0].GetComponent<Combat_Character>());

        Coroutine knockback = Character.StartCoroutine(Character.enemyTransform.GetComponent<Combat_Character>().MoveAmount(new Vector3(stats.knockBack.x * Character.Facing, stats.knockBack.y, stats.knockBack.z), 0.1f));

        //CritSuccess = 3;

        Coroutine outcome;


        switch ((int)cwd.result)
        {
            case 0:

                print("Continue");

                outcome = Character.StartCoroutine(Character.ApplyOutcome(HitSuccess, CritSuccess, Character.GetCurrentStats(stats)[Character_Stats.Stat.STR]));

                yield return Character.animationController.coroutine;

                yield return outcome;

                break;

            case 1:
                print("Pause");
                break;

            case 2:
                print("Break");
                yield break;

        }

        yield return knockback;
    }
}
