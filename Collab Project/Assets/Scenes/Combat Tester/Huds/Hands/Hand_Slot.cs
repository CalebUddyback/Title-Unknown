using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hand_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Hand hand;

    public Card card;

    public Button executeButton, discardButton;


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hand.SelectedSlot != this && hand.executedSlot != this && !hand.Locked)
        {
            //card.card_Prefab.transform.localPosition = Vector2.up * 12;

            card.card_Prefab.transform.localPosition = Vector2.up * 40f;
            card.card_Prefab.transform.localScale = Vector3.one * 1.5f;
            GetComponent<Canvas>().overrideSorting = true;
            hand.ResetPreviousSlot();
            hand.SelectedSlot = null;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hand.SelectedSlot != this && hand.executedSlot != this && !hand.Locked)
        {
            card.card_Prefab.transform.localPosition = Vector2.zero;
            card.card_Prefab.transform.localScale = Vector3.one;
            GetComponent<Canvas>().overrideSorting = false;
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (hand.SelectedSlot != this && !hand.Locked)
            hand.SelectedSlot = this;
    }

    public void ExecuteCard()
    {
        hand.ExecuteSelectedCard();
    }

    public void DiscardCard()
    {
        hand.DiscardSelectedCard();
    }

    public IEnumerator RetrieveCard()
    {
        card.transform.SetParent(transform);

        Vector3 startPos = card.GetComponent<RectTransform>().anchoredPosition;

        Vector3 startScale = card.transform.localScale;

        Vector3 targetPos = Vector3.zero;

        float timer = 0;
        float maxTime = 0.15f;

        while (timer < maxTime)
        {
            card.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startPos, targetPos, timer / maxTime);
            card.transform.localScale = Vector3.Lerp(startScale, Vector3.one, timer / maxTime);

            timer += Time.deltaTime;

            yield return null;
        }

        card.GetComponent<RectTransform>().anchoredPosition = targetPos;

        card.transform.localScale = Vector3.one;
    }
}
