using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combat_Action_Canvas : Combat_Menu
{
    public override IEnumerator WaitForChoice()
    {
        yield return base.WaitForChoice();

        switch (ButtonChoice)
        {
            case 0:

                Controller.OpenSubmenu(transform.parent.Find("Attacks"));
                break;

            default:
                yield return WaitForChoice();
                break;
        }
    }
}
