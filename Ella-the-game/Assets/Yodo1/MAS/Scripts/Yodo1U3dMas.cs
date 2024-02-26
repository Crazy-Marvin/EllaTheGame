using System;
using UnityEngine;

namespace Yodo1.MAS
{
    public class Yodo1U3dAttrackingStatus
    {
        public const int NotDetermined = 0;
        public const int Restricted = 1;
        public const int Denied = 2;
        public const int Authorized = 3;
        public const int SystemLow = -1;
    }

    public class Yodo1U3dInitializeInfo
    {
        public const int UserAge = 1;
        public const int AttrackingStatus = 2;
    }

    public class Yodo1U3dMas
    {
        public static readonly string TAG = "[Yodo1Mas] ";

        private static Yodo1AdBuildConfig adBuildConfig = null;

        public delegate void InitializeDelegate(bool success, Yodo1U3dAdError error);
        [System.Obsolete("Please use `Yodo1U3dMasCallback.OnSdkInitializedEvent` instead.\n" +
            "Yodo1U3dMasCallback.OnSdkInitializedEvent += (success, error) => { };", true)]
        public static void SetInitializeDelegate(InitializeDelegate initializeDelegate) { }

        #region Ad Delegates

        //InterstitialAd of delegate
        public delegate void InterstitialAdDelegate(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError adError);
        [System.Obsolete("Please use `Yodo1U3dMasCallback.Interstitial` instead.\n" +
            "Yodo1U3dMasCallback.Interstitial.OnAdOpenedEvent += OnInterstitialAdOpenedEvent;\n" +
            "Yodo1U3dMasCallback.Interstitial.OnAdClosedEvent += OnInterstitialAdClosedEvent;\n" +
            "Yodo1U3dMasCallback.Interstitial.OnAdErrorEvent += OnInterstitialAdErorEvent;", true)]
        public static void SetInterstitialAdDelegate(InterstitialAdDelegate interstitialAdDelegate) { }

        //BannerAd of delegate
        public delegate void BannerdAdDelegate(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError error);
        [System.Obsolete("Please use ad event in `Yodo1U3dBannerAdView` instead", true)]
        public static void SetBannerAdDelegate(BannerdAdDelegate bannerAdDelegate) { }

        //RewardVideo of delegate
        public delegate void RewardedAdDelegate(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError error);
        [System.Obsolete("Please use `Yodo1U3dMasCallback.Rewarded` instead.\n" +
            "Yodo1U3dMasCallback.Rewarded.OnAdOpenedEvent += OnRewardedAdOpenedEvent;\n" +
            "Yodo1U3dMasCallback.Rewarded.OnAdClosedEvent += OnRewardedAdClosedEvent;\n" +
            "Yodo1U3dMasCallback.Rewarded.OnAdErrorEvent += OnRewardedAdErorEvent;\n" +
            "Yodo1U3dMasCallback.Rewarded.OnAdReceivedRewardEvent += OnAdReceivedRewardEvent;", true)]
        public static void SetRewardedAdDelegate(RewardedAdDelegate rewardedAdDelegate) { }

        #endregion

        /// <summary>
        /// Initialize the default instance of Yodo1 MAS SDK.
        /// </summary>
        [Obsolete("Please use Yodo1U3dMas.InitializeMasSdk()", true)]
        public static void InitializeSdk() { }

        /// <summary>
        /// Initialize the default instance of Yodo1 MAS SDK.
        /// </summary>
        public static void InitializeMasSdk()
        {
            if (adBuildConfig == null)
            {
                Yodo1AdBuildConfig buildConfig = new Yodo1AdBuildConfig();
                SetAdBuildConfig(buildConfig);
            }
            string appKey = _InitializeSdk();
            if (appKey != null)
            {
                Yodo1U3dMas.InitMasWithAppKey(appKey);
            }
        }

        private static string _InitializeSdk()
        {
            if (Yodo1U3dMasCallback.isInitialized())
            {
                Debug.LogWarning(Yodo1U3dMas.TAG + "The SDK has been initialized, please do not initialize the SDK repeatedly.");
                return null;
            }

            var type = typeof(Yodo1U3dMasCallback);
            var sdkObj = new GameObject("Yodo1U3dMasCallback", type).GetComponent<Yodo1U3dMasCallback>(); // Its Awake() method sets Instance.
            if (Yodo1U3dMasCallback.Instance != sdkObj)
            {
                Debug.LogError(Yodo1U3dMas.TAG + "It looks like you have the " + type.Name + " on a GameObject in your scene. Please remove the script from your scene.");
                return null;
            }

            Yodo1AdSettings settings = Resources.Load("Yodo1/Yodo1AdSettings", typeof(Yodo1AdSettings)) as Yodo1AdSettings;
            if (settings == null)
            {
                Debug.LogError(Yodo1U3dMas.TAG + "The SDK has not been initialized yet. The Yodo1AdSettings is missing.");
                return null;
            }

            string appKey = string.Empty;
#if UNITY_ANDROID
            appKey = settings.androidSettings.AppKey.Trim();
#elif UNITY_IOS
            appKey = settings.iOSSettings.AppKey.Trim();
#endif
            Debug.Log(Yodo1U3dMas.TAG + "The SDK is initializing, the app key is " + appKey);

#if UNITY_EDITOR
            Yodo1EditorAds.InitializeAds();
            Yodo1U3dMasCallback.ForwardEvent("onSdkInitializedEvent");
#endif
            Yodo1U3dMasCallback.PrintAutoGameInfo();
            return appKey;
        }

        /// <summary>
        /// Initialize with app key.
        /// </summary>
        /// <param name="appKey">The app key obtained from MAS Developer Platform.</param>
        static void InitMasWithAppKey(string appKey)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.InitMasWithAppKey(appKey);
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.InitMasWithAppKey(appKey);
#endif
            }
        }


        public static void ShowPopupToReportAd()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.ShowPopupToReportAd();
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.ShowPopupToReportAd();
#endif
            }
        }

        public static void ShowDebugger()
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.ShowDebugger();
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.ShowDebugger();
#endif
            }
        }

        #region Privacy Methods
        /// <summary>
        /// MAS SDK requires that publishers set a flag indicating whether a user located in the European Economic Area (i.e., EEA/GDPR data subject) has provided opt-in consent for the collection and use of personal data.
        /// If the user has consented, please set the flag to true. If the user has not consented, please set the flag to false.
        ///
        /// This method must be called before InitializeSdk method.
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

        public static bool IsGDPRUserConsent()
        {
            bool ret = false;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                ret = Yodo1U3dAdsIOS.IsGDPRUserConsent();
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                ret = Yodo1U3dAdsAndroid.IsGDPRUserConsent();
#endif
            }
            return ret;
        }

        /// <summary>
        /// To ensure COPPA, GDPR, and Google Play policy compliance, you should indicate whether a user is a child.
        /// If the user is known to be in an age-restricted category (i.e., under the age of 13) please set the flag to true.
        /// If the user is known to not be in an age-restricted category (i.e., age 13 or older) please set the flag to false.
        ///
        /// This method must be called before InitializeSdk method.
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

        public static bool IsCOPPAAgeRestricted()
        {
            bool ret = false;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                ret = Yodo1U3dAdsIOS.IsCOPPAAgeRestricted();
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                ret = Yodo1U3dAdsAndroid.IsCOPPAAgeRestricted();
#endif
            }
            return ret;
        }

        /// <summary>
        /// Publishers may choose to display a "Do Not Sell My Personal Information" link.
        /// Such publishers may choose to set a flag indicating whether a user located in California, USA has opted to not have their personal data sold.
        ///
        /// This method must be called before InitializeSdk method.
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

        public static bool IsCCPADoNotSell()
        {
            bool ret = false;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                ret = Yodo1U3dAdsIOS.IsCCPADoNotSell();
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                ret = Yodo1U3dAdsAndroid.IsCCPADoNotSell();
#endif
            }
            return ret;
        }
        #endregion

        #region Privacy Method China
        /// <summary>
        /// According to Chinese data protection laws, testimonial advertising and commercial marketing to Chinese users through automated decision-making methods should be accompanied by options that do not target their personal characteristics or provide Chinese users with a way to reject them.
        /// Based on the above requirements, You must set the user's choice through the 'setPersonalizedState' API,
        /// through which you will send MAS information about that user has refused to the option based on his personal characteristics through automated decision-making.
        /// If set to 'Yes', MAS will no longer provide the user with personalized information based on its persistent identifiers.
        ///
        /// This method must be called before InitializeSdk method.
        /// </summary>
        /// <param name="disablePersonal"></param>
        public static void SetPersonalizedState(bool disablePersonal)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE

#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.SetPersonalizedState(disablePersonal);
#endif
            }
        }
        #endregion

        public static int GetUserAge()
        {
            int age = 0;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                age = Yodo1U3dAdsIOS.GetUserAge();
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                age = Yodo1U3dAdsAndroid.GetUserAge();
#endif
            }
            return age;
        }

        public static int GetAttrackingStatus()
        {
            int status = Yodo1U3dAttrackingStatus.NotDetermined;
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                status = Yodo1U3dAdsIOS.GetAttrackingStatus();
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Debug.LogWarning("Obtaining is not supported on the Android platform");
#endif
            }
            return status;
        }

        #region AdBuildConfig
        public static void SetAdBuildConfig(Yodo1AdBuildConfig yodo1AdBuildConfig)
        {
            if (yodo1AdBuildConfig == null)
            {
                return;
            }

            adBuildConfig = yodo1AdBuildConfig;

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

        /// <summary>
        /// Pausing game when interstitial or reward video ads are playing. The default value is True.
        ///
        /// Note:
        /// 1. Pausing audios, the pausing of audios in game is only possible if the developers have used Unity's default audio source.
        /// If the developers have handled audio separately, pausing of audios will not happen, and the developers will have to handle game pause according to their code.
        /// 2. Pausing of physics, animations etc will function normally.
        /// </summary>
        /// <param name="pauseGame"><c>true</c>, the game will be paused when the interstitial or reward video ads are playing, <c>false</c> otherwise.</param>
        public static void SetAutoPauseGame(bool autoPauseGame)
        {
            Yodo1U3dMasCallback.SetAutoPauseGame(autoPauseGame);
        }

        #region Banner Ad Methods
        /// <summary>
        /// Whether the banner ads have been loaded.
        /// </summary>
        /// <returns><c>true</c>, if the banner ads have been loaded, <c>false</c> otherwise.</returns>
        //[System.Obsolete("After the ShowBanner method is called, the banner ad will be displayed automatically when it is loaded successfully", true)]
        [System.Obsolete("Please use `Yodo1U3dBannerAdView` instead", true)]
        public static bool IsBannerAdLoaded()
        {
            return false;
        }

        /// <summary>
        /// Shows the banner ad.
        /// The banner ad will be displayed automatically after loaded.
        /// It is recommended that this method be called after the MAS SDK initialization successful.
        /// </summary>
        [System.Obsolete("Please use `Yodo1U3dBannerAdView` instead", true)]
        public static void ShowBannerAd() { }

        /// <summary>
        /// Shows the banner ad.
        /// The banner ad will be displayed automatically after loaded.
        /// It is recommended that this method be called after the MAS SDK initialization successful.
        /// </summary>
        /// <param name="placementId">The ad placement</param>
        [System.Obsolete("Please use `Yodo1U3dBannerAdView` instead", true)]
        public static void ShowBannerAd(string placementId) { }

        /// <summary>
        /// Shows the banner ad.
        /// The banner ad will be displayed automatically after loaded.
        /// It is recommended that this method be called after the MAS SDK initialization successful.
        /// </summary>
        /// <param name="align">The banner ad alignment</param>
        [System.Obsolete("Please use `Yodo1U3dBannerAdView` instead", true)]
        public static void ShowBannerAd(int align) { }

        /// <summary>
        /// Shows the banner ad.
        /// The banner ad will be displayed automatically after loaded.
        /// It is recommended that this method be called after the MAS SDK initialization successful.
        /// </summary>
        /// <param name="align">The banner ad position</param>
        /// <param name="offsetX">horizontal offset, offsetX > 0, offset right. offsetX < 0, offset left. if align = Yodo1Mas.BannerLeft, offsetX < 0 is invalid (Only Android)</param>
        /// <param name="offsetY">vertical offset, offsetY > 0, offset bottom. offsetY < 0, offset top.if align = Yodo1Mas.BannerTop, offsetY < 0 is invalid(Only Android)</param>
        [System.Obsolete("Please use `Yodo1U3dBannerAdView` instead", true)]
        public static void ShowBannerAd(int align, int offsetX, int offsetY) { }

        /// <summary>
        /// Shows the banner ad.
        /// The banner ad will be displayed automatically after loaded.
        /// It is recommended that this method be called after the MAS SDK initialization successful.
        /// </summary>
        /// <param name="placementId">The banner ad placement</param>
        /// <param name="align">The banner ad position</param>
        /// <param name="offsetX">horizontal offset, offsetX > 0, the banner will move to the right. offsetX < 0, the banner will move to the left. if align = Yodo1Mas.BannerLeft, offsetX < 0 is invalid (Only Android</param>
        /// <param name="offsetY">vertical offset, offsetY > 0, the banner will move to the bottom. offsetY < 0, the banner will move to the top. if align = Yodo1Mas.BannerTop, offsetY < 0 is invalid(Only Android)</param>
        [System.Obsolete("Please use `Yodo1U3dBannerAdView` instead", true)]
        public static void ShowBannerAd(string placementId, int align, int offsetX, int offsetY) { }

        /// <summary>
        /// Hide banner ads
        /// </summary>
        [System.Obsolete("Please use `Yodo1U3dBannerAdView` instead", true)]
        public static void DismissBannerAd() { }

        /// <summary>
        /// hide banner ads
        /// </summary>
        /// <param name="destroy">if destroy == true, the ads displayed in the next call to showBanner are different. if destroy == false, the ads displayed in the next call to showBanner are same</param>
        [System.Obsolete("Please use `Yodo1U3dBannerAdView` instead", true)]
        public static void DismissBannerAd(bool destroy) { }

        #endregion

        #region Interstitial Ad Methods
        /// <summary>
        /// Whether the interstitial ads have been loaded.
        /// </summary>
        /// <returns><c>true</c>, if the interstitial ads have been loaded complete, <c>false</c> otherwise.</returns>
        [Obsolete("Please use Yodo1U3dInterstitialAd.GetInstance().IsLoaded()", true)]
        public static bool IsInterstitialAdLoaded()
        {
            return false;
        }

        /// <summary>
        /// Shows the interstitial ad.
        /// </summary>
        ///
        [Obsolete("Please use Yodo1U3dInterstitialAd.GetInstance().ShowAd()", true)]
        public static void ShowInterstitialAd() { }

        /// <summary>
        /// Shows the interstitial ad with placement id.
        /// </summary>
        /// <param name="placementId"></param>
        ///
        [Obsolete("Please use Yodo1U3dInterstitialAd.GetInstance().ShowAd(placementId)", true)]
        public static void ShowInterstitialAd(string placementId) { }

        #endregion

        #region Rewarded Ad Methods
        /// <summary>
        /// Whether the reward video ads have been loaded.
        /// </summary>
        /// <returns><c>true</c>, if the reward video ads have been loaded complete, <c>false</c> otherwise.</returns>
        [Obsolete("Please use Yodo1U3dRewardAd.GetInstance().IsLoaded()", true)]
        public static bool IsRewardedAdLoaded()
        {
            return false;
        }

        /// <summary>
        /// Shows the reward video ad.
        /// </summary>
        ///
        [Obsolete("Please use Yodo1U3dRewardAd.GetInstance().ShowAd()", true)]
        public static void ShowRewardedAd() { }

        /// <summary>
        /// Shows the reward video ad.
        /// </summary>
        /// <param name="placementId"></param>
        ///
        [Obsolete("Please use Yodo1U3dRewardAd.GetInstance().ShowAd(placementId)", true)]
        public static void ShowRewardedAd(string placementId) { }

        #endregion
    }
}