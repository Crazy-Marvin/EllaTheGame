using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using EasyMobile.Internal;

namespace EasyMobile
{
    [AddComponentMenu("")]
    public class Advertising : MonoBehaviour
    {
        public static Advertising Instance { get; private set; }

        // Supported ad clients.
        private static AdColonyClientImpl sAdColonyClient;
        private static AdMobClientImpl sAdMobClient;
        private static AppLovinClientImpl sAppLovinClient;
        private static ChartboostClientImpl sChartboostClient;
        private static AudienceNetworkClientImpl sAudienceNetworkClient;
        private static FairBidClientImpl sFairBidClient;
        private static MoPubClientImpl sMoPubClient;
        private static IronSourceClientImpl sIronSourceClient;
        private static TapjoyClientImpl sTapjoyClient;
        private static UnityAdsClientImpl sUnityAdsClient;
        private static VungleClientImpl sVungleClient;

        // Default ad clients for each ad types.
        private static AdClientImpl sDefaultBannerAdClient;
        private static AdClientImpl sDefaultInterstitialAdClient;
        private static AdClientImpl sDefaultRewardedAdClient;

        // For storing removeAds status.
        private const string AD_REMOVE_STATUS_PPKEY = "EM_REMOVE_ADS";
        private const int AD_ENABLED = 1;
        private const int AD_DISABLED = -1;

        // Auto load ads.
        private static readonly float DEFAULT_TIMESTAMP = -1000;
        private static IEnumerator autoLoadAdsCoroutine;
        private static AutoAdLoadingMode currentAutoLoadAdsMode = AutoAdLoadingMode.None;
        private static float lastDefaultInterstitialAdLoadTimestamp = DEFAULT_TIMESTAMP;
        private static float lastDefaultRewardedAdLoadTimestamp = DEFAULT_TIMESTAMP;
        private static Dictionary<string, float> lastCustomInterstitialAdsLoadTimestamp = new Dictionary<string, float>();
        private static Dictionary<string, float> lastCustomRewardedAdsLoadTimestamp = new Dictionary<string, float>();

        /// <summary>
        /// Since <see cref="AutoLoadAdsMode"/> can be changed in both <see cref="Update"/> method and from outside script,
        /// there are chances that its value will be updated twice. We use this flag to prevent that. 
        /// </summary>
        private static bool isUpdatingAutoLoadMode = false;

        // Currently shown banner ads.
        private static Dictionary<AdNetwork, List<AdPlacement>> activeBannerAds = new Dictionary<AdNetwork, List<AdPlacement>>();

        #region Ad Events

        /// <summary>
        /// Occurs when an interstitial ad is closed.
        /// </summary>
        public static event Action<InterstitialAdNetwork, AdPlacement> InterstitialAdCompleted;

        /// <summary>
        /// Occurs when a rewarded ad is skipped (the user didn't complete watching
        /// the ad and therefore is not entitled to the reward).
        /// </summary>
        public static event Action<RewardedAdNetwork, AdPlacement> RewardedAdSkipped;

        /// <summary>
        /// Occurs when a rewarded ad completed and the user should be rewarded.
        /// </summary>
        public static event Action<RewardedAdNetwork, AdPlacement> RewardedAdCompleted;

        /// <summary>
        /// Occurs when ads have been removed.
        /// </summary>
        public static event Action AdsRemoved;

        #endregion

        #region Ad Clients

        /// <summary>
        /// Gets the AdColony client.
        /// </summary>
        /// <value>The ad colony client.</value>
        public static AdColonyClientImpl AdColonyClient
        {
            get
            {
                if (sAdColonyClient == null)
                    sAdColonyClient = SetupAdClient(AdNetwork.AdColony) as AdColonyClientImpl;
                return sAdColonyClient;
            }
        }

        /// <summary>
        /// Gets the AdMob client.
        /// </summary>
        /// <value>The ad mob client.</value>
        public static AdMobClientImpl AdMobClient
        {
            get
            {
                if (sAdMobClient == null)
                    sAdMobClient = SetupAdClient(AdNetwork.AdMob) as AdMobClientImpl;
                return sAdMobClient;
            }
        }

        /// <summary>
        /// Gets the AppLovin client.
        /// </summary>
        /// <value>The AppLovin client.</value>
        public static AppLovinClientImpl AppLovinClient
        {
            get
            {
                if (sAppLovinClient == null)
                    sAppLovinClient = SetupAdClient(AdNetwork.AppLovin) as AppLovinClientImpl;
                return sAppLovinClient;
            }
        }


        /// <summary>
        /// Gets the Chartboost client.
        /// </summary>
        /// <value>The chartboost client.</value>
        public static ChartboostClientImpl ChartboostClient
        {
            get
            {
                if (sChartboostClient == null)
                    sChartboostClient = SetupAdClient(AdNetwork.Chartboost) as ChartboostClientImpl;
                return sChartboostClient;
            }
        }

        /// <summary>
        /// Gets the Facebook Audience Network client.
        /// </summary>
        /// <value>The audience network client.</value>
        public static AudienceNetworkClientImpl AudienceNetworkClient
        {
            get
            {
                if (sAudienceNetworkClient == null)
                    sAudienceNetworkClient = SetupAdClient(AdNetwork.AudienceNetwork) as AudienceNetworkClientImpl;
                return sAudienceNetworkClient;
            }
        }

        /// <summary>
        /// Gets the FairBid client.
        /// </summary>
        /// <value>The FairBid client.</value>
        public static FairBidClientImpl FairBidClient
        {
            get
            {
                if (sFairBidClient == null)
                    sFairBidClient = SetupAdClient(AdNetwork.FairBid) as FairBidClientImpl;
                return sFairBidClient;
            }
        }

        /// <summary>
        /// Gets the MoPub client.
        /// </summary>
        /// <value>The mo pub client.</value>
        public static MoPubClientImpl MoPubClient
        {
            get
            {
                if (sMoPubClient == null)
                    sMoPubClient = SetupAdClient(AdNetwork.MoPub) as MoPubClientImpl;
                return sMoPubClient;
            }
        }

        /// <summary>
        /// Gets the ironSource client.
        /// </summary>
        /// <value>The iron source client.</value>
        public static IronSourceClientImpl IronSourceClient
        {
            get
            {
                if (sIronSourceClient == null)
                    sIronSourceClient = SetupAdClient(AdNetwork.IronSource) as IronSourceClientImpl;
                return sIronSourceClient;
            }
        }

        /// <summary>
        /// Gets the Tapjoy client.
        /// </summary>
        /// <value>The tap joy client.</value>
        public static TapjoyClientImpl TapjoyClient
        {
            get
            {
                if (sTapjoyClient == null)
                    sTapjoyClient = SetupAdClient(AdNetwork.TapJoy) as TapjoyClientImpl;
                return sTapjoyClient;
            }
        }

        /// <summary>
        /// Gets the Unity Ads client.
        /// </summary>
        /// <value>The unity ads client.</value>
        public static UnityAdsClientImpl UnityAdsClient
        {
            get
            {
                if (sUnityAdsClient == null)
                    sUnityAdsClient = SetupAdClient(AdNetwork.UnityAds) as UnityAdsClientImpl;
                return sUnityAdsClient;
            }
        }

        public static VungleClientImpl VungleClient
        {
            get
            {
                if (sVungleClient == null)
                    sVungleClient = SetupAdClient(AdNetwork.Vungle) as VungleClientImpl;
                return sVungleClient;
            }
        }

        private static AdClientImpl DefaultBannerAdClient
        {
            get
            {
                if (sDefaultBannerAdClient == null)
                {
#if UNITY_IOS
                    sDefaultBannerAdClient = GetWorkableAdClient((AdNetwork)EM_Settings.Advertising.IosDefaultAdNetworks.bannerAdNetwork);
#elif UNITY_ANDROID
                    sDefaultBannerAdClient = GetWorkableAdClient((AdNetwork)EM_Settings.Advertising.AndroidDefaultAdNetworks.bannerAdNetwork);
#else
                    sDefaultBannerAdClient = GetWorkableAdClient(AdNetwork.None);
#endif
                }
                return sDefaultBannerAdClient;
            }
        }

        private static AdClientImpl DefaultInterstitialAdClient
        {
            get
            {
                if (sDefaultInterstitialAdClient == null)
                {
#if UNITY_IOS
                    sDefaultInterstitialAdClient = GetWorkableAdClient((AdNetwork)EM_Settings.Advertising.IosDefaultAdNetworks.interstitialAdNetwork);
#elif UNITY_ANDROID
                    sDefaultInterstitialAdClient = GetWorkableAdClient((AdNetwork)EM_Settings.Advertising.AndroidDefaultAdNetworks.interstitialAdNetwork);
#else
                    sDefaultInterstitialAdClient = GetWorkableAdClient(AdNetwork.None);
#endif
                }
                return sDefaultInterstitialAdClient;
            }
        }

        private static AdClientImpl DefaultRewardedAdClient
        {
            get
            {
                if (sDefaultRewardedAdClient == null)
                {
#if UNITY_IOS
                    sDefaultRewardedAdClient = GetWorkableAdClient((AdNetwork)EM_Settings.Advertising.IosDefaultAdNetworks.rewardedAdNetwork);
#elif UNITY_ANDROID
                    sDefaultRewardedAdClient = GetWorkableAdClient((AdNetwork)EM_Settings.Advertising.AndroidDefaultAdNetworks.rewardedAdNetwork);
#else
                    sDefaultRewardedAdClient = GetWorkableAdClient(AdNetwork.None);
#endif
                }
                return sDefaultRewardedAdClient;
            }
        }

        #endregion

        #region MonoBehaviour Events

        void Awake()
        {
            if (Instance != null)
                Destroy(this);
            else
                Instance = this;
        }

        void Start()
        {
            // Show FairBid Test Suite if needed.
            if (EM_Settings.Advertising.FairBid.ShowTestSuite)
                FairBidClient.ShowTestSuite();

            AutoAdLoadingMode = EM_Settings.Advertising.AutoAdLoadingMode;
        }

        void Update()
        {
            // Always track EM_Settings.Advertising.AutoLoadAdsMode so that we can adjust
            // accordingly if it was changed elsewhere.
            if (!isUpdatingAutoLoadMode && currentAutoLoadAdsMode != EM_Settings.Advertising.AutoAdLoadingMode)
            {
                AutoAdLoadingMode = EM_Settings.Advertising.AutoAdLoadingMode;
            }
        }

        #endregion

        #region Consent Management API

        /// <summary>
        /// Raised when the module-level data privacy consent is changed.
        /// </summary>
        public static event Action<ConsentStatus> DataPrivacyConsentUpdated
        {
            add { AdvertisingConsentManager.Instance.DataPrivacyConsentUpdated += value; }
            remove { AdvertisingConsentManager.Instance.DataPrivacyConsentUpdated -= value; }
        }

        /// <summary>
        /// The module-level data privacy consent status, 
        /// default to ConsentStatus.Unknown.
        /// </summary>
        public static ConsentStatus DataPrivacyConsent
        {
            get { return AdvertisingConsentManager.Instance.DataPrivacyConsent; }
        }

        /// <summary>
        /// Grants module-level data privacy consent.
        /// This consent persists across app launches.
        /// </summary>
        /// <remarks>
        /// This method is a wrapper of <c>AdvertisingConsentManager.Instance.GrantDataPrivacyConsent</c>.
        /// </remarks>
        public static void GrantDataPrivacyConsent()
        {
            AdvertisingConsentManager.Instance.GrantDataPrivacyConsent();
        }

        /// <summary>
        /// Grants the provider-level data privacy consent for the specified ad network.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        public static void GrantDataPrivacyConsent(AdNetwork adNetwork)
        {
            // Use GetAdClient so the client won't be initialized (until it's actually used).
            // This is necessary as the consent should be set prior initialization for most networks.
            GetAdClient(adNetwork).GrantDataPrivacyConsent();
        }

        /// <summary>
        /// Revokes the module-level data privacy consent.
        /// This consent persists across app launches.
        /// </summary>
        /// <remarks>
        /// This method is a wrapper of <c>AdvertisingConsentManager.Instance.RevokeDataPrivacyConsent</c>.
        /// </remarks>
        public static void RevokeDataPrivacyConsent()
        {
            AdvertisingConsentManager.Instance.RevokeDataPrivacyConsent();
        }

        /// <summary>
        /// Revokes the provider-level data privacy consent of the specified ad network.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        public static void RevokeDataPrivacyConsent(AdNetwork adNetwork)
        {
            GetAdClient(adNetwork).RevokeDataPrivacyConsent();
        }

        /// <summary>
        /// Gets the data privacy consent status for the specified ad network.
        /// </summary>
        /// <returns>The data privacy consent.</returns>
        /// <param name="adNetwork">Ad network.</param>
        public static ConsentStatus GetDataPrivacyConsent(AdNetwork adNetwork)
        {
            return GetAdClient(adNetwork).DataPrivacyConsent;
        }

        #endregion

        #region Ads API

        //------------------------------------------------------------
        // Auto Ad-Loading.
        //------------------------------------------------------------

        /// <summary>
        /// Gets or sets auto ad-loading mode.
        /// </summary>
        public static AutoAdLoadingMode AutoAdLoadingMode
        {
            get
            {
                return currentAutoLoadAdsMode;
            }
            set
            {
                if (value == currentAutoLoadAdsMode)
                    return;

                isUpdatingAutoLoadMode = true;
                EM_Settings.Advertising.AutoAdLoadingMode = value;
                currentAutoLoadAdsMode = value;
                isUpdatingAutoLoadMode = false;

                if (autoLoadAdsCoroutine != null)
                    Instance.StopCoroutine(autoLoadAdsCoroutine);

                switch (value)
                {
                    case AutoAdLoadingMode.LoadDefaultAds:
                        {
                            autoLoadAdsCoroutine = CRAutoLoadDefaultAds();
                            Instance.StartCoroutine(autoLoadAdsCoroutine);
                            break;
                        }

                    case AutoAdLoadingMode.LoadAllDefinedPlacements:
                        {
                            autoLoadAdsCoroutine = CRAutoLoadAllAds();
                            Instance.StartCoroutine(autoLoadAdsCoroutine);
                            break;
                        }

                    case AutoAdLoadingMode.None:
                    default:
                        autoLoadAdsCoroutine = null;
                        break;
                }
            }
        }

        //------------------------------------------------------------
        // Banner Ads.
        //------------------------------------------------------------

        /// <summary>
        /// Shows the default banner ad at the specified position.
        /// </summary>
        /// <param name="position">Position.</param>
        public static void ShowBannerAd(BannerAdPosition position)
        {
            ShowBannerAd(DefaultBannerAdClient, AdPlacement.Default, position, BannerAdSize.SmartBanner);
        }

        /// <summary>
        /// Shows a banner ad of the default banner ad network
        /// at the specified position and size using the default placement.
        /// </summary>
        /// <param name="position">Position.</param>
        /// <param name="size">Banner ad size.</param>
        public static void ShowBannerAd(BannerAdPosition position, BannerAdSize size)
        {
            ShowBannerAd(DefaultBannerAdClient, AdPlacement.Default, position, size);
        }

        /// <summary>
        /// Shows a banner ad of the specified ad network at the specified position and size
        /// using the default placement.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="position">Position.</param>
        /// <param name="size">Ad size, applicable for AdMob banner only.</param>
        public static void ShowBannerAd(BannerAdNetwork adNetwork, BannerAdPosition position, BannerAdSize size)
        {
            ShowBannerAd(GetWorkableAdClient((AdNetwork)adNetwork), AdPlacement.Default, position, size);
        }

        /// <summary>
        /// Shows the banner ad with the specified parameters.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="placement">Placement.</param>
        /// <param name="position">Position.</param>
        /// <param name="size">Size.</param>
        public static void ShowBannerAd(BannerAdNetwork adNetwork, AdPlacement placement, BannerAdPosition position, BannerAdSize size)
        {
            ShowBannerAd(GetWorkableAdClient((AdNetwork)adNetwork), placement, position, size);
        }

        /// <summary>
        /// Hides the default banner ad.
        /// </summary>
        public static void HideBannerAd()
        {
            HideBannerAd(DefaultBannerAdClient, AdPlacement.Default);
        }

        /// <summary>
        /// Hides the banner ad of the specified ad network at the specified placement.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="placement">Placement. Pass <c>AdPlacement.Default</c> to specify the default placement.</param>
        public static void HideBannerAd(BannerAdNetwork adNetwork, AdPlacement placement)
        {
            HideBannerAd(GetWorkableAdClient((AdNetwork)adNetwork), placement);
        }

        /// <summary>
        /// Destroys the default banner ad.
        /// </summary>
        public static void DestroyBannerAd()
        {
            DestroyBannerAd(DefaultBannerAdClient, AdPlacement.Default);
        }

        /// <summary>
        /// Destroys the banner ad of the specified ad network at the specified placement.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="placement">Placement. Pass <c>AdPlacement.Default</c> to specify the default placement.</param>
        public static void DestroyBannerAd(BannerAdNetwork adNetwork, AdPlacement placement)
        {
            DestroyBannerAd(GetWorkableAdClient((AdNetwork)adNetwork), placement);
        }

        //------------------------------------------------------------
        // Interstitial Ads.
        //------------------------------------------------------------

        /// <summary>
        /// Loads the default interstitial ad.
        /// </summary>
        public static void LoadInterstitialAd()
        {
            LoadInterstitialAd(DefaultInterstitialAdClient, AdPlacement.Default);
        }

        /// <summary>
        /// Loads the interstitial ad of the default interstitial ad network at the specified placement.
        /// </summary>
        /// <param name="placement">Placement.</param>
        public static void LoadInterstitialAd(AdPlacement placement)
        {
            LoadInterstitialAd(DefaultInterstitialAdClient, placement);
        }

        /// <summary>
        /// Loads the interstitial ad of the specified ad network at the specified placement.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="placement">Placement. Pass <c>AdPlacement.Default</c> to specify the default placement.</param>
        public static void LoadInterstitialAd(InterstitialAdNetwork adNetwork, AdPlacement placement)
        {
            LoadInterstitialAd(GetWorkableAdClient((AdNetwork)adNetwork), placement);
        }

        /// <summary>
        /// Determines whether the default interstitial ad is ready to show.
        /// </summary>
        /// <returns><c>true</c> if the ad is ready; otherwise, <c>false</c>.</returns>
        public static bool IsInterstitialAdReady()
        {
            return IsInterstitialAdReady(DefaultInterstitialAdClient, AdPlacement.Default);
        }

        /// <summary>
        /// Determines whether the interstitial ad of the default interstitial ad network 
        /// at the specified placement is ready to show.
        /// </summary>
        /// <returns><c>true</c> if the ad is ready; otherwise, <c>false</c>.</returns>
        /// <param name="placement">Placement.</param>
        public static bool IsInterstitialAdReady(AdPlacement placement)
        {
            return IsInterstitialAdReady(DefaultInterstitialAdClient, placement);
        }

        /// <summary>
        /// Determines whether the interstitial ad of the specified ad network 
        /// at the specified placement is ready to show.
        /// </summary>
        /// <returns><c>true</c> if the ad is ready; otherwise, <c>false</c>.</returns>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="placement">Placement. Pass <c>AdPlacement.Default</c> to specify the default placement.</param>
        public static bool IsInterstitialAdReady(InterstitialAdNetwork adNetwork, AdPlacement placement)
        {
            return IsInterstitialAdReady(GetWorkableAdClient((AdNetwork)adNetwork), placement);
        }

        /// <summary>
        /// Shows the default interstitial ad.
        /// </summary>
        public static void ShowInterstitialAd()
        {
            ShowInterstitialAd(DefaultInterstitialAdClient, AdPlacement.Default);
        }

        /// <summary>
        /// Shows the interstitial ad of the default interstitial ad network at the specified placement.
        /// </summary>
        /// <param name="placement">Placement.</param>
        public static void ShowInterstitialAd(AdPlacement placement)
        {
            ShowInterstitialAd(DefaultInterstitialAdClient, placement);
        }

        /// <summary>
        /// Shows the interstitial ad of the specified ad network at the specified placement.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="placement">Placement. Pass <c>AdPlacement.Default</c> to specify the default placement.</param>
        public static void ShowInterstitialAd(InterstitialAdNetwork adNetwork, AdPlacement placement)
        {
            ShowInterstitialAd(GetWorkableAdClient((AdNetwork)adNetwork), placement);
        }

        //------------------------------------------------------------
        // Rewarded Ads.
        //------------------------------------------------------------

        /// <summary>
        /// Loads the default rewarded ad.
        /// </summary>
        public static void LoadRewardedAd()
        {
            LoadRewardedAd(DefaultRewardedAdClient, AdPlacement.Default);
        }

        /// <summary>
        /// Loads the rewarded ad of the default rewarded ad network at the specified placement.
        /// </summary>
        /// <param name="placement">Placement.</param>
        public static void LoadRewardedAd(AdPlacement placement)
        {
            LoadRewardedAd(DefaultRewardedAdClient, placement);
        }

        /// <summary>
        /// Loads the rewarded ad of the specified ad network at the specified placement.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="placement">Placement. Pass <c>AdPlacement.Default</c> to specify the default placement.</param>
        public static void LoadRewardedAd(RewardedAdNetwork adNetwork, AdPlacement placement)
        {
            LoadRewardedAd(GetWorkableAdClient((AdNetwork)adNetwork), placement);
        }

        /// <summary>
        /// Determines whether the default rewarded ad is ready to show.
        /// </summary>
        /// <returns><c>true</c> if the ad is ready; otherwise, <c>false</c>.</returns>
        public static bool IsRewardedAdReady()
        {
            return IsRewardedAdReady(DefaultRewardedAdClient, AdPlacement.Default);
        }

        /// <summary>
        /// Determines whether the rewarded ad of the default rewarded ad network
        /// at the specified placement is ready to show.
        /// </summary>
        /// <returns><c>true</c> if the ad is ready; otherwise, <c>false</c>.</returns>
        /// <param name="placement">Placement.</param>
        public static bool IsRewardedAdReady(AdPlacement placement)
        {
            return IsRewardedAdReady(DefaultRewardedAdClient, placement);
        }

        /// <summary>
        /// Determines whether the rewarded ad of the specified ad network
        /// at the specified placement is ready to show.
        /// </summary>
        /// <returns><c>true</c> if the ad is ready; otherwise, <c>false</c>.</returns>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="placement">Placement. Pass <c>AdPlacement.Default</c> to specify the default placement.</param>
        public static bool IsRewardedAdReady(RewardedAdNetwork adNetwork, AdPlacement placement)
        {
            return IsRewardedAdReady(GetWorkableAdClient((AdNetwork)adNetwork), placement);
        }

        /// <summary>
        /// Shows the default rewarded ad.
        /// </summary>
        public static void ShowRewardedAd()
        {
            ShowRewardedAd(DefaultRewardedAdClient, AdPlacement.Default);
        }

        /// <summary>
        /// Shows the rewarded ad of the default rewarded ad network at the specified placement.
        /// </summary>
        /// <param name="placement">Placement.</param>
        public static void ShowRewardedAd(AdPlacement placement)
        {
            ShowRewardedAd(DefaultRewardedAdClient, placement);
        }

        /// <summary>
        /// Shows the rewarded ad of the specified ad network at the specified placement.
        /// </summary>
        /// <param name="adNetwork">Ad network.</param>
        /// <param name="placement">Placement. Pass <c>AdPlacement.Default</c> to specify the default placement.</param>
        public static void ShowRewardedAd(RewardedAdNetwork adNetwork, AdPlacement placement)
        {
            ShowRewardedAd(GetWorkableAdClient((AdNetwork)adNetwork), placement);
        }

        //------------------------------------------------------------
        // Ads Removal.
        //------------------------------------------------------------

        /// <summary>
        /// Determines whether ads were removed.
        /// </summary>
        /// <returns><c>true</c> if ads were removed; otherwise, <c>false</c>.</returns>
        public static bool IsAdRemoved()
        {
            return (StorageUtil.GetInt(AD_REMOVE_STATUS_PPKEY, AD_ENABLED) == AD_DISABLED);
        }

        /// <summary>
        /// Removes ads permanently. This is intended to be used with the "Remove Ads" button.
        /// This will hide any banner ad if it is being shown and
        /// prohibit future loading and showing of all ads, except rewarded ads.
        /// You can pass <c>true</c> to <c>revokeConsents</c> to also revoke the data privacy consent
        /// of the Advertising module and all supported networks (which may be desirable as rewarded ads are still available).
        /// Note that this method uses PlayerPrefs to store the ad removal status with no encryption/scrambling.
        /// </summary>
        /// <param name="revokeConsents">If set to <c>true</c> revoke consents.</param>
        public static void RemoveAds(bool revokeConsents = false)
        {
            // Destroy banner ad if any.
            DestroyAllBannerAds();

            // Revoke all clients' consent if needed.
            if (revokeConsents)
            {
                RevokeAllNetworksDataPrivacyConsent();
                RevokeDataPrivacyConsent();
            }

            // Store ad removal status.
            StorageUtil.SetInt(AD_REMOVE_STATUS_PPKEY, AD_DISABLED);
            StorageUtil.Save();

            // Fire event
            if (AdsRemoved != null)
                AdsRemoved();

            Debug.Log("Ads were removed.");
        }

        /// <summary>
        /// Resets the ads removal status and allows showing ads again.
        /// This is intended for testing purpose only. Note that this method
        /// doesn't restore the data privacy consent of the Advertising module
        /// and the supported networks. 
        /// </summary>
        public static void ResetRemoveAds()
        {
            // Update ad removal status.
            StorageUtil.SetInt(AD_REMOVE_STATUS_PPKEY, AD_ENABLED);
            StorageUtil.Save();

            Debug.Log("Ads were re-enabled.");
        }

        #endregion // Public API

        #region Deprecated API

        [Obsolete("This method was deprecated. Please use AutoAdLoadingMode property instead.")]
        public static bool IsAutoLoadDefaultAds()
        {
            return EM_Settings.Advertising.AutoAdLoadingMode == AutoAdLoadingMode.LoadDefaultAds;
        }

        [Obsolete("This method was deprecated. Please use AutoAdLoadingMode property instead.")]
        public static void EnableAutoLoadDefaultAds(bool isAutoLoad)
        {
            EM_Settings.Advertising.AutoAdLoadingMode = isAutoLoad ? AutoAdLoadingMode.LoadDefaultAds : AutoAdLoadingMode.None;
        }

        [Obsolete("This method was deprecated. Please use AutoAdLoadingMode property instead.")]
        public static void SetAutoLoadDefaultAds(bool isAutoLoad)
        {
            EnableAutoLoadDefaultAds(isAutoLoad);
        }

        [Obsolete("This method was deprecated. Please use " +
            "LoadInterstitialAd(InterstitialAdNetwork adNetwork, AdPlacement placement) instead.")]
        public static void LoadInterstitialAd(InterstitialAdNetwork adNetwork, AdLocation location)
        {
            LoadInterstitialAd(adNetwork, location.ToAdPlacement());
        }

        [Obsolete("This method was deprecated. Please use " +
            "IsInterstitialAdReady(InterstitialAdNetwork adNetwork, AdPlacement placement) instead.")]
        public static bool IsInterstitialAdReady(InterstitialAdNetwork adNetwork, AdLocation location)
        {
            return IsInterstitialAdReady(adNetwork, location.ToAdPlacement());
        }

        [Obsolete("This method was deprecated. Please use " +
            "ShowInterstitialAd(InterstitialAdNetwork adNetwork, AdPlacement placement) instead.")]
        public static void ShowInterstitialAd(InterstitialAdNetwork adNetwork, AdLocation location)
        {
            ShowInterstitialAd(adNetwork, location.ToAdPlacement());
        }

        [Obsolete("This method was deprecated. Please use " +
            "LoadRewardedAd(RewardedAdNetwork adNetwork, AdPlacement placement) instead.")]
        public static void LoadRewardedAd(RewardedAdNetwork adNetwork, AdLocation location)
        {
            LoadRewardedAd(adNetwork, location.ToAdPlacement());
        }

        [Obsolete("This method was deprecated. Please use " +
            "IsRewardedAdReady(RewardedAdNetwork adNetwork, AdPlacement placement) instead.")]
        public static bool IsRewardedAdReady(RewardedAdNetwork adNetwork, AdLocation location)
        {
            return IsRewardedAdReady(adNetwork, location.ToAdPlacement());
        }

        [Obsolete("This method was deprecated. Please use " +
            "ShowRewardedAd(RewardedAdNetwork adNetwork, AdPlacement placement) instead.")]
        public static void ShowRewardedAd(RewardedAdNetwork adNetwork, AdLocation location)
        {
            ShowRewardedAd(adNetwork, location.ToAdPlacement());
        }

        #endregion

        #region Internal Stuff

        #region Auto-Load ads

        // This coroutine regularly checks if intersititial and rewarded ads are loaded, if they aren't
        // it will automatically perform loading.
        // If ads were removed, other ads will no longer be loaded except rewarded ads since they are
        // shown under user discretion and therefore can still possibly be used even if ads were removed.
        private static IEnumerator CRAutoLoadDefaultAds(float delay = 0)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);

            while (true)
            {
                foreach (AdType type in Enum.GetValues(typeof(AdType)))
                {
                    switch (type)
                    {
                        case AdType.Interstitial:
                            if (!IsInterstitialAdReady() && !IsAdRemoved())
                            {
                                if (Time.realtimeSinceStartup - lastDefaultInterstitialAdLoadTimestamp >= EM_Settings.Advertising.AdLoadingInterval)
                                {
                                    LoadInterstitialAd();
                                    lastDefaultInterstitialAdLoadTimestamp = Time.realtimeSinceStartup;
                                }
                            }
                            break;
                        case AdType.Rewarded:
                            if (!IsRewardedAdReady())
                            {
                                if (Time.realtimeSinceStartup - lastDefaultRewardedAdLoadTimestamp >= EM_Settings.Advertising.AdLoadingInterval)
                                {
                                    LoadRewardedAd();
                                    lastDefaultRewardedAdLoadTimestamp = Time.realtimeSinceStartup;
                                }
                            }
                            break;
                        default:
                            break;
                    }
                }

                yield return new WaitForSeconds(EM_Settings.Advertising.AdCheckingInterval);
            }
        }

        /// <summary>
        /// This coroutine load all available ads (default and custom) of all imported networks.
        /// </summary>
        /// <param name="delay">Delay time when starting the coroutine.</param>
        private static IEnumerator CRAutoLoadAllAds(float delay = 0)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);

            List<IAdClient> availableInterstitialNetworks = GetAvailableNetworks<InterstitialAdNetwork>();
            List<IAdClient> availableRewardedNetworks = GetAvailableNetworks<RewardedAdNetwork>();

            while (true)
            {
                LoadAllInterstitialAds(availableInterstitialNetworks);
                LoadAllRewardedAds(availableRewardedNetworks);
                yield return new WaitForSeconds(EM_Settings.Advertising.AdCheckingInterval);
            }
        }

        /// <summary>
        /// Load all available interstitial ads of specific clients.
        /// </summary>
        private static void LoadAllInterstitialAds(List<IAdClient> clients)
        {
            if (IsAdRemoved())
                return;

            foreach (var client in clients)
            {
                /// Load all defined interstitial ad placements.
                var customPlacements = client.DefinedCustomInterstitialAdPlacements;

                if (customPlacements == null)
                {
                    customPlacements = new List<AdPlacement>();
                }

                if (!customPlacements.Contains(AdPlacement.Default))
                    customPlacements.Add(AdPlacement.Default);  // always load the Default placement.

                foreach (var placement in customPlacements)
                {
                    if (!client.IsValidPlacement(placement, AdType.Interstitial))
                        continue;

                    string tempIndex = client.Network.ToString() + placement.ToString();

                    if (IsInterstitialAdReady(client, placement))
                        continue;

                    if (!lastCustomInterstitialAdsLoadTimestamp.ContainsKey(tempIndex))
                        lastCustomInterstitialAdsLoadTimestamp.Add(tempIndex, DEFAULT_TIMESTAMP);

                    if (Time.realtimeSinceStartup - lastCustomInterstitialAdsLoadTimestamp[tempIndex] < EM_Settings.Advertising.AdLoadingInterval)
                        continue;

                    LoadInterstitialAd(client, placement);
                    lastCustomInterstitialAdsLoadTimestamp[tempIndex] = Time.realtimeSinceStartup;
                }
            }
        }

        /// <summary>
        /// Load all available rewarded ads of specific clients. 
        /// </summary>
        private static void LoadAllRewardedAds(List<IAdClient> clients)
        {
            foreach (var client in clients)
            {
                /// Load all custom rewarded ads if available.
                var customPlacements = client.DefinedCustomRewardedAdPlacements;

                if (customPlacements == null)
                {
                    customPlacements = new List<AdPlacement>();
                }

                // Add the Default placement to the loading list. Some networks
                // may only allow loading one rewarded ad at a time (subsequent loadings can
                // only be done if previous ad has been consumed), so we make sure the
                // Default placement is always loaded first by inserting it at the first index.
                if (!customPlacements.Contains(AdPlacement.Default))
                    customPlacements.Insert(0, AdPlacement.Default);

                foreach (var placement in customPlacements)
                {
                    if (!client.IsValidPlacement(placement, AdType.Rewarded))
                        continue;

                    string tempIndex = client.Network.ToString() + placement.ToString();

                    if (IsRewardedAdReady(client, placement))
                        continue;

                    if (!lastCustomRewardedAdsLoadTimestamp.ContainsKey(tempIndex))
                        lastCustomRewardedAdsLoadTimestamp.Add(tempIndex, DEFAULT_TIMESTAMP);

                    if (Time.realtimeSinceStartup - lastCustomRewardedAdsLoadTimestamp[tempIndex] < EM_Settings.Advertising.AdLoadingInterval)
                        continue;

                    LoadRewardedAd(client, placement);
                    lastCustomRewardedAdsLoadTimestamp[tempIndex] = Time.realtimeSinceStartup;
                }
            }
        }

        /// <summary>
        /// Returns all imported ads networks.
        /// </summary>
        private static List<IAdClient> GetAvailableNetworks<T>()
        {
            List<IAdClient> availableNetworks = new List<IAdClient>();
            foreach (T network in Enum.GetValues(typeof(T)))
            {
                AdClientImpl client = GetAdClient((AdNetwork)(Enum.Parse(typeof(T), network.ToString())));
                if (client.IsSdkAvail)
                {
                    var workableClient = GetWorkableAdClient((AdNetwork)(Enum.Parse(typeof(T), network.ToString())));
                    availableNetworks.Add(workableClient);
                }
            }
            return availableNetworks;
        }

        #endregion

        private static void ShowBannerAd(IAdClient client, AdPlacement placement, BannerAdPosition position, BannerAdSize size)
        {
            if (IsAdRemoved())
            {
                Debug.Log("Could not show banner ad: ads were removed.");
                return;
            }

            client.ShowBannerAd(placement, position, size);
            AddActiveBannerAd(client.Network, placement);
        }

        private static void HideBannerAd(IAdClient client, AdPlacement placement)
        {
            client.HideBannerAd(placement);
            RemoveActiveBannerAd(client.Network, placement);
        }

        private static void DestroyBannerAd(IAdClient client, AdPlacement placement)
        {
            client.DestroyBannerAd(placement);
            RemoveActiveBannerAd(client.Network, placement);
        }

        private static void DestroyAllBannerAds()
        {
            foreach (KeyValuePair<AdNetwork, List<AdPlacement>> pair in activeBannerAds)
            {
                if (pair.Value != null && pair.Value.Count > 0)
                {
                    var client = GetWorkableAdClient(pair.Key);
                    foreach (var placement in pair.Value)
                        client.DestroyBannerAd(placement);
                }
            }

            activeBannerAds.Clear();
        }

        private static void LoadInterstitialAd(IAdClient client, AdPlacement placement)
        {
            if (IsAdRemoved())
                return;

            client.LoadInterstitialAd(placement);
        }

        private static bool IsInterstitialAdReady(IAdClient client, AdPlacement placement)
        {
            if (IsAdRemoved())
                return false;

            return client.IsInterstitialAdReady(placement);
        }

        private static void ShowInterstitialAd(IAdClient client, AdPlacement placement)
        {
            if (IsAdRemoved())
            {
                Debug.Log("Could not show interstitial ad: ads were disabled by RemoveAds().");
                return;
            }

            client.ShowInterstitialAd(placement);
        }

        // Note that rewarded ads should still be available after ads removal.
        // which is why we don't check if ads were removed in the following methods.

        private static void LoadRewardedAd(IAdClient client, AdPlacement placement)
        {
            client.LoadRewardedAd(placement);
        }

        private static bool IsRewardedAdReady(IAdClient client, AdPlacement placement)
        {
            return client.IsRewardedAdReady(placement);
        }

        private static void ShowRewardedAd(IAdClient client, AdPlacement placement)
        {
            client.ShowRewardedAd(placement);
        }

        /// <summary>
        /// Adds the given network and placement to the dict of currently shown banner ads,
        /// if the dict hasn't contained them already.
        /// </summary>
        /// <param name="network">Network.</param>
        /// <param name="placement">Placement.</param>
        private static void AddActiveBannerAd(AdNetwork network, AdPlacement placement)
        {
            List<AdPlacement> bannerAdPlacements;
            activeBannerAds.TryGetValue(network, out bannerAdPlacements);

            // This network already has some active placements 
            // (rarely happens but logically possible).
            if (bannerAdPlacements != null)
            {
                if (!bannerAdPlacements.Contains(placement))
                    bannerAdPlacements.Add(placement);
            }
            // This network hasn't had any active placements yet.
            else
            {
                activeBannerAds[network] = new List<AdPlacement>() { placement };
            }
        }

        /// <summary>
        /// Removes the given network and placement from the dict of currently shown banner ads,
        /// if they exists in the dict.
        /// </summary>
        /// <param name="network">Network.</param>
        /// <param name="placement">Placement.</param>
        private static void RemoveActiveBannerAd(AdNetwork network, AdPlacement placement)
        {
            List<AdPlacement> bannerAdPlacements;
            activeBannerAds.TryGetValue(network, out bannerAdPlacements);

            if (bannerAdPlacements != null)
            {
                bannerAdPlacements.Remove(placement);
            }
        }

        /// <summary>
        /// Grants the data privacy consent to all supported networks. Use with care.
        /// </summary>
        private static void GrantAllNetworksDataPrivacyConsent()
        {
            foreach (AdNetwork network in Enum.GetValues(typeof(AdNetwork)))
            {
                if (network != AdNetwork.None)
                    GrantDataPrivacyConsent(network);
            }
        }

        /// <summary>
        /// Revokes the data privacy consent of all supported networks. Use with care.
        /// </summary>
        private static void RevokeAllNetworksDataPrivacyConsent()
        {
            foreach (AdNetwork network in Enum.GetValues(typeof(AdNetwork)))
            {
                if (network != AdNetwork.None)
                    RevokeDataPrivacyConsent(network);
            }
        }

        /// <summary>
        /// Gets the singleton ad client of the specified network.
        /// This may or may not be initialized.
        /// </summary>
        /// <returns>The ad client.</returns>
        /// <param name="network">Network.</param>
        private static AdClientImpl GetAdClient(AdNetwork network)
        {
            switch (network)
            {
                case AdNetwork.AdColony:
                    return AdColonyClientImpl.CreateClient();
                case AdNetwork.AdMob:
                    return AdMobClientImpl.CreateClient();
                case AdNetwork.AppLovin:
                    return AppLovinClientImpl.CreateClient();
                case AdNetwork.Chartboost:
                    return ChartboostClientImpl.CreateClient();
                case AdNetwork.AudienceNetwork:
                    return AudienceNetworkClientImpl.CreateClient();
                case AdNetwork.FairBid:
                    return FairBidClientImpl.CreateClient();
                case AdNetwork.IronSource:
                    return IronSourceClientImpl.CreateClient();
                case AdNetwork.MoPub:
                    return MoPubClientImpl.CreateClient();
                case AdNetwork.TapJoy:
                    return TapjoyClientImpl.CreateClient();
                case AdNetwork.UnityAds:
                    return UnityAdsClientImpl.CreateClient();
                case AdNetwork.None:
                    return NoOpClientImpl.CreateClient();
                case AdNetwork.Vungle:
                    return VungleClientImpl.CreateClient();
                default:
                    throw new NotImplementedException("No client implemented for the network:" + network.ToString());
            }

        }

        /// <summary>
        /// Grabs the singleton ad client for the specified network and performs
        /// necessary setup for it, including initializing it and subscribing to its events.
        /// </summary>
        /// <returns>The ad client.</returns>
        /// <param name="network">Network.</param>
        private static AdClientImpl SetupAdClient(AdNetwork network)
        {
            AdClientImpl client = GetAdClient(network);
            Debug.Log(client);

            if (client != null && client.Network != AdNetwork.None)
            {
                // Subscribe client's events.
                SubscribeAdClientEvents(client);

                // Initialize ad client.
                if (!client.IsInitialized)
                    client.Init();
            }

            return client;
        }

        /// <summary>
        /// Grabs the ready to work (done initialization, setup, etc.)
        /// ad client for the specified network.
        /// </summary>
        /// <returns>The workable ad client.</returns>
        /// <param name="network">Network.</param>
        private static AdClientImpl GetWorkableAdClient(AdNetwork network)
        {
            switch (network)
            {
                case AdNetwork.AdColony:
                    return AdColonyClient;
                case AdNetwork.AdMob:
                    return AdMobClient;
                case AdNetwork.AppLovin:
                    return AppLovinClient;
                case AdNetwork.Chartboost:
                    return ChartboostClient;
                case AdNetwork.AudienceNetwork:
                    return AudienceNetworkClient;
                case AdNetwork.FairBid:
                    return FairBidClient;
                case AdNetwork.MoPub:
                    return MoPubClient;
                case AdNetwork.IronSource:
                    return IronSourceClient;
                case AdNetwork.UnityAds:
                    return UnityAdsClient;
                case AdNetwork.TapJoy:
                    return TapjoyClient;
                case AdNetwork.None:
                    return NoOpClientImpl.CreateClient();
                case AdNetwork.Vungle:
                    return VungleClient;
                default:
                    throw new NotImplementedException("No client found for the network:" + network.ToString());
            }
        }

        private static void SubscribeAdClientEvents(IAdClient client)
        {
            if (client == null)
                return;

            client.InterstitialAdCompleted += OnInternalInterstitialAdCompleted;
            client.RewardedAdSkipped += OnInternalRewardedAdSkipped;
            client.RewardedAdCompleted += OnInternalRewardedAdCompleted;
        }

        private static void OnInternalInterstitialAdCompleted(IAdClient client, AdPlacement placement)
        {
            if (InterstitialAdCompleted != null)
                InterstitialAdCompleted((InterstitialAdNetwork)client.Network, placement);
        }

        private static void OnInternalRewardedAdSkipped(IAdClient client, AdPlacement placement)
        {
            if (RewardedAdSkipped != null)
                RewardedAdSkipped((RewardedAdNetwork)client.Network, placement);
        }

        private static void OnInternalRewardedAdCompleted(IAdClient client, AdPlacement placement)
        {
            if (RewardedAdCompleted != null)
                RewardedAdCompleted((RewardedAdNetwork)client.Network, placement);
        }

        #endregion
    }
}