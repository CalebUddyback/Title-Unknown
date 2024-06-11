using System.Collections;
using System.Collections.Generic;
using System.Linq;
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

                requiredMenus = new Attack.RequiredMenu[]
                {
                    new Attack.RequiredMenu("Charges"),
                    new Attack.RequiredMenu("Targets", "Charges"),
                },

                info = new Attack.Info[]
                {
                    new Attack.Info(5),
                    new Attack.Info(7),
                    new Attack.Info(10)
                },


            },

            new Attack("Jump Kick", nameof(Jump_Kick))
            {
                chargeTime = 1f,

                info = new Attack.Info[]
                {
                    new Attack.Info(18),
                }
            },

            new Attack("Throw Kunai", nameof(Throw_Kunai))
            {


                info = new Attack.Info[]
                {
                    new Attack.Info(5),
                }
            },

            new Attack("Multi Hit", nameof(Multi_Hit))
            {
                chargeTime = 1,

 

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

    private void Start()
    {
       
    }

    IEnumerator Combo()
    {
        bool done = false;

        int charge = 0;

        int i = 0;

        while (!done)
        {

            switch (i)
            {

                case 0:

                    yield return SubMenuController.OpenSubMenu("Charges", Enumerable.Range(1, attack.maxCharges).Select(n => n.ToString()).ToList());

                    if (SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                    {
                        i++;

                        charge = SubMenuController.CurrentSubMenu.ButtonChoice;
                    }
                    else
                    {
                        attack = null;

                        yield break;
                    }


                    break;

                case 1:

                    if (targets.Count > 0)
                        targets.RemoveAt(targets.Count - 1);

                    while (true)
                    {
                        yield return SubMenuController.OpenSubMenu("Targets", TurnController.GetPlayerNames(Facing));

                        if (SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {

                            if (Facing == 1)
                                targets.Add(TurnController.right_Players[SubMenuController.CurrentSubMenu.ButtonChoice].transform);
                            else
                                targets.Add(TurnController.right_Players[SubMenuController.CurrentSubMenu.ButtonChoice].transform);

                            if (targets.Count > charge)
                            {
                                i++;
                                break;
                            }

                        }
                        else
                        {
                            if (targets.Count <= 0)
                            {
                                i--;
                                break;
                            }
                            else
                            {
                                targets.RemoveAt(targets.Count - 1);
                            }
                        }
                    }


                    break;

                case 2:

                    yield return SubMenuController.OpenSubMenu("Confirm", new List<string>() { "Confirm" });

                    if (SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                    {
                        done = true;
                    }
                    else
                    {
                        i--;
                    }

                    break;
            }

        }


        attack.Execute = Combo_Action(charge);
    }

    IEnumerator Combo_Action(int charge)
    {
        enemyTransform = targets[0];

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

        enemyTransform = targets[1];

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

        enemyTransform = targets[2];

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
        bool done = false;

        int i = 0;


        while (!done)
        {

            switch (i)
            {
                case 0:

                    if (targets.Count > 0)
                        targets.RemoveAt(targets.Count - 1);

  
                    yield return SubMenuController.OpenSubMenu("Targets", TurnController.GetPlayerNames(Facing));

                    if (SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                    {
                        if (Facing == 1)
                            targets.Add(TurnController.right_Players[SubMenuController.CurrentSubMenu.ButtonChoice].transform);
                        else
                            targets.Add(TurnController.right_Players[SubMenuController.CurrentSubMenu.ButtonChoice].transform);

                        i++;
                    }
                    else
                    {
                        attack = null;
                        yield break;
                    }

                    break;

                case 1:

                    yield return SubMenuController.OpenSubMenu("Confirm", new List<string>() { "Confirm" });

                    if (SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                    {
                        done = true;
                    }
                    else
                    {
                        i--;
                    }

                    break;
            }

        }


        attack.Execute = Jump_Kick_Action();
    }

    IEnumerator Jump_Kick_Action()
    {
        enemyTransform = targets[0];

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
        bool done = false;

        int i = 0;


        while (!done)
        {

            switch (i)
            {
                case 0:

                    if (targets.Count > 0)
                        targets.RemoveAt(targets.Count - 1);


                    yield return SubMenuController.OpenSubMenu("Targets", TurnController.GetPlayerNames(Facing));

                    if (SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                    {
                        if (Facing == 1)
                            targets.Add(TurnController.right_Players[SubMenuController.CurrentSubMenu.ButtonChoice].transform);
                        else
                            targets.Add(TurnController.right_Players[SubMenuController.CurrentSubMenu.ButtonChoice].transform);

                        i++;
                    }
                    else
                    {
                        attack = null;
                        yield break;
                    }

                    break;

                case 1:

                    yield return SubMenuController.OpenSubMenu("Confirm", new List<string>() { "Confirm" });

                    if (SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                    {
                        done = true;
                    }
                    else
                    {
                        i--;
                    }

                    break;
            }

        }


        attack.Execute = Throw_Kunai_Action();
    }

    IEnumerator Throw_Kunai_Action()
    {
        enemyTransform = targets[0];

        attack.GetOutcome();

        yield return MoveInRange(new Vector3(-1.75f, 0, 0));

        animationController.Clip("Sakura Kunai");

        yield return WaitForKeyFrame();

        GameObject kunai = Instantiate(kunaiPrefab, animationController.instatiatePoint.position, Quaternion.identity);

        //yield return ProjectileArch(kunai.transform, new Vector3(-0.1f, 0.2f, 0), 0.4f);

        Coroutine tragectory = StartCoroutine(ProjectileArch(kunai.transform, new Vector3(0.8f, 0f, 0f), 0.4f));


        if(Facing == 1)
            yield return new WaitWhile(() => kunai.transform.position.x <= targets[0].position.x);
        else
            yield return new WaitWhile(() => kunai.transform.position.x >= targets[0].position.x);

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
        bool done = false;

        int i = 0;


        while (!done)
        {

            switch (i)
            {
                case 0:

                    if (targets.Count > 0)
                        targets.RemoveAt(targets.Count - 1);


                    yield return SubMenuController.OpenSubMenu("Targets", TurnController.GetPlayerNames(Facing));

                    if (SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                    {
                        if (Facing == 1)
                            targets.Add(TurnController.right_Players[SubMenuController.CurrentSubMenu.ButtonChoice].transform);
                        else
                            targets.Add(TurnController.right_Players[SubMenuController.CurrentSubMenu.ButtonChoice].transform);

                        i++;
                    }
                    else
                    {
                        attack = null;
                        yield break;
                    }

                    break;

                case 1:

                    yield return SubMenuController.OpenSubMenu("Confirm", new List<string>() { "Confirm" });

                    if (SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                    {
                        done = true;
                    }
                    else
                    {
                        i--;
                    }

                    break;
            }

        }


        attack.Execute = Multi_Hit_Action();
    }

    IEnumerator Multi_Hit_Action()
    {

        enemyTransform = targets[0];
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

        yield return MoveAmount(new Vector3(0.3f * -Facing, 0, 0));

        yield return animationController.coroutine;
        animationController.Clip("Sakura Idle");
    }
}

