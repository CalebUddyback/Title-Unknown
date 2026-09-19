using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Outcome_Bubble : MonoBehaviour
{

    public Transform container;
    public Outcome_String outcome_String;
    public Outcome_Number outcome_Number;

    public TMP_ColorGradient heal_Color;

    public List<Coroutine> coroutine = new List<Coroutine>();

    public void Input(Sprite spr, int num, Color clr)
    {
        Outcome_Number outcome =  Instantiate(outcome_Number.gameObject, container).GetComponent<Outcome_Number>();

        coroutine.Add(StartCoroutine(outcome.Display(spr, num, clr)));
    }

    public void Input(string str, Color clr)
    {
        Outcome_String outcome = Instantiate(outcome_String.gameObject, container).GetComponent<Outcome_String>();

        coroutine.Add(StartCoroutine(outcome.Display(str, clr)));
    }

    private void Start()
    {
        StartCoroutine(Delete());
    }

    IEnumerator Delete()
    {
        yield return null;

        for (int i = 0; i < coroutine.Count; i++)
        {
            yield return coroutine[i];
        }

        Destroy(gameObject);
    }
}
