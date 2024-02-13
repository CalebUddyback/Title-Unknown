using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Sakura : MonoBehaviour
{
    public List<Attack> attackList;

    public Transform enemy;

    public UnityEngine.UI.Button attackBtn;

    public UnityEngine.UI.Button punchBtn, uppercutBtn, kickBtn, comboBtn;

    public GameObject Attacks;

    private Vector3 startingPos;

    private void Start()
    {
        attackList = new List<Attack>()
        {
            new Attack(nameof(Punch), 10, new Vector3(-0.35f, 0, 0)),
            new Attack(nameof(Uppercut), 10, new Vector3(-0.35f, 0, 0)),
            new Attack(nameof(Kick), 10, new Vector3(-0.35f, 0, 0)),
            new Attack(nameof(Combo), 10, new Vector3(-0.35f, 0, 0)),
        };

        //attackBtn.transform.GetChild(0).GetComponent<UnityEngine.UI.Text>().text = attackList[0].name;
        //attackBtn.onClick.AddListener(() => StartCoroutine(attackList[0].Action));
        attackBtn.onClick.AddListener(() =>  AttackCanvas());




        punchBtn.onClick.AddListener(() => StartCoroutine(AttackChoice(0)));

        uppercutBtn.onClick.AddListener(() => StartCoroutine(AttackChoice(1)));

        kickBtn.onClick.AddListener(() => StartCoroutine(AttackChoice(2)));

        comboBtn.onClick.AddListener(() => StartCoroutine(AttackChoice(3)));
    }

    public void AttackCanvas()
    {
        Attacks.gameObject.SetActive(true);
    }

    public IEnumerator AttackChoice(int index)
    {
        Attacks.GetComponent<CanvasGroup>().interactable = false;

        startingPos = transform.position;

        yield return MoveInRange(attackList[index].range);

        attackList[index].Run(this);

        yield return attackList[index].coroutine;

        yield return new WaitForSeconds(0.3f);

        yield return Reset(startingPos);

        Attacks.GetComponent<CanvasGroup>().interactable = true;
    }

    public IEnumerator MoveInRange(Vector3 range)
    {
        Vector3 startPos = transform.position;

        Vector3 targetPos = enemy.position + range;

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

    public IEnumerator Reset(Vector3 pos)
    {
        Vector3 startPos = transform.position;

        float timer = 0;
        float maxTime = 0.3f;

        while (timer < maxTime)
        {
            transform.position = Vector3.Lerp(startPos, pos, timer / maxTime);

            timer += Time.deltaTime;

            yield return null;
        }

        transform.position = pos;
    }


    public class Attack
    {
        public string name;
        public int damage;
        public Vector3 range;
        public Coroutine coroutine;

        public Attack(string name, int damage, Vector3 range)
        {
            this.name = name;
            this.damage = damage;
            this.range = range;
        }

        public void Run(MonoBehaviour owner)
        {
            coroutine = owner.StartCoroutine(name);
        }
    }

    IEnumerator Punch()
    {
        gameObject.GetComponent<AnimationController>().Clip("Sakura Punch");

        yield return new WaitForSeconds(0.25f);

        gameObject.GetComponent<AnimationController>().Clip("Sakura Idle");

        yield return null;
    }

    IEnumerator Uppercut()
    {
        gameObject.GetComponent<AnimationController>().Clip("Sakura Uppercut");

        yield return new WaitForSeconds(0.333f);

        gameObject.GetComponent<AnimationController>().Clip("Sakura Idle");

        yield return null;
    }

    IEnumerator Kick()
    {
        gameObject.GetComponent<AnimationController>().Clip("Sakura Kick");

        yield return new WaitForSeconds(0.417f);

        gameObject.GetComponent<AnimationController>().Clip("Sakura Idle");

        yield return null;
    }

    IEnumerator Combo()
    {
        yield return Punch();

        yield return Uppercut();

        yield return Kick();

        yield return null;
    }
}

