using System;
using System.Collections.Generic;

namespace Yodo1.MAS
{
    /**
     * The instance of this class is used to record the network information selected by the developer
     */
    public class Yodo1AdNetworkConfigCacheData
    {

        public SDKGroupType sdkType;
        public string sdkVersion;
        public string latestSdkVersion;
        public List<string> networks;

        public Yodo1AdNetworkConfigCacheData()
        {
        }

        public Yodo1AdNetworkConfigCacheData(SDKGroupType sdkType, string sdkVersion, string latestSdkVersion, List<string> networks)
        {
            this.sdkType = sdkType;
            this.sdkVersion = sdkVersion;
            this.latestSdkVersion = latestSdkVersion;
            this.networks = networks;
        }
    }
}
