using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combat_Attack_Canvas1 : SubMenu
{
    private void Awake()
    {
        List<string> attackNames = new List<string>();
        
        foreach(Combat_Character.Attack attack in transform.root.GetComponent<Combat_Character>().attackList)
        {
            attackNames.Add(attack.name);
        }
        
        AddButtons(attackNames);
    }


    public override IEnumerator WaitForChoice()
    {
        yield return base.WaitForChoice();

        yield return null;      // Give menu time to close if -1

        Combat_Character.Attack attack = transform.root.GetComponent<Combat_Character>().attackList[ButtonChoice];

        List<int> outputs = new List<int>();

        int i = 0;

        Reloop:

        if (attack.requiredMenus != null)
        {

            SubMenu reqMenu = transform.parent.Find(attack.requiredMenus[i].Menu).GetComponent<SubMenu>();

            //SubMenuController.OpenSubMenu(reqMenu);

            while (i < attack.requiredMenus.Length)
            {

                yield return SubMenuController.CurrentCD.coroutine;

                if (reqMenu.ButtonChoice == -1)
                {

                    if (i - 1 < 0)
                        yield break;

                    outputs.RemoveAt(i);
                    i--;

                    reqMenu = transform.parent.Find(attack.requiredMenus[i].Menu).GetComponent<SubMenu>();
                }
                else
                {

                    if (i + 1 > attack.requiredMenus.Length - 1)
                        break;

                    outputs.Add(reqMenu.ButtonChoice);
                    i++;

                    reqMenu = transform.parent.Find(attack.requiredMenus[i].Menu).GetComponent<SubMenu>();

                    //SubMenuController.OpenSubMenu(reqMenu);

                    //if(attack.requiredMenus[i].DependantMenu != "")
                    //{
                    //    reqMenu.dependant_Variable = transform.parent.Find(attack.requiredMenus[i].DependantMenu).GetComponent<SubMenu>().ButtonChoice;
                    //}
                }
            }
        }

        SubMenu confirmMenu = transform.parent.Find("Confirm").GetComponent<SubMenu>();

        //SubMenuController.OpenSubMenu(confirmMenu);

        yield return SubMenuController.CurrentCD.coroutine;

        if (confirmMenu.ButtonChoice == -1)
        {
            if (i >= 0)
                goto Reloop;
        }

        //transform.root.GetComponent<Combat_Character>().AttackChoice(attack, outputs.ToArray());
    }
}
