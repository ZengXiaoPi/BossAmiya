using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossAmiya
{
    public class BossAmiyaAnim : CreatureAnimScript
    {
        public void SetScript(BossAmiya script)
        {
            this.script = script;
            this.animator = base.gameObject.GetComponent<SkeletonAnimation>();
            this.animator.AnimationState.SetAnimation(0, "A_Default", false);
            this.animator.AnimationState.SetAnimation(0, "A_Idle", true);
        }
        public void Default()
        {
            this.animator.AnimationState.SetAnimation(0, "A_Default", false);
            this.animator.AnimationState.SetAnimation(0, "A_Idle", true);
        }
        public void Attack()
        {
            this.animator.AnimationState.SetAnimation(0, "A_Attack", false);
        }
        public new void Move()
        {
            this.animator.AnimationState.SetAnimation(0, "A_Move", true);
        }
        public BossAmiya script;
        public new SkeletonAnimation animator;
    }
}
