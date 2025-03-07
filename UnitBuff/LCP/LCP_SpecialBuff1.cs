using System;

namespace BossAmiya
{
    /// <summary>
    /// 最大生命值+25%，造成的伤害+30%，但受到的伤害+20%；移动速度-20%。
    /// 最大生命值逻辑在LCP
    /// </summary>
    public class LCP_SpecialBuff1 : UnitBuf
    {
        public override void Init(UnitModel model)
        {
            this.remainTime = float.MaxValue;
            base.Init(model);
        }
        public override float MovementScale()
        {
            return 0.8f;
        }
        public override float OnTakeDamage(UnitModel attacker, DamageInfo damageInfo)
        {
            return 1.2f;
        }

        public override float OnGiveDamageMult(UnitModel target, DamageInfo dmg)
        {
            return 1.3f;
        }
    }
}
