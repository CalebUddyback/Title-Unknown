using System.Collections;
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
    public class Punch : Skill
    {
        public Punch(Combat_Character character) : base(character)
        {
            name = "Punch";

            skill_Stats = new Skill_Stats[1];

            skill_Stats[0] = new Skill_Stats()
            {
                attack = 1,

                statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
                {
                    {Character_Stats.Stat.AS, 1 },
                })
                {
                    name = name + " exhaust",
                },
            };

            type = Type.Physical;
            range = Range.Close;

            effect = true;
            description = "Upon successful hit; decrease AS by 1 second(s).";

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

                        // Returning to this submenu

                        if (character.chosenAction.targets.Count > 0)
                            character.chosenAction.targets.RemoveAt(character.chosenAction.targets.Count - 1);

                        Combat_Character target = null;

                        yield return character.SubMenuController.OpenSubMenu("Targets", character.TurnController.GetPlayerNames(character.Facing));

                        yield return null;

                        while (character.SubMenuController.CurrentSubMenu.ButtonChoice == -2)
                        {
                            if (character.SubMenuController.CurrentSubMenu.hoveringButton > -1)
                            {
                                int hovering = character.SubMenuController.CurrentSubMenu.hoveringButton;

                                if (character.Facing == 1)
                                    target = character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.hoveringButton].GetComponent<Combat_Character>();

                                character.TurnController.left_descriptionBox.ATK_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.ATK].ToString();
                                character.TurnController.left_descriptionBox.HIT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.PhHit].ToString();
                                character.TurnController.left_descriptionBox.CRT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.Crit].ToString();
                            }
                            yield return null;
                        }

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {

                            if (character.Facing == 1)
                                character.chosenAction.targets.Add(character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);
                            else
                                character.chosenAction.targets.Add(character.TurnController.left_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);

                            i++;

                        }
                        else
                        {
                            character.chosenAction = null;
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
            character.enemyTransform = targets[0];

            //GetOutcome(skill_Stats[0], targets[0]);

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

            GetOutcome(skill_Stats[0], targets[0]);

            Dictionary<Character_Stats.Stat, int> ownerDirectory = character.character_Stats.GetCombatStats(skill_Stats[0]);

            Dictionary<Character_Stats.Stat, int> targetDirectory = targets[0].GetComponent<Combat_Character>().character_Stats.GetCurrentStats();

            print(ownerDirectory[Character_Stats.Stat.ATK] + " " + targetDirectory[Character_Stats.Stat.DEF]);

            Coroutine outcome;


            switch ((int)cd.result)
            {
                case 0:

                    print("Continue");

                    outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.character_Stats.GetCombatStats(skill_Stats[0], targets[0])[Character_Stats.Stat.ATK]));

                    if (HitSuccess == 1 && !targets[0].GetComponent<Combat_Character>().blocking)
                        yield return character.Hud.AffectTimerProgress(-1);

                    yield return character.animationController.coroutine;
                    //character.animationController.Clip("Sakura Idle");

                    yield return outcome;

                    break;

                case 1:
                    print("Pause");
                    break;

                case 2:
                    print("Break");
                    yield break;

            }
        }

    }
    public Punch punch;

    [System.Serializable]
    public class Uppercut : Skill
    {
        public Uppercut(Combat_Character character) : base(character)
        {
            name = "Uppercut";

            skill_Stats = new Skill_Stats[1];

            skill_Stats[0] = new Skill_Stats()
            {
                attack = 1,

                statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
                {
                    {Character_Stats.Stat.AS, 1 },
                })
                {
                    name = name + " exhaust",
                },
            };

            type = Type.Physical;
            range = Range.Close;

            description = "A simple uppercut";

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

                        // Returning to this submenu

                        if (character.chosenAction.targets.Count > 0)
                            character.chosenAction.targets.RemoveAt(character.chosenAction.targets.Count - 1);

                        Combat_Character target = null;

                        yield return character.SubMenuController.OpenSubMenu("Targets", character.TurnController.GetPlayerNames(character.Facing));

                        yield return null;

                        while (character.SubMenuController.CurrentSubMenu.ButtonChoice == -2)
                        {
                            if (character.SubMenuController.CurrentSubMenu.hoveringButton > -1)
                            {
                                if (character.Facing == 1)
                                    target = character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.hoveringButton].GetComponent<Combat_Character>();

                                character.TurnController.left_descriptionBox.ATK_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.ATK].ToString();
                                character.TurnController.left_descriptionBox.HIT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.PhHit].ToString();
                                character.TurnController.left_descriptionBox.CRT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.Crit].ToString();
                            }
                            yield return null;
                        }

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {

                            if (character.Facing == 1)
                                character.chosenAction.targets.Add(character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);
                            else
                                character.chosenAction.targets.Add(character.TurnController.left_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);

                            i++;

                        }
                        else
                        {
                            character.chosenAction = null;
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
            character.enemyTransform = targets[0];

            //GetOutcome(skill_Stats[0], targets[0]);

            /*CAMERA CONTROL*/

            character.mcamera.GetComponent<MainCamera>().BlackOut(0.9f, 0.5f);

            Vector3 camTargetPos = new Vector3(targets[0].position.x, 0.5f, targets[0].position.z - 1.5f);

            yield return character.mcamera.GetComponent<MainCamera>().LerpMoveIE(camTargetPos, 0.5f);

            yield return character.MoveInRange(new Vector3(-0.35f, 0, 0));

            character.animationController.Clip("Sakura Uppercut");

            yield return character.WaitForKeyFrame();

            //yield return character.TurnController.Reactions(Turn_Controller.Stage.IMPACT, info[0]);

            CoroutineWithData cd = new CoroutineWithData(character, character.TurnController.Reactions(Turn_Controller.Stage.IMPACT, this));
            yield return cd.coroutine;

            GetOutcome(skill_Stats[0], targets[0]);

            Dictionary<Character_Stats.Stat, int> ownerDirectory = character.character_Stats.GetCombatStats(skill_Stats[0]);

            Dictionary<Character_Stats.Stat, int> targetDirectory = targets[0].GetComponent<Combat_Character>().character_Stats.GetCurrentStats();

            print(ownerDirectory[Character_Stats.Stat.ATK] + " " + targetDirectory[Character_Stats.Stat.DEF]);

            Coroutine outcome;


            switch ((int)cd.result)
            {
                case 0:

                    print("Continue");

                    outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.character_Stats.GetCombatStats(skill_Stats[0], targets[0])[Character_Stats.Stat.ATK]));

                    //if (HitSuccess == 1 && !targets[0].GetComponent<Combat_Character>().blocking)
                    //    yield return targets[0].GetComponent<Combat_Character>().Hud.AffectTimerProgress(1);

                    yield return character.animationController.coroutine;
                    //character.animationController.Clip("Sakura Idle");

                    yield return outcome;

                    break;

                case 1:
                    print("Pause");
                    break;

                case 2:
                    print("Break");
                    yield break;

            }
        }

    }
    public Uppercut uppercut;

    [System.Serializable]
    public class Kick : Skill
    {
        public Kick(Combat_Character character) : base(character)
        {
            name = "Kick";

            skill_Stats = new Skill_Stats[1];

            skill_Stats[0] = new Skill_Stats()
            {
                attack = 1,

                statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
                {
                    {Character_Stats.Stat.AS, 1 },
                })
                {
                    name = name + " exhaust",
                },
            };

            type = Type.Physical;
            range = Range.Close;

            effect = true;
            description = "Upon successful hit; increase target AS by 1 second(s).";

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

                        // Returning to this submenu

                        if (character.chosenAction.targets.Count > 0)
                            character.chosenAction.targets.RemoveAt(character.chosenAction.targets.Count - 1);

                        Combat_Character target = null;

                        yield return character.SubMenuController.OpenSubMenu("Targets", character.TurnController.GetPlayerNames(character.Facing));

                        yield return null;

                        while (character.SubMenuController.CurrentSubMenu.ButtonChoice == -2)
                        {
                            if (character.SubMenuController.CurrentSubMenu.hoveringButton > -1)
                            {
                                if (character.Facing == 1)
                                    target = character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.hoveringButton].GetComponent<Combat_Character>();

                                character.TurnController.left_descriptionBox.ATK_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.ATK].ToString();
                                character.TurnController.left_descriptionBox.HIT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.PhHit].ToString();
                                character.TurnController.left_descriptionBox.CRT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.Crit].ToString();
                            }
                            yield return null;
                        }

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {

                            if (character.Facing == 1)
                                character.chosenAction.targets.Add(character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);
                            else
                                character.chosenAction.targets.Add(character.TurnController.left_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);

                            i++;

                        }
                        else
                        {
                            character.chosenAction = null;
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
            character.enemyTransform = targets[0];

            //GetOutcome(skill_Stats[0], targets[0]);

            /*CAMERA CONTROL*/

            character.mcamera.GetComponent<MainCamera>().BlackOut(0.9f, 0.5f);

            Vector3 camTargetPos = new Vector3(targets[0].position.x, 0.5f, targets[0].position.z - 1.5f);

            yield return character.mcamera.GetComponent<MainCamera>().LerpMoveIE(camTargetPos, 0.5f);

            yield return character.MoveInRange(new Vector3(-0.35f, 0, 0));

            character.animationController.Clip("Sakura Kick");

            yield return character.WaitForKeyFrame();

            //yield return character.TurnController.Reactions(Turn_Controller.Stage.IMPACT, info[0]);

            CoroutineWithData cd = new CoroutineWithData(character, character.TurnController.Reactions(Turn_Controller.Stage.IMPACT, this));
            yield return cd.coroutine;

            GetOutcome(skill_Stats[0], targets[0]);

            Dictionary<Character_Stats.Stat, int> ownerDirectory = character.character_Stats.GetCombatStats(skill_Stats[0]);

            Dictionary<Character_Stats.Stat, int> targetDirectory = targets[0].GetComponent<Combat_Character>().character_Stats.GetCurrentStats();

            print(ownerDirectory[Character_Stats.Stat.ATK] + " " + targetDirectory[Character_Stats.Stat.DEF]);

            Coroutine outcome;


            switch ((int)cd.result)
            {
                case 0:

                    print("Continue");

                    outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.character_Stats.GetCombatStats(skill_Stats[0], targets[0])[Character_Stats.Stat.ATK]));

                    if (HitSuccess == 1 && !targets[0].GetComponent<Combat_Character>().blocking)
                        yield return targets[0].GetComponent<Combat_Character>().Hud.AffectTimerProgress(1);

                    yield return character.animationController.coroutine;
                    //character.animationController.Clip("Sakura Idle");

                    yield return outcome;

                    break;

                case 1:
                    print("Pause");
                    break;

                case 2:
                    print("Break");
                    yield break;

            }
        }

    }
    public Kick kick;


    [System.Serializable]
    public class Combo : Skill
    {
        public Combo(Combat_Character character) : base(character)
        {
            name = "Punch, Punch, Kick!";

            maxLevel = 3;

            skill_Stats = new Skill_Stats[3];

            skill_Stats[0] = new Skill_Stats()
            {
                attack = 1,

                statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
                {
                    { Character_Stats.Stat.AS, 1 },
                })
                {
                    name = name + " exhaust",
                },
            };

            skill_Stats[1] = new Skill_Stats()
            {
                attack = 1,

                statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
                {
                    { Character_Stats.Stat.AS, 2 },
                })
                {
                    name = name + " exhaust",
                },
            };

            skill_Stats[2] = new Skill_Stats()
            {
                attack = 1,

                statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
                {
                    { Character_Stats.Stat.AS, 3 },
                })
                {
                    name = name + " exhaust",
                },
            };



            type = Type.Physical;
            range = Range.Close;

            effect = true;
            description = "If last hit is successful; increase target AS by 1 second(s).";

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

                                character.TurnController.left_descriptionBox.title.text = name + " Level " + hovering;

                                character.TurnController.left_descriptionBox.ATK_Mult.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "x" + hovering;

                                character.TurnController.left_descriptionBox.ATK_Mult.SetActive((hovering > 1) ? true : false);

                                int projectedValue = character.character_Stats.GetCombatStats(skill_Stats[hovering-1])[Character_Stats.Stat.AS];
                                //character.TurnController.left_descriptionBox.REC_Num.color = character.character_Stats.CompareStat(Character_Stats.Stat.AS, projectedValue, true);
                                character.TurnController.left_descriptionBox.REC_Num.text = projectedValue.ToString();

                                character.TurnController.left_descriptionBox.HIT_Num.text = (skill_Stats[hovering-1].accuracy != 0) ? skill_Stats[hovering-1].accuracy.ToString() : "-";

                                character.TurnController.left_descriptionBox.CRT_Num.text = (skill_Stats[hovering-1].critical != 0) ? skill_Stats[hovering-1].critical.ToString() : "-";
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
                            character.chosenAction = null;
                            yield break;
                        }


                        break;

                    case 1:

                        // Returning to this submenu

                        if (character.chosenAction.targets.Count > 0)
                            character.chosenAction.targets.RemoveAt(character.chosenAction.targets.Count - 1);

                        Combat_Character target = null;

                        yield return character.SubMenuController.OpenSubMenu("Targets", character.TurnController.GetPlayerNames(character.Facing));

                        while (character.SubMenuController.CurrentSubMenu.ButtonChoice == -2)
                        {
                            if (character.SubMenuController.CurrentSubMenu.hoveringButton > -1)
                            {
                                if (character.Facing == 1)
                                    target = character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.hoveringButton].GetComponent<Combat_Character>();

                                character.TurnController.left_descriptionBox.ATK_Num.text = character.character_Stats.GetCombatStats(skill_Stats[level-1], target)[Character_Stats.Stat.ATK].ToString();
                                character.TurnController.left_descriptionBox.HIT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[level-1], target)[Character_Stats.Stat.PhHit].ToString();
                                character.TurnController.left_descriptionBox.CRT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[level-1], target)[Character_Stats.Stat.Crit].ToString();
                            }
                            yield return null;
                        }

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {

                            if (character.Facing == 1)
                                character.chosenAction.targets.Add(character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);
                            else
                                character.chosenAction.targets.Add(character.TurnController.left_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);

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

            //GetOutcome(skill_Stats[0], targets[0]);

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

            GetOutcome(skill_Stats[0], targets[0]);

            Dictionary<Character_Stats.Stat, int> ownerDirectory = character.character_Stats.GetCombatStats(skill_Stats[0]);

            Dictionary<Character_Stats.Stat, int> targetDirectory = targets[0].GetComponent<Combat_Character>().character_Stats.GetCurrentStats();

            print(ownerDirectory[Character_Stats.Stat.ATK] + " " + targetDirectory[Character_Stats.Stat.DEF]);

            Coroutine outcome;


            switch ((int)cd.result)
            {
                case 0:

                    print("Continue");

                    outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.character_Stats.GetCombatStats(skill_Stats[0], targets[0])[Character_Stats.Stat.ATK]));

                    //if (HitSuccess == 1 && !targets[0].GetComponent<Combat_Character>().blocking)
                    //    yield return targets[0].GetComponent<Combat_Character>().Hud.AffectTimerProgress(1);

                    yield return character.animationController.coroutine;
                    //character.animationController.Clip("Sakura Idle");

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

            //if (HitSuccess == 1 && !targets[0].GetComponent<Combat_Character>().blocking)
            //    yield return targets[0].GetComponent<Combat_Character>().Hud.AffectTimerProgress(1);

            yield return character.animationController.coroutine;
            //character.animationController.Clip("Sakura Idle");

            yield return outcome;

            if (level == 2)
                yield break;

            // three

            yield return character.MoveInRange(new Vector3(-0.35f, 0, 0));

            GetOutcome(skill_Stats[2], targets[0]);

            character.animationController.Clip("Sakura Kick");

            yield return character.WaitForKeyFrame();
            outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.character_Stats.GetCombatStats(skill_Stats[2], targets[0])[Character_Stats.Stat.ATK]));

            if (HitSuccess == 1 && !targets[0].GetComponent<Combat_Character>().blocking)
                yield return targets[0].GetComponent<Combat_Character>().Hud.AffectTimerProgress(1);

            yield return character.animationController.coroutine;
            //character.animationController.Clip("Sakura Idle");

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

            skill_Stats = new Skill_Stats[1];

            skill_Stats[0] = new Skill_Stats()
            {
                attack = 20,
                accuracy = -3,
                critical = 10,

                statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
                {
                    { Character_Stats.Stat.AS, 3 },
                })
                {
                    name = name + " exhaust",
                },
            };

            effect = true;
            description = "Upon successful hit; increase target AS by 2 second(s).";
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

                        if (character.chosenAction.targets.Count > 0)
                            character.chosenAction.targets.RemoveAt(character.chosenAction.targets.Count - 1);

                        Combat_Character target = null;

                        yield return character.SubMenuController.OpenSubMenu("Targets", character.TurnController.GetPlayerNames(character.Facing));

                        yield return null;

                        while (character.SubMenuController.CurrentSubMenu.ButtonChoice == -2)
                        {
                            if (character.SubMenuController.CurrentSubMenu.hoveringButton > -1)
                            {
                                if (character.Facing == 1)
                                    target = character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.hoveringButton].GetComponent<Combat_Character>();

                                character.TurnController.left_descriptionBox.ATK_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.ATK].ToString();
                                character.TurnController.left_descriptionBox.HIT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.PhHit].ToString();
                                character.TurnController.left_descriptionBox.CRT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.Crit].ToString();
                            }
                            yield return null;
                        }

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {

                            if (character.Facing == 1)
                                character.chosenAction.targets.Add(character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);
                            else
                                character.chosenAction.targets.Add(character.TurnController.left_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);

                            i++;

                        }
                        else
                        {
                            character.chosenAction = null;
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
            character.enemyTransform = character.chosenAction.targets[0];

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
                yield return targets[0].GetComponent<Combat_Character>().Hud.AffectTimerProgress(2);
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

            chargeTime = 1;

            maxLevel = 2;

            skill_Stats = new Skill_Stats[1];

            skill_Stats[0] = new Skill_Stats()
            {
                attack = 5,
                accuracy = -5,
                critical = 5,

                statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
                {
                    { Character_Stats.Stat.AS, 3 },
                })
                {
                    name = name + " exhaust",
                },
            };

            range = Range.Far;

            effect = true;
            description = "Charge to a max of 2; each charge requires " + chargeTime + " second(s). Each charge will throw an extra kunai.";
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
                            character.TurnController.left_descriptionBox.title.text = name + " x " + (level + 1);

                        break;

                    case 1:

                        if (level >= maxLevel)
                        {
                            i = 2;
                            break;
                        }

                        character.TurnController.left_descriptionBox.container.gameObject.SetActive(true);

                        yield return character.SubMenuController.OpenSubMenu("Charges", new List<string>() { "Execute", "Charge" });

                        while (character.SubMenuController.CurrentSubMenu.ButtonChoice < -1)
                        {
                            if (character.SubMenuController.CurrentSubMenu.hoveringButton > -1)
                            {

                                character.TurnController.left_descriptionBox.ATK_Num.text = Mathf.Abs(character.character_Stats.GetCombatStats(skill_Stats[0])[Character_Stats.Stat.ATK]).ToString();

                                float projectedValue = character.SubMenuController.CurrentSubMenu.hoveringButton + level + 1;

                                character.TurnController.left_descriptionBox.ATK_Mult.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "x" + projectedValue;

                                character.TurnController.left_descriptionBox.HIT_Num.text = "-";
                                character.TurnController.left_descriptionBox.CRT_Num.text = "-";

                                character.TurnController.left_descriptionBox.ATK_Mult.SetActive((projectedValue > 1) ? true : false);

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
                            character.chosenAction = null;
                            level = 0;
                            yield break;
                        }

                        break;

                    case 2:

                        // Fully Charged

                        character.TurnController.left_descriptionBox.container.gameObject.SetActive(true);

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
                            targets.RemoveAt(character.chosenAction.targets.Count - 1);

                        yield return character.SubMenuController.OpenSubMenu("Targets", character.TurnController.GetPlayerNames(character.Facing));

                        Combat_Character target = null;

                        while (character.SubMenuController.CurrentSubMenu.ButtonChoice == -2)
                        {
                            if (character.SubMenuController.CurrentSubMenu.hoveringButton > -1)
                            {
                                if (character.Facing == 1)
                                    target = character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.hoveringButton].GetComponent<Combat_Character>();

                                character.TurnController.left_descriptionBox.ATK_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.ATK].ToString();
                                character.TurnController.left_descriptionBox.HIT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.PhHit].ToString();
                                character.TurnController.left_descriptionBox.CRT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.Crit].ToString();
                            }
                            yield return null;
                        }

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {
                            if (character.Facing == 1)
                                character.chosenAction.targets.Add(character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);
                            else
                                character.chosenAction.targets.Add(character.TurnController.left_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);

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

                            character.TurnController.left_descriptionBox.container.gameObject.SetActive(false);
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

            //character.animationController.Clip("Sakura Idle");

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

            level = 4;

            skill_Stats = new Skill_Stats[1];

            skill_Stats[0] = new Skill_Stats();

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

                        if (character.chosenAction.targets.Count > 0)
                            character.chosenAction.targets.RemoveAt(character.chosenAction.targets.Count - 1);

                        Combat_Character target = null;

                        yield return character.SubMenuController.OpenSubMenu("Targets", character.TurnController.GetPlayerNames(character.Facing));

                        while (character.SubMenuController.CurrentSubMenu.ButtonChoice == -2)
                        {
                            if (character.SubMenuController.CurrentSubMenu.hoveringButton > -1)
                            {
                                if (character.Facing == 1)
                                    target = character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.hoveringButton].GetComponent<Combat_Character>();

                                character.TurnController.left_descriptionBox.ATK_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.ATK].ToString();
                                character.TurnController.left_descriptionBox.HIT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.PhHit].ToString();
                                character.TurnController.left_descriptionBox.CRT_Num.text = character.character_Stats.GetCombatStats(skill_Stats[0], target)[Character_Stats.Stat.Crit].ToString();
                            }
                            yield return null;
                        }

                        yield return character.SubMenuController.CurrentCD.coroutine;

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {
                            if (character.Facing == 1)
                                character.chosenAction.targets.Add(character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);
                            else
                                character.chosenAction.targets.Add(character.TurnController.left_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);

                            i++;
                        }
                        else
                        {
                            character.chosenAction = null;
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
            //character.animationController.Clip("Sakura Idle");

            yield return outcome;
        }
    }
    public Multi_Hit multi_Hit;


    [System.Serializable]
    public class Guard : Spell
    {
        public Guard(Combat_Character character) : base(character)
        {
            this.character = character;

            name = "Guard";

            skill_Stats = new Skill_Stats[2];

            skill_Stats[0] = new Skill_Stats()
            {
                statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
                {
                    { Character_Stats.Stat.AS, 3 },
                })
                {
                    name = name + " exhaust",
                },
            };

            skill_Stats[1] = new Skill_Stats()
            {
                statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
                {
                    {Character_Stats.Stat.DEF, 100 },
                    {Character_Stats.Stat.PhAvo, -1000 }
                })
                {
                    name = name + " effect",
                },
            };


            effect = true;
            description = "At the point of impact on this character; increase DEF by " + skill_Stats[1].statChanger.statChanges[Character_Stats.Stat.DEF] + ".";

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
            character.animationController.Clip("Sakura Buff");

            yield return character.WaitForKeyFrame();

            character.SetSkill(this, image);

            yield return character.animationController.coroutine;
            //character.animationController.Clip("Sakura Idle");
        }

        public override bool Condition(Turn_Controller.Stage stage, Skill info)
        {

            if (character.TurnController.characterTurn.enemyTransform == character.transform && this.stage == stage && info.type == Type.Physical && info.range == Range.Close)
                return true;

            return false;
        }

        public override IEnumerator Action2(int slot)
        {
            character.blocking = true;

            character.character_Stats.AddStatChanger(skill_Stats[1].statChanger);

            character.ClearSkill(slot);

            yield return 0;
        }
    }
    public Guard guard;

    [System.Serializable]
    public class Heal : Spell
    {

        int heal = 10;

        public Heal(Combat_Character character) : base(character)
        {
            this.character = character;

            name = "Heal";

            chargeTime = 2;


            skill_Stats = new Skill_Stats[1];

            skill_Stats[0] = new Skill_Stats()
            {
                statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
                {
                    { Character_Stats.Stat.AS, 2 },
                })
                {
                    name = name + " exhaust",
                },
            };

            type = Type.Magic;
            range = Range.Far;

            maxLevel = 1;

            chargeAnimation = "Sakura Charge";

            effect = true;
            description = "Heal this character for " + heal + " hp immediatly. At the start of this character's next turn; this character may heal again for " + heal + " hp.";

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

                        character.TurnController.left_descriptionBox.container.gameObject.SetActive(true);

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
                            character.chosenAction = null;
                            level = 0;
                            yield break;
                        }

                        break;

                    case 2:

                        //Execute

                        //yield return character.SubMenuController.OpenSubMenu("Confirm", new List<string>() { "Confirm"});
                        //
                        //yield return character.SubMenuController.CurrentCD.coroutine;
                        //
                        //if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        //{
                            character.character_Stats.AddStatChanger(skill_Stats[0].statChanger);

                            done = true;
                            charging = false;
                            Execute = Action();
                            level = 0;
                        //}

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

                            character.TurnController.left_descriptionBox.container.gameObject.SetActive(false);
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

            character.Health += totalHeal;

            character.Mana += skill_Stats[0].mana;


            if (CritSuccess != 1)
                Instantiate(character.outcome_Bubble_Prefab, character.outcome_Bubble_Pos.position, Quaternion.identity).Input(totalHeal, "CRITICAL", Color.green);
            else
                Instantiate(character.outcome_Bubble_Prefab, character.outcome_Bubble_Pos.position, Quaternion.identity).Input(totalHeal);

            //Instantiate(character.outcome_Bubble_Prefab, character.outcome_Bubble_Pos.position, Quaternion.identity).Input(totalHeal);

            character.SetSkill(this, image);

            yield return character.animationController.coroutine;
            //character.animationController.Clip("Sakura Idle");
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

            character.Health += heal;

            character.Mana += skill_Stats[0].mana;

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
        characterName = "Sakura";
        InitializeSkills();
    }

    public override void InitializeSkills()
    {
        base.InitializeSkills();

        punch = new Punch(this);
        uppercut = new Uppercut(this);
        kick = new Kick(this);
        combo = new Combo(this);
        jump_Kick = new Jump_Kick(this);
        throw_Kunai = new Throw_Kunai(this);
        multi_Hit = new Multi_Hit(this);
        guard = new Guard(this);
        heal = new Heal(this);

        attackList = new List<Skill>()
        {
            punch,
            uppercut,
            kick,
            combo,
            jump_Kick,
            throw_Kunai,
            multi_Hit,
            guard,
            heal,
        };
    }

    public override IEnumerator Damage()
    {
        animationController.Clip("Sakura Idle");
        yield return null;
        animationController.Clip("Sakura Damaged");

        //yield return animationController.coroutine;
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

                Skill skill = combo;

                ActionChoice(skill);

                chosenAction.targets.Add(target);
                //chosenAttack.targets.Add(TurnController.left_Players[0].transform);

                int level = 3;

                character_Stats.AddStatChanger(skill.skill_Stats[level-1].statChanger);

                chosenAction.Execute = ((Combo)skill).Action(level);

                skill.level = 3;


                DescriptionBox dBox = SubMenuController.owner.TurnController.left_descriptionBox;

                if(SubMenuController.owner.TurnController.right_Players.Contains(this))
                    dBox = SubMenuController.owner.TurnController.right_descriptionBox;


                Spell spell = skill as Spell;

                if (spell != null)
                {
                    dBox.container.GetComponent<UnityEngine.UI.Image>().color = new Color(0.1058824f, 0.254902f, 0.2226822f, 0.7372549f);
                }
                else
                {
                    dBox.container.GetComponent<UnityEngine.UI.Image>().color = new Color(0.1058824f, 0.1215686f, 0.254902f, 0.7372549f);
                }

                dBox.title.text = skill.name;

                if (skill.level > 0)
                {
                    dBox.ATK_Mult.SetActive(true);
                    dBox.ATK_Mult.transform.GetChild(0).GetComponent<TMPro.TextMeshProUGUI>().text = "x" + skill.level.ToString();
                }
                else
                {
                    dBox.ATK_Mult.SetActive(false);
                }


                var output = character_Stats.GetCombatStats(skill.skill_Stats[0], target);

                dBox.ATK_Num.text = output[Character_Stats.Stat.ATK].ToString();
                dBox.ATK_Num.color = Color.white;

                dBox.HIT_Num.text = output[Character_Stats.Stat.PhHit].ToString().ToString();

                dBox.CRT_Num.text = output[Character_Stats.Stat.Crit].ToString().ToString();

                dBox.REC_Num.text = output[Character_Stats.Stat.AS].ToString().ToString();
                dBox.REC_Num.color = Color.white;

                dBox.MP_Num.text = skill.skill_Stats[0].mana.ToString();

                string typeText = "[" + skill.type.ToString().ToUpper();

                if (skill.effect)
                {
                    typeText += " / EFFECT";
                }

                typeText += "]";

                dBox.type.text = typeText;

                dBox.DescriptionText(skill);

                dBox.container.SetActive(true);

                break;

            case 1:

                ActionChoice(multi_Hit);

                chosenAction.targets.Add(target);

                chosenAction.Execute = multi_Hit.Action();

                break;
        }

        yield return new WaitForSeconds(1);     // Pretend to Think

        EndTurn();

        yield return null;
    }
}

