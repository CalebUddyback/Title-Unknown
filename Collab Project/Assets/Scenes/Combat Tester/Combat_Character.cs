using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Combat_Character : MonoBehaviour
{
    public Vector3 startingPos;

    public Transform enemy;

    public Combat_Menu_Controller Menu;

    public class Attack
    {
        public string name;
        public string methodName;
        public int damage;
        public bool requiresRange = true;
        public Vector3 range;
        public Coroutine coroutine;

        public Attack(string name, string methodName)
        {
            this.name = name;
            this.methodName = methodName;
        }

        public void Run(MonoBehaviour owner)
        {
            coroutine = owner.StartCoroutine(methodName);
        }
    }

    public List<Attack> attackList;

    public bool eventFrame = false;

    public void Start()
    {
        StartTurn();
    }

    public void StartTurn()
    {
        Menu.gameObject.SetActive(true);
    }

    public string AttackName(int index)
    {
        return attackList[index].name;
    }

    public IEnumerator AttackChoice(int index)
    {
        startingPos = transform.position;

        Menu.ResetMenus();

        if(attackList[index].requiresRange)
            yield return MoveInRange(attackList[index].range);

        attackList[index].Run(this);

        yield return attackList[index].coroutine;

        yield return new WaitForSeconds(0.3f);

        yield return ResetPos();

        StartTurn();
    }

    public void EventFrame()
    {
        print("Contact");
        eventFrame = true;
    }


    public IEnumerator MoveInRange(Vector3 range)
    {
        Vector3 startPos = transform.position;

        Vector3 targetPos = new Vector3(enemy.position.x + range.x, transform.position.y , 0);

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


        yield return null;
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
        yield return new WaitUntil(() => eventFrame == true);

        eventFrame = false;
    }

    public IEnumerator Impact()
    {
        gameObject.GetComponent<AnimationController>().Pause();

        yield return new WaitForSeconds(0.2f); // contact pause

        gameObject.GetComponent<AnimationController>().Play();
    }

}
