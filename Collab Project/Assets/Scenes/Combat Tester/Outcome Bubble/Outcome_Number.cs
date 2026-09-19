using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Outcome_Number : MonoBehaviour
{
    public Transform element;
    public Transform digits;

    private void Awake()
    {
        element.gameObject.SetActive(false);

        foreach (Transform child in digits)
        {
            child.gameObject.SetActive(false);
        }
    }

    public IEnumerator Display(Sprite spr, int n, Color clr)
    {
        element.GetChild(0).GetComponent<Image>().sprite = spr;

        string num = Mathf.Abs(n).ToString();

        foreach (Transform child in digits)
        {
            child.GetComponent<TextMeshProUGUI>().color = clr;
        }

        //gameObject.SetActive(true);

        if(element.GetChild(0).GetComponent<Image>().sprite != null)
            element.gameObject.SetActive(true);

        for (int i = 0; i < num.Length; i++)
        {
            digits.GetChild(i).GetComponent<TextMeshProUGUI>().text = num[i].ToString();
            digits.GetChild(i).gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(0.167f); // Grow length

        yield return new WaitForSeconds(0.5f);  // view window

        for (int i = 0; i < num.Length; i++)
        {
            digits.GetChild(i).GetComponent<Animation>().Play("Wiggle");
            yield return new WaitForSeconds(0.125f);
        }

        if (element.GetChild(0).GetComponent<Image>().sprite != null)
        {
            element.GetChild(0).GetComponent<Animation>().Play("Wiggle Sprite");
            yield return new WaitForSeconds(0.250f);
        }

        Destroy(gameObject);
    }
}
