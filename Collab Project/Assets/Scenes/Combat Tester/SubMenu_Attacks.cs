using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubMenu_Attacks : SubMenu
{
    public override IEnumerator WaitForChoice()
    {
        GetComponent<ScrollRect>().verticalScrollbar.value = 1;

        yield return base.WaitForChoice();

        yield return null;      // Give menu time to close if -1

        Combat_Character combat_Character = SubMenuController.owner;

        combat_Character.AttackChoice(combat_Character.attackList[ButtonChoice]);

        yield return combat_Character.chosenAttack.SubMenus(combat_Character);

        if(combat_Character.chosenAttack != null)
            combat_Character.EndTurn();
    }
}
