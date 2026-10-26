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

    public Decks deck;

    public bool negated = false;

    public bool usable;
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

    public void Start()
    {
        cardText.text = skill.displayName;
        manaCost.text = Mathf.Abs(skill.manaCost).ToString();

        if (skill.Target.damage > 0)
        {
            int dam = skill.Character.GetCurrentStats()[Character_Stats.Stat.STR] + skill.Target.damage;

            GameObject effect = Instantiate(effects.GetChild(0).gameObject, effects);

            effect.gameObject.SetActive(true);

            //effect.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Deal " + "<color=red>" + "<b>" + dam1 + " - " + dam2 + "</b>" + "</color>" + " Damage";

            effect.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Deal: " + "<b>" + dam + "</b>" + " damage";

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

        if (skill.Target.mana != 0)
        {
            string s = (skill.Target.mana > 0) ? "Gain: " : "Lose: ";

            GameObject effect = Instantiate(effects.GetChild(0).gameObject, effects);

            effect.gameObject.SetActive(true);

            effect.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = s + skill.Target.mana + " MP";
        }

    }

    public void Discardable(bool d)
    {
        discardLines.gameObject.SetActive(d);
    }

}
