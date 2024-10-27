using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public abstract class Combat_Character : MonoBehaviour
{
    public bool cpu = false;

    public string characterName = "";

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
        public int chargeTime;              //could be a manditory call before every attack. Make zero for no charge before execution
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

            var chances = character.GetCombatStats(stats, target);

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

    public Skill chosenAction;

    public Spell[] setSpells = new Spell[3];

    public int Facing { get; set; } = 1;


    [Header("Turn Controller")]

    public bool spotLight = false;

    public Character_Hud Hud;

    public Turn_Controller TurnController { get; set; }

    //public string characterName = "No Name";
    public int Health
    {
        get
        {
            return character_Stats.health;
        }
        set
        {
            int former = character_Stats.health;

            character_Stats.health = value;

            if (character_Stats.health <= 0)
            {
                character_Stats.health = 0;
                Defeated = true;
            }

            if (character_Stats.health > character_Stats.max_Health)
                character_Stats.health = character_Stats.max_Health;

            Hud.healthBar.Adjust(former, character_Stats.health);
        }
    }

    public int Mana
    {
        get
        {
            return character_Stats.mana;
        }
        set
        {
            int former = character_Stats.mana;

            character_Stats.mana = value;

            if (character_Stats.mana <= 0)
                character_Stats.mana = 0;
            
            if (character_Stats.mana > character_Stats.max_Mana)
                character_Stats.mana = character_Stats.max_Mana;

            Hud.manaBar.Adjust(former, character_Stats.mana);
        }
    }

    public bool blocking = false;

    public IEnumerator StartFocus()
    {
        int totalTime = GetCurrentStats()[Character_Stats.Stat.AS];

        yield return Hud.ScrollTimerTo(totalTime);
    }


    public IEnumerator Charging()
    {
        Hud.timer_ChargeIndicator.SetActive(true);

        yield return Hud.ScrollTimerTo(chosenAction.chargeTime);
    }

    public void StartTurn()
    {
        spotLight = true;

        blocking = false;

        IncrementStatChangers(TurnController.comboState);

        if (cpu)
            StartCoroutine(CpuDecisionMaking());
        else
        {
            if (chosenAction != null && chosenAction.charging)
            {
                StartCoroutine(chosenAction.SubMenus(this));
            }
            else
                StartCoroutine(SubMenuController.OpenSubMenu("Actions", new List<string> { "Attack", "Defend", "Items", "Rest" }));
        }

    }

    public void EndTurn()
    {
        SubMenuController.ResetMenus();

        spotLight = false;

        //StartCoroutine(Charging(chosenAttack.chargeTime));
    }

    public Transform uiPoint;

    //Not Used for now but will be usful for world to ui canvas point tracking

    public void MenuPositioning()
    {
        Vector3 targPos = uiPoint.position;

        Vector3 camForward = TurnController.mainCamera.transform.forward;

        Vector3 camPos = TurnController.mainCamera.transform.position + camForward;

        float distanceInFrontOfCamera = Vector3.Dot(targPos - camPos, camForward);

        if(distanceInFrontOfCamera < 0f)
        {
            targPos -= camForward * distanceInFrontOfCamera;
        }

        SubMenuController.gameObject.transform.position = RectTransformUtility.WorldToScreenPoint(TurnController.mainCamera.cam, targPos);
    }

    public abstract IEnumerator CpuDecisionMaking();

    public void ActionChoice(Skill attack)
    {
        chosenAction = attack;
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
        //animationController.Clip("Sakura Idle");    //this to allow for repeat calls

        TurnController.mainCamera.SetIdleSway(0.1f);

        Hud.timer_ChargeIndicator.SetActive(false);

        yield return chosenAction.Execute;

        TurnController.mainCamera.ResetIdleSway();
    }

    public IEnumerator MoveInRange(Vector3 range)
    {
        Vector3 startPos = transform.position;

        Vector3 targetPos = new Vector3(enemyTransform.position.x + range.x * Facing, transform.position.y , enemyTransform.position.z);

        if (startPos == targetPos)
            yield break;

        animationController.Clip("Sakura Idle");

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
        if (transform.position != startingPos)
        {
            Vector3 currentPos = transform.position;

            float timer = 0;
            float maxTime = 0.3f;

            animationController.Clip(characterName + " Idle");

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

        if(chosenAction != null && chosenAction.charging && chosenAction.chargeAnimation != "")
            animationController.Clip(chosenAction.chargeAnimation);      // this should be a check on the chosenskill to see if charge was BROKEN and whether to continue or not

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

    public IEnumerator ApplyOutcome(int success, int critical, int damage)
    {

        damage *= -1;
        damage *= critical;

        //critical = 3;

        switch (success)
        {
            case 0:
                Instantiate(outcome_Bubble_Prefab, enemy.outcome_Bubble_Pos.position, Quaternion.identity).Input("MISS");
                yield return StartCoroutine(enemyTransform.GetComponent<Combat_Character>().Dodge());
                break;

            case 1:

                enemy.Health += damage;

                if (enemy.blocking)
                {
                    StartCoroutine(enemy.Block());
                }
                else
                {
                    TurnController.combo_Counter.SetComboCount();
                    StartCoroutine(enemy.Damage());
                }

                if (critical != 1)
                {
                    Instantiate(outcome_Bubble_Prefab, enemy.outcome_Bubble_Pos.position, Quaternion.identity).Input(damage, "CRITICAL", Color.yellow);
                    TurnController.mainCamera.WhiteOut(this, enemy, 0.25f * critical);
                }
                else
                    Instantiate(outcome_Bubble_Prefab, enemy.outcome_Bubble_Pos.position, Quaternion.identity).Input(damage);


                yield return Impact(0.25f * critical);

                if (enemy.Defeated)
                {
                    yield return null;
                    yield return enemy.animationController.coroutine;
                    yield return null;
                    enemy.animationController.Clip(enemy.characterName + " Defeated");
                    yield return enemy.animationController.coroutine;
                }

                break;
        }
    }


    public bool Defeated{ get; private set; }


    public abstract IEnumerator Damage();

    public abstract IEnumerator Block();

    public abstract IEnumerator Dodge();

    public class Defense : Skill
    {
        public Defense(Combat_Character character) : base(character)
        {
            this.character = character;

            skill_Stats = new Skill_Stats[]
            {
                new Skill_Stats
                {
                    attack = 5,
                    accuracy = -5,
                    critical = 5,

                    statChanger = new StatChanger
                    (
                        new Dictionary<Character_Stats.Stat, int>
                        {
                            {Character_Stats.Stat.DEF, 0 },
                            {Character_Stats.Stat.PhAvo, -1000 },
                            {Character_Stats.Stat.AS, 10 },
                        }
                    )
                    {
                        name = "Defense Exhaust",
                    }
                },

            };
        }

        public override IEnumerator SubMenus(MonoBehaviour owner)
        {
            bool done = false;

            int i = 0;

            while (!done)
            {

                switch (i)
                {
                    case 0:

                        yield return character.SubMenuController.OpenSubMenu("Confirm", new List<string>() { "Confirm" });

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {
                            skill_Stats[0].statChanger.statChanges[Character_Stats.Stat.DEF] = character.GetCurrentStats()[Character_Stats.Stat.STR] + character.weapon.defense;
                            character.AddStatChanger(skill_Stats[0].statChanger);

                            done = true;
                        }
                        else
                        {
                            character.chosenAction = null;
                            yield break;
                        }

                        break;
                }

            }

            Execute = Action();
        }

        public IEnumerator Action()
        {
            //character.animationController.Clip(character.characterName + " Block");

            character.blocking = true;

            Instantiate(character.outcome_Bubble_Prefab, character.outcome_Bubble_Pos.position, Quaternion.identity).Input("DEFENSE");

            yield return null;
        }
    }
    public Defense defense;

    public class Rest : Skill
    {
        public Rest(Combat_Character character) : base(character)
        {
            this.character = character;

            skill_Stats = new Skill_Stats[]
            {
                new Skill_Stats
                {
                    statChanger = new StatChanger
                    (
                        new Dictionary<Character_Stats.Stat, int>
                        {
                            {Character_Stats.Stat.AS, 2 },
                        }
                    )
                    {
                        name = "Rest Exhaust",
                    }
                },


            };
        }

        public override IEnumerator SubMenus(MonoBehaviour owner)
        {
            bool done = false;

            int i = 0;

            while (!done)
            {

                switch (i)
                {
                    case 0:

                        yield return character.SubMenuController.OpenSubMenu("Confirm", new List<string>() { "Confirm" });

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {
                            character.AddStatChanger(skill_Stats[0].statChanger);

                            done = true;
                        }
                        else
                        {
                            character.chosenAction = null;
                            yield break;
                        }

                        break;
                }

            }

            Execute = Action();
        }

        public IEnumerator Action()
        {
            character.animationController.Clip(character.characterName + " Buff");

            yield return character.WaitForKeyFrame();

            Instantiate(character.outcome_Bubble_Prefab, character.outcome_Bubble_Pos.position, Quaternion.identity).Input("REST");

            character.Mana += 20;

            yield return character.animationController.coroutine;
            character.animationController.Clip(character.characterName + " Idle");
        }
    }
    public Rest rest;

    public virtual void InitializeSkills()
    {
        defense = new Defense(this);
        rest = new Rest(this);
    }



    /****STATS ****/

    [Header("Stats/Equipment")]
    
    public Character_Stats character_Stats;

    public Weapon weapon;

    [Header("Buffs/Debuffs")]

    [SerializeField]
    private List<StatChanger> statChangers = new List<StatChanger>();

    private Dictionary<Character_Stats.Stat, int> GetCoreStats()
    {
        var baseStats = new Dictionary<Character_Stats.Stat, int>
        {
            {Character_Stats.Stat.STR, character_Stats.strength },
            {Character_Stats.Stat.MAG, character_Stats.magic },
            {Character_Stats.Stat.DEX, character_Stats.dexterity },
            {Character_Stats.Stat.SPD, character_Stats.speed},
            {Character_Stats.Stat.DEF, character_Stats.defense},
            {Character_Stats.Stat.RES, character_Stats.resistance},
            {Character_Stats.Stat.LCK, character_Stats.luck},
        };

        return baseStats;
    }

    private Dictionary<Character_Stats.Stat, int> GetOffenseStats(Dictionary<Character_Stats.Stat, int> core)
    {

        var offenseStats = new Dictionary<Character_Stats.Stat, int>()
        {
            {Character_Stats.Stat.ATK, core[Character_Stats.Stat.STR]},

            {Character_Stats.Stat.PhHit, core[Character_Stats.Stat.DEX]},
            {Character_Stats.Stat.PhAvo, core[Character_Stats.Stat.SPD]},

            {Character_Stats.Stat.MgHit, (core[Character_Stats.Stat.DEX] + core[Character_Stats.Stat.LCK]) / 2 },
            {Character_Stats.Stat.MgAvo, (core[Character_Stats.Stat.SPD] + core[Character_Stats.Stat.LCK]) / 2 },

            {Character_Stats.Stat.Crit, (core[Character_Stats.Stat.DEX] + core[Character_Stats.Stat.LCK]) / 2 },
            {Character_Stats.Stat.CritAvo, core[Character_Stats.Stat.LCK] },

            {Character_Stats.Stat.AS, 21 - core[Character_Stats.Stat.SPD]},
        };

        return offenseStats;
    }

    private Dictionary<Character_Stats.Stat, int> GetBaseStats()
    {
        var baseStats = GetCoreStats();

        foreach (var stat in GetOffenseStats(baseStats))
            baseStats.Add(stat.Key, stat.Value);

        baseStats[Character_Stats.Stat.ATK] += weapon.attack;
        baseStats[Character_Stats.Stat.PhHit] += weapon.accuracy;
        baseStats[Character_Stats.Stat.Crit] += weapon.critical;

        return baseStats;
    }

    public Dictionary<Character_Stats.Stat, int> GetCurrentStats()
    {
        var currentStats = GetCoreStats();

        foreach (var changer in statChangers)
        {
            for (int i = 0; i < 7; i++)
            {
                Character_Stats.Stat stat = currentStats.ElementAt(i).Key;
                currentStats[stat] += changer.statChanges[stat];
            }
        }

        foreach (var stat in GetOffenseStats(currentStats))
            currentStats.Add(stat.Key, stat.Value);

        foreach (var changer in statChangers)
        {
            for (int i = 7; i < 15; i++)
            {
                Character_Stats.Stat stat = currentStats.ElementAt(i).Key;
                currentStats[stat] += changer.statChanges[stat];
            }
        }

        currentStats[Character_Stats.Stat.ATK] += weapon.attack;
        currentStats[Character_Stats.Stat.PhHit] += weapon.accuracy;
        currentStats[Character_Stats.Stat.Crit] += weapon.critical;

        return currentStats;
    }


    public Dictionary<Character_Stats.Stat, int> GetCombatStats(Combat_Character.Skill.Skill_Stats skillStats)
    {
        Dictionary<Character_Stats.Stat, int> combatStats = GetCurrentStats();

        combatStats[Character_Stats.Stat.ATK] += skillStats.attack;
        combatStats[Character_Stats.Stat.PhHit] += skillStats.accuracy;
        combatStats[Character_Stats.Stat.Crit] += skillStats.critical;

        if (skillStats.statChanger != null)
            combatStats[Character_Stats.Stat.AS] += skillStats.statChanger.statChanges[Character_Stats.Stat.AS];

        return combatStats;
    }

    public Dictionary<Character_Stats.Stat, int> GetCombatStats(Combat_Character.Skill.Skill_Stats skillStats, Transform target)
    {
        return GetCombatStats(skillStats, target.GetComponent<Combat_Character>());
    }

    public Dictionary<Character_Stats.Stat, int> GetCombatStats(Combat_Character.Skill.Skill_Stats skillStats, Combat_Character target)
    {
        Dictionary<Character_Stats.Stat, int> ownerStats = GetCombatStats(skillStats);

        Dictionary<Character_Stats.Stat, int> targetStats = target.GetCurrentStats();

        ownerStats[Character_Stats.Stat.ATK] = Mathf.Clamp(ownerStats[Character_Stats.Stat.ATK] - targetStats[Character_Stats.Stat.DEF], 0, 9999);

        ownerStats[Character_Stats.Stat.PhHit] = Mathf.Clamp(ownerStats[Character_Stats.Stat.PhHit] - targetStats[Character_Stats.Stat.PhAvo], 0, 100);

        ownerStats[Character_Stats.Stat.Crit] = Mathf.Clamp(ownerStats[Character_Stats.Stat.Crit] - targetStats[Character_Stats.Stat.CritAvo], 0, 100);

        return ownerStats;
    }

    public Color CompareStat(Character_Stats.Stat stat, int value, bool reverse)
    {
        int i = 0;

        if (GetBaseStats()[stat] < value)
            i = 1;

        if (GetBaseStats()[stat] > value)
            i = -1;

        if (reverse)
            i *= -1;

        if (i == 1)
            return Color.blue;
        else if (i == -1)
            return Color.red;
        else
            return Color.white;
    }

    public void AddStatChanger(StatChanger statChanger)
    {
        statChangers.Add(statChanger);
    }

    public void RemoveStatChanger(StatChanger statChanger)
    {
        statChangers.Remove(statChanger);
    }

    public void IncrementStatChangers(bool comboState)
    {
        // Statchangers should have individual logic that is called thorugh abstract methods

        for (int i = 0; i < statChangers.Count;)
        {
            statChangers[i].duration += statChangers[i].incrementDirection;

            if (statChangers[i].duration <= 0)
                statChangers.RemoveAt(i);
            else
                i++;
        }

    }

    public void ClearStatChangers()
    {
        statChangers.Clear();
    }

}
