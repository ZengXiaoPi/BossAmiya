using Spine.Unity;
using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossAmiya
{
    public class KLAnim : CreatureAnimScript
    {
        public void SetScript(KL script)
        {
            this.script = script;
            animator = base.gameObject.GetComponent<SkeletonAnimation>();
            this.animator.transform.localScale /= 3.4f;
            this.animator.AnimationState.Event += this.OnEvent;
            animator.AnimationState.SetAnimation(0, "Default", false);
            animator.AnimationState.SetAnimation(0, "B_Idle", true);
        }
        public void Default()
        {
            animator.AnimationState.SetAnimation(0, "Default", false);
            animator.AnimationState.SetAnimation(0, "B_Idle", true);
        }
        public new void Move()
        {
            animator.AnimationState.SetAnimation(0, "B_Move", true);
        }
        public void Attack()
        {
            animator.AnimationState.SetAnimation(0, "B_Attack", false);
        }
        public void SpecialAttack()
        {
            animator.AnimationState.SetAnimation(0, "B_Skill_2", false);
        }
        public override bool HasDeadMotion()
        {
            return true;
        }
        public override void PlayDeadMotion()
        {
            animator.AnimationState.SetAnimation(0, "B_Die", false);
        }
        private void OnEvent(TrackEntry trackEntry, Event e)
        {
            if (Event != null)
            {
                Event(trackEntry, e);
            }
        }
        public KL script;
        public new SkeletonAnimation animator;
        public KLEvent Event;
        public delegate void KLEvent(TrackEntry trackEntry, Event e);
    }
}
