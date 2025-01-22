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
            SefiraConversationController.Instance.UpdateConversation(Sprites.Mon2trSprite, Sprites.Mon2tr_Color, LocalizeTextDataModel.instance.GetText("Mon2tr_Desc"));
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
                result = unit.hp > 0f && unit.IsAttackTargetable() && unit != this.model && !(creatureModel.script is BossAmiya) && !(creatureModel.script is Kaltsit) && !(creatureModel.script is LCP);
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
            return LocalizeTextDataModel.instance.GetText("Mon2tr_Name");
        }
        public Mon2trAnim animscript;
        public BossAmiya master;
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
                if (Extension.IsInRange(actor, target, 5f) && attackTimer <= 0f)
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
                if (e.Data.Name == "OnAttack" && Extension.IsInRange(this.actor, this.target, 5f))
                {
                    MovableObjectNode movableNode = this.target.GetMovableNode();
                    this.target.TakeDamage(this.actor, new DamageInfo(RwbpType.R, 15, 30));
                    this.target.TakeDamage(this.actor, new DamageInfo(RwbpType.P, 1, 3));
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
        public Mon2tr script;
        private float attackTimer;
    }
}
