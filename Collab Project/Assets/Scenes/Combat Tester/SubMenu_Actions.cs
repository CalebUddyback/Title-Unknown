using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubMenu_Actions : SubMenu
{
    //public override void AdditionalSetup()
    //{
    //    AddButtonListeners();
    //}


    public override IEnumerator WaitForChoice()
    {
        yield return base.WaitForChoice();

        Combat_Character combat_Character = SubMenuController.owner;

        switch (ButtonChoice)
        {
            case 0:
                yield return SubMenuController.OpenSubMenu("Attacks", SubMenuController.owner.attackList);
                break;

            case 1:

                combat_Character.ActionChoice(combat_Character.defense);

                yield return combat_Character.chosenAction.SubMenus(combat_Character);

                if (combat_Character.chosenAction != null)
                    combat_Character.EndTurn();

                break;

            case 3:

                combat_Character.ActionChoice(combat_Character.rest);

                yield return combat_Character.chosenAction.SubMenus(combat_Character);

                if (combat_Character.chosenAction != null)
                    combat_Character.EndTurn();

                break;

            default:
                yield return StartCoroutine(WaitForChoice());
                break;
        }
    }
}
