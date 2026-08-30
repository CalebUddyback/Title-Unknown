using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Linq;

public class Card_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IPointerUpHandler
{
    public Decks deck;

    public Card card;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!deck.Locked)
        {
            deck.HoverSlot = this;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!deck.Locked)
        {
            if (deck.HoverSlot != this)
                return;
            else
                deck.HoverSlot = null;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (deck.SelectedSlot != this && !deck.Locked)
            {
                deck.SelectedSlot = this;
            }
        }
    }


    public void OnPointerUp(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            deck.SelectedSlot = null;
        }
    }

    public void DiscardCard()
    {
        deck.DiscardSelectedCard();
    }

    public IEnumerator RetrieveCard()
    {
        if (card == null)
            yield break;

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
        }

        transform.localScale = Vector3.one;
        GetComponent<RectTransform>().sizeDelta = new Vector2(deck.cardSize.x - deck.cardSpacing, deck.cardSize.y);
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        GetComponent<Canvas>().overrideSorting = false;
    }
}
