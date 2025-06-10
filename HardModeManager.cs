using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace BossAmiya
{
    public class HardModeManager
    {
        private static HardModeManager _instance;
        public static HardModeManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new HardModeManager();
                }
                return _instance;
            }
        }
        public bool isHardModeBool = false;
        public bool isHardMode()
        {
            return this.isHardModeBool;
        }
        public void setIsHardMode(bool mode)
        {
            this.isHardModeBool = mode;
        }
    }
}
