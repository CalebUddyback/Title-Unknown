using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Turn_Controller : MonoBehaviour
{
    public Canvas left_Huds, right_Huds;

    public List<Combat_Character> left_Players = new List<Combat_Character>();

    public List<Combat_Character> right_Players = new List<Combat_Character>();

    public Transform left_Positions, right_Positions;

    private List<Combat_Character> all_Players;

    private Queue<Combat_Character> turnQueue = new Queue<Combat_Character>();

    public enum Stage { TURN_START, TURN_END, ACTION_START, IMPACT, ACTION_END}

    public Combat_Character characterTurn;

    public DescriptionBox descriptionBox;

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

        foreach (Combat_Character character in all_Players)
        {
            character.TurnController = this;

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
            character.StartFocus();
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


        StartCoroutine(RotateTurns());
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
        while (true)
        {
            if (turnQueue.Count <= 0)
            {
                float globalDelta = Time.deltaTime;

                foreach (Combat_Character character in all_Players)
                {
                    character.Hud.IncrementTimer(globalDelta);

                    if (character.Hud.GetTimeLeft() <= 0)
                    {
                        turnQueue.Enqueue(character);
                        character.Hud.EndTimer();
                    }
                }
            }
            else
            {
                foreach (Combat_Character character in all_Players)
                {
                    character.Hud.timer_Progress = Mathf.Ceil((character.Hud.timer_Progress) * 10f) / 10f;
                }

                while (turnQueue.Count > 0)
                {

                    characterTurn = turnQueue.Dequeue();

                    characterTurn.Hud.SetTimerColor(Color.white);

                    CoroutineWithData cd = new CoroutineWithData(this, Reactions(Stage.TURN_START, null));
                    yield return cd.coroutine;

                    // Move Camera 

                    Vector3 camTargetPos = new Vector3(0, 0.55f, characterTurn.transform.position.z - 2.5f);

                    yield return characterTurn.mcamera.GetComponent<MainCamera>().LerpMoveIE(camTargetPos, 0.5f);

                    characterTurn.StartTurn();


                    while (characterTurn.spotLight)
                    {
                        characterTurn.MenuPositioning();
                        yield return null;
                    }

                    // Reset Camera

                    //yield return characterTurn.mcamera.GetComponent<MainCamera>().Reset(0f);

                    if (!characterTurn.chosenAttack.charging)
                    {
                        yield return characterTurn.StartAttack();

                        characterTurn.StartFocus();

                        characterTurn.chosenAttack = null;
                    }
                    else
                    {
                        StartCoroutine(characterTurn.Charging(1f));
                    }

                    if (turnQueue.Count > 0 && turnQueue.Peek().Hud.GetTimeLeft() != 0)
                    {
                        print("KNOCK OUT");
                        turnQueue.Dequeue();
                    }
                }
            }

            yield return null;
        }
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

    public IEnumerator Reactions(Stage stage, Combat_Character.Skill.Info info)
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

            character.MenuPositioning();

            yield return character.SubMenuController.OpenSubMenu("Prompts", labels);

            foreach (Combat_Character c in all_Players)
                c.animationController.Play();

            if (character.SubMenuController.CurrentSubMenu.ButtonChoice == -1)
            {
                continue;
            }
            else
            {
                character.SubMenuController.ResetMenus();

                CoroutineWithData cd = new CoroutineWithData(this, character.setSpells[indexs[character.SubMenuController.CurrentSubMenu.ButtonChoice]].Action2(indexs[character.SubMenuController.CurrentSubMenu.ButtonChoice]));
                yield return cd.coroutine;

                yield return (int)cd.result;
                break;
            }
        }
    }
}
