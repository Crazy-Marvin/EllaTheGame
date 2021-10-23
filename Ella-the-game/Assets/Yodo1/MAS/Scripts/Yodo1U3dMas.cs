using UnityEngine;

namespace Yodo1.MAS
{
    public class Yodo1U3dMas
    {
        static bool initialized = false;

        public delegate void InitializeDelegate(bool success, Yodo1U3dAdError error);
        public static void SetInitializeDelegate(InitializeDelegate initializeDelegate)
        {
            Yodo1U3dMasCallback.SetInitializeDelegate(initializeDelegate);
        }

        #region Ad Delegate

        //InterstitialAd of delegate
        public delegate void InterstitialAdDelegate(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError adError);
        public static void SetInterstitialAdDelegate(InterstitialAdDelegate interstitialAdDelegate)
        {
            Yodo1U3dMasCallback.SetInterstitialAdDelegate(interstitialAdDelegate);
        }

        //ShowBanner of delegate
        public delegate void BannerdAdDelegate(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError error);
        public static void SetBannerAdDelegate(BannerdAdDelegate bannerAdDelegate)
        {
            Yodo1U3dMasCallback.SetBannerAdDelegate(bannerAdDelegate);
        }

        //RewardVideo of delegate
        public delegate void RewardedAdDelegate(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError error);
        public static void SetRewardedAdDelegate(RewardedAdDelegate rewardedAdDelegate)
        {
            Yodo1U3dMasCallback.SetRewardedAdDelegate(rewardedAdDelegate);
        }

        #endregion

        public static void InitializeSdk()
        {
            if (initialized)
            {
                Debug.LogWarning("[Yodo1 Mas] The SDK has been initialized, please do not initialize the SDK repeatedly.");
                return;
            }

            var type = typeof(Yodo1U3dMasCallback);
            var sdkObj = new GameObject("Yodo1U3dMasCallback", type).GetComponent<Yodo1U3dMasCallback>(); // Its Awake() method sets Instance.
            if (Yodo1U3dMasCallback.Instance != sdkObj)
            {
                Debug.LogError("[Yodo1 Mas] It looks like you have the " + type.Name + " on a GameObject in your scene. Please remove the script from your scene.");
                return;
            }

            Yodo1AdSettings settings = Resources.Load("Yodo1/Yodo1AdSettings", typeof(Yodo1AdSettings)) as Yodo1AdSettings;
            if (settings == null)
            {
                Debug.LogError("[Yodo1 Mas] The SDK has not been initialized yet. The Yodo1AdSettings is missing.");
                return;
            }

            string appKey = string.Empty;
#if UNITY_ANDROID
            appKey = settings.androidSettings.AppKey.Trim();
#elif UNITY_IOS
            appKey = settings.iOSSettings.AppKey.Trim();
#endif
            Debug.Log("[Yodo1 Mas] The SDK has been initialized, the app key is " + appKey);
            Yodo1U3dMas.InitWithAppKey(appKey);

            initialized = true;
        }

        /// <summary>
        /// Initialize with app key.
        /// </summary>
        /// <param name="appKey">The app key obtained from MAS Developer Platform.</param>
        static void InitWithAppKey(string appKey)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.InitWithAppKey(appKey);
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.InitWithAppKey(appKey);
#endif
            }
        }

        #region Privacy
        /// <summary>
        /// MAS SDK requires that publishers set a flag indicating whether a user located in the European Economic Area (i.e., EEA/GDPR data subject) has provided opt-in consent for the collection and use of personal data. If the user has consented, please set the flag to true. If the user has not consented, please set the flag to false.
        /// </summary>
        /// <param name="consent"></param>
        public static void SetGDPR(bool consent)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.SetUserConsent(consent);
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.SetUserConsent(consent);
#endif
            }
        }

        /// <summary>
        /// To ensure COPPA, GDPR, and Google Play policy compliance, you should indicate whether a user is a child. If the user is known to be in an age-restricted category (i.e., under the age of 13) please set the flag to true. If the user is known to not be in an age-restricted category (i.e., age 13 or older) please set the flag to false.
        /// </summary>
        /// <param name="underAgeOfConsent"></param>
        public static void SetCOPPA(bool underAgeOfConsent)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.SetTagForUnderAgeOfConsent(underAgeOfConsent);
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.SetTagForUnderAgeOfConsent(underAgeOfConsent);
#endif
            }
        }

        /// <summary>
        /// Publishers may choose to display a "Do Not Sell My Personal Information" link. Such publishers may choose to set a flag indicating whether a user located in California, USA has opted to not have their personal data sold.
        /// </summary>
        /// <param name="doNotSell"></param>
        public static void SetCCPA(bool doNotSell)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.SetDoNotSell(doNotSell);
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.SetDoNotSell(doNotSell);
#endif
            }
        }
        #endregion

        #region AdBuildConfig
        public static void SetAdBuildConfig(Yodo1AdBuildConfig yodo1AdBuildConfig)
        {
            if (yodo1AdBuildConfig == null)
            {
                return;
            }
            string configStr = yodo1AdBuildConfig.toJson();

            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.SetAdBuildConfig(configStr);
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.SetAdBuildConfig(configStr);
#endif
            }
        }
        #endregion

        #region Banner
        /// <summary>
        /// Whether the banner ads have been loaded.
        /// </summary>
        /// <returns><c>true</c>, if the banner ads have been loaded, <c>false</c> otherwise.</returns>
        public static bool IsBannerAdLoaded()
        {
            if (!initialized)
            {
                Debug.LogError("[Yodo1 Mas] The SDK has not been initialized yet. Please initialize the SDK first.");
                return false;
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                return Yodo1U3dAdsIOS.IsBannerAdLoaded();
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                return Yodo1U3dAdsAndroid.IsBannerAdLoaded();
#endif
            }
            return false;
        }

        /// <summary>
        /// Shows the banner ad.
        /// </summary>
        public static void ShowBannerAd()
        {
            if (!initialized)
            {
                Debug.LogError("[Yodo1 Mas] The SDK has not been initialized yet. Please initialize the SDK first.");
                return;
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.ShowBannerAd();
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.ShowBannerAd();
#endif
            }
        }

        /// <summary>
        /// Shows the banner ad.
        /// </summary>
        /// <param name="placementId">The ad placement</param>
        public static void ShowBannerAd(string placementId)
        {
            if (!initialized)
            {
                Debug.LogError("[Yodo1 Mas] The SDK has not been initialized yet. Please initialize the SDK first.");
                return;
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.ShowBannerAd(placementId);
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.ShowBannerAd(placementId);
#endif
            }
        }

        /// <summary>
        /// Shows the banner ad.
        /// </summary>
        /// <param name="align">banner alignment</param>
        public static void ShowBannerAd(int align)
        {
            if (!initialized)
            {
                Debug.LogError("[Yodo1 Mas] The SDK has not been initialized yet. Please initialize the SDK first.");
                return;
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.ShowBannerAd(align);
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.ShowBannerAd(align);
#endif
            }
        }

        /// <summary>
        /// Shows the banner ad.
        /// </summary>
        /// <param name="align">banner alignment</param>
        /// <param name="offsetX">horizontal offset, offsetX > 0, offset right. offsetX < 0, offset left. if align = Yodo1Mas.BannerLeft, offsetX < 0 is invalid (Only Android)</param>
        /// <param name="offsetY">vertical offset, offsetY > 0, offset bottom. offsetY < 0, offset top.if align = Yodo1Mas.BannerTop, offsetY < 0 is invalid(Only Android)</param>
        public static void ShowBannerAd(int align, int offsetX, int offsetY)
        {
            if (!initialized)
            {
                Debug.LogError("[Yodo1 Mas] The SDK has not been initialized yet. Please initialize the SDK first.");
                return;
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.ShowBannerAd(align, offsetX, offsetY);
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.ShowBannerAd(align, offsetX, offsetY);
#endif
            }
        }

        /// <summary>
        /// Shows the banner ad.
        /// </summary>
        /// <param name="placementId">The ad placement</param>
        /// <param name="align">banner alignment</param>
        /// <param name="offsetX">horizontal offset, offsetX > 0, the banner will move to the right. offsetX < 0, the banner will move to the left. if align = Yodo1Mas.BannerLeft, offsetX < 0 is invalid (Only Android</param>
        /// <param name="offsetY">vertical offset, offsetY > 0, the banner will move to the bottom. offsetY < 0, the banner will move to the top. if align = Yodo1Mas.BannerTop, offsetY < 0 is invalid(Only Android)</param>
        public static void ShowBannerAd(string placementId, int align, int offsetX, int offsetY)
        {
            if (!initialized)
            {
                Debug.LogError("[Yodo1 Mas] The SDK has not been initialized yet. Please initialize the SDK first.");
                return;
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.ShowBannerAd(placementId, align, offsetX, offsetY);
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.ShowBannerAd(placementId, align, offsetX, offsetY);
#endif
            }
        }

        public static void DismissBannerAd()
        {
            if (!initialized)
            {
                Debug.LogError("[Yodo1 Mas] The SDK has not been initialized yet. Please initialize the SDK first.");
                return;
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.DismissBannerAd();
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.DismissBannerAd();
#endif
            }
        }

        /// <summary>
        /// hide banner ads
        /// </summary>
        /// <param name="destroy">if destroy == true, the ads displayed in the next call to showBanner are different. if destroy == false, the ads displayed in the next call to showBanner are same</param>
        public static void DismissBannerAd(bool destroy)
        {
            if (!initialized)
            {
                Debug.LogError("[Yodo1 Mas] The SDK has not been initialized yet. Please initialize the SDK first.");
                return;
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.DismissBannerAd(destroy);
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.DismissBannerAd(destroy);
#endif
            }
        }

        #endregion

        #region Interstitial
        /// <summary>
        /// Whether the interstitial ads have been loaded.
        /// </summary>
        /// <returns><c>true</c>, if the interstitial ads have been loaded complete, <c>false</c> otherwise.</returns>
        public static bool IsInterstitialAdLoaded()
        {
            if (!initialized)
            {
                Debug.LogError("[Yodo1 Mas] The SDK has not been initialized yet. Please initialize the SDK first.");
                return false;
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                return Yodo1U3dAdsIOS.IsInterstitialLoaded();
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                return Yodo1U3dAdsAndroid.IsInterstitialLoaded();
#endif
            }
            return false;
        }

        /// <summary>
        /// Shows the interstitial ad.
        /// </summary>
        public static void ShowInterstitialAd()
        {
            if (!initialized)
            {
                Debug.LogError("[Yodo1 Mas] The SDK has not been initialized yet. Please initialize the SDK first.");
                return;
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.ShowInterstitialAd();
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.ShowInterstitialAd();
#endif
            }
        }

        /// <summary>
        /// Shows the interstitial ad with placement id.
        /// </summary>
        /// <param name="placementId"></param>
        public static void ShowInterstitialAd(string placementId)
        {
            if (!initialized)
            {
                Debug.LogError("[Yodo1 Mas] The SDK has not been initialized yet. Please initialize the SDK first.");
                return;
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.ShowInterstitialAd(placementId);
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.ShowInterstitialAd(placementId);
#endif
            }
        }

        #endregion

        #region Rewarded Video
        /// <summary>
        /// Whether the reward video ads have been loaded.
        /// </summary>
        /// <returns><c>true</c>, if the reward video ads have been loaded complete, <c>false</c> otherwise.</returns>
        public static bool IsRewardedAdLoaded()
        {
            if (!initialized)
            {
                Debug.LogError("[Yodo1 Mas] The SDK has not been initialized yet. Please initialize the SDK first.");
                return false;
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                return Yodo1U3dAdsIOS.IsRewardedAdLoaded();
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                return Yodo1U3dAdsAndroid.IsRewardedAdLoaded();
#endif
            }
            return false;
        }

        /// <summary>
        /// Shows the reward video ad.
        /// </summary>
        public static void ShowRewardedAd()
        {
            if (!initialized)
            {
                Debug.LogError("[Yodo1 Mas] The SDK has not been initialized yet. Please initialize the SDK first.");
                return;
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.ShowRewardedAd();
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.ShowRewardedAd();
#endif
            }
        }

        /// <summary>
        /// Shows the reward video ad.
        /// </summary>
        /// <param name="placementId"></param>
        public static void ShowRewardedAd(string palcementId)
        {
            if (!initialized)
            {
                Debug.LogError("[Yodo1 Mas] The SDK has not been initialized yet. Please initialize the SDK first.");
                return;
            }
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.ShowRewardedAd(palcementId);
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.ShowRewardedAd(palcementId);
#endif
            }
        }
        #endregion
    }
}