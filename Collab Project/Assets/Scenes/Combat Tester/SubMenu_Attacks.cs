using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubMenu_Attacks : SubMenu
{
    public override IEnumerator WaitForChoice()
    {
        Combat_Character combat_Character = SubMenuController.owner;

        GetComponent<ScrollRect>().verticalScrollbar.value = 1;

        //yield return base.WaitForChoice();

        DescriptionBox dBox = SubMenuController.owner.TurnController.left_descriptionBox;

        ButtonChoice = -2;

        hoveringButton = -1;

        while ( ButtonChoice == -2)
        {

            if (hoveringButton == -1)
            {
                dBox.container.SetActive(false);
            }
            else
            {

                dBox.container.SetActive(true);

                Combat_Character.Skill skill = SubMenuController.owner.attackList[buttonContainer.GetChild(hoveringButton).GetComponent<SubMenu_Button>().storedIndex];

                Combat_Character.Spell spell = skill as Combat_Character.Spell;

                if(spell != null)
                {
                    dBox.container.GetComponent<Image>().color = new Color(0.1058824f, 0.254902f, 0.2226822f, 0.7372549f);
                }
                else
                {
                    dBox.container.GetComponent<Image>().color = new Color(0.1058824f, 0.1215686f, 0.254902f, 0.7372549f);
                }

                dBox.title.text = skill.name;

                if (skill.level > 0)
                {
                    dBox.ATK_Mult.SetActive(true);
                    dBox.ATK_Mult.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "x" + skill.level.ToString();
                }
                else
                {
                    dBox.ATK_Mult.SetActive(false);
                }

                //dBox.ATK_Num.text = Mathf.Abs(combat_Character.character_Stats.GetCurrentStats()[Character_Stats.Stat.ATK]).ToString();
                dBox.ATK_Num.color = Color.white;

                dBox.REC_Num.text = combat_Character.character_Stats.GetCurrentStats()[Character_Stats.Stat.AS].ToString();
                dBox.REC_Num.color = Color.white;


                dBox.MP_Num.text = skill.skill_Stats[0].mana.ToString();


                dBox.ATK_Num.text = Mathf.Abs(combat_Character.character_Stats.GetCombatStats(skill.skill_Stats[0])[Character_Stats.Stat.ATK]).ToString();

                dBox.HIT_Num.text = skill.skill_Stats[0].accuracy != 0 ? skill.skill_Stats[0].accuracy.ToString() : "-";

                dBox.CRT_Num.text = skill.skill_Stats[0].critical != 0 ? skill.skill_Stats[0].critical.ToString() : "-";

                //dBox.CRT_Num.text = combat_Character.character_Stats.GetCombatStats(skill.skill_Stats[0])[Character_Stats.Stat.Crit].ToString();


                if (skill.skill_Stats[0].statChanger.GetStat(Character_Stats.Stat.AS))
                    dBox.REC_Num.text = (combat_Character.character_Stats.GetCombatStats(skill.skill_Stats[0])[Character_Stats.Stat.AS]).ToString();

                


                string typeText = "[" + skill.type.ToString().ToUpper();

                if (skill.effect)
                {
                    typeText += " / EFFECT";
                }

                typeText += "]";

                dBox.type.text = typeText;

                dBox.descriptionText.text = skill.description;
            }

            yield return null;
        }


        GetComponent<CanvasGroup>().interactable = false;

        yield return new WaitForSeconds(0.1f);  // This delay is to prevent quick double clicks (My Mouse is broken :/)

        if (ButtonChoice > -1)
            SubMenuController.subMenuStage = 0;
        else if (ButtonChoice == -1)
        {
            //SubMenuController.owner.chosenAttack = null;
            Return();
        }



        yield return null;      // Give menu time to close if -1

        combat_Character.ActionChoice(combat_Character.attackList[ButtonChoice]);

        yield return combat_Character.chosenAction.SubMenus(combat_Character);

        if(combat_Character.chosenAction != null)
            combat_Character.EndTurn();
    }
}
