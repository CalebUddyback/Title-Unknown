using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(SubMenu))]

public abstract class Combat_Menu : MonoBehaviour
{
    public int buttonChoice = -2;       // -2 (Waiting), -1 (Return)

    public bool requirement_Menu = false;  // Decide in inspector

    public Combat_Menu_Controller Controller { get; private set; }

    private void Start()
    {
        Controller = transform.parent.GetComponent<Combat_Menu_Controller>();

        foreach(Transform button in GetComponent<SubMenu>().content)
        {
            button.GetComponent<Button>().onClick.AddListener(() => buttonChoice = button.GetSiblingIndex());
        }

        GetComponent<SubMenu>().returnButton.onClick.AddListener(() => buttonChoice = -1);
    }

    public virtual IEnumerator WaitForChoice()
    {
        buttonChoice = -2;

        yield return new WaitUntil(() => buttonChoice != -2);

        //print("Button: " + buttonChoice);

        if (buttonChoice == -1)
        {
            Return();
        }
    }

    public void Return()
    {
        buttonChoice = -1;

        Controller.CloseSubmenu();
    }
}
