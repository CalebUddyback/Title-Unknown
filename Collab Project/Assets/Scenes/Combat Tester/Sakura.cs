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
        attackChoice.GetOutcome();

        yield return MoveInRange(attackChoice.range);

        animationController.Clip("Sakura Punch");

        yield return WaitForKeyFrame();
        yield return ApplyOutcome();

        yield return animationController.coroutine;

        animationController.Clip("Sakura Idle");
    }

    IEnumerator Uppercut()
    {
        attackChoice.GetOutcome();

        yield return MoveInRange(attackChoice.range);

        animationController.Clip("Sakura Uppercut");

        yield return WaitForKeyFrame();
        yield return ApplyOutcome();

        yield return animationController.coroutine;

        animationController.Clip("Sakura Idle");
    }

    IEnumerator Kick()
    {
        attackChoice.GetOutcome();

        yield return MoveInRange(attackChoice.range);

        animationController.Clip("Sakura Kick");

        yield return WaitForKeyFrame();
        yield return ApplyOutcome();

        yield return animationController.coroutine;

        animationController.Clip("Sakura Idle");
    }

    IEnumerator Combo()
    {
        yield return Punch();

        yield return Uppercut();

        yield return Kick();
    }

    IEnumerator Jump_Kick()
    {
        attackChoice.GetOutcome();

        yield return MoveInRange(attackChoice.range);


        float maxTime = 0.4f;

        GetComponent<Rigidbody>().isKinematic = true;

        animationController.Clip("Sakura Jump");

        StartCoroutine(JumpInRange(new Vector3(-0.3f, 0.1f, 0), maxTime));

        yield return new WaitForSeconds(0.2085f);

        //gameObject.GetComponent<AnimationController>().Clip("Sakura Fall");

        //yield return new WaitForSeconds((maxTime / 2) - 0.1665f);

        animationController.Clip("Sakura Jump Kick");

        yield return WaitForKeyFrame();
        yield return ApplyOutcome();

        GetComponent<Rigidbody>().isKinematic = false;

        yield return animationController.coroutine;

        //gameObject.GetComponent<AnimationController>().Clip("Sakura Fall");

        yield return new WaitUntil(() => GetComponent<Rigidbody>().velocity.y > -0.1f);

        animationController.Clip("Sakura Landing");

        yield return animationController.coroutine;
    }

    IEnumerator Throw_Kunai()
    {
        attackChoice.GetOutcome();

        animationController.Clip("Sakura Kunai");

        yield return WaitForKeyFrame();

        GameObject kunai = Instantiate(kunaiPrefab, animationController.instatiatePoint.position, Quaternion.identity);

        yield return ProjectileArch(kunai.transform, new Vector3(-0.1f, 0.2f, 0), 0.4f);

        Destroy(kunai);

        yield return ApplyOutcome();

        animationController.Clip("Sakura Idle");

    }

    public IEnumerator ProjectileArch(Transform instance, Vector3 range, float maxTime)
    {
        /* This Method Works for Flying enemies */


        Vector3 startPos = instance.position;
        Vector3 targetPos = enemy.position + range;

        float archHeight = 0.1f;

        float timer = 0;

        while (timer < maxTime)
        {

            float x0 = startPos.x;
            float x1 = targetPos.x;
            float dist = x1 - x0;

            float nextX = Mathf.Lerp(startPos.x, targetPos.x, timer / maxTime);
            float baseY = Mathf.Lerp(startPos.y, targetPos.y, timer / maxTime);
            float arc = archHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
            Vector3 nextPos = new Vector3(nextX, baseY + arc, instance.position.z);

            instance.rotation = LookAt2D(nextPos - instance.position);
            instance.position = nextPos;

            timer += Time.deltaTime;

            yield return null;
        }

        instance.position = targetPos;

        yield return null;

        Quaternion LookAt2D(Vector2 forward)
        {
            return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
        }
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
    }
}

