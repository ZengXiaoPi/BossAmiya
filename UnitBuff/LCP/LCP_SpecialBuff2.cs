using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossAmiya
{
    /// <summary>
    /// 受到伤害时，如果来源距自己较近（2.5距离内），则此次伤害降低50%。
    /// </summary>
    public class LCP_SpecialBuff2 : UnitBuf
    {
        public override void Init(UnitModel model)
        {
            remainTime = float.MaxValue;
            base.Init(model);
        }
        public override float OnTakeDamage(UnitModel attacker, DamageInfo damageInfo)
        {
            if (this.model == null || attacker == null) return base.OnTakeDamage(attacker, damageInfo);
            if (!HardModeManager.Instance.isHardMode())
            {
                if (Extension.IsInRange(this.model, attacker, 2.5f)) return 0.5f;
            }
            else
            {
                if (Extension.IsInRange(this.model, attacker, 4f)) return 0.2f;
            }
            return base.OnTakeDamage(attacker, damageInfo);
        }
    }
}
