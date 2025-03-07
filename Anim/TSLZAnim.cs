using Spine.Unity;
using Spine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossAmiya
{
    public class TSLZAnim : CreatureAnimScript
    {
        public void SetScript(TSLZ script)
        {
            this.script = script;
            animator = base.gameObject.GetComponent<SkeletonAnimation>();
            this.animator.transform.localScale *= 1.3f;
            this.animator.AnimationState.Event += this.OnEvent;
            animator.AnimationState.SetAnimation(0, "Default", false);
            animator.AnimationState.SetAnimation(0, "Start", false);
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
        public TSLZ script;
        public new SkeletonAnimation animator;
        public TSLZEvent Event;
        public delegate void TSLZEvent(TrackEntry trackEntry, Event e);
    }
}
