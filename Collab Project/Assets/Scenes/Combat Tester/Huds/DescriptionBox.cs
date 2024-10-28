using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;
using UnityEngine.UI;

public class DescriptionBox : MonoBehaviour
{
    public GameObject container;
    public TextMeshProUGUI title;
    public GameObject ATK_Mult;
    public TextMeshProUGUI ATK_Label, ATK_Num, HIT_Label, HIT_Num, CRT_Label, CRT_Num, REC_Label, REC_Num, MP_Label, MP_Num;
    public TextMeshProUGUI type;
    [SerializeField]
    private TextMeshProUGUI description;

    private string[] keywords = { "charge", "heal", "increase", "decrease", "hp", "second(s)"};
    private char[] endPunc = { '.', ',', ';' };

    public void Description(Combat_Character.Skill skill)
    {
        title.text = skill.name;

        if (skill is Combat_Character.Spell spell)
            container.GetComponent<Image>().color = new Color(0.1058824f, 0.254902f, 0.2226822f, 0.7372549f);
        else
            container.GetComponent<Image>().color = new Color(0.1058824f, 0.1215686f, 0.254902f, 0.7372549f);


        if (skill.numOfHits > 0)
        {
            ATK_Mult.SetActive(true);
            ATK_Mult.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "x" + skill.numOfHits.ToString();
        }
        else
        {
            ATK_Mult.SetActive(false);
        }



        //int projectedValue = combat_Character.character_Stats.GetCombatStats(skill.skill_Stats[0])[Character_Stats.Stat.ATK];
        //dBox.ATK_Num.color = combat_Character.character_Stats.CompareStat(Character_Stats.Stat.ATK, projectedValue, false);
        ATK_Num.color = Color.white;

        REC_Num.text = skill.character.GetCurrentStats()[Character_Stats.Stat.AS].ToString();
        REC_Num.color = Color.white;


        MP_Num.text = skill.skill_Stats[0].mana.ToString();


        ATK_Num.text = Mathf.Abs(skill.character.GetCombatStats(skill.skill_Stats[0])[Character_Stats.Stat.ATK]).ToString();

        HIT_Num.text = skill.skill_Stats[0].accuracy != 0 ? skill.skill_Stats[0].accuracy.ToString() : "-";

        CRT_Num.text = skill.skill_Stats[0].critical != 0 ? skill.skill_Stats[0].critical.ToString() : "-";

        //dBox.CRT_Num.text = combat_Character.character_Stats.GetCombatStats(skill.skill_Stats[0])[Character_Stats.Stat.Crit].ToString();


        if (skill.skill_Stats[0].statChanger != null)
            REC_Num.text = (skill.character.GetCombatStats(skill.skill_Stats[0])[Character_Stats.Stat.AS]).ToString();




        string typeText = "[" + skill.type.ToString().ToUpper();

        if (skill.effect)
        {
            typeText += " / EFFECT";
        }

        typeText += "]";

        type.text = typeText;



        string output = "";

        if (skill.effect)
        {
            if (skill.description != "")
            {

                string[] words = skill.description.Split(' ');

                foreach (string word in words)
                {
                    string currentWord = word;

                    char[] chars = currentWord.ToCharArray();

                    string ending = " ";

                    if (endPunc.Contains(chars[chars.Length - 1]))
                        ending = chars[chars.Length - 1] + " ";

                    currentWord = new string(chars).Trim(endPunc);

                    if (keywords.Contains(currentWord.ToLower()) || int.TryParse(currentWord, out int i) || Enum.TryParse(currentWord, true, out Character_Stats.Stat e))
                    {
                        currentWord = char.ToUpper(currentWord[0]) + currentWord.Substring(1);
                        currentWord = "<color=yellow>" + "<b>" + currentWord + "</b>" + "</color>";
                    }

                    output += currentWord + ending;
                }

            }
        }
        else
        {
            output = "<i>" + skill.description + "</i>";
        }

        description.text = output.TrimEnd();
    }
}
