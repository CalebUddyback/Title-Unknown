using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Outcome_Bubble : MonoBehaviour
{
    public Canvas STRING_canvas;
    public Canvas INT_canvas;

    string numText, stringText;

    public TMP_ColorGradient heal_Color;

    private void Awake()
    {
        foreach (Transform child in INT_canvas.transform)
            child.gameObject.SetActive(false);

        INT_canvas.gameObject.SetActive(false);

        foreach (Transform child in STRING_canvas.transform)
            child.gameObject.SetActive(false);

        STRING_canvas.gameObject.SetActive(false);
    }

    private void Start()
    {
        // For Testing

        //Input(100);
    }

    public void Input(int num)
    {
        //numText = (num <= 0) ? (num * -1).ToString() : "+" + num.ToString();

        if(num <= 0)
        {
            numText = (num * -1).ToString();
        }
        else
        {
            numText = "+" + num.ToString();

            foreach (Transform digit in INT_canvas.transform)
            {
                digit.GetComponent<TextMeshPro>().colorGradientPreset = heal_Color;
            }
        }

        INT_canvas.gameObject.SetActive(true);
        StartCoroutine(NumPlaying(numText));
    }

    public void Input(int num, Color clr)
    {

        numText = (num <= 0) ? (num * -1).ToString() : "+" + num.ToString();


        foreach(Transform digit in INT_canvas.transform)
        {
            digit.GetComponent<TextMeshPro>().color = clr;
        }

        INT_canvas.gameObject.SetActive(true);
        StartCoroutine(NumPlaying(numText));
    }

    public void Input(string str)
    {
        stringText = str;
        STRING_canvas.gameObject.SetActive(true);
        StartCoroutine(StringPlaying(str));
    }

    public void Input(int num, string str)
    {

        numText = (num <= 0)  ? (num * -1).ToString() : "+" + num.ToString();

        stringText = str;

        INT_canvas.gameObject.SetActive(true);
        STRING_canvas.gameObject.SetActive(true);

        StartCoroutine(NumPlaying(numText));
        StartCoroutine(StringPlaying(stringText));
    }

    public void Input(int num, string str, Color clr)
    {

        numText = (num <= 0) ? (num * -1).ToString() : "+" + num.ToString();

        STRING_canvas.transform.GetChild(0).GetComponent<TextMeshPro>().color = clr;

        foreach (Transform digit in INT_canvas.transform)
        {
            digit.GetComponent<TextMeshPro>().color = clr;
        }


        stringText = str;

        INT_canvas.gameObject.SetActive(true);
        STRING_canvas.gameObject.SetActive(true);

        StartCoroutine(NumPlaying(numText));
        StartCoroutine(StringPlaying(stringText));
    }

    IEnumerator NumPlaying(string num)
    {
        for (int i = 0; i < num.Length; i++)
        {
            INT_canvas.transform.GetChild(i).GetComponent<TextMeshPro>().text = num[i].ToString();
            INT_canvas.transform.GetChild(i).GetComponent<RectTransform>().localScale = Vector3.zero;
            INT_canvas.transform.GetChild(i).gameObject.SetActive(true);
        }

        for (int i = 0; i < num.Length; i++)
        {
            INT_canvas.transform.GetChild(i).GetComponent<RectTransform>().localScale = Vector3.one;
            INT_canvas.transform.GetChild(i).GetComponent<Animation>().Play("Wiggle");

            yield return new WaitForSeconds(0.04f);
        }

        yield return new WaitForSeconds(0.09f);

        yield return new WaitWhile(() => INT_canvas.transform.GetChild(num.Length-1).GetComponent<Animation>().isPlaying);


        for (int i = 0; i < num.Length; i++)
        {
            INT_canvas.transform.GetChild(i).GetComponent<Animation>().Play("Fade");
        }

        if(stringText != null)
            STRING_canvas.transform.GetChild(0).GetComponent<Animation>().Play("Fade");


        yield return new WaitWhile(() => INT_canvas.transform.GetChild(0).GetComponent<Animation>().isPlaying);

        Destroy(gameObject);
    }

    IEnumerator StringPlaying(string str)
    {
        STRING_canvas.transform.GetChild(0).GetComponent<TextMeshPro>().text = str;
        STRING_canvas.transform.GetChild(0).gameObject.SetActive(true);

        if (numText == null)
        {
            yield return new WaitForSeconds(0.12f);

            STRING_canvas.transform.GetChild(0).GetComponent<Animation>().Play();

            yield return new WaitWhile(() => STRING_canvas.transform.GetChild(0).GetComponent<Animation>().isPlaying);

            Destroy(gameObject);
        }
    }
}
