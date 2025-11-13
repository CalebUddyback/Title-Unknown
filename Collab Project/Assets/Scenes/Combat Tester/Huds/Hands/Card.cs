using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Card: MonoBehaviour
{
    public TextMeshProUGUI cardText, manaCost;

    public Image locked;

    public Image usableIMG;

    public GameObject discardLines;

    public Transform effects;

    public Skill skill;

    public Decks hand;

    public bool negated = false;

    private bool usable;
    public bool Usable
    {
        get
        {
            return usable;
        }
        set
        {
            usable = value;
            usableIMG.color = usable ?  Color.green : Color.red;
        }
    }

    private void Awake()
    {
        //gameObject.SetActive(false);
    }

    public void Start()
    {
        cardText.text = skill.displayName;
        manaCost.text = Mathf.Abs(skill.manaCost).ToString();

        if (skill.DamageVariation.y > 0)
        {
            int dam1 = skill.Character.GetCurrentStats()[Character_Stats.Stat.STR] + skill.DamageVariation.x;
            int dam2 = skill.Character.GetCurrentStats()[Character_Stats.Stat.STR] + skill.DamageVariation.y;

            GameObject effect = Instantiate(effects.GetChild(0).gameObject, effects);

            effect.gameObject.SetActive(true);

            //effect.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Deal " + "<color=red>" + "<b>" + dam1 + " - " + dam2 + "</b>" + "</color>" + " Damage";

            if (skill.DamageVariation.x == skill.DamageVariation.y)
                effect.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Deal: " + "<b>" + dam2 + "</b>" + " damage";
            else
                effect.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Deal: " + "<b>" + dam1 + "-" + dam2 + "</b>" + " damage";
        }

        if (skill.discard)
        {
            GameObject effect = Instantiate(effects.GetChild(0).gameObject, effects);

            effect.gameObject.SetActive(true);

            effect.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Discard: 1 card";
        }

        if (skill.description != "")
        {
            GameObject effect = Instantiate(effects.GetChild(0).gameObject, effects);

            effect.gameObject.SetActive(true);

            effect.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = skill.description;
        }

        if (skill.mana != 0)
        {
            string s = (skill.mana > 0) ? "Gain: " : "Lose: ";

            GameObject effect = Instantiate(effects.GetChild(0).gameObject, effects);

            effect.gameObject.SetActive(true);

            effect.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = s + skill.mana + " MP";
        }

    }

    public void Discardable(bool d)
    {
        discardLines.gameObject.SetActive(d);
    }

}
