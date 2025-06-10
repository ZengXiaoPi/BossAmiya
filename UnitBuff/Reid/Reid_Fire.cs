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
        public override void Init(UnitModel model)
        {
            base.Init(model);
            this.remainTime = 5f;
        }
        public override void FixedUpdate()
        {
            _timer += Time.deltaTime;
            if (_timer >= 0.4f)
            {
                _timer = 0f;
                if (!HardModeManager.Instance.isHardMode())
                {
                    this.model.hp -= 1;
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
        private float _timer;
    }
}
