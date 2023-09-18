using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile.Internal;

namespace EasyMobile
{
    public class MoPubClientImpl : AdClientImpl
    {
        private const string NO_SDK_MESSAGE = "SDK missing. Please import the MoPub plugin.";

#if EM_MOPUB
        private bool mIsDefaultBannerAdReady;
        private bool mIsDefaultInterstitialAdReady;

        private MoPubSettings mAdSettings;

        private BannerAdPosition mDefaultBannerPosition = BannerAdPosition.Bottom;
        private BannerAdSize mDefaultBannerSize = new BannerAdSize(-1, -1);

        /// <summary>
        /// We're gonna save all the loaded custom banners here.
        /// </summary>
        /// Key: The AdPlacement used to load the banner.
        /// Value: Loaded banner's position & size.
        private Dictionary<AdPlacement, KeyValuePair<BannerAdPosition, BannerAdSize>> mCustomBanners;

        /// <summary>
        /// We're gonna save all the loaded custom interstitial ads here.
        /// </summary>
        /// Key: The AdPlacement used to load the interstitial ad.
        /// Value: Loaded interstitial ad's state (ready or not).
        private Dictionary<AdPlacement, bool> mCustomInterstitialAds;

        /// <summary>
        /// We're gonna save all the loaded custom rewarded video ads here.
        /// </summary>
        private List<AdPlacement> mCustomRewardedVideoAds;
#endif

        #region MoPub Events

#if EM_MOPUB

        /// <summary>
        /// Fired when the SDK has finished initializing
        /// </summary>
        public event Action<string> OnSdkInitializedEvent
        {
            add { MoPubManager.OnSdkInitializedEvent += value; }
            remove { MoPubManager.OnSdkInitializedEvent -= value; }
        }

        /// <summary>
        /// Fired when an ad loads in the banner. Includes the ad height.
        /// </summary>
        public event Action<string, float> OnAdLoadedEvent
        {
            add { MoPubManager.OnAdLoadedEvent += value; }
            remove { MoPubManager.OnAdLoadedEvent -= value; }
        }

        /// <summary>
        /// Fired when an ad fails to load for the banner
        /// </summary>
        public event Action<string, string> OnAdFailedEvent
        {
            add { MoPubManager.OnAdFailedEvent += value; }
            remove { MoPubManager.OnAdFailedEvent -= value; }
        }

        /// <summary>
        /// Android only. Fired when a banner ad is clicked
        /// </summary>
        public event Action<string> OnAdClickedEvent
        {
            add { MoPubManager.OnAdClickedEvent += value; }
            remove { MoPubManager.OnAdClickedEvent -= value; }
        }

        /// <summary>
        /// Android only. Fired when a banner ad expands to encompass a greater portion of the screen
        /// </summary>
        public event Action<string> OnAdExpandedEvent
        {
            add { MoPubManager.OnAdExpandedEvent += value; }
            remove { MoPubManager.OnAdExpandedEvent -= value; }
        }

        /// <summary>
        /// Android only. Fired when a banner ad collapses back to its initial size
        /// </summary>
        public event Action<string> OnAdCollapsedEvent
        {
            add { MoPubManager.OnAdCollapsedEvent += value; }
            remove { MoPubManager.OnAdCollapsedEvent -= value; }
        }

        /// <summary>
        /// Fired when an interstitial ad is loaded and ready to be shown
        /// </summary>
        public event Action<string> OnInterstitialLoadedEvent
        {
            add { MoPubManager.OnInterstitialLoadedEvent += value; }
            remove { MoPubManager.OnInterstitialLoadedEvent -= value; }
        }

        /// <summary>
        /// Fired when an interstitial ad fails to load
        /// </summary>
        public event Action<string, string> OnInterstitialFailedEvent
        {
            add { MoPubManager.OnInterstitialFailedEvent += value; }
            remove { MoPubManager.OnInterstitialFailedEvent -= value; }
        }

        /// <summary>
        /// Fired when an interstitial ad is dismissed
        /// </summary>
        public event Action<string> OnInterstitialDismissedEvent
        {
            add { MoPubManager.OnInterstitialDismissedEvent += value; }
            remove { MoPubManager.OnInterstitialDismissedEvent -= value; }
        }

        /// <summary>
        /// iOS only. Fired when an interstitial ad expires
        /// </summary>
        public event Action<string> OnInterstitialExpiredEvent
        {
            add { MoPubManager.OnInterstitialExpiredEvent += value; }
            remove { MoPubManager.OnInterstitialExpiredEvent -= value; }
        }

        /// <summary>
        /// Android only. Fired when an interstitial ad is displayed
        /// </summary>
        public event Action<string> OnInterstitialShownEvent
        {
            add { MoPubManager.OnInterstitialShownEvent += value; }
            remove { MoPubManager.OnInterstitialShownEvent -= value; }
        }

        /// <summary>
        /// Android only. Fired when an interstitial ad is clicked
        /// </summary>
        public event Action<string> OnInterstitialClickedEvent
        {
            add { MoPubManager.OnInterstitialClickedEvent += value; }
            remove { MoPubManager.OnInterstitialClickedEvent -= value; }
        }

        /// <summary>
        /// Fired when a rewarded video finishes loading and is ready to be displayed
        /// </summary>
        public event Action<string> OnRewardedVideoLoadedEvent
        {
            add { MoPubManager.OnRewardedVideoLoadedEvent += value; }
            remove { MoPubManager.OnRewardedVideoLoadedEvent -= value; }
        }

        /// <summary>
        /// Fired when a rewarded video fails to load. Includes the error message
        /// </summary>
        public event Action<string, string> OnRewardedVideoFailedEvent
        {
            add { MoPubManager.OnRewardedVideoFailedEvent += value; }
            remove { MoPubManager.OnRewardedVideoFailedEvent -= value; }
        }

        /// <summary>
        /// iOS only. Fired when a rewarded video expires
        /// </summary>
        public event Action<string> OnRewardedVideoExpiredEvent
        {
            add { MoPubManager.OnRewardedVideoExpiredEvent += value; }
            remove { MoPubManager.OnRewardedVideoExpiredEvent -= value; }
        }

        /// <summary>
        /// Fired when an rewarded video is displayed
        /// </summary>
        public event Action<string> OnRewardedVideoShownEvent
        {
            add { MoPubManager.OnRewardedVideoShownEvent += value; }
            remove { MoPubManager.OnRewardedVideoShownEvent -= value; }
        }

        /// <summary>
        /// Fired when an rewarded video is clicked
        /// </summary>
        public event Action<string> OnRewardedVideoClickedEvent
        {
            add { MoPubManager.OnRewardedVideoClickedEvent += value; }
            remove { MoPubManager.OnRewardedVideoClickedEvent -= value; }
        }

        /// <summary>
        /// Fired when a rewarded video fails to play. Includes the error message
        /// </summary>
        public event Action<string, string> OnRewardedVideoFailedToPlayEvent
        {
            add { MoPubManager.OnRewardedVideoFailedToPlayEvent += value; }
            remove { MoPubManager.OnRewardedVideoFailedToPlayEvent -= value; }
        }

        /// <summary>
        /// Fired when a rewarded video completes. Includes all the data available about the reward
        /// </summary>
        public event Action<string, string, float> OnRewardedVideoReceivedRewardEvent
        {
            add { MoPubManager.OnRewardedVideoReceivedRewardEvent += value; }
            remove { MoPubManager.OnRewardedVideoReceivedRewardEvent -= value; }
        }

        /// <summary>
        /// Fired when a rewarded video closes
        /// </summary>
        public event Action<string> OnRewardedVideoClosedEvent
        {
            add { MoPubManager.OnRewardedVideoClosedEvent += value; }
            remove { MoPubManager.OnRewardedVideoClosedEvent -= value; }
        }

        /// <summary>
        /// iOS only. Fired when a rewarded video event causes another application to open
        /// </summary>
        public static event Action<string> OnRewardedVideoLeavingApplicationEvent
        {
            add { MoPubManager.OnRewardedVideoLeavingApplicationEvent += value; }
            remove { MoPubManager.OnRewardedVideoLeavingApplicationEvent -= value; }
        }

#endif

        #endregion  // MoPub-Specific Events

        #region Singleton

        private static MoPubClientImpl sInstance = null;

        private MoPubClientImpl()
        {
        }

        /// <summary>
        /// Returns the singleton client.
        /// </summary>
        /// <returns>The client.</returns>
        public static MoPubClientImpl CreateClient()
        {
            if (sInstance == null)
            {
                sInstance = new MoPubClientImpl();
            }
            return sInstance;
        }

        #endregion

        #region AdClient Overrides

        public override AdNetwork Network { get { return AdNetwork.MoPub; } }

        public override bool IsBannerAdSupported { get { return true; } }

        public override bool IsInterstitialAdSupported { get { return true; } }

        public override bool IsRewardedAdSupported { get { return true; } }

        public override bool IsInitialized
        {
            get
            {
#if EM_MOPUB
                return mIsInitialized && MoPub.IsSdkInitialized;
#else
                return mIsInitialized;
#endif
            }
        }

        public override bool IsSdkAvail
        {
            get
            {
#if EM_MOPUB
                return true;
#else
                return false;
#endif
            }
        }

        public override bool IsValidPlacement(AdPlacement placement, AdType type)
        {
#if EM_MOPUB
            string id;
            if (placement == AdPlacement.Default)
            {
                switch (type)
                {
                    case AdType.Rewarded:
                        id = mAdSettings.DefaultRewardedAdId.Id;
                        break;
                    case AdType.Interstitial:
                        id = mAdSettings.DefaultInterstitialAdId.Id;
                        break;
                    default:
                        id = mAdSettings.DefaultBannerId.Id;
                        break;
                }
            }
            else
            {
                switch (type)
                {
                    case AdType.Rewarded:
                        id = FindIdForPlacement(mAdSettings.CustomRewardedAdIds, placement);
                        break;
                    case AdType.Interstitial:
                        id = FindIdForPlacement(mAdSettings.CustomInterstitialAdIds, placement);
                        break;
                    default:
                        id = FindIdForPlacement(mAdSettings.CustomBannerIds, placement);
                        break;
                }
            }

            if (string.IsNullOrEmpty(id))
                return false;
            else
                return true;
#else
            return false;
#endif
        }

        protected override Dictionary<AdPlacement, AdId> CustomInterstitialAdsDict
        {
            get
            {
#if EM_MOPUB
                return mAdSettings == null ? null : mAdSettings.CustomInterstitialAdIds;
#else
                return null;
#endif
            }
        }

        protected override Dictionary<AdPlacement, AdId> CustomRewardedAdsDict
        {
            get
            {
#if EM_MOPUB
                return mAdSettings == null ? null : mAdSettings.CustomRewardedAdIds;
#else
                return null;
#endif
            }
        }

        protected override string NoSdkMessage { get { return NO_SDK_MESSAGE; } }

        protected override void InternalInit()
        {
#if EM_MOPUB

            if (!mIsInitialized)
            {
                mIsInitialized = true;
                mAdSettings = EM_Settings.Advertising.MoPub;

                mCustomBanners = new Dictionary<AdPlacement, KeyValuePair<BannerAdPosition, BannerAdSize>>();
                mCustomInterstitialAds = new Dictionary<AdPlacement, bool>();
                mCustomRewardedVideoAds = new List<AdPlacement>();

                RegisterAllBannerEvents();
                RegisterAllInterstitialEvents();
                RegisterAllRewardedVideoEvents();
                RegisterInitializedEvent();
            }

            InitMopubSdk();

#endif
        }

        protected override void InternalShowBannerAd(AdPlacement placement, BannerAdPosition position, BannerAdSize size)
        {
#if EM_MOPUB
            string id = placement == AdPlacement.Default ?
                mAdSettings.DefaultBannerId.Id :
                FindIdForPlacement(mAdSettings.CustomBannerIds, placement);

            if (string.IsNullOrEmpty(id))
            {
                Debug.LogFormat("Attempting to show {0} banner ad with an undefined ID at placement {1}",
                    Network.ToString(),
                    AdPlacement.GetPrintableName(placement));
                return;
            }

            if (placement.Equals(AdPlacement.Default)) // Default banner...
            {
                /// Create a new banner if user request a
                /// banner ad with different position or size.
                if (position != mDefaultBannerPosition || size != mDefaultBannerSize)
                {
                    Debug.Log("Creating new default banner...");
                    mDefaultBannerPosition = position;
                    mDefaultBannerSize = size;
                    mIsDefaultBannerAdReady = false;
                }

                if (!mIsDefaultBannerAdReady)
                    CreateBannerAd(id, position, size);

                MoPub.ShowBanner(id, true);
            }
            else // Custom banner...
            {
                /// Create a new banner if the banner with this key hasn't been initialized or
                /// user request new banner with existed key but different position or size.
                bool shouldCreateFlag = !mCustomBanners.ContainsKey(placement) ||
                                        mCustomBanners[placement].Key != position ||
                                        mCustomBanners[placement].Value != size;

                if (shouldCreateFlag)
                {
                    Debug.Log("Creating new custom banner...");
                    /// Create new banner & save its position & size to compare later.
                    CreateBannerAd(id, position, size);
                    mCustomBanners[placement] = new KeyValuePair<BannerAdPosition, BannerAdSize>(position, size);
                }

                MoPub.ShowBanner(id, true);
            }
#endif
        }

        protected override void InternalHideBannerAd(AdPlacement placement)
        {
#if EM_MOPUB
            if (placement.Equals(AdPlacement.Default)) // Default banner...
            {
                if (!mIsDefaultBannerAdReady)
                    return;

                MoPub.ShowBanner(mAdSettings.DefaultBannerId.Id, false);
            }
            else // Custom banner...
            {
                if (!mCustomBanners.ContainsKey(placement) || !mAdSettings.CustomBannerIds.ContainsKey(placement))
                    return;

                MoPub.ShowBanner(mAdSettings.CustomBannerIds[placement].Id, false);
            }
#endif
        }

        protected override void InternalDestroyBannerAd(AdPlacement placement)
        {
#if EM_MOPUB
            if (placement.Equals(AdPlacement.Default)) // Default banner...
            {
                if (!mIsDefaultBannerAdReady)
                    return;

                MoPub.DestroyBanner(mAdSettings.DefaultBannerId.Id);
                mIsDefaultBannerAdReady = false;
            }
            else // Custom banner...
            {
                if (!mCustomBanners.ContainsKey(placement) || !mAdSettings.CustomBannerIds.ContainsKey(placement))
                    return;

                MoPub.DestroyBanner(mAdSettings.CustomBannerIds[placement].Id);
                mCustomBanners.Remove(placement);
            }
#endif
        }

        protected override bool InternalIsInterstitialAdReady(AdPlacement placement)
        {
#if EM_MOPUB
            if (placement.Equals(AdPlacement.Default)) // Default interstitial
            {
                return mIsDefaultInterstitialAdReady;
            }
            else // Custom interstitial
            {
                return mCustomInterstitialAds != null &&
                mCustomInterstitialAds.ContainsKey(placement) &&
                mCustomInterstitialAds[placement];
            }
#else
            return false;
#endif
        }

        protected override void InternalLoadInterstitialAd(AdPlacement placement)
        {
#if EM_MOPUB
            string id = placement == AdPlacement.Default ?
                mAdSettings.DefaultInterstitialAdId.Id :
                FindIdForPlacement(mAdSettings.CustomInterstitialAdIds, placement);

            if (string.IsNullOrEmpty(id))
            {
                Debug.LogFormat("Attempting to load {0} interstitial ad with an undefined ID at placement {1}",
                    Network.ToString(),
                    AdPlacement.GetPrintableName(placement));
                return;
            }

            if (placement.Equals(AdPlacement.Default)) // Default interstitial ad...
            {
                MoPub.RequestInterstitialAd(id);
            }
            else // Custom interstitial ad...
            {
                if (!mCustomInterstitialAds.ContainsKey(placement))
                    mCustomInterstitialAds.Add(placement, false);

                MoPub.RequestInterstitialAd(id);
            }
#endif
        }

        protected override void InternalShowInterstitialAd(AdPlacement placement)
        {
#if EM_MOPUB
            if (placement.Equals(AdPlacement.Default)) // Default interstitial ad...
            {
                MoPub.ShowInterstitialAd(mAdSettings.DefaultInterstitialAdId.Id);
                mIsDefaultInterstitialAdReady = false;
            }
            else // Custom interstitial ad...
            {
                if (!mCustomInterstitialAds.ContainsKey(placement) || !mAdSettings.CustomInterstitialAdIds.ContainsKey(placement))
                    return;

                MoPub.ShowInterstitialAd(mAdSettings.CustomInterstitialAdIds[placement].Id);
                mCustomInterstitialAds[placement] = false;
            }
#endif
        }

        protected override bool InternalIsRewardedAdReady(AdPlacement placement)
        {
#if EM_MOPUB
            if (placement.Equals(AdPlacement.Default)) // Default rewarded ad...
            {
                return MoPub.HasRewardedVideo(mAdSettings.DefaultRewardedAdId.Id);
            }
            else // Custom rewarded ad...
            {
                if (mAdSettings.CustomRewardedAdIds.ContainsKey(placement))
                    return MoPub.HasRewardedVideo(mAdSettings.CustomRewardedAdIds[placement].Id);
                else
                    return false;
            }
#else
            return false;
#endif
        }

        protected override void InternalLoadRewardedAd(AdPlacement placement)
        {
#if EM_MOPUB
            string id = placement == AdPlacement.Default ?
                mAdSettings.DefaultRewardedAdId.Id :
                FindIdForPlacement(mAdSettings.CustomRewardedAdIds, placement);

            if (string.IsNullOrEmpty(id))
            {
                Debug.LogFormat("Attempting to load {0} rewarded ad with an undefined ID at placement {1}",
                    Network.ToString(),
                    AdPlacement.GetPrintableName(placement));
                return;
            }

            if (placement.Equals(AdPlacement.Default)) // Default rewarded ad...
            {
                MoPub.RequestRewardedVideo(id);
            }
            else // Custom rewarded ad...
            {
                if (!mCustomRewardedVideoAds.Contains(placement))
                    mCustomRewardedVideoAds.Add(placement);

                MoPub.RequestRewardedVideo(id);
            }
#endif
        }

        protected override void InternalShowRewardedAd(AdPlacement placement)
        {
#if EM_MOPUB
            if (placement.Equals(AdPlacement.Default))
            {
                MoPub.ShowRewardedVideo(mAdSettings.DefaultRewardedAdId.Id);
            }
            else
            {
                if (mAdSettings.CustomRewardedAdIds.ContainsKey(placement))
                    MoPub.ShowRewardedVideo(mAdSettings.CustomRewardedAdIds[placement].Id);
            }
#endif
        }

        #endregion

        #region IConsentRequirable Overrides

        private const string DATA_PRIVACY_CONSENT_KEY = "EM_Ads_MoPub_DataPrivacyConsent";

        protected override string DataPrivacyConsentSaveKey { get { return DATA_PRIVACY_CONSENT_KEY; } }

        protected override void ApplyDataPrivacyConsent(ConsentStatus consent)
        {
#if EM_MOPUB
            if (!MoPub.IsSdkInitialized)
                return;     // MoPub requires initialization before setting consent.

            switch (consent)
            {
                case ConsentStatus.Granted:
                    MoPub.PartnerApi.GrantConsent();
                    break;
                case ConsentStatus.Revoked:
                    MoPub.PartnerApi.RevokeConsent();
                    break;
                case ConsentStatus.Unknown:
                default:
                    break;
            }
#endif
        }

        #endregion

#if EM_MOPUB

        #region Ad Event Handlers

        private void OnBannerAdClicked(string adUnitId)
        {

        }

        private void OnBannerAdCollapsed(string adUnitId)
        {

        }

        private void OnBannerAdExpanded(string adUnitId)
        {

        }

        private void OnBannerAdFailed(string adUnitId, string message)
        {
            /// If the failed banner is the default banner.
            if (adUnitId.Equals(mAdSettings.DefaultBannerId.Id))
            {
                mIsDefaultBannerAdReady = false;
                return;
            }

            /// If the failed banner is a loaded custom one.
            AdPlacement placement;
            if (TryFindCustomBannerPlacementWithId(adUnitId, out placement) && mCustomBanners.ContainsKey(placement))
            {
                mCustomBanners.Remove(placement);
            }

            Debug.LogWarning("An unexpected banner ad failed event. Id: " + adUnitId);
        }

        private void OnBannerAdLoaded(string adUnitId, float height)
        {
            /// If the loaded banner is the default banner.
            if (mAdSettings.DefaultBannerId.Id.Equals(adUnitId))
                mIsDefaultBannerAdReady = true;
        }

        private void OnInterstitialAdClicked(string adUnitId)
        {

        }

        private void OnInterstitialAdDismissed(string adUnitId)
        {
            Debug.Log("OnInterstitialAdDismissed: " + adUnitId);

            /// If the dismissed interstitial ad is the default one.
            if (adUnitId.Equals(mAdSettings.DefaultInterstitialAdId.Id))
            {
                OnInterstitialAdCompleted(AdPlacement.Default);
                return;
            }

            /// If the dismissed interstitial ad is a custom one.
            AdPlacement placement;
            if (TryFindCustomInterstitialAdPlacementWithId(adUnitId, out placement) && mCustomInterstitialAds.ContainsKey(placement))
            {
                OnInterstitialAdCompleted(placement);
                return;
            }

            Debug.LogWarning("An unexpected interstitial ad is dismissed. Id: " + adUnitId);
        }

        private void OnInterstitialExpired(string adUnitId)
        {
            SetInterstitialAdLoadedState(false, adUnitId, "An unexpected interstitial ad is expired.");
        }

        private void OnInterstitialFailed(string adUnitId, string errorMessage)
        {
            SetInterstitialAdLoadedState(false, adUnitId, "An unexpected interstitial ad is failed: " + errorMessage + ".");
        }

        private void OnInterstitialLoaded(string adUnitId)
        {
            SetInterstitialAdLoadedState(true, adUnitId, "An unexpected interstitial ad is loaded");
        }

        private void OnInterstitialShowed(string adUnitId)
        {
            Debug.Log("OnInterstitialShowed: " + adUnitId);
        }

        private void OnRewardedAdClicked(string adUnitId)
        {

        }

        private void OnRewardedAdClosed(string adUnitId)
        {
            /// Check if the closed rewarded video ad is the default one.
            if (mAdSettings.DefaultRewardedAdId.Id.Equals(adUnitId))
            {
                OnRewardedAdCompleted(AdPlacement.Default);
                return;
            }

            /// Check if the closed ad is a custom one.
            AdPlacement placement;
            if (TryFindCustomRewardedVideoAdPlacementWithId(adUnitId, out placement))
            {
                OnRewardedAdCompleted(placement);
                return;
            }

            Debug.LogWarning("An unexpected rewarded video was closed. Id: " + adUnitId);
        }

        private void OnRewardedAdExpired(string adUnitId)
        {

        }

        private void OnRewardedAdFailed(string adUnitId, string errorMessage)
        {

        }

        private void OnRewardedAdFailedToPlay(string adUnitId, string errorMessage)
        {
            Debug.Log(string.Format("OnRewardedAdFailedToPlay. {0}, {1}", adUnitId, errorMessage));
        }

        private void OnRewardedAdLeavingApplication(string adUnitId)
        {

        }

        private void OnRewardedAdLoaded(string adUnitId)
        {

        }

        private void OnRewardedAdRewardReceived(string adUnitId, string arg2, float arg3)
        {
            Debug.Log(string.Format("OnRewardedAdReceived. {0}, {1}, {2}", adUnitId, arg2, arg3));
        }

        private void OnRewardedAdShowed(string adUnitId)
        {
            Debug.Log("OnRewardedAdShowed: " + adUnitId);
        }

        #endregion

        #region Initialize methods

        protected void InitMopubSdk()
        {
            RuntimeHelper.RunCoroutine(InitMoPubSdkCoroutine());
        }

        protected IEnumerator InitMoPubSdkCoroutine()
        {
            bool initResult = mAdSettings.UseAdvancedSetting ? AdvancedInit() : NormalInit();
            if (!initResult)
            {
                Debug.LogError("Failed to initialize MoPub.");
                yield break;
            }
            yield return new WaitUntil(() => MoPub.IsSdkInitialized);

            // Apply GDPR consent (if any) *after* the SDK is initialized
            // (the MoPub's grant and revoke methods internally check if the SDK is initialized).
            var consent = GetApplicableDataPrivacyConsent();
            ApplyDataPrivacyConsent(consent);

            LoadAllAdUnits();

            if (mAdSettings.ReportAppOpen)
            {
#if UNITY_IOS
                MoPub.ReportApplicationOpen(mAdSettings.ITuneAppID);
#elif UNITY_ANDROID
                MoPub.ReportApplicationOpen();
#endif
            }

            MoPub.EnableLocationSupport(mAdSettings.EnableLocationPassing);
        }

        /// <summary>
        /// Init Mopub sdk with an id from <see cref="GetInitializeId"/>.
        /// </summary>
        protected bool NormalInit()
        {
            string initId = GetInitializeId();
            if (string.IsNullOrEmpty(initId))
            {
                Debug.LogError("MoPubClient init error. Please provide at least one default ad id.");
                return false;
            }

            MoPub.InitializeSdk(initId);
            return true;
        }

        /// <summary>
        /// Init Mopub sdk with extra <see cref="MoPubBase.SdkConfiguration"/> from <see cref="GetSdkConfiguration"/>.
        /// </summary>
        protected bool AdvancedInit()
        {
            try
            {
                MoPub.InitializeSdk(GetSdkConfiguration());
                return true;
            }
            catch (Exception e)
            {
                Debug.LogError("MoPub init error: " + e.Message);
                return false;
            }
        }

        /// <summary>
        /// Gets a valid id for initialization.
        /// </summary>
        protected string GetInitializeId()
        {
            if (!string.IsNullOrEmpty(mAdSettings.DefaultBannerId.Id))
                return mAdSettings.DefaultBannerId.Id;

            if (!string.IsNullOrEmpty(mAdSettings.DefaultInterstitialAdId.Id))
                return mAdSettings.DefaultInterstitialAdId.Id;

            if (!string.IsNullOrEmpty(mAdSettings.DefaultRewardedAdId.Id))
                return mAdSettings.DefaultRewardedAdId.Id;

            return null;
        }

        ///<summary>
        /// Gets SDK Configuration for advanced initialization.
        /// </summary>
        protected MoPub.SdkConfiguration GetSdkConfiguration()
        {
            return new MoPub.SdkConfiguration
            {
                AdUnitId = GetInitializeId(),
                AllowLegitimateInterest = mAdSettings.AllowLegitimateInterest,
                LogLevel = mAdSettings.MoPubLogLevel.ToMoPubLogLevel(),
                MediatedNetworks = GetMediatedNetworks()
            };
        }

        protected MoPubBase.MediatedNetwork[] GetMediatedNetworks()
        {
            return mAdSettings.MediatedNetworks.Select(network => network.ToMoPubMediatedNetwork()).ToArray();
        }

        protected void LoadAllAdUnits()
        {
            string[] allBannerIds = GetAllIds(mAdSettings.DefaultBannerId.Id, mAdSettings.CustomBannerIds);
            string[] allInterstitialIds = GetAllIds(mAdSettings.DefaultInterstitialAdId.Id, mAdSettings.CustomInterstitialAdIds);
            string[] allRewardedVideoIds = GetAllIds(mAdSettings.DefaultRewardedAdId.Id, mAdSettings.CustomRewardedAdIds);

            MoPub.LoadBannerPluginsForAdUnits(allBannerIds);
            MoPub.LoadInterstitialPluginsForAdUnits(allInterstitialIds);
            MoPub.LoadRewardedVideoPluginsForAdUnits(allRewardedVideoIds);
        }

        protected string[] GetAllIds(string defaultId, Dictionary<AdPlacement, AdId> customIds)
        {
            List<string> allIds = new List<string>();

            if (defaultId != null)
                allIds.Add(defaultId);

            if (mAdSettings.CustomBannerIds != null)
            {
                foreach (var pair in customIds)
                {
                    if (pair.Value == null)
                        continue;

                    if (allIds.Contains(pair.Value.Id)) // Ignore duplicated values.
                        continue;

                    allIds.Add(pair.Value.Id);
                }
            }

            return allIds.ToArray();
        }

        protected void RegisterAllBannerEvents()
        {
            MoPubManager.OnAdClickedEvent += OnBannerAdClicked;
            MoPubManager.OnAdCollapsedEvent += OnBannerAdCollapsed;
            MoPubManager.OnAdExpandedEvent += OnBannerAdExpanded;
            MoPubManager.OnAdFailedEvent += OnBannerAdFailed;
            MoPubManager.OnAdLoadedEvent += OnBannerAdLoaded;
        }

        protected void RegisterAllInterstitialEvents()
        {
            MoPubManager.OnInterstitialClickedEvent += OnInterstitialAdClicked;
            MoPubManager.OnInterstitialDismissedEvent += OnInterstitialAdDismissed;
            MoPubManager.OnInterstitialExpiredEvent += OnInterstitialExpired;
            MoPubManager.OnInterstitialFailedEvent += OnInterstitialFailed;
            MoPubManager.OnInterstitialLoadedEvent += OnInterstitialLoaded;
            MoPubManager.OnInterstitialShownEvent += OnInterstitialShowed;
        }

        protected void RegisterAllRewardedVideoEvents()
        {
            MoPubManager.OnRewardedVideoClickedEvent += OnRewardedAdClicked;
            MoPubManager.OnRewardedVideoClosedEvent += OnRewardedAdClosed;
            MoPubManager.OnRewardedVideoExpiredEvent += OnRewardedAdExpired;
            MoPubManager.OnRewardedVideoFailedEvent += OnRewardedAdFailed;
            MoPubManager.OnRewardedVideoFailedToPlayEvent += OnRewardedAdFailedToPlay;
            MoPubManager.OnRewardedVideoLeavingApplicationEvent += OnRewardedAdLeavingApplication;
            MoPubManager.OnRewardedVideoLoadedEvent += OnRewardedAdLoaded;
            MoPubManager.OnRewardedVideoReceivedRewardEvent += OnRewardedAdRewardReceived;
            MoPubManager.OnRewardedVideoShownEvent += OnRewardedAdShowed;
        }

        protected void RegisterInitializedEvent()
        {
            MoPubManager.OnSdkInitializedEvent += OnSdkInitializedHandle;
        }

        #endregion

        #region Other methods

        protected MoPub.AdPosition ToMoPubAdPosition(BannerAdPosition adPosition)
        {
            switch (adPosition)
            {
                case BannerAdPosition.Top:
                    return MoPub.AdPosition.TopCenter;

                case BannerAdPosition.Bottom:
                    return MoPub.AdPosition.BottomCenter;

                case BannerAdPosition.TopLeft:
                    return MoPub.AdPosition.TopLeft;

                case BannerAdPosition.TopRight:
                    return MoPub.AdPosition.TopRight;

                case BannerAdPosition.BottomLeft:
                    return MoPub.AdPosition.BottomLeft;

                case BannerAdPosition.BottomRight:
                    return MoPub.AdPosition.BottomRight;

                default:
                    return MoPub.AdPosition.Centered;
            }
        }

        protected MoPub.MaxAdSize ToNearestMoPubBannerType(BannerAdSize adSize)
        {
            if (adSize.IsSmartBanner)
                return MoPub.MaxAdSize.Width320Height50;
            else if (adSize.Height < 70) // (50+90)/2
                return MoPub.MaxAdSize.Width320Height50;
            else if (adSize.Height < 170)   // (90 + 250)/2
                return MoPub.MaxAdSize.Width728Height90;
            else
                return MoPub.MaxAdSize.Width300Height250;
        }

        protected void CreateBannerAd(string id, BannerAdPosition position, BannerAdSize size)
        {
#if UNITY_ANDROID
            // MoPub doesn't allow setting banner type on Android, so we'll ignore 'size'.
            MoPub.RequestBanner(id, ToMoPubAdPosition(position));
#else
            if (size == BannerAdSize.SmartBanner)
                MoPub.RequestBanner(id, ToMoPubAdPosition(position));
            else
                MoPub.RequestBanner(id, ToMoPubAdPosition(position), ToNearestMoPubBannerType(size));
#endif
        }

        protected bool TryFindCustomBannerPlacementWithId(string id, out AdPlacement placement)
        {
            return TryFindCustomAdPlacementWithId(mAdSettings.CustomBannerIds, id, out placement);
        }

        protected bool TryFindCustomInterstitialAdPlacementWithId(string id, out AdPlacement placement)
        {
            return TryFindCustomAdPlacementWithId(mAdSettings.CustomInterstitialAdIds, id, out placement);
        }

        protected bool TryFindCustomRewardedVideoAdPlacementWithId(string id, out AdPlacement placement)
        {
            return TryFindCustomAdPlacementWithId(mAdSettings.CustomRewardedAdIds, id, out placement);
        }

        private bool TryFindCustomAdPlacementWithId(Dictionary<AdPlacement, AdId> customSource, string id, out AdPlacement placement)
        {
            if (customSource == null)
            {
                placement = null;
                return false;
            }

            foreach (var pair in customSource)
            {
                if (pair.Value != null && pair.Value.Id.Equals(id))
                {
                    placement = pair.Key;
                    return true;
                }
            }

            placement = null;
            return false;
        }

        /// <summary>
        /// Finds an interstitial ad with its unit id and set its loaded status if available.
        /// </summary>
        /// <param name="loadedStatus">The status to check if the ad has been loaded or not.</param>
        /// <param name="adUnitId">Id of the ad unit.</param>
        /// <param name="adNotFoundMessage">Message to display when we couldn't find any ad with given id.</param>
        protected void SetInterstitialAdLoadedState(bool loadedStatus, string adUnitId,
                                                    string adNotFoundMessage = "SetInterstitialAdLoadedState. Error: ad not found.")
        {
            /// If the interstitial ad is the default one.
            if (adUnitId.Equals(mAdSettings.DefaultInterstitialAdId.Id))
            {
                mIsDefaultInterstitialAdReady = loadedStatus;
                return;
            }

            /// If the interstitial ad is a custom one.
            AdPlacement placement;
            if (TryFindCustomInterstitialAdPlacementWithId(adUnitId, out placement) && mCustomInterstitialAds.ContainsKey(placement))
            {
                mCustomInterstitialAds[placement] = loadedStatus;
                return;
            }

            Debug.LogWarning(adNotFoundMessage + ". Id: " + adUnitId);
        }

        #endregion

        #region GDPR Consent

        protected void OnSdkInitializedHandle(string param)
        {
            Debug.Log("MoPub client has been initialized.");

            MoPubManager.OnConsentDialogLoadedEvent += OnConsentDialogLoadedEventHandle;

            if (mAdSettings.AutoRequestConsent)
            {
                RequestGdprConsent();
            }
        }

        /// <summary>
        /// Loads and shows GDPR consent dialog if applicable.
        /// </summary>
        public void RequestGdprConsent()
        {
            if (!CheckInitialize() || mAdSettings == null)
                return;

            if (mAdSettings.ForceGdprApplicable)
            {
                ForceGdprApplicable();
            }

            bool? isGdprAvailable = IsGdprApplicable;

            if (isGdprAvailable == null)
            {
                Debug.LogWarning("IsGDPRAvailable is null!!!");
                return;
            }

            if (!isGdprAvailable.Value)
            {
                Debug.LogWarning("IsGDPRAvailable value is false: " + isGdprAvailable.Value);
                return;
            }

            if (!ShouldShowConsentDialog)
            {
                Debug.LogWarning("ShouldShowConsentDialog is false: " + ShouldShowConsentDialog);
                return;
            }

            LoadConsentDialog();
        }

        protected void OnConsentDialogLoadedEventHandle()
        {
            if (mAdSettings.AutoRequestConsent)
            {
                ShowConsentDialog();
            }
        }

        #region Public APIs

        /// <summary>
        /// Fired when the SDK has finished loading (retrieving from the web) the MoPub consent dialog interstitial.
        /// </summary>
        public event Action OnConsentDialogLoadedEvent
        {
            add { MoPubManager.OnConsentDialogLoadedEvent += value; }
            remove { MoPubManager.OnConsentDialogLoadedEvent -= value; }
        }

        /// <summary>
        /// Fired when an error occurred while attempting to load the MoPub consent dialog.
        /// </summary>
        public event Action<string> OnConsentDialogFailedEvent
        {
            add { MoPubManager.OnConsentDialogFailedEvent += value; }
            remove { MoPubManager.OnConsentDialogFailedEvent -= value; }
        }

        /// <summary>
        /// Fired when the SDK has been notified of a change in the user's consent status for data tracking.
        /// </summary>
        public event Action<MoPub.Consent.Status, MoPub.Consent.Status, bool> OnConsentStatusChangedEvent
        {
            add { MoPubManager.OnConsentStatusChangedEvent += value; }
            remove { MoPubManager.OnConsentStatusChangedEvent -= value; }
        }

        /// <summary>
        /// Fired when the MoPub consent dialog has been presented on screen.
        /// </summary>
        public event Action OnConsentDialogShown
        {
            add { MoPubManager.OnConsentDialogShownEvent += value; }
            remove { MoPubManager.OnConsentDialogShownEvent -= value; }
        }

        /// <summary>
        /// Notifies the MoPub SDK that this user has granted consent to this app.
        /// </summary>
        public void GrantConsent()
        {
            MoPub.PartnerApi.GrantConsent();
        }

        /// <summary>
        /// Notifies the MoPub SDK that this user has denied consent to this app.
        /// </summary>
        public void RevokeConsent()
        {
            MoPub.PartnerApi.RevokeConsent();
        }

        /// <summary>
        /// Whether or not this app is allowed to collect Personally Identifiable Information (PII) from the user.
        /// </summary>
        public bool CanCollectPersonalInfo { get { return MoPub.CanCollectPersonalInfo; } }

        /// <summary>
        /// The user's current consent state for the app to collect Personally Identifiable Information (PII).
        /// </summary>
        public MoPub.Consent.Status CurrentConsentStatus { get { return MoPub.CurrentConsentStatus; } }

        /// <summary>
        /// Checks to see if a publisher should load and then show a consent dialog.
        /// </summary>
        public bool ShouldShowConsentDialog { get { return MoPub.ShouldShowConsentDialog; } }

        /// <summary>
        /// Sends off an asynchronous network request to load the MoPub consent dialog.
        /// </summary>
        public void LoadConsentDialog()
        {
            MoPub.LoadConsentDialog();
        }

        /// <summary>
        /// Flag indicating whether the MoPub consent dialog is currently loaded and showable.
        /// </summary>
        public bool IsConsentDialogReady { get { return MoPub.IsConsentDialogReady; } }

        /// <summary>
        /// Flag indicating whether the MoPub consent dialog is currently loaded and showable.
        /// </summary>
        [Obsolete("Use IsConsentDialogReady instead.")]
        public bool IsConsentDialogLoaded { get { return IsConsentDialogReady; } }

        /// <summary>
        /// If the MoPub consent dialog is loaded, this will take over the screen and show it.
        /// </summary>
        public void ShowConsentDialog()
        {
            MoPub.ShowConsentDialog();
        }

        /// <summary>
        /// Flag indicating whether data collection is subject to the General Data Protection Regulation (GDPR).
        /// Returns true for Yes, False for No, Null for Unknown (from startup until server responds during SDK initialization).
        /// </summary>
        public bool? IsGdprApplicable { get { return MoPub.IsGdprApplicable; } }

        /// <summary>
        /// Forces the SDK to treat this app as in a GDPR region. Setting this will permanently force GDPR rules for this
        /// user unless this app is uninstalled or the data for this app is cleared.
        /// </summary>
        public void ForceGdprApplicable()
        {
            MoPub.ForceGdprApplicable();
        }

        /// <summary>
        /// Set this to an ISO language code (e.g., "en-US") if you wish 
        /// <see cref="CurrentConsentPrivacyPolicyUrl"/> and <see cref="CurrentVendorListUrl"/>
        /// to point to a web resource that is localized to a specific language.
        /// </summary>
        public string ConsentLanguageCode
        {
            get { return MoPub.ConsentLanguageCode; }
            set { MoPub.ConsentLanguageCode = value; }
        }

        /// <summary>
        /// The URL for the privacy policy this user has consented to.
        /// </summary>
        public Uri CurrentConsentPrivacyPolicyUrl { get { return MoPub.PartnerApi.CurrentConsentPrivacyPolicyUrl; } }

        /// <summary>
        /// The URL for the list of vendors this user has consented to.
        /// </summary>
        public Uri CurrentVendorListUrl { get { return MoPub.PartnerApi.CurrentVendorListUrl; } }

        /// <summary>
        /// The list of vendors this user has consented to in IAB format.
        /// </summary>
        public string CurrentConsentIabVendorListFormat { get { return MoPub.PartnerApi.CurrentConsentIabVendorListFormat; } }

        /// <summary>
        /// The version for the privacy policy this user has consented to.
        /// </summary>
        public string CurrentConsentPrivacyPolicyVersion { get { return MoPub.PartnerApi.CurrentConsentPrivacyPolicyVersion; } }

        /// <summary>
        /// The version for the list of vendors this user has consented to.
        /// </summary>
        public string CurrentConsentVendorListVersion { get { return MoPub.PartnerApi.CurrentConsentVendorListVersion; } }

        /// <summary>
        /// The list of vendors this user has previously consented to in IAB format.
        /// </summary>
        public string PreviouslyConsentedIabVendorListFormat { get { return MoPub.PartnerApi.PreviouslyConsentedIabVendorListFormat; } }

        /// <summary>
        /// The version for the privacy policy this user has previously consented to.
        /// </summary>
        public string PreviouslyConsentedPrivacyPolicyVersion { get { return MoPub.PartnerApi.PreviouslyConsentedPrivacyPolicyVersion; } }

        /// <summary>
        /// The version for the vendor list this user has previously consented to.
        /// </summary>
        public string PreviouslyConsentedVendorListVersion { get { return MoPub.PartnerApi.PreviouslyConsentedVendorListVersion; } }

        #endregion

        #endregion

#endif

    }
}
