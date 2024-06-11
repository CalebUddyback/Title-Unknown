using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMenu_Controller : MonoBehaviour
{
    public List<SubMenu> menuStack = new List<SubMenu>();


    public SubMenu CurrentSubMenu {get; private set;}

    public CoroutineWithData CurrentCD { get; private set; }

    public GameObject subMenuPrefab;

    public int subMenuStage = 0;

    public Transform pages;

    public GameObject pagesPrefab;

    [HideInInspector]
    public Combat_Character owner => transform.parent.parent.GetComponent<Combat_Character>();

    private void OnEnable()
    {
        OpenSubMenu("Actions");
    }

    public bool CheckForSubMenu(string name)
    {
        if (transform.Find(name) != null)
            return true;
        else
            return false;
    }

    public SubMenu CreateSubMenu(string name)
    {
        SubMenu newSubMenu = Instantiate(subMenuPrefab, transform).GetComponent<SubMenu>();

        newSubMenu.transform.SetSiblingIndex(transform.childCount);

        newSubMenu.gameObject.name = name;

        menuStack.Add(newSubMenu);

        AdjustPagesAlpha();

        CurrentSubMenu = newSubMenu;

        CurrentCD = new CoroutineWithData(this, newSubMenu.WaitForChoice());

        return newSubMenu;
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

        AdjustPagesAlpha();

        CurrentSubMenu = nextSubMenu;

        CurrentCD = new CoroutineWithData(this, nextSubMenu.WaitForChoice());

        return nextSubMenu;
    }

    public IEnumerator OpenSubMenu(string nextSubMenuName, List<string> buttonLabels)
    {
        if (transform.Find(nextSubMenuName) == null)
        {
            print("Menu Name Does Not Exist");
            yield break;
        }

        if (subMenuStage == 0)
        {

            SubMenu nextSubMenu = transform.Find(nextSubMenuName).GetComponent<SubMenu>();

            menuStack.Add(nextSubMenu);

            if (CurrentSubMenu != null)
                CurrentSubMenu.gameObject.SetActive(false);

            nextSubMenu.AddButtons(buttonLabels);


            yield return null;

            nextSubMenu.GetComponent<CanvasGroup>().interactable = true;
            nextSubMenu.gameObject.SetActive(true);

            AdjustPagesAlpha();

            CurrentSubMenu = nextSubMenu;

            CurrentCD = new CoroutineWithData(this, nextSubMenu.WaitForChoice());

        }

        yield return CurrentCD.coroutine;
    }

    public void CloseSubMenu()
    {
        CurrentCD.Stop();

        CurrentSubMenu.gameObject.SetActive(false);
        menuStack.RemoveAt(menuStack.Count - 1);


        CurrentSubMenu = menuStack[menuStack.Count - 1];
        CurrentSubMenu.GetComponent<CanvasGroup>().interactable = true;
        CurrentSubMenu.gameObject.SetActive(true);

        AdjustPagesAlpha();

        CurrentCD = new CoroutineWithData(this, CurrentSubMenu.WaitForChoice());
    }

    void AdjustPagesAlpha()
    {

        return;

        float a = 1;

        while(pages.childCount < menuStack.Count - 1)
        {
            Instantiate(pagesPrefab, pages);
        }

        while (pages.childCount > menuStack.Count - 1)
        {
            Destroy(pages.GetChild(0));
        }

        for (int i = pages.childCount - 1; i >= 0; i--)
        {
 
            pages.GetChild(i).GetComponent<CanvasGroup>().alpha = a;

            a -= 0.333f;
        }
    }

    public void ResetMenus()
    {

        for (int i = 0; i < menuStack.Count; i++)
        {
            menuStack[i].gameObject.SetActive(false);
        }

        menuStack.Clear();
        gameObject.SetActive(false);

    }
}
