using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatBar : MonoBehaviour
{
    [HideInInspector]
    public TextMeshProUGUI currentText;
    [HideInInspector]
    public TextMeshProUGUI maxText;
    private int max;
    public float overflowMax;

    public Transform displayBarTransform;
    private List<DisplayBar> displayBar = new List<DisplayBar>();

    public float chaseSpeed = 0.75f;

    public Animation flash;

    public Coroutine followBarCO;

    public void Initialize(int current, int max)
    {
        this.max = max;

        foreach (Transform b in displayBarTransform)
            displayBar.Add(b.GetComponent<DisplayBar>());

        overflowMax = max;

        displayBar[0].frontBar.fillAmount = displayBar[0].backBar.fillAmount = current / (float)max;

        for (int i = 1; i < displayBar.Count; i++)
            displayBar[i].frontBar.fillAmount = displayBar[i].backBar.fillAmount = 0;

        maxText.text = max.ToString();
        currentText.text = current.ToString();
    }

    public void Adjust(int former, int current)
    {
        currentText.text = current.ToString();

        if (followBarCO != null)
            StopCoroutine(followBarCO);

        followBarCO = StartCoroutine(Adjusting(former, current));
    }

    IEnumerator Adjusting(int former, int current)
    {
        Image lead, follow;

        int currentBar = Mathf.Clamp(Mathf.CeilToInt(current / (float)max) - 1, 0, displayBar.Count - 1); 
        int formerBar = Mathf.Clamp(Mathf.CeilToInt(former / (float)max) - 1, 0, displayBar.Count - 1);

        flash.Play();

        if (current < former)
        {
            for (int i = formerBar; i >= currentBar; i--)
            {
                lead = displayBar[i].frontBar;
                displayBar[i].backBar.color = displayBar[i].negChange;

                if (current <= max * (displayBar.Count - 1))
                {
                    lead.fillAmount = (current - (max * i)) / (float)max;
                    overflowMax = max;
                }
                else
                    lead.fillAmount = (current - (max * i)) / overflowMax;

                yield return null;
            }
        }
        else
        {
            for (int i = formerBar; i <= currentBar; i++)
            {
                lead = displayBar[i].backBar;
                displayBar[i].backBar.color = displayBar[i].posChange;

                if (current <= max * (displayBar.Count - 1))
                    lead.fillAmount = (current - (max * i)) / (float)max;
                else
                {
                    if (current - (max * (displayBar.Count - 1)) > overflowMax)
                    {
                        overflowMax = current - (max * (displayBar.Count - 1));
                        lead.fillAmount = 1;
                    }
                    else
                        lead.fillAmount = (current - (max * i)) / overflowMax;
                }

                yield return null;
            }
        }

        yield return new WaitForSeconds(1);

        if (current < former)
        {
            for (int i = formerBar; i >= currentBar; i--)
            {
                lead = displayBar[i].frontBar;
                follow = displayBar[i].backBar;
                displayBar[i].backBar.color = displayBar[i].negChange;

                currentText.color = (i > 0) ? displayBar[i].frontBar.color : Color.white;

                while (displayBar[i].backBar.fillAmount > displayBar[i].frontBar.fillAmount)
                {
                    follow.fillAmount = Mathf.MoveTowards(follow.fillAmount, lead.fillAmount, chaseSpeed * Time.deltaTime);
                    yield return null;
                }

                follow.fillAmount = lead.fillAmount;

                yield return null;
            }
        }
        else
        {
            for (int i = formerBar; i <= currentBar; i++)
            {
                lead = displayBar[i].backBar;
                follow = displayBar[i].frontBar;
                displayBar[i].backBar.color = displayBar[i].posChange;

                currentText.color = (i > 0) ? displayBar[i].frontBar.color : Color.white;

                while (displayBar[i].backBar.fillAmount > displayBar[i].frontBar.fillAmount)
                {
                    follow.fillAmount = Mathf.MoveTowards(follow.fillAmount, lead.fillAmount, chaseSpeed * Time.deltaTime);
                    yield return null;
                }

                follow.fillAmount = lead.fillAmount;

                yield return null;
            }
        }

        followBarCO = null;
    }

    public void ClearOverflow()
    {
        for (int i = 1; i < displayBar.Count; i++)
        {
            displayBar[i].frontBar.fillAmount = 0;
            displayBar[i].backBar.fillAmount = 0;
        }

        currentText.text = max.ToString();

        currentText.color = Color.white;

        overflowMax = max;
    }
}
