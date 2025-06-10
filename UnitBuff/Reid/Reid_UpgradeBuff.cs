using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BossAmiya
{
    public class Reid_UpgradeBuff : UnitBuf
    {
        public Reid_UpgradeBuff(Reid script)
        {
            this.script = script;
        }
        public override void Init(UnitModel model)
        {
            this.model = model;
            this.remainTime = float.MaxValue;
        }
        public override float OnGiveDamageMult(UnitModel target, DamageInfo dmg)
        {
            float multiDamage = 1f;
            if (!HardModeManager.Instance.isHardMode())
            {
                multiDamage = (model.maxHp - model.hp) / model.maxHp * 3f + 1f;
            }
            else
            {
                multiDamage = (model.maxHp - model.hp) / model.maxHp * 2f * 3f + 1f;
                if (multiDamage > 4f)
                {
                    multiDamage = 4f;
                }
            }
            if (script.phase == ReidPhase.Phase2 && model.hp / model.maxHp < 0.5f)
            {
                multiDamage += 1f;
            }
            return multiDamage;
        }
        public override float OnTakeDamage(UnitModel attacker, DamageInfo damageInfo)
        {
            if (script.phase == ReidPhase.Phase2)
            {
                return base.OnTakeDamage(attacker, damageInfo);
            }
            if (script.isChangingPhase && script.phase == ReidPhase.Phase1)
            {
                return 0f;
            }
            if (model.hp <= damageInfo.min && script.phase == ReidPhase.Phase1)
            {
                model.hp = 1f;
                script.isChangingPhase = true;
                (this.model as CreatureModel).commandQueue.Clear();
                script.animscript.IntoChangingPhase();
                return 0f;
            }
            return base.OnTakeDamage(attacker, damageInfo);
        }
        private Reid script;
    }
}
