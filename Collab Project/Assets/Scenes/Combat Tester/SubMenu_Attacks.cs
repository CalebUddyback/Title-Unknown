using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubMenu_Attacks : SubMenu
{
    public override IEnumerator WaitForChoice()
    {
        yield return base.WaitForChoice();

        yield return null;      // Give menu time to close if -1

        Combat_Character combat_Character = SubMenuController.owner;

        combat_Character.AttackChoice(combat_Character.attackList[ButtonChoice]);

        combat_Character.attack.SubMenus(combat_Character);

        yield return combat_Character.attack.coroutine;

        if(combat_Character.attack != null)
            combat_Character.EndTurn();
    }
}
