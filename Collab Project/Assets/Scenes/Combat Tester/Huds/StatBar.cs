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

    public Transform displayBarTransform;
    private List<DisplayBar> displayBar = new List<DisplayBar>();
    int currentBar = 0;

    public Animation flash;

    public Coroutine followBarCO;

    public void Initialize(int current, int max)
    {
        this.max = max;

        foreach (Transform b in displayBarTransform)
            displayBar.Add(b.GetComponent<DisplayBar>());

        displayBar[0].frontBar.fillAmount = displayBar[0].backBar.fillAmount = current / (float)max;

        for (int i = 1; i < displayBar.Count; i++)
        {
            displayBar[i].frontBar.fillAmount = displayBar[i].backBar.fillAmount = 0;
        }

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
        int difference = current - former;

        Image lead, follow;

        if (difference <= 0)
        {
            lead = displayBar[currentBar].frontBar;
            follow = displayBar[currentBar].backBar;
            displayBar[currentBar].backBar.color = displayBar[currentBar].negChange;
        }
        else
        {
            lead = displayBar[currentBar].backBar;
            follow = displayBar[currentBar].frontBar;
            displayBar[currentBar].backBar.color = displayBar[currentBar].posChange;
        }

        flash.Play();

        if(former <= max && current <= max)
        {
            lead.fillAmount = current / (float)max;

            yield return new WaitForSeconds(1);
        }
        else if(former <= max && current > max)
        {
            currentText.color = displayBar[currentBar + 1].frontBar.color;

            lead.fillAmount = 1;
            displayBar[currentBar + 1].backBar.fillAmount = (current - max) / (float)max;
            displayBar[currentBar + 1].backBar.color = displayBar[currentBar + 1].posChange;

            yield return new WaitForSeconds(1);

            while (displayBar[currentBar].backBar.fillAmount > displayBar[currentBar].frontBar.fillAmount)
            {
                follow.fillAmount = Mathf.MoveTowards(follow.fillAmount, lead.fillAmount, 0.5f * Time.deltaTime);
                yield return null;
            }

            currentBar++;

            lead = displayBar[currentBar].backBar;
            follow = displayBar[currentBar].frontBar;
        }
        else if (former >= max && current > max)
        {
            lead.fillAmount = (current - max) / (float)max;

            yield return new WaitForSeconds(1);
        }
        else if (former > max && current <= max)
        {
            currentText.color = Color.white;

            lead.fillAmount = 0;
            displayBar[currentBar - 1].frontBar.fillAmount = current / (float)max;
            displayBar[currentBar - 1].backBar.color = displayBar[currentBar - 1].negChange;

            yield return new WaitForSeconds(1);

            while (displayBar[currentBar].backBar.fillAmount > displayBar[currentBar].frontBar.fillAmount)
            {
                follow.fillAmount = Mathf.MoveTowards(follow.fillAmount, lead.fillAmount, 0.5f * Time.deltaTime);
                yield return null;
            }

            currentBar--;

            lead = displayBar[currentBar].frontBar;
            follow = displayBar[currentBar].backBar;
        }

        while (displayBar[currentBar].backBar.fillAmount > displayBar[currentBar].frontBar.fillAmount)
        {
            follow.fillAmount = Mathf.MoveTowards(follow.fillAmount, lead.fillAmount, 0.5f * Time.deltaTime);
            yield return null;
        }

        follow.fillAmount = lead.fillAmount;

        followBarCO = null;
    }
}
