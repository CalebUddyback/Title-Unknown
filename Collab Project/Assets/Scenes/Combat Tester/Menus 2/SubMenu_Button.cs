using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System.Collections.Generic;

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

    public bool returnButton = false;


    public void OnPointerEnter(PointerEventData eventData)
    {
        if(!returnButton)
            SubMenu_Controller_2.hovering = transform.GetSiblingIndex();


        //if (SubMenu_Controller_2.currentButtons[transform.GetSiblingIndex()].StoredInfo is Combat_Character.Skill)
        //{
        //    SubMenu_Controller_2.Owner.TurnController.left_descriptionBox.container.SetActive(true);
        //    SubMenu_Controller_2.Owner.TurnController.left_descriptionBox.Description(SubMenu_Controller_2.currentButtons[transform.GetSiblingIndex()].StoredInfo as Combat_Character.Skill);
        //}

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
        //if (SubMenu_Controller_2.currentButtons[transform.GetSiblingIndex()].StoredInfo != null)
        //{
        //    SubMenu_Controller_2.Owner.TurnController.left_descriptionBox.container.SetActive(false);
        //}

        //SubMenu.hoveringButton = -1;
        //
        //if (storedSkill != null)
        //    SubMenu.SubMenuController.Owner.TurnController.left_descriptionBox.container.SetActive(false);

    }
}
