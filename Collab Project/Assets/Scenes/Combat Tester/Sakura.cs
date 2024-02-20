using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sakura : Combat_Character
{
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
        };
    }

    IEnumerator Punch()
    {
        gameObject.GetComponent<AnimationController>().Clip("Sakura Punch");

        yield return Impact();

        yield return gameObject.GetComponent<AnimationController>().coroutine;

        gameObject.GetComponent<AnimationController>().Clip("Sakura Idle");

        yield return null;
    }

    IEnumerator Uppercut()
    {
        gameObject.GetComponent<AnimationController>().Clip("Sakura Uppercut");

        yield return Impact();

        yield return gameObject.GetComponent<AnimationController>().coroutine;

        gameObject.GetComponent<AnimationController>().Clip("Sakura Idle");

        yield return null;
    }

    IEnumerator Kick()
    {
        gameObject.GetComponent<AnimationController>().Clip("Sakura Kick");

        yield return Impact();

        yield return gameObject.GetComponent<AnimationController>().coroutine;

        gameObject.GetComponent<AnimationController>().Clip("Sakura Idle");

        yield return null;
    }

    IEnumerator Combo()
    {
        yield return Punch();

        yield return Uppercut();

        yield return Kick();

        yield return null;
    }

    IEnumerator Jump_Kick()
    {

        float maxTime = 0.4f;

        GetComponent<Rigidbody>().isKinematic = true;

        gameObject.GetComponent<AnimationController>().Clip("Sakura Jump");

        StartCoroutine(JumpInRange(new Vector3(-0.3f, 0.1f, 0), maxTime));

        yield return new WaitForSeconds(0.2085f);

        //gameObject.GetComponent<AnimationController>().Clip("Sakura Fall");

        //yield return new WaitForSeconds((maxTime / 2) - 0.1665f);

        gameObject.GetComponent<AnimationController>().Clip("Sakura Jump Kick");

        yield return Impact();

        GetComponent<Rigidbody>().isKinematic = false;

        yield return gameObject.GetComponent<AnimationController>().coroutine;

        //gameObject.GetComponent<AnimationController>().Clip("Sakura Fall");

        yield return new WaitUntil(() => GetComponent<Rigidbody>().velocity.y > -0.1f);

        gameObject.GetComponent<AnimationController>().Clip("Sakura Landing");


        // wait until landed 

        yield return new WaitForSeconds(1);

        yield return null;
    }

}

