using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parry_Skill : Skill
{
    public override bool SetCondition()
    {
        if (Character.currentPhase != Combat_Character.Phase.Main)
            return false;

        return true;
    }

    public override bool ReactCondition()
    {
        if (!set)
            return false;

        if (Character.TurnController.resolveStack.Count <= 0)
            return false;

        //if (manaCost > Character.Mana())
        //    return false;
        //
        //if (!skill.chosen_Targets.Contains(Character.transform))
        //    return false;
        //if (skill.selection != Selection.Singular)
        //    return false;
        //
        //if (skill.range != Range.Close)
        //    return false;
        //
        //if (skill.type != Type.Offensive && skill.type != Type.Defensive)
        //    return false;

        return true;
    }

    public override IEnumerator SetUp()
    {

        yield return CharacterTargeting();

        yield return null;
    }

    public override IEnumerator Execute()
    {

        Character.enemyTransform = chosen_Targets[0];

        Character.TurnController.resolveStack[Character.TurnController.resolveStack.Count - 2].negated = true;

        Character.animationController.Play();

        Character.animationController.Clip(animationName);

        GetOutcome(Character.TurnController.characterTurn);

        yield return Character.WaitForKeyFrame();

        Character.animationController.Pause();

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
