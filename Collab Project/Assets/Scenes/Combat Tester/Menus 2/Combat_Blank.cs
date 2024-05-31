using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat_Blank : SubMenu
{
    public override IEnumerator WaitForChoice()
    {
        yield return base.WaitForChoice();
    }
}
