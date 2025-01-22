using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace BossAmiya
{
    public class KaltsitAnim : CreatureAnimScript
    {
        public void SetScript(Kaltsit script)
        {
            this.script = script;
            animator = base.gameObject.GetComponent<SkeletonAnimation>();
            this.animator.transform.localScale /= 1.3f;
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
        public void Attack()
        {
            animator.AnimationState.SetAnimation(0, "Attack", false);
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
        public Kaltsit script;
        public new SkeletonAnimation animator;
        public KaltsitEvent Event;
        public delegate void KaltsitEvent (TrackEntry trackEntry, Event e);
    }
}
