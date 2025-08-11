using Spine;
using System;
using System.Collections.Generic;
using static BossAmiya.Mon2trAnim;
using UnityEngine;
using static BossAmiya.GoriaAnim;

namespace BossAmiya
{
    public class Goria : CreatureBase
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
            if (nearEnemy.Count > 0 && !(this.model.GetCurrentCommand() is GoriaAttackCommand))
            {
                this.model.commandQueue.Clear();
                this.model.commandQueue.SetAgentCommand(new GoriaAttackCommand(nearEnemy[0]));
            }
            else
            {
                if (!(this.model.GetCurrentCommand() is MoveCreatureCommand) && !(this.model.GetCurrentCommand() is GoriaAttackCommand))
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
            this.master.CheckDeadCreature();
            this.master.summonKLTimer = 0f;
            return true;
        }
        public override void OnViewInit(CreatureUnit unit)
        {
            try
            {
                animscript = (GoriaAnim)unit.animTarget;
                animscript.SetScript(this);
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        private void Init()
        {
            attackMP = 0;
            SefiraConversationController.Instance.UpdateConversation(Sprites.GoriaSprite, Sprites.Goria_Color, LocalizeTextDataModel.instance.GetText("Goria_Desc"));
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
            return LocalizeTextDataModel.instance.GetText("Goria_Name");
        }
        public GoriaAnim animscript;
        public BossAmiya master;
        private bool isInited = false;
        public int attackMP = 0;
    }
    public class GoriaAttackCommand : CreatureCommand
    {
        public GoriaAttackCommand(UnitModel unitModel)
        {
            this.target = unitModel;
        }
        public override void OnInit(CreatureModel creature, CreatureCommandQueue cmdQueue)
        {
            base.OnInit(creature, cmdQueue);
            model = creature;
            script = creature.script as Goria;
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
                if (Extension.IsInRange(actor, target, 5f) && attackTimer <= 0f && script.attackMP >= 3)
                {
                    attackTimer = 6f;
                    movableNode.StopMoving();
                    script.animscript.SpecialAttack();
                    script.animscript.Event = new GoriaEvent(SpecialDamage);
                    script.attackMP = 0;
                }
                else if (Extension.IsInRange(actor, target, 5f) && attackTimer <= 0f)
                {
                    attackTimer = 4.5f;
                    movableNode.StopMoving();
                    script.animscript.NormalAttack();
                    script.animscript.Event = new GoriaEvent(NormalDamage);
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
                        this.target.TakeDamage(this.actor, new DamageInfo(RwbpType.R, 30, 50));
                        this.target.AddUnitBuf(new Goria_CantMove());
                        DamageParticleEffect.Invoker(this.target, RwbpType.R, this.actor);
                    }
                    else
                    {
                        if (this.target is WorkerModel)
                        {
                            var target_asworker = target as WorkerModel;
                            target_asworker.Die();
                        }
                        else
                        {
                            target.hp = 0;
                        }
                    }
                }
                script.attackMP++;
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
                    if (target.hp < 0)
                    {
                        script.attackMP = 3;
                    }
                    if (this.target is WorkerModel)
                    {
                        var target_asworker = target as WorkerModel;
                        target_asworker.Die();
                    }
                    else
                    {
                        target.TakeDamage(this.actor, new DamageInfo(RwbpType.R, 75, 75));
                        DamageParticleEffect.Invoker(this.target, RwbpType.R, this.actor);
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
        public Goria script;
        private float attackTimer;
    }
}
