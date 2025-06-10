using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossAmiya
{
    public class Reid_Slow : UnitBuf
    {
        public Reid_Slow(float slowTime)
        {
            this.remainTime = slowTime;
        }
        public override float MovementScale()
        {
            return 0f;
        }
    }
}
