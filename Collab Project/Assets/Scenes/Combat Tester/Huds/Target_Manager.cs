using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_Manager : MonoBehaviour
{
    public List<Combat_Character> elligbleTargets;

    public Skill.Selection selection;

    private Targetable hovering;

    public Targetable Hovering
    {
        get
        {
            return hovering;
        }
        set
        {
            if(hovering != null)
                hovering.selectionArrow.GetComponent<UnityEngine.UI.Image>().color = new Color(0, 0, 0, 0.5f);

            hovering = value;

            if (hovering != null)
                hovering.selectionArrow.GetComponent<UnityEngine.UI.Image>().color = Color.black;
        }
    }

   
}
