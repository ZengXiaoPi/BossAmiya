using HPHelper;
using Spine.Unity;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using WorkerSpine;

namespace BossAmiya
{
    public class EquipmentPatch
    {
        [HPHelper(typeof(AgentModel), "Panic")]
        [HPPrefix]
        public static bool OverridePanic(ref AgentModel __instance)
        {
            if (__instance.Equipment.weapon.metaInfo.id == 521801)
            {
                var script = __instance.Equipment.weapon.script as BossAmiya_weapon;
                if (!script.isRecoverying)
                {
                    script.OnPanic();
                }
                return false;
            }
            return true;
        }
        [HPHelper(typeof(AgentModel), "OnDie")]
        [HPPrefix]
        public static bool OnDieOverride(ref AgentModel __instance)
        {
            if (__instance.Equipment.weapon.metaInfo.id == 521801)
            {
                __instance.GetUnit().animChanger.ChangeAnimatorWithUniqueFace("BossBirdWeapon_BossAmiya", false);
                __instance.GetUnit().animEventHandler.SetAnimEvent(null);
                __instance.GetUnit().animChanger.SetState(true);
                __instance.GetUnit().animChanger.state.SetAnimation(0, "Die", false);
                __instance.hp = __instance.maxHp;
                __instance.SetInvincible(true);
                __instance.GetMovableNode().StopMoving();
                __instance.StopStun();
                __instance.StopAction();
                SefiraManager.instance.GetSefira(__instance.currentSefira).OnAgentCannotControll(__instance);
            }
            return true;
        }
    }
}
