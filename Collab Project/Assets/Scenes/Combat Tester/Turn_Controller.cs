using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class Turn_Controller : MonoBehaviour
{
    public Canvas left_Huds, right_Huds;

    private List<Combat_Character> all_Players;

    public List<Combat_Character> left_Players, right_Players;

    public enum Stage { TURN_START, TURN_END, ACTION_START, IMPACT, ACTION_END}

    public Combat_Character characterTurn;

    public DescriptionBox descriptionBox;

    public TextMeshProUGUI instructions;

    public Transform all_Hands;

    public GameObject handPrefab;

    public Draw_Selection draw_Selection;

    public Combat_Camera mainCamera;

    public Combo_Counter left_Combo_Counter, right_Combo_Counter;

    public Transform target_Arrows;

    public Transform damage_Bubbles;

    public Combat_Character hoveringOver;

    public Combat_Character selected;

    public List<Combat_Character> currentTurnOrder = new List<Combat_Character>();

    public Button endTurnButton;

    private bool endTurn = false;

    public Turn_Timeline turn_Timeline;

    public GameObject raycastBlocker;


    private void Start()
    {
        endTurnButton.onClick.AddListener(() => endTurn = true);

        StartCoroutine(InitializeCharacters());
    }

    IEnumerator InitializeCharacters()
    {
        foreach (Transform position in GameObject.Find("Left Players").transform)     // Players should be instantiated into positions eventually
        {
            if (position.childCount == 0)
                continue;

            Combat_Character character = position.GetChild(0).GetComponent<Combat_Character>();
            left_Players.Add(character);
            character.transform.localPosition = Vector3.zero;
        }

        foreach (Transform position in GameObject.Find("Right Players").transform)
        {
            if (position.childCount == 0)
                continue;

            Combat_Character character = position.GetChild(0).GetComponent<Combat_Character>();
            right_Players.Add(character);
            character.transform.localPosition = Vector3.zero;
            character.Facing = -1;
        }


        all_Players = new List<Combat_Character>(left_Players);
        all_Players.AddRange(right_Players);

        left_Huds.transform.GetChild(0).gameObject.SetActive(false);
        right_Huds.transform.GetChild(0).gameObject.SetActive(false);

        foreach (Combat_Character character in all_Players)
        {
            character.TurnController = this;

            character.TurnController.mainCamera = mainCamera;

            character.ClearStatChangers();        //This is only for DEBUGING

            if (left_Players.Contains(character))
            {
                character.Hud = Instantiate(left_Huds.transform.GetChild(0).gameObject.GetComponent<Character_Hud>(), left_Huds.transform);
            }
            else
            {
                character.Hud = Instantiate(right_Huds.transform.GetChild(0).gameObject.GetComponent<Character_Hud>(), right_Huds.transform);
            }

            character.Hud.gameObject.SetActive(true);

            character.Hud.diplayName.text = character.gameObject.name;
            character.Hud.TurnController = this;

            character.Health = character.character_Stats.max_Health; // remove this line for persistant stats
            character.Hud.healthBar.Initialize(character.Health, character.character_Stats.max_Health); 

            character.Mana = character.character_Stats.max_Mana;
            character.Hud.manaBar.Initialize(character.Mana, character.character_Stats.max_Mana);

            Hand newHand = Instantiate(handPrefab, all_Hands).GetComponent<Hand>();

            character.hand = newHand;

            newHand.character = character;

            character.target_Arrow = Instantiate(character.target_Arrow, target_Arrows);
        }

        foreach (Combat_Character character in all_Players)
        {
            character.Hud.charge_Timer.text = "";

            character.Hud.charge_Timer.rectTransform.sizeDelta = new Vector2(6, character.Hud.charge_Timer.fontSize * 30);

            for (int i = 29; i >= 0; i--)
            {
                character.Hud.charge_Timer.text += i + "\n";
            }

            yield return null;
        }

        yield return new WaitForSeconds(1f);

        StartCoroutine(RotateTurns());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            print("Time Affected By Host");
            foreach (Combat_Character character in all_Players)
            {
                StartCoroutine(character.Hud.AffectTimerProgress(1));
            }
        }

        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            print("Time Affected By Host");
            foreach (Combat_Character character in all_Players)
            {
                StartCoroutine(character.Hud.AffectTimerProgress(-1));
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            StartCoroutine(characterTurn.hand.GenerateCards(1, true));
        }
    }

    public List<string> GetPlayerNames(int i)
    {

        List<string> names = new List<string>();

        if (i == 1)
        {
            foreach (Combat_Character character in right_Players)
            {
                names.Add(character.name);
            }

        }

        if (i == -1)
        {
            foreach (Combat_Character character in left_Players)
            {
                names.Add(character.name);
            }
        }

        if (i == 0)
        {
            foreach (Combat_Character character in all_Players)
            {
                names.Add(character.name);
            }
        }


        return names;
    }


    IEnumerator RotateTurns()
    {

        currentTurnOrder = all_Players.OrderByDescending(o => o.Reaction).ToList();

        while (true)
        {
            while (currentTurnOrder.Count < 7)
            {
                var nextTurnOrder = all_Players.OrderByDescending(o => o.Reaction).ToList();

                while (currentTurnOrder[currentTurnOrder.Count - 1] == nextTurnOrder[0])
                {
                    nextTurnOrder = all_Players.OrderByDescending(o => o.Reaction).ToList();
                    Debug.Log("Shuffle");
                }

                currentTurnOrder.AddRange(nextTurnOrder);
            }


            characterTurn = currentTurnOrder[0];

            //yield return new WaitForSeconds(0.3f);

            // Move Camera 

            //yield return characterTurn.TurnController.mainCamera.Reset(0.2f);

            yield return turn_Timeline.Shift();

            //characterTurn.Hud.timer_Animations.Play("Pulser_Burst");

            characterTurn.Hud.SetTimerColor(Color.white);

            characterTurn.Hud.GetComponent<Animator>().Play("Turn_Indicate");

            //yield return new WaitForSeconds(0.417f);

            yield return characterTurn.MoveAmount(new Vector3((0.6f - characterTurn.transform.parent.localPosition.x) * characterTurn.Facing, 0, 0));

            characterTurn.Hud.timer_Animations.Play("Pulser_Pulse");

            endTurnButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Rest";

            yield return characterTurn.StartTurn();

            yield return new WaitUntil(() => endTurn == true);

            endTurn = false;

            yield return characterTurn.EndTurn();

            characterTurn.Hud.timer_Animations.Play("Pulser_Burst");

            // decrement charge

            yield return ResetPositions();

            // remove start of list 

            currentTurnOrder.RemoveAt(0);

            yield return null;
        }
    }

    public void ResetAnimations()
    {
        foreach (Transform target in characterTurn.hand.distinctTargets)
        {
            if(target.GetComponent<Combat_Character>().blocking)
                target.GetComponent<Combat_Character>().animationController.Clip("Block_Hold");
            else
                target.GetComponent<Combat_Character>().animationController.Clip("Idle");
        }
    }

    public IEnumerator ResetPositions()
    {
        yield return new WaitForSeconds(0.4f);

        descriptionBox.container.SetActive(false);

        left_Combo_Counter.ResetComboCount();

        right_Combo_Counter.ResetComboCount();

        characterTurn.Hud.GetComponent<Animator>().Play("Turn_Indicate_End");

        yield return new WaitUntil(() => damage_Bubbles.childCount == 0);

        // Reset Camera

        mainCamera.BlackOut(0f, 0.5f);

        Coroutine cam = StartCoroutine(mainCamera.Reset(0.2f));

        //  Reset Involved Characters

        Coroutine coroutine = null;

        foreach (Transform target in characterTurn.hand.distinctTargets)
            coroutine = StartCoroutine(target.GetComponent<Combat_Character>().ResetPos());

        yield return coroutine;

        foreach (Transform target in characterTurn.hand.distinctTargets)
            yield return target.GetComponent<Combat_Character>().Hud.healthBar.followBarCO;

        yield return cam;
    }


    public IEnumerator Reactions(Stage stage)
    {
        //foreach (Combat_Character character in all_Players)
        //{
        //    List<string> labels = new List<string>();
        //    List<int> indexs = new List<int>();
        //
        //    for (int i = 0; i < character.setSpells.Length; i++)
        //    {
        //        if (character.setSpells[i] == null || character.setSpells[i].Condition(stage, info) == false)
        //            continue;
        //        else
        //        {
        //            labels.Add(character.setSpells[i].name);
        //            indexs.Add(i);
        //        }
        //    }
        //
        //    if (labels.Count == 0)
        //    {
        //        yield return 0;
        //        continue;
        //    }
        //
        //    foreach (Combat_Character c in all_Players)
        //        c.animationController.Pause();
        //
        //    //character.MenuPositioning();
        //
        //    yield return character.SubMenuController.OpenSubMenu("Prompts", labels);
        //
        //    yield return character.SubMenuController.CurrentCD.coroutine;
        //
        //    foreach (Combat_Character c in all_Players)
        //        c.animationController.Play();
        //
        //    if (character.SubMenuController.CurrentSubMenu.ButtonChoice == -1)
        //    {
        //        continue;
        //    }
        //    else
        //    {
        //        character.SubMenuController.ResetMenus();
        //
        //        character.animationController.Clip(character.characterName + " Idle");
        //        yield return null;
        //        CoroutineWithData cd = new CoroutineWithData(this, character.setSpells[indexs[character.SubMenuController.CurrentSubMenu.ButtonChoice]].Action2(indexs[character.SubMenuController.CurrentSubMenu.ButtonChoice]));
        //        yield return cd.coroutine;
        //
        //        yield return (int)cd.result;
        //        break;
        //    }
        //}

        yield return 0;
    }

    public void CheckAllCards()
    {
        foreach(Combat_Character character in all_Players)
        {
            foreach (Card card in character.hand.cards)
            {
                card.card_Prefab.Usable = card.UseCondition();
            }
        }
    }

    public IEnumerator Blink()
    {
        raycastBlocker.SetActive(true);
        yield return null;
        raycastBlocker.SetActive(false);
    }
}
