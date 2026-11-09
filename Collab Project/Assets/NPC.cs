using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPC : MonoBehaviour
{
    public AnimationController animationController;

    public Transform faceToward;

    public Transform follow;

    private float followDistance = 0.4f;

    private float stopDistance = 0.25f;

    [HideInInspector]
    public float walkSpeed = 0.3f;

    private void Start()
    {
        if (follow != null)
        {
            StartCoroutine(Following());
        }
    }

    private void Update()
    {
        if(faceToward != null)
        {
            float dir = faceToward.position.x < transform.position.x ? -1 : 1;

            animationController.transform.localScale = new Vector3(dir, 1, 1);
        }
    }

    IEnumerator Following()
    {
        while (true)
        {
            animationController.Clip("Idle", 1);

            yield return new WaitUntil(() => Vector3.Distance(transform.position, follow.position) > followDistance);

            float dir = follow.position.x < transform.position.x ? -1 : 1;

            animationController.transform.localScale = new Vector3(dir, 1, 1);

            animationController.Clip("Walk", 1);

            while (Vector3.Distance(transform.position, follow.position) > stopDistance)
            {
                transform.position = Vector3.MoveTowards(transform.position, follow.position, walkSpeed / 100f);

                yield return null;
            }

        }
    }
}
