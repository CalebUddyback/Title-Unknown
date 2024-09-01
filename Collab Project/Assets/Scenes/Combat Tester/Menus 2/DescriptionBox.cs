using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DescriptionBox : MonoBehaviour
{
    public GameObject container;
    public TextMeshProUGUI title;
    public TextMeshProUGUI DMG_Txt, CRT_Txt, REC_Txt, HIT_Txt;
    public TextMeshProUGUI type;
    public TextMeshProUGUI descriptionText;


    public void UpdateInfo()
    {
        if (transform.root.GetComponent<Turn_Controller>().characterTurn != null)
        {
            Combat_Character characterTurn = transform.root.GetComponent<Turn_Controller>().characterTurn;

            if (characterTurn.SubMenuController.CurrentSubMenu != null)
            {

                if (characterTurn.SubMenuController.CurrentSubMenu.gameObject.name == "Attacks")
                {
                    if (characterTurn.SubMenuController.CurrentSubMenu.hoveringButton == -1)
                    {
                        container.SetActive(false);
                        return;
                    }

                    container.SetActive(true);

                    Combat_Character.Skill skillInfo = characterTurn.attackList[characterTurn.SubMenuController.CurrentSubMenu.buttonContainer.GetChild(characterTurn.SubMenuController.CurrentSubMenu.hoveringButton).GetComponent<SubMenu_Button>().storedIndex];

                    title.text = skillInfo.name;

                    DMG_Txt.text = Mathf.Abs(skillInfo.baseInfo[0].damage).ToString();
                    CRT_Txt.text = skillInfo.baseInfo[0].critical.ToString();
                    REC_Txt.text = (skillInfo.focusPenalty/10f + characterTurn.focusSpeed).ToString();
                    HIT_Txt.text = skillInfo.baseInfo[0].accuracy.ToString();


                    string typeText = "[" + skillInfo.baseInfo[0].type.ToString().ToUpper();

                    if (skillInfo.effect)
                    {
                        typeText += " / EFFECT";
                    }

                    typeText += "]";

                    type.text = typeText;

                    descriptionText.text = skillInfo.description;

                }

                if (characterTurn.SubMenuController.CurrentSubMenu.gameObject.name == "Targets")
                {

                    Combat_Character.Skill skillInfo = characterTurn.chosenAttack;

                    if (characterTurn.SubMenuController.CurrentSubMenu.hoveringButton == -1)
                    {
                        CRT_Txt.text = characterTurn.chosenAttack.baseInfo[0].critical.ToString();
                        HIT_Txt.text = characterTurn.chosenAttack.baseInfo[0].accuracy.ToString();
                        //HIT_Txt.text = characterTurn.stats.GetCurrentStats(skillInfo)[Stats.Stat.PhHit].ToString();
                        return;
                    }


                    Combat_Character target = null;

                    if (characterTurn.Facing == 1)
                        target = characterTurn.TurnController.right_Players[characterTurn.SubMenuController.CurrentSubMenu.hoveringButton].GetComponent<Combat_Character>();


                    CRT_Txt.text = characterTurn.stats.GetCombatStats(characterTurn, target)[Stats.Stat.Crit].ToString() + "%";
                    HIT_Txt.text = characterTurn.stats.GetCombatStats(characterTurn, target)[Stats.Stat.PhHit].ToString() + "%";

                }
            }
        }
    }
}
