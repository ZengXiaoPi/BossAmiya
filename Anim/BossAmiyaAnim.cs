using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;

namespace BossAmiya
{
    public class BossAmiyaAnim : CreatureAnimScript
    {
        public void SetScript(BossAmiya script)
        {
            this.script = script;
            this.animator = base.gameObject.GetComponent<SkeletonAnimation>();
            this.animator.AnimationState.Event += this.OnEvent;
            this.animator.AnimationState.SetAnimation(0, "A_Default", false);
            this.animator.AnimationState.SetAnimation(0, "A_Idle", true);
        }
        public void Default()
        {
            if (BossAmiya.amiyaPhase == 1)
            {
                this.animator.AnimationState.SetAnimation(0, "A_Default", false);
                this.animator.AnimationState.SetAnimation(0, "A_Idle", true);
            }
            else if (BossAmiya.amiyaPhase == 2)
            {
                this.animator.AnimationState.SetAnimation(0, "B_Default", false);
                this.animator.AnimationState.SetAnimation(0, "B_Idle", true);
            }
        }
        public void Attack()
        {
            this.animator.AnimationState.SetAnimation(0, "A_Attack", false);
        }
        public new void Move()
        {
            if (BossAmiya.amiyaPhase == 1)
            {
                this.animator.AnimationState.SetAnimation(0, "A_Move", true);
            }
            else if (BossAmiya.amiyaPhase == 2)
            {
                this.animator.AnimationState.SetAnimation(0, "B_Move", true);
            }
        }
        public void Phase1Attack()
        {
            this.animator.AnimationState.SetAnimation(0, "A_Attack", false);
        }
        public void IntoPhase2(List<ChildCreatureModel> creatureList, List<AgentModel> agentList)
        {
            TrackEntry te1 = this.animator.AnimationState.SetAnimation(0, "Revive_Begin", false);
            te1.Complete += delegate
            {
                TrackEntry te2 = this.animator.AnimationState.SetAnimation(0, "Revive_Loop", false);
                te2.Complete += delegate
                {
                    TrackEntry te3 = this.animator.AnimationState.SetAnimation(0, "Revive_End", false);
                    te3.Complete += delegate
                    {
                        try
                        {
                            foreach (ChildCreatureModel creature in creatureList)
                            {
                                creature.Suppressed();
                            }
                            foreach (AgentModel agent in agentList)
                            {
                                agent.Die();
                            }
                            BossAmiya.amiyaPhase = 2;
                            script.isSignOfContinuation = false;
                            MapNode centerNode = SefiraManager.instance.GetSefira(SefiraEnum.YESOD).sefiraPassage.centerNode;
                            script.model.GetMovableNode().SetCurrentNode(centerNode);
                            script.model.GetMovableNode().StopMoving();
                            this.Default();
                            PlaySpeedSettingUI.instance.SetNormalSpeedForcely();
                            PlaySpeedSettingUI.instance.UpdateButton();
                            script.AddBlockedEvent();
                            script.InitFUCKEffect();
                            Harmony_Patch.logger.Info("IntoPhase2");
                        }
                        catch (Exception e)
                        {
                            Harmony_Patch.logger.Error(e);
                        }
                    };
                };
            };
        }
        public void IntoPhase2()
        {
            TrackEntry te1 = this.animator.AnimationState.SetAnimation(0, "Revive_Begin", false);
            te1.Complete += delegate
            {
                TrackEntry te2 = this.animator.AnimationState.SetAnimation(0, "Revive_Loop", false);
                te2.Complete += delegate
                {
                    TrackEntry te3 = this.animator.AnimationState.SetAnimation(0, "Revive_End", false);
                    te3.Complete += delegate
                    {
                        try
                        {
                            BossAmiya.amiyaPhase = 2;
                            script.isSignOfContinuation = false;
                            MapNode centerNode = SefiraManager.instance.GetSefira(SefiraEnum.YESOD).sefiraPassage.centerNode;
                            script.model.GetMovableNode().SetCurrentNode(centerNode);
                            script.model.GetMovableNode().StopMoving();
                            this.Default();
                            PlaySpeedSettingUI.instance.SetNormalSpeedForcely();
                            PlaySpeedSettingUI.instance.UpdateButton();
                            script.AddBlockedEvent();
                            script.InitFUCKEffect();
                            Harmony_Patch.logger.Info("IntoPhase2");
                        }
                        catch (Exception e)
                        {
                            Harmony_Patch.logger.Error(e);
                        }
                    };
                };
            };
        }
        public void WillShock()
        {
            TrackEntry te1 = this.animator.AnimationState.SetAnimation(0, "A_Blink_Begin", false);
            te1.Complete += delegate
            {
                this.script.movable.SetCurrentNode(MapGraph.instance.GetCreatureRoamingPoint());
                foreach (AgentModel agent in AgentManager.instance.GetAgentList())
                {
                    agent.TakeDamage(this.script.model, new DamageInfo(RwbpType.N, script.WillShockDamage));
                }
                script.WillShockDamage += 2;
                TrackEntry te2 = this.animator.AnimationState.SetAnimation(0, "A_Blink_End", false);
                te2.Complete += delegate
                {
                    this.Default();
                    BossAmiya.isWillShocking = false;
                };
            };
        }
        public void AttackInPhase2()
        {
            TrackEntry te = this.animator.AnimationState.SetAnimation(0, "B_Attack", false);
            te.Complete += delegate
            {
                this.Default();
            };
        }
        public void FuckALLLLLLL()
        {
            this.animator.AnimationState.SetAnimation(0, "B_Skill_1", false);
        }
        public override bool HasDeadMotion()
        {
            return true;
        }
        public override void PlayDeadMotion()
        {
            TrackEntry te1 = this.animator.AnimationState.SetAnimation(0, "B_Die", false);
            te1.Complete += delegate
            {
                TrackEntry te2 = this.animator.AnimationState.SetAnimation(0, "C_Start", false);
                te2.Complete += delegate
                {
                    this.animator.AnimationState.SetAnimation(0, "C_Die", false);
                };
            };
        }
        private void OnEvent(TrackEntry trackEntry, Event e)
        {
            if (Event != null)
            {
                Event(trackEntry, e);
            }
        }
        public BossAmiya script;
        public new SkeletonAnimation animator;
        public BossAmiyaEvent Event;
        public delegate void BossAmiyaEvent(TrackEntry trackEntry, Event e);
    }
}
