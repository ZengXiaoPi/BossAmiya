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
        public static Sprite GoriaSprite = Extension.CreateSprite("/Creature/Portrait/Goria.png", SpriteMeshType.FullRect);
        public static Sprite KLSprite = Extension.CreateSprite("/Creature/Portrait/KL.png", SpriteMeshType.FullRect);
        public static Sprite TSLZSprite = Extension.CreateSprite("/Creature/Portrait/TSLZ.png", SpriteMeshType.FullRect);
        public static Sprite LamalianSprite = Extension.CreateSprite("/Creature/Portrait/Lamalian.png", SpriteMeshType.FullRect);
        public static Sprite ReidSprite = Extension.CreateSprite("/Creature/Portrait/Reid.png", SpriteMeshType.FullRect);

        public static Color Amiya_Color = new Color32(184, 183, 183, byte.MaxValue);
        public static Color Kaltsit_Color = new Color32(38, 182, 50, byte.MaxValue);
        public static Color LCP_Color = new Color32(255, 255, 255, byte.MaxValue);
        public static Color Mon2tr_Color = new Color32(192, 192, 192, byte.MaxValue);
        public static Color Goria_Color = new Color32(180, 77, 76, byte.MaxValue);
        public static Color KL_Color = new Color32(151, 144, 131, byte.MaxValue);
        public static Color TSLZ_Color = new Color32(126, 116, 99, byte.MaxValue);
        public static Color Lamalian_Color = new Color32(181, 188, 203, byte.MaxValue);
        public static Color Reid_Color = new Color32(200, 43, 35, byte.MaxValue);

        public static Color Escape_Relic_Color = new Color32(160, 67, 68, byte.MaxValue);
    }
}
