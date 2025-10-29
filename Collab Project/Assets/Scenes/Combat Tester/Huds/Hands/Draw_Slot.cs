using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draw_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    [HideInInspector]
    public Draw_Selection draw_Selection;

    public Animation animator;

    [HideInInspector]
    public Card card;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if(draw_Selection.SelectedSlot != this)
            GetComponent<Image>().color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (draw_Selection.SelectedSlot != this)
            GetComponent<Image>().color = Color.grey;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (draw_Selection.SelectedSlot != this)
            draw_Selection.SelectedSlot = this;
    }
}
