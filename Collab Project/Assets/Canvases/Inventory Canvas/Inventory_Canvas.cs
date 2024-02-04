using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory_Canvas : MonoBehaviour
{
    public Button backButton;

    public Transform itemContainer;

    public Sprite emptySlotSprite;

    private int chosenItemIndex = -2;       // -2 waiting integer;

    private void Awake()
    {
        backButton.onClick.AddListener(() => chosenItemIndex = -1);
    }

    public IEnumerator ChooseItem(Character character)
    {
        List<Item> inventory = character.inventory;

        chosenItemIndex = -2;

        for (int i = 0; i < inventory.Count; i++)
        {
            itemContainer.GetChild(i).GetComponent<Button>().interactable = true;
            itemContainer.GetChild(i).GetComponent<Button>().onClick.AddListener(() => chosenItemIndex = i-1);
            itemContainer.GetChild(i).GetComponent<Image>().sprite = inventory[i].icon;
        }     

        yield return new WaitUntil(() => chosenItemIndex != -2);

        if (chosenItemIndex > -1)
        {
            itemContainer.GetChild(chosenItemIndex).GetComponent<Button>().interactable = false;
            itemContainer.GetChild(chosenItemIndex).GetComponent<Button>().onClick.RemoveAllListeners();
            itemContainer.GetChild(chosenItemIndex).GetComponent<Image>().sprite = emptySlotSprite;
        }

        yield return chosenItemIndex;
    }
}
