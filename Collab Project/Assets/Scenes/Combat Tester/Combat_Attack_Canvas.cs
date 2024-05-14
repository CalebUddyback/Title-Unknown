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
        //foreach (Transform button in GetComponent<SubMenu>().content)
        //{
        //    button.GetChild(0).GetComponent<Text>().text = transform.root.GetComponent<Combat_Character>().AttackName(button.GetSiblingIndex());
        //}

        foreach(Combat_Character.Attack attack in transform.root.GetComponent<Combat_Character>().attackList)
        {
            Instantiate(buttonPrefab, buttonContainer).transform.GetChild(0).GetComponent<Text>().text = attack.name;
        }
    }


    public override IEnumerator WaitForChoice()
    {         
        yield return base.WaitForChoice();

        yield return null;      // Give menu time to close if -1

        Combat_Character.Attack attack = transform.root.GetComponent<Combat_Character>().attackList[buttonChoice];

        List<int> outputs = new List<int>();

        if (attack.requiredMenus != null)
        {

            int i = 0;

            Combat_Menu reqMenu = transform.parent.Find(attack.requiredMenus[i]).GetComponent<Combat_Menu>();

            Controller.OpenSubmenu(reqMenu);

            while (i < attack.requiredMenus.Length)
            {

                yield return Controller.CurrentCD.coroutine;

                if (reqMenu.buttonChoice == -1)
                {

                    if (i-1 < 0)
                        yield break;

                    outputs.RemoveAt(i);
                    i--;

                    reqMenu = transform.parent.Find(attack.requiredMenus[i]).GetComponent<Combat_Menu>();
                }
                else
                {

                    if (i + 1 >= attack.requiredMenus.Length)
                        break;

                    outputs.Add(reqMenu.buttonChoice);
                    i++;

                    reqMenu = transform.parent.Find(attack.requiredMenus[i]).GetComponent<Combat_Menu>();

                    Controller.OpenSubmenu(reqMenu);
                }
            }
        }

        transform.root.GetComponent<Combat_Character>().AttackChoice(attack, outputs.ToArray());
    }
}
