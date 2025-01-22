using CommandWindow;
using CreatureInfo;
using Harmony;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace BossAmiya
{
    public class Harmony_Patch
    {
        public static readonly string VERSION = "1.0.0";
        public static YKMTLog logger;
        public static string path = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));

        public static Sprite RealDamage_Sprite = Extension.CreateSprite("Image/RealDamage.png", SpriteMeshType.FullRect);
        public static Color RealDamage_Color = new Color32(184, 183, 183, byte.MaxValue);
        public static List<UnitModel> RealDamage_TempList = new List<UnitModel>();
        public Harmony_Patch()
        {
            logger = new YKMTLog(path + "/Logs", false);
            int errorNum = 0;
            HarmonyInstance harmony = HarmonyInstance.Create("BossAmiya.zxp");
            logger.Info("—————————————————NEW GAME———————————————————");
            try
            {
                // 真实伤害 for amiya
                harmony.Patch(typeof(CreatureInfoStatRoot).GetMethod("WorkDamageListInit", AccessTools.all, null, new Type[0], null), null, new HarmonyMethod(typeof(Harmony_Patch).GetMethod("CreatureInfoStatRoot_WorkDamageListInit")), null); errorNum++;
                harmony.Patch(typeof(WorkAllocateRegion).GetMethod("OnObserved", AccessTools.all, null, new Type[] { typeof(CreatureModel) }, null), null, new HarmonyMethod(typeof(Harmony_Patch).GetMethod("WorkAllocateRegion_OnObserved")), null); errorNum++;
                harmony.Patch(typeof(WorkerModel).GetMethod("TakeDamage", AccessTools.all, null, new Type[] { typeof(UnitModel), typeof(DamageInfo) }, null), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("WorkModel_TakeDamage")), null); errorNum++;
                harmony.Patch(typeof(DamageEffect).GetMethod("SetData", AccessTools.all, null, new Type[] { typeof(RwbpType), typeof(int), typeof(DefenseInfo.Type), typeof(UnitModel) }, null), null, new HarmonyMethod(typeof(Harmony_Patch).GetMethod("DamageEffect_SetData")), null); errorNum++;

                // BGM
                harmony.Patch(typeof(SoundEffectPlayer).GetMethod("Stop", AccessTools.all, null, new Type[0], null), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("SoundEffectPlayer_Stop")), null, null); errorNum++;

                // 镇压区域贴图修正
                harmony.Patch(typeof(CreatureSuppressRegion).GetMethod("SetData", AccessTools.all, null, new Type[] { typeof(UnitModel) }, null), null, new HarmonyMethod(typeof(Harmony_Patch).GetMethod("CreatureSuppressRegion_SetData")), null); errorNum++;
            }
            catch (Exception ex)
            {
                logger.Error($"When patch, the error occurred. Error number: {errorNum}");
                logger.Error(ex);
            }
            logger.Info($"Story started. BossAmiya v{VERSION} is loaded.");
            logger.Info($"故事开始。魔王阿米娅 v{VERSION} 已加载。");
        }
        public static void CreatureInfoStatRoot_WorkDamageListInit(ref CreatureInfoStatRoot __instance)
        {
            if (__instance.CurrentModel != null)
            {
                if (__instance.CurrentModel.metaInfo.id == 521892L)
                {
                    __instance.WorkDamageType.text = "C.E.";
                    __instance.WorkDamageType.color = RealDamage_Color;
                    __instance.WorkDamageFill.sprite = RealDamage_Sprite;
                }
            }
        }
        public static void WorkAllocateRegion_OnObserved(ref WorkAllocateRegion __instance)
        {
            if (__instance.CurrentModel == null) return;
            if (__instance.CurrentModel.metaInfo.id != 521892L) return;
            __instance.WorkDamageType.text = "C.E.";
            __instance.WorkDamageType.color = RealDamage_Color;
            __instance.WorkDamageFill.sprite = RealDamage_Sprite;
        }
        public static void WorkModel_TakeDamage(ref WorkerModel __instance, UnitModel actor, DamageInfo dmg)
        {
            if (!(actor is CreatureModel)) return;
            if ((actor as CreatureModel).metaInfo.id != 521892L) return;
            if (RealDamage_TempList.Contains(__instance)) return;
            RealDamage_TempList.Add(__instance);
        }
        public static void DamageEffect_SetData(ref DamageEffect __instance, RwbpType type, int damage, DefenseInfo.Type defense, UnitModel unit)
        {
            if (type == RwbpType.N && RealDamage_TempList.Contains(unit))
            {
                RealDamage_TempList.Remove(unit);
                __instance.DamageCount.color = RealDamage_Color;
                __instance.DamageContext.color = RealDamage_Color;
                __instance.Frame.color = RealDamage_Color;
                __instance.IconOut.color = Color.grey;
                __instance.DefenseTypeInner.color = Color.grey;
                __instance.DefenseTypeText.color = RealDamage_Color;
                __instance.Icon.sprite = RealDamage_Sprite;
            }
        }
        public static void SoundEffectPlayer_Stop(ref SoundEffectPlayer __instance)
        {
            if (BossAmiya.fullBGMplayer == null) return;
            if (__instance == BossAmiya.fullBGMplayer)
            {
                BossAmiya.loopBGMplayer = BossAmiya.PlayBGM(file: "bgm-loop.wav", loop: true);
            }
        }
        public static void CreatureSuppressRegion_SetData(ref CreatureSuppressRegion __instance, UnitModel target)
        {
            CreatureModel fixcreature = target as CreatureModel;
            if (fixcreature.script is Kaltsit)
            {
                __instance.Portrait.sprite = Sprites.KaltsitSprite;
            }
            else if (fixcreature.script is Mon2tr)
            {
                __instance.Portrait.sprite = Sprites.Mon2trSprite;
            }
            else if (fixcreature.script is LCP)
            {
                __instance.Portrait.sprite = Sprites.LCPSprite;
            }
        }
    }
}