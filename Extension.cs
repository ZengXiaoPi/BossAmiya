using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;

namespace BossAmiya
{
    public static class Extension
    {
        public static T GetPrivateField<T>(this object instance, string fieldname)
        {
            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;
            return (T)((object)instance.GetType().GetField(fieldname, bindingAttr).GetValue(instance));
        }
        public static void SetPrivateField(this object instance, string fieldname, object value)
        {
            BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.Static | BindingFlags.NonPublic;
            instance.GetType().GetField(fieldname, bindingAttr).SetValue(instance, value);
        }
        public static T CallPrivateMethod<T>(this object instance, string name, params object[] param)
        {
            T result;
            try
            {
                BindingFlags bindingAttr = BindingFlags.Instance | BindingFlags.NonPublic;
                result = (T)((object)instance.GetType().GetMethod(name, bindingAttr).Invoke(instance, param));
            }
            catch
            {
                result = default(T);
            }
            return result;
        }
        public static T CallPrivateStaticMethod<T>(this object instance, string name, params object[] param)
        {
            BindingFlags bindingAttr = BindingFlags.Static | BindingFlags.NonPublic;
            return (T)((object)instance.GetType().GetMethod(name, bindingAttr).Invoke(null, param));
        }
        public static Sprite CreateSprite(string file, SpriteMeshType meshtype)
        {
            string text = Harmony_Patch.path + "/" + file;
            Sprite result;
            if (File.Exists(text))
            {
                Texture2D texture2D = new Texture2D(1, 1);
                texture2D.LoadImage(File.ReadAllBytes(text));
                result = Sprite.Create(texture2D, new Rect(0f, 0f, (float)texture2D.width, (float)texture2D.height), new Vector2(0.5f, 0.5f), 100f, 0U, meshtype);
            }
            else
            {
                result = null;
            }
            return result;
        }
        public static Texture2D CreateTexture2D(string file)
        {
            Texture2D result = null;
            string text = Harmony_Patch.path + "/" + file;
            if (File.Exists(text))
            {
                result = new Texture2D(1, 1);
                result.LoadImage(File.ReadAllBytes(text));
            }
            return result;
        }
        public static void RecoveryHPForCreature(CreatureModel model, int recoveryHP)
        {
            if (model.hp + recoveryHP > model.metaInfo.maxHp)
            {
                model.hp = model.metaInfo.maxHp;
            }
            else
            {
                model.hp += recoveryHP;
            }
        }
        public static void RecoveryHPForCreature(CreatureModel model, float recoveryHPFloat)
        {
            int recoveryHP = (int)Mathf.Round(recoveryHPFloat);
            if (model.hp + recoveryHP > model.metaInfo.maxHp)
            {
                model.hp = model.metaInfo.maxHp;
            }
            else
            {
                model.hp += recoveryHP;
            }
        }
        public static bool IsInRange(UnitModel owner, UnitModel target, float range)
        {
            return MovableObjectNode.GetDistance(owner.GetMovableNode(), target.GetMovableNode()) - owner.radius - target.radius <= range;
        }
        public static bool HasBuff(UnitModel unit, Type buf)
        {
            if (unit == null) return false;
            foreach (UnitBuf unitbuf in unit.GetUnitBufList())
            {
                if (buf.IsInstanceOfType(unitbuf))
                {
                    Harmony_Patch.logger.Info("Has buff: " + buf.Name);
                    return true;
                }
            }
            return false;
        }
        public static void RemoveBuff(UnitModel unit, Type buf)
        {
            if (unit == null) return;

            var bufList = unit.GetUnitBufList();
            if (bufList == null) return;

            // 使用倒序遍历避免集合修改异常
            for (int i = bufList.Count - 1; i >= 0; i--)
            {
                try
                {
                    UnitBuf unitbuf = bufList[i];
                    if (buf.IsInstanceOfType(unitbuf))
                    {
                        unit.RemoveUnitBuf(unitbuf);
                    }
                }
                catch (Exception ex)
                {
                    Harmony_Patch.logger.Error($"Error removing buff at index {i}: {ex.Message}");
                }
            }
        }

        private static readonly List<Type> ExcludedTypes =
        [
            typeof(BossAmiya),
            typeof(Goria),
            typeof(Kaltsit),
            typeof(LCP),
            typeof(Mon2tr),
            typeof(KL),
            typeof(TSLZ),
            typeof(Lamalian),
            typeof(Reid)
        ];

        public static bool CheckIsHostileCreature(CreatureModel creature)
        {
            if (ExcludedTypes.Contains(creature.script.GetType()))
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public static List<T> GetRandomElements<T>(IList<T> list, int count)
        {
            if (count > list.Count)
                count = list.Count;
            var copy = new List<T>(list);
            var result = new List<T>(count);
            System.Random rand = new System.Random();
            for (int i = 0; i < count; i++)
            {
                int randomIndex = rand.Next(i, copy.Count);
                (copy[i], copy[randomIndex]) = (copy[randomIndex], copy[i]);
                result.Add(copy[i]);
            }
            return result;
        }
    }
}
