using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BossAmiya
{
    /// <summary>
    /// 在10s内受到的伤害+15%
    /// </summary>
    public class LCP_SpecialBuff5 : UnitBuf
    {
        public override float OnTakeDamage(UnitModel attacker, DamageInfo damageInfo)
        {
            return 1.15f;
        }
        public override void FixedUpdate()
        {
            _time -= Time.deltaTime;
            if (_time <= 0)
            {
                Destroy();
            }
        }
        private float _time = 10f;
    }
}
