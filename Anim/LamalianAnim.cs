using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossAmiya
{
    public class LamalianAnim : CreatureAnimScript
    {
        public void SetScript(Lamalian script)
        {
            this.script = script;
            animator = base.gameObject.GetComponent<SkeletonAnimation>();
            this.animator.transform.localScale *= 1f;
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
        public Lamalian script;
        public new SkeletonAnimation animator;
        public LamalianEvent Event;
        public delegate void LamalianEvent(TrackEntry trackEntry, Event e);
    }
}
