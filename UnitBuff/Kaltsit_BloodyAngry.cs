using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossAmiya
{
    public class Kaltsit_BloodyAngry : UnitBuf
    {
        public override float OnTakeDamage(UnitModel attacker, DamageInfo damageInfo)
        {
            return 2.25f;
        }
        public override float GetDamageFactor(UnitModel target, DamageInfo info)
        {
            return 2.25f;
        }
    }
}
