using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace BossAmiya
{
    public static class Sprites
    {
        public static Sprite AmiyaSprite = Extension.CreateSprite("/Creature/Portrait/BossAmiya.png", SpriteMeshType.FullRect);
        public static Sprite KaltsitSprite = Extension.CreateSprite("/Creature/Portrait/Kaltsit.png", SpriteMeshType.FullRect);
        public static Sprite LCPSprite = Extension.CreateSprite("/Creature/Portrait/LCP.png", SpriteMeshType.FullRect);
        public static Sprite Mon2trSprite = Extension.CreateSprite("/Creature/Portrait/Mon2tr.png", SpriteMeshType.FullRect);

        public static Color Amiya_Color = new Color32(184, 183, 183, byte.MaxValue);
        public static Color Kaltsit_Color = new Color32(38, 182, 50, byte.MaxValue);
        public static Color LCP_Color = new Color32(255, 255, 255, byte.MaxValue);
        public static Color Mon2tr_Color = new Color32(192, 192, 192, byte.MaxValue);
    }
}
