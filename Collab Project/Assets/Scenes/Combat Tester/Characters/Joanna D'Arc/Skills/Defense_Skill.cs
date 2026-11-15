using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Defense_Skill : Skill
{
    public override bool UseCondition()
    {
        return base.UseCondition();
    }

    public override IEnumerator SetUp()
    {
        yield return owner.deck.DiscardCards();

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
        //Character.animationController.PlaySpeed(1)

        Coroutine outcome = owner.StartCoroutine(owner.ApplyOutcome(this));

        yield return outcome;

        //Character.AdjustDefense(null, defense, "", 1);
        //
        //yield return Character.animationController.coroutine;
        //
        //Character.animationController.Clip("Idle", 1);
    }
}
