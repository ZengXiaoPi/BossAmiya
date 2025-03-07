using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BossAmiya
{
    /// <summary>
    /// 在15s内受到的伤害+20%，且在3s内移动速度-40%。
    /// </summary>
    public class LCP_SpecialBuff6 : UnitBuf
    {
        public override void Init(UnitModel model)
        {
            remainTime = 15f;
            base.Init(model);
        }
        public override float OnTakeDamage(UnitModel attacker, DamageInfo damageInfo)
        {
            return 1.2f;
        }
        public override float MovementScale()
        {
            return 0.6f;
        }
    }
}
