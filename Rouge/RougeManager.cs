using NewGameMode;
using System.IO;

namespace BossAmiya
{
    public class RougeManager
    {
        private static RougeManager _instance;
        public static RougeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RougeManager();
                }
                return _instance;
            }
        }
        public bool isRougeInstalled = false;
        public void Init()
        {
            isRougeInstalled = false;
            var modsDir = new DirectoryInfo(Harmony_Patch.path).Parent.FullName;
            if (Directory.Exists(Path.Combine(modsDir, "ykmt_LobotomyRougeLike")))
            {
                isRougeInstalled = true;
            }
        }
        public bool isHasRelic()
        {
            if (!isRougeInstalled) return false;
            foreach (var meme in MemeManager.instance.current_list)
            {
                if (meme.script is BossAmiya_Relic) return true;
            }
            return false;
        }
    }
}
