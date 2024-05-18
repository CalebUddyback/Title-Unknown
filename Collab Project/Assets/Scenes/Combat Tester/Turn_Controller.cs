using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn_Controller : MonoBehaviour
{
    public GameObject leftCharaterHudPrefab, rightCharaterHudPrefab;

    public Canvas left_Huds, right_Huds;

    public List<Combat_Character> left_Players = new List<Combat_Character>();

    public List<Combat_Character> right_Players = new List<Combat_Character>();

    public Transform left_Positions, right_Positions;

    private List<Combat_Character> allPlayers;

    private Queue<Combat_Character> turnQueue = new Queue<Combat_Character>();

    private Queue<Combat_Character> actionQueue = new Queue<Combat_Character>();

    private void Start()
    {
        StartCoroutine(InitializeCharacters());
    }

    IEnumerator InitializeCharacters()
    {
        yield return null;

        allPlayers = new List<Combat_Character>(left_Players);
        allPlayers.AddRange(right_Players);

        foreach (Combat_Character character in allPlayers)
        {
            character.TurnController = this;

            if(left_Players.Contains(character))
                character.Hud = Instantiate(leftCharaterHudPrefab.GetComponent<Character_Hud>(), left_Huds.transform);
            else
                character.Hud = Instantiate(rightCharaterHudPrefab.GetComponent<Character_Hud>(), right_Huds.transform);

            character.Hud.diplayName.text = character.gameObject.name;
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

    public void AddToTurnQueue(Combat_Character character)
    {
        turnQueue.Enqueue(character);
    }

    public void AddToActionQueue(Combat_Character character)
    {
        actionQueue.Enqueue(character);
    }

    IEnumerator RotateTurns()
    {
        while (true)
        {
            if (actionQueue.Count > 0)
            {
                Combat_Character character = actionQueue.Dequeue();

                foreach (Combat_Character c in allPlayers)
                {
                    c.ToggleTurnTime();
                }

                yield return character.StartAttack();


                // Make timer BLUE

                character.Hud.timer.color = Color.blue;

                foreach (Combat_Character c in allPlayers)
                {
                    c.ToggleTurnTime();
                }
            }


            if (turnQueue.Count > 0)
            {
                // Stop TurnTimers

                foreach (Combat_Character c in allPlayers)
                {
                    c.ToggleTurnTime();
                }

                Combat_Character character = turnQueue.Dequeue();


                // Make timer WHITE

                character.Hud.timer.color = Color.white;

                // Move Camera 

                Vector3 targetPos = new Vector3(0, 0.618f, character.transform.position.z - 2.5f);

                yield return character.mcamera.GetComponent<MainCamera>().LerpMove(targetPos, 0.5f);


                character.StartTurn();


                yield return new WaitUntil(() => !character.turn);

                // Make timer RED

                character.Hud.timer.color = Color.red;

                // Reset Camera

                targetPos = new Vector3(0, 0.618f, -2.5f);

                yield return character.mcamera.GetComponent<MainCamera>().LerpMove(targetPos, 0.5f);

                // Continue TurnTimers

                foreach (Combat_Character c in allPlayers)
                {
                    c.ToggleTurnTime();
                }
            }

            yield return null;
        }
    }
}
