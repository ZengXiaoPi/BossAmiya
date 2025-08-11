using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
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

        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        private static extern int MessageBoxW(
            IntPtr hWnd,          // 父窗口句柄，0 表示没有父窗口
            string lpText,       // 消息正文
            string lpCaption,    // 窗口标题
            uint uType);         // 按钮/图标等样式标志
        /// <summary>
        /// 调用 Windows API 弹出消息框
        /// </summary>
        /// <param name="text">要显示的文字</param>
        /// <param name="caption">标题栏文字</param>
        /// <param name="type">按钮/图标组合（参见 MSDN MessageBox 常量）</param>
        /// <returns>用户点击的按钮对应的返回值（IDOK、IDCANCEL、IDYES、IDNO …）</returns>
        public static int ShowMessageBox(string text, string caption = "提示", uint type = 0)
        {
            // hWnd 设为 0，表示框没有父窗口（独立弹出）
            return MessageBoxW(IntPtr.Zero, text, caption, type);
        }
    }
    /// <summary>
    /// 数学类扩展方法
    /// </summary>
    public static class MathEx
    {
        /// <summary>
        /// 远离 0 向上舍入
        /// </summary>
        public static decimal RoundUp(this decimal value, sbyte digits)
        {
            if (digits == 0)
            {
                return (value >= 0 ? decimal.Ceiling(value) : decimal.Floor(value));
            }

            decimal multiple = Convert.ToDecimal(Math.Pow(10, digits));
            return (value >= 0 ? decimal.Ceiling(value * multiple) : decimal.Floor(value * multiple)) / multiple;
        }

        /// <summary>
        /// 靠近 0 向下舍入
        /// </summary>
        public static decimal RoundDown(this decimal value, sbyte digits)
        {
            if (digits == 0)
            {
                return (value >= 0 ? decimal.Floor(value) : decimal.Ceiling(value));
            }

            decimal multiple = Convert.ToDecimal(Math.Pow(10, digits));
            return (value >= 0 ? decimal.Floor(value * multiple) : decimal.Ceiling(value * multiple)) / multiple;
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        public static decimal RoundEx(this decimal value, sbyte digits)
        {
            if (digits >= 0)
            {
                return decimal.Round(value, digits, MidpointRounding.AwayFromZero);
            }

            decimal multiple = Convert.ToDecimal(Math.Pow(10, -digits));
            return decimal.Round(value / multiple, MidpointRounding.AwayFromZero) * multiple;
        }

        /// <summary>
        /// 远离 0 向上舍入
        /// </summary>
        public static double RoundUp(this double value, sbyte digits)
        {
            return decimal.ToDouble(Convert.ToDecimal(value).RoundUp(digits));
        }

        /// <summary>
        /// 靠近 0 向下舍入
        /// </summary>
        public static double RoundDown(this double value, sbyte digits)
        {
            return decimal.ToDouble(Convert.ToDecimal(value).RoundDown(digits));
        }

        /// <summary>
        /// 四舍五入
        /// </summary>
        public static double RoundEx(this double value, sbyte digits)
        {
            return decimal.ToDouble(Convert.ToDecimal(value).RoundEx(digits));
        }
    }
}
