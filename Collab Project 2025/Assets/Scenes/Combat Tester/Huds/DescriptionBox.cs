using UnityEngine;
using TMPro;
using System.Linq;
using System;
using UnityEngine.UI;

public class DescriptionBox : MonoBehaviour
{
    public GameObject container;
    public TextMeshProUGUI title;
    public TextMeshProUGUI ATK_Label, ATK_Num, ATK_Mult, HIT_Label, HIT_Num, CRT_Label, CRT_Num, REC_Label, REC_Num, MP_Label, MP_Num;
    public TextMeshProUGUI type;
    [SerializeField]
    private TextMeshProUGUI description;

    private string[] keywords = { "charge", "heal", "increase", "decrease", "hp", "second(s)"};
    private char[] endPunc = { '.', ',', ';' };

    public void Description(Card card)
    {
        title.text = card.name_;
    
        //if (skill is Combat_Character.Spell spell)
        //    container.GetComponent<Image>().color = new Color(0.1058824f, 0.254902f, 0.2226822f, 0.7372549f);
        //else
            container.GetComponent<Image>().color = new Color(0.1058824f, 0.1215686f, 0.254902f, 0.7372549f);
    
    
        //ATK_Mult.text = "x" + skill.numOfHits.ToString();
        //
        //ATK_Mult.transform.parent.gameObject.SetActive(skill.numOfHits > 1 ? true : false);
    
        //int projectedValue = combat_Character.character_Stats.GetCombatStats(skill.skill_Stats[0])[Character_Stats.Stat.ATK];
        //dBox.ATK_Num.color = combat_Character.character_Stats.CompareStat(Character_Stats.Stat.ATK, projectedValue, false);
        ATK_Num.color = Color.white;
    
        //REC_Num.text = skill.Character.GetCurrentStats()[Character_Stats.Stat.AS].ToString();
        REC_Num.color = Color.white;
    
    
        MP_Num.text = card.stats.mana.ToString();
    
    
        //ATK_Num.text = Mathf.Abs(skill.Character.GetCombatStats(skill.skill_Stats)[Character_Stats.Stat.ATK]).ToString();
    
        //HIT_Num.text = skill.skill_Stats.accuracy != 0 ? skill.skill_Stats.accuracy.ToString() : "-";
    
        CRT_Num.text = card.stats.critical != 0 ? card.stats.critical.ToString() : "-";
    
        //dBox.CRT_Num.text = combat_Character.character_Stats.GetCombatStats(skill.skill_Stats[0])[Character_Stats.Stat.Crit].ToString();
    
    
        //if (skill.skill_Stats[0].statChanger != null)
        //    REC_Num.text = (skill.character.GetCombatStats(skill.skill_Stats[0])[Character_Stats.Stat.AS]).ToString();
    
    
    
    
        string typeText = "[" + card.type.ToString().ToUpper();
    
        if (card.effect)
            typeText += " / EFFECT";
    
        typeText += "]";
    
        type.text = typeText;
    
    
    
        string output = "";
    
        if (card.effect)
        {
            if (card.description != "")
            {
    
                string[] words = card.description.Split(' ');
    
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
            output = "<i>" + card.description + "</i>";
        }
    
        description.text = output.TrimEnd();
    }

}
