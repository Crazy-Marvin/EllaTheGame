using System;
using UnityEngine;

namespace Yodo1.MAS
{
    [Serializable]
    public class Yodo1AdSettings : ScriptableObject
    {
        [HideInInspector]
        public Yodo1PlatformSettings_Android androidSettings;

        [HideInInspector]
        public Yodo1PlatformSettings_iOS iOSSettings;

        [HideInInspector]
        public Yodo1AdDynamicNetworkSettings_Android androidDynamicNetworkSettings;

        [HideInInspector]
        public Yodo1AdDynamicNetworkSettings_IOS iosDynamicNetworkSettings;

        public Yodo1AdSettings()
        {
            this.androidSettings = new Yodo1PlatformSettings_Android();
            this.iOSSettings = new Yodo1PlatformSettings_iOS();
            this.androidDynamicNetworkSettings = new Yodo1AdDynamicNetworkSettings_Android();
            this.iosDynamicNetworkSettings = new Yodo1AdDynamicNetworkSettings_IOS();
        }

    }
}



