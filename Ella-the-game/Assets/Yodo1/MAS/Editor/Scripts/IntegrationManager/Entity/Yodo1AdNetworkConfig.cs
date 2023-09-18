using System;
using System.Collections.Generic;
using UnityEngine;

namespace Yodo1.MAS
{
    [Serializable]
    public class Yodo1AdNetworkConfig : ISerializationCallbackReceiver
    {

        public string sdkVersion;
        public string latestSdkversion;
        public string sdkDownloadUrl;
        public Yodo1AdNetwork[] android;
        public Yodo1AdNetwork[] ios;
        public List<Yodo1AdNetwork> androidStandard;
        public List<Yodo1AdNetwork> androidFamily;
        public List<Yodo1AdNetwork> iosStandard;

        public Yodo1AdNetworkConfig()
        {
        }

        private void FilterNetworksBySDKGroup()
        {
            if(android != null && android.Length > 0)
            {
                androidStandard = new List<Yodo1AdNetwork>();
                androidFamily = new List<Yodo1AdNetwork>();
                foreach (Yodo1AdNetwork network in android)
                {
                    if(network == null || network.supported == null || network.supported.Length == 0)
                    {
                        return;
                    }

                    foreach(int temp in network.supported)
                    {
                        if((int)SDKGroupType.AndroidStandard == temp) {
                            androidStandard.Add(network);
                        }
                        else if((int)SDKGroupType.AndroidFamily == temp)
                        {
                            androidFamily.Add(network);
                        }

                    }
                }
            }

            if(ios != null && ios.Length > 0)
            {
                iosStandard = new List<Yodo1AdNetwork>();
                foreach (Yodo1AdNetwork network in ios)
                {
                    if (network == null || network.supported == null || network.supported.Length == 0)
                    {
                        return;
                    }

                    foreach (int temp in network.supported)
                    {
                        if ((int)SDKGroupType.IOSStandard == temp)
                        {
                            iosStandard.Add(network);
                        }
                    }
                }
            }
        }

        public void OnAfterDeserialize()
        {
            FilterNetworksBySDKGroup();
        }

        public void OnBeforeSerialize()
        {
        }
    }

    public enum SDKGroupType : int
    {
        AndroidStandard = 0,
        AndroidFamily = 1,
        IOSStandard = 2,
    };
}
