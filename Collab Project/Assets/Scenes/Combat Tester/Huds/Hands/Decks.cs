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

    public Transform drawDeck_Pos;
    public List<Card> drawDeck;

    public Transform hand_Pos;
    public List<Card> hand;

    public Transform discardDeck_Pos;
    public List<Card> discardDeck;

    public List<string> cardsPlayed = new List<string>();

    public readonly int maxCardsInHand = 7;

    public bool discarding = false;

    private Coroutine cardCoroutine;

    public bool cardRemoved = false;

    private Hand_Slot cardToRemove;

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
            else if (selectedSlot.card.skill.UseCondition() == true && executedSlot == null)
            {
                selectedSlot.executeButton.gameObject.SetActive(true);
            }
        }
    }

    public List<Transform> distinctTargets = new List<Transform>();

    private void Start()
    {
        cardSize = card_Prefab.gameObject.GetComponent<RectTransform>().sizeDelta;

        foreach (Transform skill in character.skills.transform)
        {
            Card card = Instantiate(card_Prefab, drawDeck_Pos);

            card.GetComponent<RectTransform>().anchoredPosition = drawDeck_Pos.GetComponent<RectTransform>().anchoredPosition;

            card.gameObject.SetActive(false);

            card.gameObject.name = skill.name + " Card";

            card.skill = skill.GetComponent<Skill>();

            drawDeck.Add(card);
        }
    }

    public void ExecuteSelectedCard()
    {
        executedSlot = SelectedSlot;

        SelectedSlot = null;

        executedSlot.executeButton.gameObject.SetActive(false);

        //executedSlot.card.card_Prefab.transform.localPosition = Vector2.up * 36;

        StartCoroutine(CardSetUp());

        cardsPlayed.Add(executedSlot.card.skill.displayName);

        character.TurnController.endTurnButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "End Turn";
    }

    public void DiscardSelectedCard()
    {
        cardToRemove = SelectedSlot;
    }

    IEnumerator CardSetUp()
    {
        Locked = true; 

        character.TurnController.endTurnButton.interactable = false;

        cardRemoved = false;

        yield return executedSlot.card.skill.SetUp();

        cardCoroutine = StartCoroutine(executedSlot.card.skill.Action());

        character.Mana(executedSlot.card.skill.manaCost, false);

        Debug.Log(character.Mana());

        character.TurnController.CheckAllCards();

        yield return null;

        yield return RemoveCard(executedSlot);

        Locked = true;

        yield return cardCoroutine;

        Locked = false;

        distinctTargets.AddRange(executedSlot.card.skill.chosen_Targets);

        executedSlot.card.skill.chosen_Targets.Clear();

        executedSlot = null;
    }

    public Hand_Slot CreateSlot()
    {
        for (int r = 0; r < hand.Count; r++)
        {
            hand[r].transform.SetParent(transform);
        }

        Hand_Slot newSlot = Instantiate(slot_Prefab, hand_Pos).GetComponent<Hand_Slot>();

        newSlot.transform.SetSiblingIndex(Random.Range(0, hand_Pos.childCount));

        newSlot.hand = this;

        return newSlot;
    }

    public IEnumerator DrawCards(int amount, bool autoUnlock)
    {
        Locked = true;

        if (amount > drawDeck.Count)
            amount = drawDeck.Count;

        for (int i = 0; i < amount; i++)
        {

            Hand_Slot newSlot = CreateSlot();

            yield return null;

            yield return ShiftCards();

            int x = Random.Range(0, drawDeck.Count);

            Card drawnCard = drawDeck[x];

            drawDeck.Remove(drawnCard);

            drawnCard.hand = this;

            drawnCard.GetComponent<RectTransform>().anchoredPosition = new Vector3(newSlot.GetComponent<RectTransform>().localPosition.x, newSlot.GetComponent<RectTransform>().position.y);

            float displayScale = 1.5f;

            drawnCard.transform.localScale = new Vector3(displayScale, displayScale, 1);

            drawnCard.GetComponent<RectTransform>().localRotation *= Quaternion.Euler(0, -1, 0);

            drawnCard.locked.gameObject.SetActive(true);

            //yield return new WaitForSeconds(0.2f);

            newSlot.card = drawnCard;

            drawnCard.gameObject.SetActive(true);

            yield return newSlot.RetrieveCard();

            hand.Add(drawnCard);

            hand = hand.OrderBy(o => o.transform.parent.GetSiblingIndex()).ToList();
        }

        if (autoUnlock)
            Locked = false;
    }

    public IEnumerator RemoveCard(Hand_Slot slot)
    {
        Locked = true;

        for (int r = 0; r < hand.Count; r++)
        {
            if(hand[r] == slot.card)
                hand[r].transform.SetParent(discardDeck_Pos);
            else
                hand[r].transform.SetParent(transform.parent);

            yield return null;
        }

        hand.Remove(slot.card);

        discardDeck.Add(slot.card);

        //Destroy(slot.card.gameObject); // Optional

        slot.card.GetComponent<Animation>().Play();

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
            yield return RemoveCard(hand_Pos.GetChild(i).GetComponent<Hand_Slot>());
            yield return null;
        }
    }

    public IEnumerator DiscardCards(int amount)
    {
        Locked = false;
        discarding = true;

        foreach (Card card in hand)
        {
            if (executedSlot != null && card == executedSlot.card)
                continue;

            card.Discardable(true);
        }

        int i = 0;

        while(i < amount)
        {
            character.TurnController.instructions.text = "Discard " + (amount - i) + (((amount - i) > 1) ? " Cards" : " Card");

            yield return new WaitUntil(() => cardToRemove != null);

            if (i == amount - 1)
            {
                foreach (Card card in hand)
                    card.Discardable(false);
            }

            yield return RemoveCard(cardToRemove);
            cardToRemove = null;
            i++;
        }

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
