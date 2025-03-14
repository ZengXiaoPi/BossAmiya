﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BossAmiya
{
    /// <summary>
    /// 在2s内移动速度-25%。
    /// </summary>
    public class LCP_SpecialBuff3 : UnitBuf
    {
        public override void Init(UnitModel model)
        {
            remainTime = 2;
            base.Init(model);
        }
        public override float MovementScale()
        {
            return 0.75f;
        }
    }
}
