using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using static BossAmiya.KaltsitAnim;

namespace BossAmiya
{
    public class Kaltsit : CreatureBase
    {
        private ChildCreatureModel Model
        {
            get
            {
                return this.model as ChildCreatureModel;
            }
        }
        public override void OnViewInit(CreatureUnit unit)
        {
            try
            {
                animscript = (KaltsitAnim)unit.animTarget;
                animscript.SetScript(this);
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        public override bool OnAfterSuppressed()
        {
            this.master.childs.Remove(this.Model);
            this.isInited = false;
            this.animscript.PlayDeadMotion();
            if (this.Mon2trModel != null && this.Mon2trModel.hp > 0f)
            {
                Mon2trModel.AddUnitBuf(new Kaltsit_BloodyAngry());
            }
            if (this.Mon2trModel.hp <= 0f)
            {
                master.summonLCPTimer = 0f;
            }
            this.master.CheckDeadCreature();
            return true;
        }
        public override void Escape()
        {
            try
            {
                if (!this.model.IsEscaped())
                {
                    this.model.Escape();
                }
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        private void Init()
        {
            summonMon2trTimer = 120f;
            if (RougeManager.Instance.isHasRelic())
            {
                summonMon2trTimer = 15f;
            }
            isSummoning = false;
            Mon2trModel = null;
            SefiraConversationController.Instance.UpdateConversation(Sprites.KaltsitSprite, Sprites.Kaltsit_Color, LocalizeTextDataModel.instance.GetText("Kaltsit_Desc"));
            isInited = true;
        }
        public override void UniqueEscape()
        {
            try
            {
                this.model.CheckNearWorkerEncounting();
                List<UnitModel> nearEnemy = this.GetNearEnemy();
                if (this.model.state == CreatureState.ESCAPE && !this.isInited)
                {
                    Init();
                }
                if (summonMon2trTimer > 0f)
                {
                    summonMon2trTimer -= Time.deltaTime;
                }
                if (summonMon2trTimer <= 0f && !BossAmiya.SummonedCreature.Contains("Mon2tr"))
                {
                    Harmony_Patch.logger.Info("Summon Mon2tr.");
                    this.movable.StopMoving();
                    isSummoning = true;
                    this.model.commandQueue.Clear();
                    BossAmiya.SummonedCreature.Add("Mon2tr");
                    MovableObjectNode movableNode = this.model.GetMovableNode();
                    TrackEntry te1 = this.animscript.animator.AnimationState.SetAnimation(0, "Skill_Begin", false);
                    te1.Complete += delegate
                    {
                        TrackEntry te2 = this.animscript.animator.AnimationState.SetAnimation(0, "Skill_Loop", false);
                        te2.Complete += delegate
                        {
                            TrackEntry te3 = this.animscript.animator.AnimationState.SetAnimation(0, "Skill_End", false);
                            te3.Complete += delegate
                            {
                                this.animscript.Default();
                                this.MakeChildCreature(this.movable, "Mon2tr", "Custom/Mon2trAnim");
                                Extension.RecoveryHPForCreature(this.model, this.model.metaInfo.maxHp * 0.1f);
                                isSummoning = false;
                            };
                        };
                    };
                }
                else if (nearEnemy.Count > 0 && !(this.model.GetCurrentCommand() is KaltsitAttackCommand) && !isSummoning)
                {
                    this.model.commandQueue.Clear();
                    this.model.commandQueue.SetAgentCommand(new KaltsitAttackCommand(nearEnemy[0]));
                }
                else
                {
                    if (!(this.model.GetCurrentCommand() is MoveCreatureCommand) && !(this.model.GetCurrentCommand() is KaltsitAttackCommand) && !isSummoning)
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
        public ChildCreatureModel MakeChildCreature(MovableObjectNode node, string ID, string Anim)
        {
            ChildCreatureModel childCreatureModel = null;
            try
            {
                long instID = master.model.AddChildCreatureModel(ID, Anim);
                childCreatureModel = master.model.GetChildCreatureModel(instID);
                childCreatureModel.Unit.init = true;
                childCreatureModel.GetMovableNode().SetDirection(UnitDirection.LEFT);
                childCreatureModel.Escape();
                childCreatureModel.GetMovableNode().Assign(node);
                childCreatureModel.SetSpeed(1.7f);
                childCreatureModel.baseMaxHp = 2000;
                childCreatureModel.hp = 2000;
                childCreatureModel.SetDefenseId("Mon2tr");
                Mon2trModel = childCreatureModel;
                if (childCreatureModel.script is Mon2tr)
                {
                    Mon2tr mon2tr = childCreatureModel.script as Mon2tr;
                    mon2tr.kaltsit = this;
                }
                master.childs.Add(childCreatureModel);
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
            return childCreatureModel;
        }
        public override void SetModel(CreatureModel model)
        {
            this.model = model;
            this.SetParentAbnormality(((ChildCreatureModel)model).parent.script as BossAmiya);
        }
        private bool IsHostile(MovableObjectNode mov)
        {
            UnitModel unit = mov.GetUnit();
            bool result;
            if (unit is CreatureModel)
            {
                CreatureModel creatureModel = unit as CreatureModel;
                result = unit.hp > 0f && unit.IsAttackTargetable() && unit != this.model && Extension.CheckIsHostileCreature(creatureModel);
            }
            else
            {
                result = unit.hp > 0f && unit.IsAttackTargetable();
            }
            return result;
        }
        public List<UnitModel> GetNearEnemy()
        {
            List<UnitModel> list = new List<UnitModel>();
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
        public void SetParentAbnormality(BossAmiya master)
        {
            this.master = master;
        }

        public override string GetName()
        {
            return LocalizeTextDataModel.instance.GetText("Kaltsit_Name");
        }
        private void MakeMovement()
        {
            this.model.ClearCommand();
            this.animscript.Move();
            this.model.MoveToNode(MapGraph.instance.GetCreatureRoamingPoint());
        }
        private bool isInited = false;
        public KaltsitAnim animscript;
        public BossAmiya master;
        private float summonMon2trTimer = 30f;
        public bool isSummoning = false;
        public ChildCreatureModel Mon2trModel = null;
        public bool IsSummonedMon2tr
        {
            get
            {
                if (BossAmiya.SummonedCreature.Contains("Mon2tr"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }
    public class KaltsitAttackCommand : CreatureCommand
    {
        public KaltsitAttackCommand(UnitModel unitModel)
        {
            this.target = unitModel;
        }
        public override void OnInit(CreatureModel creature, CreatureCommandQueue cmdQueue)
        {
            base.OnInit(creature, cmdQueue);
            model = creature;
            script = creature.script as Kaltsit;
            attackTimer = 0f;
        }
        public override void Execute()
        {
            try
            {
                if (actor.state != CreatureState.ESCAPE)
                {
                    Finish();
                }
                else
                {
                    if (!target.IsAttackTargetable() || !this.actor.IsHostile(target))
                    {
                        Finish();
                    }
                    else
                    {
                        PassageObjectModel passage = target.GetMovableNode().GetPassage();
                        if (passage == null || passage != this.actor.GetMovableNode().GetPassage())
                        {
                            Finish();
                        }
                        else
                        {
                            this.MoveOrAttack();
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        private void MoveOrAttack()
        {
            try
            {
                attackTimer -= Time.deltaTime;
                MovableObjectNode movableNode = actor.GetMovableNode();
                if (Extension.IsInRange(actor, target, 3.5f) && attackTimer <= 0f)
                {
                    attackTimer = 2f;
                    movableNode.StopMoving();
                    script.animscript.Attack();
                    script.animscript.Event = new KaltsitEvent(NormalDamage);
                }
                else
                {
                    if (!movableNode.IsMoving() && this.attackTimer <= 0f)
                    {
                        movableNode.MoveToMovableNode(this.target.GetMovableNode(), false);
                        this.script.animscript.Move();
                    }
                }
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        public void NormalDamage(TrackEntry trackEntry, Event e)
        {
            try
            {
                if (e.Data.Name == "OnAttack" && Extension.IsInRange(this.actor, this.target, 3.5f))
                {
                    if (!script.IsSummonedMon2tr)
                    {
                        this.target.TakeDamage(this.actor, new DamageInfo(RwbpType.R, 7, 15));
                    }
                    else
                    {
                        this.target.TakeDamage(this.actor, new DamageInfo(RwbpType.R, 10, 20));
                        this.target.TakeDamage(this.actor, new DamageInfo(RwbpType.W, target.maxMental * 0.075f));
                    }
                    DamageParticleEffect.Invoker(this.target, RwbpType.R, this.actor);
                }
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        public UnitModel target;
        public CreatureModel model;
        public Kaltsit script;
        private float attackTimer;
    }
}
