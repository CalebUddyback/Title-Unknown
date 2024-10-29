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

        yield return Owner.ActionChoice(Owner.GetSkill(ButtonChoice)).SubMenus(Owner);

        if(Owner.chosenAction != null)
            Owner.EndTurn();
    }
}
