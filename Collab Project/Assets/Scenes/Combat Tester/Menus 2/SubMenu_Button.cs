using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SubMenu_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SubMenu subMenu { get; set; }
    private Combat_Character.Skill skillInfo = null;
    public TextMeshProUGUI buttonText;
    public string HoverTxt = "";

    public void StoreSkillInfo(Combat_Character.Skill skill)
    {
        buttonText.text = skill.name;
        HoverTxt = skill.description;
        skillInfo = skill;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (subMenu != null)
        {
            subMenu.SubMenuController.owner.TurnController.descriptionBox.gameObject.SetActive(true);
            subMenu.SubMenuController.owner.TurnController.descriptionBox.title.text = skillInfo.name;

            subMenu.SubMenuController.owner.TurnController.descriptionBox.DMGTxt.text = Mathf.Abs(skillInfo.baseInfo[0].damage).ToString();

            subMenu.SubMenuController.owner.TurnController.descriptionBox.RECTxt.text = skillInfo.focusPenalty.ToString();

            string typeText = "[" + skillInfo.baseInfo[0].type.ToString().ToUpper();

            if (skillInfo.effect)
            {
                typeText += " / EFFECT";
            }

            typeText += "]";

            subMenu.SubMenuController.owner.TurnController.descriptionBox.type.text = typeText;

            subMenu.SubMenuController.owner.TurnController.descriptionBox.descriptionText.text = HoverTxt;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (subMenu != null)
        {
            subMenu.SubMenuController.owner.TurnController.descriptionBox.gameObject.SetActive(false);
        }
    }
}
