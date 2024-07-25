﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class Sakura : Combat_Character
{


    /* This old skill contains code to allow for seperate targeting submenu loop (in submenu override class) */
    [System.Serializable]
    public class Combo_OLD : Skill
    {
        public Combo_OLD(Combat_Character character)
        {
            this.character = character;

            name = "Combo";

            chargeTime = 1f;
            maxCharges = 3;

            image = Resources.Load<Sprite>("Skill Icons/Block Icon");

            baseInfo = new Info[]
            {
                 new Info(50, Type.Physical, Range.Close),
                 new Info(70, Type.Physical, Range.Close),
                 new Info(100, Type.Physical, Range.Close)
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

                        List<string> labels = Enumerable.Range(1, maxCharges).Select(n => n.ToString()).ToList();

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

            /*CAMERA CONTROL*/

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


    [System.Serializable]
    public class Combo : Skill
    {
        public Combo(Combat_Character character)
        {
            this.character = character;

            name = "Combo";

            chargeTime = 0.1f;
            maxCharges = 3;

            image = Resources.Load<Sprite>("Skill Icons/Block Icon");

            baseInfo = new Info[]
            {
                 new Info(50, Type.Physical, Range.Close),
                 new Info(70, Type.Physical, Range.Close),
                 new Info(100, Type.Physical, Range.Close)
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

                        List<string> labels = Enumerable.Range(1, maxCharges).Select(n => n.ToString()).ToList();

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

                        // Returning to this submenu

                        if (character.chosenAttack.targets.Count > 0)
                            character.chosenAttack.targets.RemoveAt(character.chosenAttack.targets.Count - 1);

                        yield return character.SubMenuController.OpenSubMenu("Targets", character.TurnController.GetPlayerNames(character.Facing));

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {

                            if (character.Facing == 1)
                                character.chosenAttack.targets.Add(character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);
                            else
                                character.chosenAttack.targets.Add(character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);

                            i++;

                        }
                        else
                        {
                            i--;
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

            /*CAMERA CONTROL*/

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

            yield return character.MoveInRange(new Vector3(-0.35f, 0, 0));

            character.animationController.Clip("Sakura Kick");

            yield return character.WaitForKeyFrame();
            outcome = character.StartCoroutine(character.ApplyOutcome(currentInfo[2]));

            yield return character.animationController.coroutine;
            character.animationController.Clip("Sakura Idle");

            yield return outcome;
        }

    }
    public Combo combo;


    [System.Serializable]
    public class Jump_Kick : Skill
    {
        public Jump_Kick(Combat_Character character)
        {
            this.character = character;

            name = "Jump Kick";

            chargeTime = 1.5f;

            image = Resources.Load<Sprite>("Skill Icons/Block Icon");

            baseInfo = new Info[]
            {
                 new Info(18, Type.Physical, Range.Close),
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

                        if (character.chosenAttack.targets.Count > 0)
                            character.chosenAttack.targets.RemoveAt(character.chosenAttack.targets.Count - 1);


                        yield return character.SubMenuController.OpenSubMenu("Targets", character.TurnController.GetPlayerNames(character.Facing));

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {
                            if (character.Facing == 1)
                                character.chosenAttack.targets.Add(character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);
                            else
                                character.chosenAttack.targets.Add(character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);

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
            character.enemyTransform = character.chosenAttack.targets[0];

            character.chosenAttack.GetOutcome();

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
            Coroutine outcome = character.StartCoroutine(character.ApplyOutcome(character.chosenAttack.currentInfo[0]));

            if (character.chosenAttack.currentInfo[0].success != 0)
                yield return outcome;

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

        public Throw_Kunai(Combat_Character character)
        {
            this.character = character;

            name = "Throw Kunai";

            baseInfo = new Info[]
            {
                new Info(5, Type.Physical, Range.Far),
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

                        if (targets.Count > 0)
                            targets.RemoveAt(character.chosenAttack.targets.Count - 1);


                        yield return character.SubMenuController.OpenSubMenu("Targets", character.TurnController.GetPlayerNames(character.Facing));

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {
                            if (character.Facing == 1)
                                character.chosenAttack.targets.Add(character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);
                            else
                                character.chosenAttack.targets.Add(character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);

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

            GetOutcome();

            yield return character.MoveInRange(new Vector3(-1.75f, 0, 0));

            character.animationController.Clip("Sakura Kunai");

            yield return character.WaitForKeyFrame();

            GameObject kunai = Instantiate(kunaiPrefab, character.animationController.instatiatePoint.position, Quaternion.identity);

            //yield return ProjectileArch(kunai.transform, new Vector3(-0.1f, 0.2f, 0), 0.4f);

            Coroutine tragectory = character.StartCoroutine(character.ProjectileArch(kunai.transform, new Vector3(0.8f, 0f, 0f), 0.4f));


            if (character.Facing == 1)
                yield return new WaitWhile(() => kunai.transform.position.x <= targets[0].position.x);
            else
                yield return new WaitWhile(() => kunai.transform.position.x >= targets[0].position.x);

            if (currentInfo[0].success != 0)
            {
                character.StopCoroutine(tragectory);
                Destroy(kunai);
            }

            Coroutine outcome = character.StartCoroutine(character.ApplyOutcome(currentInfo[0]));

            yield return tragectory;

            Destroy(kunai, 2);

            character.animationController.Clip("Sakura Idle");

            yield return outcome;
        }
    }
    public Throw_Kunai throw_Kunai;


    [System.Serializable]
    public class Multi_Hit : Skill
    {
        public Multi_Hit(Combat_Character character)
        {
            this.character = character;

            name = "Multi Hit";

            baseInfo = new Info[]
            {
                new Info(5, Type.Physical, Range.Close),
                new Info(5, Type.Physical, Range.Close),
                new Info(5, Type.Physical, Range.Close),
                new Info(7, Type.Physical, Range.Close),
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

                        if (targets.Count > 0)
                            targets.RemoveAt(character.chosenAttack.targets.Count - 1);


                        yield return character.SubMenuController.OpenSubMenu("Targets", character.TurnController.GetPlayerNames(character.Facing));

                        if (character.SubMenuController.CurrentSubMenu.ButtonChoice > -1)
                        {
                            if (character.Facing == 1)
                                character.chosenAttack.targets.Add(character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);
                            else
                                character.chosenAttack.targets.Add(character.TurnController.right_Players[character.SubMenuController.CurrentSubMenu.ButtonChoice].transform);

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
            GetOutcome();



            /*CAMERA CONTROL*/

            character.mcamera.GetComponent<MainCamera>().BlackOut(0.9f, 0.5f);

            Vector3 camTargetPos = new Vector3(targets[0].position.x, 0.5f, targets[0].position.z - 1.5f);

            yield return character.mcamera.GetComponent<MainCamera>().LerpMoveIE(camTargetPos, 0.5f);



            yield return character.MoveInRange(new Vector3(-0.35f, 0, 0));

            character.animationController.Clip("Sakura Multi Hit");

            yield return character.WaitForKeyFrame();

            //Coroutine outcome = character.StartCoroutine(character.ApplyOutcome(info[0]));




            CoroutineWithData cd = new CoroutineWithData(character, character.TurnController.Reactions(Turn_Controller.Stage.IMPACT, currentInfo[0]));
            yield return cd.coroutine;
            
            Coroutine outcome = null;


            switch ((int)cd.result)
            {
                case 0:    
                    print("Continue");
                    outcome = character.StartCoroutine(character.ApplyOutcome(currentInfo[0]));           
                    break;
            
                case 1:
                    print("Pause");
                    break;
            
                case 2:
                    print("Break");
                    yield break;
            
            }
            //
            //
            //
            for (int i = 1; i < 4; i++)
            {
                yield return character.WaitForKeyFrame();

                currentInfo[i].success = currentInfo[0].success;

                if (currentInfo[i].success != 0)
                    outcome = character.StartCoroutine(character.ApplyOutcome(currentInfo[i]));
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

        public Defense(Combat_Character character)
        {
            this.character = character;

            name = "Block";

            chargeTime = 0.5f;

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
            character.animationController.Clip("Sakura Buff");
        
            yield return character.WaitForKeyFrame();
        
            character.Hud.SkillSlot(0, image);
        
            character.setSpells[0] = this;
        
            yield return character.animationController.coroutine;
            character.animationController.Clip("Sakura Idle");
        }

        public override bool Condition(Turn_Controller.Stage stage, Info info)
        {
            if (character.TurnController.characterTurn.enemyTransform == character.transform && this.stage == stage && info.type == Type.Physical && info.range == Range.Close)
                return true;

            return false;
        }

        public override IEnumerator Action2()
        {
            //character.TurnController.characterTurn.chosenAttack.Success = 0.5f;

            foreach(Info info in character.TurnController.characterTurn.chosenAttack.currentInfo)
            {
                info.damage = 0;
                info.success = 0.5f;
            }

            character.Hud.ClearSkillSlot(0);

            character.setSpells[0] = null;

            yield return 0;
        }
    }
    public Defense defense;


    public class Heal : Spell
    {

        public Heal(Combat_Character character)
        {
            this.character = character;

            name = "Heal";

            chargeTime = 1f;

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

            Execute = Action();
        }

        public IEnumerator Action()
        {
            character.animationController.Clip("Sakura Buff");

            yield return character.WaitForKeyFrame();

            character.Hud.SkillSlot(1, image);

            character.setSpells[1] = this;

            yield return character.animationController.coroutine;
            character.animationController.Clip("Sakura Idle");
        }

        public override bool Condition(Turn_Controller.Stage stage, Info info)
        {
            if (character.TurnController.characterTurn == character && this.stage == stage)
                return true;

            return false;
        }

        public override IEnumerator Action2()
        {
            character.animationController.Clip("Sakura Buff");

            yield return character.WaitForKeyFrame();

            character.Hud.ClearSkillSlot(1);

            character.setSpells[1] = null;

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
        int r = Random.Range(0, 2);

        switch (r)
        {
            case 0:

                AttackChoice(combo);

                chosenAttack.targets.Add(TurnController.left_Players[0].transform);
                chosenAttack.targets.Add(TurnController.left_Players[0].transform);
                chosenAttack.targets.Add(TurnController.left_Players[0].transform);

                chosenAttack.Execute = combo.Action(2);

                break;

            case 1:

                AttackChoice(multi_Hit);

                chosenAttack.targets.Add(TurnController.left_Players[0].transform);

                chosenAttack.Execute = multi_Hit.Action();

                break;
        }

        EndTurn();

        yield return null;
    }
}

