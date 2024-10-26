using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Character_Hud : MonoBehaviour
{
    [HideInInspector]
    public Turn_Controller TurnController;

    public TextMeshProUGUI diplayName;

    public TextMeshProUGUI timer_Numbers;
    public int timer_Progress = 0;
    private Color timer_BaseColor = new Color(1,1,1,0.5f);
    public Animation timer_Pulser;
    public GameObject chargeIndicator;

    public StatBar healthBar, manaBar;

    public Transform skills;

    /***** TIMER *****/

    public int GetTimeLeft()
    {
        return timer_Progress;
    }

    public void SetTimerColor(Color clr)
    {
        timer_Numbers.color = clr;
    }

    public IEnumerator FadeColorToBase()
    {
        Color startColor = timer_Numbers.color;
        Color targetColor = timer_BaseColor;

        float timer = 0;
        float maxTime = 0.3f;

        while (timer < maxTime)
        {
            timer_Numbers.color = Color.Lerp(startColor, targetColor, timer / maxTime);

            timer += Time.deltaTime;

            yield return null;
        }

        timer_Numbers.color = targetColor;
    }

    public IEnumerator AffectTimerProgress(int amount)
    {
        if (amount < 0)
            SetTimerColor(Color.green);
        else
            SetTimerColor(Color.red);

        yield return ScrollTimerTo(timer_Progress + amount);
    }

    public IEnumerator ScrollTimerTo(int changeAmount)
    {
        Vector3 startPos = timer_Numbers.rectTransform.anchoredPosition;

        Vector3 targetPos = new Vector3(0, -1 * timer_Numbers.fontSize * (changeAmount));
        
        float timer = 0;
        float maxTime = 0.3f;

        while (timer < maxTime)
        {
            timer_Numbers.rectTransform.anchoredPosition = Vector3.Lerp(startPos, targetPos, timer / maxTime);
        
            timer += Time.deltaTime;
        
            yield return null;
        }
        
        timer_Numbers.rectTransform.anchoredPosition = targetPos;
        
        timer_Progress = changeAmount;

        yield return new WaitForSeconds(0.3f);

        StartCoroutine(FadeColorToBase());
    }

    public void SkillSlot(int slot, Sprite image)
    {
        skills.GetChild(slot).GetChild(0).GetComponent<Image>().sprite = image;
    }


    public Sprite emptySlot;

    public void ClearSkillSlot(int slot)
    {
        skills.GetChild(slot).GetChild(0).GetComponent<Image>().sprite = emptySlot;
    }
}
