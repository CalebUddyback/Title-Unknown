using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Training_Dummy : Combat_Character
{
    private void Awake()
    {
        health = 100;

        mana = 0;
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

        yield return MoveAmount(new Vector3(0.3f * -Facing, 0, 0));

        yield return animationController.coroutine;
        animationController.Clip("Sakura Idle");
    }

    public override IEnumerator CpuDecisionMaking()
    {
        EndTurn();

        yield return null;
    }
}

