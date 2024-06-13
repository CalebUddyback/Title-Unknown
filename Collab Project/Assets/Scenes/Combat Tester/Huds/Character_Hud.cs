using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Character_Hud : MonoBehaviour
{
    [HideInInspector]
    public Turn_Controller TurnController;

    [HideInInspector]
    public Text diplayName;

    [SerializeField]
    private Image radialFill;

    [SerializeField]
    private Text numbers;

    public Transform skills;

    float actual_Progress = 0;

    public void SetTimerColor(Color color)
    {
        radialFill.color = color;
    }

    public IEnumerator Timer(float reqTime, Color col)
    {
        radialFill.color = col;

        radialFill.fillAmount = 0;

        actual_Progress = reqTime;

        //numbers.text = Mathf.FloorToInt(actual_Progress).ToString();
        numbers.text = actual_Progress.ToString("F1");

        while (0 <= actual_Progress)
        {
            yield return new WaitUntil(() => TurnController.TurnTime);

            actual_Progress -= Time.deltaTime;

            //numbers.text = Mathf.CeilToInt(actual_Progress).ToString();
            numbers.text = actual_Progress.ToString("F1");

            radialFill.fillAmount = (reqTime - actual_Progress) / reqTime;
        }

        radialFill.fillAmount = 1;

        actual_Progress = 0;

        numbers.text = "";
    }

    public void EffectProgress(float amount)
    {
        actual_Progress += amount;
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
