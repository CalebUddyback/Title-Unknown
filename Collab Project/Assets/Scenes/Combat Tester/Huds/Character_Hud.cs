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

    [SerializeField]
    private Image timer_RadialFill;
    [SerializeField]
    private TextMeshProUGUI timer_numbers;
    public Transform skills;
    float timer_reqTime = 0;
    float timer_Progress = 0;

    public Image health_Green;
    public Image health_Red;
    public TextMeshProUGUI health_Text;

    public void SetTimerColor(Color color)
    {
        timer_RadialFill.color = color;
    }

    public float GetTimeLeft()
    {
        return timer_Progress;
    }

    public IEnumerator Timer(float reqTime, Color clr)
    {
        timer_RadialFill.color = clr;

        timer_RadialFill.fillAmount = 0;

        timer_reqTime = timer_Progress = reqTime;

        //numbers.text = Mathf.FloorToInt(actual_Progress).ToString();
        timer_numbers.text = timer_Progress.ToString("F1");

        while (0 <= timer_Progress)
        {
            yield return new WaitUntil(() => TurnController.TurnTime);

            timer_Progress -= Time.deltaTime;

            //numbers.text = Mathf.CeilToInt(actual_Progress).ToString();
            timer_numbers.text = timer_Progress.ToString("F1");

            timer_RadialFill.fillAmount = (timer_reqTime - timer_Progress) / timer_reqTime;
        }

        timer_RadialFill.fillAmount = 1;

        timer_Progress = 0;

        timer_numbers.text = "";
    }

    public void AffectProgress(float amount)
    {
        timer_Progress += amount;

        if (timer_Progress > timer_reqTime)
        {
            timer_reqTime = timer_Progress;
            timer_RadialFill.fillAmount = 0;
        }
        else
        {
            timer_RadialFill.fillAmount = (timer_reqTime - timer_Progress) / timer_reqTime;
        }

        timer_numbers.text = timer_Progress.ToString("F1");
    }

    /***** HEALTH *****/

    Coroutine redBarCO;

    public void AdjustHealth(int health)
    {
        if (redBarCO != null)
        {
            StopCoroutine(redBarCO);
        }

        redBarCO = StartCoroutine(Adjusting(health));
    }

    public IEnumerator Adjusting(int health)
    {
        health_Text.text = health.ToString();

        health_Green.fillAmount = health / 100f;

        health_Green.GetComponent<Animation>().Play();

        yield return new WaitForSeconds(1);

        //float timer = 0;
        //float startValue = health_Red.fillAmount;
        //
        //while (timer < 1)
        //{
        //    health_Red.fillAmount = Mathf.Lerp(startValue, health_Green.fillAmount, timer);
        //
        //    timer += Time.deltaTime;
        //
        //    yield return null;
        //}

        while(health_Red.fillAmount > health_Green.fillAmount)
        {
            health_Red.fillAmount = Mathf.MoveTowards(health_Red.fillAmount, health_Green.fillAmount, 0.5f * Time.deltaTime);
            yield return null;
        }

        health_Red.fillAmount = health_Green.fillAmount;

        redBarCO = null;
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
