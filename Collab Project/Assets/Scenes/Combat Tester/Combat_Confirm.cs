﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat_Confirm : Combat_Menu
{
    public override IEnumerator WaitForChoice()
    {
        yield return base.WaitForChoice();

        switch (buttonChoice)
        {
            case 0:
                Return();
                break;

            case 1:
                break;
        }

        yield return null;
    }
}
