using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Combat_Character : MonoBehaviour
{
    public bool cpu = false;

    public Vector3 startingPos;

    public Transform enemyTransform;

    private Combat_Character enemy => enemyTransform.GetComponent<Combat_Character>();

    public SubMenu_Controller SubMenuController;

    public AnimationController animationController;

    public Outcome_Bubble outcome_Bubble_Prefab;

    public Transform outcome_Bubble_Pos;


    public class Attack
    {
        public string name;
        public string methodName;
        public float chargeTime = 0.5f;
        public int maxCharges = 0;
        public int numOfTargets = 0;

        public class RequiredMenu
        {
            public string Menu { get; private set; } = "";
            public string DependantMenu { get; private set; } = "";

            public RequiredMenu(string menu, string dependantMenu)
            {
                Menu = menu;
                DependantMenu = dependantMenu;
            }

            public RequiredMenu(string menu)
            {
                Menu = menu;
            }
        }

        public RequiredMenu[] requiredMenus;
        public Info[] info;
        public Coroutine coroutine;

        public IEnumerator Execute;

        public Attack(string name, string methodName)
        {
            this.name = name;
            this.methodName = methodName;
        }

        public class Info
        {
            public int damage;

            public Info(int damage)
            {
                this.damage = damage;
            }
        }

        public void SubMenus(MonoBehaviour owner)
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

            //Success = 0;

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

    public List<string> GetAttackNames()
    {
        List<string> list = new List<string>();

        foreach(Attack attack in attackList)
        {
            list.Add(attack.name);
        }

        return list;
    }

    public Attack attack;

    //public List<Transform> targets = new List<Transform>();

    public int Facing { get; set; } = 1;


    [Header("Turn Controller")]

    public bool spotLight = false;

    public float actual_Progess;

    public Character_Hud Hud;

    public Turn_Controller TurnController { get; set; }


    [Header("Stats")]

    //public string characterName = "No Name";

    public float focusSpeed = 1f;

    public void StartFocus()
    { 
        StartCoroutine(Focusing());
    }

    public void StartCharging(float t)
    {
        StartCoroutine(Charging(t));
    }

    public bool TurnTime { get; private set; } = true;

    public void ToggleTurnTime()
    {
        TurnTime = !TurnTime;
    }

    public void ToggleTurnTime(bool set)
    {
        TurnTime = set;
    }

    IEnumerator Focusing()
    {

        //timer.color = Color.blue;

        Hud.timer.fillAmount = 0;

        float requiredFocus = 3;

        actual_Progess = requiredFocus;

        while (0 <= actual_Progess)
        {
            yield return new WaitUntil(() => TurnTime);

            actual_Progess -= Time.deltaTime * focusSpeed;

            Hud.timer.fillAmount = (requiredFocus - actual_Progess) / requiredFocus;

            yield return null;
        }

        Hud.timer.fillAmount = 1;

        actual_Progess = 0;

        TurnController.AddToTurnQueue(this);

    }

    IEnumerator Charging(float chargeTime)
    {

        SubMenuController.ResetMenus();

        spotLight = false;

        //timer.color = Color.red;

        Hud.timer.fillAmount = 0;

        actual_Progess = chargeTime;

        while (0 <= actual_Progess)
        {
            yield return new WaitUntil(() => TurnTime);

            actual_Progess -= Time.deltaTime;

            Hud.timer.fillAmount = (chargeTime - actual_Progess) / chargeTime;

            yield return null;
        }

        Hud.timer.fillAmount = 1;

        actual_Progess = chargeTime;

        TurnController.AddToActionQueue(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            actual_Progess -= 0.5f;
        }

        MenuPositioning();
    }

    public void StartTurn()
    {
        spotLight = true;

        if (cpu)
            CpuDecisionMaking();
        else
            SubMenuController.gameObject.SetActive(true);

    }

    public void EndTurn()
    {
        
    }

    public Transform uiPoint;
    public Camera mcamera;

    void MenuPositioning()
    {
        Vector3 targPos = uiPoint.position;

        Vector3 camForward = mcamera.transform.forward;

        Vector3 camPos = mcamera.transform.position + camForward;

        float distanceInFrontOfCamera = Vector3.Dot(targPos - camPos, camForward);

        if(distanceInFrontOfCamera < 0f)
        {
            targPos -= camForward * distanceInFrontOfCamera;
        }

        SubMenuController.gameObject.transform.position = RectTransformUtility.WorldToScreenPoint(mcamera, targPos);
    }

    public virtual void CpuDecisionMaking()
    {
        int i = Random.Range(0, attackList.Count);

        int r;

        Attack atk = attackList[i];

        if (atk.requiredMenus != null)
        {
            r = Random.Range(0, atk.maxCharges);

            AttackChoice(attackList[i]);
        }
        else
            AttackChoice(attackList[i]);

    }

    public void AttackChoice(Attack atk)
    {
        attack = atk;
    }

    public IEnumerator StartAttack()
    {
        //Menu.ResetMenus();

        //Attack attack = this.attack;     //This allows for combos to change attackchoice without breaking this coroutine

        //yield return Charging(attack.chargeTime);

        //attack.Execute(this);

        //yield return attack.coroutine;

        yield return attack.Execute;

        yield return new WaitForSeconds(0.3f);

        Coroutine charReset = StartCoroutine(ResetPos());

        Coroutine enemyReset = StartCoroutine(enemy.ResetPos());

        yield return charReset;

        yield return enemyReset;
    }

    public IEnumerator MoveInRange(Vector3 range)
    {
        Vector3 startPos = transform.position;

        Vector3 targetPos = new Vector3(enemyTransform.position.x + range.x * Facing, transform.position.y , enemyTransform.position.z);

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

    public IEnumerator JumpInRange(Vector3 range)
    {
        yield return JumpInRange(range, 0.3f);
    }

    public IEnumerator JumpInRange(Vector3 range, float maxTime)
    {
        /* This Method Works for Flying enemies */


        Vector3 startPos = transform.position;
        Vector3 targetPos = new Vector3(enemyTransform.position.x + range.x * Facing, enemyTransform.transform.position.y + range.y, enemyTransform.position.z);

        float archHeight = 0.25f;

        float timer = 0;
        //float maxTime = 0.4f;

        while (timer < maxTime)
        {

            float x0 = startPos.x;
            float x1 = targetPos.x;
            float dist = x1 - x0;

            float nextX = Mathf.Lerp(startPos.x, targetPos.x, timer / maxTime);
            float nextY = Mathf.Lerp(startPos.y, targetPos.y, timer / maxTime);
            float nextZ = Mathf.Lerp(startPos.z, targetPos.z, timer / maxTime);
            float arc = archHeight * (nextX - x0) * (nextX - x1) / (-0.25f * dist * dist);
            Vector3 nextPos = new Vector3(nextX, nextY + arc, nextZ);

            transform.position = nextPos;

            timer += Time.deltaTime;

            yield return null;
        }

        transform.position = targetPos;
    }

    public IEnumerator ResetPos()
    {
        Vector3 currentPos = transform.position;

        float timer = 0;
        float maxTime = 0.3f;

        while (timer < maxTime)
        {
            float xLerp = Mathf.Lerp(currentPos.x, startingPos.x, timer / maxTime);

            //float yLerp = Mathf.Lerp(currentPos.y, startingPos.y, timer / maxTime);

            float zLerp = Mathf.Lerp(currentPos.z, startingPos.z, timer / maxTime);

            transform.position = new Vector3(xLerp, transform.position.y, zLerp);

            timer += Time.deltaTime;

            yield return null;
        }

        transform.position = startingPos;

        EndTurn();
    }

    public IEnumerator WaitForKeyFrame()
    {
        yield return new WaitUntil(() => animationController.eventFrame == true);

        animationController.eventFrame = false;
    }

    public IEnumerator ApplyOutcome(Attack.Info info)
    {
        switch (attack.Success)
        {
            case 0:
                yield return StartCoroutine(enemyTransform.GetComponent<Combat_Character>().Dodge());
                break;

            case 0.5f:
                StartCoroutine(enemy.Block());
                Instantiate(outcome_Bubble_Prefab, enemy.outcome_Bubble_Pos.position, Quaternion.identity).text.text = (info.damage/2).ToString();
                yield return Impact();
                break;

            case 1:
                StartCoroutine(enemy.Damage());
                Instantiate(outcome_Bubble_Prefab, enemy.outcome_Bubble_Pos.position, Quaternion.identity).text.text = info.damage.ToString();
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
        Vector3 targetPos = new Vector3(enemyTransform.position.x + range.x * Facing, enemyTransform.transform.position.y + range.y, enemyTransform.position.z);

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
