using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Hand_Slot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public Decks decks;

    public Card card;

    public Button setButton, executeButton, discardButton;

    public bool set = false;

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (decks.SelectedSlot != this && decks.executedSlot != this && !decks.Locked)
        {
            //card.GetComponent<RectTransform>().anchoredPosition  = Vector2.up * 12f;        
            transform.localScale = Vector3.one * 2f;

            GetComponent<RectTransform>().sizeDelta = new Vector2(decks.cardSize.x + decks.cardSpacing + 5, decks.cardSize.y);
            card.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);

            LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
            GetComponent<Canvas>().overrideSorting = true;
            decks.ResetPreviousSlot();
            decks.SelectedSlot = null;
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (decks.SelectedSlot != this && decks.executedSlot != this && !decks.Locked)
        {
            ResetCard();
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
        {
            if (decks.SelectedSlot != this && !decks.Locked)
                decks.SelectedSlot = this;
        }
    }

    public void SetCard()
    {
        decks.SetSelectedSlot();
    }

    public void ExecuteCard()
    {
        decks.ExecuteSelectedSlot();
    }

    public void DiscardCard()
    {
        decks.DiscardSelectedCard();
    }

    public IEnumerator RetrieveCard()
    {
        if (card == null)
            yield break;

        GetComponent<RectTransform>().sizeDelta = new Vector2(decks.cardSize.x - decks.cardSpacing, decks.cardSize.y);

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
        GetComponent<RectTransform>().sizeDelta = new Vector2(decks.cardSize.x - decks.cardSpacing, decks.cardSize.y);
        LayoutRebuilder.ForceRebuildLayoutImmediate(transform.parent.GetComponent<RectTransform>());
        GetComponent<Canvas>().overrideSorting = false;
    }
}
