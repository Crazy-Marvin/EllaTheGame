using System;
using UnityEngine;

namespace Yodo1.MAS
{
    [Serializable]
    public class Yodo1PlatformSettings
    {
        public string AppKey;
        public string AdmobAppID;
        public string BundleID;

        public Yodo1PlatformSettings()
        {
            this.AppKey = string.Empty;
            this.AdmobAppID = string.Empty;
            this.BundleID = string.Empty;
        }
    }
}



