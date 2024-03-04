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

        transform.root.GetComponent<Combat_Character>().AttackChoice(buttonChoice);
    
        yield return null;
    }
}
