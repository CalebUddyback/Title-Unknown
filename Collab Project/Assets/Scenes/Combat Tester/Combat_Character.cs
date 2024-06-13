using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

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

    [System.Serializable]
    public class Attack         //This will have to be seperated and serialized for imagaes to used
    {

        public string name;
        string methodName;
        public enum Stage { Attack, Defense };
        public Stage stage;
        public enum Type {Physical, Magic};
        public Sprite image;
        public float chargeTime = 1f;
        public int maxCharges = 0;

        [HideInInspector]
        public List<Transform> targets = new List<Transform>();

        [System.Serializable]
        public class Info
        {
            public int damage;

            public Type type;

            public Info(int damage)
            {
                this.damage = damage;
                this.type = Type.Physical;
            }

            public Info(int damage, Type type)
            {
                this.damage = damage;
                this.type = type;
            }
        }

        public Info[] info;
    
        public Attack(string name, string methodName)
        {
            this.name = name;
            this.methodName = methodName;
        }
   

        public IEnumerator SubMenus(MonoBehaviour owner)
        {
            yield return owner.StartCoroutine(methodName);
        }

        public IEnumerator Action;

        public string RectionName;

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

    [HideInInspector]
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

    public Attack chosenAttack;

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

    IEnumerator Focusing()
    {
        //if (cpu)
        //    yield break;


        yield return Hud.Timer(focusSpeed, Color.blue);

        TurnController.AddToTurnQueue(this);
    }

    public void StartTurn()
    {
        spotLight = true;

        if (cpu)
            StartCoroutine(CpuDecisionMaking());
        else
            SubMenuController.OpenSubMenu("Actions");
        //SubMenuController.gameObject.SetActive(true);

    }

    public void EndTurn()
    {
        SubMenuController.ResetMenus();

        spotLight = false;

        StartCoroutine(Charging(chosenAttack.chargeTime));
    }

    IEnumerator Charging(float chargeTime)
    {
        yield return Hud.Timer(chargeTime, Color.red);

        TurnController.AddToActionQueue(this);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Hud.EffectProgress(0.5f);
        }

        //MenuPositioning();
    }

    public Transform uiPoint;
    public Camera mcamera;

    public void MenuPositioning()
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

    public abstract IEnumerator CpuDecisionMaking();

    public void AttackChoice(Attack attack)
    {
        chosenAttack = attack;
    }

    public IEnumerator StartAttack()
    {
        yield return chosenAttack.Action;

        yield return new WaitForSeconds(0.3f);

        Coroutine charReset = StartCoroutine(ResetPos());

        List<Coroutine> targetResets = new List<Coroutine>();

        foreach (Transform target in chosenAttack.targets.Distinct())
            targetResets.Add(StartCoroutine(target.GetComponent<Combat_Character>().ResetPos()));

        yield return charReset;

        foreach (Coroutine target in targetResets)
            yield return target;

        chosenAttack.targets.Clear();
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
    }


    public IEnumerator WaitForKeyFrame()
    {
        yield return new WaitUntil(() => animationController.eventFrame == true);

        animationController.eventFrame = false;
    }

    public IEnumerator Impact()
    {
        animationController.Pause();

        enemy.animationController.Pause();

        yield return new WaitForSeconds(0.2f); // contact pause

        animationController.Play();

        enemy.animationController.Play();
    }

    public List<Attack> setSkills = new List<Attack>();

    public IEnumerator ApplyOutcome(Attack.Info info)
    {

        CoroutineWithData cd = new CoroutineWithData(this, Events(info));
        yield return cd.coroutine;

        if ((bool)cd.result)
            yield break;

        switch (chosenAttack.Success)
        {
            case 0:
                Instantiate(outcome_Bubble_Prefab, enemy.outcome_Bubble_Pos.position, Quaternion.identity).text.text = "DODGE";
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

    public IEnumerator Events(Attack.Info myAttackInfo)
    {
        List<string> labels = new List<string>();

        foreach (Attack enemyskill in enemy.setSkills)
        {
            if (enemyskill.info[0].type == myAttackInfo.type)
            {
                labels.Add(enemyskill.name);
            }
        }

        if (labels.Count > 0)
        {
            animationController.Pause();

            enemy.animationController.Pause();

            yield return enemy.SubMenuController.OpenSubMenu("Prompts", labels);

            if (enemy.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
            {
                enemy.StartCoroutine(enemy.setSkills[enemy.SubMenuController.CurrentSubMenu.ButtonChoice].RectionName);
                enemy.setSkills.RemoveAt(enemy.SubMenuController.CurrentSubMenu.ButtonChoice);
                enemy.Hud.ClearSkillSlot(enemy.SubMenuController.CurrentSubMenu.ButtonChoice);
                enemy.SubMenuController.ResetMenus();

                animationController.Play();
                enemy.animationController.Play();

                yield return true;
            }
        }
        else
        {
            yield return false;
        }
    }


    public abstract IEnumerator Damage();

    public abstract IEnumerator Block();

    public abstract IEnumerator Dodge();

}
