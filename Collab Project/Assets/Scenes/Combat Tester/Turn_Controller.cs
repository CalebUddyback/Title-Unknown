using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;
using TMPro;

public class Turn_Controller : MonoBehaviour
{
    private List<Combat_Character> all_Players;

    public enum Stage { TURN_START, TURN_END, ACTION_START, IMPACT, ACTION_END}

    public Combat_Character characterTurn;

    public DescriptionBox descriptionBox;

    public TextMeshProUGUI instructions;

    public GameObject decks_Prefab;

    public Draw_Selection draw_Selection;

    public Combat_Camera mainCamera;

    public GameObject target_Arrow_Prefab;
    public Transform target_Arrows;

    public GameObject reaction_Mark_Prefab;
    public Transform reaction_Marks;

    public Transform damage_Bubbles;

    public Combat_Character hoveringOver;

    public Combat_Character selectedCharacter;

    public List<Combat_Character> currentTurnOrder = new List<Combat_Character>();

    public Button endTurnButton;

    private bool endTurn = false;

    private bool endReaction = false;

    public Turn_Timeline turn_Timeline;

    public GameObject raycastBlocker;

    [System.Serializable]
    public class Team
    {
        public Canvas huds;
        public Transform positions;
        public List<Combat_Character> members = new List<Combat_Character>();
        public Team Opposition;
        public Transform decks;
        public Decks visibleDeck;
        public Combo_Counter combo_Counter;
        public int facing;
    }
    public Team[] teams = new Team[2];

    private void Start()
    {
        endTurnButton.onClick.AddListener(() => endTurn = true);

        StartCoroutine(InitializeCharacters());
    }

    IEnumerator InitializeCharacters()
    {
        all_Players = new List<Combat_Character>();

        foreach (Team team in teams)
        {
            foreach (Transform position in team.positions)     // Players should be instantiated into positions eventually
            {
                if (position.childCount == 0)
                    continue;

                Combat_Character character = position.GetChild(0).GetComponent<Combat_Character>();
                team.members.Add(character);
                character.transform.localPosition = Vector3.zero;
                character.Facing = team.facing;
                team.huds.transform.GetChild(0).gameObject.SetActive(false);
            }

            foreach (Combat_Character character in team.members)
            {
                character.TurnController = this;

                character.TurnController.mainCamera = mainCamera;

                character.ClearStatChangers();        //This is only for DEBUGING

                character.Hud = Instantiate(team.huds.transform.GetChild(0).gameObject.GetComponent<Character_Hud>(), team.huds.transform);

                character.Hud.gameObject.SetActive(true);

                character.Hud.diplayName.text = character.gameObject.name;
                character.Hud.TurnController = this;

                character.InitialHealth = character.character_Stats.max_Health; // remove this line for persistant stats
                character.Hud.healthBar.Initialize(character.Health(), character.character_Stats.max_Health);

                character.InitialMana = character.character_Stats.max_Mana;
                character.Hud.manaBar.Initialize(character.Mana(), character.character_Stats.max_Mana);

                Decks newHand = Instantiate(decks_Prefab, team.decks).GetComponent<Decks>();

                newHand.GetComponent<RectTransform>().localPosition += Vector3.up * -170;

                //newHand.gameObject.gameObject.SetActive(false);

                character.decks = newHand;

                newHand.character = character;

                foreach (Transform skill in character.skills.transform)
                {
                    if (skill.gameObject.activeSelf == false)
                        continue;

                    Card card = Instantiate(newHand.card_Prefab, newHand.drawDeck);

                    card.GetComponent<RectTransform>().anchoredPosition = newHand.drawDeck.GetComponent<RectTransform>().anchoredPosition;

                    card.gameObject.SetActive(false);

                    card.gameObject.name = skill.name + " Card";

                    card.skill = skill.GetComponent<Skill>();
                }

                newHand.drawDeckQuantity.text = newHand.drawDeck.childCount.ToString();

                newHand.discardDeckQuantity.text = newHand.discardDeck.childCount.ToString();

                yield return newHand.ShuffleCards();

                character.target_Arrow = Instantiate(target_Arrow_Prefab, target_Arrows);

                character.reaction_Arrow = Instantiate(reaction_Mark_Prefab, reaction_Marks);

                character.Team = team;
            }

            foreach (Combat_Character character in team.members)
            {
                character.Hud.charge_Timer.text = "";

                character.Hud.charge_Timer.rectTransform.sizeDelta = new Vector2(6, character.Hud.charge_Timer.fontSize * 30);

                for (int i = 29; i >= 0; i--)
                {
                    character.Hud.charge_Timer.text += i + "\n";
                }

                yield return null;
            }

            all_Players.AddRange(team.members);
        }

        yield return new WaitForSeconds(1f);

        teams[0].Opposition = teams[1];
        teams[1].Opposition = teams[0];

        StartCoroutine(RotateTurns());
    }

    //IEnumerator OLDInitializeCharacters()
    //{
    //    foreach (Transform position in GameObject.Find("Left Players").transform)     // Players should be instantiated into positions eventually
    //    {
    //        if (position.childCount == 0)
    //            continue;
    //
    //        Combat_Character character = position.GetChild(0).GetComponent<Combat_Character>();
    //        left_Players.Add(character);
    //        character.transform.localPosition = Vector3.zero;
    //    }
    //
    //    foreach (Transform position in GameObject.Find("Right Players").transform)
    //    {
    //        if (position.childCount == 0)
    //            continue;
    //
    //        Combat_Character character = position.GetChild(0).GetComponent<Combat_Character>();
    //        right_Players.Add(character);
    //        character.transform.localPosition = Vector3.zero;
    //        character.Facing = -1;
    //    }
    //
    //
    //    all_Players = new List<Combat_Character>(left_Players);
    //    all_Players.AddRange(right_Players);
    //
    //    left_Huds.transform.GetChild(0).gameObject.SetActive(false);
    //    right_Huds.transform.GetChild(0).gameObject.SetActive(false);
    //
    //    foreach (Combat_Character character in all_Players)
    //    {
    //        character.TurnController = this;
    //
    //        character.TurnController.mainCamera = mainCamera;
    //
    //        character.ClearStatChangers();        //This is only for DEBUGING
    //
    //        if (left_Players.Contains(character))
    //        {
    //            character.Hud = Instantiate(left_Huds.transform.GetChild(0).gameObject.GetComponent<Character_Hud>(), left_Huds.transform);
    //        }
    //        else
    //        {
    //            character.Hud = Instantiate(right_Huds.transform.GetChild(0).gameObject.GetComponent<Character_Hud>(), right_Huds.transform);
    //        }
    //
    //        character.Hud.gameObject.SetActive(true);
    //
    //        character.Hud.diplayName.text = character.gameObject.name;
    //        character.Hud.TurnController = this;
    //
    //        character.InitialHealth = character.character_Stats.max_Health; // remove this line for persistant stats
    //        character.Hud.healthBar.Initialize(character.Health(), character.character_Stats.max_Health);
    //
    //        character.InitialMana = character.character_Stats.max_Mana;
    //        character.Hud.manaBar.Initialize(character.Mana(), character.character_Stats.max_Mana);
    //
    //        Decks newHand = Instantiate(decks_Prefab, (left_Players.Contains(character)) ? left_Hands : right_Hands).GetComponent<Decks>();
    //
    //        newHand.GetComponent<RectTransform>().localPosition += Vector3.up * -170;
    //
    //        //newHand.gameObject.gameObject.SetActive(false);
    //
    //        character.cards = newHand;
    //
    //        newHand.character = character;
    //
    //        foreach (Transform skill in character.skills.transform)
    //        {
    //            if (skill.gameObject.activeSelf == false)
    //                continue;
    //
    //            Card card = Instantiate(newHand.card_Prefab, newHand.drawDeck);
    //
    //            card.GetComponent<RectTransform>().anchoredPosition = newHand.drawDeck.GetComponent<RectTransform>().anchoredPosition;
    //
    //            card.gameObject.SetActive(false);
    //
    //            card.gameObject.name = skill.name + " Card";
    //
    //            card.skill = skill.GetComponent<Skill>();
    //        }
    //
    //        newHand.drawDeckQuantity.text = newHand.drawDeck.childCount.ToString();
    //
    //        newHand.discardDeckQuantity.text = newHand.discardDeck.childCount.ToString();
    //
    //        yield return newHand.ShuffleCards();
    //
    //        character.target_Arrow = Instantiate(target_Arrow_Prefab, target_Arrows);
    //
    //        character.reaction_Arrow = Instantiate(reaction_Mark_Prefab, reaction_Marks);
    //    }
    //
    //    foreach (Combat_Character character in all_Players)
    //    {
    //        character.Hud.charge_Timer.text = "";
    //
    //        character.Hud.charge_Timer.rectTransform.sizeDelta = new Vector2(6, character.Hud.charge_Timer.fontSize * 30);
    //
    //        for (int i = 29; i >= 0; i--)
    //        {
    //            character.Hud.charge_Timer.text += i + "\n";
    //        }
    //
    //        yield return null;
    //    }
    //
    //    yield return new WaitForSeconds(1f);
    //
    //    StartCoroutine(RotateTurns());
    //}

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
            StartCoroutine(characterTurn.decks.DrawCards(1, true));
        }
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
                    Debug.Log("Re-roll");
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

            characterTurn.animationController.GetComponent<SpriteRenderer>().sortingOrder = 1;

            yield return characterTurn.StartTurn();

            yield return new WaitUntil(() => endTurn == true);

            endTurn = false;

            yield return characterTurn.EndTurn();

            characterTurn.animationController.GetComponent<SpriteRenderer>().sortingOrder = 0;

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
        foreach (Transform target in characterTurn.decks.distinctTargets)
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

        //descriptionBox.container.SetActive(false);

        foreach(Team team in teams)
            team.combo_Counter.ResetComboCount();

        characterTurn.Hud.GetComponent<Animator>().Play("Turn_Indicate_End");

        yield return new WaitUntil(() => damage_Bubbles.childCount == 0);

        // Reset Camera

        mainCamera.BlackOut(0f, 0.5f);

        Coroutine cam = StartCoroutine(mainCamera.Reset(0.2f));

        //  Reset Involved Characters

        Coroutine coroutine = null;

        foreach (Transform target in characterTurn.decks.distinctTargets)
            coroutine = StartCoroutine(target.GetComponent<Combat_Character>().ResetPos());

        yield return coroutine;

        foreach (Transform target in characterTurn.decks.distinctTargets)
            yield return target.GetComponent<Combat_Character>().Hud.healthBar.followBarCO;

        yield return cam;
    }

    public void CheckAllCards()
    {
        foreach(Combat_Character character in all_Players)
        {
            foreach (Card card_Prefab in character.decks.hand)
            {
                card_Prefab.Usable = card_Prefab.skill.UseCondition();
            }
        }
    }

    public IEnumerator Reactions(Skill skill, Stage stage)
    {
        selectedCharacter = null;

        List<Combat_Character> reactors = new List<Combat_Character>();

        skill.Character.Team.visibleDeck.Locked = true;

        if(skill.Character.Team.Opposition.visibleDeck != null)
            skill.Character.Team.Opposition.visibleDeck.Locked = false;

        foreach (Combat_Character character in skill.Character.Team.Opposition.members)
        {
            foreach (Transform child in character.decks.hand_Pos)
            {
                Hand_Slot slot = child.GetComponent<Hand_Slot>();

                if (slot.set && !resolveStack.Contains(slot.card) && slot.card.skill.ReactCondition(skill, stage))
                {
                    StartCoroutine(Mark(character));
                    reactors.Add(character);
                    break;
                }

            }
        }

        if (reactors.Count > 0)
        {
            endTurnButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "Skip";

            endTurnButton.onClick.RemoveAllListeners();

            endTurnButton.onClick.AddListener(() => endReaction = true);

            while (endReaction == false)
            {

                if (selectedCharacter != null && reactors.Contains(selectedCharacter))
                {
                    Combat_Character temp = selectedCharacter;
                    selectedCharacter = null;

                    if (temp.Team.visibleDeck != temp.decks)
                        yield return temp.decks.Raise();

                    temp.decks.Locked = false;
                }
                yield return null;
            }

            endTurnButton.onClick.RemoveAllListeners();

            endTurnButton.onClick.AddListener(() => endTurn = true);

            endTurnButton.transform.GetChild(0).GetComponent<TextMeshProUGUI>().text = "End Turn";
        }

        yield return 0;
    }

    IEnumerator Mark(Combat_Character character)
    {
        character.reaction_Arrow.SetActive(true);

        while (endReaction == false)
        {
            character.reaction_Arrow.GetComponent<RectTransform>().anchoredPosition = mainCamera.UIPosition(character.outcome_Bubble_Pos.position);

            if(character.Team.visibleDeck == character.decks)
                character.reaction_Arrow.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.white;
            else if (character == character.TurnController.hoveringOver)
                character.reaction_Arrow.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.yellow;
            else
                character.reaction_Arrow.transform.GetChild(0).GetComponent<TextMeshProUGUI>().color = Color.red;

            yield return null;
        }

        character.reaction_Arrow.SetActive(false);
    }

    public List<Card> resolveStack = new List<Card>();

    public IEnumerator ResolveCards()
    {

        while(resolveStack.Count > 0)
        {
            Card card = resolveStack[resolveStack.Count - 1];

            if (!card.negated)
            {
                //Debug.Log(card.skill.name + " resolved", card.gameObject);
                yield return card.skill.Resolve();
            }
            else
            {
                //Debug.Log(card.skill.name + " negated");
                card.negated = false;
            }

            //yield return card.transform.parent.GetComponent<Hand_Slot>().decks.RemoveCard(card.transform.parent.GetComponent<Hand_Slot>());


            resolveStack.Remove(card);
        }
    }

    public IEnumerator Blink()
    {
        raycastBlocker.SetActive(true);
        yield return null;
        raycastBlocker.SetActive(false);
    }
}
