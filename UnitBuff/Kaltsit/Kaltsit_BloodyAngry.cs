using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossAmiya
{
    public class Kaltsit_BloodyAngry : UnitBuf
    {
        public override void Init(UnitModel model)
        {
            remainTime = float.MaxValue;
            base.Init(model);
        }
        public override float OnTakeDamage(UnitModel attacker, DamageInfo damageInfo)
        {
            if (HardModeManager.Instance.isHardMode())
            {
                return 1f;
            }
            return 2.25f;
        }
        public override float OnGiveDamageMult(UnitModel target, DamageInfo dmg)
        {
            return 2.25f;
        }
    }
}
