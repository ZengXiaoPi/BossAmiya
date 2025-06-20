﻿using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace BossAmiya
{
    public class BossAmiya : CreatureBase, IObserver
    {
        public static void Log(string text)
        {
            Notice.instance.Send("AddSystemLog", new object[]
            {
                text
            });
        }
        public void OnNotice(string notice, params object[] param)
        {
            if (this.model.qliphothCounter <= 0)
            {
                return;
            }
            if (notice == NoticeName.OnAgentDead)
            {
                WorkerModel agentModel = param[0] as WorkerModel;
                if (agentModel == null)
                {
                    return;
                }
                if (agentModel.DeadType == DeadType.EXECUTION)
                {
                    return;
                }
                if (this.dead.Contains(agentModel))
                {
                    return;
                }
                this.dead.Add(agentModel);
                this.deadCnt++;
                if (this.deadCnt >= 10)
                {
                    this.deadCnt = 0;
                    this.model.SubQliphothCounter();
                }
            }
            else if (notice == NoticeName.OnOfficerDie)
            {
                OfficerModel officerModel = param[0] as OfficerModel;
                if (officerModel == null)
                {
                    return;
                }
                if (officerModel.DeadType == DeadType.EXECUTION)
                {
                    return;
                }
                if (this.dead.Contains(officerModel))
                {
                    return;
                }
                this.dead.Add(officerModel);
                this.deadCnt++;
                if (this.deadCnt >= 10)
                {
                    this.deadCnt = 0;
                    this.model.SubQliphothCounter();
                }
            }
        }

        public override void ActivateQliphothCounter()
        {
            Escape();
        }
        public override void OnEnterRoom(UseSkill skill)
        {
            if (skill.skillTypeInfo.rwbpType == RwbpType.P)
            {
                skill.agent.Die();
                this.Escape();
            }
        }
        public override void OnFinishWork(UseSkill skill)
        {
            if (skill.GetCurrentFeelingState() == CreatureFeelingState.GOOD)
            {
                this.AddedQliphothCounter();
                skill.agent.RecoverHP(skill.agent.maxHp * 0.15f);
            }
            if (skill.elapsedTime >= 75f)
            {
                this.model.SubQliphothCounter();
            }
        }
        public override void Escape()
        {
            if (!model.IsEscaped())
            {
                try
                {
                    summonKaltsitTimer = 10f;
                    summonLCPTimer = 300f;
                    summonGoriaTimer = 600f;
                    summonKLTimer = 900f;
                    summonLamalianTimer = 1200f;
                    TheSignOfContinuation = 1500f;
                    SummonedCreature.Clear();
                    isSummoning = false;
                    isWillShocking = false;
                    isSignOfContinuation = false;
                    WillShock = 30f;
                    WillShockDamage = 6;
                    if (!HardModeManager.Instance.isHardMode())
                    {
                        isEmergency = false;
                        RandomEventLayer.currentLayer.AddTypo(RandomEventLayer.currentLayer.MakeDefaultTypoSession("ISW-DF", LocalizeTextDataModel.instance.GetText("AmiyaEscape_Title"), LocalizeTextDataModel.instance.GetText("AmiyaEscape_Desc"), Sprites.Amiya_Color, ""));
                    }
                    else
                    {
                        isEmergency = true;
                        RandomEventLayer.currentLayer.AddTypo(RandomEventLayer.currentLayer.MakeDefaultTypoSession("ISW-DF-Emergency", LocalizeTextDataModel.instance.GetText("AmiyaEscape_Title_Relic"), LocalizeTextDataModel.instance.GetText("AmiyaEscape_Desc_Relic"), Sprites.Escape_Relic_Color, ""));
                        EnterEmergencyState();
                    }
                    SefiraConversationController.Instance.UpdateConversation(Sprites.AmiyaSprite, Sprites.Amiya_Color, LocalizeTextDataModel.instance.GetText("BossAmiya_Desc"));
                    BgmManager.instance.FadeOut();
                    fullBGMplayer = PlayBGM("bgm-full.wav");
                    amiyaPhase = 1;
                    model.Escape();
                }
                catch (Exception ex)
                {
                    Harmony_Patch.logger.Error(ex);
                }
            }
        }
        private void EnterEmergencyState()
        {
            summonKaltsitTimer = 10f;
            summonLCPTimer = 150f;
            summonGoriaTimer = 300f;
            summonKLTimer = 600f;
            summonLamalianTimer = 900f;
            TheSignOfContinuation = 1200f;
            WillShockDamage = 12;
        }
        public override void OnStageStart()
        {
            base.OnStageStart();
            ParamInit();
        }
        public override void OnStageRelease()
        {
            base.OnStageRelease();
            ParamInit();
            this.SetObserver(false);
        }
        public override void OnStageEnd()
        {
            base.OnStageEnd();
            this.ParamInit();
            this.SetObserver(false);
        }
        public override void ParamInit()
        {
            deadCnt = 0;
            animscript.Default();
            if (!this.observingDanger)
            {
                this.SetObserver(true);
            }
        }
        private void SetObserver(bool active)
        {
            if (active)
            {
                global::Notice.instance.Observe(global::NoticeName.OnAgentDead, this);
                global::Notice.instance.Observe(global::NoticeName.OnOfficerDie, this);
            }
            else
            {
                global::Notice.instance.Remove(global::NoticeName.OnAgentDead, this);
                global::Notice.instance.Remove(global::NoticeName.OnOfficerDie, this);
            }
            this.observingDanger = active;
        }
        public override void OnViewInit(CreatureUnit unit)
        {
            base.OnViewInit(unit);
            animscript = (BossAmiyaAnim)unit.animTarget;
            animscript.SetScript(this);
        }
        public override void OnReturn()
        {
            if (fullBGMplayer != null) fullBGMplayer.Stop();
            if (loopBGMplayer != null) loopBGMplayer.Stop();
            BgmManager.instance.FadeIn();
            model.ResetQliphothCounter();
            animscript.Default();
        }
        public override bool isAttackInWorkProcess()
        {
            return false;
        }
        public override void OnSkillFailWorkTick(UseSkill skill)
        {
            if (!CheckCanTakeDamage(skill.agent)) return;
            skill.agent.TakeDamage(model, new DamageInfo(RwbpType.N, UnityEngine.Random.Range(3, 7)));
        }
        public override void UniqueEscape()
        {
            try
            {
                if (amiyaPhase == 1)
                {
                    CheckTheSignOfContinuation();
                    if (isSignOfContinuation) return;
                    if (!isWillShocking)
                    {
                        CheckKaltsitSummon();
                        CheckLCPSummon();
                        CheckGoriaSummon();
                        CheckKLSummon();
                        CheckLamalianSummon();
                    }
                    if (!isSummoning)
                    {
                        CheckWillShock();
                    }
                    if (!(this.model.GetCurrentCommand() is MoveCreatureCommand) && !isSummoning && !isWillShocking)
                    {
                        this.MakeMovement();
                    }
                }
                else if (amiyaPhase == 2)
                {
                    if (!(this.model.GetCurrentCommand() is MoveCreatureCommand) && !isSummoning && !isWillShocking)
                    {
                        this.MakeMovement();
                    }
                }
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        public List<UnitModel> GetNearEnemy()
        {
            List<UnitModel> list = [];
            PassageObjectModel passage = this.model.GetMovableNode().GetPassage();
            if (passage != null)
            {
                foreach (MovableObjectNode movableObjectNode in passage.GetEnteredTargets())
                {
                    bool flag2 = this.IsHostile(movableObjectNode);
                    if (flag2)
                    {
                        UnitModel unit = movableObjectNode.GetUnit();
                        list.Add(unit);
                    }
                }
            }
            return list;
        }
        public bool IsHostile(MovableObjectNode mov)
        {
            UnitModel unit = mov.GetUnit();
            bool result;
            if (unit is CreatureModel)
            {
                CreatureModel creatureModel = unit as CreatureModel;
                result = unit.hp > 0f && unit.IsAttackTargetable() && unit != this.model && !(creatureModel.script is BossAmiya) && !(creatureModel.script is Kaltsit) && !(creatureModel.script is LCP) && !(creatureModel.script is Mon2tr);
            }
            else
            {
                result = unit.hp > 0f && unit.IsAttackTargetable();
            }
            return result;
        }
        private void CheckWillShock()
        {
            if (WillShock <= 0f && !isWillShocking)
            {
                isWillShocking = true;
                if (!HardModeManager.Instance.isHardMode())
                {
                    WillShock = 120f;
                }
                else
                {
                    WillShock = 80f;
                }
                this.animscript.WillShock();
            }
            else
            {
                WillShock -= Time.deltaTime;
            }
        }
        private void CheckTheSignOfContinuation()
        {
            if (TheSignOfContinuation <= 0f && !isSignOfContinuation)
            {
                this.model.commandQueue.Clear();
                this.movable.StopMoving();
                int deadCount = 0;
                var luckCreatureList = new List<ChildCreatureModel>();
                foreach (ChildCreatureModel unit in childs)
                {
                    if (unit.hp > 0f)
                    {
                        luckCreatureList.Add(unit);
                        deadCount++;
                    }
                }
                if (deadCount > 0)
                {
                    var agentList = AgentManager.instance.GetAgentList();
                    var AliveAgentList = new List<AgentModel>();
                    foreach (AgentModel agent in agentList)
                    {
                        if (!agent.IsDead())
                        {
                            AliveAgentList.Add(agent);
                        }
                    }
                    var luckyAgentList = Extension.GetRandomElements<AgentModel>(AliveAgentList, deadCount * 2);
                    isSignOfContinuation = true;
                    animscript.IntoPhase2(luckCreatureList, luckyAgentList);
                }
                else
                {
                    isSignOfContinuation = true;
                    animscript.IntoPhase2();
                }
            }
            else
            {
                TheSignOfContinuation -= Time.deltaTime;
            }
        }
        private void CheckKaltsitSummon()
        {
            if (summonKaltsitTimer <= 0f && this.model.IsEscaped() && !isSummoning & !SummonedCreature.Contains("Kaltsit"))
            {
                Harmony_Patch.logger.Info("Summoning Kaltsit");
                SummonChildCreatureByAmiya("Kaltsit", "Custom/KaltsitAnim");
            }
            else if (summonLCPTimer >= 0f)
            {
                summonKaltsitTimer -= Time.deltaTime;
            }
        }
        private void CheckLCPSummon()
        {
            if (summonLCPTimer <= 0f && this.model.IsEscaped() && !isSummoning & !SummonedCreature.Contains("LCP"))
            {
                Harmony_Patch.logger.Info("Summoning LCP");
                SummonChildCreatureByAmiya("LCP", "Custom/LCPAnim");
            }
            else if (summonLCPTimer >= 0f)
            {
                summonLCPTimer -= Time.deltaTime;
            }
        }
        private void CheckGoriaSummon()
        {
            if (summonGoriaTimer <= 0f && this.model.IsEscaped() && !isSummoning & !SummonedCreature.Contains("Goria"))
            {
                Harmony_Patch.logger.Info("Summoning Goria");
                SummonChildCreatureByAmiya("Goria", "Custom/GoriaAnim");
            }
            else if (summonGoriaTimer >= 0f)
            {
                summonGoriaTimer -= Time.deltaTime;
            }
        }
        private void CheckKLSummon()
        {
            if (summonKLTimer <= 0f && this.model.IsEscaped() && !isSummoning & !SummonedCreature.Contains("KL"))
            {
                Harmony_Patch.logger.Info("Summoning KL");
                SummonChildCreatureByAmiya("KL", "Custom/KLAnim");
            }
            else if (summonKLTimer >= 0f)
            {
                summonKLTimer -= Time.deltaTime;
            }
        }
        private void CheckLamalianSummon()
        {
            if (summonLamalianTimer <= 0f && this.model.IsEscaped() && !isSummoning & !SummonedCreature.Contains("Lamalian"))
            {
                Harmony_Patch.logger.Info("Summoning LML");
                SummonChildCreatureByAmiya("Lamalian", "Custom/LamalianAnim");
            }
            else if (summonLamalianTimer >= 0f)
            {
                summonLamalianTimer -= Time.deltaTime;
            }
        }
        private void SummonChildCreatureByAmiya(string script, string anim)
        {
            isSummoning = true;
            SummonedCreature.Add(script);
            this.model.commandQueue.Clear();
            TrackEntry te1 = this.animscript.animator.AnimationState.SetAnimation(0, "A_Skill_Begin", false);
            te1.Complete += delegate
            {
                TrackEntry te2 = this.animscript.animator.AnimationState.SetAnimation(0, "A_Skill_Loop", false);
                te2.Complete += delegate
                {
                    TrackEntry te3 = this.animscript.animator.AnimationState.SetAnimation(0, "A_Skill_End", false);
                    te3.Complete += delegate
                    {
                        this.animscript.Default();
                        if (script == "Lamalian")
                        {
                            this.MakeChildCreature(this.movable, "Reid", "Custom/ReidAnim");
                        }
                        this.MakeChildCreature(this.movable, script, anim);
                        BossAmiya.isSummoning = false;
                    };
                };
            };
        }
        public override bool IsAttackTargetable()
        {
            if (amiyaPhase == 1) return false;
            return true;
        }
        public void MakeMovement()
        {
            this.model.ClearCommand();
            this.model.MoveToNode(MapGraph.instance.GetCreatureRoamingPoint());
            this.animscript.Move();
        }
        public static bool CheckCanTakeDamage(UnitModel model)
        {
            DefenseInfo defenseInfo = model.defense.Copy();
            bool red = defenseInfo.R < 0f;
            bool white = defenseInfo.W < 0f;
            bool black = defenseInfo.B < 0f;
            bool pale = defenseInfo.P < 0f;
            if (red && white && black && pale)
            {
                return false;
            }
            return true;
        }
        public void CheckDeadCreature()
        {
            if (summonGoriaTimer <= 0 && summonKLTimer <= 0 && summonLamalianTimer <= 0 && summonKaltsitTimer <= 0 && summonLCPTimer <= 0)
            {
                if (childs.Count == 0)
                {
                    TheSignOfContinuation = 0f;
                }
            }
        }
        public ChildCreatureModel MakeChildCreature(MovableObjectNode node, string ID, string Anim)
        {
            ChildCreatureModel childCreatureModel = null;
            try
            {
                long instID = model.AddChildCreatureModel(ID, Anim);
                childCreatureModel = model.GetChildCreatureModel(instID);
                childCreatureModel.Unit.init = true;
                childCreatureModel.GetMovableNode().SetDirection(UnitDirection.LEFT);
                childCreatureModel.Escape();
                childCreatureModel.GetMovableNode().Assign(node);
                if (ID == "Kaltsit")
                {
                    if (!HardModeManager.Instance.isHardMode())
                    {
                        childCreatureModel.baseMaxHp = 750;
                        childCreatureModel.hp = 750;
                    }
                    else
                    {
                        childCreatureModel.baseMaxHp = 2000;
                        childCreatureModel.hp = 2000;
                    }
                    childCreatureModel.SetSpeed(1.5f);
                    childCreatureModel.SetDefenseId("Kaltsit");
                    childCreatureModel.RiskLevel = RiskLevel.WAW;
                }
                else if (ID == "LCP")
                {
                    childCreatureModel.baseMaxHp = 2000;
                    childCreatureModel.hp = 2000;
                    childCreatureModel.SetSpeed(0.5f);
                    childCreatureModel.SetDefenseId("LCP");
                    childCreatureModel.RiskLevel = RiskLevel.ALEPH;
                }
                else if (ID == "Goria")
                {
                    childCreatureModel.baseMaxHp = 1200;
                    childCreatureModel.hp = 1200;
                    childCreatureModel.SetSpeed(0.7f);
                    childCreatureModel.SetDefenseId("Goria");
                    childCreatureModel.RiskLevel = RiskLevel.WAW;
                }
                else if (ID == "KL")
                {
                    childCreatureModel.baseMaxHp = 1500;
                    childCreatureModel.hp = 1500;
                    childCreatureModel.SetSpeed(1.6f);
                    childCreatureModel.SetDefenseId("KL");
                    childCreatureModel.RiskLevel = RiskLevel.ALEPH;
                }
                else if (ID == "Lamalian")
                {
                    childCreatureModel.baseMaxHp = 800;
                    childCreatureModel.hp = 800;
                    childCreatureModel.SetSpeed(1.6f);
                    childCreatureModel.SetDefenseId("Lamalian");
                    childCreatureModel.RiskLevel = RiskLevel.WAW;
                }
                else if (ID == "Reid")
                {
                    childCreatureModel.baseMaxHp = 1800;
                    childCreatureModel.hp = 1800;
                    childCreatureModel.SetSpeed(2.3f);
                    childCreatureModel.SetDefenseId("Reid");
                    childCreatureModel.RiskLevel = RiskLevel.ALEPH;
                }
                this.childs.Add(childCreatureModel);
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
            return childCreatureModel;
        }
        public static SoundEffectPlayer PlayBGM(string file, float volume = 1f, float maxDistance = 25f, bool loop = false)
        {
            SoundEffectPlayer result;
            try
            {
                WWW www = new WWW("file://" + Harmony_Patch.path + "/Bgm/" + file);
                while (!www.isDone)
                {
                }
                AudioClip audioClip = www.GetAudioClip(false, false);
                GameObject gameObject = Prefab.LoadPrefab("SoundEffectPlayer");
                SoundEffectPlayer component = gameObject.GetComponent<SoundEffectPlayer>();
                AudioSource component2 = gameObject.GetComponent<AudioSource>();
                gameObject.transform.SetParent(Camera.main.transform);
                gameObject.transform.localPosition = Vector3.zero;
                component2.clip = audioClip;
                component2.volume = volume;
                component2.loop = loop;
                component2.Play();
                component.src = component2;
                component.src.maxDistance = maxDistance;
                component.src.rolloffMode = AudioRolloffMode.Linear;
                component.SetDestroyTime(loop ? float.PositiveInfinity : component2.clip.length);
                result = component;
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
                result = null;
            }
            return result;
        }
        public BossAmiyaAnim animscript;
        public static int amiyaPhase = 0;
        public List<ChildCreatureModel> childs = [];
        public static SoundEffectPlayer fullBGMplayer;
        public static SoundEffectPlayer loopBGMplayer;
        public float summonKaltsitTimer = 10f;
        public float summonLCPTimer = 300f;
        public float summonGoriaTimer = 600f;
        public float summonKLTimer = 900f;
        public float summonLamalianTimer = 1500f;
        public float TheSignOfContinuation = 2100f;
        public bool isSignOfContinuation = false;
        public float WillShock = 120f;
        public int WillShockDamage = 1;
        public static bool isSummoning = false;
        public static bool isWillShocking = false;
        public static List<string> SummonedCreature = [];
        private int deadCnt = 0;
        private List<UnitModel> dead = [];
        public bool isEmergency = false;
        public bool observingDanger = false;
    }
}
