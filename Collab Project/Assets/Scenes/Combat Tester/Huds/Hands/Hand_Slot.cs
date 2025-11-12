using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hand_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Decks hand;

    public Card card;

    public Button executeButton, discardButton;


    public void OnPointerEnter(PointerEventData eventData)
    {
        if (hand.SelectedSlot != this && hand.executedSlot != this && !hand.Locked)
        {
            //card.GetComponent<RectTransform>().anchoredPosition  = Vector2.up * 12f;        
            transform.localScale = Vector3.one * 2f;

            GetComponent<RectTransform>().sizeDelta = new Vector2(hand.cardSize.x + hand.cardSpacing + 5, hand.cardSize.y);
            card.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
            GetComponent<Canvas>().overrideSorting = true;
            hand.ResetPreviousSlot();
            hand.SelectedSlot = null;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (hand.SelectedSlot != this && hand.executedSlot != this && !hand.Locked)
        {
            ResetCard();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (hand.SelectedSlot != this && !hand.Locked)
            hand.SelectedSlot = this;
    }

    public void ExecuteCard()
    {
        hand.ExecuteSelectedSlot();
    }

    public void DiscardCard()
    {
        hand.DiscardSelectedCard();
    }

    public IEnumerator RetrieveCard()
    {
        if (card == null)
            yield break;

        GetComponent<RectTransform>().sizeDelta = new Vector2(hand.cardSize.x - hand.cardSpacing, hand.cardSize.y);

        card.transform.SetParent(transform);

        Vector3 startPos = card.GetComponent<RectTransform>().anchoredPosition;

        Vector3 startScale = card.transform.localScale;

        Vector3 targetPos = Vector3.zero;

        float timer = 0;
        float maxTime = 0.3f;

        while (timer < maxTime)
        {
            card.GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startPos, targetPos, timer / maxTime);
            card.transform.localScale = Vector3.Lerp(startScale, Vector3.one, timer / maxTime);

            timer += Time.deltaTime;

            yield return null;
        }

        card.GetComponent<RectTransform>().anchoredPosition = targetPos;

        card.transform.localScale = Vector3.one;

        yield return null;
    }

    public void ResetCard()
    {
        if (card != null)
        {
            card.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
            card.GetComponent<RectTransform>().localRotation *= Quaternion.Euler(0, -1, 0);
        }

        transform.localScale = Vector3.one;
        GetComponent<RectTransform>().sizeDelta = new Vector2(hand.cardSize.x - hand.cardSpacing, hand.cardSize.y);
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        GetComponent<Canvas>().overrideSorting = false;
    }
}
