using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

public class SubMenu_Button : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public SubMenu SubMenu { get; set; }
    public SubMenu_Controller_2 SubMenu_Controller_2 { get; set; }
    public TextMeshProUGUI buttonText;

    public Combat_Character.Skill storedSkill = null;

    public void StoreSkill(Combat_Character.Skill skill)
    {
        storedSkill = skill;
    }


    public void OnPointerEnter(PointerEventData eventData)
    {
        
        if (SubMenu_Controller_2.currentButtons[transform.GetSiblingIndex()].storedSkill != null)
        {
            SubMenu_Controller_2.Owner.TurnController.left_descriptionBox.container.SetActive(true);
            SubMenu_Controller_2.Owner.TurnController.left_descriptionBox.Description(SubMenu_Controller_2.currentButtons[transform.GetSiblingIndex()].storedSkill);
        }

        //SubMenu.hoveringButton = transform.GetSiblingIndex();
        //
        //if (storedSkill != null)
        //{
        //    SubMenu.SubMenuController.Owner.TurnController.left_descriptionBox.container.SetActive(true);
        //    SubMenu.SubMenuController.Owner.TurnController.left_descriptionBox.Description(storedSkill);
        //}

    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SubMenu_Controller_2.currentButtons[transform.GetSiblingIndex()].storedSkill != null)
        {
            SubMenu_Controller_2.Owner.TurnController.left_descriptionBox.container.SetActive(false);
        }

        //SubMenu.hoveringButton = -1;
        //
        //if (storedSkill != null)
        //    SubMenu.SubMenuController.Owner.TurnController.left_descriptionBox.container.SetActive(false);

    }
}
