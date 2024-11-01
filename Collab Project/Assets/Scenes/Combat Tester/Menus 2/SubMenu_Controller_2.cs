using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubMenu_Controller_2 : MonoBehaviour
{
    public TextMeshProUGUI title;
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public Button returnButton;

    public Combat_Character Owner => transform.parent.parent.GetComponent<Combat_Character>();

    [System.Serializable]
    public class SubMenu_2
    {
        public string ID;
        public string title;
        public int ButtonChoice = -2;       // -2 (Waiting), -1 (Return)
        public SubMenu_2 previousSubMenu;
    }

    public class Tab
    {
        public string buttonText;
        public string nextID;
        public Combat_Character.Skill storedSkill = null;
        public IEnumerator submenuTree;


        public Tab(string t, string s)
        {
            buttonText = t;
            nextID = s;
        }

        public Tab(string t, IEnumerator e)
        {
            buttonText = t;
            submenuTree = e;
        }

        public void StoreSkill(Combat_Character.Skill skill)
        {
            storedSkill = skill;
        }
    }

    public Dictionary<string, SubMenu_2> dictionary = new Dictionary<string, SubMenu_2>();      // we want a dictionary so we can grab results of previous submenus more efficiently

    public SubMenu_2 currentSubMenu;

    public Tab[] currentButtons;

    private void Start()
    {
        returnButton.onClick.AddListener(() => currentSubMenu.ButtonChoice = -1);
        StartCoroutine(DefaultMenu());
    }

    public IEnumerator DefaultMenu()
    {
        string subMenuID = "Actions";

        bool done = false;

        dictionary.Clear();

        currentSubMenu = null;

        do
        {
            switch (subMenuID)
            {
                case "Actions":
                    OpenSubMenu(subMenuID, "Actions", new Tab[]{ new Tab("Attacks", "Attacks"), new Tab("Defense", "Confirm"), new Tab("Items", "x"), new Tab("Rest", "Confirm") });
                    yield return WaitForChoice();
                    break;

                case "Attacks":
                    OpenSubMenu(subMenuID, "Attacks", Owner.attackList.ToArray());
                    yield return WaitForChoice();
                    break;

                case "Confirm":
                    ConfirmSubMenu();
                    yield return WaitForChoice();
                    break;

                case "Return":
                    subMenuID = currentSubMenu.previousSubMenu.ID;
                    CloseSubMenu();
                    break;

                case "Done":
                    done = true;
                    break;

                default:
                    Debug.Log("Menu Dead End");
                    currentSubMenu.ButtonChoice = -2;
                    subMenuID = subMenuID.Substring(0, subMenuID.Length - 2);
                    break;
            }

            IEnumerator WaitForChoice()
            {
                yield return new WaitWhile(() => currentSubMenu.ButtonChoice == -2);

                UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);

                transform.GetChild(0).gameObject.SetActive(false);

                yield return null;

                transform.GetChild(0).gameObject.SetActive(true);

                if (currentSubMenu.ButtonChoice > -1)
                {
                    if (currentButtons[currentSubMenu.ButtonChoice].submenuTree != null)
                    {
                        Owner.ActionChoice(Owner.attackList[currentSubMenu.ButtonChoice]);
                        yield return StartCoroutine(currentButtons[currentSubMenu.ButtonChoice].submenuTree);

                        CloseSubMenu();
                    }
                    else
                        subMenuID = currentButtons[currentSubMenu.ButtonChoice].nextID;
                }
                else
                {
                    subMenuID = "Return";
                }
            }

        }
        while (!done);

        Debug.Log("Done");

        transform.GetChild(0).gameObject.SetActive(false);
    }

    public SubMenu_2 OpenSubMenu(string nextID, string nextTitle, object[] nextButtons)
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

        return currentSubMenu;
    }

    public void AdjustButtons(object[] objArray)
    {
        if (objArray.Length <= buttonContainer.childCount)
        {
            for (int i = 0; i < buttonContainer.childCount; i++)
            {
                if (i < objArray.Length)
                    buttonContainer.GetChild(i).gameObject.SetActive(true);
                else
                    buttonContainer.GetChild(i).gameObject.SetActive(false);
            }
        }
        else
        {
            for (int i = buttonContainer.childCount; i < objArray.Length; i++)
            {
                Button newButton = Instantiate(buttonPrefab, buttonContainer).GetComponent<Button>();

                newButton.onClick.AddListener(() => currentSubMenu.ButtonChoice = newButton.transform.GetSiblingIndex());

                newButton.GetComponent<SubMenu_Button>().SubMenu_Controller_2 = this;
            }
        }

        Tab[] buttons = new Tab[objArray.Length];

        for (int i = 0; i < objArray.Length; i++)
        {
            SubMenu_Button buttonInst = buttonContainer.GetChild(i).GetComponent<SubMenu_Button>();

            if (objArray is string[])
            {
                string label = (string)objArray[i];

                buttons[i] = new Tab(label, "");
                buttonInst.buttonText.text = (string)objArray[i];
            }
            else if (objArray is Tab[])
            {
                Tab tab = (Tab)objArray[i];

                buttons[i] = tab;
                buttonInst.buttonText.text = tab.buttonText;
            }
            else if (objArray is Combat_Character.Skill[])
            {
                Combat_Character.Skill skill = (Combat_Character.Skill)objArray[i];
                buttons[i] = new Tab(skill.name, skill.SubMenus(this));
                buttons[i].StoreSkill(skill);
                buttonInst.buttonText.text = skill.name;
            }

            if (buttons[i].nextID == "x")
                buttonInst.GetComponent<Button>().interactable = false;
            else
                buttonInst.GetComponent<Button>().interactable = true;
        }

        currentButtons = buttons;
    }

    public SubMenu_2 ConfirmSubMenu()
    {
        SubMenu_2 previousSubMenu = null;

        Tab[] newButtons = new Tab[] { new Tab("Confirm", "Done"), new Tab("Cancel", "Return") };

        title.text = currentButtons[currentSubMenu.ButtonChoice].buttonText;

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

        return currentSubMenu;
    }

    public void CloseSubMenu()
    {
        for (int i = 0; i < 2; i++)
        {
            string removeID = currentSubMenu.ID;

            if(removeID == "Actions")
                currentSubMenu = null;
            else
                currentSubMenu = currentSubMenu.previousSubMenu;

            dictionary.Remove(removeID);
        }
    }
}
