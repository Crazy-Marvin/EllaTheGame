using System;
using UnityEngine;

namespace Yodo1.MAS
{
    [Serializable]
    public class Yodo1PlatformSettings_iOS : Yodo1PlatformSettings
    {
        public bool ChinaRegion;
        public bool GlobalRegion;
        public string AppLovinSdkKey;

        public Yodo1PlatformSettings_iOS()
        {
            this.ChinaRegion = false;
            this.GlobalRegion = true;
        }

    }
}



