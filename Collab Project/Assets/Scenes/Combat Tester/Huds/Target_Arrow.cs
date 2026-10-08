using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Target_Arrow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Combat_Character owner;

    public Color hoverColor = Color.white;
    public Color unHoverColor = new Color(0, 0, 0, 0.5f);

    private void OnDisable()
    {
        Hovering(false);
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        Hovering(true);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        Hovering(false);
    }

    public void Hovering(bool x)
    {

        if (x)
            owner.TurnController.CharacterHoveringOver = owner;
        else
            owner.TurnController.CharacterHoveringOver = null;

        Highlight(x);
    }

    public void Highlight(bool x)
    {

        if (x)
        {
            GetComponent<Image>().color = hoverColor;
            owner.animationController.GetComponent<SpriteRenderer>().material.SetFloat("Outline_Thickness", 1f);
        }
        else
        {
            GetComponent<Image>().color = unHoverColor;
            owner.animationController.GetComponent<SpriteRenderer>().material.SetFloat("Outline_Thickness", 0f);
        }

    }

    private void LateUpdate()
    {
        GetComponent<RectTransform>().anchoredPosition = owner.TurnController.mainCamera.UIPosition(owner.outcome_Bubble_Pos.position);
    }
}
