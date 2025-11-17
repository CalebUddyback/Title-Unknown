using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Draw_Selection : MonoBehaviour
{
    public Turn_Controller turn_Controller;

    public Transform slots;

    private Draw_Slot selectedSlot;
    public Draw_Slot SelectedSlot
    {
        get
        {
            return selectedSlot;
        }
        set
        {
            if(selectedSlot != null)
                selectedSlot.GetComponent<Image>().color = Color.grey;

            selectedSlot = value;

            if(value != null)
            {
                selectedSlot.GetComponent<Image>().color = Color.yellow;
                drawButton.interactable = true;
            }
            else
            {
                drawButton.interactable = false;
            }
        }
    }

    [HideInInspector]
    public Draw_Slot chosenSlot;

    public Button drawButton, refreshButton;

    private readonly int refreshCost = -5;

    public void Awake()
    {
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        if (turn_Controller.characterTurn.Mana() < refreshCost)
            refreshButton.interactable = false;
        else
            refreshButton.interactable = true;
    }

    public IEnumerator ChooseCard()
    {
        gameObject.SetActive(true);

        yield return GenerateCards();

        yield return new WaitUntil(() => chosenSlot != null);

        gameObject.SetActive(false);

        chosenSlot.GetComponent<Image>().color = Color.grey;

        chosenSlot.card.usableIMG.gameObject.SetActive(true);

        Hand_Slot slot = turn_Controller.characterTurn.decks.CreateSlot();

        slot.card = chosenSlot.card;

        chosenSlot = null;

        yield return null;

        yield return turn_Controller.characterTurn.decks.ShiftCards();

        turn_Controller.characterTurn.decks.drawDeckQuantity.text = turn_Controller.characterTurn.decks.drawDeck.childCount.ToString();

        yield return slot.RetrieveCard();

        turn_Controller.characterTurn.decks.hand.Add(slot.card);

        turn_Controller.characterTurn.decks.hand = turn_Controller.characterTurn.decks.hand.OrderBy(o => o.transform.parent.GetSiblingIndex()).ToList();

        for (int i = 0; i < transform.childCount; i++)
        {
            if (slots.GetChild(i).GetComponent<Draw_Slot>().card == slot.card)
                continue;

            slots.GetChild(i).GetComponent<Draw_Slot>().card.gameObject.SetActive(false);

            slots.GetChild(i).GetComponent<Draw_Slot>().card.transform.SetParent(turn_Controller.characterTurn.decks.drawDeck);

            slots.GetChild(i).GetComponent<Draw_Slot>().card.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }
    }

    private IEnumerator GenerateCards()
    {
        // Random Draw from Entire Deck

        //int[] x = new int[3] {-1,-1,-1};
        //
        //for (int i = 0; i < x.Length; i++)
        //{
        //    int r;
        //
        //    do
        //    {
        //        r = Random.Range(0, turn_Controller.characterTurn.hand.drawDeck.Count);
        //    }
        //    while (x.Contains(r));
        //
        //    x[i] = r;
        //}

        // 3 draw From top deck

        for (int i = 0; i < transform.childCount; i++)
        {
            Card card = turn_Controller.characterTurn.decks.drawDeck.GetChild(0).GetComponent<Card>();

            slots.GetChild(i).GetComponent<Draw_Slot>().card = card;

            card.transform.SetParent(slots.GetChild(i));

            card.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;

            card.transform.SetSiblingIndex(slots.GetChild(i).childCount - 2);

            card.usableIMG.gameObject.SetActive(false);

            card.gameObject.SetActive(true);

            slots.GetChild(i).GetComponent<Draw_Slot>().animator.Play();

            yield return new WaitForSeconds(0.25f);
        }
    }

    public void TakeCard()
    {
        chosenSlot = selectedSlot;

        selectedSlot = null;
    }

    public void Refresh()
    {

        turn_Controller.characterTurn.AdjustMana(refreshCost, false);

        if (turn_Controller.characterTurn.Mana() < refreshCost)
            refreshButton.interactable = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            slots.GetChild(i).GetComponent<Draw_Slot>().card.gameObject.SetActive(false);

            slots.GetChild(i).GetComponent<Draw_Slot>().card.transform.SetParent(turn_Controller.characterTurn.decks.drawDeck);

            slots.GetChild(i).GetComponent<Draw_Slot>().card.GetComponent<RectTransform>().anchoredPosition = Vector2.zero;
        }

        SelectedSlot = null;

        StartCoroutine(GenerateCards());
    }
}
