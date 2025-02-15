using UnityEngine;

namespace BossAmiya
{
    public class Element_ElementBreaking : UnitBuf
    {
        public Element_ElementBreaking(AgentModel model, DarkElementController controller)
        {
            __owner = model;
            __controller = controller;
        }
        public override void Init(UnitModel model)
        {
            phase = 1;
        }
        public override void FixedUpdate()
        {
            _time -= Time.deltaTime;
            _tempTime += Time.deltaTime;
            __controller.SetProgress(_time / 30f * 100f);
            if (_time <= 15f && phase == 1)
            {
                _subDamage = 0.5f;
                phase = 2;
            }
            if (_time <= 0f)
            {
                ElementUI.Instance.RemoveElementUIFromAgent(__owner);
                Destroy();
            }
            if (_tempTime >= 1f && phase == 1)
            {
                __owner.hp -= 4;
                _tempTime = 0f;
            }
            if (phase == 2)
            {
                _subDamage = 1 - _time / 15f * 0.5f;
            }
        }
        public override float GetDamageFactor()
        {
            return _subDamage;
        }
        private AgentModel __owner;
        private DarkElementController __controller;
        private int phase = 0;
        private float _tempTime = 0f;
        private float _time = 30f;
        private float _subDamage = 1f;
    }
}
