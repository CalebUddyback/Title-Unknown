using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Decks : MonoBehaviour
{

    private bool locked = false;
    public bool Locked
    {
        get
        {
            return locked;
        }
        set
        {
            locked = value;
            owner.TurnController.endTurnButton.interactable = !value;

            foreach (Card card in hand)
            {
                if (executedSlot!= null && card == executedSlot.card)
                    continue;

                card.locked.gameObject.SetActive(value);
            }

            if(value == false)
                StartCoroutine(owner.TurnController.Blink());
        }
    }

    public Combat_Character owner;

    public Card_Slot slot_Prefab;

    public Card card_Prefab;

    public Vector2 cardSize;

    public Transform drawDeck;
    public TextMeshProUGUI drawDeckQuantity;

    public Transform hand_Pos;
    public List<Card> hand;

    public Transform discardDeck;
    public TextMeshProUGUI discardDeckQuantity;

    public List<string> cardsPlayed = new List<string>();

    public readonly int maxCardsInHand = 7;

    public bool discarding = false;

    public bool cardRemoved = false;

    public List<Card_Slot> cardsToRemove;

    public Card_Slot hoverSlot;

    public Card_Slot HoverSlot
    {
        get
        {
            return hoverSlot;
        }
        set
        {
            if(hoverSlot != null && hoverSlot != SelectedSlot)
                hoverSlot.ResetCard();
            
            hoverSlot = value;

            if (hoverSlot == null)
                return;

            hoverSlot.transform.localScale = Vector3.one * 2f;

            hoverSlot.GetComponent<RectTransform>().sizeDelta = new Vector2(cardSize.x + cardSpacing + 5, cardSize.y);
            hoverSlot.card.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, 0);

            LayoutRebuilder.ForceRebuildLayoutImmediate(hoverSlot.transform.parent.GetComponent<RectTransform>());
            hoverSlot.GetComponent<Canvas>().overrideSorting = true;
        }
    }

    public Card_Slot selectedSlot;

    public Card_Slot SelectedSlot
    {
        get
        {
            return selectedSlot;
        }
        set
        {
            if (value != null)
            {
                selectedSlot = value;

                if (selectedSlot != executedSlot)
                    selectedSlot.card.GetComponent<RectTransform>().anchoredPosition = Vector2.up * 12;

                StartCoroutine(selectedSlot.card.skill.CharacterTargeting());
            }
            else
            {
                if(owner.TurnController.hoveringOver != null)
                {
                    SelectedSlot.card.skill.chosen_Targets.Add(owner.TurnController.hoveringOver);

                    ExecutedSlot = selectedSlot;

                    owner.TurnController.hoveringOver = null;
                }

                if (selectedSlot != null)
                    SelectedSlot.ResetCard();

                selectedSlot = value;
            }
        }
    }

    public Card_Slot executedSlot;

    public Card_Slot ExecutedSlot
    {
        get
        {
            return executedSlot;
        }
        set
        {
            executedSlot = value;

            if (value == null)
                return;

            ExecuteSelected();
        }
    }

    public List<Combat_Character> distinctTargets = new List<Combat_Character>();

    private void Start()
    {
        cardSize = card_Prefab.gameObject.GetComponent<RectTransform>().sizeDelta;
    }

    public IEnumerator ShuffleCards()
    {
        for (int i = 0; i < drawDeck.childCount-1; i++)
        {
            drawDeck.GetChild(i).SetSiblingIndex(Random.Range(0, drawDeck.childCount));
            yield return null;
        }
    }

    public Coroutine cardCoroutine;

    public void ExecuteSelected()
    {
        Card card = executedSlot.card;

        cardsPlayed.Add(card.skill.displayName);

        owner.TurnController.endTurnButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "End Turn";

        cardCoroutine = StartCoroutine(CardSetUp());

        IEnumerator CardSetUp()
        {
            Locked = true;

            owner.TurnController.endTurnButton.interactable = false;

            cardRemoved = false;

            owner.TurnController.resolveStack.Add(card);

            yield return card.skill.SetUp();

            owner.AdjustMana(card.skill.manaCost, false);

            cardsToRemove.Add(executedSlot);

            StartCoroutine(RemoveSlots(cardsToRemove, true));

            executedSlot = null;

            cardCoroutine = null;

            yield return card.skill.Execute();

            owner.TurnController.CheckAllCards();

            if (owner = owner.TurnController.characterTurn)
                yield return owner.TurnController.ResolveCards();

            Locked = false;

            distinctTargets.AddRange(card.skill.chosen_Targets);

            card.skill.chosen_Targets.Clear();
        }
    }

    public void DiscardSelectedCard()
    {
        SelectedSlot = null;

        cardsToRemove.Add(selectedSlot);
    }

    public IEnumerator DiscardCards()
    {
        Locked = false;

        discarding = true;

        foreach (Card card in hand)
        {
            if (executedSlot != null && card == executedSlot.card)
                continue;

            card.Discardable(true);
        }

        owner.TurnController.instructions.text = "Discard a card";

        yield return new WaitWhile(() => cardsToRemove == null);


        foreach (Card card in hand)
            card.Discardable(false);

        //yield return RemoveSlots(cardsToRemove, false);
        //
        //cardsToRemove.Clear();

        discarding = false;

        Locked = false;

    }


    public IEnumerator RecoupeCards()
    {
        Debug.Log("Recoupe");

        int r = discardDeck.childCount;

        for (int i = 0; i < r; i++)
        {
            discardDeck.GetChild(0).SetParent(drawDeck);

            drawDeckQuantity.text = drawDeck.childCount.ToString();

            discardDeckQuantity.text = (discardDeck.childCount).ToString();

            yield return null;
        }

        yield return ShuffleCards();

        yield return new WaitForSeconds(0.4f);
    }

    public IEnumerator DrawCards(int amount, bool autoUnlock, bool startLeft)
    {
        Locked = true;

        if (amount > drawDeck.childCount + discardDeck.childCount)
            amount = drawDeck.childCount + discardDeck.childCount;

        Card_Slot[] slots = CreateSlots(amount, startLeft);

        yield return null;

        foreach(RectTransform slot in hand_Pos)
        {
            slot.sizeDelta = new Vector2(cardSize.x - cardSpacing, cardSize.y);
        }

        StartCoroutine(ShiftCards());

        Coroutine co = null;

        for (int i = 0; i < amount; i++)
        {
            if (drawDeck.childCount <= 0)
                yield return RecoupeCards();

            co = StartCoroutine(PullCard(slots[i]));

            yield return new WaitForSeconds(0.15f);
        }

        yield return co;

        hand = hand.OrderBy(o => o.transform.parent.GetSiblingIndex()).ToList();

        if (autoUnlock)
            Locked = false;

        IEnumerator PullCard(Card_Slot newSlot)
        {
            Card drawnCard = drawDeck.GetChild(0).GetComponent<Card>();

            drawnCard.deck = this;

            drawnCard.GetComponent<RectTransform>().anchoredPosition = new Vector3(newSlot.GetComponent<RectTransform>().localPosition.x, newSlot.GetComponent<RectTransform>().position.y);

            float displayScale = 1.5f;

            drawnCard.transform.localScale = new Vector3(displayScale, displayScale, 1);

            drawnCard.GetComponent<RectTransform>().localRotation *= Quaternion.Euler(0, -1, 0);

            drawnCard.locked.gameObject.SetActive(true);

            //yield return new WaitForSeconds(0.2f);

            newSlot.card = drawnCard;

            drawnCard.gameObject.SetActive(true);

            drawDeckQuantity.text = (drawDeck.childCount - 1).ToString();

            yield return newSlot.RetrieveCard();

            hand.Add(drawnCard);
        }
    }

    public Card_Slot[] CreateSlots(int amount, bool startLeft)
    {
        for (int r = 0; r < hand.Count; r++)
        {
            hand[r].transform.SetParent(transform);
        }

        Card_Slot[] newSlots = new Card_Slot[amount];

        for (int i = 0; i < amount; i++)
        {
            newSlots[i] = Instantiate(slot_Prefab, hand_Pos).GetComponent<Card_Slot>();

            if(startLeft)
                newSlots[i].transform.SetSiblingIndex(0);

            newSlots[i].deck = this;
        }


        return newSlots;
    }

    public IEnumerator RemoveSlots(List<Card_Slot> slotsToRemove, bool autoShift)
    {

        Locked = true;

        float time = slotsToRemove[0].GetComponent<Animation>().clip.length;

        for (int i = 0; i < slotsToRemove.Count; i++)
        {
            slotsToRemove[i].GetComponent<Animation>().Play();
            slotsToRemove[i].card.gameObject.SetActive(false);

            slotsToRemove[i].card.transform.SetParent(discardDeck);

            slotsToRemove[i].card.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            discardDeckQuantity.text = (discardDeck.childCount).ToString();

            hand.Remove(slotsToRemove[i].card);

            slotsToRemove[i].card.skill.set = false;

            slotsToRemove[i].card = null;

            yield return new WaitForSeconds(time/2);
        }

        yield return new WaitForSeconds(time / 2);

        foreach (Transform slot in hand_Pos.transform)
        {
            if (slot.GetComponent<Card_Slot>().card != null)
                slot.GetComponent<Card_Slot>().card.transform.SetParent(transform.parent);
        }

        yield return null;

        for (int i = 0; i < slotsToRemove.Count; i++)
        {
            Destroy(slotsToRemove[i].gameObject);

            yield return null;
        }

        cardRemoved = true;

        cardsToRemove = new List<Card_Slot>();

        if(autoShift)
            yield return ShiftCards();

        Locked = false;
    }

    public IEnumerator Clear()
    {

        List<Card_Slot> slots = new List<Card_Slot>();


        foreach (Transform slot in hand_Pos)
        {
            if (slot.GetComponent<Card_Slot>().card.skill.set)
                continue;

            slots.Add(slot.GetComponent<Card_Slot>());
        }

        yield return RemoveSlots(slots, true);
    }

    [HideInInspector]
    public float cardSpacing = 10f;

    public IEnumerator ShiftCards()
    {
        Locked = true;

        Coroutine retrieval = null;

        for (int r = 0; r < hand_Pos.childCount; r++)
        { 
            retrieval = StartCoroutine(hand_Pos.GetChild(r).GetComponent<Card_Slot>().RetrieveCard());
        }

        yield return retrieval;

        Locked = false;
    }

    public IEnumerator Raise( bool autoLock)
    {
        //gameObject.SetActive(true);

        if (owner.Team.visibleDeck != owner.deck)
        {

            Decks temp = owner.Team.visibleDeck;

            owner.Team.visibleDeck = this;

            if (temp != null && temp != this)
                yield return temp.Lower(false);

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

        Locked = autoLock;
    }

    public IEnumerator Lower(bool autoNull)
    {
        Locked = true;

        if(autoNull)
            owner.Team.visibleDeck = null;

        SelectedSlot = null; ;

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

        //gameObject.SetActive(false);
    }
}
