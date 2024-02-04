using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Action_Canvas : MonoBehaviour
{
    public Button nodeButton, itemsButton, mapButton, restButton;
    public Text nodeButtonTxt;

    private Button chosenAction = null;

    private void Awake()
    {
        nodeButton.onClick.AddListener(() => chosenAction = nodeButton);
        restButton.onClick.AddListener(() => chosenAction = restButton);
        itemsButton.onClick.AddListener(() => chosenAction = itemsButton);
    }

    public IEnumerator ChooseAction(Character character, Node_Effect nodeEffect)
    {
        chosenAction = null;

        nodeButton.interactable = false;

        if (nodeEffect == null)
        {
            nodeButtonTxt.text = "No Space Action";
        }
        else
        {
            nodeButton.interactable = true;

            if (!nodeEffect.isSearched)
                nodeButtonTxt.text = "Search";
            else
                nodeButtonTxt.text = nodeEffect.ActionName;
        }

        yield return new WaitUntil(() => chosenAction != null);

        nodeButton.interactable = false;

        if (chosenAction == nodeButton)
            yield return "node";
        else if (chosenAction == itemsButton)
            yield return "item";
        else if (chosenAction == restButton)
            yield return "rest";
        else
            yield return null;
    }
}
