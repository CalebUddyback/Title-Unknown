using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SubMenu_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SubMenu SubMenu { get; set; }
    public TextMeshProUGUI buttonText;
    public string HoverTxt = "";

    public Combat_Character.Skill storedSkill = null;

    public void StoreSkill(Combat_Character.Skill skill)
    {
        storedSkill = skill;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {

        SubMenu.hoveringButton = transform.GetSiblingIndex();

        if (storedSkill != null)
        {
            SubMenu.SubMenuController.Owner.TurnController.left_descriptionBox.container.SetActive(true);
            SubMenu.SubMenuController.Owner.TurnController.left_descriptionBox.Description(storedSkill);
        }

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        SubMenu.hoveringButton = -1;

        if (storedSkill != null)
            SubMenu.SubMenuController.Owner.TurnController.left_descriptionBox.container.SetActive(false);

    }
}
