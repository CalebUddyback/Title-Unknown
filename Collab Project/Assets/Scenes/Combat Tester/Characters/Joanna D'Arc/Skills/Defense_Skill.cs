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
        yield return Character.deck.DiscardCards();

        //yield return CharacterTargeting();

        yield return null;
    }

    public override IEnumerator Execute()
    {
        Character.animationController.Clip(animationName, 1);

        GetOutcome(chosen_Targets[0].GetComponent<Combat_Character>());

        yield return Character.WaitForKeyFrame(0);

        yield return new WaitUntil(() => Character.deck.cardRemoved == true);
    }

    public override IEnumerator Resolve()
    {
        Character.animationController.PlaySpeed(1);

        //Character.blocking = true;


        // Character deffense up

        Character.AdjustDefense(null, defense, 1);

        //yield return null;
        yield return Character.animationController.coroutine;
    }
}
