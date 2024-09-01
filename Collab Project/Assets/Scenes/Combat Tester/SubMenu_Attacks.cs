using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubMenu_Attacks : SubMenu
{
    public override IEnumerator WaitForChoice()
    {
        GetComponent<ScrollRect>().verticalScrollbar.value = 1;

        //yield return base.WaitForChoice();


        ButtonChoice = -2;

        while( ButtonChoice == -2)
        {

            // if hovering over button display info

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

        Combat_Character combat_Character = SubMenuController.owner;

        combat_Character.AttackChoice(combat_Character.attackList[ButtonChoice]);

        yield return combat_Character.chosenAttack.SubMenus(combat_Character);

        if(combat_Character.chosenAttack != null)
            combat_Character.EndTurn();
    }
}
