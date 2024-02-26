using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combat_Attack_Canvas : Combat_Menu
{
    private void Start()
    {
        foreach (Transform button in GetComponent<SubMenu>().content)
        {
            button.GetChild(0).GetComponent<Text>().text = transform.root.GetComponent<Combat_Character>().AttackName(button.GetSiblingIndex());
        }
    }


    public override IEnumerator WaitForChoice()
    {
        yield return base.WaitForChoice();

        transform.root.GetComponent<Combat_Character>().AttackChoice(buttonChoice);
    
        yield return null;
    }
}
