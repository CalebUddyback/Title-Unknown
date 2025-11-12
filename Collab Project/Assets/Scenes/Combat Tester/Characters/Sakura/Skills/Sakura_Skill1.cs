using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sakura_Skill1: Skill
{
    public override bool UseCondition()
    {
        if (Character.currentPhase != Combat_Character.Phase.Main)
            return false;

        if (manaCost > Character.Mana())
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



    public override IEnumerator Execute()
    {
        Character.enemyTransform = chosen_Targets[0];

        GetOutcome(chosen_Targets[0].GetComponent<Combat_Character>());

        /*CAMERA CONTROL*/

        //character.mCamera.BlackOut(0.9f, 0.5f);

        Vector3 camTargetPos = new Vector3(chosen_Targets[0].position.x, 0.45f, chosen_Targets[0].position.z - 2f);

        yield return Character.TurnController.mainCamera.MovingTo(camTargetPos, 0.5f);

        yield return Character.MoveInRange(new Vector3(-(intervals.distance), 0, 0));

        Character.animationController.Clip(animationName);

        yield return Character.WaitForKeyFrame();

        Character.animationController.Pause();

        Character.Enemy.animationController.Pause();

        CoroutineWithData cwd = new CoroutineWithData(Character, Character.TurnController.Reactions(Turn_Controller.Stage.IMPACT));
        yield return cwd.coroutine;
    }

    public override IEnumerator Resolve()
    {
        Coroutine outcome = Character.StartCoroutine(Character.ApplyOutcome(this));

        yield return outcome;
    }
}
