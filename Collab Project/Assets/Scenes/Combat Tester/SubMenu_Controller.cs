using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SubMenu_Controller : MonoBehaviour
{
    public List<SubMenu> menuStack = new List<SubMenu>();

    [HideInInspector]
    public SubMenu currentSubMenu;

    public CoroutineWithData CurrentCD { get; private set; }

    public GameObject subMenuPrefab;

    public Transform pages;

    public GameObject pagesPrefab;

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

        currentSubMenu = newSubMenu;

        CurrentCD = new CoroutineWithData(this, newSubMenu.WaitForChoice());

        return newSubMenu;
    }

    /* Use to add to menu stack and change alpha */

    public SubMenu OpenSubMenu(SubMenu nextSubMenu)
    {
        //nextSubMenu.transform.SetSiblingIndex(menuStack.Count);

        if(currentSubMenu != null)
            currentSubMenu.gameObject.SetActive(false);

        nextSubMenu.GetComponent<CanvasGroup>().interactable = true;
        nextSubMenu.gameObject.SetActive(true);

        menuStack.Add(nextSubMenu);

        AdjustPagesAlpha();

        currentSubMenu = nextSubMenu;

        CurrentCD = new CoroutineWithData(this, nextSubMenu.WaitForChoice());

        return nextSubMenu;
    }

    public SubMenu OpenSubMenu(Transform nextSubmenu)
    {
        return OpenSubMenu(nextSubmenu.GetComponent<SubMenu>());
    }

    public SubMenu OpenSubMenu(string nextSubmenu)
    {
        if (transform.Find(nextSubmenu) == null)
            return null;

        return OpenSubMenu(transform.Find(nextSubmenu).GetComponent<SubMenu>());
    }

    public SubMenu OpenSubMenu(SubMenu nextSubMenu, List<string> buttonLabels)
    {
        //nextSubMenu.transform.SetSiblingIndex(menuStack.Count);

        if (currentSubMenu != null)
            currentSubMenu.gameObject.SetActive(false);

        nextSubMenu.AddButtons(buttonLabels);
        nextSubMenu.GetComponent<CanvasGroup>().interactable = true;
        nextSubMenu.gameObject.SetActive(true);

        menuStack.Add(nextSubMenu);

        AdjustPagesAlpha();

        currentSubMenu = nextSubMenu;

        CurrentCD = new CoroutineWithData(this, nextSubMenu.WaitForChoice());

        return nextSubMenu;
    }

    public SubMenu OpenSubMenu(string nextSubmenu, List<string> buttonLabels)
    {
        if (transform.Find(nextSubmenu) == null)
            return null;

        return OpenSubMenu(transform.Find(nextSubmenu).GetComponent<SubMenu>(), buttonLabels);
    }

    public void CloseSubMenu()
    {
        CurrentCD.Stop();

        currentSubMenu.gameObject.SetActive(false);
        menuStack.RemoveAt(menuStack.Count - 1);


        currentSubMenu = menuStack[menuStack.Count - 1];
        currentSubMenu.GetComponent<CanvasGroup>().interactable = true;
        currentSubMenu.gameObject.SetActive(true);

        AdjustPagesAlpha();

        CurrentCD = new CoroutineWithData(this, currentSubMenu.WaitForChoice());

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
