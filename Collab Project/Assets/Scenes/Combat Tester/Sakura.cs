﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Sakura : Combat_Character
{


    /* This old skill contains code to allow for seperate targeting submenu loop (in submenu override class) */
    /*
    public class Combo_OLD : Skill
    {
        public Combo_OLD(Combat_Character character) : base(character)
        {
            this.character = character;
    
            name = "Combo OLD";
    
            //chargeTimes = 1f;
            levels = 3;
    
            baseInfo = new Info[]
            {
                 new Info(50, 0, 0, Type.Physical, Range.Close),
                 new Info(70, 0 , 0, Type.Physical, Range.Close),
                 new Info(100, 0, 0, Type.Physical, Range.Close)
            };
    
        }
    
        public override IEnumerator SubMenus(MonoBehaviour owner)
        {
    
            bool done = false;
    
            int charge = 0;
    
            int i = 0;
    
            while (!done)
            {
    
                switch (i)
                {
    
                    case 0:
    
                        List<string> labels = Enumerable.Range(1, levels).Select(n => n.ToString()).ToList();
    
                        yield return character.SubMenuController.OpenSubMenu("Charges", labels);
    
                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {
                            i++;
    
                            charge = character.SubMenuController.CurrentSubMenu.ButtonChoice;
                        }
                        else
                        {
                            character.chosenAttack = null;
    
                            yield break;
                        }
    
    
                        break;
    
                    case 1:
    
                        if (character.chosenAttack.targets.Count > 0)
                            character.chosenAttack.targets.RemoveAt(character.chosenAttack.targets.Count - 1);
    
                        while (true)
                        {
                            yield return character.SubMenuController.OpenSubMenu("Targets", character.TurnController.GetPlayerNames(character.Facing));
    
                            if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                            {
    
                                if (character.Facing == 1)
                                    character.chosenAttack.targets.Add(character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);
                                else
                                    character.chosenAttack.targets.Add(character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);
    
                                if (character.chosenAttack.targets.Count > charge)
                                {
                                    i++;
                                    break;
                                }
    
                            }
                            else
                            {
    
                                // Returning to this submenu
    
                                if (character.chosenAttack.targets.Count <= 0)
                                {
                                    i--;
                                    break;
                                }
                                else
                                {
                                    character.chosenAttack.targets.RemoveAt(character.chosenAttack.targets.Count - 1);
                                }
                            }
                        }
    
    
                        break;
    
                    case 2:
    
                        yield return character.SubMenuController.OpenSubMenu("Confirm", new List<string>() { "Confirm" });
    
                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {
                            done = true;
                        }
                        else
                        {
                            i--;
                        }
    
                        break;
                }
    
            }
    
            Execute = Action(charge);
        }
    
        public IEnumerator Action(int charge)
        {
            character.enemyTransform = targets[0];
    
            character.chosenAttack.GetOutcome();
    
            //CAMERA CONTROL
    
            character.mcamera.GetComponent<MainCamera>().BlackOut(0.9f, 0.5f);
    
            Vector3 camTargetPos = new Vector3(targets[0].position.x, 0.5f, targets[0].position.z - 1.5f);
    
            yield return character.mcamera.GetComponent<MainCamera>().LerpMoveIE(camTargetPos, 0.5f);
    
    
            yield return character.MoveInRange(new Vector3(-0.35f, 0, 0));
    
            character.animationController.Clip("Sakura Punch");
    
            yield return character.WaitForKeyFrame();
    
            //yield return character.TurnController.Reactions(Turn_Controller.Stage.IMPACT, info[0]);
    
            CoroutineWithData cd = new CoroutineWithData(character, character.TurnController.Reactions(Turn_Controller.Stage.IMPACT, currentInfo[0]));
            yield return cd.coroutine;
    
            Coroutine outcome;
    
    
            switch ((int)cd.result)
            {
                case 0:
    
                    print("Continue");
    
                    outcome = character.StartCoroutine(character.ApplyOutcome(currentInfo[0]));
    
                    yield return character.animationController.coroutine;
                    character.animationController.Clip("Sakura Idle");
    
                    yield return outcome;
    
                    break;
    
                case 1:
                    print("Pause");
                    break;
    
                case 2:
                    print("Break");
                    yield break;
    
            }
    
            if (charge == 0)
                yield break;
    
            // two
    
            character.enemyTransform = targets[1];
    
            //character.chosenAttack.GetOutcome();
    
            yield return character.MoveInRange(new Vector3(-0.35f, 0, 0));
    
            character.animationController.Clip("Sakura Uppercut");
    
            yield return character.WaitForKeyFrame();
            outcome = character.StartCoroutine(character.ApplyOutcome(currentInfo[1]));
    
            yield return character.animationController.coroutine;
            character.animationController.Clip("Sakura Idle");
    
            yield return outcome;
    
            if (charge == 1)
                yield break;
    
            // three
    
            character.enemyTransform = targets[2];
    
            //character.chosenAttack.GetOutcome();
    
            yield return character.MoveInRange(new Vector3(-0.35f, 0, 0));
    
            character.animationController.Clip("Sakura Kick");
    
            yield return character.WaitForKeyFrame();
            outcome = character.StartCoroutine(character.ApplyOutcome(currentInfo[2]));
    
            yield return character.animationController.coroutine;
            character.animationController.Clip("Sakura Idle");
    
            yield return outcome;
        }  
    
    }
    public Combo_OLD combo_OLD;
    */

    [System.Serializable]
    public class Combo : Skill
    {
        public Combo(Combat_Character character) : base(character)
        {
            name = "Punch, Punch, Kick!";

            maxLevel = 3;

            skill_Stats = new Skill_Stats[]
            {
                new Skill_Stats
                {
                    attack = 1,

                    statChanger = new StatChanger()
                    {
                        name = "Skill Exhaust",

                        statChanges = new Dictionary<Character_Stats.Stat, float>
                        {  
                            {Character_Stats.Stat.AS, 0.5f },
                        }
                    }
                },

                new Skill_Stats
                {
                    attack = 1,

                    statChanger = new StatChanger()
                    {
                        name = "Skill Exhaust",

                        statChanges = new Dictionary<Character_Stats.Stat, float>
                        {
                            {Character_Stats.Stat.AS, 0.6f },
                        }
                    }
                },

                new Skill_Stats
                {
                    attack = 1,

                    statChanger = new StatChanger()
                    {
                        name = "Skill Exhaust",

                        statChanges = new Dictionary<Character_Stats.Stat, float>
                        {
                            {Character_Stats.Stat.AS, 0.7f },
                        }
                    }
                },

            };

            type = Type.Physical;
            range = Range.Close;

            effect = true;
            description = "Upon each successful hit; STAGGER target by 0.1 seconds.";

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

                        level = 0;

                        List<string> labels = Enumerable.Range(1, maxLevel).Select(n => n.ToString()).ToList();

                        yield return character.SubMenuController.OpenSubMenu("Levels", labels);

                        while (character.SubMenuController.CurrentSubMenu.ButtonChoice == -2)
                        {
                            if (character.SubMenuController.CurrentSubMenu.hoveringButton > -1)
                            {
                                int hovering = character.SubMenuController.CurrentSubMenu.hoveringButton + 1;

                                character.TurnController.descriptionBox.title.text = name + " Level " + hovering;

                                character.TurnController.descriptionBox.ATK_Mult.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "x" + hovering;

                                character.TurnController.descriptionBox.ATK_Mult.SetActive((hovering > 1) ? true : false);

                                float projectedValue = character.character_Stats.GetCombatStats(skill_Stats[hovering-1])[Character_Stats.Stat.AS];
                                int comparison = character.character_Stats.CompareStat(Character_Stats.Stat.AS, projectedValue);
                                character.TurnController.descriptionBox.REC_Num.color = (comparison == 1) ? Color.red : (comparison == -1) ? Color.blue : Color.white;
                                character.TurnController.descriptionBox.REC_Num.text = projectedValue.ToString();

                                character.TurnController.descriptionBox.HIT_Num.text = (skill_Stats[hovering-1].accuracy != 0) ? skill_Stats[hovering-1].accuracy.ToString() : "-";

                                character.TurnController.descriptionBox.CRT_Num.text = (skill_Stats[hovering-1].critical != 0) ? skill_Stats[hovering-1].critical.ToString() : "-";
                            }

                            yield return null;
                        }

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {
                            i++;

                            level = character.SubMenuController.CurrentSubMenu.ButtonChoice + 1;
                        }
                        else
                        {
                            character.chosenAttack = null;
                            yield break;
                        }


                        break;

                    case 1:

                        // Returning to this submenu

                        if (character.chosenAttack.targets.Count > 0)
                            character.chosenAttack.targets.RemoveAt(character.chosenAttack.targets.Count - 1);

                        Combat_Character target = null;

                        yield return character.SubMenuController.OpenSubMenu("Targets", character.TurnController.GetPlayerNames(character.Facing));

                        while (character.SubMenuController.CurrentSubMenu.ButtonChoice == -2)
                        {
                            if (character.SubMenuController.CurrentSubMenu.hoveringButton > -1)
                            {
                                if (character.Facing == 1)
                                    target = character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.hoveringButton].GetComponent<Combat_Character>();

                                character.TurnController.descriptionBox.ATK_Num.text = character.character_Stats.GetCombatStats(skill_Stats[level-1], target)[Character_Stats.Stat.ATK].ToString();
                                character.TurnController.descriptionBox.HIT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[level-1], target)[Character_Stats.Stat.PhHit].ToString();
                                character.TurnController.descriptionBox.CRT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[level-1], target)[Character_Stats.Stat.Crit].ToString();
                            }
                            yield return null;
                        }

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {

                            if (character.Facing == 1)
                                character.chosenAttack.targets.Add(character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);
                            else
                                character.chosenAttack.targets.Add(character.TurnController.left_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);

                            i++;

                        }
                        else
                        {
                            i--;
                        }

                        break;

                    case 2:

                        yield return character.SubMenuController.OpenSubMenu("Confirm", new List<string>() { "Confirm" });

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {
                            character.character_Stats.AddStatChanger(skill_Stats[level-1].statChanger);
                            Execute = Action(level);
                            level = 0;
                            done = true;
                        }
                        else
                        {
                            i--;
                        }

                        break;
                }

            }
        }

        public IEnumerator Action(int level)
        {
            character.enemyTransform = targets[0];

            GetOutcome(skill_Stats[0], targets[0]);

            /*CAMERA CONTROL*/

            character.mcamera.GetComponent<MainCamera>().BlackOut(0.9f, 0.5f);

            Vector3 camTargetPos = new Vector3(targets[0].position.x, 0.5f, targets[0].position.z - 1.5f);

            yield return character.mcamera.GetComponent<MainCamera>().LerpMoveIE(camTargetPos, 0.5f);


            yield return character.MoveInRange(new Vector3(-0.35f, 0, 0));

            character.animationController.Clip("Sakura Punch");

            yield return character.WaitForKeyFrame();

            //yield return character.TurnController.Reactions(Turn_Controller.Stage.IMPACT, info[0]);

            CoroutineWithData cd = new CoroutineWithData(character, character.TurnController.Reactions(Turn_Controller.Stage.IMPACT, this));
            yield return cd.coroutine;

            Dictionary<Character_Stats.Stat, float> ownerDirectory = character.character_Stats.GetCombatStats(skill_Stats[0]);

            Dictionary<Character_Stats.Stat, float> targetDirectory = targets[0].GetComponent<Combat_Character>().character_Stats.GetCurrentStats();

            print(ownerDirectory[Character_Stats.Stat.ATK] + " " + targetDirectory[Character_Stats.Stat.DEF]);

            Coroutine outcome;


            switch ((int)cd.result)
            {
                case 0:

                    print("Continue");

                    outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.character_Stats.GetCombatStats(skill_Stats[0], targets[0])[Character_Stats.Stat.ATK]));

                    if (HitSuccess == 1)
                        targets[0].GetComponent<Combat_Character>().Hud.AffectProgress(0.1f);

                    yield return character.animationController.coroutine;
                    character.animationController.Clip("Sakura Idle");

                    yield return outcome;

                    break;

                case 1:
                    print("Pause");
                    break;

                case 2:
                    print("Break");
                    yield break;

            }

            if (level == 1)
                yield break;

            // two

            yield return character.MoveInRange(new Vector3(-0.35f, 0, 0));

            GetOutcome(skill_Stats[1], targets[0]);

            character.animationController.Clip("Sakura Uppercut");

            yield return character.WaitForKeyFrame();
            outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.character_Stats.GetCombatStats(skill_Stats[1], targets[0])[Character_Stats.Stat.ATK]));

            if (HitSuccess == 1)
                targets[0].GetComponent<Combat_Character>().Hud.AffectProgress(0.1f);

            yield return character.animationController.coroutine;
            character.animationController.Clip("Sakura Idle");

            yield return outcome;

            if (level == 2)
                yield break;

            // three

            yield return character.MoveInRange(new Vector3(-0.35f, 0, 0));

            GetOutcome(skill_Stats[2], targets[0]);

            character.animationController.Clip("Sakura Kick");

            yield return character.WaitForKeyFrame();
            outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.character_Stats.GetCombatStats(skill_Stats[2], targets[0])[Character_Stats.Stat.ATK]));

            if (HitSuccess == 1)
                targets[0].GetComponent<Combat_Character>().Hud.AffectProgress(0.1f);

            yield return character.animationController.coroutine;
            character.animationController.Clip("Sakura Idle");

            yield return outcome;
        }

    }
    public Combo combo;


    [System.Serializable]
    public class Jump_Kick : Skill
    {
        public Jump_Kick(Combat_Character character) : base(character)
        {
            this.character = character;

            name = "Jump Kick";

            skill_Stats = new Skill_Stats[]
            {
                new Skill_Stats
                {
                    attack = 20,
                    accuracy = -5,
                    critical = 10,

                    statChanger = new StatChanger()
                    {
                        name = "Skill Exhaust",

                        statChanges = new Dictionary<Character_Stats.Stat, float>
                        {
                            {Character_Stats.Stat.AS, 1.0f },
                        }
                    }
                }
            };

            effect = true;
            description = "Upon each successful hit; STAGGER target by 0.2 seconds.";
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

                        if (character.chosenAttack.targets.Count > 0)
                            character.chosenAttack.targets.RemoveAt(character.chosenAttack.targets.Count - 1);

                        Combat_Character target = null;

                        yield return character.SubMenuController.OpenSubMenu("Targets", character.TurnController.GetPlayerNames(character.Facing));

                        yield return null;

                        while (character.SubMenuController.CurrentSubMenu.ButtonChoice == -2)
                        {
                            if (character.SubMenuController.CurrentSubMenu.hoveringButton > -1)
                            {
                                if (character.Facing == 1)
                                    target = character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.hoveringButton].GetComponent<Combat_Character>();


                                character.TurnController.descriptionBox.HIT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.PhHit].ToString();
                                character.TurnController.descriptionBox.CRT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.Crit].ToString();
                            }
                            yield return null;
                        }

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {

                            if (character.Facing == 1)
                                character.chosenAttack.targets.Add(character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);
                            else
                                character.chosenAttack.targets.Add(character.TurnController.left_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);

                            i++;

                        }
                        else
                        {
                            character.chosenAttack = null;
                            yield break;
                        }

                        break;

                    case 1:

                        yield return character.SubMenuController.OpenSubMenu("Confirm", new List<string>() { "Confirm" });

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {

                            character.character_Stats.AddStatChanger(skill_Stats[0].statChanger);

                            done = true;
                        }
                        else
                        {
                            i--;
                        }

                        break;
                }

            }


            Execute = Action();
        }

        public IEnumerator Action()
        {
            character.enemyTransform = character.chosenAttack.targets[0];

            GetOutcome(skill_Stats[0], targets[0]);

            yield return character.MoveInRange(new Vector3(-1f, 0, 0));


            float maxTime = 0.4f;

            character.GetComponent<Rigidbody>().isKinematic = true;

            character.animationController.Clip("Sakura Jump");

            character.StartCoroutine(character.JumpInRange(new Vector3(-0.3f, 0.1f, 0), maxTime));

            yield return new WaitForSeconds(0.2085f);

            //gameObject.GetComponent<AnimationController>().Clip("Sakura Fall");

            //yield return new WaitForSeconds((maxTime / 2) - 0.1665f);

            character.animationController.Clip("Sakura Jump Kick");

            yield return character.WaitForKeyFrame();
            Coroutine outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.character_Stats.GetCombatStats(skill_Stats[0], targets[0])[Character_Stats.Stat.ATK]));

            if (HitSuccess == 1)
            {
                targets[0].GetComponent<Combat_Character>().Hud.AffectProgress(0.2f);
                yield return outcome;
            }

            character.GetComponent<Rigidbody>().isKinematic = false;

            yield return character.animationController.coroutine;

            //gameObject.GetComponent<AnimationController>().Clip("Sakura Fall");

            yield return new WaitUntil(() => character.GetComponent<Rigidbody>().velocity.y > -0.1f);

            character.animationController.Clip("Sakura Landing");

            yield return character.animationController.coroutine;

            yield return outcome;
        }
    }
    public Jump_Kick jump_Kick;

    [System.Serializable]
    public class Throw_Kunai : Skill
    {
        public GameObject kunaiPrefab => Resources.Load<GameObject>("Projectiles/Kunai");

        public Throw_Kunai(Combat_Character character) : base(character)
        {
            this.character = character;

            name = "Throw Kunai";

            maxLevel = 2;

            skill_Stats = new Skill_Stats[]
            {
                new Skill_Stats
                {
                    attack = 5,
                    accuracy = -5,
                    critical = 5,

                    statChanger = new StatChanger
                    {
                        statChanges = new Dictionary<Character_Stats.Stat, float>
                        {
                            {Character_Stats.Stat.AS, 0.5f },
                        }
                    }
                },
            };

            range = Range.Far;

            effect = true;
            description = "CHARGE to a max of 2; each charge requires " + (skill_Stats[0].statChanger.statChanges[Character_Stats.Stat.AS]).ToString("F1") + " seconds. Each charge will throw an extra kunai.";
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

                        if (charging)
                            level++;

                        i++;

                        if (level > 0)
                            character.TurnController.descriptionBox.title.text = name + " x " + (level + 1);

                        break;

                    case 1:

                        if (level >= maxLevel)
                        {
                            i = 2;
                            break;
                        }

                        character.TurnController.descriptionBox.container.gameObject.SetActive(true);

                        yield return character.SubMenuController.OpenSubMenu("Charges", new List<string>() { "Execute", "Charge" });

                        while (character.SubMenuController.CurrentSubMenu.ButtonChoice < -1)
                        {
                            if (character.SubMenuController.CurrentSubMenu.hoveringButton > -1)
                            {

                                character.TurnController.descriptionBox.ATK_Num.text = Mathf.Abs(character.character_Stats.GetCombatStats(skill_Stats[0])[Character_Stats.Stat.ATK]).ToString();

                                float projectedValue = character.SubMenuController.CurrentSubMenu.hoveringButton + level + 1;

                                character.TurnController.descriptionBox.ATK_Mult.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "x" + projectedValue;

                                character.TurnController.descriptionBox.HIT_Num.text = "-";
                                character.TurnController.descriptionBox.CRT_Num.text = "-";

                                character.TurnController.descriptionBox.ATK_Mult.SetActive((projectedValue > 1) ? true : false);

                            }
                            yield return null;
                        }

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {
                            if (character.SubMenuController.CurrentSubMenu.ButtonChoice == 0)
                            {
                                i = 3;
                            }
                            else
                            {
                                i = 5;
                            }
                        }
                        else
                        {
                            character.chosenAttack = null;
                            level = 0;
                            yield break;
                        }

                        break;

                    case 2:

                        // Fully Charged

                        character.TurnController.descriptionBox.container.gameObject.SetActive(true);

                        yield return character.SubMenuController.OpenSubMenu("Charges", new List<string>() { "Execute" });

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {
                                i = 3;
                        }
                        else
                        {
                            print("Cannot Return");
                        }

                        break;

                    case 3:

                        // Targets

                        if (targets.Count > 0)
                            targets.RemoveAt(character.chosenAttack.targets.Count - 1);

                        yield return character.SubMenuController.OpenSubMenu("Targets", character.TurnController.GetPlayerNames(character.Facing));

                        Combat_Character target = null;

                        while (character.SubMenuController.CurrentSubMenu.ButtonChoice == -2)
                        {
                            if (character.SubMenuController.CurrentSubMenu.hoveringButton > -1)
                            {
                                if (character.Facing == 1)
                                    target = character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.hoveringButton].GetComponent<Combat_Character>();


                                character.TurnController.descriptionBox.HIT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.PhHit].ToString();
                                character.TurnController.descriptionBox.CRT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.Crit].ToString();
                            }
                            yield return null;
                        }

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {
                            if (character.Facing == 1)
                                character.chosenAttack.targets.Add(character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);
                            else
                                character.chosenAttack.targets.Add(character.TurnController.left_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);

                            i = 4;
                        }
                        else
                        {
                            if (level >= maxLevel)
                                i = 2;
                            else
                                i = 1;
                        }

                        break;

                    case 4:

                        //Execute

                        yield return character.SubMenuController.OpenSubMenu("Confirm", new List<string>() { "Confirm" });

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {
                            character.character_Stats.AddStatChanger(skill_Stats[0].statChanger);
                            done = true;
                            charging = false;
                            Execute = Action(level + 1);
                            level = 0;
                        }
                        else
                        {
                            i = 3;
                        }

                        break;

                    case 5:

                        // Charge

                        yield return character.SubMenuController.OpenSubMenu("Confirm", new List<string>() { "Confirm" });

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {
                            done = true;

                            charging = true;

                            character.TurnController.descriptionBox.container.gameObject.SetActive(false);
                        }
                        else
                        {
                            i = 1;
                        }

                        break;
                }

            }

            character.EndTurn();
        }

        public IEnumerator Action(int charges)
        {
            character.enemyTransform = targets[0];

            GetOutcome(skill_Stats[0], targets[0]);

            character.animationController.Clip("Sakura Idle");

            yield return character.MoveInRange(new Vector3(-1.75f, 0, 0));

            character.animationController.Clip("Sakura Kunai");

            yield return character.WaitForKeyFrame();

            GameObject[] kunai = new GameObject[charges];

            for (int i = 0; i < charges; i++)
            {
                kunai[i] = Instantiate(kunaiPrefab, character.animationController.instatiatePoint.position, Quaternion.identity);
            }


            Coroutine tragectory = character.StartCoroutine(character.ProjectileArch(kunai[0].transform, new Vector3(0.8f, 0f, 0f), 0.4f));

            Coroutine[] otherTrags = new Coroutine[charges - 1];

            for (int i = 0; i < charges - 1; i++)
            {
                otherTrags[i] = character.StartCoroutine(Kunai(kunai[i + 1], 0.1f * (i + 1)));
            }


            if (character.Facing == 1)
                yield return new WaitWhile(() => kunai[0].transform.position.x <= targets[0].position.x);
            else
                yield return new WaitWhile(() => kunai[0].transform.position.x >= targets[0].position.x);

            if (HitSuccess != 0)
            {
                character.StopCoroutine(tragectory);
                Destroy(kunai[0]);
            }

            Coroutine outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.character_Stats.GetCombatStats(skill_Stats[0], targets[0])[Character_Stats.Stat.ATK]));

            yield return tragectory;

            Destroy(kunai[0], 2);

            character.animationController.Clip("Sakura Idle");

            yield return outcome;

            for (int i = 0; i < otherTrags.Length; i++)
            {
                yield return otherTrags[i];
            }

        }

        IEnumerator Kunai(GameObject kunai, float delay)
        {

            yield return new WaitForSeconds(delay);

            Coroutine tragectory = character.StartCoroutine(character.ProjectileArch(kunai.transform, new Vector3(0.8f, 0f, 0f), 0.4f));

            Coroutine outcome = null;

            if (character.Facing == 1)
                yield return new WaitWhile(() => kunai.transform.position.x <= targets[0].position.x);
            else
                yield return new WaitWhile(() => kunai.transform.position.x >= targets[0].position.x);

            if (HitSuccess != 0)
            {
                character.StopCoroutine(tragectory);
                Destroy(kunai);
                outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.character_Stats.GetCombatStats(skill_Stats[0], targets[0])[Character_Stats.Stat.ATK]));
            }

            yield return tragectory;

            Destroy(kunai, 2);

            yield return outcome;
        }
    }
    public Throw_Kunai throw_Kunai;

    [System.Serializable]
    public class Multi_Hit : Skill
    {
        public Multi_Hit(Combat_Character character) : base(character)
        {
            this.character = character;

            name = "Multi Hit";

            skill_Stats = new Skill_Stats[]
            {
                new Skill_Stats
                {
                    statChanger = new StatChanger
                    {
                        statChanges = new Dictionary<Character_Stats.Stat, float>
                        {

                        }
                    }
                }

            };

            level = 4;

            description = "Release a flury of 4 punches.";
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

                        if (character.chosenAttack.targets.Count > 0)
                            character.chosenAttack.targets.RemoveAt(character.chosenAttack.targets.Count - 1);

                        Combat_Character target = null;

                        yield return character.SubMenuController.OpenSubMenu("Targets", character.TurnController.GetPlayerNames(character.Facing));

                        while (character.SubMenuController.CurrentSubMenu.ButtonChoice == -2)
                        {
                            if (character.SubMenuController.CurrentSubMenu.hoveringButton > -1)
                            {
                                if (character.Facing == 1)
                                    target = character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.hoveringButton].GetComponent<Combat_Character>();


                                character.TurnController.descriptionBox.HIT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.PhHit].ToString();
                                character.TurnController.descriptionBox.CRT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.Crit].ToString();
                            }
                            yield return null;
                        }

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {
                            if (character.Facing == 1)
                                character.chosenAttack.targets.Add(character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);
                            else
                                character.chosenAttack.targets.Add(character.TurnController.left_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);

                            i++;
                        }
                        else
                        {
                            character.chosenAttack = null;
                            yield break;
                        }

                        break;


                    case 1:

                        yield return character.SubMenuController.OpenSubMenu("Confirm", new List<string>() { "Confirm" });

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {
                            done = true;
                        }
                        else
                        {
                            i--;
                        }

                        break;
                }

            }


            Execute = Action();
        }

        public IEnumerator Action()
        {
            character.enemyTransform = targets[0];
            GetOutcome(skill_Stats[0], targets[0]);



            /*CAMERA CONTROL*/

            character.mcamera.GetComponent<MainCamera>().BlackOut(0.9f, 0.5f);

            Vector3 camTargetPos = new Vector3(targets[0].position.x, 0.5f, targets[0].position.z - 1.5f);

            yield return character.mcamera.GetComponent<MainCamera>().LerpMoveIE(camTargetPos, 0.5f);



            yield return character.MoveInRange(new Vector3(-0.35f, 0, 0));

            character.animationController.Clip("Sakura Multi Hit");

            yield return character.WaitForKeyFrame();

            //Coroutine outcome = character.StartCoroutine(character.ApplyOutcome(info[0]));




            CoroutineWithData cd = new CoroutineWithData(character, character.TurnController.Reactions(Turn_Controller.Stage.IMPACT, this));
            yield return cd.coroutine;
            
            Coroutine outcome = null;

            switch ((int)cd.result)
            {
                case 0:    
                    print("Continue");
                    outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.character_Stats.GetCombatStats(skill_Stats[0], targets[0])[Character_Stats.Stat.ATK]));           
                    break;
            
                case 1:
                    print("Pause");
                    break;
            
                case 2:
                    print("Break");
                    yield break;
            
            }

            for (int i = 1; i < 4; i++)
            {
                yield return character.WaitForKeyFrame();

                if (HitSuccess != 0)
                    outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.character_Stats.GetCombatStats(skill_Stats[0], targets[0])[Character_Stats.Stat.ATK]));
            }


            yield return character.animationController.coroutine;
            character.animationController.Clip("Sakura Idle");

            yield return outcome;
        }
    }
    public Multi_Hit multi_Hit;


    [System.Serializable]
    public class Defense : Spell
    {
        public Defense(Combat_Character character) : base(character)
        {
            this.character = character;

            name = "Block";

            skill_Stats = new Skill_Stats[]
            {
                new Skill_Stats{
                    statChanger = new StatChanger
                    {
                        name = "Block Exaust",

                        statChanges = new Dictionary<Character_Stats.Stat, float>
                        {
                            {Character_Stats.Stat.AS, 1.1f },
                        }
                    },
                },
                new Skill_Stats{
                    statChanger = new StatChanger()
                    {
                        name = "Block 2",

                        statChanges = new Dictionary<Character_Stats.Stat, float>
                        {
                                {Character_Stats.Stat.DEF, 100f },
                                {Character_Stats.Stat.PhAvo, -1 }
                        }
                    },
                }

            };

            effect = true;
            description = "At the point of impact on this character; increase defense by " + skill_Stats[1].statChanger.statChanges[Character_Stats.Stat.DEF] + ".";

            stage = Turn_Controller.Stage.IMPACT;

            image = Resources.Load<Sprite>("Skill Icons/Block Icon");
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
                            character.character_Stats.AddStatChanger(skill_Stats[0].statChanger);

                            done = true;
                        }
                        else
                        {
                            character.chosenAttack = null;
                            yield break;
                        }

                        break;
                }

            }

            Execute = Action();
        }

        public IEnumerator Action()
        {
            character.animationController.Clip("Sakura Buff");

            yield return character.WaitForKeyFrame();

            character.SetSkill(this, image);

            yield return character.animationController.coroutine;
            character.animationController.Clip("Sakura Idle");
        }

        public override bool Condition(Turn_Controller.Stage stage, Skill info)
        {

            if (character.TurnController.characterTurn.enemyTransform == character.transform && this.stage == stage && info.type == Type.Physical && info.range == Range.Close)
                return true;

            return false;
        }

        public override IEnumerator Action2(int slot)
        {   
            character.character_Stats.AddStatChanger(skill_Stats[1].statChanger);

            character.ClearSkill(slot);

            yield return 0;
        }
    }
    public Defense defense;

    [System.Serializable]
    public class Heal : Spell
    {

        int heal = 10;

        public Heal(Combat_Character character) : base(character)
        {
            this.character = character;

            name = "Heal";

            skill_Stats = new Skill_Stats[]
            {
                new Skill_Stats
                {
                    mana = -15,

                    statChanger = new StatChanger
                    {
                        name = "Heal Exhaust",

                        statChanges = new Dictionary<Character_Stats.Stat, float>
                        {
                            {Character_Stats.Stat.AS, 0.3f },
                        }
                    },
                },
            };

            type = Type.Magic;
            range = Range.Far;

            maxLevel = 1;

            chargeAnimation = "Sakura Charge";

            effect = true;
            description = "HEAL this character for " + heal + " immediatly. At the start of this character's next turn; this character may HEAL again for " + heal + ".";

            stage = Turn_Controller.Stage.TURN_START;

            image = Resources.Load<Sprite>("Skill Icons/Heal Icon");
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

                        character.TurnController.descriptionBox.container.gameObject.SetActive(true);

                        if (charging)
                            level++;

                        i++;

                        break;

                    case 1:

                        if (level >= maxLevel)
                        {
                            i = 2;
                            break;
                        }

                        yield return character.SubMenuController.OpenSubMenu("Charges", new List<string>() { "Charge" });

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {
                            i = 3;
                        }
                        else
                        {
                            character.chosenAttack = null;
                            level = 0;
                            yield break;
                        }

                        break;

                    case 2:

                        //Execute

                        yield return character.SubMenuController.OpenSubMenu("Confirm", new List<string>() { "Confirm"});

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {
                            character.character_Stats.AddStatChanger(skill_Stats[0].statChanger);

                            done = true;
                            charging = false;
                            Execute = Action();
                            level = 0;
                        }
                        else
                        {
                            i = 1;
                        }

                        break;

                    case 3:

                        // Charge

                        yield return character.SubMenuController.OpenSubMenu("Confirm", new List<string>() { "Confirm" });

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {
                            done = true;

                            charging = true;

                            character.animationController.Clip(chargeAnimation);

                            character.TurnController.descriptionBox.container.gameObject.SetActive(false);
                        }
                        else
                        {
                            i = 1;
                        }

                        break;
                }

            }

            character.EndTurn();
        }

        public IEnumerator Action()
        {
            GetOutcome(skill_Stats[0],character.transform);

            character.animationController.Clip("Sakura Buff");

            yield return character.WaitForKeyFrame();

            // Should try to push thorough ApplyOutcome eventually

            int totalHeal = heal * CritSuccess;

            character.AdjustHealth(totalHeal);

            character.AdjustMana(Mathf.RoundToInt(skill_Stats[0].mana));


            if (CritSuccess != 1)
                Instantiate(character.outcome_Bubble_Prefab, character.outcome_Bubble_Pos.position, Quaternion.identity).Input(totalHeal, "CRITICAL", Color.green);
            else
                Instantiate(character.outcome_Bubble_Prefab, character.outcome_Bubble_Pos.position, Quaternion.identity).Input(totalHeal);

            //Instantiate(character.outcome_Bubble_Prefab, character.outcome_Bubble_Pos.position, Quaternion.identity).Input(totalHeal);

            character.SetSkill(this, image);

            yield return character.animationController.coroutine;
            character.animationController.Clip("Sakura Idle");
        }

        public override bool Condition(Turn_Controller.Stage stage, Skill info)
        {
            if (character.TurnController.characterTurn == character && this.stage == stage)
                return true;

            return false;
        }

        public override IEnumerator Action2(int slot)
        {
            character.animationController.Clip("Sakura Buff");

            yield return character.WaitForKeyFrame();

            character.AdjustHealth(heal);

            character.AdjustMana(Mathf.RoundToInt(skill_Stats[0].mana));

            Instantiate(character.outcome_Bubble_Prefab, character.outcome_Bubble_Pos.position, Quaternion.identity).Input(heal);

            character.ClearSkill(slot);

            yield return character.animationController.coroutine;
            character.animationController.Clip("Sakura Idle");

            yield return 0;
        }

    }
    public Heal heal;


    private void Awake()
    {
        combo = new Combo(this);
        jump_Kick = new Jump_Kick(this);
        throw_Kunai = new Throw_Kunai(this);
        multi_Hit = new Multi_Hit(this);
        defense = new Defense(this);
        heal = new Heal(this);

        attackList = new List<Skill>()
        {
            combo,
            jump_Kick,
            throw_Kunai,
            multi_Hit,
            defense,
            heal,
        };

        health = 100;

        mana = 100;
    }

    public override IEnumerator Damage()
    {
        animationController.Clip("Sakura Idle");
        yield return null;
        animationController.Clip("Sakura Damaged");

        yield return animationController.coroutine;
    }

    public override IEnumerator Block()
    {
        animationController.Clip("Sakura Idle");
        yield return null;
        animationController.Clip("Sakura Block");
        yield return animationController.coroutine;
    }

    public override IEnumerator Dodge()
    {
        animationController.Clip("Sakura Idle");
        yield return null;
        animationController.Clip("Sakura Dodge");

        yield return MoveAmount(new Vector3(0.3f * -Facing, 0, 0));

        yield return animationController.coroutine;
        animationController.Clip("Sakura Idle");
    }

    public override IEnumerator CpuDecisionMaking()
    {
        Transform target;

        if (Facing == 1)
            target = TurnController.right_Players[0].transform;
        else
            target = TurnController.left_Players[0].transform;


        int r = Random.Range(0, 1);

        switch (r)
        {
            case 0:

                AttackChoice(combo);

                chosenAttack.targets.Add(target);
                //chosenAttack.targets.Add(TurnController.left_Players[0].transform);

                int level = 3;

                character_Stats.AddStatChanger(combo.skill_Stats[level-1].statChanger);

                chosenAttack.Execute = combo.Action(level);

                break;

            case 1:

                AttackChoice(multi_Hit);

                chosenAttack.targets.Add(target);

                chosenAttack.Execute = multi_Hit.Action();

                break;
        }

        EndTurn();

        yield return null;
    }
}

