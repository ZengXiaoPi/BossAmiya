using UnityEngine;
using static UnityEngine.UI.CanvasScaler;

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
            this.remainTime = 30f;
            this.model = model;
            phase = 1;
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            _tempTime += Time.deltaTime;
            __controller.SetProgress(remainTime / 30f * 100f);
            if (remainTime <= 15f && phase == 1)
            {
                _subDamage = 0.5f;
                phase = 2;
            }
            if (remainTime <= 0f)
            {
                ElementUI.Instance.RemoveElementUIFromAgent(__owner);
                ElementManager.Instance.ElementUnitIsBreaking.Remove(__owner);
                Destroy();
            }
            if (_tempTime >= 1f && phase == 1)
            {
                __owner.hp -= 4;
                _tempTime = 0f;
            }
            if (phase == 2)
            {
                _subDamage = 1 - remainTime / 15f * 0.5f;
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
        private float _subDamage = 1f;
    }
}
