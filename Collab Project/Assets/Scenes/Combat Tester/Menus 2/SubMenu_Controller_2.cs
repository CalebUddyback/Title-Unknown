using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class SubMenu_Controller_2 : MonoBehaviour
{
    public TextMeshProUGUI title;
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public SubMenu_Button returnButton;

    public Combat_Character Owner => transform.parent.parent.GetComponent<Combat_Character>();

    public string subMenuID = "Actions";

    public bool waiting = true;

    public int hovering = 0;

    [System.Serializable]
    public class SubMenu_2
    {
        public string ID;
        public string title;
        public int buttonChoice = -2;
        public SubMenu_2 previousSubMenu;
    }

    [Serializable]
    public class Tab
    {
        public string buttonText;
        public string nextID;
        public IEnumerator nextMethod;

        public Tab(string t, string n)
        {
            buttonText = t;
            nextID = n;
        }

        public Tab(string t, IEnumerator e)
        {
            buttonText = t;
            nextMethod = e;
        }
    }

    public Dictionary<string, SubMenu_2> dictionary = new Dictionary<string, SubMenu_2>();      // we want a dictionary so we can grab results of previous submenus more efficiently

    public SubMenu_2 currentSubMenu;

    public Tab[] currentButtons;

    private void Start()
    {
        returnButton.returnButton = true;
        returnButton.GetComponent<Button>().onClick.AddListener(() => Select(-1));
    }

    public IEnumerator Menus()
    {
        bool done = false;

        dictionary.Clear();

        currentSubMenu = null;

        transform.GetChild(0).gameObject.SetActive(true);

        if (Owner.chosenAction != null && Owner.chosenAction.charging)
            subMenuID = "Charging";
        else
            subMenuID = "Actions";

        do
        {
            switch (subMenuID)
            {
                case "Actions":
                    OpenSubMenu(subMenuID, "Actions", new Tab[] { new Tab("Attacks", "Attacks"), new Tab("Defense", ConfirmSubMenu("Done")), new Tab("Items", "x"), new Tab("Rest", "Confirm") });

                    while (waiting)
                    {
                        yield return null;
                    }

                    UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                    transform.GetChild(0).gameObject.SetActive(false);

                    yield return null;

                    int b = currentSubMenu.buttonChoice;

                    subMenuID = currentButtons[b].nextID;
                    yield return currentButtons[b].nextMethod;

                    break;

                case "Attacks":
                    OpenSubMenu(subMenuID, "Attacks", Owner.attackList.ToArray());

                    while (waiting)
                    {
                        Owner.TurnController.left_descriptionBox.Description(Owner.attackList[hovering]);
                        Owner.TurnController.left_descriptionBox.container.SetActive(true);
                        yield return null;
                    }

                    UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
                    transform.GetChild(0).gameObject.SetActive(false);

                    yield return null;

                    b = currentSubMenu.buttonChoice;

                    if (b == -1)
                        yield return Return();
                    else
                    {
                        Owner.ActionChoice(Owner.attackList[b]);

                        subMenuID = currentButtons[b].nextID;
                        yield return currentButtons[b].nextMethod;
                    }

                    break;

                case "Charging":
                    yield return Owner.chosenAction.SubMenus(this);
                    break;

                case "Done":
                    done = true;
                    break;

                default:
                    Debug.Log(subMenuID + " Case Does Not Exist");
                    subMenuID = currentSubMenu.ID;
                    yield return Return();
                    break;
            }
        }
        while (!done);

        Debug.Log("Done");

        Owner.EndTurn();
    }

    public void OpenSubMenu(string nextID, string nextTitle, object[] nextButtons)
    {
        SubMenu_2 previousSubMenu = null;

        if (dictionary.Count > 0)
        {
            previousSubMenu = currentSubMenu;
            returnButton.gameObject.SetActive(true);
        }
        else
            returnButton.gameObject.SetActive(false);

        currentSubMenu = new SubMenu_2()
        {
            ID = nextID,
            title = nextTitle,
            previousSubMenu = previousSubMenu,
        };

        AdjustButtons(nextButtons);

        title.text = currentSubMenu.title;

        dictionary.Add(currentSubMenu.ID, currentSubMenu);

        transform.GetChild(0).gameObject.SetActive(true);

        hovering = 0;

        waiting = true;
    }

    public void AdjustButtons(object[] objArray)
    {
        if (objArray.Length <= buttonContainer.childCount)
        {
            for (int i = 0; i < buttonContainer.childCount; i++)
            {
                buttonContainer.GetChild(i).gameObject.SetActive( i < objArray.Length ? true : false);
            }
        }
        else
        {
            for (int i = buttonContainer.childCount; i < objArray.Length; i++)
            {
                Button newButton = Instantiate(buttonPrefab, buttonContainer).GetComponent<Button>();

                newButton.onClick.AddListener(() => Select(newButton.transform.GetSiblingIndex()));

                newButton.GetComponent<SubMenu_Button>().SubMenu_Controller_2 = this;
            }
        }

        Tab[] buttons = new Tab[objArray.Length];

        for (int i = 0; i < objArray.Length; i++)
        {
            SubMenu_Button buttonInst = buttonContainer.GetChild(i).GetComponent<SubMenu_Button>();

            if (objArray is Tab[])
            {
                Tab tab = (Tab)objArray[i];

                buttons[i] = tab;
                buttonInst.buttonText.text = tab.buttonText;
            }
            else if (objArray is Combat_Character.Skill[])
            {
                Combat_Character.Skill skill = (Combat_Character.Skill)objArray[i];

                buttons[i] = new Tab(skill.name, "Start")
                {
                    nextMethod = skill.SubMenus(this),
                };

                buttonInst.buttonText.text = skill.name;
            }

            buttonInst.GetComponent<Button>().interactable = (buttons[i].nextID == "x") ? false : true;
        }

        currentButtons = buttons;
    }

    public IEnumerator ConfirmSubMenu(string nextID)
    {
        SubMenu_2 previousSubMenu = null;

        Tab[] newButtons = new Tab[] { new Tab("Confirm", nextID), new Tab("Cancel", Return()) };

        title.text = "Confirm";

        if (dictionary.Count > 0)
        {
            previousSubMenu = currentSubMenu;
            returnButton.gameObject.SetActive(true);
        }
        else
            returnButton.gameObject.SetActive(false);

        currentSubMenu = new SubMenu_2()
        {
            ID = "Confirm",
            title = title.text,
            previousSubMenu = previousSubMenu,
        };

        AdjustButtons(newButtons);

        dictionary.Add(currentSubMenu.ID, currentSubMenu);

        transform.GetChild(0).gameObject.SetActive(true);

        hovering = 0;

        waiting = true;

        while (waiting)
        {
            yield return null;
        }

        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
        transform.GetChild(0).gameObject.SetActive(false);

        yield return null;

        if (currentSubMenu.buttonChoice == -1)
        {
            yield return Return();
        }
        else
        {
            subMenuID = currentButtons[currentSubMenu.buttonChoice].nextID;
            yield return currentButtons[currentSubMenu.buttonChoice].nextMethod;
        }
    }

    public void Select(int b)
    {
        currentSubMenu.buttonChoice = b;

        waiting = false;
    }

    public IEnumerator Return()
    {
        if (currentSubMenu.previousSubMenu != null)
            subMenuID = currentSubMenu.previousSubMenu.ID;

        CloseSubMenu();

        yield return null;
    }

    public void CloseSubMenu()
    {
        for (int i = 0; i < 2; i++)
        {
            string removeID = currentSubMenu.ID;

            if (removeID == "Actions")
            {
                currentSubMenu = null;
                dictionary.Remove(removeID);
                break;
            }
            else
            {
                currentSubMenu = currentSubMenu.previousSubMenu;
                dictionary.Remove(removeID);
            }
        }
    }
}
