using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;
using System;

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

    public void DescriptionText(Combat_Character.Skill skill)
    {
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
