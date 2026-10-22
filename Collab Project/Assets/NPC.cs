using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public AnimationController animationController;

    public Transform patrolNodes;

    List<Vector3> nodes = new List<Vector3>();

    public float walkSpeed = 1;

    public float delay = 3;


    private void Start()
    {
        nodes.Add(animationController.transform.position);

        foreach (Transform n in patrolNodes)
        {
            nodes.Add(n.position);
        }

        StartCoroutine(Patrol());
    }


    IEnumerator Patrol()
    {

        int i = 1;

        while (true)
        {
            animationController.Clip("Idle", 1);

            yield return new WaitForSeconds(delay);

            Vector3 targetPos = nodes[i];

            animationController.Clip("Walk", 1);

            float dir = animationController.transform.position.x - targetPos.x < 0 ? -1 : 1;

            Debug.Log(dir);

            animationController.transform.localScale = new Vector3(dir,1,1);

            while (animationController.transform.position != targetPos)
            {
                animationController.transform.position = Vector3.MoveTowards(animationController.transform.position, targetPos, walkSpeed/100f);

                yield return null;
            }

            animationController.transform.position = targetPos;

            if (i + 1 > nodes.Count - 1)
                i = 0;
            else
                i++;
        }

    }
}
