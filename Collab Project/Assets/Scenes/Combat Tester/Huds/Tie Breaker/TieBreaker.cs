using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TieBreaker : MonoBehaviour
{
    [HideInInspector]
    public Animation Animation => GetComponent<Animation>();

    public int FavouredSide { get; private set; }

    public void PickSide()
    {
        FavouredSide = Random.Range(0, 2) == 0 ? -1 : 1;

        if (FavouredSide == -1)
            Animation.Play("Left Glow");
        else
            Animation.Play("Right Glow");
    }

    public void PickSide(int side)
    {
        FavouredSide = side;

        if (FavouredSide == -1)
            Animation.Play("Left Glow");
        else
            Animation.Play("Right Glow");
    }

    public void FlipSide()
    {
        FavouredSide *= -1;

        if (FavouredSide == -1)
            Animation.Play("Left Glow");
        else
            Animation.Play("Right Glow");
    }
}
