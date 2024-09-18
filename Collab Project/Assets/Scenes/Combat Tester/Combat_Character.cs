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

        public class Skill_Stats
        {
            public int attack;
            public int accuracy;
            public int critical;
            public int mana;

            public StatChanger statChanger;
        }
        public Skill_Stats[] skill_Stats;

        public enum Type { Physical, Magic };
        public Type type = Type.Physical;
        public enum Range { Close, Far };
        public Range range = Range.Close;

        public int level = 0;
        public int maxLevel = 0;

        public bool charging;
        public string chargeAnimation = "";

        public int CritSuccess { get; set; }
        public int HitSuccess { get; set; }

        [HideInInspector]
        public List<Transform> targets = new List<Transform>();

        public Skill(Combat_Character character) => this.character = character;

        public abstract IEnumerator SubMenus(MonoBehaviour owner);

        public IEnumerator Execute;

        public void GetOutcome(Skill_Stats stats, Transform t)
        {
            Combat_Character target = t.GetComponent<Combat_Character>();

            var chances = character.character_Stats.GetCombatStats(stats, target);

            int critRoll = Random.Range(0, 100);

            CritSuccess = critRoll < chances[Character_Stats.Stat.Crit] ? 3 : 1;

            int hitRoll = (Random.Range(0, 100) + Random.Range(0, 100)) / 2;

            HitSuccess = hitRoll < chances[Character_Stats.Stat.PhHit] ? 1 : 0;

            print(character.name + ": " + hitRoll  + " (<" + chances[Character_Stats.Stat.PhHit] + "?) " + critRoll + " (<" + chances[Character_Stats.Stat.Crit] + "?)");

            //Debug

            //Success = 1;
        }
    }

    /* SPELLS NEED A MONOBEHAVIOR ASPECT SO THAT AN INSTANCE (COMPONENT) CAN BE ADDED TO SKILL SLOT */ 

    public abstract class Spell : Skill
    {
        public Sprite image;

        public Turn_Controller.Stage stage;

        public Spell(Combat_Character character) : base(character)
        {
 
        }

        public abstract bool Condition(Turn_Controller.Stage stage, Skill info);

        public abstract IEnumerator Action2(int slot);
    }


    public List<Skill> attackList;

    public Skill chosenAttack;

    public Spell[] setSpells = new Spell[3];

    public int Facing { get; set; } = 1;


    [Header("Turn Controller")]

    public bool spotLight = false;

    public Character_Hud Hud;

    public Turn_Controller TurnController { get; set; }


    [Header("Stats")]

    //public string characterName = "No Name";

    public int health;

    public int mana;

    public Character_Stats character_Stats;

    public void StartFocus()
    { 
        StartCoroutine(Focusing());
    }

    IEnumerator Focusing()
    {

        //float totalTime = (chosenAttack != null) ? stats.GetCurrentStats()[Stats.Stat.AS] + chosenAttack.GetCurrentStats()[Skill.Stat.REC] : stats.GetCurrentStats()[Stats.Stat.AS];

        float totalTime = character_Stats.GetCurrentStats()[Character_Stats.Stat.AS];

        Hud.SetTimer(totalTime, Color.grey);

        yield return new WaitUntil(() => Hud.GetTimeLeft() <= character_Stats.GetCurrentStats()[Character_Stats.Stat.AS]);

        Hud.SetTimerColor(Color.blue);

    }

    public void StartTurn()
    {
        spotLight = true;

        character_Stats.IncrementStatChangers();

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

        if(chosenAttack != null && chosenAttack.charging && chosenAttack.chargeAnimation != "")
            animationController.Clip(chosenAttack.chargeAnimation);      // this should be a check on the chosenskill to see if charge was BROKEN and whether to continue or not
    }


    public IEnumerator WaitForKeyFrame()
    {
        yield return new WaitUntil(() => animationController.eventFrame == true);

        animationController.eventFrame = false;
    }

    public IEnumerator Impact(float timer)
    {
        animationController.Pause();

        enemy.animationController.Pause();

        yield return new WaitForSeconds(timer); // contact pause

        animationController.Play();

        enemy.animationController.Play();
    }

    public IEnumerator ApplyOutcome(int success, int critical, float damage)
    {

        damage *= -1;
        damage *= critical;

        switch (success)
        {
            case 0:
                Instantiate(outcome_Bubble_Prefab, enemy.outcome_Bubble_Pos.position, Quaternion.identity).Input("MISS");
                yield return StartCoroutine(enemyTransform.GetComponent<Combat_Character>().Dodge());
                break;

            case 1:

                if (damage > -5 && damage <= 0)
                {
                    StartCoroutine(enemy.Block());
                }
                else
                {
                    StartCoroutine(enemy.Damage());
                }

                if (critical != 1)
                    Instantiate(outcome_Bubble_Prefab, enemy.outcome_Bubble_Pos.position, Quaternion.identity).Input(Mathf.RoundToInt(damage), "CRITICAL", Color.yellow);
                else
                    Instantiate(outcome_Bubble_Prefab, enemy.outcome_Bubble_Pos.position, Quaternion.identity).Input(Mathf.RoundToInt(damage));

                enemy.AdjustHealth(Mathf.RoundToInt(damage));
                yield return Impact(0.25f * critical);
                break;
        }
    }


    public void AdjustHealth(int amount)
    {
        health += amount;

        Hud.AdjustHealth(health, amount);
    }

    public void AdjustMana(int amount)
    {
        mana += amount;

        Hud.AdjustMana(mana, amount);
    }

    public abstract IEnumerator Damage();

    public abstract IEnumerator Block();

    public abstract IEnumerator Dodge();

}
