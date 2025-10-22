using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice_Canvas : MonoBehaviour
{
    public GameObject dice_Prefab;

    public Transform allDice;

    public bool diceMatch = false;


    public void SetDice(int amount)
    {
        for (int i = 0; i < allDice.childCount; i++)
        {
            Destroy(allDice.GetChild(i).gameObject);
        }

        for (int i = 0; i < amount; i++)
        {
            Instantiate(dice_Prefab, allDice);
        }
    }


    public IEnumerator RollDice()
    {

        GetDiceTotal = 0;

        for (int i = 0; i < allDice.childCount; i++)
        {
            allDice.GetChild(i).GetComponent<Dice>().StartRolling();
        }

        for (int i = 0; i < allDice.childCount; i++)
        {
            yield return allDice.GetChild(i).GetComponent<Dice>().rolling;
            GetDiceTotal += allDice.GetChild(i).GetComponent<Dice>().GetResult;
        }


        if (allDice.childCount == 1)
            yield break;

        diceMatch = true;

        for (int i = 1; i < allDice.childCount; i++)
        {
            if (allDice.GetChild(0).GetComponent<Dice>().GetResult != allDice.GetChild(i).GetComponent<Dice>().GetResult)
            {
                diceMatch = false;
                break;
            }
        }

        if (diceMatch)
            print("MATCH");
    }

    public int GetDiceTotal { get; private set; } = 0;
}
