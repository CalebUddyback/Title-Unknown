using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decisiveness_Skill : Skill
{
    public override bool UseCondition()
    {
        if (Character.currentPhase != phase)
            return false;

        if (manaCost + Character.Mana <= 0)
            return false;

        if (Character.deck.drawDeck.childCount < 3)
            return false;

        return true;
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
        Character.enemyTransform = chosen_Targets[0].transform;

        //GetOutcome(chosen_Targets[0].GetComponent<Combat_Character>());

        /*CAMERA CONTROL*/

        //character.mCamera.BlackOut(0.9f, 0.5f);

        Vector3 camTargetPos = new Vector3(chosen_Targets[0].transform.position.x, 0.45f, chosen_Targets[0].transform.position.z - 2f);

        yield return Character.TurnController.mainCamera.MovingTo(camTargetPos, 0.5f);

        Character.animationController.Clip(animationName, 1f);

        yield return Character.WaitForKeyFrame(0);

        stage = Stage.Impact;

        CoroutineWithData cwd = new CoroutineWithData(Character, Character.TurnController.Reactions());
        yield return cwd.coroutine;
    }

    public override IEnumerator Resolve()
    {
        Character.animationController.PlaySpeed(1);

        yield return Character.WaitForKeyFrame(0);

        TurnController.draw_Selection.gameObject.SetActive(true);
        yield return StartCoroutine(TurnController.draw_Selection.ChooseCard());

        Character.animationController.PlaySpeed(1);

        yield return Character.animationController.coroutine;

        Character.animationController.Clip("Idle", 1);

        yield return TurnController.mainCamera.Reset(0.2f);

        //Coroutine outcome = Character.StartCoroutine(Character.ApplyOutcome(this));
        //
        //yield return outcome;

    }
}
