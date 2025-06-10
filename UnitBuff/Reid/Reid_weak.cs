using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossAmiya
{
    public class Reid_weak : UnitBuf
    {
        public Reid_weak(float weakValue, float remainTime)
        {
            this.weakValue = weakValue;
            this.remainTime = remainTime;
        }
        public override float OnTakeDamage(UnitModel attacker, DamageInfo damageInfo)
        {
            return 1f + weakValue;
        }
        private float weakValue;
    }
}
