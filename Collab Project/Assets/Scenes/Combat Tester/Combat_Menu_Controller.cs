using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Combat_Menu_Controller : MonoBehaviour
{
    public List<Combat_Menu> menuStack = new List<Combat_Menu>();

    public CoroutineWithData CurrentCD { get; private set; }

    public void ResetMenus()
    {
        menuStack[0].GetComponent<CanvasGroup>().interactable = true;

        for (int i = 1; i < menuStack.Count; i++)
        {
            menuStack[i].gameObject.SetActive(false);
            menuStack[i].GetComponent<CanvasGroup>().interactable = true;
        }

        menuStack.Clear();
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        OpenSubmenu(transform.Find("Actions"));
    }


    /* Use to add to menu stack and change alpha */

    public void OpenSubmenu(Combat_Menu nextSubmenu)
    {
        nextSubmenu.gameObject.SetActive(true);

        menuStack.Add(nextSubmenu);

        AdjustAlpha();

        CurrentCD = new CoroutineWithData(this, nextSubmenu.WaitForChoice());

        //print("Moving to " + nextSubmenu.gameObject.name);
    }

    public void OpenSubmenu(Transform nextSubmenu)
    {
        OpenSubmenu(nextSubmenu.GetComponent<Combat_Menu>());
    }

    public void CloseSubmenu()
    {
        CurrentCD.Stop();

        menuStack[menuStack.Count - 1].gameObject.SetActive(false);
        menuStack.RemoveAt(menuStack.Count - 1);

        Combat_Menu previousSubmenu = menuStack[menuStack.Count - 1];
        previousSubmenu.GetComponent<CanvasGroup>().interactable = true;

        AdjustAlpha();

        CurrentCD = new CoroutineWithData(this, previousSubmenu.WaitForChoice());

        //print("Return to " + previousSubmenu.gameObject.name);
    }

    void AdjustAlpha()
    {
        float a = 1;

        for (int i = menuStack.Count - 1; i >= 0 ; i--)
        {
            if(i < menuStack.Count - 1)
            {
                menuStack[i].GetComponent<CanvasGroup>().interactable = false;
                menuStack[i].GetComponent<SubMenu>().returnButton.gameObject.SetActive(false);
            }
            else
            {
                if(menuStack[i].GetComponent<SubMenu>().returnable)
                    menuStack[i].GetComponent<SubMenu>().returnButton.gameObject.SetActive(true);
            }

            menuStack[i].GetComponent<CanvasGroup>().alpha = a;

            a -= 0.333f;
        }
    }
}
