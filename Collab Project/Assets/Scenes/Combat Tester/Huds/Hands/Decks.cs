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
            character.TurnController.endTurnButton.interactable = !value;

            foreach (Card card in hand)
            {
                if (executedSlot!= null && card == executedSlot.card)
                    continue;

                card.locked.gameObject.SetActive(value);
            }

            if(value == false)
                StartCoroutine(character.TurnController.Blink());
        }
    }

    public Combat_Character character;

    public Hand_Slot slot_Prefab;

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

    private List<Hand_Slot> cardsToRemove;

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
                selectedSlot.card.GetComponent<RectTransform>().anchoredPosition = Vector2.up * 12;

            if (discarding && selectedSlot != executedSlot)
            {
                selectedSlot.discardButton.gameObject.SetActive(true);
            }
            else if (executedSlot == null)
            {
                if (!selectedSlot.card.skill.set)
                {
                    if(selectedSlot.card.skill.SetCondition())
                        selectedSlot.setButton.gameObject.SetActive(true);
                }
                else
                {
                    if (selectedSlot.card.skill.ReactCondition())
                        selectedSlot.executeButton.gameObject.SetActive(true);
                }

                if (selectedSlot.card.skill.UseCondition())
                {
                    selectedSlot.executeButton.gameObject.SetActive(true);
                }
            }
        }
    }

    public List<Transform> distinctTargets = new List<Transform>();

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

    public void SetSelected()
    {
        SelectedSlot.setButton.gameObject.SetActive(false);

        SelectedSlot.card.skill.set = true;

        cardsPlayed.Add(SelectedSlot.card.skill.displayName);

        SelectedSlot = null;

        character.TurnController.endTurnButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "End Turn";

        //cardCoroutine = StartCoroutine(CardSetUp());

        //IEnumerator CardSetUp()
        //{
        //    Locked = true;
        //
        //    character.TurnController.endTurnButton.interactable = false;
        //
        //    cardRemoved = false;
        //
        //    yield return setSlot.card.skill.SetUp();
        //
        //    character.AdjustMana(setSlot.card.skill.manaCost, false);
        //
        //    setSlot.set = true;
        //
        //    character.TurnController.CheckAllCards();
        //
        //    Locked = false;
        //}
    }

    public Coroutine cardCoroutine;

    public void ExecuteSelected()
    {
        selectedSlot.executeButton.gameObject.SetActive(false);

        executedSlot = SelectedSlot;

        SelectedSlot = null;

        cardsPlayed.Add(executedSlot.card.skill.displayName);

        character.TurnController.endTurnButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "End Turn";

        cardCoroutine = StartCoroutine(CardSetUp());

        IEnumerator CardSetUp()
        {
            Locked = true;

            character.TurnController.endTurnButton.interactable = false;

            cardRemoved = false;

            character.TurnController.resolveStack.Add(executedSlot.card);

            yield return executedSlot.card.skill.SetUp();

            character.AdjustMana(executedSlot.card.skill.manaCost, false);

            StartCoroutine(RemoveSlots(new List<Hand_Slot>(){ executedSlot }, true));

            cardCoroutine = null;

            yield return executedSlot.card.skill.Execute();

            character.TurnController.CheckAllCards();


            if (character = character.TurnController.characterTurn)
                yield return character.TurnController.ResolveCards();

            Locked = false;

            distinctTargets.AddRange(executedSlot.card.skill.chosen_Targets);

            executedSlot.card.skill.chosen_Targets.Clear();
        }
    }

    public void DiscardSelectedCard()
    {

        ResetPreviousSlot();

        cardsToRemove = new List<Hand_Slot>()
        {
            SelectedSlot,
        };
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

        yield return new WaitForSeconds(0.5f);
    }

    public IEnumerator DrawCards(int amount, bool autoUnlock, bool startLeft)
    {
        Locked = true;

        if (amount > drawDeck.childCount + discardDeck.childCount)
            amount = drawDeck.childCount + discardDeck.childCount;

        Hand_Slot[] slots = CreateSlots(amount, startLeft);

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

        IEnumerator PullCard(Hand_Slot newSlot)
        {
            Card drawnCard = drawDeck.GetChild(0).GetComponent<Card>();

            drawnCard.hand = this;

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

    public Hand_Slot[] CreateSlots(int amount, bool startLeft)
    {
        for (int r = 0; r < hand.Count; r++)
        {
            hand[r].transform.SetParent(transform);
        }

        Hand_Slot[] newSlots = new Hand_Slot[amount];

        for (int i = 0; i < amount; i++)
        {
            newSlots[i] = Instantiate(slot_Prefab, hand_Pos).GetComponent<Hand_Slot>();

            if(startLeft)
                newSlots[i].transform.SetSiblingIndex(0);

            newSlots[i].decks = this;
        }


        return newSlots;
    }

    public IEnumerator RemoveSlots(List<Hand_Slot> slotsToRemove, bool autoShift)
    {
        Locked = true;

        float time = slotsToRemove[0].GetComponent<Animation>().clip.length;

        for (int i = 0; i < slotsToRemove.Count; i++)
        {
            slotsToRemove[i].GetComponent<Animation>().Play();
            slotsToRemove[i].card.gameObject.SetActive(false);

            yield return new WaitForSeconds(time/2);
        }

        yield return new WaitForSeconds(time / 2);

        for (int i = 0; i < slotsToRemove.Count; i++)
        {
            slotsToRemove[i].card.transform.SetParent(discardDeck);
            slotsToRemove[i].card.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            discardDeckQuantity.text = (discardDeck.childCount).ToString();

            hand.Remove(slotsToRemove[i].card);

            slotsToRemove[i].card.skill.set = false;

            Destroy(slotsToRemove[i].gameObject);
        }

        foreach (Transform slot in hand_Pos.transform)
        {
            slot.GetComponent<Hand_Slot>().card.transform.SetParent(transform.parent);
        }

        yield return null;

        cardRemoved = true;

        if(autoShift)
            yield return ShiftCards();

        Locked = false;
    }

    public IEnumerator Clear()
    {

        List<Hand_Slot> slots = new List<Hand_Slot>();


        foreach (Transform slot in hand_Pos)
        {
            if (slot.GetComponent<Hand_Slot>().card.skill.set)
                continue;

            slots.Add(slot.GetComponent<Hand_Slot>());
        }

        yield return RemoveSlots(slots, true);
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

        character.TurnController.instructions.text = "Discard a card";

        yield return new WaitWhile(() => cardsToRemove == null);


        foreach (Card card in hand)
            card.Discardable(false);

        yield return RemoveSlots(cardsToRemove, false);
        //cardsToRemove = null;

        Locked = true;
        discarding = false;

    }

    [HideInInspector]
    public float cardSpacing = 10f;

    public IEnumerator ShiftCards()
    {
        Coroutine retrieval = null;

        for (int r = 0; r < hand_Pos.childCount; r++)
        { 
            retrieval = StartCoroutine(hand_Pos.GetChild(r).GetComponent<Hand_Slot>().RetrieveCard());
        }

        yield return retrieval;
    }

    public void ResetPreviousSlot()
    {
        if (SelectedSlot == null)
            return;

        if (SelectedSlot == executedSlot)
            return;

        SelectedSlot.setButton.gameObject.SetActive(false);
        SelectedSlot.executeButton.gameObject.SetActive(false);
        SelectedSlot.discardButton.gameObject.SetActive(false);

        SelectedSlot.ResetCard();
    }

    public IEnumerator Raise( bool autoLock)
    {
        //gameObject.SetActive(true);

        Decks temp = character.Team.visibleDeck;

        character.Team.visibleDeck = this;

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

        Locked = autoLock;
    }

    public IEnumerator Lower(bool autoNull)
    {
        Locked = true;

        if(autoNull)
            character.Team.visibleDeck = null;

        ResetPreviousSlot();

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
