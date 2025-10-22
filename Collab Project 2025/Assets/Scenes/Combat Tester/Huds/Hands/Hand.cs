using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Hand : MonoBehaviour
{

    public bool locked = false;
    public bool Locked
    {
        get
        {
            return locked;
        }
        set
        {
            locked = value;
            character.TurnController.endTurnButton.interactable = !value;

            foreach (Card card in cards)
            {
                if (executedSlot!= null && card == executedSlot.card)
                    continue;

                card.card_Prefab.locked.gameObject.SetActive(value);
            }
        }
    }

    public Combat_Character character;

    public Hand_Slot slot_Prefab;

    public Card_Prefab card_Prefab;

    public List<Card> cards;

    public int cardsPlayed = 0;

    public readonly int maxCardsInHand = 7;

    public bool discarding = false;

    private Coroutine cardCoroutine;

    public bool cardRemoved = false;

    private Hand_Slot cardsToRemove;

    public Hand_Slot executedSlot;

    private Hand_Slot selectedSlot;

    public Hand_Slot SelectedSlot
    {
        get
        {
            return selectedSlot;
        }
        set
        {
            ResetPreviousSlot();

            selectedSlot = value;

            if (value == null)
                return;

            if (selectedSlot != executedSlot)
                selectedSlot.card.card_Prefab.transform.localPosition = Vector2.up * 24;

            if (discarding && selectedSlot != executedSlot)
            {
                selectedSlot.discardButton.gameObject.SetActive(true);
            }
            else if (selectedSlot.card.UseCondition() == true && executedSlot == null)
            {
                selectedSlot.executeButton.gameObject.SetActive(true);
            }
        }
    }

    public List<Transform> distinctTargets = new List<Transform>();

    public void ExecuteSelectedCard()
    {
        executedSlot = SelectedSlot;

        SelectedSlot = null;

        executedSlot.executeButton.gameObject.SetActive(false);

        executedSlot.card.card_Prefab.transform.localPosition = Vector2.up * 36;

        StartCoroutine(CardSetUp());

        cardsPlayed++;

        character.TurnController.endTurnButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "End Turn";
    }

    public void DiscardSelectedCard()
    {
        cardsToRemove = SelectedSlot;
    }

    IEnumerator CardSetUp()
    {
        Locked = true; 

        character.TurnController.endTurnButton.interactable = false;

        cardRemoved = false;

        yield return executedSlot.card.SetUp();

        cardCoroutine = StartCoroutine(executedSlot.card.Action());

        character.Mana -= executedSlot.card.stats.mana;

        character.TurnController.CheckAllCards();

        yield return null;

        yield return RemoveCard(executedSlot);

        Locked = true;

        yield return cardCoroutine;

        Locked = false;

        //distinctTargets.Add(character.transform);
        //distinctTargets.AddRange(executedSlot.card.chosen_Targets.Select(o => o.transform).Distinct());

        distinctTargets.AddRange(executedSlot.card.chosen_Targets);

        executedSlot.card.chosen_Targets.Clear();

        executedSlot = null;
    }

    public IEnumerator GenerateCards(int amount, bool autoLock)
    {
        Locked = true;

        for (int i = 0; i < amount; i++)
        {
            for (int r = 0; r < cards.Count; r++)
            {
                cards[r].transform.SetParent(transform.parent);
            }

            Hand_Slot newSlot = Instantiate(slot_Prefab, transform).GetComponent<Hand_Slot>();

            newSlot.transform.SetSiblingIndex(Random.Range(0, transform.childCount));

            newSlot.hand = this;

            yield return null;

            yield return ShiftCards();

            int x = Random.Range(0, character.Deck.Length);

            Card newCard = Instantiate(character.Deck[x], new Vector2(newSlot.transform.position.x, newSlot.transform.position.y + 200), Quaternion.identity).GetComponent<Card>();

            newCard.hand = this;

            newSlot.card = newCard;

            newCard.card_Prefab = Instantiate(card_Prefab, newCard.transform).GetComponent<Card_Prefab>();

            newCard.card_Prefab.locked.gameObject.SetActive(true);

            yield return newSlot.RetrieveCard();

            cards.Add(newCard);

            cards = cards.OrderBy(o => o.transform.parent.GetSiblingIndex()).ToList();
        }

        if(autoLock)
            Locked = false;
    }

    public IEnumerator GenerateCards(Card card, int amount, bool autoLock)
    {
        Locked = true;

        for (int i = 0; i < amount; i++)
        {
            for (int r = 0; r < cards.Count; r++)
            {
                cards[r].transform.SetParent(transform.parent);
            }

            Hand_Slot newSlot = Instantiate(slot_Prefab, transform).GetComponent<Hand_Slot>();

            newSlot.transform.SetSiblingIndex(Random.Range(0, transform.childCount));

            newSlot.hand = this;

            yield return null;

            yield return ShiftCards();

            Card newCard = Instantiate(card, new Vector2(newSlot.transform.position.x, newSlot.transform.position.y + 200), Quaternion.identity).GetComponent<Card>();

            newCard.hand = this;

            newSlot.card = newCard;

            yield return newSlot.RetrieveCard();

            cards.Add(newCard);

            cards = cards.OrderBy(o => o.transform.parent.GetSiblingIndex()).ToList();
        }

        if (autoLock)
        {
            yield return new WaitForSeconds(0.3f);
            Locked = false;
        }
    }

    public IEnumerator RemoveCard(Hand_Slot slot)
    {
        Locked = true;

        for (int r = 0; r < cards.Count; r++)
        {
            cards[r].transform.SetParent(transform.parent);
        }

        cards.Remove(slot.card);

        Destroy(slot.card.gameObject); // Optional

        Destroy(slot.gameObject);

        yield return null;

        yield return ShiftCards();

        cardRemoved = true;

        Locked = false;
    }

    public IEnumerator DiscardCards(int amount)
    {
        Locked = false;
        discarding = true;

        foreach (Card card in cards)
        {
            if (executedSlot != null && card == executedSlot.card)
                continue;

            card.card_Prefab.Discardable(true);
        }

        int i = 0;

        while(i < amount)
        {
            character.TurnController.instructions.text = "Discard " + (amount - i) + (((amount - i) > 1) ? " Cards" : " Card");

            yield return new WaitUntil(() => cardsToRemove != null);

            if (i == amount - 1)
            {
                foreach (Card card in cards)
                    card.card_Prefab.Discardable(false);
            }

            yield return RemoveCard(cardsToRemove);
            cardsToRemove = null;
            i++;
        }

        Locked = true;
        discarding = false;

    }

    private IEnumerator ShiftCards()
    {
        int handMinCardCapcity = 5;
        
        float cardWidth = 125f;

        float spacing = 5f;
        
        if (transform.childCount <= handMinCardCapcity)
            GetComponent<HorizontalLayoutGroup>().spacing = spacing;
        else
        {
            float cw = (cardWidth / (transform.childCount - 1)) * (transform.childCount - handMinCardCapcity);

            float x = (cardWidth - cw) * (transform.childCount - 1) + cardWidth;

            float y = (cardWidth * handMinCardCapcity) + (spacing * (handMinCardCapcity - 1));

            float sw = (x - y) / (transform.childCount - 1);

            GetComponent<HorizontalLayoutGroup>().spacing = -(cw + sw);
        }

        yield return null;


        Coroutine retrieval = null;

        for (int r = 0; r < transform.childCount; r++)
        {
            if (transform.GetChild(r).GetComponent<Hand_Slot>().card == null)
                continue;

            retrieval = StartCoroutine(transform.GetChild(r).GetComponent<Hand_Slot>().RetrieveCard());
        }

        yield return retrieval;

        yield return null;
    }

    public void ResetPreviousSlot()
    {
        if (SelectedSlot == null)
            return;

        if (SelectedSlot == executedSlot)
            return;

        SelectedSlot.executeButton.gameObject.SetActive(false);
        SelectedSlot.discardButton.gameObject.SetActive(false);

        SelectedSlot.card.card_Prefab.transform.localPosition = Vector2.zero;
    }

    public IEnumerator Raise()
    {
        gameObject.SetActive(true);

        Vector3 startPos = new Vector3(0, -170f, 0);

        Vector3 targetPos = new Vector3(0, 0, 0);

        float timer = 0;
        float maxTime = 0.2f;

        while (timer < maxTime)
        {
            GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startPos, targetPos, timer / maxTime);

            timer += Time.deltaTime;

            yield return null;
        }

        GetComponent<RectTransform>().anchoredPosition = targetPos;
    }

    public IEnumerator Lower()
    {
        Locked = true;

        ResetPreviousSlot();

        yield return new WaitForSeconds(0.3f);

        Vector3 startPos = new Vector3(0, 0, 0);

        Vector3 targetPos = new Vector3(0, -170f, 0);

        float timer = 0;
        float maxTime = 0.2f;

        while (timer < maxTime)
        {
            GetComponent<RectTransform>().anchoredPosition = Vector3.Lerp(startPos, targetPos, timer / maxTime);

            timer += Time.deltaTime;

            yield return null;
        }

        GetComponent<RectTransform>().anchoredPosition = targetPos;


        gameObject.SetActive(false);
    }
}
