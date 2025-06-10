using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BossAmiya
{
    public class Mon2tr_HardBuff : UnitBuf
    {
        public Mon2tr_HardBuff(Mon2tr script)
        {
            this.script = script;
        }
        public override void Init(UnitModel model)
        {
            base.Init(model);
            this.remainTime = float.MaxValue;
        }
        public override void FixedUpdate()
        {
            _timer += Time.deltaTime;
            if (_timer >= 1f)
            {
                script.model.hp -= script.model.maxHp * 0.005f;
                _timer = 0f;
            }
        }
        public override float MovementScale()
        {
            return 1.3f;
        }
        private float _timer;
        private Mon2tr script;
    }
}
