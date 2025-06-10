using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using static BossAmiya.KLAnim;

namespace BossAmiya
{
    public class KL : CreatureBase
    {
        private ChildCreatureModel Model
        {
            get
            {
                return this.model as ChildCreatureModel;
            }
        }
        public override void UniqueEscape()
        {
            if (this.model.state == CreatureState.ESCAPE && !this.isInited)
            {
                Init();
            }
            if (cwjTimer < 30f)
            {
                cwjTimer += Time.deltaTime;
            }
            if (cssTimer < 70f)
            {
                cssTimer += Time.deltaTime;
            }
            CheckSummon();
            this.model.CheckNearWorkerEncounting();
            List<UnitModel> nearEnemy = this.GetNearEnemy();
            if (nearEnemy.Count > 0 && !(this.model.GetCurrentCommand() is KLAttackCommand) && !isSummoning)
            {
                this.model.commandQueue.Clear();
                this.model.commandQueue.SetAgentCommand(new KLAttackCommand(nearEnemy[0]));
            }
            else if (!isSummoning)
            {
                if (!(this.model.GetCurrentCommand() is MoveCreatureCommand) && !(this.model.GetCurrentCommand() is KLAttackCommand))
                {
                    this.MakeMovement();
                }
            }
        }
        public override bool OnAfterSuppressed()
        {
            this.master.childs.Remove(this.Model);
            this.isInited = false;
            this.animscript.PlayDeadMotion();
            foreach (UnitModel unit in TSLZList)
            {
                unit.hp = 0;
            }
            this.master.summonLamalianTimer = 0f;
            this.master.CheckDeadCreature();
            return true;
        }
        public override void OnViewInit(CreatureUnit unit)
        {
            try
            {
                animscript = (KLAnim)unit.animTarget;
                animscript.SetScript(this);
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        private void CheckSummon()
        {
            if (isSummoning == true) return;
            if (cssTimer < 70f) return;
            if (TSLZList.Count >= 5) return;
            Harmony_Patch.logger.Info("Summon TSLZ.");
            isSummoning = true;
            this.model.commandQueue.Clear();
            this.movable.StopMoving();
            MovableObjectNode movableNode = this.model.GetMovableNode();
            animscript.Event = null;
            TrackEntry te1 = this.animscript.animator.AnimationState.SetAnimation(0, "B_Revive_Begin", false);
            te1.Complete += delegate
            {
                TrackEntry te2 = this.animscript.animator.AnimationState.SetAnimation(0, "B_Revive_Loop", false);
                te2.Complete += delegate
                {
                    if (TSLZList.Count < 4)
                    {
                        this.MakeChildCreature(this.movable, "TSLZ", "Custom/TSLZAnim");
                    }
                    this.MakeChildCreature(this.movable, "TSLZ", "Custom/TSLZAnim");
                    TrackEntry te3 = this.animscript.animator.AnimationState.SetAnimation(0, "B_Revive_End", false);
                    te3.Complete += delegate
                    {
                        this.animscript.Default();
                        isSummoning = false;
                        cssTimer = 0f;
                    };
                };
            };
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
                childCreatureModel.SetSpeed(1f);
                if (!HardModeManager.Instance.isHardMode())
                {
                    childCreatureModel.baseMaxHp = 70;
                    childCreatureModel.hp = 70;
                }
                else
                {
                    childCreatureModel.baseMaxHp = 150;
                    childCreatureModel.hp = 150;
                }
                childCreatureModel.SetDefenseId("TSLZ");
                TSLZ tslz = childCreatureModel.script as TSLZ;
                tslz.KLInstance = this;
                TSLZList.Add(childCreatureModel);
                SefiraConversationController.Instance.UpdateConversation(Sprites.TSLZSprite, Sprites.TSLZ_Color, LocalizeTextDataModel.instance.GetText("TSLZ_Desc"));
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
            return childCreatureModel;
        }
        private void Init()
        {
            cwjTimer = 0f;
            cssTimer = 10f;
            isSummoning = false;
            SefiraConversationController.Instance.UpdateConversation(Sprites.KLSprite, Sprites.KL_Color, LocalizeTextDataModel.instance.GetText("KL_Desc"));
            isInited = true;
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
        private void MakeMovement()
        {
            this.model.ClearCommand();
            this.animscript.Move();
            this.model.MoveToNode(MapGraph.instance.GetCreatureRoamingPoint());
        }
        public void SetParentAbnormality(BossAmiya master)
        {
            this.master = master;
        }
        public override void SetModel(CreatureModel model)
        {
            this.model = model;
            this.SetParentAbnormality(((ChildCreatureModel)model).parent.script as BossAmiya);
        }
        public bool IsHostile(MovableObjectNode mov)
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
        public override string GetName()
        {
            return LocalizeTextDataModel.instance.GetText("KL_Name");
        }
        public override string GetRiskLevel()
        {
            return "ALEPH";
        }
        public KLAnim animscript;
        public BossAmiya master;
        private bool isInited = false;
        public float cwjTimer = 0f;
        public float cssTimer = 15f;
        public bool isSummoning = false;
        public List<UnitModel> TSLZList = [];
    }
    public class KLAttackCommand : CreatureCommand
    {
        public KLAttackCommand(UnitModel unitModel)
        {
            this.target = unitModel;
        }
        public override void OnInit(CreatureModel creature, CreatureCommandQueue cmdQueue)
        {
            base.OnInit(creature, cmdQueue);
            model = creature;
            script = creature.script as KL;
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
                if (Extension.IsInRange(actor, target, 5f) && attackTimer <= 0f && ((script.cwjTimer >= 30f && !HardModeManager.Instance.isHardMode()) || (script.cwjTimer >= 20f && HardModeManager.Instance.isHardMode())))
                {
                    attackTimer = 5.5f;
                    _tempDamageCount = 1;
                    movableNode.StopMoving();
                    script.animscript.SpecialAttack();
                    script.animscript.Event = new KLEvent(SpecialDamage);
                }
                else if (Extension.IsInRange(actor, target, 5f) && attackTimer <= 0f)
                {
                    attackTimer = 1.5f;
                    movableNode.StopMoving();
                    script.animscript.Attack();
                    script.animscript.Event = new KLEvent(NormalDamage);
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
                if (e.Data.Name == "OnAttack" && Extension.IsInRange(this.actor, this.target, 5f))
                {
                    if (!HardModeManager.Instance.isHardMode())
                    {
                        this.target.TakeDamage(this.actor, new DamageInfo(RwbpType.W, 4, 12));
                        DamageParticleEffect.Invoker(this.target, RwbpType.W, this.actor);
                    }
                    else
                    {
                        Harmony_Patch.RealDamage_TempList.Add(this.target);
                        this.target.TakeDamage(this.actor, new DamageInfo(RwbpType.N, 4, 8));
                        this.target.TakeDamage(this.actor, new DamageInfo(RwbpType.W, 6, 16));
                        DamageParticleEffect.Invoker(this.target, RwbpType.W, this.actor);
                    }
                }
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        public void SpecialDamage(TrackEntry trackEntry, Event e)
        {
            try
            {
                if (e.Data.Name == "OnAttack" && Extension.IsInRange(this.actor, this.target, 5f))
                {
                    if (_tempDamageCount == 1)
                    {
                        this.target.TakeDamage(this.actor, new DamageInfo(RwbpType.W, 16, 16));
                        DamageParticleEffect.Invoker(this.target, RwbpType.W, this.actor);
                    }
                    else if (_tempDamageCount == 2)
                    {
                        this.target.TakeDamage(this.actor, new DamageInfo(RwbpType.R, 20, 20));
                        DamageParticleEffect.Invoker(this.target, RwbpType.R, this.actor);
                    }
                    else if (_tempDamageCount == 3)
                    {
                        this.target.TakeDamage(this.actor, new DamageInfo(RwbpType.B, 12, 12));
                        DamageParticleEffect.Invoker(this.target, RwbpType.B, this.actor);
                    }
                    else if (_tempDamageCount == 4)
                    {
                        this.target.TakeDamage(this.actor, new DamageInfo(RwbpType.P, 10, 10));
                        DamageParticleEffect.Invoker(this.target, RwbpType.P, this.actor);
                    }
                    else if (_tempDamageCount == 5)
                    {
                        Harmony_Patch.RealDamage_TempList.Add(this.target);
                        this.target.TakeDamage(this.actor, new DamageInfo(RwbpType.N, 25, 25));
                        DamageParticleEffect.Invoker(this.target, RwbpType.N, this.actor);
                        script.cwjTimer = 0f;
                    }
                    _tempDamageCount++;
                }
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        private int _tempDamageCount = 1;
        public UnitModel target;
        public CreatureModel model;
        public KL script;
        private float attackTimer;
    }
}
