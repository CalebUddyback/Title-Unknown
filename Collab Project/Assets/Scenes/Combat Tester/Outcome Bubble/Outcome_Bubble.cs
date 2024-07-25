using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Outcome_Bubble : MonoBehaviour
{
    public Canvas STRING_canvas;
    public Canvas INT_canvas;

    string numCo, stringCo;

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
        numCo = num.ToString();
        INT_canvas.gameObject.SetActive(true);
        StartCoroutine(NumPlaying(numCo));
    }

    public void Input(string str)
    {
        stringCo = str;
        STRING_canvas.gameObject.SetActive(true);
        StartCoroutine(StringPlaying(str));
    }

    public void Input(int num, string str)
    {
        numCo = num.ToString();
        stringCo = str;

        INT_canvas.gameObject.SetActive(true);
        STRING_canvas.gameObject.SetActive(true);

        StartCoroutine(NumPlaying(num.ToString()));
        StartCoroutine(StringPlaying(str));
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

        if(stringCo != null)
            STRING_canvas.transform.GetChild(0).GetComponent<Animation>().Play("Fade");


        yield return new WaitWhile(() => INT_canvas.transform.GetChild(0).GetComponent<Animation>().isPlaying);

        Destroy(gameObject);
    }

    IEnumerator StringPlaying(string str)
    {
        STRING_canvas.transform.GetChild(0).GetComponent<TextMeshPro>().text = str;
        STRING_canvas.transform.GetChild(0).gameObject.SetActive(true);

        if (numCo == null)
        {
            yield return new WaitForSeconds(0.12f);

            STRING_canvas.transform.GetChild(0).GetComponent<Animation>().Play();

            yield return new WaitWhile(() => STRING_canvas.transform.GetChild(0).GetComponent<Animation>().isPlaying);

            Destroy(gameObject);
        }
    }
}
