using System.Collections;
using UnityEngine;
using TMPro;

public class Outcome_Bubble : MonoBehaviour
{
    public Canvas STRING_canvas;
    public Canvas INT_canvas;

    string numText, stringText;

    public TMP_ColorGradient heal_Color;

    public Coroutine coroutine;

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
        Input(num, "", Color.white);
    }

    public void Input(int num, Color clr)
    {
        Input(num, "", clr);
    }

    public void Input(string str)
    {
        Input(0, str, Color.white);
    }

    public void Input(string str, Color clr)
    {
        Input(0, str, clr);
    }

    public void Input(int num, string str, Color clr)
    {
        num = Mathf.Clamp(num, -999, 999);

        if (num > 0 && clr == Color.white)
        {
            foreach (Transform digit in INT_canvas.transform)
            {
                digit.GetComponent<TextMeshProUGUI>().colorGradientPreset = heal_Color;
            }

            STRING_canvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().colorGradientPreset = heal_Color;
        }
        else
        {
            foreach (Transform digit in INT_canvas.transform)
            {
                digit.GetComponent<TextMeshProUGUI>().color = clr;
            }

            STRING_canvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = clr;
        }

        numText = Mathf.Abs(num).ToString();

        stringText = str;

        coroutine = StartCoroutine(Playing(numText, str));
    }

    IEnumerator Playing(string num, string str)
    {
        if (num != "0" && str != "" || str == "")
        {
            INT_canvas.gameObject.SetActive(true);

            for (int i = 0; i < num.Length; i++)
            {
                INT_canvas.transform.GetChild(i).GetComponent<TextMeshProUGUI>().text = num[i].ToString();
                INT_canvas.transform.GetChild(i).gameObject.SetActive(true);
            }
        }

        if (str != "")
        {
            STRING_canvas.gameObject.SetActive(true);

            STRING_canvas.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = str;
            STRING_canvas.transform.GetChild(0).gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(0.167f); // Grow length

        yield return new WaitForSeconds(0.5f);

        if (num != "0" && str != "" || str == "")
        {
            for (int i = 0; i < num.Length; i++)
            {
                INT_canvas.transform.GetChild(i).GetComponent<Animation>().Play("Wiggle");
                yield return new WaitForSeconds(0.125f);
            }
            yield return new WaitWhile(() => INT_canvas.transform.GetChild(num.Length - 1).GetComponent<Animation>().isPlaying);
        }

        if (str != "")
        {
            STRING_canvas.transform.GetChild(0).GetComponent<Animation>().Play("Fade");
            yield return new WaitWhile(() => STRING_canvas.transform.GetChild(0).GetComponent<Animation>().isPlaying);
        }

        Destroy(gameObject);
    }
}
