using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;

public abstract class Combat_Character : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public bool cpu = false;

    public string characterName = "";

    public Transform enemyTransform;

    public Combat_Character Enemy => enemyTransform.GetComponent<Combat_Character>();

    public AnimationController animationController;

    public Outcome_Bubble outcome_Bubble_Prefab;

    public Transform outcome_Bubble_Pos;

    public GameObject target_Arrow;

    public enum Phase {Waiting, Draw, Main, Action, End}
    public Phase currentPhase = Phase.Waiting;

    public Transform skills;

    [HideInInspector]
    public Decks cards;

    public int Facing { get; set; } = 1;

    private bool firstTurn = true;


    [Header("Turn Controller")]

    public bool doneTurn = false;

    public Character_Hud Hud;

    public Turn_Controller TurnController { get; set; }

    //public string characterName = "No Name";

    private int health;

    public int InitialHealth
    {
        set
        {
            health = value;  
        }
    }

    public int Health()
    {
        return health;
    }

    public void AdjustHealth(int change, float mutiplier)
    {
        int former = health;

        health += change;

        Outcome_Bubble bubble = Instantiate(outcome_Bubble_Prefab, TurnController.damage_Bubbles);
        bubble.GetComponent<RectTransform>().anchoredPosition = TurnController.mainCamera.UIPosition(outcome_Bubble_Pos.position);

        if(change <= 0)
        {
            if (mutiplier > 1)
            {
                bubble.Input(change, Color.yellow, "CRITICAL", Color.yellow);
                TurnController.mainCamera.WhiteOut(this, this, 0.25f * mutiplier);
            }
            else
            {
                bubble.Input(change, Color.red);
            }
        }
        else
        {
            bubble.Input(change, Color.green);
        }

        if (health <= 0)
        {
            health = 0;
            Defeated = true;
        }

        if (health > character_Stats.max_Health)
            health = character_Stats.max_Health;

        Hud.healthBar.Adjust(former, health);
    }


    private int mana;

    public int InitialMana
    {
        set
        {
            mana = value;
        }
    }

    public int Mana()
    {
        return mana;
    }

    public void AdjustMana(int change, bool show)
    {
        int former = mana;

        mana += change;

        if (show)
        {
            Outcome_Bubble bubble = Instantiate(outcome_Bubble_Prefab, TurnController.damage_Bubbles);
            bubble.GetComponent<RectTransform>().anchoredPosition = TurnController.mainCamera.UIPosition(outcome_Bubble_Pos.position);

            if (change > 0)
                bubble.Input(change, new Color(0, 0.5019608f, 1));
            else
                bubble.Input(change, Color.white);



        }

        if (mana > character_Stats.max_Mana)
            mana = character_Stats.max_Mana;

        Hud.manaBar.Adjust(former, mana);
    }

    public bool blocking = false;

    public IEnumerator Charging()
    {
        Hud.timer_ChargeIndicator.SetActive(true);

        yield return Hud.ScrollTimerTo(cards.SelectedSlot.card.skill.chargeTime);
    }

    public IEnumerator StartTurn()
    {
        TurnController.CheckAllCards();

        yield return cards.Raise();

        yield return new WaitForSeconds(0.5f);

        // Draw

        bool selectedDraw = Random.Range(0, 100) < 0 ? true : false;

        if (cards.drawDeck.childCount < 3)
            selectedDraw = false;

        int d = 5;

        if (selectedDraw)
            d--;

        if (firstTurn)
        {
            yield return cards.DrawCards(d, true);
        }
        else
        {
            //int d = (hand.cards.Count < 5) ? 5 - hand.cards.Count : 1;
   
            yield return cards.DrawCards(d, !selectedDraw);

            if (selectedDraw)
                yield return StartCoroutine(TurnController.draw_Selection.ChooseCard());
        }

        cards.Locked = false;

        if (blocking)
        {
            blocking = false;

            animationController.Clip("Block_Unset");

            yield return animationController.coroutine;

            print("Done");
        }


        int startMP = 20;
        AdjustMana(startMP, true);

        currentPhase = Phase.Draw;

        TurnController.CheckAllCards();

        // Main

        currentPhase = Phase.Main;

        // Turn controller should check all hands for usable cards

        TurnController.CheckAllCards();

        TurnController.instructions.text = "Select Card";

    }

    public IEnumerator EndTurn()
    {
        TurnController.endTurnButton.interactable = false;

        if (cards.cardsPlayed.Count == 0)
        {

            int restMP = 20;
            AdjustMana(restMP, true);
        }

        cards.cardsPlayed.Clear();

        cards.distinctTargets.Add(transform);

        cards.distinctTargets = cards.distinctTargets.Select(o => o.transform).Distinct().ToList();

        // End 

        currentPhase = Phase.End;

        TurnController.CheckAllCards();

        cards.ResetPreviousSlot();

        yield return cards.Clear();

        /** Yu-gi-oh style clean up phase **/
        //if (hand.cards.Count > hand.maxCardsInHand)
        //{
        //    hand.ResetPreviousSlot();
        //    hand.SelectedSlot = null;
        //    yield return hand.DiscardCards(hand.cards.Count - hand.maxCardsInHand);
        //}

        TurnController.instructions.text = "";

        TurnController.ResetAnimations();

        yield return null;

        yield return cards.Lower();

        firstTurn = false;

        // Done

        currentPhase = Phase.Waiting;
    }

    public abstract IEnumerator CpuDecisionMaking();


    public IEnumerator MoveInRange(Vector3 range)
    {
        Vector3 startPos = transform.position;

        Vector3 targetPos = new Vector3(enemyTransform.position.x + (range.x * Facing), transform.position.y , enemyTransform.position.z);

        if (startPos == targetPos)
            yield break;

        animationController.Clip("Idle");

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

    public IEnumerator MoveAmount(Vector3 amount)
    {
        yield return MoveAmount(amount, 0.25f);
    }

    public IEnumerator MoveAmount(Vector3 amount, float t)
    {
        Vector3 startPos = transform.position;

        Vector3 targetPos = startPos + amount;

        if (transform.position == targetPos)
            yield break;

        float timer = 0;
        float maxTime = t;

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
        if (transform.localPosition != Vector3.zero)
        {
            Vector3 currentPos = transform.localPosition;

            float timer = 0;
            float maxTime = 0.3f;

            while (timer < maxTime)
            {
                float xLerp = Mathf.Lerp(currentPos.x, 0f, timer / maxTime);

                //float yLerp = Mathf.Lerp(currentPos.y, startingPos.y, timer / maxTime);

                float zLerp = Mathf.Lerp(currentPos.z, 0f, timer / maxTime);

                transform.localPosition = new Vector3(xLerp, transform.position.y, zLerp);

                timer += Time.deltaTime;

                yield return null;
            }

            transform.localPosition = Vector3.zero;

        }

    }


    public IEnumerator WaitForKeyFrame()
    {
        yield return new WaitUntil(() => animationController.eventFrame == true);

        animationController.eventFrame = false;
    }

    public IEnumerator ApplyOutcome(Skill skill)
    {

        int damage = GetCurrentStats(skill)[Character_Stats.Stat.STR];

        damage *= -1;

        damage *= skill.CritSuccess;

        damage = Mathf.Clamp(damage, -999, 999); // outcome bubble can only display 3 digits

        // damage should NOT be changed beyond this point


        Coroutine dodge = null;

        switch (skill.HitSuccess)
        {
            case 0:
                Outcome_Bubble bubble = Instantiate(outcome_Bubble_Prefab, TurnController.damage_Bubbles);
                bubble.GetComponent<RectTransform>().anchoredPosition = TurnController.mainCamera.UIPosition(Enemy.outcome_Bubble_Pos.position);
                bubble.Input("MISS", Color.white);

                animationController.Play();
                Enemy.animationController.Play();

                dodge = StartCoroutine(Enemy.Dodge());
                break;

            case 1:

                if (Enemy.blocking)
                {
                    if (Enemy.Mana() + damage <= 0)
                    {
                        Enemy.animationController.Clip("Block_Break");
                        Enemy.blocking = false;
                    }
                    else
                        StartCoroutine(Enemy.Block());

                    Enemy.AdjustMana(damage, true);
                }
                else
                {

                    if (TurnController.left_Players.Contains(this))
                        TurnController.left_Combo_Counter.SetComboCount();
                    else
                        TurnController.right_Combo_Counter.SetComboCount();

                    StartCoroutine(Enemy.Damage());
                    yield return null;

                    Enemy.AdjustHealth(damage, skill.CritSuccess);
                }

                yield return new WaitForSeconds(0.25f * skill.CritSuccess); // Impact Delay

                animationController.Play();
                Enemy.animationController.Play();

                yield return Enemy.MoveAmount(new Vector3(skill.intervals.knockBack.x * Facing, skill.intervals.knockBack.y, skill.intervals.knockBack.z), 0.1f); ;
                break;
        }

        yield return dodge;
        yield return animationController.coroutine;

        if (Enemy.Defeated)
        {
            Enemy.animationController.Clip("Defeated");
            yield return Enemy.animationController.coroutine;
        }
    }


    public bool Defeated{ get; private set; }


    public virtual IEnumerator Damage()
    {
        animationController.Clip("Move_Hurt");
        yield return null;
    }

    public virtual IEnumerator Block()
    {
        animationController.Clip("Block_Impact");
        yield return animationController.coroutine;
    }

    public virtual IEnumerator Dodge()
    {
        animationController.Clip("Move_BackDash");


        yield return MoveAmount(new Vector3(0.3f * -Facing, 0, 0));

        yield return animationController.coroutine;
    }


    public int reaction;

    public int Reaction
    {
        get
        {
            return reaction = Random.Range(0, character_Stats.initiative);
        }

        set
        {
            reaction = value;
        }
    }



    /****STATS ****/

    [Header("Stats/Equipment")]
    
    public Character_Stats character_Stats;

    public Weapon weapon;

    [Header("Buffs/Debuffs")]

    [SerializeField]
    private List<StatChanger> statChangers = new List<StatChanger>();

    private Dictionary<Character_Stats.Stat, int> GetBaseStats()
    {
        var baseStats = new Dictionary<Character_Stats.Stat, int>
        {
            {Character_Stats.Stat.STR, character_Stats.strength },
            {Character_Stats.Stat.CRT, character_Stats.critical },
            {Character_Stats.Stat.SPD, character_Stats.speed},
            {Character_Stats.Stat.LCK, character_Stats.luck},
            {Character_Stats.Stat.INI, character_Stats.initiative},
        };

        return baseStats;
    }

    public Dictionary<Character_Stats.Stat, int> GetCurrentStats()
    {
        var currentStats = GetBaseStats();

        foreach (var changer in statChangers)
        {
            for (int i = 0; i < changer.statChanges.Count; i++)
            {
                Character_Stats.Stat stat = currentStats.ElementAt(i).Key;
                currentStats[stat] += changer.statChanges[stat];
            }
        }

        if(weapon != null)
        {
            currentStats[Character_Stats.Stat.STR] += weapon.attack;
            currentStats[Character_Stats.Stat.CRT] += weapon.critical;
        }

        return currentStats;
    }

    public Dictionary<Character_Stats.Stat, int> GetCurrentStats(Skill skill)
    {
        var currentStats = GetCurrentStats();

        int attack = Random.Range(skill.DamageVariation.x, skill.DamageVariation.y + 1);

        currentStats[Character_Stats.Stat.STR] += attack;
        currentStats[Character_Stats.Stat.CRT] += skill.critical;

        return currentStats;
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


    public void OnPointerEnter(PointerEventData eventData)
    {
        TurnController.hoveringOver = this;
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (TurnController.hoveringOver == this)
            TurnController.hoveringOver = null;
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        TurnController.selected = this;
    }
}
