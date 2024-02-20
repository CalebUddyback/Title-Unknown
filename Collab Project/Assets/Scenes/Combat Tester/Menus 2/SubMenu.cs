using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Combat_Menu))]

public class SubMenu : MonoBehaviour
{
    public bool returnable;
    public string[] buttons;

    public Button returnButton;
    public Transform content;

    private void Start()
    {
        if (!returnable)
            returnButton.gameObject.SetActive(false);
    }

    public void Buttons(GameObject button)
    {
        foreach(string btn in buttons)
        {
            GameObject b = Instantiate(button, content);
            b.transform.GetChild(0).GetComponent<Text>().text = btn;
        }
    }
}
