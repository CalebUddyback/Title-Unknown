using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public Coroutine coroutine;

    public bool eventFrame = false;

    public Transform instatiatePoint;

    private void FixedUpdate()
    {
        //gameObject.GetComponent<Animator>().SetFloat("Velocity.y", gameObject.GetComponent<Rigidbody>().velocity.y);
    }

    public void Clip(string trigger)
    {
        GetComponent<Animator>().Play(trigger);

        print("Playing " + trigger);
        coroutine = StartCoroutine(Playing());

    }

    public void Pause()
    {
        GetComponent<Animator>().speed = 0;
    }

    public void Play()
    {
        GetComponent<Animator>().speed = 1;
    }

    IEnumerator Playing()
    {
        yield return null;      // THIS HAS TO BE HERE TO CLEAR LAST ANIMATION

        yield return new WaitUntil(() => GetComponent<Animator>().GetCurrentAnimatorStateInfo(0).normalizedTime > 1);
    }

    public void EventFrame()
    {
        print("Contact");
        eventFrame = true;
    }
}
