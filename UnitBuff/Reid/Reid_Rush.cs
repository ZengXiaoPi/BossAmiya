using UnityEngine;

namespace BossAmiya
{
    public class Reid_Rush_OLD : UnitBuf
    {
        public Reid_Rush_OLD()
        {
            this.remainTime = float.MaxValue;
            this.moveScale = 0f;
            keepTime = 0f;
            _timer = 0f;
        }
        public override void FixedUpdate()
        {
            try
            {
                keepTime += Time.deltaTime;
                _timer += Time.deltaTime;
                if (!HardModeManager.Instance.isHardMode() && _timer >= 0.1f)
                {
                    moveScale += 0.018f;
                    if (moveScale > 5f)
                    {
                        moveScale = 5f;
                    }
                    _timer = 0f;
                }
                else if (_timer >= 0.1f)
                {
                    moveScale += 0.03f;
                    if (moveScale > 7.5f)
                    {
                        moveScale = 7.5f;
                    }
                    _timer = 0f;
                }
            (model as CreatureModel).movementScale = 1f + moveScale;
            }
            catch (System.Exception ex)
            {
                Harmony_Patch.logger.Error(ex);
            }
        }
        public float _timer;
        public float moveScale;
        public float keepTime;
    }
}
