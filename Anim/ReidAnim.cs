using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossAmiya
{
    public class ReidAnim : CreatureAnimScript
    {
        public void SetScript(Reid script)
        {
            this.script = script;
            animator = base.gameObject.GetComponent<SkeletonAnimation>();
            this.animator.transform.localScale /= 3.7f;
            this.animator.AnimationState.Event += this.OnEvent;
            animator.AnimationState.SetAnimation(0, "Default", false);
            animator.AnimationState.SetAnimation(0, "Idle", true);
        }
        public void Default()
        {
            animator.AnimationState.SetAnimation(0, "Default", false);
            animator.AnimationState.SetAnimation(0, "Idle", true);
        }
        public void IntoChangingPhase()
        {
            var te1 = animator.AnimationState.SetAnimation(0, "Revive_Begin", false);
            te1.Complete += delegate
            {
                animator.AnimationState.SetAnimation(0, "Revive_Loop", true);
            };
        }
        public void OutOfChangingPhase()
        {
            var te1 = animator.AnimationState.SetAnimation(0, "Revive_End", false);
            te1.Complete += delegate
            {
                Default();
            };
        }
        public new void Move()
        {
            animator.AnimationState.SetAnimation(0, "Move", true);
        }
        public void Attack()
        {
            animator.AnimationState.SetAnimation(0, "Attack", false);
        }
        public void StartRushing()
        {
            animator.AnimationState.SetAnimation(0, "Skill_Begin", false);
        }
        public void RushingMoving()
        {
            animator.AnimationState.SetAnimation(0, "Skill_Loop", true);
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
        public Reid script;
        public ReidEvent Event;
        public new SkeletonAnimation animator;
        public delegate void ReidEvent(TrackEntry trackEntry, Event e);
    }
}
