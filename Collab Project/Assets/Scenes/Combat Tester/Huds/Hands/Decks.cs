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

    private Coroutine cardCoroutine;

    public bool cardRemoved = false;

    private Hand_Slot cardToRemove;

    public Hand_Slot executedSlot;

    public Hand_Slot setSlot;

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
            else if (selectedSlot.card.skill.UseCondition() == true && executedSlot == null && !selectedSlot.card.skill.mustBeSet)
            {
                selectedSlot.executeButton.gameObject.SetActive(true);
            }
            else
            {
                selectedSlot.setButton.gameObject.SetActive(true);
            }
        }
    }

    public List<Transform> distinctTargets = new List<Transform>();

    public Stack<Card> resolveStack = new Stack<Card>();

    private void Start()
    {
        cardSize = card_Prefab.gameObject.GetComponent<RectTransform>().sizeDelta;
    }

    public IEnumerator ShuffleCards()
    {
        Debug.Log("Shuffle");

        for (int i = 0; i < drawDeck.childCount-1; i++)
        {
            drawDeck.GetChild(i).SetSiblingIndex(Random.Range(0, drawDeck.childCount));
            yield return null;
        }

    }

    public void SetSelectedSlot()
    {
        setSlot = SelectedSlot;

        SelectedSlot = null;

        setSlot.setButton.gameObject.SetActive(false);

        //executedSlot.card.card_Prefab.transform.localPosition = Vector2.up * 36;

        StartCoroutine(CardSetUp());

        cardsPlayed.Add(setSlot.card.skill.displayName);

        character.TurnController.endTurnButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "End Turn";

        IEnumerator CardSetUp()
        {
            Locked = true;

            character.TurnController.endTurnButton.interactable = false;

            cardRemoved = false;

            yield return setSlot.card.skill.SetUp();

            character.AdjustMana(setSlot.card.skill.manaCost, false);

            setSlot.set = true;

            character.TurnController.CheckAllCards();

            Locked = false;
        }
    }

    public void ExecuteSelectedSlot()
    {
        executedSlot = SelectedSlot;

        SelectedSlot = null;

        executedSlot.executeButton.gameObject.SetActive(false);

        //executedSlot.card.card_Prefab.transform.localPosition = Vector2.up * 36;

        StartCoroutine(CardSetUp());

        cardsPlayed.Add(executedSlot.card.skill.displayName);

        character.TurnController.endTurnButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "End Turn";

        IEnumerator CardSetUp()
        {
            Locked = true;

            character.TurnController.endTurnButton.interactable = false;

            cardRemoved = false;

            yield return executedSlot.card.skill.SetUp();

            resolveStack.Push(executedSlot.card);

            character.AdjustMana(executedSlot.card.skill.manaCost, false);

            yield return executedSlot.card.skill.Execute();

            character.TurnController.CheckAllCards();

            // Resolve Cards

            for (int i = 0; i < resolveStack.Count;)
            {
                Card card = resolveStack.Pop();

                //Debug.Break();

                if (!card.negated)
                {
                    Debug.Log(card.skill.name + " resolved");
                    yield return card.skill.Resolve();
                }
                else
                {
                    Debug.Log(card.skill.name + " negated");
                    card.negated = false;
                }

                yield return RemoveCard(card.transform.parent.GetComponent<Hand_Slot>());
            }

            Locked = false;

            distinctTargets.AddRange(executedSlot.card.skill.chosen_Targets);

            executedSlot.card.skill.chosen_Targets.Clear();

            executedSlot = null;
        }
    }

    public void DiscardSelectedCard()
    {
        cardToRemove = SelectedSlot;
    }


    public Hand_Slot CreateSlot()
    {
        for (int r = 0; r < hand.Count; r++)
        {
            hand[r].transform.SetParent(transform);
        }

        Hand_Slot newSlot = Instantiate(slot_Prefab, hand_Pos).GetComponent<Hand_Slot>();

        newSlot.transform.SetSiblingIndex(Random.Range(0, hand_Pos.childCount));

        newSlot.decks = this;

        return newSlot;
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

    public IEnumerator DrawCards(int amount, bool autoUnlock)
    {
        Locked = true;

        if (amount > drawDeck.childCount + discardDeck.childCount)
            amount = drawDeck.childCount + discardDeck.childCount;

        Debug.Log(amount);

        for (int i = 0; i < amount; i++)
        {
            if (drawDeck.childCount <= 0)
                yield return RecoupeCards();

            yield return PullCard();
        }

        if (autoUnlock)
            Locked = false;

        IEnumerator PullCard()
        {
            Hand_Slot newSlot = CreateSlot();

            yield return null;

            yield return ShiftCards();

            //int x = Random.Range(0, drawDeck.childCount);

            int x = 0;

            Card drawnCard = drawDeck.GetChild(x).GetComponent<Card>();

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

            hand = hand.OrderBy(o => o.transform.parent.GetSiblingIndex()).ToList();
        }
    }

    public IEnumerator RemoveCard(Hand_Slot slot)
    {
        Locked = true;

        for (int r = 0; r < slot.decks.hand.Count; r++)
        {
            if(slot.decks.hand[r] == slot.card)
            {
                slot.decks.hand[r].transform.SetParent(slot.decks.discardDeck);
                slot.decks.hand[r].GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
                slot.decks.hand[r].gameObject.SetActive(false);
            }
            else
                slot.decks.hand[r].transform.SetParent(slot.decks.transform.parent);
        }

        slot.GetComponent<Animation>().Play();

        slot.decks.discardDeckQuantity.text = (slot.decks.discardDeck.childCount).ToString();

        yield return new WaitForSeconds(slot.GetComponent<Animation>().clip.length);

        slot.decks.hand.Remove(slot.card);

        //Destroy(slot.card.gameObject); // Optional

        Destroy(slot.gameObject);

        yield return null;

        yield return ShiftCards();

        cardRemoved = true;

        Locked = false;
    }

    public IEnumerator Clear()
    {
        for (int i = 0; i < hand_Pos.childCount;)
        {
            if (hand_Pos.GetChild(i).GetComponent<Hand_Slot>().set)
            {
                i++;
                continue;
            }

            yield return RemoveCard(hand_Pos.GetChild(i).GetComponent<Hand_Slot>());
            yield return null;
        }
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

        yield return new WaitUntil(() => cardToRemove != null);


        foreach (Card card in hand)
            card.Discardable(false);

        yield return RemoveCard(cardToRemove);
        cardToRemove = null;

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
