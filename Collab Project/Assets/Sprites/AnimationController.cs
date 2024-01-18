using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationController : MonoBehaviour
{
    public void Clip(string trigger)
    {
        GetComponent<Animator>().Play(trigger);
    }
}
