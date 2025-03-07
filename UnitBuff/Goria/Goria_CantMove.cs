using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BossAmiya
{
    public class Goria_CantMove : UnitBuf
    {
        public override void Init(UnitModel model)
        {
            remainTime = 1.5f;
            base.Init(model);
        }
        public override float MovementScale()
        {
            return 0f;
        }
    }
}
