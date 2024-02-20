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

    public bool contact = false;

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

        yield return MoveInRange(attackList[index].range);

        attackList[index].Run(this);

        yield return attackList[index].coroutine;

        yield return new WaitForSeconds(0.3f);

        yield return ResetPos();

        StartTurn();
    }

    public void Contact()
    {
        print("Contact");
        contact = true;
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
        Vector3 startPos = transform.position;
        Vector3 targetPos = enemy.position + range;

        float archHeight = 0.25f;

        float timer = 0;
        //float maxTime = 0.4f;

        bool falling = false;

        while (timer < maxTime)
        {

            float x0 = startPos.x;
            float x1 = targetPos.x;
            float dist = x1 - x0;

            //float nextX = Mathf.MoveTowards(transform.position.x, x1, speed * Time.deltaTime);
            //float baseY = Mathf.Lerp(startPos.y, targetPos.y, (nextX - x0) / dist);
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

    public IEnumerator Impact()
    {
        yield return new WaitUntil(() => contact == true);

        contact = false;

        gameObject.GetComponent<AnimationController>().Pause();

        yield return new WaitForSeconds(0.2f); // contact pause

        gameObject.GetComponent<AnimationController>().Play();
    }

}
