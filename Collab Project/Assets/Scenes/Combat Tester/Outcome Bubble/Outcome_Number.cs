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
        string num = Mathf.Abs(n).ToString();

        if(spr != null)
        {
            element.GetChild(0).GetComponent<Image>().sprite = spr;
            element.gameObject.SetActive(true);
        }

        float size = digits.GetComponent<GridLayoutGroup>().cellSize.x * num.Length;
        digits.GetComponent<RectTransform>().SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size);

        for (int i = 0; i < num.Length; i++)
        {
            digits.GetChild(i).GetComponent<TextMeshProUGUI>().color = clr;
            digits.GetChild(i).GetComponent<TextMeshProUGUI>().text = num[i].ToString();
            digits.GetChild(i).gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(0.167f); // Grow length

        yield return new WaitForSeconds(0.5f);  // view window

        if (spr != null)
        {
            element.GetChild(0).GetComponent<Animation>().Play("Wiggle Sprite");
            yield return new WaitForSeconds(0.125f);
        }

        for (int i = 0; i < num.Length; i++)
        {
            digits.GetChild(i).GetComponent<Animation>().Play("Wiggle");
            yield return new WaitForSeconds(0.125f);
        }

        yield return new WaitForSeconds(0.125f);

        Destroy(gameObject);
    }
}
