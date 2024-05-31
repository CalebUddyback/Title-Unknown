using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubMenu_Actions : SubMenu
{
    public override void AdditionalSetup()
    {
        AddButtonListeners();
    }


    public override IEnumerator WaitForChoice()
    {
        yield return base.WaitForChoice();

        switch (ButtonChoice)
        {
            case 0:
                SubMenu subMenu =  SubMenuController.OpenSubMenu("Attacks", transform.root.GetComponent<Combat_Character>().GetAttackNames());
                break;

            default:
                yield return WaitForChoice();
                break;
        }
    }
}
