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

    public int storedIndex = -1;

    public void StoreSkillInfo(int index)
    {
        storedIndex = index;

        //buttonText.text = skill.name;
        //HoverTxt = skill.description;
        //skillInfo = skill;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (subMenu != null)
        {
            //subMenu.SubMenuController.owner.TurnController.descriptionBox.gameObject.SetActive(true);
            //subMenu.SubMenuController.owner.TurnController.descriptionBox.title.text = skillInfo.name;
            //
            //subMenu.SubMenuController.owner.TurnController.descriptionBox.DMG_Txt.text = Mathf.Abs(skillInfo.baseInfo[0].damage).ToString();
            //subMenu.SubMenuController.owner.TurnController.descriptionBox.CRT_Txt.text = subMenu.SubMenuController.owner.stats.GetCurrentStats(skillInfo)[Stats.Stat.Crit].ToString();
            //subMenu.SubMenuController.owner.TurnController.descriptionBox.REC_Txt.text = (skillInfo.focusPenalty + subMenu.SubMenuController.owner.focusSpeed).ToString();
            //subMenu.SubMenuController.owner.TurnController.descriptionBox.HIT_Txt.text = subMenu.SubMenuController.owner.stats.GetCurrentStats(skillInfo)[Stats.Stat.PhHit].ToString();
            //
            //string typeText = "[" + skillInfo.baseInfo[0].type.ToString().ToUpper();
            //
            //if (skillInfo.effect)
            //{
            //    typeText += " / EFFECT";
            //}
            //
            //typeText += "]";
            //
            //subMenu.SubMenuController.owner.TurnController.descriptionBox.type.text = typeText;
            //
            //subMenu.SubMenuController.owner.TurnController.descriptionBox.descriptionText.text = HoverTxt;
            //
            //
            //
            subMenu.hoveringButton = transform.GetSiblingIndex();
            subMenu.SubMenuController.owner.TurnController.descriptionBox.UpdateInfo();
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (subMenu != null)
        {
            //subMenu.SubMenuController.owner.TurnController.descriptionBox.gameObject.SetActive(false);

            subMenu.hoveringButton = -1;
            subMenu.SubMenuController.owner.TurnController.descriptionBox.UpdateInfo();
        }
    }
}
