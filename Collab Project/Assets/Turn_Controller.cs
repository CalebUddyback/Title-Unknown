using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn_Controller : MonoBehaviour
{
    public List<Combat_Character> players = new List<Combat_Character>();

    public Transform progession;

    public Transform startPoint, endPoint;

    private void Start()
    {
        StartCoroutine(ProgressionBar());
    }

    IEnumerator ProgressionBar()
    {
        float timer = 0;
        float speed = 1f;

        while(timer < speed)
        {
            float lerp = Mathf.Lerp(startPoint.position.x, endPoint.position.x, timer / speed);

            progession.position = new Vector3(lerp, progession.position.y, progession.position.z);

            timer += Time.deltaTime;
            yield return null;
        }

        progession.position = new Vector3(endPoint.position.x, progession.position.y, progession.position.z);

        players[0].StartTurn();
    }
}
