using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sakura : Combat_Character
{

    //[System.Serializable]
    //public class Punch : Skill
    //{
    //    public Punch(Combat_Character character) : base(character)
    //    {
    //        name = "Punch";
    //
    //        skill_Stats = new Skill_Stats[1];
    //
    //        skill_Stats[0] = new Skill_Stats()
    //        {
    //            attack = 1,
    //
    //            statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
    //            {
    //                {Character_Stats.Stat.AS, 1 },
    //            })
    //            {
    //                name = name + " exhaust",
    //            },
    //        };
    //
    //        type = Type.Physical;
    //        range = Range.Close;
    //
    //        effect = true;
    //        description = "Upon successful hit; decrease AS by 1 second(s).";
    //
    //    }
    //
    //    public override IEnumerator SubMenus(MonoBehaviour owner)   // parameter can be changed to submenu_controller_2
    //    {
    //
    //        SubMenu_Controller_2 smc = character.SubMenuController2;
    //
    //        bool done = false;
    //
    //        do
    //        {
    //            switch (smc.subMenuID)
    //            {
    //                case "Start":
    //                    smc.subMenuID = "Targets";
    //                    break;
    //
    //                case "Targets":
    //
    //                    //Consider putting all targets in a dictionary if they need different "NextIDs"
    //
    //                    SubMenu_Controller_2.Tab[] tabs = new SubMenu_Controller_2.Tab[character.TurnController.right_Players.Count];
    //
    //                    for (int i = 0; i < tabs.Length; i++)
    //                    {
    //                        tabs[i] = new SubMenu_Controller_2.Tab(character.TurnController.right_Players[i].name, smc.ConfirmSubMenu("Execute"));
    //                    }
    //
    //                    smc.OpenSubMenu(smc.subMenuID, "Targets", tabs);
    //
    //                    while (smc.waiting)
    //                    {
    //                        var stats = character.GetCombatStats(skill_Stats[0], character.TurnController.right_Players[smc.hovering]);
    //
    //                        character.TurnController.left_descriptionBox.ATK_Num.text = stats[Character_Stats.Stat.ATK].ToString();
    //                        character.TurnController.left_descriptionBox.HIT_Num.text = stats[Character_Stats.Stat.PhHit].ToString();
    //                        character.TurnController.left_descriptionBox.CRT_Num.text = stats[Character_Stats.Stat.Crit].ToString();
    //
    //                        yield return null;
    //                    }
    //
    //                    UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
    //                    smc.transform.GetChild(0).gameObject.SetActive(false);
    //
    //                    yield return null;
    //
    //                    int b = smc.currentSubMenu.buttonChoice;
    //
    //                    if (b == -1)
    //                    {
    //                        yield return smc.Return();
    //                        yield break;
    //                    }
    //                    else
    //                    {
    //                        smc.subMenuID = smc.currentButtons[b].nextID;
    //                        yield return smc.currentButtons[b].nextMethod;
    //                    }
    //
    //                    break;
    //
    //
    //                case "Execute":
    //                    smc.subMenuID = "Done";
    //                    done = true;
    //                    character.AddStatChanger(skill_Stats[0].statChanger);
    //
    //                    Combat_Character target = character.TurnController.right_Players[smc.dictionary["Targets"].buttonChoice];
    //                    character.chosenAction.targets.Add(target.transform);
    //
    //                    Execute = Action();
    //                    break;
    //
    //                default:
    //                    Debug.Log(smc.subMenuID + " Menu Dead End");
    //                    smc.subMenuID = smc.currentSubMenu.ID;
    //                    yield return smc.Return();
    //                    break;
    //            }
    //
    //        }
    //        while (!done);
    //    }
    //
    //    public IEnumerator Action()
    //    {
    // 
    //        character.enemyTransform = targets[0];
    //
    //        //GetOutcome(skill_Stats[0], targets[0]);
    //
    //        /*CAMERA CONTROL*/
    //
    //        //character.mCamera.BlackOut(0.9f, 0.5f);
    //
    //        Vector3 camTargetPos = new Vector3(targets[0].position.x, 0.5f, targets[0].position.z - 1.5f);
    //
    //        yield return character.TurnController.mainCamera.MovingTo(camTargetPos, 0.5f);
    //
    //        yield return character.MoveInRange(new Vector3(-0.35f, 0, 0));
    //
    //        character.animationController.Clip("Sakura Punch");
    //
    //        yield return character.WaitForKeyFrame();
    //
    //        //yield return character.TurnController.Reactions(Turn_Controller.Stage.IMPACT, info[0]);
    //
    //        CoroutineWithData cd = new CoroutineWithData(character, character.TurnController.Reactions(Turn_Controller.Stage.IMPACT, this));
    //        yield return cd.coroutine;
    //
    //        GetOutcome(skill_Stats[0], targets[0]);
    //
    //        Dictionary<Character_Stats.Stat, int> ownerDirectory = character.GetCombatStats(skill_Stats[0]);
    //
    //        Dictionary<Character_Stats.Stat, int> targetDirectory = targets[0].GetComponent<Combat_Character>().GetCurrentStats();
    //
    //        print(ownerDirectory[Character_Stats.Stat.ATK] + " " + targetDirectory[Character_Stats.Stat.DEF]);
    //
    //        Coroutine outcome;
    //
    //
    //        switch ((int)cd.result)
    //        {
    //            case 0:
    //
    //                print("Continue");
    //
    //                outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.GetCombatStats(skill_Stats[0], targets[0])[Character_Stats.Stat.ATK]));
    //
    //                if (HitSuccess == 1 && !targets[0].GetComponent<Combat_Character>().blocking)
    //                    yield return character.Hud.AffectTimerProgress(-1);
    //
    //                yield return character.animationController.coroutine;
    //                //character.animationController.Clip("Sakura Idle");
    //
    //                yield return outcome;
    //
    //                break;
    //
    //            case 1:
    //                print("Pause");
    //                break;
    //
    //            case 2:
    //                print("Break");
    //                yield break;
    //
    //        }
    //    }
    //
    //}
    //[Header("Skills")]
    //public Punch punch;

//
//    [System.Serializable]
//    public class Uppercut : Skill
//    {
//        public Uppercut(Combat_Character character) : base(character)
//        {
//            name = "Uppercut";
//
//            skill_Stats = new Skill_Stats[1];
//
//            skill_Stats[0] = new Skill_Stats()
//            {
//                attack = 1,
//
//                statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
//                {
//                    {Character_Stats.Stat.AS, 1 },
//                })
//                {
//                    name = name + " exhaust",
//                },
//            };
//
//            type = Type.Physical;
//            range = Range.Close;
//
//            description = "A simple uppercut";
//
//        }
//
//        public override IEnumerator SubMenus(MonoBehaviour owner)
//        {
//            SubMenu_Controller_2 smc = character.SubMenuController2;
//
//            bool done = false;
//
//            do
//            {
//                switch (smc.subMenuID)
//                {
//                    case "Start":
//                        smc.subMenuID = "Targets";
//                        break;
//
//                    case "Targets":
//
//                        //Consider putting all targets in a dictionary if they need different "NextIDs"
//
//                        SubMenu_Controller_2.Tab[] tabs = new SubMenu_Controller_2.Tab[character.TurnController.right_Players.Count];
//
//                        for (int i = 0; i < tabs.Length; i++)
//                        {
//                            tabs[i] = new SubMenu_Controller_2.Tab(character.TurnController.right_Players[i].name, smc.ConfirmSubMenu("Execute"));
//                        }
//
//                        smc.OpenSubMenu(smc.subMenuID, "Targets", tabs);
//
//                        while (smc.waiting)
//                        {
//                            var stats = character.GetCombatStats(skill_Stats[0], character.TurnController.right_Players[smc.hovering]);
//
//                            character.TurnController.left_descriptionBox.ATK_Num.text = stats[Character_Stats.Stat.ATK].ToString();
//                            character.TurnController.left_descriptionBox.HIT_Num.text = stats[Character_Stats.Stat.PhHit].ToString();
//                            character.TurnController.left_descriptionBox.CRT_Num.text = stats[Character_Stats.Stat.Crit].ToString();
//
//                            yield return null;
//                        }
//
//                        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
//                        smc.transform.GetChild(0).gameObject.SetActive(false);
//
//                        yield return null;
//
//                        int b = smc.currentSubMenu.buttonChoice;
//
//                        if (b == -1)
//                        {
//                            yield return smc.Return();
//                            yield break;
//                        }
//                        else
//                        {
//                            smc.subMenuID = smc.currentButtons[b].nextID;
//                            yield return smc.currentButtons[b].nextMethod;
//                        }
//
//                        break;
//
//
//                    case "Execute":
//                        smc.subMenuID = "Done";
//                        done = true;
//                        character.AddStatChanger(skill_Stats[0].statChanger);
//
//                        Combat_Character target = character.TurnController.right_Players[smc.dictionary["Targets"].buttonChoice];
//                        character.chosenAction.targets.Add(target.transform);
//
//                        Execute = Action();
//                        break;
//
//                    default:
//                        Debug.Log(smc.subMenuID + " Menu Dead End");
//                        smc.subMenuID = smc.currentSubMenu.ID;
//                        yield return smc.Return();
//                        break;
//                }
//
//            }
//            while (!done);
//        }
//
//        public IEnumerator Action()
//        {
//
//            character.enemyTransform = targets[0];
//
//            //GetOutcome(skill_Stats[0], targets[0]);
//
//            /*CAMERA CONTROL*/
//
//            //character.mCamera.BlackOut(0.9f, 0.5f);
//
//            Vector3 camTargetPos = new Vector3(targets[0].position.x, 0.5f, targets[0].position.z - 1.5f);
//
//            yield return character.TurnController.mainCamera.MovingTo(camTargetPos, 0.5f);
//
//            yield return character.MoveInRange(new Vector3(-0.35f, 0, 0));
//
//            character.animationController.Clip("Sakura Uppercut");
//
//            yield return character.WaitForKeyFrame();
//
//            //yield return character.TurnController.Reactions(Turn_Controller.Stage.IMPACT, info[0]);
//
//            CoroutineWithData cd = new CoroutineWithData(character, character.TurnController.Reactions(Turn_Controller.Stage.IMPACT, this));
//            yield return cd.coroutine;
//
//            GetOutcome(skill_Stats[0], targets[0]);
//
//            Dictionary<Character_Stats.Stat, int> ownerDirectory = character.GetCombatStats(skill_Stats[0]);
//
//            Dictionary<Character_Stats.Stat, int> targetDirectory = targets[0].GetComponent<Combat_Character>().GetCurrentStats();
//
//            print(ownerDirectory[Character_Stats.Stat.ATK] + " " + targetDirectory[Character_Stats.Stat.DEF]);
//
//            Coroutine outcome;
//
//
//            switch ((int)cd.result)
//            {
//                case 0:
//
//                    print("Continue");
//
//                    outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.GetCombatStats(skill_Stats[0], targets[0])[Character_Stats.Stat.ATK]));
//
//                    //if (HitSuccess == 1 && !targets[0].GetComponent<Combat_Character>().blocking)
//                    //    yield return targets[0].GetComponent<Combat_Character>().Hud.AffectTimerProgress(1);
//
//                    yield return character.animationController.coroutine;
//                    //character.animationController.Clip("Sakura Idle");
//
//                    yield return outcome;
//
//                    break;
//
//                case 1:
//                    print("Pause");
//                    break;
//
//                case 2:
//                    print("Break");
//                    yield break;
//
//            }
//        }
//
//    }
//    public Uppercut uppercut;
//
//
//    [System.Serializable]
//    public class Kick : Skill
//    {
//        public Kick(Combat_Character character) : base(character)
//        {
//            name = "Kick";
//
//            skill_Stats = new Skill_Stats[1];
//
//            skill_Stats[0] = new Skill_Stats()
//            {
//                attack = 1,
//
//                statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
//                {
//                    {Character_Stats.Stat.AS, 1 },
//                })
//                {
//                    name = name + " exhaust",
//                },
//            };
//
//            type = Type.Physical;
//            range = Range.Close;
//
//            effect = true;
//            description = "Upon successful hit; increase target AS by 1 second(s).";
//
//        }
//
//        public override IEnumerator SubMenus(MonoBehaviour owner)
//        {
//            SubMenu_Controller_2 smc = character.SubMenuController2;
//
//            bool done = false;
//
//            do
//            {
//                switch (smc.subMenuID)
//                {
//                    case "Start":
//                        smc.subMenuID = "Targets";
//                        break;
//
//                    case "Targets":
//
//                        //Consider putting all targets in a dictionary if they need different "NextIDs"
//
//                        SubMenu_Controller_2.Tab[] tabs = new SubMenu_Controller_2.Tab[character.TurnController.right_Players.Count];
//
//                        for (int i = 0; i < tabs.Length; i++)
//                        {
//                            tabs[i] = new SubMenu_Controller_2.Tab(character.TurnController.right_Players[i].name, smc.ConfirmSubMenu("Execute"));
//                        }
//
//                        smc.OpenSubMenu(smc.subMenuID, "Targets", tabs);
//
//                        while (smc.waiting)
//                        {
//                            var stats = character.GetCombatStats(skill_Stats[0], character.TurnController.right_Players[smc.hovering]);
//
//                            character.TurnController.left_descriptionBox.ATK_Num.text = stats[Character_Stats.Stat.ATK].ToString();
//                            character.TurnController.left_descriptionBox.HIT_Num.text = stats[Character_Stats.Stat.PhHit].ToString();
//                            character.TurnController.left_descriptionBox.CRT_Num.text = stats[Character_Stats.Stat.Crit].ToString();
//
//                            yield return null;
//                        }
//
//                        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
//                        smc.transform.GetChild(0).gameObject.SetActive(false);
//
//                        yield return null;
//
//                        int b = smc.currentSubMenu.buttonChoice;
//
//                        if (b == -1)
//                        {
//                            yield return smc.Return();
//                            yield break;
//                        }
//                        else
//                        {
//                            smc.subMenuID = smc.currentButtons[b].nextID;
//                            yield return smc.currentButtons[b].nextMethod;
//                        }
//
//                        break;
//
//
//                    case "Execute":
//                        smc.subMenuID = "Done";
//                        done = true;
//                        character.AddStatChanger(skill_Stats[0].statChanger);
//
//                        Combat_Character target = character.TurnController.right_Players[smc.dictionary["Targets"].buttonChoice];
//                        character.chosenAction.targets.Add(target.transform);
//
//                        Execute = Action();
//                        break;
//
//                    default:
//                        Debug.Log(smc.subMenuID + " Menu Dead End");
//                        smc.subMenuID = smc.currentSubMenu.ID;
//                        yield return smc.Return();
//                        break;
//                }
//
//            }
//            while (!done);
//        }
//
//        public IEnumerator Action()
//        {
//
//            character.enemyTransform = targets[0];
//
//            //GetOutcome(skill_Stats[0], targets[0]);
//
//            /*CAMERA CONTROL*/
//
//            //character.mCamera.BlackOut(0.9f, 0.5f);
//
//            Vector3 camTargetPos = new Vector3(targets[0].position.x, 0.5f, targets[0].position.z - 1.5f);
//
//            yield return character.TurnController.mainCamera.MovingTo(camTargetPos, 0.5f);
//
//            yield return character.MoveInRange(new Vector3(-0.35f, 0, 0));
//
//            character.animationController.Clip("Sakura Kick");
//
//            yield return character.WaitForKeyFrame();
//
//            //yield return character.TurnController.Reactions(Turn_Controller.Stage.IMPACT, info[0]);
//
//            CoroutineWithData cd = new CoroutineWithData(character, character.TurnController.Reactions(Turn_Controller.Stage.IMPACT, this));
//            yield return cd.coroutine;
//
//            GetOutcome(skill_Stats[0], targets[0]);
//
//            Dictionary<Character_Stats.Stat, int> ownerDirectory = character.GetCombatStats(skill_Stats[0]);
//
//            Dictionary<Character_Stats.Stat, int> targetDirectory = targets[0].GetComponent<Combat_Character>().GetCurrentStats();
//
//            print(ownerDirectory[Character_Stats.Stat.ATK] + " " + targetDirectory[Character_Stats.Stat.DEF]);
//
//            Coroutine outcome;
//
//
//            switch ((int)cd.result)
//            {
//                case 0:
//
//                    print("Continue");
//
//                    outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.GetCombatStats(skill_Stats[0], targets[0])[Character_Stats.Stat.ATK]));
//
//                    if (HitSuccess == 1 && !targets[0].GetComponent<Combat_Character>().blocking)
//                        yield return targets[0].GetComponent<Combat_Character>().Hud.AffectTimerProgress(1);
//
//                    yield return character.animationController.coroutine;
//                    //character.animationController.Clip("Sakura Idle");
//
//                    yield return outcome;
//
//                    break;
//
//                case 1:
//                    print("Pause");
//                    break;
//
//                case 2:
//                    print("Break");
//                    yield break;
//
//            }
//        }
//
//    }
//    public Kick kick;
//
//
//    [System.Serializable]
//    public class Combo : Skill
//    {
//        public Combo(Combat_Character character) : base(character)
//        {
//            name = "Punch, Punch, Kick!";
//
//            maxLevel = 3;
//
//            skill_Stats = new Skill_Stats[3];
//
//            skill_Stats[0] = new Skill_Stats()
//            {
//                attack = 1,
//
//                statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
//                {
//                    { Character_Stats.Stat.AS, 1 },
//                })
//                {
//                    name = name + " exhaust",
//                },
//            };
//
//            skill_Stats[1] = new Skill_Stats()
//            {
//                attack = 1,
//
//                statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
//                {
//                    { Character_Stats.Stat.AS, 2 },
//                })
//                {
//                    name = name + " exhaust",
//                },
//            };
//
//            skill_Stats[2] = new Skill_Stats()
//            {
//                attack = 1,
//
//                statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
//                {
//                    { Character_Stats.Stat.AS, 3 },
//                })
//                {
//                    name = name + " exhaust",
//                },
//            };
//
//
//
//            type = Type.Physical;
//            range = Range.Close;
//
//            effect = true;
//            description = "If last hit is successful; increase target AS by 1 second(s).";
//
//        }
//
//        public override IEnumerator SubMenus(MonoBehaviour owner)
//        {
//
//            SubMenu_Controller_2 smc = character.SubMenuController2;
//
//            bool done = false;
//
//            do
//            {
//                switch (smc.subMenuID)
//                {
//                    case "Start":
//                        smc.subMenuID = "Level";
//                        break;
//
//                    case "Level":
//
//                        SubMenu_Controller_2.Tab[] tabs = new SubMenu_Controller_2.Tab[maxLevel];
//
//                        for (int i = 0; i < tabs.Length; i++)
//                        {
//                            tabs[i] = new SubMenu_Controller_2.Tab((i+1).ToString(), "Targets");
//                        }
//
//                        smc.OpenSubMenu(smc.subMenuID, "Level", tabs);
//
//                        while (smc.waiting)
//                        {                            
//                            character.TurnController.left_descriptionBox.title.text = name + " Level " + (smc.hovering + 1);
//
//                            character.TurnController.left_descriptionBox.ATK_Mult.text = "x" + (smc.hovering + 1).ToString();
//
//                            character.TurnController.left_descriptionBox.ATK_Mult.transform.parent.gameObject.SetActive(smc.hovering + 1 > 1 ? true : false);
//
//                            var stats = character.GetCombatStats(skill_Stats[smc.hovering]);
//
//                            character.TurnController.left_descriptionBox.REC_Num.text = stats[Character_Stats.Stat.AS].ToString();
//                            character.TurnController.left_descriptionBox.HIT_Num.text = stats[Character_Stats.Stat.PhHit].ToString();
//                            character.TurnController.left_descriptionBox.CRT_Num.text = stats[Character_Stats.Stat.Crit].ToString();
//
//                            yield return null;
//                        }
//
//                        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
//                        smc.transform.GetChild(0).gameObject.SetActive(false);
//
//                        yield return null;
//
//                        int l = smc.currentSubMenu.buttonChoice;
//
//                        if (l == -1)
//                        {
//                            yield return smc.Return();
//                            yield break;
//                        }
//                        else
//                        {
//                            smc.subMenuID = smc.currentButtons[l].nextID;
//                            yield return smc.currentButtons[l].nextMethod;
//                        }
//                        break;
//
//                    case "Targets":
//
//                        //Consider putting all targets in a dictionary if they need different "NextIDs"
//
//                        tabs = new SubMenu_Controller_2.Tab[character.TurnController.right_Players.Count];
//
//                        for (int i = 0; i < tabs.Length; i++)
//                        {
//                            tabs[i] = new SubMenu_Controller_2.Tab(character.TurnController.right_Players[i].name, smc.ConfirmSubMenu("Execute"));
//                        }
//
//                        smc.OpenSubMenu(smc.subMenuID, "Targets", tabs);
//
//                        while (smc.waiting)
//                        {
//                            var stats = character.GetCombatStats(skill_Stats[0], character.TurnController.right_Players[smc.hovering]);
//
//                            character.TurnController.left_descriptionBox.ATK_Num.text = stats[Character_Stats.Stat.ATK].ToString();
//                            character.TurnController.left_descriptionBox.HIT_Num.text = stats[Character_Stats.Stat.PhHit].ToString();
//                            character.TurnController.left_descriptionBox.CRT_Num.text = stats[Character_Stats.Stat.Crit].ToString();
//
//                            yield return null;
//                        }
//
//                        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
//                        smc.transform.GetChild(0).gameObject.SetActive(false);
//
//                        yield return null;
//
//                        int b = smc.currentSubMenu.buttonChoice;
//
//                        if (b == -1)
//                        {
//                            yield return smc.Return();
//                        }
//                        else
//                        {
//                            smc.subMenuID = smc.currentButtons[b].nextID;
//                            yield return smc.currentButtons[b].nextMethod;
//                        }
//
//                        break;
//
//
//                    case "Execute":
//                        smc.subMenuID = "Done";
//                        done = true;
//                        character.AddStatChanger(skill_Stats[0].statChanger);
//
//                        int level = smc.dictionary["Level"].buttonChoice;
//
//                        Combat_Character target = character.TurnController.right_Players[smc.dictionary["Targets"].buttonChoice];
//                        character.chosenAction.targets.Add(target.transform);
//
//                        Execute = Action(level);
//                        break;
//
//                    default:
//                        Debug.Log(smc.subMenuID + " Menu Dead End");
//                        smc.subMenuID = smc.currentSubMenu.ID;
//                        yield return smc.Return();
//                        break;
//                }
//
//            }
//            while (!done);
//        }
//
//        public IEnumerator Action(int level)
//        {
//
//            character.enemyTransform = targets[0];
//
//            //GetOutcome(skill_Stats[0], targets[0]);
//
//            /*CAMERA CONTROL*/
//
//            //character.mCamera.BlackOut(0.9f, 0.5f);
//
//            Vector3 camTargetPos = new Vector3(targets[0].position.x, 0.5f, targets[0].position.z - 1.5f);
//
//            yield return character.TurnController.mainCamera.MovingTo(camTargetPos, 0.5f);
//
//            yield return character.MoveInRange(new Vector3(-0.35f, 0, 0));
//
//            character.animationController.Clip("Sakura Punch");
//
//            yield return character.WaitForKeyFrame();
//
//            //yield return character.TurnController.Reactions(Turn_Controller.Stage.IMPACT, info[0]);
//
//            CoroutineWithData cd = new CoroutineWithData(character, character.TurnController.Reactions(Turn_Controller.Stage.IMPACT, this));
//            yield return cd.coroutine;
//
//            GetOutcome(skill_Stats[0], targets[0]);
//
//            Dictionary<Character_Stats.Stat, int> ownerDirectory = character.GetCombatStats(skill_Stats[0]);
//
//            Dictionary<Character_Stats.Stat, int> targetDirectory = targets[0].GetComponent<Combat_Character>().GetCurrentStats();
//
//            print(ownerDirectory[Character_Stats.Stat.ATK] + " " + targetDirectory[Character_Stats.Stat.DEF]);
//
//            Coroutine outcome;
//
//
//            switch ((int)cd.result)
//            {
//                case 0:
//
//                    print("Continue");
//
//                    outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.GetCombatStats(skill_Stats[0], targets[0])[Character_Stats.Stat.ATK]));
//
//                    //if (HitSuccess == 1 && !targets[0].GetComponent<Combat_Character>().blocking)
//                    //    yield return targets[0].GetComponent<Combat_Character>().Hud.AffectTimerProgress(1);
//
//                    yield return character.animationController.coroutine;
//                    //character.animationController.Clip("Sakura Idle");
//
//                    yield return outcome;
//
//                    break;
//
//                case 1:
//                    print("Pause");
//                    break;
//
//                case 2:
//                    print("Break");
//                    yield break;
//
//            }
//
//            if (level == 0)
//                yield break;
//
//            // two
//
//            yield return character.MoveInRange(new Vector3(-0.35f, 0, 0));
//
//            GetOutcome(skill_Stats[1], targets[0]);
//
//            character.animationController.Clip("Sakura Uppercut");
//
//            yield return character.WaitForKeyFrame();
//            outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.GetCombatStats(skill_Stats[1], targets[0])[Character_Stats.Stat.ATK]));
//
//            //if (HitSuccess == 1 && !targets[0].GetComponent<Combat_Character>().blocking)
//            //    yield return targets[0].GetComponent<Combat_Character>().Hud.AffectTimerProgress(1);
//
//            yield return character.animationController.coroutine;
//            //character.animationController.Clip("Sakura Idle");
//
//            yield return outcome;
//
//            if (level == 1)
//                yield break;
//
//            // three
//
//            yield return character.MoveInRange(new Vector3(-0.35f, 0, 0));
//
//            GetOutcome(skill_Stats[2], targets[0]);
//
//            character.animationController.Clip("Sakura Kick");
//
//            yield return character.WaitForKeyFrame();
//            outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.GetCombatStats(skill_Stats[2], targets[0])[Character_Stats.Stat.ATK]));
//
//            if (HitSuccess == 1 && !targets[0].GetComponent<Combat_Character>().blocking)
//                yield return targets[0].GetComponent<Combat_Character>().Hud.AffectTimerProgress(1);
//
//            yield return character.animationController.coroutine;
//            //character.animationController.Clip("Sakura Idle");
//
//            yield return outcome;
//        }
//
//    }
//    public Combo combo;
//
//
//    [System.Serializable]
//    public class Jump_Kick : Skill
//    {
//        public Jump_Kick(Combat_Character character) : base(character)
//        {
//            this.character = character;
//
//            name = "Jump Kick";
//
//            skill_Stats = new Skill_Stats[1];
//
//            skill_Stats[0] = new Skill_Stats()
//            {
//                attack = 20,
//                accuracy = -3,
//                critical = 10,
//
//                statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
//                {
//                    { Character_Stats.Stat.AS, 3 },
//                })
//                {
//                    name = name + " exhaust",
//                },
//            };
//
//            effect = true;
//            description = "Upon successful hit; increase target AS by 2 second(s).";
//        }
//
//        public override IEnumerator SubMenus(MonoBehaviour owner)
//        {
//
//            SubMenu_Controller_2 smc = character.SubMenuController2;
//
//            bool done = false;
//
//            do
//            {
//                switch (smc.subMenuID)
//                {
//                    case "Start":
//                        smc.subMenuID = "Targets";
//                        break;
//
//                    case "Targets":
//
//                        //Consider putting all targets in a dictionary if they need different "NextIDs"
//
//                        SubMenu_Controller_2.Tab[] tabs = new SubMenu_Controller_2.Tab[character.TurnController.right_Players.Count];
//
//                        for (int i = 0; i < tabs.Length; i++)
//                        {
//                            tabs[i] = new SubMenu_Controller_2.Tab(character.TurnController.right_Players[i].name, smc.ConfirmSubMenu("Execute"));
//                        }
//
//                        smc.OpenSubMenu(smc.subMenuID, "Targets", tabs);
//
//                        while (smc.waiting)
//                        {
//                            var stats = character.GetCombatStats(skill_Stats[0], character.TurnController.right_Players[smc.hovering]);
//
//                            character.TurnController.left_descriptionBox.ATK_Num.text = stats[Character_Stats.Stat.ATK].ToString();
//                            character.TurnController.left_descriptionBox.HIT_Num.text = stats[Character_Stats.Stat.PhHit].ToString();
//                            character.TurnController.left_descriptionBox.CRT_Num.text = stats[Character_Stats.Stat.Crit].ToString();
//
//                            yield return null;
//                        }
//
//                        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
//                        smc.transform.GetChild(0).gameObject.SetActive(false);
//
//                        yield return null;
//
//                        int b = smc.currentSubMenu.buttonChoice;
//
//                        if (b == -1)
//                        {
//                            yield return smc.Return();
//                            yield break;
//                        }
//                        else
//                        {
//                            smc.subMenuID = smc.currentButtons[b].nextID;
//                            yield return smc.currentButtons[b].nextMethod;
//                        }
//
//                        break;
//
//
//                    case "Execute":
//                        smc.subMenuID = "Done";
//                        done = true;
//                        character.AddStatChanger(skill_Stats[0].statChanger);
//
//                        Combat_Character target = character.TurnController.right_Players[smc.dictionary["Targets"].buttonChoice];
//                        character.chosenAction.targets.Add(target.transform);
//
//                        Execute = Action();
//                        break;
//
//                    default:
//                        Debug.Log(smc.subMenuID + " Menu Dead End");
//                        smc.subMenuID = smc.currentSubMenu.ID;
//                        yield return smc.Return();
//                        break;
//                }
//
//            }
//            while (!done);
//        }
//
//        public IEnumerator Action()
//        {
//
//            character.enemyTransform = character.chosenAction.targets[0];
//
//            GetOutcome(skill_Stats[0], targets[0]);
//
//            yield return character.MoveInRange(new Vector3(-1f, 0, 0));
//
//
//            float maxTime = 0.4f;
//
//            character.GetComponent<Rigidbody>().isKinematic = true;
//
//            character.animationController.Clip("Sakura Jump");
//
//            character.StartCoroutine(character.JumpInRange(new Vector3(-0.3f, 0.1f, 0), maxTime));
//
//            yield return new WaitForSeconds(0.2085f);
//
//            //gameObject.GetComponent<AnimationController>().Clip("Sakura Fall");
//
//            //yield return new WaitForSeconds((maxTime / 2) - 0.1665f);
//
//            character.animationController.Clip("Sakura Jump Kick");
//
//            yield return character.WaitForKeyFrame();
//            Coroutine outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.GetCombatStats(skill_Stats[0], targets[0])[Character_Stats.Stat.ATK]));
//
//            if (HitSuccess == 1)
//            {
//                yield return targets[0].GetComponent<Combat_Character>().Hud.AffectTimerProgress(2);
//                yield return outcome;
//            }
//
//            character.GetComponent<Rigidbody>().isKinematic = false;
//
//            yield return character.animationController.coroutine;
//
//            //gameObject.GetComponent<AnimationController>().Clip("Sakura Fall");
//
//            yield return new WaitUntil(() => character.GetComponent<Rigidbody>().velocity.y > -0.1f);
//
//            character.animationController.Clip("Sakura Landing");
//
//            yield return character.animationController.coroutine;
//
//            yield return outcome;
//        }
//    }
//    public Jump_Kick jump_Kick;
//
//
//    [System.Serializable]
//    public class Throw_Kunai : Skill
//    {
//        public GameObject KunaiPrefab => Resources.Load<GameObject>("Projectiles/Kunai");
//
//        public Throw_Kunai(Combat_Character character) : base(character)
//        {
//            this.character = character;
//
//            name = "Throw Kunai";
//
//            chargeTime = 1;
//
//            maxLevel = 3;
//
//            skill_Stats = new Skill_Stats[1];
//
//            skill_Stats[0] = new Skill_Stats()
//            {
//                attack = 5,
//                accuracy = -5,
//                critical = 5,
//
//                statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
//                {
//                    { Character_Stats.Stat.AS, 3 },
//                })
//                {
//                    name = name + " exhaust",
//                },
//            };
//
//            range = Range.Far;
//
//            effect = true;
//            description = "Charge to a max of 2; each charge requires " + chargeTime + " second(s). Each charge will throw an extra kunai.";
//        }
//
//        public override IEnumerator SubMenus(MonoBehaviour owner)
//        {
//            bool done = false;
//
//            SubMenu_Controller_2 smc = character.SubMenuController2;
//
//            do {
//                switch (smc.subMenuID)
//                {
//                    case "Start":
//                        smc.subMenuID = "Decision";
//
//                        if (charging)
//                            numOfHits++;
//
//                        if (numOfHits > 0)
//                            character.TurnController.left_descriptionBox.title.text = name + " x " + numOfHits;
//
//                        break;
//
//                    case "Decision":
//
//                        if (numOfHits >= maxLevel)
//                            smc.OpenSubMenu(smc.subMenuID, name, new SubMenu_Controller_2.Tab[] { new SubMenu_Controller_2.Tab("Execute", "Targets") });
//                        else
//                            smc.OpenSubMenu(smc.subMenuID, name, new SubMenu_Controller_2.Tab[] { new SubMenu_Controller_2.Tab("Execute", "Targets"), new SubMenu_Controller_2.Tab("Charge", smc.ConfirmSubMenu("Charge")) });
//
//                        while (smc.waiting)
//                        {
//                            var stats = character.GetCombatStats(skill_Stats[0], character.TurnController.right_Players[smc.hovering]);
//
//                            character.TurnController.left_descriptionBox.ATK_Mult.text = "x" + (numOfHits + smc.hovering).ToString();
//
//                            character.TurnController.left_descriptionBox.ATK_Mult.transform.parent.gameObject.SetActive(numOfHits + smc.hovering > 1 ? true : false);
//
//                            yield return null;
//                        }
//
//                        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
//                        smc.transform.GetChild(0).gameObject.SetActive(false);
//
//                        yield return null;
//
//                        int b = smc.currentSubMenu.buttonChoice;
//
//                        if (b == -1)
//                        {
//                            yield return smc.Return();
//                            yield break;
//                        }
//                        else
//                        {
//                            smc.subMenuID = smc.currentButtons[b].nextID;
//                            yield return smc.currentButtons[b].nextMethod;
//                        }
//
//                        break;
//
//                    case "Targets":
//
//                        //Consider putting all targets in a dictionary if they need different "NextIDs"
//
//                        SubMenu_Controller_2.Tab[] tabs = new SubMenu_Controller_2.Tab[character.TurnController.right_Players.Count];
//
//                        for (int i = 0; i < tabs.Length; i++)
//                        {
//                            tabs[i] = new SubMenu_Controller_2.Tab(character.TurnController.right_Players[i].name, smc.ConfirmSubMenu("Execute"));
//                        }
//
//                        smc.OpenSubMenu(smc.subMenuID, "Targets", tabs);
//
//                        while (smc.waiting)
//                        {
//                            var stats = character.GetCombatStats(skill_Stats[0], character.TurnController.right_Players[smc.hovering]);
//
//                            character.TurnController.left_descriptionBox.ATK_Num.text = stats[Character_Stats.Stat.ATK].ToString();
//                            character.TurnController.left_descriptionBox.HIT_Num.text = stats[Character_Stats.Stat.PhHit].ToString();
//                            character.TurnController.left_descriptionBox.CRT_Num.text = stats[Character_Stats.Stat.Crit].ToString();
//
//                            yield return null;
//                        }
//
//                        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
//                        smc.transform.GetChild(0).gameObject.SetActive(false);
//
//                        yield return null;
//
//                        b = smc.currentSubMenu.buttonChoice;
//
//                        if (b == -1)
//                        {
//                            yield return smc.Return();
//                        }
//                        else
//                        {
//                            smc.subMenuID = smc.currentButtons[b].nextID;
//                            yield return smc.currentButtons[b].nextMethod;
//                        }
//
//                        break;
//
//                    case "Execute":
//                        smc.subMenuID = "Done";
//                        done = true;
//                        charging = false;
//                        character.AddStatChanger(skill_Stats[0].statChanger);
//
//                        Combat_Character target = character.TurnController.right_Players[smc.dictionary["Targets"].buttonChoice];
//                        character.chosenAction.targets.Add(target.transform);
//
//                        Execute = Action(numOfHits);
//                        numOfHits = 1;
//                        break;
//
//                    case "Charge":
//                        smc.subMenuID = "Done";
//                        done = true;
//                        charging = true;
//                        break;
//
//                    default:
//                        Debug.Log("Menu Dead End");
//                        smc.subMenuID = smc.currentSubMenu.ID;
//                        yield return smc.Return();
//                        break;
//                }
//
//            }
//            while (!done);
//        }
//
//        public IEnumerator Action(int charges)
//        {
//
//            character.enemyTransform = targets[0];
//
//            GetOutcome(skill_Stats[0], targets[0]);
//
//            //character.animationController.Clip("Sakura Idle");
//
//            yield return character.MoveInRange(new Vector3(-1.75f, 0, 0));
//
//            character.animationController.Clip("Sakura Kunai");
//
//            yield return character.WaitForKeyFrame();
//
//            GameObject[] kunai = new GameObject[charges];
//
//            for (int i = 0; i < charges; i++)
//            {
//                kunai[i] = Instantiate(KunaiPrefab, character.animationController.instatiatePoint.position, Quaternion.identity);
//            }
//
//
//            Coroutine tragectory = character.StartCoroutine(character.ProjectileArch(kunai[0].transform, new Vector3(0.8f, 0f, 0f), 0.4f));
//
//            Coroutine[] otherTrags = new Coroutine[charges - 1];
//
//            for (int i = 0; i < charges - 1; i++)
//            {
//                otherTrags[i] = character.StartCoroutine(Kunai(kunai[i + 1], 0.1f * (i + 1)));
//            }
//
//
//            if (character.Facing == 1)
//                yield return new WaitWhile(() => kunai[0].transform.position.x <= targets[0].position.x);
//            else
//                yield return new WaitWhile(() => kunai[0].transform.position.x >= targets[0].position.x);
//
//            if (HitSuccess != 0)
//            {
//                character.StopCoroutine(tragectory);
//                Destroy(kunai[0]);
//            }
//
//            Coroutine outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.GetCombatStats(skill_Stats[0], targets[0])[Character_Stats.Stat.ATK]));
//
//            yield return tragectory;
//
//            Destroy(kunai[0], 2);
//
//            //character.animationController.Clip("Sakura Idle");
//
//            yield return outcome;
//
//            for (int i = 0; i < otherTrags.Length; i++)
//            {
//                yield return otherTrags[i];
//            }
//
//        }
//
//        IEnumerator Kunai(GameObject kunai, float delay)
//        {
//
//            yield return new WaitForSeconds(delay);
//
//            Coroutine tragectory = character.StartCoroutine(character.ProjectileArch(kunai.transform, new Vector3(0.8f, 0f, 0f), 0.4f));
//
//            Coroutine outcome = null;
//
//            if (character.Facing == 1)
//                yield return new WaitWhile(() => kunai.transform.position.x <= targets[0].position.x);
//            else
//                yield return new WaitWhile(() => kunai.transform.position.x >= targets[0].position.x);
//
//            if (HitSuccess != 0)
//            {
//                character.StopCoroutine(tragectory);
//                Destroy(kunai);
//                outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.GetCombatStats(skill_Stats[0], targets[0])[Character_Stats.Stat.ATK]));
//            }
//
//            yield return tragectory;
//
//            Destroy(kunai, 2);
//
//            yield return outcome;
//        }
//    }
//    public Throw_Kunai throw_Kunai;
//
//
//    [System.Serializable]
//    public class Multi_Hit : Skill
//    {
//        public Multi_Hit(Combat_Character character) : base(character)
//        {
//            this.character = character;
//
//            name = "Multi Hit";
//
//            numOfHits = 4;
//
//            skill_Stats = new Skill_Stats[1];
//
//            skill_Stats[0] = new Skill_Stats();
//
//            description = "Release a flury of 4 punches.";
//        }
//
//        public override IEnumerator SubMenus(MonoBehaviour owner)
//        {
//
//            SubMenu_Controller_2 smc = character.SubMenuController2;
//
//            bool done = false;
//
//            do
//            {
//                switch (smc.subMenuID)
//                {
//                    case "Start":
//                        smc.subMenuID = "Targets";
//                        break;
//
//                    case "Targets":
//
//                        //Consider putting all targets in a dictionary if they need different "NextIDs"
//
//                        SubMenu_Controller_2.Tab[] tabs = new SubMenu_Controller_2.Tab[character.TurnController.right_Players.Count];
//
//                        for (int i = 0; i < tabs.Length; i++)
//                        {
//                            tabs[i] = new SubMenu_Controller_2.Tab(character.TurnController.right_Players[i].name, smc.ConfirmSubMenu("Execute"));
//                        }
//
//                        smc.OpenSubMenu(smc.subMenuID, "Targets", tabs);
//
//                        while (smc.waiting)
//                        {
//                            var stats = character.GetCombatStats(skill_Stats[0], character.TurnController.right_Players[smc.hovering]);
//
//                            character.TurnController.left_descriptionBox.ATK_Num.text = stats[Character_Stats.Stat.ATK].ToString();
//                            character.TurnController.left_descriptionBox.HIT_Num.text = stats[Character_Stats.Stat.PhHit].ToString();
//                            character.TurnController.left_descriptionBox.CRT_Num.text = stats[Character_Stats.Stat.Crit].ToString();
//
//                            yield return null;
//                        }
//
//                        UnityEngine.EventSystems.EventSystem.current.SetSelectedGameObject(null);
//                        smc.transform.GetChild(0).gameObject.SetActive(false);
//
//                        yield return null;
//
//                        int b = smc.currentSubMenu.buttonChoice;
//
//                        if (b == -1)
//                        {
//                            yield return smc.Return();
//                            yield break;
//                        }
//                        else
//                        {
//                            smc.subMenuID = smc.currentButtons[b].nextID;
//                            yield return smc.currentButtons[b].nextMethod;
//                        }
//
//                        break;
//
//
//                    case "Execute":
//                        smc.subMenuID = "Done";
//                        done = true;
//
//                        Combat_Character target = character.TurnController.right_Players[smc.dictionary["Targets"].buttonChoice];
//                        character.chosenAction.targets.Add(target.transform);
//
//                        Execute = Action();
//                        break;
//
//                    default:
//                        Debug.Log(smc.subMenuID + " Menu Dead End");
//                        smc.subMenuID = smc.currentSubMenu.ID;
//                        yield return smc.Return();
//                        break;
//                }
//
//            }
//            while (!done);
//        }
//
//        public IEnumerator Action()
//        {
//
//            character.enemyTransform = targets[0];
//            GetOutcome(skill_Stats[0], targets[0]);
//
//
//
//            /*CAMERA CONTROL*/
//
//            //character.mCamera.BlackOut(0.9f, 0.5f);
//
//            Vector3 camTargetPos = new Vector3(targets[0].position.x, 0.5f, targets[0].position.z - 1.5f);
//
//            yield return character.TurnController.mainCamera.MovingTo(camTargetPos, 0.5f);
//
//
//
//            yield return character.MoveInRange(new Vector3(-0.35f, 0, 0));
//
//            character.animationController.Clip("Sakura Multi Hit");
//
//            yield return character.WaitForKeyFrame();
//
//            //Coroutine outcome = character.StartCoroutine(character.ApplyOutcome(info[0]));
//
//
//
//
//            CoroutineWithData cd = new CoroutineWithData(character, character.TurnController.Reactions(Turn_Controller.Stage.IMPACT, this));
//            yield return cd.coroutine;
//            
//            Coroutine outcome = null;
//
//            switch ((int)cd.result)
//            {
//                case 0:    
//                    print("Continue");
//                    outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.GetCombatStats(skill_Stats[0], targets[0])[Character_Stats.Stat.ATK]));           
//                    break;
//            
//                case 1:
//                    print("Pause");
//                    break;
//            
//                case 2:
//                    print("Break");
//                    yield break;
//            
//            }
//
//            for (int i = 1; i < 4; i++)
//            {
//                yield return character.WaitForKeyFrame();
//
//                if (HitSuccess != 0)
//                    outcome = character.StartCoroutine(character.ApplyOutcome(HitSuccess, CritSuccess, character.GetCombatStats(skill_Stats[0], targets[0])[Character_Stats.Stat.ATK]));
//            }
//
//
//            yield return character.animationController.coroutine;
//            //character.animationController.Clip("Sakura Idle");
//
//            yield return outcome;
//        }
//    }
//    public Multi_Hit multi_Hit;
//
//
//    [System.Serializable]
//    public class Guard : Spell
//    {
//        public Guard(Combat_Character character) : base(character)
//        {
//            this.character = character;
//
//            name = "Guard";
//
//            skill_Stats = new Skill_Stats[2];
//
//            skill_Stats[0] = new Skill_Stats()
//            {
//                statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
//                {
//                    { Character_Stats.Stat.AS, 3 },
//                })
//                {
//                    name = name + " exhaust",
//                },
//            };
//
//            skill_Stats[1] = new Skill_Stats()
//            {
//                statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
//                {
//                    {Character_Stats.Stat.DEF, 100 },
//                    {Character_Stats.Stat.PhAvo, -1000 }
//                })
//                {
//                    name = name + " effect",
//                },
//            };
//
//
//            effect = true;
//            description = "At the point of impact on this character; increase DEF by " + skill_Stats[1].statChanger.statChanges[Character_Stats.Stat.DEF] + ".";
//
//            stage = Turn_Controller.Stage.IMPACT;
//
//            image = Resources.Load<Sprite>("Skill Icons/Block Icon");
//        }
//
//        public override IEnumerator SubMenus(MonoBehaviour owner)
//        {
//
//            bool done = false;
//
//            int i = 0;
//
//
//            while (!done)
//            {
//
//                switch (i)
//                {
//                    case 0:
//
//                        yield return character.SubMenuController.OpenSubMenu("Confirm", new List<string>() { "Confirm" });
//
//                        yield return character.SubMenuController.CurrentCD.coroutine;
//
//                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
//                        {
//                            character.AddStatChanger(skill_Stats[0].statChanger);
//
//                            done = true;
//                        }
//                        else
//                        {
//                            character.chosenAction = null;
//                            yield break;
//                        }
//
//                        break;
//                }
//
//            }
//
//            Execute = Action();
//        }
//
//        public IEnumerator Action()
//        {
//            character.animationController.Clip("Sakura Buff");
//
//            yield return character.WaitForKeyFrame();
//
//            character.SetSkill(this, image);
//
//            yield return character.animationController.coroutine;
//            //character.animationController.Clip("Sakura Idle");
//        }
//
//        public override bool Condition(Turn_Controller.Stage stage, Skill info)
//        {
//
//            if (character.TurnController.characterTurn.enemyTransform == character.transform && this.stage == stage && info.type == Type.Physical && info.range == Range.Close)
//                return true;
//
//            return false;
//        }
//
//        public override IEnumerator Action2(int slot)
//        {
//            character.blocking = true;
//
//            character.AddStatChanger(skill_Stats[1].statChanger);
//
//            character.ClearSkill(slot);
//
//            yield return 0;
//        }
//    }
//    public Guard guard;
//
//    [System.Serializable]
//    public class Heal : Spell
//    {
//
//        readonly int heal = 10;
//
//        public Heal(Combat_Character character) : base(character)
//        {
//            this.character = character;
//
//            name = "Heal";
//
//            chargeTime = 2;
//
//
//            skill_Stats = new Skill_Stats[1];
//
//            skill_Stats[0] = new Skill_Stats()
//            {
//                statChanger = new StatChanger(new Dictionary<Character_Stats.Stat, int>
//                {
//                    { Character_Stats.Stat.AS, 2 },
//                })
//                {
//                    name = name + " exhaust",
//                },
//            };
//
//            type = Type.Magic;
//            range = Range.Far;
//
//            maxLevel = 1;
//
//            chargeAnimation = "Sakura Charge";
//
//            effect = true;
//            description = "Heal this character for " + heal + " hp immediatly. At the start of this character's next turn; this character may heal again for " + heal + " hp.";
//
//            stage = Turn_Controller.Stage.TURN_START;
//
//            image = Resources.Load<Sprite>("Skill Icons/Heal Icon");
//        }
//
//        public override IEnumerator SubMenus(MonoBehaviour owner)
//        {
//            bool done = false;
//
//            int i = 0;
//
//
//            while (!done)
//            {
//
//                switch (i)
//                {
//                    case 0:
//
//                        character.TurnController.left_descriptionBox.container.gameObject.SetActive(true);
//
//                        if (charging)
//                            numOfHits++;
//
//                        i++;
//
//                        break;
//
//                    case 1:
//
//                        if (numOfHits >= maxLevel)
//                        {
//                            i = 2;
//                            break;
//                        }
//
//                        yield return character.SubMenuController.OpenSubMenu("Charges", new List<string>() { "Charge" });
//
//                        yield return character.SubMenuController.CurrentCD.coroutine;
//
//                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
//                        {
//                            i = 3;
//                        }
//                        else
//                        {
//                            character.chosenAction = null;
//                            numOfHits = 0;
//                            yield break;
//                        }
//
//                        break;
//
//                    case 2:
//
//                        //Execute
//
//                        //yield return character.SubMenuController.OpenSubMenu("Confirm", new List<string>() { "Confirm"});
//                        //
//                        //yield return character.SubMenuController.CurrentCD.coroutine;
//                        //
//                        //if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
//                        //{
//                            character.AddStatChanger(skill_Stats[0].statChanger);
//
//                            done = true;
//                            charging = false;
//                            Execute = Action();
//                            numOfHits = 0;
//                        //}
//
//                        break;
//
//                    case 3:
//
//                        // Charge
//
//                        yield return character.SubMenuController.OpenSubMenu("Confirm", new List<string>() { "Confirm" });
//
//                        yield return character.SubMenuController.CurrentCD.coroutine;
//
//                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
//                        {
//                            done = true;
//
//                            charging = true;
//
//                            character.animationController.Clip(chargeAnimation);
//
//                            character.TurnController.left_descriptionBox.container.gameObject.SetActive(false);
//                        }
//                        else
//                        {
//                            i = 1;
//                        }
//
//                        break;
//                }
//
//            }
//
//            character.EndTurn();
//        }
//
//        public IEnumerator Action()
//        {
//            GetOutcome(skill_Stats[0],character.transform);
//
//            character.animationController.Clip("Sakura Buff");
//
//            yield return character.WaitForKeyFrame();
//
//            // Should try to push thorough ApplyOutcome eventually
//
//            int totalHeal = heal * CritSuccess;
//
//            character.Health += totalHeal;
//
//            character.Mana += skill_Stats[0].mana;
//
//
//            if (CritSuccess != 1)
//                Instantiate(character.outcome_Bubble_Prefab, character.outcome_Bubble_Pos.position, Quaternion.identity).Input(totalHeal, "CRITICAL", Color.green);
//            else
//                Instantiate(character.outcome_Bubble_Prefab, character.outcome_Bubble_Pos.position, Quaternion.identity).Input(totalHeal);
//
//            //Instantiate(character.outcome_Bubble_Prefab, character.outcome_Bubble_Pos.position, Quaternion.identity).Input(totalHeal);
//
//            character.SetSkill(this, image);
//
//            yield return character.animationController.coroutine;
//            //character.animationController.Clip("Sakura Idle");
//        }
//
//        public override bool Condition(Turn_Controller.Stage stage, Skill info)
//        {
//            if (character.TurnController.characterTurn == character && this.stage == stage)
//                return true;
//
//            return false;
//        }
//
//        public override IEnumerator Action2(int slot)
//        {
//            character.animationController.Clip("Sakura Buff");
//
//            yield return character.WaitForKeyFrame();
//
//            character.Health += heal;
//
//            character.Mana += skill_Stats[0].mana;
//
//            Instantiate(character.outcome_Bubble_Prefab, character.outcome_Bubble_Pos.position, Quaternion.identity).Input(heal);
//
//            character.ClearSkill(slot);
//
//            yield return character.animationController.coroutine;
//            character.animationController.Clip("Sakura Idle");
//
//            yield return 0;
//        }
//
//    }
//    public Heal heal;


    private void Awake()
    {
        characterName = "Sakura";
    }

    public override IEnumerator Damage()
    {
        animationController.Clip("Sakura Damaged");
        yield return null;
    }

    public override IEnumerator Block()
    {
        animationController.Clip("Sakura Block");
        yield return animationController.coroutine;
    }

    public override IEnumerator Dodge()
    {
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

        //switch (r)
        //{
        //    case 0:
        //
        //        Skill skill = combo;
        //
        //        ActionChoice(skill);
        //
        //        chosenAction.targets.Add(target);
        //        //chosenAttack.targets.Add(TurnController.left_Players[0].transform);
        //
        //        int level = 2;
        //
        //        AddStatChanger(skill.skill_Stats[level-1].statChanger);
        //
        //        chosenAction.Execute = ((Combo)skill).Action(level);
        //
        //        skill.numOfHits = 3;
        //
        //
        //        DescriptionBox dBox = SubMenuController.Owner.TurnController.left_descriptionBox;
        //
        //        if(SubMenuController.Owner.TurnController.right_Players.Contains(this))
        //            dBox = SubMenuController.Owner.TurnController.right_descriptionBox;
        //
        //
        //        Spell spell = skill as Spell;
        //
        //        if (spell != null)
        //        {
        //            dBox.container.GetComponent<UnityEngine.UI.Image>().color = new Color(0.1058824f, 0.254902f, 0.2226822f, 0.7372549f);
        //        }
        //        else
        //        {
        //            dBox.container.GetComponent<UnityEngine.UI.Image>().color = new Color(0.1058824f, 0.1215686f, 0.254902f, 0.7372549f);
        //        }
        //
        //        dBox.title.text = skill.name;
        //
        //        if (skill.numOfHits > 0)
        //        {
        //            dBox.ATK_Mult.gameObject.SetActive(true);
        //            dBox.ATK_Mult.text = "x" + skill.numOfHits.ToString();
        //        }
        //        else
        //        {
        //            dBox.ATK_Mult.gameObject.SetActive(false);
        //        }
        //
        //
        //        var output = GetCombatStats(skill.skill_Stats[0], target);
        //
        //        dBox.ATK_Num.text = output[Character_Stats.Stat.ATK].ToString();
        //        dBox.ATK_Num.color = Color.white;
        //
        //        dBox.HIT_Num.text = output[Character_Stats.Stat.PhHit].ToString().ToString();
        //
        //        dBox.CRT_Num.text = output[Character_Stats.Stat.Crit].ToString().ToString();
        //
        //        dBox.REC_Num.text = output[Character_Stats.Stat.AS].ToString().ToString();
        //        dBox.REC_Num.color = Color.white;
        //
        //        dBox.MP_Num.text = skill.skill_Stats[0].mana.ToString();
        //
        //        string typeText = "[" + skill.type.ToString().ToUpper();
        //
        //        if (skill.effect)
        //        {
        //            typeText += " / EFFECT";
        //        }
        //
        //        typeText += "]";
        //
        //        dBox.type.text = typeText;
        //
        //        dBox.Description(skill);
        //
        //        dBox.container.SetActive(true);
        //
        //        break;
        //
        //    case 1:
        //
        //        ActionChoice(multi_Hit);
        //
        //        chosenAction.targets.Add(target);
        //
        //        chosenAction.Execute = multi_Hit.Action();
        //
        //        break;
        //}

        yield return new WaitForSeconds(1);     // Pretend to Think

        yield return null;
    }
}

