using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubMenu_Actions : SubMenu
{
    public override IEnumerator WaitForChoice()
    {
        do
        {
            yield return base.WaitForChoice();

            switch (ButtonChoice)
            {
                case 0:

                    yield return SubMenuController.OpenSubMenu("Attacks", SubMenuController.Owner.attackList);

                    break;

                case 1:

                    yield return Owner.ActionChoice(Owner.defense).SubMenus(Owner);

                    if (Owner.chosenAction != null)
                        Owner.EndTurn();

                    break;

                case 3:

                    yield return Owner.ActionChoice(Owner.rest).SubMenus(Owner);

                    if (Owner.chosenAction != null)
                        Owner.EndTurn();

                    break;

                default:

                    ButtonChoice = -1;

                    break;
            }
        }
        while (ButtonChoice == -1);
    }
}
