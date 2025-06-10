using UnityEngine;

namespace BossAmiya
{
    public class Amiya_MentalCrash : UnitBuf
    {
        public override void Init(UnitModel model)
        {
            base.Init(model);
            this.remainTime = float.MaxValue;
            _time = 60f;
        }
        public override void FixedUpdate()
        {
            _time -= Time.deltaTime;
        }
        public override float OnGiveDamageMult(UnitModel target, DamageInfo dmg)
        {
            if (_time >= 0f)
            {
                return 1.5f;
            }
            return 1f;
        }
        private float _time = 0;
    }
}
