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
        }
        public override void FixedUpdate()
        {
            base.FixedUpdate();
            _tempTime += Time.deltaTime;
            __controller.SetProgress(remainTime / 30f * 100f);
            if (remainTime <= 0f)
            {
                ElementUI.Instance.RemoveElementUIFromAgent(__owner);
                ElementManager.Instance.ElementUnitIsBreaking.Remove(__owner);
                Destroy();
            }
            if (_tempTime >= 1f)
            {
                __owner.hp -= 4;
                _tempTime = 0f;
            }
        }
        private AgentModel __owner;
        private DarkElementController __controller;
        private float _tempTime = 0f;
    }
}
