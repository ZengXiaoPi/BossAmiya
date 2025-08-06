using GlobalBullet;
using Harmony;
using HPHelper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BossAmiya
{
    public class AwardPatch
    {
        [HPHelper(typeof(CursorManager), "CursorSet", typeof(MouseCursorType))]
        [HPPostfix]
        public static void CursorManager_CursorSet(ref CursorManager __instance, MouseCursorType type)
        {
            if (!AwardConfig.beatAmiya || !AwardConfig.useAmiyaBullet)
            {
                return;
            }
            try
            {
                if (type >= MouseCursorType.BULLET_EXECUTION)
                {
                    Texture2D bulletTexture;
                    switch (type)
                    {
                        case MouseCursorType.BULLET_HEALTH:
                            {
                                bulletTexture = BulletSprite.HPRecovery;
                                break;
                            }
                        case MouseCursorType.BULLET_MENTAL:
                            {
                                bulletTexture = BulletSprite.MPRecovery;
                                break;
                            }
                        case MouseCursorType.BULLET_EXECUTION:
                            {
                                bulletTexture = BulletSprite.Execute;
                                break;
                            }
                        case MouseCursorType.BULLET_SHIELD_R:
                            {
                                bulletTexture = BulletSprite.RedShield;
                                break;
                            }
                        case MouseCursorType.BULLET_SHIELD_W:
                            {
                                bulletTexture = BulletSprite.WhiteShield;
                                break;
                            }
                        case MouseCursorType.BULLET_SHIELD_B:
                            {
                                bulletTexture = BulletSprite.BlackShield;
                                break;
                            }
                        case MouseCursorType.BULLET_SHIELD_P:
                            {
                                bulletTexture = BulletSprite.PaleShield;
                                break;
                            }
                        case MouseCursorType.BULLET_SLOW:
                            {
                                bulletTexture = BulletSprite.Slow;
                                break;
                            }
                        default:
                            return;
                    }
                    __instance.cursorMode = CursorMode.ForceSoftware;
                    bulletTexture = WorkerSpriteManager.ScaleTexture(bulletTexture, (int)__instance.BulletCursorSize.x, (int)__instance.BulletCursorSize.y, true);
                    Vector2 hotspot = __instance.GetHotspot(type, bulletTexture);
                    Extension.SetPrivateField(__instance, "_currentHotspot", hotspot);
                    Extension.SetPrivateField(__instance, "currentCursorTexture", bulletTexture);
                    Cursor.SetCursor(bulletTexture, hotspot, __instance.cursorMode);
                }
            }
            catch (Exception e)
            {
                Harmony_Patch.logger.Error(e);
            }
        }
        [HPHelper(typeof(GlobalBulletManager), "RecoverHPBullet")]
        [HPPrefix]
        public static bool GlobalBulletManager_RecoverHPBullet(ref GlobalBulletManager __instance, UnitModel target)
        {
            if (!AwardConfig.beatAmiya || !AwardConfig.useAmiyaBullet)
            {
                return true;
            }
            WorkerModel workerModel = target as WorkerModel;
            if (workerModel != null && !workerModel.HasUnitBuf(UnitBufType.QUEENBEE_SPORE))
            {
                int shield = 0;
                if (workerModel.hp + 80 > workerModel.maxHp)
                {
                    shield = (int)workerModel.hp + 80 - workerModel.maxHp;
                    workerModel.AddUnitBuf(new global::BarrierBuf(global::RwbpType.A, shield, 15f));
                }
                workerModel.RecoverHP(80);
            }
            return false;
        }
        public static bool alreadyHealPanicWorker = false;
        [HPHelper(typeof(GlobalBulletManager), "RecoverMentalBullet")]
        [HPPrefix]
        public static bool GlobalBulletManager_RecoverMentalBullet(ref GlobalBulletManager __instance, UnitModel target)
        {
            if (!AwardConfig.beatAmiya || !AwardConfig.useAmiyaBullet)
            {
                return true;
            }
            WorkerModel workerModel = target as WorkerModel;
            if (workerModel != null)
            {
                workerModel.RecoverMental(60);
            }
            if (workerModel != null && workerModel.IsPanic() && !alreadyHealPanicWorker)
            {
                alreadyHealPanicWorker = true;
                workerModel.StopPanic();
                workerModel.mental = workerModel.maxMental;
            }
            return false;
        }
        [HPHelper(typeof(GlobalBulletManager), "ResistRBullet")]
        [HPPrefix]
        public static bool GlobalBulletManager_ResistRBullet(ref GlobalBulletManager __instance, UnitModel target)
        {
            if (!AwardConfig.beatAmiya || !AwardConfig.useAmiyaBullet)
            {
                return true;
            }
            WorkerModel workerModel = target as WorkerModel;
            if (workerModel != null)
            {
                workerModel.AddUnitBuf(new BarrierBuf(RwbpType.R, 75f, 60f));
            }
            return false;
        }
        [HPHelper(typeof(CreatureOverloadManager), "ActivateOverload", true)]
        [HPPrefix]
        public static void CreatureOverloadManager_ActivateOverload()
        {
            if (!AwardConfig.beatAmiya || !AwardConfig.useAmiyaBullet)
            {
                return;
            }
            AwardPatch.alreadyHealPanicWorker = false;
        }
        [HPHelper(typeof(GlobalBulletManager), "ResistWBullet")]
        [HPPrefix]
        public static bool GlobalBulletManager_ResistWBullet(ref GlobalBulletManager __instance, UnitModel target)
        {
            if (!AwardConfig.beatAmiya || !AwardConfig.useAmiyaBullet)
            {
                return true;
            }
            WorkerModel workerModel = target as WorkerModel;
            if (workerModel != null)
            {
                workerModel.AddUnitBuf(new BarrierBuf(RwbpType.W, 75f, 60f));
            }
            return false;
        }
        [HPHelper(typeof(GlobalBulletManager), "ResistBBullet")]
        [HPPrefix]
        public static bool GlobalBulletManager_ResistBBullet(ref GlobalBulletManager __instance, UnitModel target)
        {
            if (!AwardConfig.beatAmiya || !AwardConfig.useAmiyaBullet)
            {
                return true;
            }
            WorkerModel workerModel = target as WorkerModel;
            if (workerModel != null)
            {
                workerModel.AddUnitBuf(new BarrierBuf(RwbpType.B, 75f, 60f));
            }
            return false;
        }
        [HPHelper(typeof(GlobalBulletManager), "ResistPBullet")]
        [HPPrefix]
        public static bool GlobalBulletManager_ResistPBullet(ref GlobalBulletManager __instance, UnitModel target)
        {
            if (!AwardConfig.beatAmiya || !AwardConfig.useAmiyaBullet)
            {
                return true;
            }
            WorkerModel workerModel = target as WorkerModel;
            if (workerModel != null)
            {
                workerModel.AddUnitBuf(new BarrierBuf(RwbpType.P, 75f, 60f));
            }
            return false;
        }
        [HPHelper(typeof(GlobalBulletManager), "SlowBullet")]
        [HPPrefix]
        public static bool GlobalBulletManager_SlowBullet(ref GlobalBulletManager __instance, UnitModel target)
        {
            if (!AwardConfig.beatAmiya || !AwardConfig.useAmiyaBullet)
            {
                return true;
            }
            target.AddUnitBuf(new SlowBulletBuf(45f));
            return false;
        }
        [HPHelper(typeof(SlowBulletBuf), "MovementScale")]
        [HPPostfix]
        public static void SlowBulletBuf_MovementScale(ref float __result)
        {
            if (!AwardConfig.beatAmiya || !AwardConfig.useAmiyaBullet)
            {
                return;
            }
            __result = 0.3f;
        }
        [HPHelper(typeof(GlobalBulletManager), "ActivateBullet")]
        [HPPostfix]
        public static void GlobalBulletManager_ActivateBullet(ref GlobalBulletManager __instance,GlobalBulletType type, List<UnitModel> targets)
        {
            if (!AwardConfig.beatAmiya || !AwardConfig.useAmiyaBullet)
            {
                return;
            }
            if (type == GlobalBulletType.EXCUTE && __instance.currentBullet != 0)
            {
                __instance.currentBullet++;
            }
        }
    }
}
