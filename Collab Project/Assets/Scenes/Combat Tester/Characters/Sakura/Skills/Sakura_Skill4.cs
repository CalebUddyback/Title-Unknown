using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sakura_Skill4 : Card
{
    public override bool UseCondition()
    {
        if (Character.currentPhase != Combat_Character.Phase.Main)
            return false;

        if (stats.mana > Character.Mana)
            return false;

        if (Character.hand.cards.Count < discards + 1)
            return false;

        if (Character.hand.cards.Count < 2)
            return false;

        return true;
    }

    public override IEnumerator SetUp()
    {
        yield return Character.hand.DiscardCards(discards);

        yield return CharacterTargeting();
    }

    public override IEnumerator Action()
    {
        Character.animationController.Clip("Buff");

        yield return Character.WaitForKeyFrame();

        Character.Mana += mana;

        Instantiate(Character.outcome_Bubble_Prefab, Character.TurnController.mainCamera.UIPosition(Character.outcome_Bubble_Pos.position), Quaternion.identity, Character.TurnController.damage_Bubbles).Input(mana, new Color(0, 0.5019608f, 1));

        GetOutcome(stats, chosen_Targets[0].GetComponent<Combat_Character>());

        yield return new WaitUntil(() => Character.hand.cardRemoved == true);

        yield return Character.animationController.coroutine;
    }
}
