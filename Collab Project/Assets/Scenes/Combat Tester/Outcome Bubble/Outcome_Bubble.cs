using System.Collections;
using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class Outcome_Bubble : MonoBehaviour
{

    public Transform container;
    public Outcome_String outcome_String;
    public Outcome_Number outcome_Number;
    Vector3 pos;
    Main_Camera mainCamera;

    public TMP_ColorGradient heal_Color;

    public List<Coroutine> coroutine = new List<Coroutine>();

    public void Input(Sprite spr, int num, string type, Color clr, Main_Camera mainCamera, Transform pos)
    {
        Outcome_Number outcome =  Instantiate(outcome_Number.gameObject, container).GetComponent<Outcome_Number>();

        this.mainCamera = mainCamera;
        this.pos = pos.position;

        coroutine.Add(StartCoroutine(outcome.Display(spr, num, type, clr)));
    }

    public void Input(string str, Color clr, Main_Camera mainCamera, Transform pos)
    {
        Outcome_String outcome = Instantiate(outcome_String.gameObject, container).GetComponent<Outcome_String>();

        this.pos = pos.position;
        this.mainCamera = mainCamera;

        coroutine.Add(StartCoroutine(outcome.Display(str, clr)));
    }

    private void Start()
    {
        StartCoroutine(Delete());
    }

    private void LateUpdate()
    {
        GetComponent<RectTransform>().anchoredPosition = mainCamera.UIPosition(pos);
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
