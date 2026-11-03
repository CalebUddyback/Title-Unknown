using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC_Patrol : MonoBehaviour
{
    public NPC npc;

    public Transform route;

    List<Vector3> node = new List<Vector3>();

    public float delay = 3;

    // Start is called before the first frame update
    void Start()
    {
        node.Add(npc.animationController.transform.position);

        foreach (Transform n in route)
        {
            node.Add(n.position);
        }

        StartCoroutine(Patrol());
    }


    IEnumerator Patrol()
    {

        int i = 1;

        float stopThreshold = 0.01f;

        while (true)
        {
            npc.animationController.Clip("Idle", 1);

            yield return new WaitForSeconds(delay);

            Vector3 targetPos = node[i];

            npc.animationController.Clip("Walk", 1);

            float dir = npc.transform.position.x - targetPos.x < 0 ? -1 : 1;

            npc.animationController.transform.localScale = new Vector3(dir, 1, 1);

            while (Vector3.Distance(npc.animationController.transform.position, targetPos) > stopThreshold)
            {
                npc.transform.position = Vector3.MoveTowards(npc.transform.position, targetPos, npc.walkSpeed / 100f);

                yield return null;
            }

            if (i + 1 > node.Count - 1)
                i = 0;
            else
                i++;
        }

    }
}
