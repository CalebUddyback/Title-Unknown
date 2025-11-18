using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Parry_Skill : Skill
{
    public override bool UseCondition()
    {
        return false;
    }

    public override bool ReactCondition(Skill skill, Turn_Controller.Stage stage)
    {
        //if (Character.currentPhase != Combat_Character.Phase.Waiting)
        //    return false;
        //
        //if (manaCost > Character.Mana())
        //    return false;
        //
        //if (skill.selection != Selection.Singular)
        //    return false;
        //
        //if (skill.type != Type.Offensive && skill.type != Type.Defensive)
        //    return false;
        //
        //if (!skill.chosen_Targets.Contains(Character.transform))
        //    return false;
        //
        //if (skill.range != Range.Close)
        //    return false;


        return true;
    }

    public override IEnumerator SetUp()
    {
        //yield return CharacterTargeting();

        chosen_Targets.Add(Character.TurnController.resolveStack[Character.TurnController.resolveStack.Count - 2].hand.character.transform);

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

        CoroutineWithData cwd = new CoroutineWithData(Character, Character.TurnController.Reactions(this, Turn_Controller.Stage.IMPACT));
        yield return cwd.coroutine;
    }   

    public override IEnumerator Resolve()
    {
        Coroutine outcome = Character.StartCoroutine(Character.ApplyOutcome(this));

        yield return outcome;
    }
}
