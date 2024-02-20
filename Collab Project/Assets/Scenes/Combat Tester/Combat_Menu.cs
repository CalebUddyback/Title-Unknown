using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SubMenu))]

public abstract class Combat_Menu : MonoBehaviour
{
    public Coroutine waiting;

    public int buttonChoice = -2;

    public bool comfirmation = false;

    private void Awake()
    {
        foreach(Transform button in GetComponent<SubMenu>().content)
        {
            button.GetComponent<Button>().onClick.AddListener(() => buttonChoice = button.GetSiblingIndex());
        }

        GetComponent<SubMenu>().returnButton.onClick.AddListener(() => buttonChoice = -1);
    }

    public void OnEnable()
    {
        transform.parent.GetComponent<Combat_Menu_Controller>().OpenSubmenu(this);
        waiting = StartCoroutine(WaitForChoice());
    }

    public virtual IEnumerator WaitForChoice()
    {
        buttonChoice = -2;

        yield return new WaitUntil(() => buttonChoice != -2);

        print(buttonChoice);

        if(buttonChoice == -1)
        {
            Return();
        }
        else if (comfirmation)
        {
            transform.parent.Find("Confirm").gameObject.SetActive(true);
            yield return transform.parent.Find("Confirm").GetComponent<Combat_Menu>().waiting;
        }
    }

    public void Return()
    {
        transform.parent.GetComponent<Combat_Menu_Controller>().CloseSubmenu(transform.parent.GetComponent<Combat_Menu_Controller>().menuStack.Count - 1);

        Combat_Menu previous = transform.parent.GetComponent<Combat_Menu_Controller>().menuStack[transform.parent.GetComponent<Combat_Menu_Controller>().menuStack.Count - 1];
        previous.GetComponent<CanvasGroup>().interactable = true;

        new CoroutineWithData(previous, previous.WaitForChoice());

        gameObject.SetActive(false);
    }
}
