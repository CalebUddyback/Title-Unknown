using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn_Timeline : MonoBehaviour
{
    public Transform content;

    private float shiftDuration = 0.35f;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            StartCoroutine(Shift());
        }
    }

    public IEnumerator Shift()
    {
        Transform swap = content.GetChild(content.childCount - 1).GetChild(0);

        swap.SetParent(content.GetChild(0));
        yield return null;
        swap.GetComponent<RectTransform>().offsetMin = Vector2.zero;
        swap.GetComponent<RectTransform>().offsetMax = Vector2.zero;

        for (int i = 0; i < content.childCount - 1; i++)
        {
            if (content.GetChild(i).childCount < 1)
                continue;

            content.GetChild(i).GetChild(0).SetParent(content.GetChild(i + 1));
        }

        yield return null;

        for (int i = 0; i < content.childCount; i++)
        {
            if (content.GetChild(i).childCount < 1)
                continue;

            StartCoroutine(Shifting(content.GetChild(i).GetChild(0).GetComponent<RectTransform>()));
        }

        yield return new WaitForSeconds(shiftDuration);

        StartCoroutine(TurnIndicate(content.GetChild(content.childCount-1).GetChild(0).GetComponent<RectTransform>()));
    }

    public IEnumerator Shifting(RectTransform r)
    {
        Vector2 startOffsetMin = r.offsetMin;
        Vector2 startOffsetMax = r.offsetMax;

        Vector2 targetOffsetMin = new Vector2(startOffsetMin.x, 0);
        Vector2 targetOffsetMax = new Vector2(startOffsetMax.x, 0);

        float lerp = 0.0f;

        float smoothLerp = 0.0f;

        float elapsedTime = 0.0f;

        while (elapsedTime < shiftDuration)
        {

            lerp = Mathf.Lerp(0.0f, 1.0f, elapsedTime / shiftDuration);
            smoothLerp = Mathf.SmoothStep(0.0f, 1.0f, lerp);
            r.offsetMin = Vector3.Lerp(startOffsetMin, targetOffsetMin, smoothLerp);
            r.offsetMax = Vector3.Lerp(startOffsetMax, targetOffsetMax, smoothLerp);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        r.offsetMin = targetOffsetMin;
        r.offsetMax = targetOffsetMax;
    }

    public IEnumerator TurnIndicate(RectTransform r)
    {
        Vector2 startOffsetMin = r.offsetMin;
        Vector2 startOffsetMax = r.offsetMax;

        Vector2 targetOffsetMin = new Vector2(0, startOffsetMin.y);
        Vector2 targetOffsetMax = new Vector2(0, startOffsetMax.y);

        float lerp = 0.0f;

        float smoothLerp = 0.0f;

        float duration = 0.2f;

        float elapsedTime = 0.0f;

        while (elapsedTime < duration)
        {

            lerp = Mathf.Lerp(0.0f, 1.0f, elapsedTime / duration);
            smoothLerp = Mathf.SmoothStep(0.0f, 1.0f, lerp);
            r.offsetMin = Vector3.Lerp(startOffsetMin, targetOffsetMin, smoothLerp);
            r.offsetMax = Vector3.Lerp(startOffsetMax, targetOffsetMax, smoothLerp);
            elapsedTime += Time.deltaTime;

            yield return null;
        }

        r.offsetMin = targetOffsetMin;
        r.offsetMax = targetOffsetMax;
    }
}
