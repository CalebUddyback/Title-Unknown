using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Combat_Character : MonoBehaviour
{
    public Vector3 startingPos;

    public Transform enemy;

    public Combat_Menu_Controller Menu;

    public AnimationController animationController;

    public class Attack
    {
        public string name;
        public string methodName;
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

    public void StartTurn()
    {
        Menu.gameObject.SetActive(true);
    }

    public string AttackName(int index)
    {
        return attackList[index].name;
    }

    public Attack attackChoice;

    public void AttackChoice(int index)
    {
        attackChoice = attackList[index];

        StartCoroutine(AttackChoice());
    }

    public IEnumerator AttackChoice()
    {
        startingPos = transform.position;

        Menu.ResetMenus();

        attackChoice.Execute(this);

        yield return attackChoice.coroutine;

        yield return new WaitForSeconds(0.3f);

        yield return ResetPos();

        StartTurn();
    }


    public IEnumerator MoveInRange(Vector3 range)
    {
        Vector3 startPos = transform.position;

        Vector3 targetPos = new Vector3(enemy.position.x + range.x, transform.position.y , 0);

        if (transform.position == targetPos)
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
        float maxTime = 0.3f;

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
        Vector3 targetPos = enemy.position + range;

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
        Vector3 startPos = transform.position;

        float timer = 0;
        float maxTime = 0.3f;

        while (timer < maxTime)
        {
            transform.position = Vector3.Lerp(startPos, startingPos, timer / maxTime);

            timer += Time.deltaTime;

            yield return null;
        }

        transform.position = startingPos;
    }

    public IEnumerator WaitForKeyFrame()
    {
        yield return new WaitUntil(() => animationController.eventFrame == true);

        animationController.eventFrame = false;
    }

    public IEnumerator ApplyOutcome()
    {
        switch (attackChoice.Success)
        {
            case 0:
                yield return StartCoroutine(enemy.GetComponent<Combat_Character>().Dodge());
                break;

            case 0.5f:
                StartCoroutine(enemy.GetComponent<Combat_Character>().Block());
                yield return Impact();
                break;

            case 1:
                StartCoroutine(enemy.GetComponent<Combat_Character>().Damage());
                yield return Impact();
                break;
        }
    }

    public IEnumerator Impact()
    {
        animationController.Pause();

        enemy.GetComponent<Combat_Character>().animationController.Pause();

        yield return new WaitForSeconds(0.2f); // contact pause

        animationController.Play();

        enemy.GetComponent<Combat_Character>().animationController.Play();
    }


    public abstract IEnumerator Damage();

    public abstract IEnumerator Block();

    public abstract IEnumerator Dodge();
}
