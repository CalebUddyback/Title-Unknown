using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System.Linq;

public class Turn_Controller : MonoBehaviour
{
    public Canvas left_Huds, right_Huds;

    public List<Combat_Character> left_Players = new List<Combat_Character>();

    public List<Combat_Character> right_Players = new List<Combat_Character>();

    public Transform left_Positions, right_Positions;

    public List<Combat_Character> all_Players;

    private List<Combat_Character> readyForActionList = new List<Combat_Character>();

    public enum Stage { TURN_START, TURN_END, ACTION_START, IMPACT, ACTION_END}

    public Combat_Character characterTurn;

    public DescriptionBox left_descriptionBox, right_descriptionBox;

    public Combat_Camera mainCamera;

    public TieBreaker Tie_Breaker;

    public bool comboState = false;

    public Combo_Counter combo_Counter;

    private void Start()
    {
        StartCoroutine(InitializeCharacters());
    }

    IEnumerator InitializeCharacters()
    {
        yield return null;

        left_Players.Clear();

        right_Players.Clear();

        foreach (Transform player in GameObject.Find("Left Players").transform)
            left_Players.Add(player.GetComponent<Combat_Character>());

        foreach (Transform player in GameObject.Find("Right Players").transform)
            right_Players.Add(player.GetComponent<Combat_Character>());


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

            character.character_Stats.health = character.character_Stats.max_Health; // remove this line for persistant stats
            character.Hud.healthBar.Initialize(character.character_Stats.health, character.character_Stats.max_Health); 

            character.character_Stats.mana = character.character_Stats.max_Mana;
            character.Hud.manaBar.Initialize(character.character_Stats.max_Mana, character.character_Stats.max_Mana);
        }

        for (int i = 0; i < left_Players.Count; i++)
        {
            Combat_Character character = left_Players[i];

             character.transform.position = character.startingPos = left_Positions.GetChild(i).position;
        }

        for (int i = 0; i < right_Players.Count; i++)
        {
            Combat_Character character = right_Players[i];

            character.transform.position = character.startingPos = right_Positions.GetChild(i).position;
            character.Facing = -1;
        }

        Tie_Breaker.PickSide(-1);

        foreach (Combat_Character character in all_Players)
        {
            character.Hud.timer_Numbers.text = "";

            character.Hud.timer_Numbers.rectTransform.sizeDelta = new Vector2(6, character.Hud.timer_Numbers.fontSize * 30);

            for (int i = 29; i >= 0; i--)
            {
                character.Hud.timer_Numbers.text += i + "\n";
            }

            yield return null;

            StartCoroutine(character.StartFocus());
        }


        yield return new WaitForSeconds(0.3f);


        yield return new WaitForSeconds(1f);

        Debug.Log("Turns Disabled");

        //StartCoroutine(RotateTurns());
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
            print("Screen shook By Host");
            mainCamera.Shake();
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
        bool tie = false;

        int lowestTime = 0;

        while (true)
        {

            if (readyForActionList.Count <= 0)
            {

                var ordered = all_Players.OrderBy(o => o.Hud.GetTimeLeft()).ToList();

                lowestTime = ordered[0].Hud.GetTimeLeft();

                ordered.RemoveAll(i => i.Hud.GetTimeLeft() > lowestTime);

                foreach (Combat_Character character in ordered)
                {
                    character.Hud.timer_Animations.Play("Pulser_Burst");
                    character.Hud.SetTimerColor(Color.white);
                    readyForActionList.Add(character);

                    yield return new WaitForSeconds(0.3f);
                }

                yield return new WaitForSeconds(1f);        // time to Allow for player to view lowest time indication 

                foreach (Combat_Character character in all_Players)
                {
                    if(!character.Defeated)
                        StartCoroutine(character.Hud.ScrollTimerTo(character.Hud.GetTimeLeft() - lowestTime));
                }

            }
            else
            {
                while (readyForActionList.Count > 0)
                {

                    List<Combat_Character> favouredList = Tie_Breaker.FavouredSide == -1 ? readyForActionList.Intersect(left_Players).ToList() : readyForActionList.Intersect(right_Players).ToList();
                    List<Combat_Character> unfavouredList = Tie_Breaker.FavouredSide == -1 ? readyForActionList.Intersect(right_Players).ToList() : readyForActionList.Intersect(left_Players).ToList();

                    if (favouredList.Count > 0 && unfavouredList.Count > 0 && !tie)
                    {
                        tie = true;
                        if (Tie_Breaker.FavouredSide == -1)
                            Tie_Breaker.Animation.Play("Right Blink");
                        else
                            Tie_Breaker.Animation.Play("Left Blink");
                    }

                    if (favouredList.Count > 0)
                        characterTurn = favouredList[0];
                    else
                        characterTurn = unfavouredList[0];


                    // Move Camera 

                    if (!comboState)
                    {
                        Vector3 camTargetPos = new Vector3(0, 0.55f, characterTurn.transform.position.z - 2.5f);

                        yield return characterTurn.TurnController.mainCamera.MovingTo(camTargetPos, 0.5f);

                        characterTurn.Hud.GetComponent<Animator>().Play("Turn_Indicate");

                        yield return new WaitForSeconds(0.5f);
                    }

                    CoroutineWithData cd = new CoroutineWithData(this, Reactions(Stage.TURN_START, null));
                    yield return cd.coroutine;

                    characterTurn.Hud.timer_Animations.Play("Pulser_Pulse");

                    characterTurn.StartTurn();

                    while (characterTurn.spotLight)
                    {
                        //characterTurn.MenuPositioning();
                        yield return null;
                    }

                    characterTurn.Hud.timer_Animations.Play("Pulser_Burst");

                    if (!characterTurn.chosenAction.charging)
                    {
                        yield return characterTurn.StartFocus();

                        ComboCheck();

                        yield return characterTurn.StartAttack();

                        yield return ResetPositions();

                        characterTurn.chosenAction = null;
                    }
                    else
                    {
                        yield return characterTurn.Charging();

                        ComboCheck();

                        yield return ResetPositions();
                    }

                    readyForActionList.Remove(characterTurn);

                    if (readyForActionList.Count > 0)
                    {
                        for (int i = 0; i < readyForActionList.Count; i++)
                        {
                            if (readyForActionList[i].Hud.GetTimeLeft() > 0)
                            {
                                print("KNOCK OUT");
                                readyForActionList.Remove(readyForActionList[i]);
                            }
                        }
                    }

                }

                yield return new WaitForSeconds(1f);
            }

            if (tie)
            {
                tie = false;
                Tie_Breaker.FlipSide();
            }

            yield return null;
        }
    }

    public bool ComboCheck()
    {
        comboState = true;

        foreach (Combat_Character character in all_Players)
        {
            if (character.Hud.GetTimeLeft() <= characterTurn.Hud.GetTimeLeft() && character != characterTurn)
            {
                comboState = false;
                break;
            }
        }

        return comboState;
    }

    [SerializeField]
    private List<Transform> comboTargetList;

    public IEnumerator ResetPositions()
    {

        comboTargetList.AddRange(characterTurn.chosenAction.targets.Distinct());

        if (!comboState)
        {
            // Reset involved characters animations

            characterTurn.animationController.Clip(characterTurn.characterName + " Idle");

            foreach (Transform target in comboTargetList)
                target.GetComponent<Combat_Character>().animationController.Clip(characterTurn.characterName + " Idle");

            yield return new WaitForSeconds(0.4f);

            left_descriptionBox.container.SetActive(false);
            right_descriptionBox.container.SetActive(false);

            combo_Counter.ResetComboCount();

            characterTurn.Hud.GetComponent<Animator>().Play("Turn_Indicate_End");

            // Reset Camera

            mainCamera.BlackOut(0f, 0.5f);

            Coroutine cam = StartCoroutine(mainCamera.Reset(0.2f));


            //  Reset Involved Characters

            Coroutine charReset = StartCoroutine(characterTurn.ResetPos());

            List<Coroutine> targetResets = new List<Coroutine>();

            foreach (Transform target in comboTargetList)
                targetResets.Add(StartCoroutine(target.GetComponent<Combat_Character>().ResetPos()));

            yield return charReset;

            foreach (Coroutine target in targetResets)
                yield return target;

            yield return cam;

            comboTargetList.Clear();
        }
        else
        {
            mainCamera.BlackOut(0.9f, 0.5f);
        }

        characterTurn.chosenAction.targets.Clear();
    }


    /// <summary>
    /// This Method should be called by Turn Character's stages. 
    /// This Method should alow one reaction from the opposing team, then the Turn Character's Team (including Turn player). And continue ping-ponging
    /// When its time to React to a Reaction, all stored reactions should be taken into concideration untill no reaction can/will be made
    /// Reactions should then be Resolved backwards untill the root action (if not negated).
    /// </summary>
    /// <param name="stage"></param>
    /// <param name="info"></param>
    /// <returns></returns>
    /// 

    public IEnumerator Reactions(Stage stage, Combat_Character.Skill info)
    {
        foreach (Combat_Character character in all_Players)
        {
            List<string> labels = new List<string>();
            List<int> indexs = new List<int>();

            for (int i = 0; i < character.setSpells.Length; i++)
            {
                if (character.setSpells[i] == null || character.setSpells[i].Condition(stage, info) == false)
                    continue;
                else
                {
                    labels.Add(character.setSpells[i].name);
                    indexs.Add(i);
                }
            }

            if (labels.Count == 0)
            {
                yield return 0;
                continue;
            }

            foreach (Combat_Character c in all_Players)
                c.animationController.Pause();

            //character.MenuPositioning();

            yield return character.SubMenuController.OpenSubMenu("Prompts", labels);

            yield return character.SubMenuController.CurrentCD.coroutine;

            foreach (Combat_Character c in all_Players)
                c.animationController.Play();

            if (character.SubMenuController.CurrentSubMenu.ButtonChoice == -1)
            {
                continue;
            }
            else
            {
                character.SubMenuController.ResetMenus();

                character.animationController.Clip(character.characterName + " Idle");
                yield return null;
                CoroutineWithData cd = new CoroutineWithData(this, character.setSpells[indexs[character.SubMenuController.CurrentSubMenu.ButtonChoice]].Action2(indexs[character.SubMenuController.CurrentSubMenu.ButtonChoice]));
                yield return cd.coroutine;

                yield return (int)cd.result;
                break;
            }
        }
    }
}
