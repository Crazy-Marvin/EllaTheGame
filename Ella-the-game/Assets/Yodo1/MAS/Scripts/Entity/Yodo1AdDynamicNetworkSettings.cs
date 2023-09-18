using System;
using UnityEngine;
using System.Collections.Generic;

namespace Yodo1.MAS
{
    [Serializable]
    public class Yodo1AdDynamicNetworkSettings
    {
        public int sdkType;
        public string sdkVersion;
        public string latestSdkVersion;
        public List<string> networks;

        public Yodo1AdDynamicNetworkSettings()
        {
            this.sdkType = -1;
            this.sdkVersion = string.Empty;
            this.latestSdkVersion = string.Empty;
            this.networks = new List<string>();
        }
    }
}



