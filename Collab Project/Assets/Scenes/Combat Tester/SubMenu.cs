using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubMenu : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public Button returnButton;

    public int ButtonChoice { get; private set; } = -2;       // -2 (Waiting), -1 (Return)

    public SubMenu_Controller SubMenuController { get; private set; }

    private void Awake()
    {
        SubMenuController = transform.parent.GetComponent<SubMenu_Controller>();
    }

    private void Start()
    {
        returnButton.onClick.AddListener(() => ButtonChoice = -1);

        AdditionalSetup();
    }

    public virtual void AdditionalSetup() { }

    public void AddButtons(List<string> stringList)
    {
        for (int i = 0; i < buttonContainer.childCount; i++)
        {
            Destroy(buttonContainer.GetChild(i).gameObject);
        }

        foreach (string label in stringList)
        {
            Instantiate(buttonPrefab, buttonContainer).transform.GetChild(0).GetComponent<Text>().text = label;
        }

        AddButtonListeners();
    }

    public void AddButtonListeners()
    {
        foreach (Transform button in buttonContainer)
        {
            button.GetComponent<Button>().onClick.RemoveAllListeners();
            button.GetComponent<Button>().onClick.AddListener(() => ButtonChoice = button.GetSiblingIndex());
        }
    }

    public virtual IEnumerator WaitForChoice()
    {
        ButtonChoice = -2;

        yield return new WaitUntil(() => ButtonChoice != -2);

        //print("Button: " + ButtonChoice);

        GetComponent<CanvasGroup>().interactable = false;

        yield return new WaitForSeconds(0.1f);

        if (ButtonChoice == -1)
        {
            Return();
        }
    }

    public void Return()
    {
        ButtonChoice = -1;

        SubMenuController.CloseSubMenu();
    }
}
