using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat_Charges : SubMenu
{
    public override IEnumerator WaitForChoice()
    {
        yield return base.WaitForChoice();

        Combat_Character character = transform.root.GetComponent<Combat_Character>();

        //character.attackCharge = ButtonChoice;
    }
}
