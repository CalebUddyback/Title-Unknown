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

    public Combat_Character enemy => enemyTransform.GetComponent<Combat_Character>();

    public SubMenu_Controller SubMenuController;

    public AnimationController animationController;

    public Outcome_Bubble outcome_Bubble_Prefab;

    public Transform outcome_Bubble_Pos;

    [System.Serializable]
    public abstract class Skill
    {
        [HideInInspector]
        public Combat_Character character;

        public string name;
        public bool effect = false;
        public string description = "";
        public enum Type { Physical, Magic };
        public enum Range { Close, Far };
        public int levels = 0;
        public bool charging;
        public int maxCharges = 0;
        public int currentCharges = 0;
        public float[] chargeTimes;

        [HideInInspector]
        public List<Transform> targets = new List<Transform>();

        public int damage;
        public int critical;
        public int accuracy;
        public int focusPenalty;
        public Type type;
        public Range range;

        public enum Stat
        {
            DMG,
            CRT,
            HIT,
            REC,
        }

        [Header("Buffs/Debuffs")]
        public List<StatChanger> statChangers = new List<StatChanger>();

        public Skill(Combat_Character character)  //This is needed for a default inheritance contructor call
        {
            this.character = character;
            damage = 0;
            critical = 0;
            accuracy = 0;
            focusPenalty = 0;
            type = Type.Physical;
            range = Range.Close;
        }

        private Dictionary<Stat, int> GetBaseStats()
        {
            var baseDirectory = new Dictionary<Stat, int>
            {
                {Stat.DMG, damage },
                {Stat.CRT, critical },
                {Stat.HIT, accuracy },
                {Stat.REC, focusPenalty },
            };

            return baseDirectory;
        }

        public Dictionary<Stat, int> GetCurrentStats()
        {
            Dictionary<Stat, int> currentDirectory = GetBaseStats();

            foreach(StatChanger changer in statChangers)
            {
                foreach(KeyValuePair<Stat, int> stat in changer.skill_statChanges)
                {
                    currentDirectory[stat.Key] += stat.Value;
                }
            }

            return currentDirectory;
        }

        [System.Serializable]
        public class Info
        {
            public int damage;
            public int critical;
            public int accuracy;
            public int HitSuccess { get; set; }

            public Range range;
            public Type type;

            public Info(int damage, int critical, int accuracy, Type type, Range range)
            {
                this.damage = damage;
                this.critical = critical;
                this.accuracy = accuracy;
                this.type = type;
                this.range = range;
            }

            public Info(Info info)
            {
                damage = info.damage;
                type = info.type;
                range = info.range;
            }
        }

        public Info[] baseInfo = new Info[] { }, currentInfo = new Info[] { };

        public void SetCurrentInfo()
        {
            currentInfo = new Info[baseInfo.Length];

            for (int i = 0; i < baseInfo.Length; i++)
            {
                currentInfo[i] = new Info(baseInfo[i]);
            }
        }

        public abstract IEnumerator SubMenus(MonoBehaviour owner);

        public IEnumerator Execute;

        //public int Critical { get; set; }
        //public float Success { get; set; }

        public void GetOutcome()
        {
            foreach (Info info in currentInfo)
            {
                info.critical = Random.Range(0, 10) > 4 ? 2 : 1;

                info.HitSuccess = Random.Range(0, 99) < 75 ? 1 : 0;

                //Debug

                //Success = 1;
            }
        }
    }

    /* SPELLS NEED A MONOBEHAVIOR ASPECT SO THAT AN INSTANCE (COMPONENT) CAN BE ADDED TO SKILL SLOT */ 

    public abstract class Spell : Skill
    {
        public Sprite image;

        public Turn_Controller.Stage stage;

        public Spell(Combat_Character character) : base(character)
        {
            damage = 0;
            critical = 0;
            accuracy = 0;
            focusPenalty = 0;
        }

        public abstract bool Condition(Turn_Controller.Stage stage, Info info);

        public abstract IEnumerator Action2(int slot);
    }


    public List<Skill> attackList;

    public Skill chosenAttack;

    //[System.Serializable]
    //public abstract class Skill
    //{
    //
    //}

    public Spell[] setSpells = new Spell[3];

    //public List<Transform> targets = new List<Transform>();

    public int Facing { get; set; } = 1;


    [Header("Turn Controller")]

    public bool spotLight = false;

    public Character_Hud Hud;

    public Turn_Controller TurnController { get; set; }


    [Header("Stats")]

    //public string characterName = "No Name";

    public int health;

    [SerializeField]
    public float focusSpeed = 1f;


    public Stats stats;

    public void StartFocus()
    { 
        StartCoroutine(Focusing());
    }

    IEnumerator Focusing()
    {
        float totalTime = (chosenAttack != null) ? focusSpeed + chosenAttack.focusPenalty/10f : focusSpeed;


        Hud.SetTimer(totalTime, Color.grey);

        yield return new WaitUntil(() => Hud.GetTimeLeft() <= focusSpeed);

        Hud.SetTimerColor(Color.blue);

        //Coroutine timer = StartCoroutine(Hud.Timer(totalTime, Color.grey));
        //
        //if (chosenAttack != null)
        //    yield return new WaitUntil(() => Hud.GetTimeLeft() <= focusSpeed);
        //
        //Hud.SetTimerColor(Color.blue);
        //
        //yield return timer;
        //
        //TurnController.AddToTurnQueue(this);
    }

    public void StartTurn()
    {
        spotLight = true;

        if (cpu)
            StartCoroutine(CpuDecisionMaking());
        else
        {
            if (chosenAttack != null && chosenAttack.charging)
                StartCoroutine(chosenAttack.SubMenus(this));
            else
                StartCoroutine(SubMenuController.OpenSubMenu("Actions", new List<string> { "Attack", "Defend", "Items", "Rest"}));
        }

    }

    public void EndTurn()
    {
        SubMenuController.ResetMenus();

        spotLight = false;

        //StartCoroutine(Charging(chosenAttack.chargeTime));
    }

    public IEnumerator Charging(float chargeTime)
    {
        Hud.SetTimer(chargeTime, Color.red);
        yield return null;
        //yield return Hud.Timer(chargeTime, Color.red);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            Hud.AffectProgress(0.5f);
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

    public void AttackChoice(Skill attack)
    {
        chosenAttack = attack;
        chosenAttack.SetCurrentInfo();
    }

    public int SetSkill(Spell action, Sprite img)
    {
        for (int i = 0; i < 3; i++)
        {
            if (setSpells[i] == null)
            {
                setSpells[i] = action;
                Hud.SkillSlot(i, img);

                return i;
            }
        }

        return -1;
    }

    public void ClearSkill(int i)
    {
        Hud.ClearSkillSlot(i);

        setSpells[i] = null;
    }

    public IEnumerator StartAttack()
    {
        yield return chosenAttack.Execute;

        yield return new WaitForSeconds(0.3f);

        TurnController.descriptionBox.container.SetActive(false);

        // Reset Camera

        mcamera.GetComponent<MainCamera>().BlackOut(0f, 0.5f);

       Coroutine cam =  StartCoroutine(mcamera.GetComponent<MainCamera>().Reset(0.2f));


        //  Reset Involved Characters


        Coroutine charReset = StartCoroutine(ResetPos());

        List<Coroutine> targetResets = new List<Coroutine>();

        foreach (Transform target in chosenAttack.targets.Distinct())
            targetResets.Add(StartCoroutine(target.GetComponent<Combat_Character>().ResetPos()));

        yield return charReset;

        foreach (Coroutine target in targetResets)
            yield return target;

        yield return cam;


        /* Skill Reset */

        chosenAttack.targets.Clear();

        //if(!chosenAttack.charging)
        //    chosenAttack = null;
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

        yield return new WaitForSeconds(0.25f); // contact pause

        animationController.Play();

        enemy.animationController.Play();
    }

    public IEnumerator ApplyOutcome(Skill.Info info)
    {
        switch (info.HitSuccess)
        {
            case 0:
                Instantiate(outcome_Bubble_Prefab, enemy.outcome_Bubble_Pos.position, Quaternion.identity).Input("DODGE");
                yield return StartCoroutine(enemyTransform.GetComponent<Combat_Character>().Dodge());
                break;

            case 1:

                if (Mathf.Abs(info.damage) < 5)
                {
                    StartCoroutine(enemy.Block());
                    Instantiate(outcome_Bubble_Prefab, enemy.outcome_Bubble_Pos.position, Quaternion.identity).Input(info.damage);
                    //Instantiate(outcome_Bubble_Prefab, enemy.outcome_Bubble_Pos.position, Quaternion.identity).Input(info.damage, "BLOCK");
                }
                else
                {

                    StartCoroutine(enemy.Damage());
                    Instantiate(outcome_Bubble_Prefab, enemy.outcome_Bubble_Pos.position, Quaternion.identity).Input(info.damage);
                }

                enemy.AdjustHealth(info.damage);
                yield return Impact();
                break;
        }
    }


    public void AdjustHealth(int amount)
    {
        health += amount;

        Hud.AdjustHealth(health, amount);
    }

    public abstract IEnumerator Damage();

    public abstract IEnumerator Block();

    public abstract IEnumerator Dodge();

}
