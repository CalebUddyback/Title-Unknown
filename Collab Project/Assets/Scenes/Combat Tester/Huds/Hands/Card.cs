using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public abstract class Card : MonoBehaviour
{
    [HideInInspector]
    public Hand hand;

    public Combat_Character Character => hand.character;

    public Turn_Controller Turn_Controller => Character.TurnController;

    [HideInInspector]
    public Card_Prefab card_Prefab;

    public string displayName;
    public string animationName;
    public bool effect = false;
    public string description = "";

    public enum Scope { Self, Team, Enemy };
    public Scope scope;
    public enum Selection { Singular, All, Random };
    public Selection selection;
    public enum Type { Offensive, Defensive, Support };
    public Type type;
    public enum Range { Close, Far };
    public Range range;

    public float distance = 0.35f;

    public int chargeTime;

    [System.Serializable]
    public class Stats
    {
        public Vector2Int DamageVariation;
        public int critical;
        public int defense;
        public int mana;
    }
    public Stats stats;


    public int CritSuccess { get; set; }
    public int HitSuccess { get; set; }

    public int targetQuantity = 1;

    public List<Transform> chosen_Targets = new List<Transform>();

    public abstract bool UseCondition();

    public abstract IEnumerator SetUp();

    public IEnumerator CharacterTargeting()
    {
        List<Combat_Character> targets = new List<Combat_Character>();

        Vector3 initialCameraPosition = Character.TurnController.mainCamera.transform.position;

        Turn_Controller.instructions.text = "Select Target";

        Turn_Controller.selected = null;

        switch (scope)
        {
            case Scope.Self:
                targets.Add(Character);
                break;

            case Scope.Team:
                targets = Turn_Controller.left_Players.Contains(Character) ? Turn_Controller.left_Players : Turn_Controller.right_Players;

                yield return Character.TurnController.mainCamera.Reset(0.2f);

                break;

            case Scope.Enemy:
                targets = Turn_Controller.left_Players.Contains(Character) ? Turn_Controller.right_Players : Turn_Controller.left_Players;
                break;
        }


        if (selection == Selection.All)
            targetQuantity = targets.Count;


        for (int i = 0; i < targets.Count; i++)
        {
            targets[i].target_Arrow.gameObject.SetActive(true);
        }

        while (chosen_Targets.Count < targetQuantity)
        {
            if(scope == Scope.Self)
            {
                chosen_Targets.Add(Character.transform);
                continue;
            }


            for (int i = 0; i < targets.Count; i++)
            {
                targets[i].target_Arrow.transform.position = Turn_Controller.mainCamera.UIPosition(targets[i].outcome_Bubble_Pos.position);
            }

            Color arrowColor = Color.black;

            switch (selection)
            {
                case Selection.Singular:
                    foreach (Combat_Character c in targets)
                    {
                        if (c == Turn_Controller.hoveringOver)
                            c.target_Arrow.GetComponent<Image>().color = arrowColor;
                        else
                            c.target_Arrow.GetComponent<Image>().color = new Color(arrowColor.r, arrowColor.g, arrowColor.b, 0.5f);
                    }

                    if (targets.Contains(Turn_Controller.selected))
                        chosen_Targets.Add(Turn_Controller.selected.transform);
                    break;

                case Selection.All:
                    foreach (Combat_Character c in targets)
                    {
                        if (targets.Contains(Turn_Controller.hoveringOver))
                            c.target_Arrow.GetComponent<Image>().color = arrowColor;
                        else
                            c.target_Arrow.GetComponent<Image>().color = new Color(arrowColor.r, arrowColor.g, arrowColor.b, 0.5f);
                    }

                    if (targets.Contains(Turn_Controller.selected))
                        chosen_Targets = targets.Select(o => o.transform).ToList();
                    break;

                case Selection.Random:
                    foreach (Combat_Character c in targets)
                    {
                        if (targets.Contains(Turn_Controller.hoveringOver))
                            c.target_Arrow.GetComponent<Image>().color = arrowColor;
                        else
                            c.target_Arrow.GetComponent<Image>().color = new Color(arrowColor.r, arrowColor.g, arrowColor.b, 0.5f);
                    }

                    if (targets.Contains(Turn_Controller.selected))
                        for (int i = 0; i < targetQuantity; i++)
                        {
                            int x;

                            do
                            {
                                x = Random.Range(0, targets.Count);
                            }
                            while (chosen_Targets.Contains(targets[x].transform));

                            chosen_Targets.Add(targets[x].transform);
                        }
                    break;
            }


            yield return null;
        }

        HideTargets();
    }

    public abstract IEnumerator Action();

    public void GetOutcome(Stats stats, Combat_Character target)
    {
        int critRoll = Random.Range(0, 100);

        CritSuccess = critRoll < this.stats.critical ? 3 : 1;

        int hitRoll = (Random.Range(0, 100) + Random.Range(0, 100)) / 2;

        HitSuccess = hitRoll > target.GetCurrentStats()[Character_Stats.Stat.LCK] ? 1 : 0;
    }

    public void HideTargets()
    {
        for (int i = 0; i < Turn_Controller.target_Arrows.childCount; i++)
        {
            Turn_Controller.target_Arrows.GetChild(i).gameObject.SetActive(false);
        }
    }

    public void Start()
    {
        card_Prefab.cardText.text = displayName;
    }
}
