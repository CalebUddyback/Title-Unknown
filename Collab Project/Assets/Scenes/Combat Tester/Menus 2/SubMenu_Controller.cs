using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMenu_Controller : MonoBehaviour
{
    public List<SubMenu> menuStack = new List<SubMenu>();

    public struct Test
    {
        public string title;
        public List<string> buttons;
        public int selection;
    }

    public Dictionary<string, Test> map;

    public SubMenu CurrentSubMenu {get; private set;}

    public CoroutineWithData CurrentCD { get; private set; }

    public int subMenuStage = 0;

    [HideInInspector]
    public Combat_Character Owner => transform.parent.parent.GetComponent<Combat_Character>();


    public bool CheckForSubMenu(string name)
    {
        if (transform.Find(name) != null)
            return true;
        else
            return false;
    }

    /* Use to add to menu stack and change alpha */

    public SubMenu OpenSubMenu(string nextSubMenuName)
    {
        //nextSubMenu.transform.SetSiblingIndex(menuStack.Count);

        if (transform.Find(nextSubMenuName) == null)
            return null;

        SubMenu nextSubMenu = transform.Find(nextSubMenuName).GetComponent<SubMenu>();

        if (CurrentSubMenu != null)
            CurrentSubMenu.gameObject.SetActive(false);

        nextSubMenu.GetComponent<CanvasGroup>().interactable = true;
        nextSubMenu.gameObject.SetActive(true);

        menuStack.Add(nextSubMenu);

        CurrentSubMenu = nextSubMenu;

        CurrentCD = new CoroutineWithData(this, nextSubMenu.WaitForChoice());

        return nextSubMenu;
    }

    public IEnumerator OpenSubMenu(string nextSubMenuName, List<Combat_Character.Skill> skillList)
    {
        if (transform.Find(nextSubMenuName) == null)
        {
            print("Menu Name Does Not Exist");
            Debug.Break();
            yield break;
        }

        if (subMenuStage == 0)
        {

            SubMenu nextSubMenu = transform.Find(nextSubMenuName).GetComponent<SubMenu>();

            menuStack.Add(nextSubMenu);


            if (menuStack.Count == 1 && nextSubMenuName != "Prompts")
                nextSubMenu.returnButton.gameObject.SetActive(false);
            else
                nextSubMenu.returnButton.gameObject.SetActive(true);

            if (CurrentSubMenu != null)
                CurrentSubMenu.gameObject.SetActive(false);

            nextSubMenu.AddButtons(skillList);


            yield return null;

            nextSubMenu.GetComponent<CanvasGroup>().interactable = true;
            nextSubMenu.gameObject.SetActive(true);

            CurrentSubMenu = nextSubMenu;

            CurrentCD = new CoroutineWithData(this, nextSubMenu.WaitForChoice());

        }


        yield return CurrentCD.coroutine;
    }

    public IEnumerator OpenSubMenu(string nextSubMenuName, List<string> buttonLabels)
    {

        if (transform.Find(nextSubMenuName) == null)
        {
            print(nextSubMenuName + " Sub-Menu Does Not Exist");
            Debug.Break();
            yield break;
        }

        if (subMenuStage == 0)
        {

            SubMenu nextSubMenu = transform.Find(nextSubMenuName).GetComponent<SubMenu>();

            menuStack.Add(nextSubMenu);


            if (menuStack.Count == 1 && nextSubMenuName != "Prompts")
                nextSubMenu.returnButton.gameObject.SetActive(false);
            else
                nextSubMenu.returnButton.gameObject.SetActive(true);

            if (CurrentSubMenu != null)
            {
                CurrentSubMenu.gameObject.SetActive(false);
                //CurrentSubMenu.GetComponent<CanvasGroup>().interactable = false;
            }

            nextSubMenu.AddButtons(buttonLabels);


            yield return null;

            nextSubMenu.GetComponent<CanvasGroup>().interactable = true;
            nextSubMenu.gameObject.SetActive(true);

            CurrentSubMenu = nextSubMenu;

            CurrentCD = new CoroutineWithData(this, nextSubMenu.WaitForChoice());

        }


        //yield return CurrentCD.coroutine;
    }

    public void CloseSubMenu()
    {
        CurrentCD.Stop();

        CurrentSubMenu.gameObject.SetActive(false);
        menuStack.RemoveAt(menuStack.Count - 1);

        if(menuStack.Count - 1 >= 0)
        {
            CurrentSubMenu = menuStack[menuStack.Count - 1];
            CurrentSubMenu.GetComponent<CanvasGroup>().interactable = true;
            CurrentSubMenu.gameObject.SetActive(true);

            CurrentCD = new CoroutineWithData(this, CurrentSubMenu.WaitForChoice());
        }
    }


    public void ResetMenus()
    {
        for (int i = 0; i < menuStack.Count; i++)
        {
            menuStack[i].gameObject.SetActive(false);
        }

        menuStack.Clear();
        //gameObject.SetActive(false);
    }
}
