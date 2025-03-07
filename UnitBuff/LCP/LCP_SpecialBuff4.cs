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
        public override void Init(UnitModel model)
        {
            remainTime = 2;
            base.Init(model);
        }
        public override float MovementScale()
        {
            return 0.6f;
        }
    }
}
