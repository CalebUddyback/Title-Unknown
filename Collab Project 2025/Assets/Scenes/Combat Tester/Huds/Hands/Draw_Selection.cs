using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Draw_Selection : MonoBehaviour
{
    public Turn_Controller turn_Controller;

    public Transform slots;

    public bool allowDuplicates = false;

    [HideInInspector]
    public Card chosenCard;

    public Button refreshButton;

    private readonly int refreshCost = 5;

    public void Awake()
    {
        gameObject.SetActive(false);
    }

    public IEnumerator ChooseCard()
    {
        gameObject.SetActive(true);

        GenerateCards();

        yield return new WaitUntil(() => chosenCard != null);

        gameObject.SetActive(false);

        chosenCard.card_Prefab.usableIMG.gameObject.SetActive(true);

        yield return turn_Controller.characterTurn.hand.GenerateCards(chosenCard, 1, true);

        chosenCard = null;

        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(slots.GetChild(i).GetComponent<Draw_Slot>().card.gameObject);
        }
    }

    private void OnEnable()
    {
        if (turn_Controller.characterTurn.Mana < refreshCost)
            refreshButton.interactable = false;
    }

    private void GenerateCards()
    {
        int[] x = new int[3] {-1,-1,-1};

        for (int i = 0; i < x.Length; i++)
        {
            int r;

            do
            {
                r = Random.Range(0, turn_Controller.characterTurn.Deck.Length);
            }
            while (x.Contains(r) && !allowDuplicates);

            x[i] = r;
        }

        for (int i = 0; i < x.Length; i++)
        {
            Card card = Instantiate(turn_Controller.characterTurn.Deck[x[i]], slots.GetChild(i)).GetComponent<Card>();

            card.card_Prefab = Instantiate(turn_Controller.characterTurn.hand.card_Prefab, card.transform).GetComponent<Card_Prefab>();

            card.card_Prefab.usableIMG.gameObject.SetActive(false);

            slots.GetChild(i).GetComponent<Draw_Slot>().card = card;
        }
    }

    public void Refresh()
    {

        turn_Controller.characterTurn.Mana -= refreshCost;

        if (turn_Controller.characterTurn.Mana < refreshCost)
            refreshButton.interactable = false;

        for (int i = 0; i < transform.childCount; i++)
        {
            Destroy(slots.GetChild(i).GetComponent<Draw_Slot>().card.gameObject);
        }

        GenerateCards();
    }
}
