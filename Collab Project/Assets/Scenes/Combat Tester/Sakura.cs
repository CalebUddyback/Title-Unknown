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
            new Attack("Combo", nameof(Combo))
            {
                chargeTime = 1f,
                maxCharges = 3,

                requiredMenus = new string[]
                {
                    "Charges",
                    "Confirm"
                },

                info = new Attack.Info[]
                {
                    new Attack.Info(5),
                    new Attack.Info(7),
                    new Attack.Info(10)
                }
            },

            new Attack("Jump Kick", nameof(Jump_Kick))
            {

                requiredMenus = new string[]
                {
                    "Confirm"
                },

                info = new Attack.Info[]
                {
                    new Attack.Info(18),
                }
            },

            new Attack("Throw Kunai", nameof(Throw_Kunai))
            {
                requiredMenus = new string[]
                {
                    "Confirm"
                },

                info = new Attack.Info[]
                {
                    new Attack.Info(5),
                }
            },

            new Attack("Multi Hit", nameof(Multi_Hit))
            {
                chargeTime = 1,

                requiredMenus = new string[]
                {
                    "Confirm"
                },

                info = new Attack.Info[]
                {
                    new Attack.Info(5),
                    new Attack.Info(5),
                    new Attack.Info(5),
                    new Attack.Info(7)
                }
            },
        };
    }

    IEnumerator Combo(int[] input)
    {
        int charge = input[0];

        attack.GetOutcome();

        yield return MoveInRange(new Vector3(-0.35f, 0, 0));

        animationController.Clip("Sakura Punch");

        yield return WaitForKeyFrame();
        Coroutine outcome = StartCoroutine(ApplyOutcome(attack.info[0]));

        yield return animationController.coroutine;
        animationController.Clip("Sakura Idle");

        yield return outcome;

        if (charge == 0)
            yield break;

        // two

        attack.GetOutcome();

        yield return MoveInRange(new Vector3(-0.35f, 0, 0));

        animationController.Clip("Sakura Uppercut");

        yield return WaitForKeyFrame();
        outcome = StartCoroutine(ApplyOutcome(attack.info[1]));

        yield return animationController.coroutine;
        animationController.Clip("Sakura Idle");

        yield return outcome;

        if (charge == 1)
            yield break;

        // three

        attack.GetOutcome();

        yield return MoveInRange(new Vector3(-0.35f, 0, 0));

        animationController.Clip("Sakura Kick");

        yield return WaitForKeyFrame();
        outcome = StartCoroutine(ApplyOutcome(attack.info[2]));

        yield return animationController.coroutine;
        animationController.Clip("Sakura Idle");

        yield return outcome;
    }

    IEnumerator Jump_Kick()
    {
        attack.GetOutcome();

        yield return MoveInRange(new Vector3(-1f, 0, 0));


        float maxTime = 0.4f;

        GetComponent<Rigidbody>().isKinematic = true;

        animationController.Clip("Sakura Jump");

        StartCoroutine(JumpInRange(new Vector3(-0.3f, 0.1f, 0), maxTime));

        yield return new WaitForSeconds(0.2085f);

        //gameObject.GetComponent<AnimationController>().Clip("Sakura Fall");

        //yield return new WaitForSeconds((maxTime / 2) - 0.1665f);

        animationController.Clip("Sakura Jump Kick");

        yield return WaitForKeyFrame();
        Coroutine outcome = StartCoroutine(ApplyOutcome(attack.info[0]));

        if (attack.Success != 0)
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
        attack.GetOutcome();

        yield return MoveInRange(new Vector3(-1.75f, 0, 0));

        animationController.Clip("Sakura Kunai");

        yield return WaitForKeyFrame();

        GameObject kunai = Instantiate(kunaiPrefab, animationController.instatiatePoint.position, Quaternion.identity);

        //yield return ProjectileArch(kunai.transform, new Vector3(-0.1f, 0.2f, 0), 0.4f);

        Coroutine tragectory = StartCoroutine(ProjectileArch(kunai.transform, new Vector3(0.8f, 0f, 0f), 0.4f));

        yield return new WaitWhile(() => kunai.transform.position.x <= enemyTransform.position.x);


        if (attack.Success != 0)
        {
            StopCoroutine(tragectory);
            Destroy(kunai);
        }

        Coroutine outcome = StartCoroutine(ApplyOutcome(attack.info[0]));

        yield return tragectory;

        Destroy(kunai, 2);

        animationController.Clip("Sakura Idle");

        yield return outcome;

    }

    IEnumerator Multi_Hit()
    {
        attack.GetOutcome();

        yield return MoveInRange(new Vector3(-0.35f, 0, 0));

        animationController.Clip("Sakura Multi Hit");

        yield return WaitForKeyFrame();

        Coroutine outcome = StartCoroutine(ApplyOutcome(attack.info[0]));

        for (int i = 1; i < 4; i++)
        {
            yield return WaitForKeyFrame();

            if (attack.Success != 0)
                outcome = StartCoroutine(ApplyOutcome(attack.info[i]));
        } 

        yield return animationController.coroutine;
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

