using Spine;
using Spine.Unity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static BossAmiya.KaltsitAnim;

namespace BossAmiya
{
    public class Mon2trAnim : CreatureAnimScript
    {
        public void SetScript(Mon2tr script)
        {
            this.script = script;
            animator = base.gameObject.GetComponent<SkeletonAnimation>();
            this.animator.transform.localScale /= 3.5f;
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
            var motion = animator.AnimationState.SetAnimation(0, "Die", false);
            motion.Complete += (trackEntry) =>
            {
                List<UnitModel> allUnits = script.GetNearEnemy();
                List<UnitModel> luckyUnits = new List<UnitModel>();
                foreach (UnitModel unit in allUnits)
                {
                    if (script.IsHostile(unit.GetMovableNode()) && IsInRange(script.model, unit, 4f))
                    {
                        luckyUnits.Add(unit);
                    }
                }
                foreach (UnitModel unit in luckyUnits)
                {
                    unit.TakeDamage(script.model, new DamageInfo(RwbpType.R, 200));
                }
            };
        }
        private bool IsInRange(UnitModel owner, UnitModel target, float range)
        {
            return MovableObjectNode.GetDistance(owner.GetMovableNode(), target.GetMovableNode()) - owner.radius - target.radius <= range;
        }
        private void OnEvent(TrackEntry trackEntry, Event e)
        {
            if (Event != null)
            {
                Event(trackEntry, e);
            }
        }
        public Mon2tr script;
        public Mon2trEvent Event;
        public new SkeletonAnimation animator;
        public delegate void Mon2trEvent(TrackEntry trackEntry, Event e);
    }
}
