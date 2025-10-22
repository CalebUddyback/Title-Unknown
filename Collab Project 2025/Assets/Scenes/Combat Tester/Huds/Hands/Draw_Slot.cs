using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Draw_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Draw_Selection draw_Selection;

    public Card card;

    public void OnPointerEnter(PointerEventData eventData)
    {
        GetComponent<Image>().color = Color.white;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        GetComponent<Image>().color = Color.grey;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        draw_Selection.chosenCard = card;
        GetComponent<Image>().color = Color.grey;
    }
}
