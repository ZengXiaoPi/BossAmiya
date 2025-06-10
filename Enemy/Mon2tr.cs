using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using static BossAmiya.Mon2trAnim;

namespace BossAmiya
{
    public class Mon2tr : CreatureBase
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
            this.model.CheckNearWorkerEncounting();
            List<UnitModel> nearEnemy = this.GetNearEnemy();
            if (nearEnemy.Count > 0 && !(this.model.GetCurrentCommand() is Mon2trAttackCommand))
            {
                this.model.commandQueue.Clear();
                this.model.commandQueue.SetAgentCommand(new Mon2trAttackCommand(nearEnemy[0]));
            }
            else
            {
                if (!(this.model.GetCurrentCommand() is MoveCreatureCommand) && !(this.model.GetCurrentCommand() is Mon2trAttackCommand))
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
            if (this.kaltsit != null)
            {
                if (this.kaltsit.model.hp <= 0f)
                {
                    master.summonLCPTimer = 0f;
                }
            }
            this.master.CheckDeadCreature();
            return true;
        }
        public override void OnViewInit(CreatureUnit unit)
        {
            try
            {
                animscript = (Mon2trAnim)unit.animTarget;
                animscript.SetScript(this);
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        private void Init()
        {
            isInited = true;
            if (HardModeManager.Instance.isHardMode())
            {
                this.model.AddUnitBuf(new Mon2tr_HardBuff(this));
                SefiraConversationController.Instance.UpdateConversation(Sprites.Mon2trSprite, Sprites.Mon2tr_Color, LocalizeTextDataModel.instance.GetText("Mon2tr_Desc_Hard"));
            }
            else
            {
                SefiraConversationController.Instance.UpdateConversation(Sprites.Mon2trSprite, Sprites.Mon2tr_Color, LocalizeTextDataModel.instance.GetText("Mon2tr_Desc"));
            }
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
        public override string GetName()
        {
            if (!HardModeManager.Instance.isHardMode())
            {
                return LocalizeTextDataModel.instance.GetText("Mon2tr_Name");
            }
            else
            {
                return LocalizeTextDataModel.instance.GetText("Mon2tr_Name_Hard");
            }
        }
        public Mon2trAnim animscript;
        public BossAmiya master;
        public Kaltsit kaltsit;
        private bool isInited = false;
    }
    public class Mon2trAttackCommand : CreatureCommand
    {
        public Mon2trAttackCommand(UnitModel unitModel)
        {
            this.target = unitModel;
        }
        public override void OnInit(CreatureModel creature, CreatureCommandQueue cmdQueue)
        {
            base.OnInit(creature, cmdQueue);
            model = creature;
            script = creature.script as Mon2tr;
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
                if (((!HardModeManager.Instance.isHardMode() && Extension.IsInRange(actor, target, 5f)) || (HardModeManager.Instance.isHardMode() && Extension.IsInRange(actor, target, 6.5f))) && attackTimer <= 0f)
                {
                    attackTimer = 2f;
                    movableNode.StopMoving();
                    script.animscript.Attack();
                    script.animscript.Event = new Mon2trEvent(NormalDamage);
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
                if (e.Data.Name == "OnAttack" && Extension.IsInRange(this.actor, this.target, 5f) && !HardModeManager.Instance.isHardMode())
                {
                        this.target.TakeDamage(this.actor, new DamageInfo(RwbpType.R, 15, 30));
                        this.target.TakeDamage(this.actor, new DamageInfo(RwbpType.P, 1, 3));
                        DamageParticleEffect.Invoker(this.target, RwbpType.R, this.actor);
                }
                else if (e.Data.Name == "OnAttack" && Extension.IsInRange(this.actor, this.target, 6.5f) && HardModeManager.Instance.isHardMode())
                {
                    Harmony_Patch.RealDamage_TempList.Add(target);
                    this.target.TakeDamage(this.actor, new DamageInfo(RwbpType.N, 8, 15));
                    DamageParticleEffect.Invoker(this.target, RwbpType.R, this.actor);
                    this.model.hp += this.model.maxHp * 0.01f;
                    if (this.model.hp > this.model.maxHp)
                    {
                        this.model.hp = this.model.maxHp;
                    }
                }
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        public UnitModel target;
        public CreatureModel model;
        public Mon2tr script;
        private float attackTimer;
    }
}
