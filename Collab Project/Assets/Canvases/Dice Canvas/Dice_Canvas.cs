using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Dice_Canvas : MonoBehaviour
{
    public GameObject dice_Prefab;

    public Transform allDice;

    private int numOfDice = 2;

    public int results = 0;

    public bool diceMatch = false;

    private void Start()
    {

    }

    private void OnEnable()
    {
        for (int i = 0; i < numOfDice; i++)
        {
            Instantiate(dice_Prefab, allDice);
        }

        allDice.gameObject.SetActive(true);

        RollDice();
    }

    public void RollDice()
    {
        List<float> rollSpeedList = new List<float>();

        foreach (Transform dice in allDice)
        {
            float rollSpeed = 0;

            do
            {
                rollSpeed = Random.Range(0.05f, 0.2f);
            }
            while (rollSpeedList.Contains(rollSpeed));

            dice.GetComponent<Dice>().rollSpeed = rollSpeed;
            rollSpeedList.Add(rollSpeed);

            dice.GetComponent<Dice>().StartRoll();
        }

        StartCoroutine(WaitForResults());
    }

    public IEnumerator WaitForResults()
    {

        for (int i = 0; i < allDice.childCount; i++)
        {
            yield return new WaitUntil(() => allDice.GetChild(i).GetComponent<Dice>().IsRolling == false);
            results += allDice.GetChild(i).GetComponent<Dice>().GetResult;
        }

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

        yield return new WaitForSeconds(1f);

        foreach (Transform child in allDice)
        {
            Destroy(child.gameObject);
        }

        allDice.gameObject.SetActive(false);

        GetResults = results;

        results = 0;

    }

    public int GetResults { get; private set; } = 0;

    public void ResetResults() => GetResults = 0;
}
