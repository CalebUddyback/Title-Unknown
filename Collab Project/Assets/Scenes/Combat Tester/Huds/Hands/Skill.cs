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

    [HideInInspector]
    public List<Transform> chosen_Targets = new List<Transform>();

    public virtual bool SetCondition()
    {
        return false;
    }

    public virtual bool ReactCondition()
    {
        return false;
    }

    public virtual bool UseCondition()
    {
        return false;
    }

    public abstract IEnumerator SetUp();

    public IEnumerator CharacterTargeting()
    {
        List<Combat_Character> targets = new List<Combat_Character>();

        Vector3 initialCameraPosition = Character.TurnController.mainCamera.transform.position;

        TurnController.instructions.text = "Select Target";

        TurnController.selectedCharacter = null;

        switch (selection)
        {
            case Selection.Self:
                targets.Add(Character);
                break;

            case Selection.Team:
                targets = Character.Team.members;
                yield return Character.TurnController.mainCamera.Reset(0.2f);
                break;

            case Selection.Team_Target:
                targets = Character.Team.members;
                yield return Character.TurnController.mainCamera.Reset(0.2f);
                break;

            case Selection.Team_Random:
                targets = Character.Team.members;
                yield return Character.TurnController.mainCamera.Reset(0.2f);
                break;

            case Selection.Oppostion:
                targets = Character.Team.Opposition.members;
                yield return Character.TurnController.mainCamera.Reset(0.2f);
                break;

            case Selection.Oppostion_Target:
                targets = Character.Team.Opposition.members;
                //yield return Character.TurnController.mainCamera.Reset(0.2f);
                break;

            case Selection.Oppostion_Random:
                targets = Character.Team.Opposition.members;
                yield return Character.TurnController.mainCamera.Reset(0.2f);
                break;

            case Selection.Targeter:
                targets.Add(Character.TurnController.resolveStack[Character.TurnController.resolveStack.Count - 2].hand.character);
                break;
        }

        for (int i = 0; i < targets.Count; i++)
        {
            targets[i].target_Arrow.gameObject.SetActive(true);
        }

        Color arrowColor = Color.black;

        bool instantTarget = true;

        while (chosen_Targets.Count < 1)
        {
            if (selection == Selection.Self || selection == Selection.Targeter || selection == Selection.Team || selection == Selection.Oppostion || selection == Selection.All)
            {
                foreach (Combat_Character c in targets)
                {
                    if (targets.Contains(TurnController.hoveringOver))
                        c.target_Arrow.GetComponent<Image>().color = arrowColor;
                    else
                        c.target_Arrow.GetComponent<Image>().color = new Color(arrowColor.r, arrowColor.g, arrowColor.b, 0.5f);
                }

                if (instantTarget)
                {
                    chosen_Targets = targets.Select(o => o.transform).ToList();
                }
                else
                {
                    if (targets.Contains(TurnController.selectedCharacter))
                        chosen_Targets = targets.Select(o => o.transform).ToList();
                }
            }
            else if ( selection == Selection.Team_Target || selection == Selection.Oppostion_Target || selection == Selection.All_Target)
            {
                foreach (Combat_Character c in targets)
                {
                    if (c == TurnController.hoveringOver)
                        c.target_Arrow.GetComponent<Image>().color = arrowColor;
                    else
                        c.target_Arrow.GetComponent<Image>().color = new Color(arrowColor.r, arrowColor.g, arrowColor.b, 0.5f);
                }

                if (targets.Contains(TurnController.selectedCharacter))
                    chosen_Targets.Add(TurnController.selectedCharacter.transform);
            }
            else if (selection == Selection.Team_Random || selection == Selection.Oppostion_Random || selection == Selection.All_Random)
            {
                foreach (Combat_Character c in targets)
                {
                    if (targets.Contains(TurnController.hoveringOver))
                        c.target_Arrow.GetComponent<Image>().color = arrowColor;
                    else
                        c.target_Arrow.GetComponent<Image>().color = new Color(arrowColor.r, arrowColor.g, arrowColor.b, 0.5f);
                }

                if (targets.Contains(TurnController.selectedCharacter))
                    chosen_Targets.Add(targets[Random.Range(0, targets.Count)].transform);
            }

            for (int i = 0; i < targets.Count; i++)
                targets[i].target_Arrow.GetComponent<RectTransform>().anchoredPosition = TurnController.mainCamera.UIPosition(targets[i].outcome_Bubble_Pos.position);

            yield return null;
        }

        HideTargets();
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

    public void HideTargets()
    {
        for (int i = 0; i < TurnController.target_Arrows.childCount; i++)
        {
            TurnController.target_Arrows.GetChild(i).gameObject.SetActive(false);
        }
    }
}
