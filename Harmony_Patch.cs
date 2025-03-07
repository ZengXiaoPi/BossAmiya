using CommandWindow;
using CreatureInfo;
using Harmony;
using HPHelper;
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
        public static List<UnitModel> RealDamage_TempList = [];
        public Harmony_Patch()
        {
            logger = new YKMTLog(path + "/Logs", false);
            int errorNum = 0;
            HarmonyInstance harmony = HarmonyInstance.Create("BossAmiya.zxp");
            logger.Info("—————————————————NEW GAME———————————————————");
            try
            {
                // 初始化凋亡UI
                ElementUI.Instance.InitAB();

                // 初始化模因检测
                RougeManager.Instance.Init();

                HPPatcher.PatchAll(harmony, typeof(Harmony_Patch));
                /*
                // 真实伤害 for amiya
                harmony.Patch(typeof(CreatureInfoStatRoot).GetMethod("WorkDamageListInit", AccessTools.all, null, new Type[0], null), null, new HarmonyMethod(typeof(Harmony_Patch).GetMethod("CreatureInfoStatRoot_WorkDamageListInit")), null); errorNum++;
                harmony.Patch(typeof(WorkAllocateRegion).GetMethod("OnObserved", AccessTools.all, null, new Type[] { typeof(CreatureModel) }, null), null, new HarmonyMethod(typeof(Harmony_Patch).GetMethod("WorkAllocateRegion_OnObserved")), null); errorNum++;
                harmony.Patch(typeof(WorkerModel).GetMethod("TakeDamage", AccessTools.all, null, new Type[] { typeof(UnitModel), typeof(DamageInfo) }, null), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("WorkerModel_TakeDamage")), null); errorNum++;
                harmony.Patch(typeof(CreatureModel).GetMethod("TakeDamage", AccessTools.all, null, new Type[] { typeof(UnitModel), typeof(DamageInfo) }, null), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("CreatureModel_TakeDamage")), null); errorNum++;
                harmony.Patch(typeof(DamageEffect).GetMethod("SetData", AccessTools.all, null, new Type[] { typeof(RwbpType), typeof(int), typeof(DefenseInfo.Type), typeof(UnitModel) }, null), null, new HarmonyMethod(typeof(Harmony_Patch).GetMethod("DamageEffect_SetData")), null); errorNum++;

                // BGM
                harmony.Patch(typeof(SoundEffectPlayer).GetMethod("Stop", AccessTools.all, null, new Type[0], null), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("SoundEffectPlayer_Stop")), null, null); errorNum++;

                // 镇压区域贴图修正
                harmony.Patch(typeof(CreatureSuppressRegion).GetMethod("SetData", AccessTools.all, null, new Type[] { typeof(UnitModel) }, null), null, new HarmonyMethod(typeof(Harmony_Patch).GetMethod("CreatureSuppressRegion_SetData")), null); errorNum++;

                // 在结束一天后不得不做的
                harmony.Patch(typeof(GameManager).GetMethod("EndGame", AccessTools.all, null, new Type[0], null), null, new HarmonyMethod(typeof(Harmony_Patch).GetMethod("GameManager_EndGame")), null); errorNum++;

                // 控制台
                harmony.Patch(typeof(ConsoleScript).GetMethod("GetHmmCommand", AccessTools.all, null, new Type[] { typeof(string) }, null), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("ConsoleScript_GetHmmCommand")), null, null); errorNum++;

                // 凋亡禁止攻击
                harmony.Patch(typeof(UnitModel).GetMethod("Attack", AccessTools.all, null, new Type[] { typeof(UnitModel) }, null), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("UnitModel_Attack")), null, null); errorNum++;

                // 死亡时移除元素UI
                harmony.Patch(typeof(AgentModel).GetMethod("Die", AccessTools.all, null, new Type[0], null), new HarmonyMethod(typeof(Harmony_Patch).GetMethod("WorkerModel_Die")), null, null); errorNum++;
                */
            }
            catch (Exception ex)
            {
                logger.Error($"When patch, the error occurred. Error number: {errorNum}");
                logger.Error(ex);
            }
            logger.Info($"Story started. BossAmiya v{VERSION} is loaded.");
            logger.Info($"故事开始。魔王阿米娅 v{VERSION} 已加载。");
        }
        [HPHelper(typeof(CreatureInfoStatRoot), "WorkDamageListInit", true)]
        [HPPostfix]
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
        [HPHelper(typeof(WorkAllocateRegion), "OnObserved", typeof(CreatureModel))]
        [HPPostfix]
        public static void WorkAllocateRegion_OnObserved(ref WorkAllocateRegion __instance)
        {
            if (__instance.CurrentModel == null) return;
            if (__instance.CurrentModel.metaInfo.id != 521892L) return;
            __instance.WorkDamageType.text = "C.E.";
            __instance.WorkDamageType.color = RealDamage_Color;
            __instance.WorkDamageFill.sprite = RealDamage_Sprite;
        }
        [HPHelper(typeof(WorkerModel), nameof(WorkerModel.TakeDamage), typeof(UnitModel), typeof(DamageInfo))]
        [HPPrefix]
        public static void WorkerModel_TakeDamage(ref WorkerModel __instance, UnitModel actor, DamageInfo dmg)
        {
            if (!(actor is CreatureModel)) return;
            if ((actor as CreatureModel).metaInfo.id != 521892L) return;
            if (RealDamage_TempList.Contains(__instance)) return;
            RealDamage_TempList.Add(__instance);
        }
        [HPHelper(typeof(CreatureModel), nameof(CreatureModel.TakeDamage), typeof(UnitModel), typeof(DamageInfo))]
        [HPPrefix]
        public static void CreatureModel_TakeDamage(ref CreatureModel __instance, UnitModel actor, DamageInfo dmg)
        {
            if (!(actor is CreatureModel)) return;
            if ((actor as CreatureModel).metaInfo.id != 521892L) return;
            if (RealDamage_TempList.Contains(__instance)) return;
            RealDamage_TempList.Add(__instance);
        }
        [HPHelper(typeof(DamageEffect), "SetData", typeof(RwbpType), typeof(int), typeof(DefenseInfo.Type), typeof(UnitModel))]
        [HPPostfix]
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
        [HPHelper(typeof(SoundEffectPlayer), nameof(SoundEffectPlayer.Stop))]
        [HPPrefix]
        public static void SoundEffectPlayer_Stop(ref SoundEffectPlayer __instance)
        {
            if (BossAmiya.fullBGMplayer == null) return;
            if (__instance == BossAmiya.fullBGMplayer)
            {
                BossAmiya.loopBGMplayer = BossAmiya.PlayBGM(file: "bgm-loop.wav", loop: true);
            }
        }
        [HPHelper(typeof(CreatureSuppressRegion), nameof(CreatureSuppressRegion.SetData), typeof(UnitModel))]
        [HPPostfix]
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
            else if (fixcreature.script is Goria)
            {
                __instance.Portrait.sprite = Sprites.GoriaSprite;
            }
        }

        [HPHelper(typeof(GameManager), nameof(GameManager.EndGame))]
        [HPPostfix]
        public static void GameManager_EndGame()
        {
            ElementManager.Instance.ClearDictionary();
        }

        [HPHelper(typeof(ConsoleScript), "GetHmmCommand", typeof(string))]
        [HPPrefix]
        public static bool ConsoleScript_GetHmmCommand(string cmd, ref string __result)
        {
            try
            {
                string[] array = cmd.Split(new char[]
                {
                    ' '
                });
                string type1 = array[0].ToLower();
                string type2 = array[1].ToLower().Trim();
                string type3 = array[2].ToLower().Trim();
                int value = 0;
                if (array.Length >= 4)
                {
                    value = Convert.ToInt32(array[3].ToLower().Trim());
                }
                int value2 = 0;
                if (array.Length >= 5)
                {
                    value2 = Convert.ToInt32(array[4].ToLower().Trim());
                }

                if (type1 == "zxp")
                {
                    if (type2 == "element")
                    {
                        if (type3 == "damage")
                        {
                            var agent = AgentManager.instance.GetAgent(value);
                            if (agent != null)
                            {
                                ElementManager.Instance.GiveElementDamage(agent, value2);
                                __result = "";
                                return false;
                            }
                            else
                            {
                                logger.Error("Agent not found.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                logger.Error(ex);
            }
            return true;
        }
        [HPHelper(typeof(UnitModel), nameof(UnitModel.Attack), typeof(UnitModel))]
        [HPPrefix]
        public static bool UnitModel_Attack(UnitModel target, ref UnitModel __instance)
        {
            if (__instance is AgentModel)
            {
                if (ElementManager.Instance.ElementUnitIsBreaking.ContainsKey((AgentModel)__instance))
                {
                    if (ElementManager.Instance.ElementUnitIsBreaking[(AgentModel)__instance])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
        [HPHelper(typeof(AgentModel), "Die")]
        [HPPostfix]
        public static void WorkerModel_Die(ref WorkerModel __instance)
        {
            if (__instance is AgentModel)
            {
                if (ElementUI.Instance.GetElementController((AgentModel)__instance) != null)
                {
                    ElementUI.Instance.RemoveElementUIFromAgent((AgentModel)__instance);
                }
            }
        }
    }
}