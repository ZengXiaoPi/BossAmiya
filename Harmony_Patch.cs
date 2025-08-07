using CommandWindow;
using CreatureInfo;
using GlobalBullet;
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
        public static readonly string VERSION = "1.1.0";
        public static YKMTLog logger;
        public static string path = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));

        public static Sprite RealDamage_Sprite = Extension.CreateSprite("Image/RealDamage.png", SpriteMeshType.FullRect);
        public static Color RealDamage_Color = new Color32(184, 183, 183, byte.MaxValue);
        public static List<UnitModel> RealDamage_TempList = [];
        public Harmony_Patch()
        {
            logger = new YKMTLog(path + "/Logs", false);
            HarmonyInstance harmony = HarmonyInstance.Create("BossAmiya.zxp");
            logger.Info("—————————————————NEW GAME———————————————————");
            try
            {
                // 初始化凋亡UI
                ElementUI.Instance.InitAB();
                // 读取配置
                AwardConfigReader.ReadConfig();
                HPPatcher.PatchAll(harmony, typeof(Harmony_Patch));
                HPPatcher.PatchAll(harmony, typeof(AwardPatch));
            }
            catch (Exception ex)
            {
                logger.Error($"When patch, the error occurred.");
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
            if (actor is CreatureModel)
            {
                if ((actor as CreatureModel).metaInfo.id != 521892L) return;
                if (RealDamage_TempList.Contains(__instance)) return;
                RealDamage_TempList.Add(__instance);
            }
            else if (actor is AgentModel)
            {
                if ((actor as AgentModel).Equipment.armor.metaInfo.id == 52189301)
                {
                    if (RealDamage_TempList.Contains(__instance)) return;
                    RealDamage_TempList.Add(__instance);
                }
            }
        }
        [HPHelper(typeof(CreatureModel), nameof(CreatureModel.TakeDamage), typeof(UnitModel), typeof(DamageInfo))]
        [HPPrefix]
        public static void CreatureModel_TakeDamage(ref CreatureModel __instance, UnitModel actor, DamageInfo dmg)
        {
            if (actor is CreatureModel)
            {
                if ((actor as CreatureModel).metaInfo.id != 521892L) return;
                if (RealDamage_TempList.Contains(__instance)) return;
                RealDamage_TempList.Add(__instance);
            }
            else if (actor is AgentModel)
            {
                if ((actor as AgentModel).Equipment.armor.metaInfo.id == 52189301)
                {
                    if (RealDamage_TempList.Contains(__instance)) return;
                    RealDamage_TempList.Add(__instance);
                }
            }
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
            else if (fixcreature.script is Lamalian)
            {
                __instance.Portrait.sprite = Sprites.LamalianSprite;
            }
            else if (fixcreature.script is TSLZ)
            {
                __instance.Portrait.sprite = Sprites.TSLZSprite;
            }
            else if (fixcreature.script is Reid)
            {
                __instance.Portrait.sprite = Sprites.ReidSprite;
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
                if (cmd == "")
                {
                    return true;
                }
                string[] array = cmd.Split(new char[]
                {
                    ' '
                });
                string type1 = array[0].ToLower();
                string type2 = "";
                string type3 = "";
                if (array.Length >= 2)
                {
                    type2 = array[1].ToLower().Trim();
                }
                if (array.Length >= 3)
                {
                    type3 = array[2].ToLower().Trim();
                }
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
                    if (type2 == "mode")
                    {
                        if (type3 == "switch")
                        {
                            if (HardModeManager.Instance.isHardMode())
                            {
                                Extension.ShowMessageBox("当前模式为：普通模式。");
                                HardModeManager.Instance.setIsHardMode(false);
                                __result = "";
                                return false;
                            }
                            else
                            {
                                Extension.ShowMessageBox("当前模式为：困难模式。");
                                HardModeManager.Instance.setIsHardMode(true);
                                __result = "";
                                return false;
                            }
                        }
                    }
                }

                if ((cmd == "creature cryofbanshee" || cmd == "rimi creature cryofbanshee") && BossAmiya.amiyaPhase == 2)
                {
                    SefiraConversationController.Instance.UpdateConversation(Sprites.AmiyaSprite, Sprites.Amiya_Color, LocalizeTextDataModel.instance.GetText("BlockTT10_Amiya"));
                    __result = "";
                    return false;
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
        [HPHelper(typeof(UnitModel), "GetDmgMultiplierByEgoLevel")]
        [HPPrefix]
        public static bool UnitModel_GetDmgMultiplierByEgoLevel(int attackLevel, int defenseLevel, ref float __result)
        {
            if (RealDamage_TempList.Count > 0)
            {
                __result = 1.0f;
                return false;
            }
            return true;
        }
        [HPHelper(typeof(GlobalBulletManager), "SlowBullet")]
        [HPPrefix]
        public static void GlobalBulletManager_SlowBullet(UnitModel target)
        {
            if (target is CreatureModel && (target as CreatureModel).script is Reid)
            {
                Reid reid = (target as CreatureModel).script as Reid;
                if (!HardModeManager.Instance.isHardMode())
                {
                    target.AddUnitBuf(new Reid_weak(reid.speed / 5f * 1.5f, reid.speed / 5f * 30f));
                }
                else
                {
                    target.AddUnitBuf(new Reid_weak(reid.speed / 7.5f * 4f, reid.speed / 5f * 60f));
                }
                reid.speed = 0f;
            }
        }
        [HPHelper(typeof(GameManager), "UpdateGameSpeed")]
        [HPPostfix]
        public static void GameManager_UpdateGameSpeed()
        {
            if (BossAmiya.amiyaPhase == 2 && Time.timeScale > 2f)
            {
                SefiraConversationController.Instance.UpdateConversation(Sprites.AmiyaSprite, Sprites.Amiya_Color, LocalizeTextDataModel.instance.GetText("BlockTT10_Amiya"));
                Time.timeScale = 1f;
                Time.fixedDeltaTime = 0.02f;
            }
        }
    }
}