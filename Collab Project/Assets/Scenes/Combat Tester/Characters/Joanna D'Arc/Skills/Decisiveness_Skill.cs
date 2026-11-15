using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Decisiveness_Skill : Skill
{
    public override bool UseCondition()
    {
        if (owner.currentPhase != phase)
            return false;

        if (manaCost + owner.Mana <= 0)
            return false;

        if (owner.deck.drawDeck.childCount < 3)
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
        owner.enemyTransform = chosen_Targets[0].transform;

        //GetOutcome(chosen_Targets[0].GetComponent<Combat_Character>());

        /*CAMERA CONTROL*/

        //character.mCamera.BlackOut(0.9f, 0.5f);

        Vector3 camTargetPos = new Vector3(chosen_Targets[0].transform.position.x, 0.45f, chosen_Targets[0].transform.position.z - 2f);

        yield return owner.TurnController.mainCamera.MovingTo(camTargetPos, 0.5f);

        owner.animationController.Clip(animationName, 1f);

        yield return owner.WaitForKeyFrame(0);

        stage = Stage.Impact;

        CoroutineWithData cwd = new CoroutineWithData(owner, owner.TurnController.Reactions());
        yield return cwd.coroutine;
    }

    public override IEnumerator Resolve()
    {
        owner.animationController.PlaySpeed(1);

        yield return owner.WaitForKeyFrame(0);

        TurnController.draw_Selection.gameObject.SetActive(true);
        yield return StartCoroutine(TurnController.draw_Selection.ChooseCard());

        owner.animationController.PlaySpeed(1);

        yield return owner.animationController.coroutine;

        owner.animationController.Clip("Idle", 1);

        yield return TurnController.mainCamera.Reset(0.2f);

        //Coroutine outcome = Character.StartCoroutine(Character.ApplyOutcome(this));
        //
        //yield return outcome;

    }
}
