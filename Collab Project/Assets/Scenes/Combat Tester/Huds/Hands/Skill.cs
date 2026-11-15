using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class Skill : MonoBehaviour
{
    public Combat_Character owner => transform.parent.parent.GetComponent<Combat_Character>();

    public Turn_Controller TurnController => owner.TurnController;

    public string displayName;
    public string animationName;
    public string description = "";

    public enum Selection { Oppostion_Target, Oppostion, Oppostion_Random, Self, Team_Target, Team, Team_Random, Targeter, All_Target, All, All_Random };
    public Selection selection;
    public enum Type { Offensive, Defensive, Spell };
    public Type type;
    public enum Range { Close, Far };
    public Range range;
    public enum Stage { Moving,  Impact};
    public Stage stage;
    public Combat_Character.Phase phase;

    [Header("Requirements")]

    public int manaCost;
    public bool discard;
    public int chargeTime;

    [Header("Stats")]
    public bool guaranteedHit = false;
    public int accuracy = 100;
    public int critical;
    public Turn_Controller.Effect effect;

    public Vector2 distance = new Vector2(0.35f, 0.35f);
    public Vector3 knockBack;

    [System.Serializable]
    public class Test
    {
        public int damage;
        public int health;
        public int defense;
        public int mana;
    }

    [Header("Effects")]
    public Test target;

    public int HitChance { get; set; }
    public int HitSuccess { get; set; }
    public int CritSuccess { get; set; }

    private void OnValidate()
    {
        if (manaCost > 0)
        {
            manaCost *= -1;
            Debug.LogWarning("Positive values are not allowed for 'Mana Cost'");
        }
    }

    //[HideInInspector]
    public List<Combat_Character> chosen_Targets = new List<Combat_Character>();

    public virtual bool UseCondition()
    {
        if (owner.currentPhase != phase)
            return false;

        if (manaCost + owner.Mana <= 0)
            return false;

        if (discard)
            if (owner.deck.hand.Count < 1)
                return false;

        return true;
    }

    public abstract IEnumerator SetUp();

    public virtual IEnumerator CharacterTargeting()
    {
        List<Combat_Character> eligible_Targets = new List<Combat_Character>();

        Vector3 initialCameraPosition = owner.TurnController.mainCamera.transform.position;

        TurnController.instructions.text = "Select Target";

        switch (selection)
        {
            case Selection.Self:
                eligible_Targets.Add(owner);
                break;

            case Selection.Team:
                eligible_Targets = owner.Team.members;
                //yield return Character.TurnController.mainCamera.Reset(0.2f);
                break;

            case Selection.Team_Target:
                eligible_Targets = owner.Team.members;
                yield return owner.TurnController.mainCamera.Reset(0.2f);
                break;

            case Selection.Team_Random:
                eligible_Targets = owner.Team.members;
                //yield return Character.TurnController.mainCamera.Reset(0.2f);
                break;

            case Selection.Oppostion:
                eligible_Targets = owner.Team.Opposition.members;
                //yield return Character.TurnController.mainCamera.Reset(0.2f);
                break;

            case Selection.Oppostion_Target:
                eligible_Targets = owner.Team.Opposition.members;
                //yield return Character.TurnController.mainCamera.Reset(0.2f);
                break;

            case Selection.Oppostion_Random:
                eligible_Targets = owner.Team.Opposition.members;
                //yield return Character.TurnController.mainCamera.Reset(0.2f);
                break;

            case Selection.Targeter:
                eligible_Targets.Add(owner.TurnController.resolveStack[owner.TurnController.resolveStack.Count - 1].deck.owner);
                break;
        }

        for (int i = 0; i < TurnController.all_Players.Count; i++)
        {
            Combat_Character c = TurnController.all_Players[i];

            if (eligible_Targets.Contains(c))
            {
                StartCoroutine(c.target_Arrow.SetState(Target_Arrow.State.Target, true));

                if (guaranteedHit)
                {
                    c.target_Arrow.hitChance.text = "";
                }
                else
                {
                    HitChance = owner.GetCurrentStats(this)[Character_Stats.Stat.DEX];

                    c.target_Arrow.hitChance.text = Mathf.CeilToInt(((float)HitChance / (HitChance + c.MissChance())) * 100).ToString() + "%";
                }
            }
            else
            {
                c.target_Arrow.Shrink();
            }

            yield return null;
        }

        if (selection == Selection.Team || selection == Selection.Oppostion || selection == Selection.All)
        {
            while (TurnController.CharacterActing.deck.SelectedSlot != null)
            {
                yield return null;

                foreach (Combat_Character c in eligible_Targets)
                {
                    if (eligible_Targets.Contains(TurnController.CharacterHoveringOver) && c == TurnController.CharacterHoveringOver)
                        continue;
                    else if (eligible_Targets.Contains(TurnController.CharacterHoveringOver))
                        c.target_Arrow.Highlight(true);
                    else
                        c.target_Arrow.Highlight(false);
                }
            }

            chosen_Targets = new List<Combat_Character>(eligible_Targets);

        }
        else if (selection == Selection.Team_Random || selection == Selection.Oppostion_Random || selection == Selection.All_Random)
        {
            while (TurnController.CharacterActing.deck.SelectedSlot != null)
            {
                yield return null;

                foreach (Combat_Character c in eligible_Targets)
                {
                    if (eligible_Targets.Contains(TurnController.CharacterHoveringOver) && c == TurnController.CharacterHoveringOver)
                        continue;
                    else if (eligible_Targets.Contains(TurnController.CharacterHoveringOver))
                        c.target_Arrow.Highlight(true);
                    else
                        c.target_Arrow.Highlight(false);
                }
            }

            chosen_Targets[0] = eligible_Targets[Random.Range(0, eligible_Targets.Count)];
        }
        else
        {
            while (TurnController.CharacterActing.deck.SelectedSlot != null)
            {
                yield return null;
            }
        }

        //Hide Targets

        for (int i = 0; i < TurnController.all_Players.Count; i++)
        {
            Combat_Character c = TurnController.all_Players[i];

            if (eligible_Targets.Contains(c))
                c.target_Arrow.Shrink();

            yield return null;
        }
    }

    public abstract IEnumerator Execute();

    public abstract IEnumerator Resolve();

    public void GetOutcome(Combat_Character target)
    {
        int hitRoll = Mathf.CeilToInt((float)(Random.Range(0, HitChance) + Random.Range(0, HitChance)) / 2);

        // If equal: Clash?

        if (hitRoll > target.MissChance())
            HitSuccess = 1;
        else if (hitRoll == target.MissChance())
            HitSuccess = 0;
        else
            HitSuccess = 0;

        Debug.Log(hitRoll + " : " + target.MissChance());

        int critRoll = Random.Range(0, 100);

        CritSuccess = critRoll < critical ? 3 : 1;
    }
}
