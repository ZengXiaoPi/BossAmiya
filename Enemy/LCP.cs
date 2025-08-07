using Spine;
using System;
using System.Collections.Generic;
using UnityEngine;
using static BossAmiya.LCPAnim;

namespace BossAmiya
{
    public class LCP : CreatureBase
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
                animscript = (LCPAnim)unit.animTarget;
                animscript.SetScript(this);
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        public override bool OnAfterSuppressed()
        {
            isInited = false;
            this.master.childs.Remove(this.Model);
            this.animscript.PlayDeadMotion();
            this.master.summonGoriaTimer = 0f;
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
        public override string GetRiskLevel()
        {
            return "ALEPH";
        }
        private void Init()
        {
            LCPPhase = 1;
            GreatAttackMP = 0;
            SweepingCeremonyMP = 0;
            isChangingPhase = false;
            isAttacking = false;
            foreach (AgentModel unit in AgentManager.instance.GetAgentList())
            {
                if (unit == null) continue;
                if (unit.IsDead()) continue;
                if (Extension.HasBuff(unit, typeof(LCP_SpecialBuff1))) continue;
                unit.AddUnitBuf(new LCP_SpecialBuff1());
                unit.AddUnitBuf(new UnitStatBuf(float.MaxValue)
                {
                    maxHp = (int)Math.Round(unit.maxHp * 0.25f)
                });
            }
            this.model.AddUnitBuf(new LCP_SpecialBuff2());
            SefiraConversationController.Instance.UpdateConversation(Sprites.LCPSprite, Sprites.LCP_Color, LocalizeTextDataModel.instance.GetText("LCP_Desc"));
            isInited = true;
        }
        public override void UniqueEscape()
        {
            try
            {
                if (this.model.state == CreatureState.ESCAPE && !this.isInited)
                {
                    Init();
                }
                this.model.CheckNearWorkerEncounting();
                List<UnitModel> nearEnemy = this.GetNearEnemy();
                if (LCPPhase == 1 && this.model.hp <= this.model.maxHp * 0.5f && !isChangingPhase)
                {
                    isChangingPhase = true;
                    isAttacking = false;
                    model.commandQueue.Clear();
                    this.movable.StopMoving();
                    animscript.ChangePhase();
                }
                if (nearEnemy.Count > 0 && !(this.model.GetCurrentCommand() is LCPAttackCommand) && !isAttacking && !isChangingPhase)
                {
                    this.model.commandQueue.Clear();
                    this.model.commandQueue.SetAgentCommand(new LCPAttackCommand(nearEnemy));
                }
                else
                {
                    if (!(this.model.GetCurrentCommand() is MoveCreatureCommand) && !(this.model.GetCurrentCommand() is LCPAttackCommand) && !isAttacking && !isChangingPhase)
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
        public void SetParentAbnormality(BossAmiya master)
        {
            this.master = master;
        }

        public override string GetName()
        {
            return LocalizeTextDataModel.instance.GetText("LCP_Name");
        }
        private void MakeMovement()
        {
            this.model.ClearCommand();
            this.animscript.Move();
            this.model.MoveToNode(MapGraph.instance.GetCreatureRoamingPoint());
        }
        public int LCPPhase = 0;
        public int GreatAttackMP = 0;
        public int SweepingCeremonyMP = 0;
        public bool isAttacking = false;
        public bool isChangingPhase = false;
        public LCPAnim animscript;
        public BossAmiya master;
        private bool isInited = false;
    }
    public class LCPAttackCommand : CreatureCommand
    {
        public LCPAttackCommand(List<UnitModel> targets)
        {
            this.targets = targets;
        }
        public override void OnInit(CreatureModel creature, CreatureCommandQueue cmdQueue)
        {
            base.OnInit(creature, cmdQueue);
            model = creature;
            script = creature.script as LCP;
        }
        public override void Execute()
        {
            try
            {
                if (targets.Count <= 0)
                {
                    Finish();
                    return;
                }
                if (actor.state != CreatureState.ESCAPE)
                {
                    Finish();
                    return;
                }
                else if (script.GreatAttackMP < 4 && (script.LCPPhase != 2 || (script.LCPPhase == 2 && ((script.SweepingCeremonyMP < 9 && !HardModeManager.Instance.isHardMode()) || (script.SweepingCeremonyMP < 4 && HardModeManager.Instance.isHardMode())))) && !script.isAttacking)
                {
                    PassageObjectModel passage = targets[0].GetMovableNode().GetPassage();
                    if (!targets[0].IsAttackTargetable() || !this.actor.IsHostile(targets[0]))
                    {
                        Finish();
                    }
                    if (passage == null || passage != this.actor.GetMovableNode().GetPassage())
                    {
                        Finish();
                    }
                    else
                    {
                        this.MoveToNormalAttack();
                    }
                }
                else if (script.LCPPhase == 2 && !script.isAttacking && ((script.SweepingCeremonyMP >= 9 && !HardModeManager.Instance.isHardMode()) || (script.SweepingCeremonyMP >= 3 && HardModeManager.Instance.isHardMode())))
                {
                    this.CeremonyAttack();
                }
                else if (script.GreatAttackMP >= 3 && !script.isAttacking)
                {
                    PassageObjectModel passage = targets[0].GetMovableNode().GetPassage();
                    if (!targets[0].IsAttackTargetable() || !this.actor.IsHostile(targets[0]))
                    {
                        Finish();
                    }
                    if (passage == null || passage != this.actor.GetMovableNode().GetPassage())
                    {
                        Finish();
                    }
                    else
                    {
                        this.MoveToGreatAttack();
                    }
                }
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        private void MoveToNormalAttack()
        {
            try
            {
                attackTimer -= Time.deltaTime;
                MovableObjectNode movableNode = actor.GetMovableNode();
                if (Extension.IsInRange(actor, targets[0], 3f) && attackTimer <= 0f)
                {
                    attackTimer = 0.5f;
                    script.isAttacking = true;
                    movableNode.StopMoving();
                    script.animscript.Attack();
                    script.animscript.Event = new LCPEvent(NormalDamage);
                }
                else
                {
                    if (!movableNode.IsMoving() && this.attackTimer <= 0f)
                    {
                        movableNode.MoveToMovableNode(targets[0].GetMovableNode(), false);
                        this.script.animscript.Move();
                    }
                }
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        private void MoveToGreatAttack()
        {
            try
            {
                MovableObjectNode movableNode = actor.GetMovableNode();
                if (Extension.IsInRange(actor, targets[0], 3.5f))
                {
                    attackTimer = 0.5f;
                    movableNode.StopMoving();
                    script.isAttacking = true;
                    script.animscript.GreatAttack();
                    script.animscript.Event = new LCPEvent(GreatAttackDamage);
                }
                else
                {
                    if (!movableNode.IsMoving() && this.attackTimer <= 0f)
                    {
                        movableNode.MoveToMovableNode(targets[0].GetMovableNode(), false);
                        this.script.animscript.Move();
                    }
                }
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        private void CeremonyAttack()
        {
            try
            {
                AngelaConversationUI.instance.AddAngelaMessage(LocalizeTextDataModel.instance.GetText("Angela_Message_LCPPrepare"));
                MovableObjectNode movableNode = actor.GetMovableNode();
                script.isAttacking = true;
                SweepingShotCount = 0;
                movableNode.StopMoving();
                if (!HardModeManager.Instance.isHardMode())
                {
                    script.model.AddUnitBuf(new LCP_SpecialBuff7());
                }
                script.animscript.SweepingCeremony();
                script.animscript.Event = new LCPEvent(SweepingCeremonyDamage);
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
                if (e.Data.Name == "OnAttack" && Extension.IsInRange(actor, targets[0], 3f))
                {
                    if (script.LCPPhase == 1)
                    {
                        if (!Extension.HasBuff(targets[0], typeof(LCP_SpecialBuff3)))
                        {
                            targets[0].AddUnitBuf(new LCP_SpecialBuff3());
                        }
                        targets[0].TakeDamage(this.actor, new DamageInfo(RwbpType.R, 10, 15));
                        script.GreatAttackMP++;
                    }
                    else
                    {
                        if (!Extension.HasBuff(targets[0], typeof(LCP_SpecialBuff4)))
                        {
                            targets[0].AddUnitBuf(new LCP_SpecialBuff4());
                        }
                        targets[0].TakeDamage(this.actor, new DamageInfo(RwbpType.R, 12, 25));
                        script.GreatAttackMP++;
                        script.SweepingCeremonyMP++;
                    }
                    DamageParticleEffect.Invoker(targets[0], RwbpType.R, this.actor);
                }
                else
                {
                    script.isAttacking = false;
                    Finish();
                }
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        public void GreatAttackDamage(TrackEntry trackEntry, Event e)
        {
            try
            {
                if (e.Data.Name == "OnAttack" && Extension.IsInRange(actor, targets[0], 3.5f))
                {
                    if (script.LCPPhase == 1)
                    {
                        if (!Extension.HasBuff(targets[0], typeof(LCP_SpecialBuff5)))
                        {
                            targets[0].AddUnitBuf(new LCP_SpecialBuff5());
                        }
                        targets[0].TakeDamage(this.actor, new DamageInfo(RwbpType.P, 8, 12));
                        script.GreatAttackMP = 0;
                    }
                    else
                    {
                        if (!Extension.HasBuff(targets[0], typeof(LCP_SpecialBuff6)))
                        {
                            targets[0].AddUnitBuf(new LCP_SpecialBuff6());
                        }
                        targets[0].TakeDamage(this.actor, new DamageInfo(RwbpType.P, 10, 16));
                        script.GreatAttackMP = 0;
                        script.SweepingCeremonyMP++;
                    }
                    DamageParticleEffect.Invoker(targets[0], RwbpType.P, this.actor);
                }
                else
                {
                    script.isAttacking = false;
                    Finish();
                }
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        public void SweepingCeremonyDamage(TrackEntry trackEntry, Event e)
        {
            try
            {
                if (e.Data.Name == "OnAttack")
                {
                    if (SweepingShotCount < 120 && targets.Count > 0)
                    {
                        List<UnitModel> targets_temp = script.GetNearEnemy();
                        var target = targets_temp[UnityEngine.Random.Range(0, targets_temp.Count)];
                        PassageObjectModel passage = target.GetMovableNode().GetPassage();
                        if (target.IsAttackTargetable() && this.actor.IsHostile(target) && passage != null && passage == this.actor.GetMovableNode().GetPassage() && target.hp > 0f)
                        {
                            MovableObjectNode movableNode = target.GetMovableNode();
                            if (!HardModeManager.Instance.isHardMode())
                            {
                                target.TakeDamage(this.actor, new DamageInfo(RwbpType.R, 6, 8));
                                DamageParticleEffect.Invoker(target, RwbpType.R, this.actor);
                            }
                            else
                            {
                                Harmony_Patch.RealDamage_TempList.Add(target);
                                target.TakeDamage(this.actor, new DamageInfo(RwbpType.N, 4, 6));
                                DamageParticleEffect.Invoker(target, RwbpType.R, this.actor);
                            }
                        }
                        SweepingShotCount++;
                    }
                    else
                    {
                        script.animscript.EndSweeepingCeremony();
                        Finish();
                    }
                }
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        public CreatureModel model;
        public LCP script;
        private List<UnitModel> targets;
        private float attackTimer = 0f;
        public int SweepingShotCount = 0;
    }
}
