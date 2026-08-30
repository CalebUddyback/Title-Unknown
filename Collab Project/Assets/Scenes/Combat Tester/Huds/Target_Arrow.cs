using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Target_Arrow : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public Combat_Character owner;

    Color arrowColor = Color.black;


    private void OnEnable()
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
            owner.TurnController.hoveringOver = owner;
        else
            owner.TurnController.hoveringOver = null;

        Highlight(x);
    }

    public void Highlight(bool x)
    {

        if(x)
            GetComponent<Image>().color = arrowColor;
        else
            GetComponent<Image>().color = new Color(arrowColor.r, arrowColor.g, arrowColor.b, 0.5f);

    }

    private void LateUpdate()
    {
        GetComponent<RectTransform>().anchoredPosition = owner.TurnController.mainCamera.UIPosition(owner.outcome_Bubble_Pos.position);
    }
}
