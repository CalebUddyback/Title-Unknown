using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public abstract class Combat_Character : MonoBehaviour
{
    public bool cpu = false;

    public string characterName = "";

    public Sprite portrait;

    public Transform enemyTransform;

    public Combat_Character Enemy => enemyTransform.GetComponent<Combat_Character>();

    public Turn_Controller.Team Team { get; set; }

    public AnimationController animationController;

    public Outcome_Bubble outcome_Bubble_Prefab;
    public Outcome_Bubble current_Outcome_Bubble;
    public Transform outcome_Bubble_Pos;

    public ParticleSystem blood;

    [HideInInspector]
    public Target_Arrow target_Arrow;

    [HideInInspector]
    public GameObject reaction_Arrow;

    public enum Phase {Main, Action, End, Waiting, Reacting}
    public Phase currentPhase = Phase.Waiting;

    public Transform skills;

    [HideInInspector]
    public Decks deck;

    public int Facing { get; set; } = 1;

    private bool firstTurn = true;

    public bool reacting = false;


    [Header("Turn Controller")]

    public bool doneTurn = false;

    public Character_Hud Hud;

    public Turn_Controller TurnController { get; set; }

    public int Health { get; set; }

    public void AdjustHealth(Sprite spr, int change, string type, float mutiplier)
    {
        int former = Health;

        Health = Mathf.Clamp(Health + change, 0, character_Stats.max_Health);

        if(change <= 0)
        {
            if (mutiplier > 1)
            {
                OutcomeBubble("CRITICAL", Color.yellow);
                OutcomeBubble(spr, change, type, Color.yellow);
                TurnController.mainCamera.WhiteOut(this, this, 0.25f * mutiplier);
            }
            else
            {
                OutcomeBubble(spr, change, type, Color.red);
            }
        }
        else
        {
            OutcomeBubble(spr, change, "+HP", Color.green);
        }

        switch (Health)
        {
            case 0:
                Defeated = true;
                break;
        }

        Hud.healthBar.Adjust(former, Health);
    }

    public int Defense { get; set; }

    public void AdjustDefense(Sprite spr, int change, float mutiplier)
    {
        bool overflow = true;

        bool capTopBar = true;

        int former = Defense;

        if (!overflow)
            Defense = Mathf.Clamp(Defense + change, 0, character_Stats.max_Defense);
        else
        {
            if (capTopBar)
                Defense = Mathf.Clamp(Defense + change, 0, character_Stats.max_Defense * Hud.defenseBar.displayBarTransform.childCount);
            else
                Defense = Mathf.Clamp(Defense + change, 0, 1000);
        }

        change = Defense - former;

        if (change <= 0)
            OutcomeBubble(spr, change, "Block", Color.white);
        else
            OutcomeBubble(spr, change, "+DF", Color.white);

        Hud.defenseBar.Adjust(former, Defense);
    }

    public void ClearDefenseOverflow()
    {
        if (Defense > character_Stats.max_Defense)
        {
            Defense = character_Stats.max_Defense;

            Hud.defenseBar.ClearOverflow();
        }
    }

    public int Mana { get; set; }

    public void AdjustMana(int change, bool show)
    {
        int former = Mana;

        Mana = Mathf.Clamp(Mana + change, 0, character_Stats.max_Mana);

        if (show)
        {
            OutcomeBubble(null, change, "+MP", new Color(0, 0.5019608f, 1));
        }

        switch (Mana)
        {
            case 0:
                
                break;
        }

        Hud.manaBar.Adjust(former, Mana);
    }

    public IEnumerator Charging()
    {
        Hud.timer_ChargeIndicator.SetActive(true);

        yield return Hud.ScrollTimerTo(deck.SelectedSlot.card.skill.chargeTime);
    }

    public IEnumerator StartTurn()
    {
        yield return deck.Raise(false);

        yield return new WaitForSeconds(0.5f);

        // Draw

        bool selectedDraw = Random.Range(0, 100) < 0 ? true : false;

        if (deck.drawDeck.childCount < 3)
            selectedDraw = false;

        int d = 5;

        if (selectedDraw)
            d--;

        if (firstTurn)
        {
            yield return deck.DrawCards(d, true, false);
        }
        else
        {
            //int d = (hand.cards.Count < 5) ? 5 - hand.cards.Count : 1;
   
            yield return deck.DrawCards(d, !selectedDraw, false);

            if (selectedDraw)
                yield return StartCoroutine(TurnController.draw_Selection.ChooseCard());
        }

        int startMP = 20;
        AdjustMana(startMP, true);

        ClearDefenseOverflow();

        // Main

        currentPhase = Phase.Main;

        // Turn controller should check all hands for usable cards

        TurnController.CheckAllCards();

        TurnController.instructions.text = "Select Card";

    }

    public IEnumerator EndTurn()
    {
        TurnController.endTurnButton.interactable = false;

        if (deck.cardsPlayed.Count == 0)
        {
            int restMP = 20;
            AdjustMana(restMP, true);
        }

        deck.cardsPlayed.Clear();

        deck.distinctTargets.Add(this);

        deck.distinctTargets = deck.distinctTargets.Distinct().ToList();

        // End 

        currentPhase = Phase.End;

        TurnController.CheckAllCards();

        //deck.ResetPreviousSlot();

        yield return deck.Clear();

        /** Yu-gi-oh style clean up phase **/
        //if (hand.cards.Count > hand.maxCardsInHand)
        //{
        //    hand.ResetPreviousSlot();
        //    hand.SelectedSlot = null;
        //    yield return hand.DiscardCards(hand.cards.Count - hand.maxCardsInHand);
        //}

        TurnController.instructions.text = "";

        List<Coroutine> anims = new List<Coroutine>();

        foreach (Combat_Character target in deck.distinctTargets)
            anims.Add(StartCoroutine(target.ResetAnimation()));

        foreach (Coroutine co in anims)
            yield return co;

        yield return null;

        //yield return cards.Lower();

        firstTurn = false;

        // Done

        currentPhase = Phase.Waiting;
    }

    public IEnumerator Interuption()
    {

        yield return null;
    }

    public abstract IEnumerator CpuDecisionMaking();


    public IEnumerator MoveInRange(Vector2 range)
    {
        Vector3 startPos = transform.position;

        float min, max;

        if (enemyTransform.position.x + (range.x * Facing) < enemyTransform.position.x + (range.y * Facing))
        {
            min = enemyTransform.position.x + (range.x * Facing);
            max = enemyTransform.position.x + (range.y * Facing);
        }
        else
        {
            min = enemyTransform.position.x + (range.y * Facing);
            max = enemyTransform.position.x + (range.x * Facing);
        }

        float targetRange = Mathf.Clamp(transform.position.x, min, max);

        Vector3 targetPos = new Vector3(targetRange, transform.position.y , enemyTransform.position.z);

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

    public void OutcomeBubble(Sprite spr, int num, string type, Color col1)
    {
        float distance = 0;

        if (type == Turn_Controller.Effect.None.ToString())
            type = "";

        if (current_Outcome_Bubble != null)
        {
            distance = Vector3.Distance(current_Outcome_Bubble.GetComponent<RectTransform>().anchoredPosition, TurnController.mainCamera.UIPosition(outcome_Bubble_Pos.position));
        }

        if (current_Outcome_Bubble == null || distance > 30f)
        {
            current_Outcome_Bubble = Instantiate(outcome_Bubble_Prefab, TurnController.damage_Bubbles);
            current_Outcome_Bubble.GetComponent<RectTransform>().anchoredPosition = TurnController.mainCamera.UIPosition(outcome_Bubble_Pos.position);
        }

        current_Outcome_Bubble.Input(spr, num, type, col1);
    }

    public void OutcomeBubble(string str, Color col1)
    {
        float distance = 0;

        if (current_Outcome_Bubble != null)
        {
            distance = Vector3.Distance(current_Outcome_Bubble.GetComponent<RectTransform>().anchoredPosition, TurnController.mainCamera.UIPosition(outcome_Bubble_Pos.position));

            //Debug.Log(distance + " " + current_Outcome_Bubble.GetComponent<RectTransform>().anchoredPosition + " " + TurnController.mainCamera.UIPosition(outcome_Bubble_Pos.position));
        }

        if (current_Outcome_Bubble == null || distance > 30f)
        {
            current_Outcome_Bubble = Instantiate(outcome_Bubble_Prefab, TurnController.damage_Bubbles);
            current_Outcome_Bubble.GetComponent<RectTransform>().anchoredPosition = TurnController.mainCamera.UIPosition(outcome_Bubble_Pos.position);
        }

        current_Outcome_Bubble.Input(str, col1);
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
                Enemy.OutcomeBubble("MISS!", Color.white);

                animationController.Play();
                Enemy.animationController.Play();

                dodge = StartCoroutine(Enemy.Dodge());
                break;

            case 1:

                if (Enemy.Defense > 0)
                {
                    if (Enemy.Defense + damage <= 0)
                    {
                        int excess = damage + Enemy.Defense;

                        Enemy.AdjustDefense(TurnController.GetSprite(Turn_Controller.Effect.Block), damage, skill.CritSuccess);

                        StartCoroutine(Enemy.Break());

                        yield return new WaitForSeconds(0.25f); // Grow length

                        Enemy.animationController.Play();

                        yield return Enemy.WaitForKeyFrame();

                        Enemy.OutcomeBubble("BREAK!", Color.white);

                        //Enemy.animationController.Pause();

                        //yield return new WaitForSeconds(0.25f); // Grow length

                        if (excess < 0)
                        {
                            skill.Character.Team.combo_Counter.SetComboCount();

                            Enemy.AdjustHealth(TurnController.GetSprite(skill.effect), excess, skill.effect.ToString(), skill.CritSuccess);

                            Enemy.blood.Play();
                        }

                    }
                    else
                    {
                        StartCoroutine(Enemy.Block());
                        Enemy.AdjustDefense(TurnController.GetSprite(Turn_Controller.Effect.Block), damage, skill.CritSuccess);
                    }

                    

                    //if (blocking)
                    //{
                    //    blocking = false;
                    //
                    //    animationController.Clip("Block_Unset");
                    //
                    //    yield return animationController.coroutine;
                    //
                    //    print("Done");
                    //}
                }
                else
                {
                    skill.Character.Team.combo_Counter.SetComboCount();

                    StartCoroutine(Enemy.Damage());

                    Enemy.AdjustHealth(TurnController.GetSprite(skill.effect), damage, skill.effect.ToString(), skill.CritSuccess);

                    Enemy.blood.Play();
                }

                yield return new WaitForSeconds(0.25f * skill.CritSuccess); // Impact Delay

                animationController.Play();
                Enemy.animationController.Play();

                yield return Enemy.MoveAmount(new Vector3(skill.intervals.knockBack.x * Facing, skill.intervals.knockBack.y, skill.intervals.knockBack.z), 0.1f); ;
                break;
        }

        yield return dodge;

        yield return WaitForKeyFrame();

        animationController.Pause();
        Enemy.animationController.Pause();

        //yield return animationController.coroutine;

        if (Enemy.Defeated)
        {
            Enemy.animationController.Clip("Defeated");
            yield return Enemy.animationController.coroutine;
        }
    }

    public IEnumerator ResetAnimation()
    {
        animationController.Play();

        yield return animationController.coroutine;

        Debug.Log(characterName + " done");

        animationController.Clip("Idle");
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
        //yield return animationController.coroutine;
        yield return null;
    }

    public virtual IEnumerator Break()
    {
        Debug.Log("Block_Break");
        animationController.Clip("Block_Break");
        yield return animationController.coroutine;
    }

    public virtual IEnumerator Dodge()
    {
        animationController.Clip("Move_BackDash");

        yield return MoveAmount(new Vector3(0.3f * -Facing, 0, 0));

        yield return animationController.coroutine;
    }

    public int Reaction { get; set; }

    public int React()
    {
        return Reaction = Random.Range(0, character_Stats.initiative);
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

}
