using Spine.Unity;
using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Spine.AnimationState;

namespace BossAmiya
{
    public class GoriaAnim : CreatureAnimScript
    {
        public void SetScript(Goria script)
        {
            this.script = script;
            animator = base.gameObject.GetComponent<SkeletonAnimation>();
            this.animator.AnimationState.Event += this.OnEvent;
            animator.AnimationState.SetAnimation(0, "Default", false);
            animator.AnimationState.SetAnimation(0, "Idle", true);
        }
        public void Default()
        {
            animator.AnimationState.SetAnimation(0, "Default", false);
            animator.AnimationState.SetAnimation(0, "Idle", true);
        }
        public new void Move()
        {
            animator.AnimationState.SetAnimation(0, "Move", true);
        }
        public void NormalAttack()
        {
            TrackEntry te1 = animator.AnimationState.SetAnimation(0, "Attack_Begin", false);
            te1.Complete += delegate
            {
                TrackEntry te2 = animator.AnimationState.SetAnimation(0, "Attack_Loop", false);
                te2.Complete += delegate
                {
                    TrackEntry te3 = animator.AnimationState.SetAnimation(0, "Attack_End", false);
                };
            };
        }
        public void SpecialAttack()
        {
            int loopCount = 0;
            TrackEntryDelegate loopHandler = null;
            loopHandler = delegate
            {
                loopCount++;
                if (loopCount < 2)
                {
                    TrackEntry newTe = animator.AnimationState.SetAnimation(0, "Skill_Loop", false);
                    newTe.Complete += loopHandler;
                }
                else
                {
                    TrackEntry te3 = animator.AnimationState.SetAnimation(0, "Skill_End", false);
                }
            };
            TrackEntry te1 = animator.AnimationState.SetAnimation(0, "Skill_Begin", false);
            te1.Complete += loopHandler;
        }
        public override bool HasDeadMotion()
        {
            return true;
        }
        public override void PlayDeadMotion()
        {
            animator.AnimationState.SetAnimation(0, "Die", false);
        }
        private void OnEvent(TrackEntry trackEntry, Event e)
        {
            if (Event != null)
            {
                Event(trackEntry, e);
            }
        }
        public Goria script;
        public new SkeletonAnimation animator;
        public GoriaEvent Event;
        public delegate void GoriaEvent(TrackEntry trackEntry, Event e);
    }
}
