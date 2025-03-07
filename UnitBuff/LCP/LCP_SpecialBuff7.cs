using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossAmiya
{
    /// <summary>
    /// 受到的伤害+50%
    /// </summary>
    public class LCP_SpecialBuff7 : UnitBuf
    {
        public override void Init(UnitModel model)
        {
            remainTime = float.MaxValue;
            base.Init(model);
        }
        public override float OnTakeDamage(UnitModel attacker, DamageInfo damageInfo)
        {
            return 1.5f;
        }
    }
}
