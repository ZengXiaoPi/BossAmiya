using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BossAmiya.Mon2trAnim;
using UnityEngine;

namespace BossAmiya
{
    public class TSLZ : CreatureBase
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
            if (nearEnemy.Count > 0 && !(this.model.GetCurrentCommand() is TSLZMoveToAgentCommand))
            {
                this.model.commandQueue.Clear();
                this.model.commandQueue.SetAgentCommand(new TSLZMoveToAgentCommand(nearEnemy[0]));
            }
            else
            {
                if (!(this.model.GetCurrentCommand() is MoveCreatureCommand) && !(this.model.GetCurrentCommand() is TSLZMoveToAgentCommand))
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
            KLInstance.TSLZList.Remove(this.model);
            this.master.CheckDeadCreature();
            return true;
        }
        public override void OnViewInit(CreatureUnit unit)
        {
            try
            {
                animscript = (TSLZAnim)unit.animTarget;
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
                    if (this.IsHostile(movableObjectNode) && movableObjectNode.GetUnit() is AgentModel)
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
            return LocalizeTextDataModel.instance.GetText("TSLZ_Name");
        }
        public override string GetRiskLevel()
        {
            return "ZAYIN";
        }
        public TSLZAnim animscript;
        public BossAmiya master;
        public KL KLInstance;
        private bool isInited = false;
    }
    public class TSLZMoveToAgentCommand : CreatureCommand
    {
        public TSLZMoveToAgentCommand(UnitModel unitModel)
        {
            this.target = unitModel;
        }
        public override void OnInit(CreatureModel creature, CreatureCommandQueue cmdQueue)
        {
            base.OnInit(creature, cmdQueue);
            model = creature;
            script = creature.script as TSLZ;
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
                    else if (!(target is AgentModel))
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
                MovableObjectNode movableNode = actor.GetMovableNode();
                if (Extension.IsInRange(actor, target, 2f))
                {
                    movableNode.StopMoving();
                    AgentModel asAgent = target as AgentModel;
                    asAgent.Die();
                    asAgent.visible = false;
                    asAgent.workerAnimator.gameObject.SetActive(false);
                    this.script.model.hp = 0f;
                }
                else
                {
                    if (!movableNode.IsMoving())
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
        public UnitModel target;
        public CreatureModel model;
        public TSLZ script;
    }
}
