using Spine;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;
using static BossAmiya.ReidAnim;

namespace BossAmiya
{
    public class Reid : CreatureBase
    {
        private ChildCreatureModel Model
        {
            get
            {
                return this.model as ChildCreatureModel;
            }
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
        private void MakeMovement()
        {
            this.model.ClearCommand();
            this.animscript.Move();
            this.model.MoveToNode(MapGraph.instance.GetCreatureRoamingPoint());
        }
        public override void UniqueEscape()
        {
            try
            {
                this.model.GetMovableNode().ProcessMoveNode(this.model.metaInfo.speed * this.model.movementScale * this.model.GetMovementScaleByBuf());
                this.model.movementScale = 1f + speed;
                if (this.model.state == CreatureState.ESCAPE && !this.isInited)
                {
                    Init();
                }
                this.model.CheckNearWorkerEncounting();
                List<UnitModel> nearEnemy = this.GetNearEnemy();
                if (nearEnemy.Count > 0 && !(this.model.GetCurrentCommand() is ReidAttackCommand) && !this.isChangingPhase && !this.isRushing)
                {
                    this.model.commandQueue.Clear();
                    this.model.commandQueue.SetAgentCommand(new ReidAttackCommand(nearEnemy[0]));
                }
                else if (!this.isChangingPhase && !this.isRushing)
                {
                    if (!(this.model.GetCurrentCommand() is MoveCreatureCommand) && !(this.model.GetCurrentCommand() is ReidAttackCommand))
                    {
                        this.MakeMovement();
                    }
                }
                if (this.isChangingPhase)
                {
                    if (this.model.hp >= this.model.maxHp)
                    {
                        this.model.hp = this.model.maxHp;
                        this.isChangingPhase = false;
                        this.phase = ReidPhase.Phase2;
                        this.animscript.OutOfChangingPhase();
                        rushingTime = 0f;
                    }
                    this.model.hp += 2;
                }
                if (phase == ReidPhase.Phase2)
                {
                    if (!isRushing)
                    {
                        rushingTime -= Time.deltaTime;
                        if (rushingTime <= 0f)
                        {
                            this.isRushing = true;
                            this.model.commandQueue.Clear();
                            this.movable.StopMoving();
                            var agentList = AgentManager.instance.GetAgentList();
                            var AliveAgentList = new List<AgentModel>();
                            foreach (AgentModel agent in agentList)
                            {
                                if (!agent.IsDead())
                                {
                                    AliveAgentList.Add(agent);
                                }
                            }
                            rushTarget = AliveAgentList[UnityEngine.Random.Range(0, AliveAgentList.Count)];
                            animscript.StartRushing();
                            rushingPrepareTime = 0.5f;
                            isPreparedRushing = false;
                            rushTime = 0f;
                        }
                    }
                    else
                    {
                        rushTime += Time.deltaTime;
                        _tempTimer += Time.deltaTime;
                        if (!HardModeManager.Instance.isHardMode() && _tempTimer >= 0.1f)
                        {
                            speed += 0.018f;
                            if (speed > 5f)
                            {
                                speed = 5f;
                            }
                            _tempTimer = 0f;
                        }
                        else if (_tempTimer >= 0.1f)
                        {
                            speed += 0.03f;
                            if (speed > 7.5f)
                            {
                                speed = 7.5f;
                            }
                            _tempTimer = 0f;
                        }
                        if (rushingPrepareTime > 0f && !isPreparedRushing)
                        {
                            rushingPrepareTime -= Time.deltaTime;
                        }
                        else if (rushingPrepareTime <= 0f && !isPreparedRushing)
                        {
                            Harmony_Patch.logger.Info("Rushing");
                            tempAgentMovable = this.rushTarget.GetMovableNode();
                            this.movable.MoveToMovableNode(tempAgentMovable, false);
                            this.animscript.RushingMoving();
                            isPreparedRushing = true;
                        }
                        else if (Extension.IsInRange(this.model, this.rushTarget, 1.5f))
                        {
                            isRushing = false;
                            isPreparedRushing = false;
                            rushingPrepareTime = 0.5f;
                            rushingTime = 300f;
                            this.animscript.Default();
                            this.rushTarget.TakeDamage(this.model, new DamageInfo(RwbpType.R, (int)(60 * this.speed), (int)(60 * this.speed)));
                            DamageParticleEffect.Invoker(this.rushTarget, RwbpType.R, this.model);
                            if (!rushTarget.IsDead())
                            {
                                this.rushTarget.AddUnitBuf(new Reid_Slow(this.speed * 2));
                            }
                            if (rushTime <= 15f)
                            {
                                this.model.AddUnitBuf(new Reid_RushUpgrade());
                            }
                            speed = 0f;
                        }
                        if (isPreparedRushing && (tempAgentMovable == null || !this.movable.IsMoving()))
                        {
                            tempAgentMovable = this.rushTarget.GetMovableNode();
                            this.movable.MoveToMovableNode(tempAgentMovable, false);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        public float _tempTimer = 0f;
        public override bool OnAfterSuppressed()
        {
            try
            {
                this.master.childs.Remove(this.Model);
                this.isInited = false;
                this.animscript.PlayDeadMotion();
                this.master.CheckDeadCreature();
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
            return true;
        }
        private void Init()
        {
            isInited = true;
            isChangingPhase = false;
            isRushing = false;
            rushingTime = 300f;
            this.model.AddUnitBuf(new Reid_UpgradeBuff(this));
            if (HardModeManager.Instance.isHardMode())
            {
                SefiraConversationController.Instance.UpdateConversation(Sprites.ReidSprite, Sprites.Reid_Color, LocalizeTextDataModel.instance.GetText("Reid_Desc_Hard"));
            }
            else
            {
                SefiraConversationController.Instance.UpdateConversation(Sprites.ReidSprite, Sprites.Reid_Color, LocalizeTextDataModel.instance.GetText("Reid_Desc"));
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
        public override void OnViewInit(CreatureUnit unit)
        {
            try
            {
                animscript = (ReidAnim)unit.animTarget;
                animscript.SetScript(this);
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        public override string GetName()
        {
            if (HardModeManager.Instance.isHardMode())
            {
                return LocalizeTextDataModel.instance.GetText("Reid_Name_Hard");
            }
            else
            {
                return LocalizeTextDataModel.instance.GetText("Reid_Name");
            }
        }
        public override string GetRiskLevel()
        {
            return "ALEPH";
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
        public ReidPhase phase;
        public ReidAnim animscript;
        public BossAmiya master;
        private bool isInited;
        public bool isChangingPhase;
        public bool isRushing;
        public float rushingTime;
        private AgentModel rushTarget;
        private float rushingPrepareTime;
        private bool isPreparedRushing;
        private MovableObjectNode tempAgentMovable;

        public float speed = 0f;
        public float rushTime = 0f;
    }
    public enum ReidPhase
    {
        Phase1,
        Phase2
    }
    public class ReidAttackCommand : CreatureCommand
    {
        public ReidAttackCommand(UnitModel unitModel)
        {
            this.target = unitModel;
        }
        public override void OnInit(CreatureModel creature, CreatureCommandQueue cmdQueue)
        {
            base.OnInit(creature, cmdQueue);
            model = creature;
            script = creature.script as Reid;
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
                if (Extension.IsInRange(actor, target, 4.5f) && attackTimer <= 0f)
                {
                    attackTimer = 1.8f;
                    movableNode.StopMoving();
                    script.animscript.Attack();
                    script.animscript.Event = new ReidEvent(NormalDamage);
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
                        this.target.TakeDamage(this.actor, new DamageInfo(RwbpType.R, 5, 10));
                        DamageParticleEffect.Invoker(this.target, RwbpType.R, this.actor);
                        if (target.GetUnitBufByName("Reid_Fire") == null)
                        {
                            this.target.AddUnitBuf(new Reid_Fire());
                        }
                    }
                    else
                    {
                        this.target.TakeDamage(this.actor, new DamageInfo(RwbpType.R, 5, 16));
                        DamageParticleEffect.Invoker(this.target, RwbpType.R, this.actor);
                        if (target.GetUnitBufByName("Reid_Fire") == null)
                        {
                            this.target.AddUnitBuf(new Reid_Fire());
                        }
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
        public Reid script;
        private float attackTimer;
    }
}
