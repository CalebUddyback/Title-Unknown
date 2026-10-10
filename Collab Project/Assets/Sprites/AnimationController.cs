using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class AnimationController : MonoBehaviour
{
    public Coroutine coroutine;

    public bool keyFrame = false;
    public bool endFrame = false;

    public Transform instatiatePoint;

    public void Clip(string trigger, float playSpeed)
    {
        coroutine = StartCoroutine(Playing(trigger));
        PlaySpeed(playSpeed);
    }

    public void PlaySpeed(float s)
    {
        GetComponent<Animator>().speed = s;
    }

    IEnumerator Playing(string trigger)
    {
        GetComponent<Animator>().Play(trigger, -1, 0);

        yield return null;

        yield return new WaitUntil(() => endFrame == true);

        yield return null;

        endFrame = false;
    }

    public void KeyFrame()
    {
        keyFrame = true;
    }

    public void EndFrame()
    {
        endFrame = true;
    }
}
