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
    public float timer_Progress = 0;

    public Image health_Main;
    public Image health_Back;
    public TextMeshProUGUI health_Text;

    public Image mana_Main;
    public Image mana_Back;
    public TextMeshProUGUI mana_Text;

    public void Start()
    {
        health_Main.fillAmount = 1;
        mana_Main.fillAmount = 1;
    }

    public void SetTimerColor(Color color)
    {
        timer_RadialFill.color = color;
    }

    public float GetTimeLeft()
    {
        return timer_Progress;
    }

    public void SetTimer(float reqTime, Color clr)
    {
        timer_RadialFill.color = clr;

        timer_RadialFill.fillAmount = 0;

        timer_reqTime = timer_Progress = reqTime;
    }

    public void IncrementTimer(float globalDelta)
    {
        //timer_Progress = Mathf.Floor((timer_Progress - globalDelta) * 1000f) / 1000f;

        timer_Progress -= globalDelta;

        timer_numbers.text = timer_Progress.ToString("F1");

        timer_RadialFill.fillAmount = Mathf.Floor(((timer_reqTime - timer_Progress) / timer_reqTime) * 100f) / 100f;
    }

    public void EndTimer()
    {
        timer_RadialFill.fillAmount = 1;

        timer_Progress = 0;

        timer_numbers.text = "";
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
            //yield return new WaitUntil(() => TurnController.TurnTime);

            //timer_Progress -= Time.deltaTime;

            //numbers.text = Mathf.CeilToInt(actual_Progress).ToString();
            timer_numbers.text = timer_Progress.ToString("F1");

            timer_RadialFill.fillAmount = Mathf.Floor(((timer_reqTime - timer_Progress) / timer_reqTime) * 100f) / 100f;

            //timer_RadialFill.fillAmount = (timer_reqTime - timer_Progress) / timer_reqTime;

            yield return null;
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

    Coroutine hpBackBarCO;

    public void AdjustHealth(int health, int amount)
    {
        if (hpBackBarCO != null)
            StopCoroutine(hpBackBarCO);

        hpBackBarCO = StartCoroutine(AdjustingHealth(health, amount));
    }

    IEnumerator AdjustingHealth(int health, int amount)
    {
        health_Text.text = health.ToString();

        if (amount < 0)
        {
            health_Back.color = new Color(0.65f, 0.2f, 0.2f);

            health_Main.fillAmount = health / 100f;

            health_Main.GetComponent<Animation>().Play();

            yield return new WaitForSeconds(1);

            while (health_Back.fillAmount > health_Main.fillAmount)
            {
                health_Back.fillAmount = Mathf.MoveTowards(health_Back.fillAmount, health_Main.fillAmount, 0.5f * Time.deltaTime);
                yield return null;
            }

            health_Back.fillAmount = health_Main.fillAmount;

        }
        else
        {
            health_Back.color = new Color(0.2f, 0.5f, 0.2f);

            health_Back.fillAmount = health / 100f;

            health_Main.GetComponent<Animation>().Play();

            yield return new WaitForSeconds(1);

            while (health_Back.fillAmount > health_Main.fillAmount)
            {
                health_Main.fillAmount = Mathf.MoveTowards(health_Main.fillAmount, health_Back.fillAmount, 0.5f * Time.deltaTime);
                yield return null;
            }

            health_Main.fillAmount = health_Back.fillAmount;
        }

        hpBackBarCO = null;
    }

    /***** MANA *****/

    Coroutine mpBackBarCO;

    public void AdjustMana(int mana, int amount)
    {
        if (mpBackBarCO != null)
            StopCoroutine(mpBackBarCO);

        mpBackBarCO = StartCoroutine(AdjustingMana(mana, amount));
    }

    IEnumerator AdjustingMana(int mana, int amount)
    {
        mana_Text.text = mana.ToString();

        if (amount < 0)
        {
            //mana_Back.color = new Color(0.7f, 0.85f, 1f);

            mana_Main.fillAmount = mana / 100f;

            mana_Main.GetComponent<Animation>().Play();

            yield return new WaitForSeconds(1);

            while (mana_Back.fillAmount > mana_Main.fillAmount)
            {
                mana_Back.fillAmount = Mathf.MoveTowards(mana_Back.fillAmount, mana_Main.fillAmount, 0.5f * Time.deltaTime);
                yield return null;
            }

            mana_Back.fillAmount = mana_Main.fillAmount;

        }
        else
        {
            //mana_Back.color = new Color(0.7f, 0.85f, 1f);

            mana_Back.fillAmount = mana / 100f;

            mana_Main.GetComponent<Animation>().Play();

            yield return new WaitForSeconds(1);

            while (mana_Back.fillAmount > mana_Main.fillAmount)
            {
                mana_Main.fillAmount = Mathf.MoveTowards(mana_Main.fillAmount, mana_Back.fillAmount, 0.5f * Time.deltaTime);
                yield return null;
            }

            mana_Main.fillAmount = mana_Back.fillAmount;
        }

        mpBackBarCO = null;
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
