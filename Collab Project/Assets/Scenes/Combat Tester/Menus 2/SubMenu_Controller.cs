using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SubMenu_Controller : MonoBehaviour
{
    public List<SubMenu> subMenuList = new List<SubMenu>();

    public SubMenu CurrentSubMenu {get; private set;}

    public CoroutineWithData CurrentCD { get; private set; }

    public int subMenuStage = 0;

    public Combat_Character Owner => transform.parent.parent.GetComponent<Combat_Character>();


    public bool CheckForSubMenu(string name)
    {
        if (transform.Find(name) != null)
            return true;
        else
            return false;
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

            subMenuList.Add(nextSubMenu);


            if (subMenuList.Count == 1 && nextSubMenuName != "Prompts")
                nextSubMenu.returnButton.gameObject.SetActive(false);
            else
                nextSubMenu.returnButton.gameObject.SetActive(true);

            if (CurrentSubMenu != null)
                CurrentSubMenu.gameObject.SetActive(false);

            nextSubMenu.AddButtons(buttonLabels);


            yield return null;

            nextSubMenu.gameObject.SetActive(true);

            CurrentSubMenu = nextSubMenu;

            CurrentCD = new CoroutineWithData(this, nextSubMenu.WaitForChoice());

        }
    }

    public IEnumerator OpenSubMenu(string nextSubMenuName, List<Combat_Character.Skill> skillList)
    {
        yield return OpenSubMenu(nextSubMenuName, skillList.Select(s => s.name).ToList());

        CurrentSubMenu.StoreInButton(skillList);

        yield return null;
    }

    public void CloseSubMenu()
    {
        CurrentCD.Stop();

        CurrentSubMenu.gameObject.SetActive(false);
        subMenuList.RemoveAt(subMenuList.Count - 1);

        if(subMenuList.Count - 1 >= 0)
        {
            CurrentSubMenu = subMenuList[subMenuList.Count - 1];
            CurrentSubMenu.gameObject.SetActive(true);

            CurrentCD = new CoroutineWithData(this, CurrentSubMenu.WaitForChoice());
        }
    }


    public void ResetMenus()
    {
        for (int i = 0; i < subMenuList.Count; i++)
        {
            subMenuList[i].gameObject.SetActive(false);
        }

        subMenuList.Clear();
    }
}
