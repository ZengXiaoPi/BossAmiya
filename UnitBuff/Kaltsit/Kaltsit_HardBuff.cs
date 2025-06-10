using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BossAmiya
{
    public class Kaltsit_HardBuff : UnitBuf
    {
        public Kaltsit_HardBuff(Kaltsit script)
        {
            this.script = script;
        }
        public override void Init(UnitModel model)
        {
            base.Init(model);
            this.remainTime = float.MaxValue;
        }
        public override float MovementScale()
        {
            return 1.3f;
        }
        public override float OnTakeDamage(UnitModel attacker, DamageInfo damageInfo)
        {
            if (script.Mon2trModel != null)
            {
                if (script.Mon2trModel.hp > 0)
                {
                    DamageInfo newInfo = damageInfo.Copy();
                    newInfo.max *= 0.25f;
                    newInfo.min *= 0.25f;
                    script.Mon2trModel.TakeDamage(newInfo);
                    return 0f;
                }
            }
            return 1f;
        }
        public override void FixedUpdate()
        {
            _timer += Time.deltaTime;
            if (_timer >= 3f)
            {
                script.model.hp += script.model.maxHp * 0.02f;
                if (script.model.hp > script.model.maxHp)
                {
                    script.model.hp = script.model.maxHp;
                }
                if (script.Mon2trModel != null)
                {
                    if (script.Mon2trModel.hp > 0)
                    {
                        script.Mon2trModel.hp += script.model.maxHp * 0.02f;
                        if (script.Mon2trModel.GetMovableNode().GetPassage() == script.model.GetMovableNode().GetPassage())
                        {
                            script.Mon2trModel.hp += script.Mon2trModel.maxHp * 0.018f;
                        }
                        if (script.Mon2trModel.hp > script.Mon2trModel.maxHp)
                        {
                            script.Mon2trModel.hp = script.Mon2trModel.maxHp;
                        }
                    }
                }
                _timer = 0f;
            }
        }
        private float _timer = 0f;
        private Kaltsit script;
    }
}
