using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class Skill : MonoBehaviour
{
    public Combat_Character Character => transform.parent.parent.GetComponent<Combat_Character>();

    public Turn_Controller TurnController => Character.TurnController;

    public string displayName;
    public string animationName;
    public bool effect = false;
    public string description = "";

    public enum Selection { Oppostion_Target, Oppostion, Oppostion_Random, Self, Team_Target, Team, Team_Random, Targeter, All_Target, All, All_Random };
    public Selection selection;
    public enum Type { Offensive, Defensive, Support };
    public Type type;
    public enum Range { Close, Far };
    public Range range;
    public enum Stage { Moving,  Impact};
    public Stage stage;

    [Header("Requirements")]

    public bool discard;
    public int chargeTime;
    public bool set = false;

    [Header("Stats")]

    public int manaCost;
    public Vector2Int DamageVariation;
    public int critical;

    [System.Serializable]
    public class Intervals
    {
        public float distance = 0.35f;
        public Vector3 knockBack;
    }
    public Intervals intervals;

    [Header("Effects")]

    public int health;
    public int mana;

    public int CritSuccess { get; set; }
    public int HitSuccess { get; set; }

    private void OnValidate()
    {
        if (manaCost > 0)
        {
            manaCost *= -1;
            Debug.LogWarning("Positive values are not allowed for 'Mana Cost'");
        }

        if (DamageVariation.x < 0)
        {
            DamageVariation.x *= -1;
            Debug.LogWarning("Positive values are not allowed for 'DamageVariation.x'");
        }

        if (DamageVariation.y < 0)
        {
            DamageVariation.y *= -1;
            Debug.LogWarning("Positive values are not allowed for 'DamageVariation.y'");
        }

        if (DamageVariation.y < DamageVariation.x)
        {
            DamageVariation.x = DamageVariation.y;
            Debug.LogWarning("DamageVariation.y' cannot be lower than 'DamageVariation.x'");
        }

    }

    //[HideInInspector]
    public List<Combat_Character> chosen_Targets = new List<Combat_Character>();

    public virtual bool ReactCondition()
    {
        return false;
    }

    public virtual bool UseCondition()
    {
        return false;
    }

    public abstract IEnumerator SetUp();

    public virtual IEnumerator CharacterTargeting()
    {
        List<Combat_Character> eligible_Targets = new List<Combat_Character>();

        Vector3 initialCameraPosition = Character.TurnController.mainCamera.transform.position;

        TurnController.instructions.text = "Select Target";

        switch (selection)
        {
            case Selection.Self:
                eligible_Targets.Add(Character);
                break;

            case Selection.Team:
                eligible_Targets = Character.Team.members;
                //yield return Character.TurnController.mainCamera.Reset(0.2f);
                break;

            case Selection.Team_Target:
                eligible_Targets = Character.Team.members;
                //yield return Character.TurnController.mainCamera.Reset(0.2f);
                break;

            case Selection.Team_Random:
                eligible_Targets = Character.Team.members;
                //yield return Character.TurnController.mainCamera.Reset(0.2f);
                break;

            case Selection.Oppostion:
                eligible_Targets = Character.Team.Opposition.members;
                //yield return Character.TurnController.mainCamera.Reset(0.2f);
                break;

            case Selection.Oppostion_Target:
                eligible_Targets = Character.Team.Opposition.members;
                //yield return Character.TurnController.mainCamera.Reset(0.2f);
                break;

            case Selection.Oppostion_Random:
                eligible_Targets = Character.Team.Opposition.members;
                //yield return Character.TurnController.mainCamera.Reset(0.2f);
                break;

            case Selection.Targeter:
                eligible_Targets.Add(Character.TurnController.resolveStack[Character.TurnController.resolveStack.Count - 2].deck.owner);
                break;
        }

        for (int i = 0; i < eligible_Targets.Count; i++)
        {
            eligible_Targets[i].target_Arrow.gameObject.SetActive(true);
        }

        Color arrowColor = Color.black;

        if (selection == Selection.Team || selection == Selection.Oppostion || selection == Selection.All)
        {
            while (TurnController.characterTurn.deck.SelectedSlot != null)
            {
                yield return null;

                foreach (Combat_Character c in eligible_Targets)
                {
                    if (eligible_Targets.Contains(TurnController.hoveringOver) && c == TurnController.hoveringOver)
                        continue;
                    else if (eligible_Targets.Contains(TurnController.hoveringOver))
                        c.target_Arrow.Highlight(true);
                    else
                        c.target_Arrow.Highlight(false);
                }
            }

            if(chosen_Targets.Count > 0)
                chosen_Targets = eligible_Targets;

        }
        else if (selection == Selection.Team_Random || selection == Selection.Oppostion_Random || selection == Selection.All_Random)
        {
            while (TurnController.characterTurn.deck.SelectedSlot != null)
            {
                yield return null;

                foreach (Combat_Character c in eligible_Targets)
                {
                    if (eligible_Targets.Contains(TurnController.hoveringOver) && c == TurnController.hoveringOver)
                        continue;
                    else if (eligible_Targets.Contains(TurnController.hoveringOver))
                        c.target_Arrow.Highlight(true);
                    else
                        c.target_Arrow.Highlight(false);
                }
            }

            if (chosen_Targets.Count > 0)
                chosen_Targets.Add(eligible_Targets[Random.Range(0, eligible_Targets.Count)]);
        }
        else
        {
            while (TurnController.characterTurn.deck.SelectedSlot != null)
            {
                yield return null;
            }
        }

        //Hide Targets

        for (int i = 0; i < TurnController.target_Arrows.childCount; i++)
        {
            TurnController.target_Arrows.GetChild(i).gameObject.SetActive(false);
        }
    }

    public abstract IEnumerator Execute();

    public abstract IEnumerator Resolve();

    public void GetOutcome(Combat_Character target)
    {
        int critRoll = Random.Range(0, 100);

        CritSuccess = critRoll < critical ? 3 : 1;

        int hitRoll = (Random.Range(0, 100) + Random.Range(0, 100)) / 2;

        HitSuccess = hitRoll > target.GetCurrentStats()[Character_Stats.Stat.LCK] ? 1 : 0;
    }
}
