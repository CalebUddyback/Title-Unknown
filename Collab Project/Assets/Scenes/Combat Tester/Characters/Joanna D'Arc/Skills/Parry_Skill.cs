using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parry_Skill : Skill
{
    public override bool UseCondition()
    {
        if (TurnController.resolveStack.Count <= 0)
            return false;

        if (!TurnController.resolveStack[owner.TurnController.resolveStack.Count - 1].skill.chosen_Targets.Contains(owner))
            return false;

        return base.UseCondition();
    }

    public override IEnumerator SetUp()
    {

        //yield return CharacterTargeting();

        yield return null;

    }

    public override IEnumerator Execute()
    {

        owner.enemyTransform = chosen_Targets[0].transform;

        owner.TurnController.resolveStack[owner.TurnController.resolveStack.Count - 2].negated = true;

        owner.animationController.PlaySpeed(1f);

        owner.animationController.Clip(animationName, 1);

        GetOutcome(owner.TurnController.characterTurn);

        yield return owner.WaitForKeyFrame(0);

        stage = Stage.Impact;

        CoroutineWithData cwd = new CoroutineWithData(owner, owner.TurnController.Reactions());
        yield return cwd.coroutine;
    }   

    public override IEnumerator Resolve()
    {
        Coroutine outcome = owner.StartCoroutine(owner.ApplyOutcome(this));

        yield return outcome;
    }
}
