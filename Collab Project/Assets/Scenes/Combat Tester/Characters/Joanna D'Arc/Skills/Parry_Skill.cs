using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parry_Skill : Skill
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

        Character.enemyTransform = chosen_Targets[0].transform;

        Character.TurnController.resolveStack[Character.TurnController.resolveStack.Count - 2].negated = true;

        Character.animationController.PlaySpeed(1f);

        Character.animationController.Clip(animationName, 1);

        GetOutcome(Character.TurnController.characterTurn);

        yield return Character.WaitForKeyFrame(0);

        stage = Stage.Impact;

        CoroutineWithData cwd = new CoroutineWithData(Character, Character.TurnController.Reactions());
        yield return cwd.coroutine;
    }   

    public override IEnumerator Resolve()
    {
        Coroutine outcome = Character.StartCoroutine(Character.ApplyOutcome(this));

        yield return outcome;
    }
}
