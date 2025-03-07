using NewGameMode;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossAmiya
{
    public class BossAmiya_Relic : MemeScriptBase
    {
        public override float AgentDamageTimes()
        {
            return 1.1f;
        }
        public override float CreatureMaxHPTimes()
        {
            return 1.1f;
        }
    }
}
