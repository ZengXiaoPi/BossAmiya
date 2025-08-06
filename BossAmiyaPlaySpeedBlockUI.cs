using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine.UI;

namespace BossAmiya
{
    public class BossAmiyaPlaySpeedBlockUI : global::GameStatusUI.PlaySpeedSettingBlockedUI
    {
        public BossAmiyaPlaySpeedBlockUI()
        {
            pauseText = LocalizeTextDataModel.instance.GetText("BlockPhase_Amiya");
            escapeText = LocalizeTextDataModel.instance.GetText("BlockEscape_Amiya");
            this.AddAction(global::PlaySpeedSettingBlockFunction.TIMESTOP_PAUSE, new global::GameStatusUI.PlaySpeedSettingBlockedUI.voidAction(this.OnTryPause));
            this.AddAction(global::PlaySpeedSettingBlockFunction.SPEEDMULTIPLIER, new global::GameStatusUI.PlaySpeedSettingBlockedUI.voidAction(this.OnTryPause));
            this.AddAction(global::PlaySpeedSettingBlockFunction.MANUAL, new global::GameStatusUI.PlaySpeedSettingBlockedUI.voidAction(this.OnTryPause));
            this.AddAction(global::PlaySpeedSettingBlockFunction.ESCAPE, new global::GameStatusUI.PlaySpeedSettingBlockedUI.voidAction(this.OnTryEscape));
        }
        public override bool IsFunctionEnabled(PlaySpeedSettingBlockFunction function)
        {
            if (function == PlaySpeedSettingBlockFunction.TIMESTOP_PAUSE || function == PlaySpeedSettingBlockFunction.SPEEDMULTIPLIER)
            {
                return true;
            }
            if (function == global::PlaySpeedSettingBlockFunction.ESCAPE || function == global::PlaySpeedSettingBlockFunction.MANUAL)
            {
                return !GameStatusUI.GameStatusUI.Window.sceneController.IsAgentAlldead;
            }
            return false;
        }
        public void OnTryPause()
        {
            if (!this.IsFunctionEnabled(global::PlaySpeedSettingBlockFunction.ESCAPE))
            {
                return;
            }
            SefiraConversationController.Instance.UpdateConversation(Sprites.AmiyaSprite, Sprites.Amiya_Color, pauseText);
            OnShow();
        }
        public void OnTryEscape()
        {
            if (!this.IsFunctionEnabled(global::PlaySpeedSettingBlockFunction.ESCAPE))
            {
                return;
            }
            SefiraConversationController.Instance.UpdateConversation(Sprites.AmiyaSprite, Sprites.Amiya_Color, escapeText);
            OnShow();
        }
        public string pauseText = "";
        public string escapeText = "";
    }
}
