using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossAmiya
{
    /// <summary>
    /// 受到的伤害+200%
    /// </summary>
    public class LCP_SpecialBuff7 : UnitBuf
    {
        public override float OnTakeDamage(UnitModel attacker, DamageInfo damageInfo)
        {
            return 3f;
        }
    }
}
