using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sakura : Combat_Character
{
    public GameObject kunaiPrefab;

    private void Awake()
    {
        attackList = new List<Attack>()
        {
            new Attack("Punch", nameof(Punch))
            {
                damage = 10,
                range =  new Vector3(-0.35f, 0, 0)
            },

            new Attack("Uppercut", nameof(Uppercut))
            {
                damage = 10,
                range =  new Vector3(-0.35f, 0, 0)
            },

            new Attack("Kick", nameof(Kick))
            {
                damage = 10,
                range =  new Vector3(-0.35f, 0, 0)
            },

            new Attack("Dragon Crossing", nameof(Combo))
            {
                damage = 10,
                range =  new Vector3(-0.35f, 0, 0)
            },

            new Attack("Jump Kick", nameof(Jump_Kick))
            {
                damage = 10,
                range =  new Vector3(-1f, 0, 0)
            },

            new Attack("Throw Kunai", nameof(Throw_Kunai))
            {
                damage = 10,
            },
        };
    }

    IEnumerator Punch()
    {
        attackInfo.GetOutcome();

        yield return MoveInRange(attackInfo.range);

        animationController.Clip("Sakura Punch");

        yield return WaitForKeyFrame();
        Coroutine outcome = StartCoroutine(ApplyOutcome());

        yield return animationController.coroutine;
        animationController.Clip("Sakura Idle");

        yield return outcome;
    }

    IEnumerator Uppercut()
    {
        attackInfo.GetOutcome();

        yield return MoveInRange(attackInfo.range);

        animationController.Clip("Sakura Uppercut");

        yield return WaitForKeyFrame();
        Coroutine outcome = StartCoroutine(ApplyOutcome());

        yield return animationController.coroutine;
        animationController.Clip("Sakura Idle");

        yield return outcome;
    }

    IEnumerator Kick()
    {
        attackInfo.GetOutcome();

        yield return MoveInRange(attackInfo.range);

        animationController.Clip("Sakura Kick");

        yield return WaitForKeyFrame();
        Coroutine outcome = StartCoroutine(ApplyOutcome());

        yield return animationController.coroutine;
        animationController.Clip("Sakura Idle");

        yield return outcome;
    }

    IEnumerator Combo()
    {
        int luck = Random.Range(0, 7);

        for (int i = 0; i <= luck; i++)
        {
            switch (i)
            {
                case 0:
                    attackInfo = attackList[0];
                    yield return Punch();
                    break;
                case 1:
                    attackInfo = attackList[1];
                    yield return Uppercut();
                    break;
                case 2:
                    attackInfo = attackList[2];
                    yield return Kick();
                    break;
                case 3:
                    attackInfo = attackList[0];
                    yield return Punch();
                    break;
                case 4:
                    attackInfo = attackList[0];
                    yield return Punch();
                    break;
                case 5:
                    attackInfo = attackList[2];
                    yield return Kick();
                    break;
                case 6:
                    attackInfo = attackList[4];
                    yield return Jump_Kick();
                    break;
                default:
                    Debug.Break();
                    break;
            }
        }
    }

    IEnumerator Jump_Kick()
    {
        attackInfo.GetOutcome();

        yield return MoveInRange(attackInfo.range);


        float maxTime = 0.4f;

        GetComponent<Rigidbody>().isKinematic = true;

        animationController.Clip("Sakura Jump");

        StartCoroutine(JumpInRange(new Vector3(-0.3f, 0.1f, 0), maxTime));

        yield return new WaitForSeconds(0.2085f);

        //gameObject.GetComponent<AnimationController>().Clip("Sakura Fall");

        //yield return new WaitForSeconds((maxTime / 2) - 0.1665f);

        animationController.Clip("Sakura Jump Kick");

        yield return WaitForKeyFrame();
        Coroutine outcome = StartCoroutine(ApplyOutcome());

        if (attackInfo.Success != 0)
            yield return outcome;

        GetComponent<Rigidbody>().isKinematic = false;

        yield return animationController.coroutine;

        //gameObject.GetComponent<AnimationController>().Clip("Sakura Fall");

        yield return new WaitUntil(() => GetComponent<Rigidbody>().velocity.y > -0.1f);

        animationController.Clip("Sakura Landing");

        yield return animationController.coroutine;

        yield return outcome;
    }

    IEnumerator Throw_Kunai()
    {
        attackInfo.GetOutcome();

        animationController.Clip("Sakura Kunai");

        yield return WaitForKeyFrame();

        GameObject kunai = Instantiate(kunaiPrefab, animationController.instatiatePoint.position, Quaternion.identity);

        //yield return ProjectileArch(kunai.transform, new Vector3(-0.1f, 0.2f, 0), 0.4f);

        Coroutine tragectory = StartCoroutine(ProjectileArch(kunai.transform, new Vector3(0.8f, 0f, 0f), 0.4f));

        yield return new WaitWhile(() => kunai.transform.position.x <= enemyTransform.position.x);


        if (attackInfo.Success != 0)
        {
            StopCoroutine(tragectory);
            Destroy(kunai);
        }

        Coroutine outcome = StartCoroutine(ApplyOutcome());

        yield return tragectory;

        Destroy(kunai, 2);

        animationController.Clip("Sakura Idle");

        yield return outcome;

    }


    public override IEnumerator Damage()
    {
        animationController.Clip("Sakura Idle");
        yield return null;
        animationController.Clip("Sakura Damaged");
        yield return animationController.coroutine;
    }

    public override IEnumerator Block()
    {
        animationController.Clip("Sakura Idle");
        yield return null;
        animationController.Clip("Sakura Block");
        yield return animationController.coroutine;
    }

    public override IEnumerator Dodge()
    {
        animationController.Clip("Sakura Idle");
        yield return null;
        animationController.Clip("Sakura Dodge");

        yield return MoveAmount(new Vector3(0.3f,0,0));

        yield return animationController.coroutine;
        animationController.Clip("Sakura Idle");
    }
}

