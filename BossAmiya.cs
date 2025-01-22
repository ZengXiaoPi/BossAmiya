using Spine;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.WindowsRuntime;
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
            if (notice == global::NoticeName.OnAgentDead)
            {
                global::AgentModel agentModel = param[0] as global::AgentModel;
                if (agentModel == null)
                {
                    return;
                }
                if (agentModel.DeadType == global::DeadType.EXECUTION)
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
            else if (notice == global::NoticeName.OnOfficerDie)
            {
                global::OfficerModel officerModel = param[0] as global::OfficerModel;
                if (officerModel == null)
                {
                    return;
                }
                if (officerModel.DeadType == global::DeadType.EXECUTION)
                {
                    return;
                }
                if (this.dead.Contains(officerModel))
                {
                    return;
                }
                this.dead.Add(officerModel);
                this.deadCnt++;
                if (this.deadCnt >= 5)
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
                    SummonedCreature.Clear();
                    isSummoning = false;
                    if (UnityEngine.Random.Range(0, 2) == 0)
                    {
                        AngelaConversationUI.instance.AddAngelaMessage(LocalizeTextDataModel.instance.GetText("Angela_Message_1").Replace("$1", GetName()));
                    }
                    else
                    {
                        AngelaConversationUI.instance.AddAngelaMessage(LocalizeTextDataModel.instance.GetText("Angela_Message_2").Replace("$1", GetName()));
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
        public override void OnStageStart()
        {
            base.OnStageStart();
            ParamInit();
        }
        public override void OnStageRelease()
        {
            base.OnStageRelease();
            ParamInit();
        }
        public override void ParamInit()
        {
            deadCnt = 0;
            animscript.Default();
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
                    CheckKaltsitSummon();
                    CheckLCPSummon();
                    if (!(model.GetCurrentCommand() is MoveCreatureCommand) && !isSummoning)
                    {
                        MakeMovement();
                    }
                }
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        private void CheckKaltsitSummon()
        {
            if (summonKaltsitTimer <= 0f && this.model.IsEscaped() && !isSummoning & !SummonedCreature.Contains("Kaltsit"))
            {
                Harmony_Patch.logger.Info("Summoning Kaltsit");
                SummonChildCreatureByAmiya("Kaltsit", "Custom/KaltsitAnim");
            }
            else
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
            else
            {
                summonLCPTimer -= Time.deltaTime;
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
        private void MakeMovement()
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
                    childCreatureModel.baseMaxHp = 1500;
                    childCreatureModel.hp = 1500;
                    childCreatureModel.SetSpeed(1.5f);
                    childCreatureModel.SetDefenseId("Kaltsit");
                    childCreatureModel.RiskLevel = RiskLevel.WAW;
                }
                else if (ID == "LCP")
                {
                    childCreatureModel.baseMaxHp = 2500;
                    childCreatureModel.hp = 2500;
                    childCreatureModel.SetSpeed(0.5f);
                    childCreatureModel.SetDefenseId("LCP");
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
        public List<ChildCreatureModel> childs = new List<ChildCreatureModel>();
        public static SoundEffectPlayer fullBGMplayer;
        public static SoundEffectPlayer loopBGMplayer;
        private float summonKaltsitTimer = 5f;
        private float summonLCPTimer = 300f;
        public static bool isSummoning = false;
        public static List<string> SummonedCreature = new List<string>();
        private int deadCnt = 0;
        private List<UnitModel> dead = new List<UnitModel>();
    }
}
