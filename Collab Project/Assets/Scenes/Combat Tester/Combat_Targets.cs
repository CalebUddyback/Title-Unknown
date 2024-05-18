﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Combat_Targets : Combat_Menu
{
    public GameObject buttonPrefab;
    public Transform buttonContainer;


    private void Awake()
    {
        if (transform.root.GetComponent<Combat_Character>().Facing == 1)
        {
            foreach (Combat_Character character in transform.root.GetComponent<Combat_Character>().TurnController.right_Players)
            {
                Instantiate(buttonPrefab, buttonContainer).transform.GetChild(0).GetComponent<Text>().text = character.gameObject.name;
            }
        }
    }

    public override IEnumerator WaitForChoice()
    {
        yield return base.WaitForChoice();

        yield return null;      // Give menu time to close if -1

        Combat_Character character = transform.root.GetComponent<Combat_Character>();

        character.targets.Add(character.TurnController.right_Players[ButtonChoice].transform);

        if (dependant_Variable > 0)
        {
            for (int i = 0; i < dependant_Variable; i++)
            {
                //Combat_Menu menu = new GameObject("Targets " + i, typeof(Combat_Menu), typeof(SubMenu)).GetComponent<Combat_Menu>();

                Combat_Menu menu = Instantiate(gameObject, Controller.transform).GetComponent<Combat_Menu>();
                menu.GetComponent<CanvasGroup>().interactable = true;
                menu.transform.SetSiblingIndex(transform.GetSiblingIndex() + i + 1);
                menu.gameObject.SetActive(false);
                menu.GetComponent<Combat_Targets>().dependant_Variable = 0;
                Controller.OpenSubmenu(menu);

                yield return Controller.CurrentCD.coroutine;

                character.targets.Add(character.TurnController.right_Players[ButtonChoice].transform);
            }

        }

    }
}