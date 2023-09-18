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

        public Yodo1AdSettings()
        {
            this.androidSettings = new Yodo1PlatformSettings_Android();
            this.iOSSettings = new Yodo1PlatformSettings_iOS();
        }

    }
}



