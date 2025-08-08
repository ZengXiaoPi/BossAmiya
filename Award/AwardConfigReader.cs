using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace BossAmiya
{
    /*
     * The config file:
     * 
    <rewardConfig>
        <beatAmiya>
            <desc>是否击败阿米娅</desc>
            <value>false</value>
        </beatAmiya>
        <useAmiyaBullet>
            <desc>在击败阿米娅，“炉芯终曲”后，是否使用本模组的加强子弹</desc>
            <value>true</value>
        </useAmiyaBullet>
        <allowAmiyaEscape>
            <desc>在击败阿米娅，“炉芯终曲”后，是否允许阿米娅出逃</desc>
            <value>false</value>
        </allowAmiyaEscape>
        <allowAmiyaSubCounter>
            <desc>在击败阿米娅，“炉芯终曲”后，阿米娅是否会因为员工死亡掉落扣除逆卡巴拉计数器</desc>
            <value>false</value>
        </allowAmiyaSubCounter>
        <allowAmiyaUpSuccess>
            <desc>在击败阿米娅，“炉芯终曲”后，是否增加阿米娅成功率</desc>
            <value>false</value>
        </allowAmiyaUpSuccess>
    </rewardConfig>
    */
    public static class AwardConfigReader
    {
        public static string path = Path.GetDirectoryName(Uri.UnescapeDataString(new UriBuilder(Assembly.GetExecutingAssembly().CodeBase).Path));
        private readonly static string xmlFilePath = path + "/Reward.xml";
        private static XmlDocument xmlDoc;
        public static void ReadConfig()
        {
            xmlDoc = new();
            xmlDoc.Load(xmlFilePath);
            AwardConfig.beatAmiya = bool.Parse(xmlDoc.SelectSingleNode("/rewardConfig/beatAmiya/value").InnerText);
            AwardConfig.useAmiyaBullet = bool.Parse(xmlDoc.SelectSingleNode("/rewardConfig/useAmiyaBullet/value").InnerText);
            AwardConfig.allowAmiyaEscape = bool.Parse(xmlDoc.SelectSingleNode("/rewardConfig/allowAmiyaEscape/value").InnerText);
            AwardConfig.allowAmiyaSubCounter = bool.Parse(xmlDoc.SelectSingleNode("/rewardConfig/allowAmiyaSubCounter/value").InnerText);
            AwardConfig.allowAmiyaUpSuccess = bool.Parse(xmlDoc.SelectSingleNode("/rewardConfig/allowAmiyaUpSuccess/value").InnerText);
            AwardConfig.playBGM = bool.Parse(xmlDoc.SelectSingleNode("/Config/playBGM/value").InnerText);
        }
        private static void SaveConfig()
        {
            try
            {
                xmlDoc.Save(xmlFilePath);
            }
            catch (Exception ex)
            {
                Harmony_Patch.logger.Error($"Save Config Error: {ex}");
            }
        }
        public static void SetConfigValue(string configName, bool value)
        {
            XmlNode valueNode = xmlDoc.SelectSingleNode($"/rewardConfig/{configName}/value");
            if (valueNode != null)
            {
                valueNode.InnerText = value.ToString().ToLower();
                SaveConfig();
            }
            else
            {
                Console.WriteLine($"找不到配置项: {configName}");
            }
        }
    }
    public static class AwardConfig
    {
        public static bool beatAmiya;
        public static bool useAmiyaBullet;
        public static bool allowAmiyaEscape;
        public static bool allowAmiyaSubCounter;
        public static bool allowAmiyaUpSuccess;
        public static bool playBGM;
    }
}
