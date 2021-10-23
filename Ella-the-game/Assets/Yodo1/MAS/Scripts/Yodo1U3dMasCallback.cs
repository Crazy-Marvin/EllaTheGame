using UnityEngine;
using System.Collections.Generic;

namespace Yodo1.MAS
{
    public class Yodo1U3dMasCallback : MonoBehaviour
    {
        private const int FLAG_INITIALIZE = 0;
        private const int FLAG_AD_EVENT = 1;

        private const int EVENT_INITIALIZE_FAILURE = 0;
        private const int EVENT_INITIALIZE_SUCCESS = 1;

        public enum AdType
        {
            Rewarded = 1,
            Interstitial = 2,
            Banner = 3,
        }

        public static Yodo1U3dMasCallback Instance { get; private set; }

        public string SdkMethodName
        {
            get
            {
                return "Yodo1U3dMasCallbackResult";
            }
        }

        public string SdkObjectName
        {
            get
            {
                return gameObject.name;
            }
        }

        private static Yodo1U3dMas.InitializeDelegate _initializeDelegate;
        public static void SetInitializeDelegate(Yodo1U3dMas.InitializeDelegate initializeDelegate)
        {
            _initializeDelegate = initializeDelegate;
        }

        #region Ad Delegate

        //InterstitialAd of delegate
        private static Yodo1U3dMas.InterstitialAdDelegate _interstitialAdDelegate;
        public static void SetInterstitialAdDelegate(Yodo1U3dMas.InterstitialAdDelegate interstitialAdDelegate)
        {
            _interstitialAdDelegate = interstitialAdDelegate;
        }

        //ShowBanner of delegate
        private static Yodo1U3dMas.BannerdAdDelegate _bannerDelegate;
        public static void SetBannerAdDelegate(Yodo1U3dMas.BannerdAdDelegate bannerDelegate)
        {
            _bannerDelegate = bannerDelegate;
        }

        //RewardVideo of delegate
        private static Yodo1U3dMas.RewardedAdDelegate _rewardedAdDelegate;
        public static void SetRewardedAdDelegate(Yodo1U3dMas.RewardedAdDelegate rewardedAdDelegate)
        {
            _rewardedAdDelegate = rewardedAdDelegate;
        }

        #endregion

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        public void Yodo1U3dMasCallbackResult(string result)
        {
            Debug.Log("[Yodo1 Mas] The SDK callback result:" + result + "\n");

            Dictionary<string, object> obj = (Dictionary<string, object>)Yodo1JSON.Deserialize(result);
            if (obj == null)
            {
                return;
            }

            if (!obj.ContainsKey("flag") || !obj.ContainsKey("data"))
            {
                return;
            }

            string jsonData = obj["data"].ToString();
            Dictionary<string, object> dataDic = (Dictionary<string, object>)Yodo1JSON.Deserialize(jsonData);
            if (dataDic == null)
            {
                return;
            }

            int flag = int.Parse(obj["flag"].ToString());
            if (flag == FLAG_INITIALIZE)
            {
                bool success = false;
                Yodo1U3dAdError error = new Yodo1U3dAdError();

                if (dataDic.ContainsKey("success"))
                {
                    success = int.Parse(dataDic["success"].ToString()) == EVENT_INITIALIZE_SUCCESS ? true : false;
                }
                if (dataDic.ContainsKey("error"))
                {
                    string errorStr = dataDic["error"].ToString();
                    error = Yodo1U3dAdError.createWithJson(errorStr);
                }

                if (_initializeDelegate != null)
                {
                    _initializeDelegate(success, error);
                }
            }
            else if (flag == FLAG_AD_EVENT)
            {
                AdType type = AdType.Rewarded;
                if (dataDic.ContainsKey("type"))
                {
                    type = (AdType)int.Parse(dataDic["type"].ToString());
                }
                else
                {
                    return;
                }
                Yodo1U3dAdEvent eventCode = Yodo1U3dAdEvent.AdError;
                if (dataDic.ContainsKey("code"))
                {
                    eventCode = (Yodo1U3dAdEvent)int.Parse(dataDic["code"].ToString());
                }
                string message;
                if (dataDic.ContainsKey("message"))
                {
                    message = dataDic["message"].ToString();
                }
                Yodo1U3dAdError error = new Yodo1U3dAdError();
                if (dataDic.ContainsKey("error"))
                {
                    error = Yodo1U3dAdError.createWithJson(dataDic["error"].ToString());
                }

                switch (type)
                {
                    case AdType.Rewarded:
                        {
                            if (_rewardedAdDelegate != null)
                            {
                                _rewardedAdDelegate(eventCode, error);
                            }
                        }
                        break;
                    case AdType.Interstitial:
                        if (_interstitialAdDelegate != null)
                        {
                            _interstitialAdDelegate(eventCode, error);
                        }
                        break;
                    case AdType.Banner:
                        if (_bannerDelegate != null)
                        {
                            _bannerDelegate(eventCode, error);
                        }
                        break;
                    default:
                        break;
                }
            }
        }
    }
}