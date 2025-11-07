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

    public int discards;
    public int chargeTime;

    public int health;
    public int mana;

    [System.Serializable]
    public class Stats
    {
        public Vector2Int DamageVariation;
        public int critical;
        public int mana;
        public Vector3 knockBack;

    }
    public Stats stats;


    public int CritSuccess { get; set; }
    public int HitSuccess { get; set; }

    public int targetQuantity = 1;

    [HideInInspector]
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
        card_Prefab.manaCost.text = stats.mana.ToString();

        if (stats.DamageVariation.y > 0)
        {
            int dam1 = Character.GetCurrentStats()[Character_Stats.Stat.STR] + stats.DamageVariation.x;
            int dam2 = Character.GetCurrentStats()[Character_Stats.Stat.STR] + stats.DamageVariation.y;

            GameObject effect = Instantiate(card_Prefab.effects.GetChild(0).gameObject, card_Prefab.effects);

            effect.gameObject.SetActive(true);

            //effect.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Deal " + "<color=red>" + "<b>" + dam1 + " - " + dam2 + "</b>" + "</color>" + " Damage";

            if (stats.DamageVariation.x == stats.DamageVariation.y)
                effect.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Deal: " + "<b>" + dam2 + "</b>" + " damage";
            else
                effect.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Deal: " + "<b>" + dam1 + "-" + dam2 + "</b>" + " damage";
        }

        if(discards > 0)
        {
            GameObject effect = Instantiate(card_Prefab.effects.GetChild(0).gameObject, card_Prefab.effects);

            effect.gameObject.SetActive(true);

            effect.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Discard: " + discards + ((discards > 1) ? " cards" : " card");
        }

        if (description != "")
        {
            GameObject effect = Instantiate(card_Prefab.effects.GetChild(0).gameObject, card_Prefab.effects);

            effect.gameObject.SetActive(true);

            effect.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = description;
        }

        if (mana != 0)
        {
            string s = (mana > 0) ? "Gain: " : "Lose: ";

            GameObject effect = Instantiate(card_Prefab.effects.GetChild(0).gameObject, card_Prefab.effects);

            effect.gameObject.SetActive(true);

            effect.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = s + mana + " MP";
        }

    }
}
