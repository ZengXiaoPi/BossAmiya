using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BossAmiya
{
    /// <summary>
    /// 在2s内移动速度-40%
    /// </summary>
    public class LCP_SpecialBuff4 : UnitBuf
    {
        public override float MovementScale()
        {
            return 0.6f;
        }
        public override void FixedUpdate()
        {
            _time -= Time.deltaTime;
            if (_time <= 0)
            {
                Destroy();
            }
        }
        private float _time = 2f;
    }
}
