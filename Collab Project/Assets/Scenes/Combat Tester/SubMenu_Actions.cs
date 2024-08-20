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

        switch (ButtonChoice)
        {
            case 0:
                yield return SubMenuController.OpenSubMenu("Attacks", SubMenuController.owner.attackList);
                break;

            default:
                yield return StartCoroutine(WaitForChoice());
                break;
        }
    }
}
