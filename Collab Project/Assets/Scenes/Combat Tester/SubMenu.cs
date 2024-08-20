using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class SubMenu : MonoBehaviour
{
    public GameObject buttonPrefab;
    public Transform buttonContainer;
    public Button returnButton;
    public TextMeshProUGUI title;


    public int ButtonChoice = -2;       // -2 (Waiting), -1 (Return)

    public SubMenu_Controller SubMenuController { get; private set; }

    private void Awake()
    {
        SubMenuController = transform.parent.GetComponent<SubMenu_Controller>();

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
        //for (int i = 0; i < buttonContainer.childCount; i++)
        //{
        //    Destroy(buttonContainer.GetChild(i).gameObject);
        //}
        //
        //foreach (string label in stringList)
        //{
        //    SubMenu_Button buttonInst = Instantiate(buttonPrefab, buttonContainer).GetComponent<SubMenu_Button>();
        //    buttonInst.transform.GetChild(0).GetComponent<Text>().text = label;
        //}


        if (stringList.Count < buttonContainer.childCount)
        {
            for (int i = stringList.Count; i < buttonContainer.childCount; i++)
            {
                if (i < 4)
                    buttonContainer.GetChild(i).gameObject.SetActive(false);
                else
                    Destroy(buttonContainer.GetChild(i).gameObject);
            }
        }

        if(stringList.Count > buttonContainer.childCount)
        {
            for (int i = buttonContainer.childCount; i < stringList.Count; i++)
            {
                Instantiate(buttonPrefab, buttonContainer);
            }
        }

        for (int i = 0; i < stringList.Count; i++)
        {
            buttonContainer.GetChild(i).GetChild(0).GetComponent<TextMeshProUGUI>().text = stringList[i];
        }

        AddButtonListeners();
    }

    public void AddButtons(List<Combat_Character.Skill> skillList)
    {
        //for (int i = 0; i < buttonContainer.childCount; i++)
        //{
        //    Destroy(buttonContainer.GetChild(i).gameObject);
        //}
        //
        //foreach (Combat_Character.Skill skill in skillList)
        //{
        //    SubMenu_Button buttonInst = Instantiate(buttonPrefab, buttonContainer).GetComponent<SubMenu_Button>();
        //    buttonInst.StoreSkillInfo(skill);
        //    buttonInst.subMenu = this;
        //}


        if (skillList.Count < buttonContainer.childCount)
        {
            for (int i = skillList.Count; i < buttonContainer.childCount; i++)
            {
                if (i < 4)
                    buttonContainer.GetChild(i).gameObject.SetActive(false);
                else
                    Destroy(buttonContainer.GetChild(i).gameObject);
            }
        }

        if (skillList.Count > buttonContainer.childCount)
        {
            for (int i = buttonContainer.childCount; i < skillList.Count; i++)
            {
                Instantiate(buttonPrefab, buttonContainer);
            }
        }

        for (int i = 0; i < skillList.Count; i++)
        {
            SubMenu_Button buttonInst = buttonContainer.GetChild(i).GetComponent<SubMenu_Button>();
            buttonInst.StoreSkillInfo(skillList[i]);
            buttonInst.subMenu = this;
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

        //GetComponent<CanvasGroup>().interactable = false;

        yield return new WaitForSeconds(0.1f);  // This delay is to prevent quick double clicks (My Mouse is broken :/)

        if (ButtonChoice > -1)
            SubMenuController.subMenuStage = 0;
        else if (ButtonChoice == -1)
            Return();
    }

    public void Return()
    {
        ButtonChoice = -1;

        if(SubMenuController.menuStack.Count > 1)
            SubMenuController.subMenuStage = 1;

        SubMenuController.CloseSubMenu();
    }
}
