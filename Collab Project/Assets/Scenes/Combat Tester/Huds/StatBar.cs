using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class StatBar : MonoBehaviour
{

    public TextMeshProUGUI currentText;
    public TextMeshProUGUI maxText;
    public Image backBar;
    public Image mainBar;
    public Animation flash;

    public Color negChange = new Color(0.65f, 0.2f, 0.2f);
    public Color posChange = new Color(0.2f, 0.5f, 0.2f);

    Coroutine backBarCO;

    public void Initialize(int current, float max)
    {
        mainBar.fillAmount = backBar.fillAmount = current / max;

        maxText.text = max.ToString();
        currentText.text = current.ToString();
    }

    public void Adjust(int former, int currrent)
    {
        if (backBarCO != null)
            StopCoroutine(backBarCO);

        backBarCO = StartCoroutine(Adjusting(former, currrent));
    }

    IEnumerator Adjusting(int former, int current)
    {
        currentText.text = current.ToString();

        int difference = current - former;

        Image lead, follow;

        if (difference < 0)
        {
            lead = mainBar;
            follow = backBar;
            backBar.color = negChange;
        }
        else
        {
            lead = mainBar;
            follow = backBar;
            backBar.color = posChange;
        }

        lead.fillAmount = current / float.Parse(maxText.text);

        flash.Play();

        yield return new WaitForSeconds(1);

        while (backBar.fillAmount > mainBar.fillAmount)
        {
            follow.fillAmount = Mathf.MoveTowards(follow.fillAmount, lead.fillAmount, 0.5f * Time.deltaTime);
            yield return null;
        }

        follow.fillAmount = lead.fillAmount;

        backBarCO = null;
    }

}
