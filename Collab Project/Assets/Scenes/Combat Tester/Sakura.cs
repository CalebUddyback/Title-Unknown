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
                numOfTargets = 3,

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

        List<Transform> targets = new List<Transform>();

        int i = 0;
        int t = 0;


        while (!done)
        {

            switch (i)
            {
                case 0:

                    yield return SubMenuOutput("Charges", Enumerable.Range(1, attack.maxCharges).Select(n=>n.ToString()).ToList());

                    if (SubMenuController.currentSubMenu.ButtonChoice > -1)
                    {
                        i++;

                        charge = SubMenuController.currentSubMenu.ButtonChoice;
                    }
                    else
                    {
                        subMenuStage = 0;
                        attack = null;
                        yield break;
                    }


                    break;

                case 1:

                    if (t > 0)
                        t--;

                    while (-1 < t && t <= charge)
                    {
                        yield return SubMenuOutput("Targets", TurnController.GetPlayerNames(Facing));

                        if (SubMenuController.currentSubMenu.ButtonChoice > -1)
                        {
                            if (Facing == 1)
                                targets.Add(TurnController.right_Players[SubMenuController.currentSubMenu.ButtonChoice].transform);
                            else
                                targets.Add(TurnController.right_Players[SubMenuController.currentSubMenu.ButtonChoice].transform);

                            t++;
                        }
                        else
                        {
                            t--;
                        }
                    }

                    if (t > 0)
                    {
                        i++;
                    }
                    else
                    {
                        i--;
                        t = 0;
                    }

                    break;

                case 2:

                    yield return SubMenuOutput("Confirm", new List<string>() {"Confirm"});

                    if (SubMenuController.currentSubMenu.ButtonChoice > -1)
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


        attack.exe = Combo(charge, targets);
    }


    IEnumerator Combo(int charge, List<Transform> targets)
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

        int charge = 0;

        List<Transform> targets = new List<Transform>();

        int i = 0;

        while (!done)
        {

            switch (i)
            {
                case 0:

                    SubMenu reqMenu = SubMenuController.CreateSubMenu("Charges");

                    reqMenu.AddButtons(new List<string>() { "1", "2", "3" });

                    yield return SubMenuController.CurrentCD.coroutine;

                    charge = reqMenu.ButtonChoice;

                    if (reqMenu.ButtonChoice > -1)
                        i++;

                    break;

                case 1:

                    // for each charge


                    reqMenu = SubMenuController.CreateSubMenu("Targets");

                    reqMenu.AddButtons(TurnController.GetPlayerNames(Facing));

                    yield return SubMenuController.CurrentCD.coroutine;

                    if (reqMenu.ButtonChoice > -1)
                    {
                        if (Facing == 1)
                            targets.Add(TurnController.right_Players[reqMenu.ButtonChoice].transform);
                        else
                            targets.Add(TurnController.right_Players[reqMenu.ButtonChoice].transform);


                        i++;
                    }
                    else
                    {
                        i--;
                    }

                    break;

                case 2:

                    SubMenu confirmMenu = SubMenuController.OpenSubMenu("Confirm");

                    yield return SubMenuController.CurrentCD.coroutine;

                    if (confirmMenu.ButtonChoice > -1)
                        done = true;
                    else
                        i--;

                    break;
            }

        }


        attack.exe = Jump_Kick(charge, targets);
    }

    IEnumerator Jump_Kick(int charge, List<Transform> targets)
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


        if(Facing == 1)
            yield return new WaitWhile(() => kunai.transform.position.x <= enemyTransform.position.x);
        else
            yield return new WaitWhile(() => kunai.transform.position.x >= enemyTransform.position.x);

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

        yield return MoveAmount(new Vector3(0.3f * -Facing, 0, 0));

        yield return animationController.coroutine;
        animationController.Clip("Sakura Idle");
    }
}

