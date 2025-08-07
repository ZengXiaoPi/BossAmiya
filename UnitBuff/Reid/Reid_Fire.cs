using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BossAmiya
{
    public class Reid_Fire :UnitBuf
    {
        public Reid_Fire(UnitModel actor)
        {
            this.actor = actor;
        }
        public override void Init(UnitModel model)
        {
            base.Init(model);
            if (!HardModeManager.Instance.isHardMode())
            {
                this.remainTime = 5f;
            }
            else
            {
                this.remainTime = 20f;
            }
        }
        public override void FixedUpdate()
        {
            _timer += Time.deltaTime;
            if (_timer >= 0.6f)
            {
                _timer = 0f;
                if (!HardModeManager.Instance.isHardMode())
                {
                    this.model.TakeDamage(new DamageInfo(RwbpType.R, 3, 3));
                    DamageParticleEffect.Invoker(this.model, RwbpType.R, actor);
                }
                else
                {
                    this.model.hp -= 2;
                }
                if (model.hp <= 0 && model is WorkerModel)
                {
                    (model as WorkerModel).OnDie();
                }
            }
        }
        private UnitModel actor;
        private float _timer;
    }
}
