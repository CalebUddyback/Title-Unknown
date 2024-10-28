using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SubMenu_Attacks : SubMenu
{
    private Combat_Character.Skill displayedSkill;

    public override IEnumerator WaitForChoice()
    {
        Combat_Character combat_Character = SubMenuController.Owner;

        GetComponent<ScrollRect>().verticalScrollbar.value = 1;

        DescriptionBox dBox = SubMenuController.Owner.TurnController.left_descriptionBox;

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

                Combat_Character.Skill skill = SubMenuController.Owner.attackList[buttonContainer.GetChild(hoveringButton).GetComponent<SubMenu_Button>().storedIndex];

                if (displayedSkill != skill)
                {
                    displayedSkill = skill;
                    dBox.Description(skill);
                }
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
