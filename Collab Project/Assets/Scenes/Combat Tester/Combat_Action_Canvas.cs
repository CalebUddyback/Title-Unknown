using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combat_Action_Canvas : Combat_Menu
{
    public override IEnumerator WaitForChoice()
    {
        print("waiting");

        yield return base.WaitForChoice();

        switch (buttonChoice)
        {
            case 0:
                transform.parent.Find("Attacks").gameObject.SetActive(true);
                break;

            default:
                yield return WaitForChoice();
                break;
        }

        yield return null;
    }
}
