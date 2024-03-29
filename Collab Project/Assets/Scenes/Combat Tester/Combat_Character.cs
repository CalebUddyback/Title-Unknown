﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Combat_Character : MonoBehaviour
{
    public float startingXPos;

    public Transform enemyTransform;

    private Combat_Character enemy => enemyTransform.GetComponent<Combat_Character>();

    public Combat_Menu_Controller Menu;

    public AnimationController animationController;

    public Outcome_Bubble outcome_Bubble_Prefab;

    public Transform outcome_Bubble_Pos;


    public class Attack
    {
        public string name;
        public string methodName;
        public float chargeTime = 0.5f;
        public int damage;
        public float precision = 10;
        public Vector3 range;
        public Coroutine coroutine;

        public Attack(string name, string methodName)
        {
            this.name = name;
            this.methodName = methodName;
        }

        public void Execute(MonoBehaviour owner)
        {
            coroutine = owner.StartCoroutine(methodName);
        }

        public int Critical { get; private set;}
        public float Success { get; private set;}

        public void GetOutcome()
        {

            Success = 0;

            Critical = Random.Range(0, 10) > 4 ? 2 : 1;

            for (int i = 0; i < 2; i++)
            {
                float roll = Random.Range(0f, 10f) > 5 ? Success += 0.5f : Success;
            }

            string outcome = "";

            if (Critical == 2)
                outcome += "CRITICAL ";

            switch (Success)
            {
                case 0:
                    outcome += "Miss";
                    break;
                case 0.5f:
                    outcome += "Block";
                    break;
                case 1:
                    outcome += "Hit";
                    break;
                default:
                    print("ERROR: " + Success);
                    break;
                        
            }

            print(outcome);
        }
    }

    public List<Attack> attackList;


    [Header("Turn Controller")]

    public UnityEngine.UI.Image timer;

    public float actual_Progess;


    private void Start()
    {
        startingXPos = transform.position.x;
        StartCoroutine(Focusing());
    }

    IEnumerator Focusing()
    {
        float speed = 1f;

        timer.color = Color.blue;

        timer.fillAmount = 0;

        actual_Progess = 0;

        while (actual_Progess < speed)
        {
            timer.fillAmount = actual_Progess / speed;

            actual_Progess += Time.deltaTime;

            yield return null;
        }


        timer.fillAmount = 1;

        actual_Progess = speed;

        StartTurn();
    }

    IEnumerator Charging(float chargeTime)
    {
        timer.color = Color.red;

        timer.fillAmount = 0;

        actual_Progess = 0;

        while (actual_Progess < chargeTime)
        {
            timer.fillAmount = actual_Progess / chargeTime;

            actual_Progess += Time.deltaTime;

            yield return null;
        }


        timer.fillAmount = 1;

        actual_Progess = chargeTime;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            actual_Progess -= 0.5f;
        }

        UITEST();
    }

    public void StartTurn()
    {
        Menu.gameObject.SetActive(true);
    }

    public Transform uiPoint;
    public Camera mcamera;

    void UITEST()
    {
        Vector3 targPos = uiPoint.position;

        Vector3 camForward = mcamera.transform.forward;

        Vector3 camPos = mcamera.transform.position + camForward;

        float distanceInFrontOfCamera = Vector3.Dot(targPos - camPos, camForward);

        if(distanceInFrontOfCamera < 0f)
        {
            targPos -= camForward * distanceInFrontOfCamera;
        }

        Menu.gameObject.transform.position = RectTransformUtility.WorldToScreenPoint(mcamera, targPos);
    }

    public string AttackName(int index)
    {
        return attackList[index].name;
    }

    public Attack attackInfo;

    public void AttackChoice(int index)
    {
        attackInfo = attackList[index];

        StartCoroutine(AttackChoice());
    }

    public IEnumerator AttackChoice()
    {
        Menu.ResetMenus();

        Attack main = attackInfo;     //This allows for combos to change attackchoice without breaking this coroutine

        yield return Charging(attackInfo.chargeTime);

        main.Execute(this);

        yield return main.coroutine;

        yield return new WaitForSeconds(0.3f);

        Coroutine charReset = StartCoroutine(ResetPos());

        Coroutine enemyRest = StartCoroutine(enemy.ResetPos());

        yield return charReset;

        StartCoroutine(Focusing());

        yield return enemyRest;
    }


    public IEnumerator MoveInRange(Vector3 range)
    {
        Vector3 startPos = transform.position;

        Vector3 targetPos = new Vector3(enemyTransform.position.x + range.x, transform.position.y , 0);

        if (startPos == targetPos)
            yield break;

        float timer = 0;
        float maxTime = 0.3f;

        while (timer < maxTime)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, timer / maxTime);

            timer += Time.deltaTime;

            yield return null;
        }

        transform.position = targetPos;
    }

    public IEnumerator MoveAmount(Vector3 amuont)
    {
        Vector3 startPos = transform.position;

        Vector3 targetPos = startPos + amuont;

        if (transform.position == targetPos)
            yield break;

        float timer = 0;
        float maxTime = 0.25f;

        while (timer < maxTime)
        {
            transform.position = Vector3.Lerp(startPos, targetPos, timer / maxTime);

            timer += Time.deltaTime;

            yield return null;
        }

        transform.position = targetPos;
    }

    public IEnumerator JumpInRange(Vector3 range, float maxTime)
    {
        /* This Method Works for Flying enemies */


        Vector3 startPos = transform.position;
        Vector3 targetPos = enemyTransform.position + range;

        float archHeight = 0.25f;

        float timer = 0;
        //float maxTime = 0.4f;

        while (timer < maxTime)
        {

            float x0 = startPos.x;
            float x1 = targetPos.x;
            float dist = x1 - x0;

            float nextX = Mathf.Lerp(startPos.x, targetPos.x, timer / maxTime);
            float baseY = Mathf.Lerp(startPos.y, targetPos.y, timer / maxTime);
            float arc = archHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
            Vector3 nextPos = new Vector3(nextX, baseY + arc, transform.position.z);

            transform.position = nextPos;

            timer += Time.deltaTime;

            yield return null;
        }

        transform.position = targetPos;
    }

    public IEnumerator ResetPos()
    {
        float currentXPos = transform.position.x;

        float timer = 0;
        float maxTime = 0.3f;

        while (timer < maxTime)
        {
            float lerp = Mathf.Lerp(currentXPos, startingXPos, timer / maxTime);

            transform.position = new Vector3(lerp, transform.position.y, transform.position.z);

            timer += Time.deltaTime;

            yield return null;
        }

        transform.position = new Vector3(startingXPos, transform.position.y, transform.position.z);
    }

    public IEnumerator WaitForKeyFrame()
    {
        yield return new WaitUntil(() => animationController.eventFrame == true);

        animationController.eventFrame = false;
    }

    public IEnumerator ApplyOutcome()
    {
        switch (attackInfo.Success)
        {
            case 0:
                yield return StartCoroutine(enemyTransform.GetComponent<Combat_Character>().Dodge());
                break;

            case 0.5f:
                StartCoroutine(enemy.Block());
                Instantiate(outcome_Bubble_Prefab, enemy.outcome_Bubble_Pos.position, Quaternion.identity).text.text = (attackInfo.damage/2).ToString();
                yield return Impact();
                break;

            case 1:
                StartCoroutine(enemy.Damage());
                Instantiate(outcome_Bubble_Prefab, enemy.outcome_Bubble_Pos.position, Quaternion.identity).text.text = attackInfo.damage.ToString();
                yield return Impact();
                break;
        }
    }

    public IEnumerator Impact()
    {
        animationController.Pause();

        enemy.animationController.Pause();

        yield return new WaitForSeconds(0.2f); // contact pause

        animationController.Play();

        enemy.animationController.Play();
    }


    public abstract IEnumerator Damage();

    public abstract IEnumerator Block();

    public abstract IEnumerator Dodge();

    public IEnumerator ProjectileArch(Transform instance, Vector3 range, float maxTime)
    {
        /* This Method Works for Flying enemies */


        Vector3 startPos = instance.position;
        Vector3 targetPos = enemyTransform.position + range;

        float archHeight = 0.1f;

        float timer = 0;

        while (timer < maxTime)
        {

            float x0 = startPos.x;
            float x1 = targetPos.x;
            float dist = x1 - x0;

            float nextX = Mathf.Lerp(startPos.x, targetPos.x, timer / maxTime);
            float baseY = Mathf.Lerp(startPos.y, targetPos.y, timer / maxTime);
            float arc = archHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
            Vector3 nextPos = new Vector3(nextX, baseY + arc, instance.position.z);

            instance.rotation = LookAt2D(nextPos - instance.position);
            instance.position = nextPos;

            timer += Time.deltaTime;

            yield return null;
        }

        instance.position = targetPos;

        Quaternion LookAt2D(Vector2 forward)
        {
            return Quaternion.Euler(0, 0, Mathf.Atan2(forward.y, forward.x) * Mathf.Rad2Deg);
        }
    }

}
