using Spine;
using Spine.Unity;

namespace BossAmiya
{
    public class LCPAnim : CreatureAnimScript
    {
        public void SetScript(LCP script)
        {
            this.script = script;
            animator = base.gameObject.GetComponent<SkeletonAnimation>();
            this.animator.transform.localScale /= 1.5f;
            this.animator.AnimationState.Event += this.OnEvent;
            animator.AnimationState.SetAnimation(0, "C1_Default", false);
            animator.AnimationState.SetAnimation(0, "C1_Idle", true);
        }
        public void Default()
        {
            if (script.LCPPhase == 1)
            {
                animator.AnimationState.SetAnimation(0, "C1_Default", false);
                animator.AnimationState.SetAnimation(0, "C1_Idle", true);
            }
            else
            {
                animator.AnimationState.SetAnimation(0, "C2_Idle", true);
            }
        }
        public override bool HasDeadMotion()
        {
            return true;
        }
        public override void PlayDeadMotion()
        {
            if (script.LCPPhase == 1) animator.AnimationState.SetAnimation(0, "C1_Die", false);
            else animator.AnimationState.SetAnimation(0, "C2_Die", false);
        }
        public void ChangePhase()
        {
            if (script.LCPPhase == 1)
            {
                TrackEntry te1 = animator.AnimationState.SetAnimation(0, "C1_Die", false);
                te1.Complete += (trackEntry) =>
                {
                    TrackEntry te2 = animator.AnimationState.SetAnimation(0, "C1_Die_Loop", false);
                    te2.Complete += (trackEntry2) =>
                    {
                        TrackEntry te3 = animator.AnimationState.SetAnimation(0, "C1_Die_End", false);
                        te3.Complete += (trackEntry3) =>
                        {
                            script.LCPPhase = 2;
                            script.isChangingPhase = false;
                            if (HardModeManager.Instance.isHardMode())
                            {
                                script.model.hp = script.model.maxHp;
                            }
                            this.Default();
                        };
                    };
                };
            }
        }
        public new void Move()
        {
            if (script.LCPPhase == 1) animator.AnimationState.SetAnimation(0, "C1_Move", true);
            else animator.AnimationState.SetAnimation(0, "C2_Move", true);
        }
        public void GreatAttack()
        {
            TrackEntry te;
            if (script.LCPPhase == 1)
            {
                te = animator.AnimationState.SetAnimation(0, "C1_Attack", false);
            }
            else
            {
                te = animator.AnimationState.SetAnimation(0, "C2_Attack_2", false);
            }
            te.Complete += (trackEntry) =>
            {
                script.isAttacking = false;
            };
        }
        public void Attack()
        {
            TrackEntry te;
            if (script.LCPPhase == 1) te = animator.AnimationState.SetAnimation(0, "C1_Skill_1", false);
            else te = animator.AnimationState.SetAnimation(0, "C2_Skill_1", false);
            te.Complete += (trackEntry) =>
            {
                script.isAttacking = false;
            };
        }
        public void SweepingCeremony()
        {
            if (script.LCPPhase == 1) return;
            TrackEntry te1 = animator.AnimationState.SetAnimation(0, "C2_Skill_2_Begin", false);
            te1.Complete += (trackEntry) =>
            {
                TrackEntry te2 = animator.AnimationState.SetAnimation(0, "C2_Skill_2_Idle", false);
                te2.Complete += (trackEntry2) =>
                {
                    TrackEntry te3 = animator.AnimationState.SetAnimation(0, "C2_Skill_2_Idle", false);
                    te3.Complete += (trackEntry3) =>
                    {
                        animator.AnimationState.SetAnimation(0, "C2_Skill_2_Loop", true);
                    };
                };
            };
        }
        public void EndSweeepingCeremony()
        {
            TrackEntry te = animator.AnimationState.SetAnimation(0, "C2_Skill_2_End", false);
            te.Complete += (trackEntry) =>
            {
                script.isAttacking = false;
                script.SweepingCeremonyMP = 0;
                Extension.RemoveBuff(script.model, typeof(LCP_SpecialBuff7));
                this.Default();
            };
        }
        private void OnEvent(TrackEntry trackEntry, Event e)
        {
            if (Event != null)
            {
                Event(trackEntry, e);
            }
        }
        public LCP script;
        public LCPEvent Event;
        public new SkeletonAnimation animator;
        public delegate void LCPEvent(TrackEntry trackEntry, Event e);
    }
}
