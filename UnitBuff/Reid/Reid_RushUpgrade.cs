using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossAmiya
{
    public class Reid_RushUpgrade : UnitBuf
    {
        public override void Init(UnitModel model)
        {
            this.model = model;
            this.remainTime = 20f;
        }
        public override float OnGiveDamageMult(UnitModel target, DamageInfo dmg)
        {
            return 2.2f;
        }
        public override float MovementScale()
        {
            return 1.4f;
        }
    }
}
