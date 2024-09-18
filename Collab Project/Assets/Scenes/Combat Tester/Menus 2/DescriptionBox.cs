using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DescriptionBox : MonoBehaviour
{
    public GameObject container;
    public TextMeshProUGUI title;
    public GameObject ATK_Mult;
    public TextMeshProUGUI ATK_Label, ATK_Num, HIT_Label, HIT_Num, CRT_Label, CRT_Num, REC_Label, REC_Num, MP_Label, MP_Num;
    public TextMeshProUGUI type;
    public TextMeshProUGUI descriptionText;


    public void UpdateInfo()
    {
        //if (transform.root.GetComponent<Turn_Controller>().characterTurn != null)
        //{
        //    Combat_Character characterTurn = transform.root.GetComponent<Turn_Controller>().characterTurn;
        //
        //    if (characterTurn.SubMenuController.CurrentSubMenu != null)
        //    {
        //
        //        if (characterTurn.SubMenuController.CurrentSubMenu.gameObject.name == "Targets")
        //        {
        //
        //            Combat_Character.Skill skillInfo = characterTurn.chosenAttack;
        //
        //            if (characterTurn.SubMenuController.CurrentSubMenu.hoveringButton == -1)
        //            {                    
        //                return;
        //            }
        //
        //
        //            Combat_Character target = null;
        //
        //            if (characterTurn.Facing == 1)
        //                target = characterTurn.TurnController.right_Players[characterTurn.SubMenuController.CurrentSubMenu.hoveringButton].GetComponent<Combat_Character>();
        //
        //
        //            HIT_Num.text = characterTurn.stats.GetCombatStats(characterTurn, target)[Stats.Stat.PhHit].ToString();
        //            CRT_Num.text = characterTurn.stats.GetCombatStats(characterTurn, target)[Stats.Stat.Crit].ToString();
        //
        //        }
        //    }
        //}
    }
}
