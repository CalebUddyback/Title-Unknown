using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AnimationController : MonoBehaviour
{
    public Coroutine coroutine;

    public bool eventFrame = false;

    public Transform instatiatePoint;

    public void Clip(string trigger)
    {
        if (!trigger.Contains("Idle"))
            coroutine = StartCoroutine(Playing(trigger));
        else
            StartCoroutine(Playing(trigger));
    }

    public void Pause()
    {
        GetComponent<Animator>().speed = 0;
    }

    public void Play()
    {
        GetComponent<Animator>().speed = 1;
    }

    IEnumerator Playing(string trigger)
    {
        GetComponent<Animator>().SetTrigger("Reset");

        yield return null;      // THIS HAS TO BE HERE TO CLEAR LAST ANIMATION

        GetComponent<Animator>().Play(trigger);

        yield return new WaitWhile(() => GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime <= 1);
    }

    public void EventFrame()
    {
        eventFrame = true;
    }
}
