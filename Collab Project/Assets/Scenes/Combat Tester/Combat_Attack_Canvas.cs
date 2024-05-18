using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combat_Attack_Canvas : Combat_Menu
{
    public GameObject buttonPrefab;
    public Transform buttonContainer;

    private void Awake()
    {
        foreach(Combat_Character.Attack attack in transform.root.GetComponent<Combat_Character>().attackList)
        {
            Instantiate(buttonPrefab, buttonContainer).transform.GetChild(0).GetComponent<Text>().text = attack.name;
        }
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

            Combat_Menu reqMenu = transform.parent.Find(attack.requiredMenus[i].Menu).GetComponent<Combat_Menu>();

            Controller.OpenSubmenu(reqMenu);

            while (i < attack.requiredMenus.Length)
            {

                yield return Controller.CurrentCD.coroutine;

                if (reqMenu.ButtonChoice == -1)
                {

                    if (i - 1 < 0)
                        yield break;

                    outputs.RemoveAt(i);
                    i--;

                    reqMenu = transform.parent.Find(attack.requiredMenus[i].Menu).GetComponent<Combat_Menu>();
                }
                else
                {

                    if (i + 1 > attack.requiredMenus.Length - 1)
                        break;

                    outputs.Add(reqMenu.ButtonChoice);
                    i++;

                    reqMenu = transform.parent.Find(attack.requiredMenus[i].Menu).GetComponent<Combat_Menu>();

                    Controller.OpenSubmenu(reqMenu);

                    if(attack.requiredMenus[i].DependantMenu != "")
                    {
                        reqMenu.dependant_Variable = transform.parent.Find(attack.requiredMenus[i].DependantMenu).GetComponent<Combat_Menu>().ButtonChoice;
                    }
                }
            }
        }

        Combat_Menu confirmMenu = transform.parent.Find("Confirm").GetComponent<Combat_Menu>();

        Controller.OpenSubmenu(confirmMenu);

        yield return Controller.CurrentCD.coroutine;

        if (confirmMenu.ButtonChoice == -1)
        {
            if (i >= 0)
                goto Reloop;
        }

        transform.root.GetComponent<Combat_Character>().AttackChoice(attack, outputs.ToArray());
    }
}
