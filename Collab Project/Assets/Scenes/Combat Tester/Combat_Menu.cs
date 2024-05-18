using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SubMenu))]

public abstract class Combat_Menu : MonoBehaviour
{
    public int ButtonChoice { get; private set; } = -2;       // -2 (Waiting), -1 (Return)

    public int dependant_Variable = 0;

    public Combat_Menu_Controller Controller { get; private set; }

    private void Start()
    {
        Controller = transform.parent.GetComponent<Combat_Menu_Controller>();

        foreach(Transform button in GetComponent<SubMenu>().content)
        {
            button.GetComponent<Button>().onClick.AddListener(() => ButtonChoice = button.GetSiblingIndex());
        }

        GetComponent<SubMenu>().returnButton.onClick.AddListener(() => ButtonChoice = -1);
    }

    public virtual IEnumerator WaitForChoice()
    {
        ButtonChoice = -2;

        yield return new WaitUntil(() => ButtonChoice != -2);

        //print("Button: " + buttonChoice);

        if (ButtonChoice == -1)
        {
            Return();
        }
    }

    public void Return()
    {
        ButtonChoice = -1;

        Controller.CloseSubmenu();
    }
}
