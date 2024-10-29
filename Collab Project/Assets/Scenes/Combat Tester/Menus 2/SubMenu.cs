using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

public class SubMenu : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public Button returnButton;
    public TextMeshProUGUI title;

    public int hoveringButton = -1;
    public int ButtonChoice = -2;       // -2 (Waiting), -1 (Return)

    public SubMenu_Controller SubMenuController => transform.parent.GetComponent<SubMenu_Controller>();

    public Combat_Character Owner => SubMenuController.Owner;

    private void Awake()
    {
        title.text = gameObject.name;
    }

    private void Start()
    {
        returnButton.onClick.AddListener(() => ButtonChoice = -1);

        AdditionalSetup();
    }

    public virtual void AdditionalSetup() { }

    public void AddButtons(List<string> stringList)
    {

        if (stringList.Count < buttonContainer.childCount)
        {
            for (int i = stringList.Count; i < buttonContainer.childCount; i++)
            {
                buttonContainer.GetChild(i).gameObject.SetActive(false);
            }
        }

        if(stringList.Count > buttonContainer.childCount)
        {
            for (int i = buttonContainer.childCount; i < stringList.Count; i++)
            {
                Button newButton = Instantiate(buttonPrefab, buttonContainer).GetComponent<Button>();

                newButton.onClick.AddListener(() => ButtonChoice = newButton.transform.GetSiblingIndex());
            }
        }

        for (int i = 0; i < stringList.Count; i++)
        {
            SubMenu_Button buttonInst = buttonContainer.GetChild(i).GetComponent<SubMenu_Button>();
            buttonInst.buttonText.text = stringList[i];
            buttonInst.SubMenu = this;
        }
    }

    public void StoreInButton(List<Combat_Character.Skill> skillList)
    {
        for (int i = 0; i < skillList.Count; i++)
        {
            buttonContainer.GetChild(i).GetComponent<SubMenu_Button>().StoreSkill(skillList[i]);
        }
    }

    public virtual IEnumerator WaitForChoice()
    {
        hoveringButton = -1;

        ButtonChoice = -2;

        GetComponent<ScrollRect>().verticalScrollbar.value = 1; // may not be needed for  returning to submenu

        yield return new WaitUntil(() => ButtonChoice != -2);

        //print("Button: " + ButtonChoice);

        GetComponent<CanvasGroup>().interactable = false;

        yield return new WaitForSeconds(0.1f);  // This delay is to prevent quick double clicks (My Mouse is broken :/)

        GetComponent<CanvasGroup>().interactable = true;

        if (ButtonChoice > -1)
            SubMenuController.subMenuStage = 0;
        else if (ButtonChoice == -1)
            Return();
    }

    public void Return()
    {
        ButtonChoice = -1;

        if(SubMenuController.subMenuList.Count > 1)
            SubMenuController.subMenuStage = 1;

        SubMenuController.CloseSubMenu();
    }
}
