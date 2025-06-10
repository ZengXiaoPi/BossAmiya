using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BossAmiya
{
    public class Amiya_RecoverMP : UnitBuf
    {
        public Amiya_RecoverMP(BossAmiya_weapon weapon)
        {
            this.weapon = weapon;
        }
        public override void Init(UnitModel model)
        {
            this.model = model;
            agent = model as AgentModel;
            this.remainTime = float.MaxValue;
        }
        public override void FixedUpdate()
        {
            if (model.mental >= model.maxMental)
            {
                agent.GetWorkerUnit().animChanger.state.SetAnimation(0, "InRecover_End", false);
                SefiraManager.instance.GetSefira(agent.currentSefira).OnAgentReturnControll();
                weapon.isRecoverying = false;
                this.Destroy();
            }
            _time += Time.deltaTime;
            if (_time >= 1)
            {
                if (model.mental > model.maxMental * 0.95f)
                {
                    model.mental = model.maxMental;
                }
                model.mental += model.maxMental * 0.05f;
                _time = 0;
            }
        }
        public override float OnTakeDamage(UnitModel attacker, DamageInfo damageInfo)
        {
            return 1.5f;
        }
        private float _time = 0;
        private AgentModel agent;
        private BossAmiya_weapon weapon;
    }
}
