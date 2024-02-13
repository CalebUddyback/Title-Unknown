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

        foreach(Transform slot in itemContainer)
        {
            slot.GetComponent<Button>().onClick.AddListener(() =>  chosenItemIndex = slot.transform.GetSiblingIndex());
        }
    }

    public IEnumerator ChooseItem(Character character)
    {
        List<Item> inventory = character.inventory;

        chosenItemIndex = -2;


        for (int i = 0; i < itemContainer.childCount; i++)
        {
            if (i < inventory.Count)
            {
                if (inventory[i].usable)
                    itemContainer.GetChild(i).GetComponent<Button>().interactable = true;

                itemContainer.GetChild(i).GetComponent<Image>().sprite = inventory[i].icon;
            }
            else
            {
                itemContainer.GetChild(i).GetComponent<Button>().interactable = false;
                itemContainer.GetChild(i).GetComponent<Image>().sprite = emptySlotSprite;
            }
        }     

        yield return new WaitUntil(() => chosenItemIndex != -2);        

        yield return chosenItemIndex;
    }
}
