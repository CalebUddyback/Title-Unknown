using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Combo_Counter : MonoBehaviour
{
    private TextMeshProUGUI num => GetComponent<TextMeshProUGUI>();
    public TextMeshProUGUI text;

    [SerializeField]
    private int comboCount = 0;

    private void Awake()
    {
        gameObject.SetActive(false);
    }

    public void SetComboCount()
    {
        comboCount++;

        if(comboCount > 1)
            gameObject.SetActive(true);

        num.text = comboCount.ToString();
    }

    public void ResetComboCount()
    {
        gameObject.SetActive(false);
        comboCount = 0;
    } 
}
