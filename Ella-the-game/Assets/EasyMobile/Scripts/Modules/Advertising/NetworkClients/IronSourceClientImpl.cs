using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile
{
    public class IronSourceClientImpl : AdClientImpl
    {
        private const string NO_SDK_MESSAGE = "SDK missing. Please import the ironSource plugin.";

#if EM_IRONSOURCE

        protected IronSourceSettings mAdSettings;
        protected bool mIsBannerAdLoaded = false;
        protected IronSourceBannerSize mCurrentBannerAdSize = IronSourceBannerSize.SMART;
        protected IronSourceBannerPosition mCurrentBannerAdPos = IronSourceBannerPosition.BOTTOM;
        protected AdPlacement mCurrentBannerAdPlacement = null;

        protected bool mRewardedVideoIsCompleted = false;

        /// <summary>
        /// The lastest <see cref="AdPlacement"/> requested through the <see cref="InternalShowRewardedAd"/> method.
        /// </summary>
        protected AdPlacement mLastShownRewardedAdPlacement = AdPlacement.Default;

        /// <summary>
        /// The lastest <see cref="AdPlacement"/> requested through the <see cref="InternalShowInterstitialAd"/> method.
        /// </summary>
        protected AdPlacement mLastShownInterstitialPlacement = AdPlacement.Default;

#endif

        #region IronSource Events

#if EM_IRONSOURCE

        // ******************************* Rewarded Ad Events ******************************* //

        /// <summary>
        /// Occurs when a rewarded video ad failed to show.
        /// </summary>
        public event Action<IronSourceError> OnRewardedVideoAdShowFailedEvent
        {
            add { IronSourceEvents.onRewardedVideoAdShowFailedEvent += value; }
            remove { IronSourceEvents.onRewardedVideoAdShowFailedEvent -= value; }
        }

        /// <summary>
        /// Occurs when a rewarded video ad is opened.
        /// </summary>
        public event Action OnRewardedVideoAdOpenedEvent
        {
            add { IronSourceEvents.onRewardedVideoAdOpenedEvent += value; }
            remove { IronSourceEvents.onRewardedVideoAdOpenedEvent -= value; }
        }

        /// <summary>
        /// Occurs when a rewarded video ad is closed.
        /// </summary>
        public event Action OnRewardedVideoAdClosedEvent
        {
            add { IronSourceEvents.onRewardedVideoAdClosedEvent += value; }
            remove { IronSourceEvents.onRewardedVideoAdClosedEvent -= value; }
        }

        /// <summary>
        /// Occurs when a rewarded video ad started.
        /// </summary>
        public event Action OnRewardedVideoAdStartedEvent
        {
            add { IronSourceEvents.onRewardedVideoAdStartedEvent += value; }
            remove { IronSourceEvents.onRewardedVideoAdStartedEvent -= value; }
        }

        /// <summary>
        /// Occurs when a rewarded video ad ended.
        /// </summary>
        public event Action OnRewardedVideoAdEndedEvent
        {
            add { IronSourceEvents.onRewardedVideoAdEndedEvent += value; }
            remove { IronSourceEvents.onRewardedVideoAdEndedEvent -= value; }
        }

        /// <summary>
        /// Occurs when a rewarded video ad rewarded.
        /// </summary>
        public event Action<IronSourcePlacement> OnRewardedVideoAdRewardedEvent
        {
            add { IronSourceEvents.onRewardedVideoAdRewardedEvent += value; }
            remove { IronSourceEvents.onRewardedVideoAdRewardedEvent -= value; }
        }

        /// <summary>
        /// Occurs when a rewarded video ad is clicked.
        /// </summary>
        public event Action<IronSourcePlacement> OnRewardedVideoAdClickedEvent
        {
            add { IronSourceEvents.onRewardedVideoAdClickedEvent += value; }
            remove { IronSourceEvents.onRewardedVideoAdClickedEvent -= value; }
        }

        /// <summary>
        /// Occurs when a rewarded video's availability changed.
        /// </summary>
        public event Action<bool> OnRewardedVideoAvailabilityChangedEvent
        {
            add { IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += value; }
            remove { IronSourceEvents.onRewardedVideoAvailabilityChangedEvent -= value; }
        }

        // ******************************* Interstitial Ad Events ******************************* //

        /// <summary>
        /// Occurs when an interstitial ad is ready.
        /// </summary>
        public event Action OnInterstitialAdReadyEvent
        {
            add { IronSourceEvents.onInterstitialAdReadyEvent += value; }
            remove { IronSourceEvents.onInterstitialAdReadyEvent -= value; }
        }

        /// <summary>
        /// Occurs when an interstitial ad failed to load.
        /// </summary>
        public event Action<IronSourceError> OnInterstitialAdLoadFailedEvent
        {
            add { IronSourceEvents.onInterstitialAdLoadFailedEvent += value; }
            remove { IronSourceEvents.onInterstitialAdLoadFailedEvent -= value; }
        }

        /// <summary>
        /// Occurs when an interstitial ad opened.
        /// </summary>
        public event Action OnInterstitialAdOpenedEvent
        {
            add { IronSourceEvents.onInterstitialAdOpenedEvent += value; }
            remove { IronSourceEvents.onInterstitialAdOpenedEvent -= value; }
        }

        /// <summary>
        /// Occurs when an interstitial ad closed.
        /// </summary>
        public event Action OnInterstitialAdClosedEvent
        {
            add { IronSourceEvents.onInterstitialAdClosedEvent += value; }
            remove { IronSourceEvents.onInterstitialAdClosedEvent -= value; }
        }

        /// <summary>
        /// Occurs when an interstitial ad shown successfully.
        /// </summary>
        public event Action OnInterstitialAdShowSucceededEvent
        {
            add { IronSourceEvents.onInterstitialAdShowSucceededEvent += value; }
            remove { IronSourceEvents.onInterstitialAdShowSucceededEvent -= value; }
        }

        /// <summary>
        /// Occurs when an interstitial ad failed to show.
        /// </summary>
        public event Action<IronSourceError> OnInterstitialAdShowFailedEvent
        {
            add { IronSourceEvents.onInterstitialAdShowFailedEvent += value; }
            remove { IronSourceEvents.onInterstitialAdShowFailedEvent -= value; }
        }

        /// <summary>
        /// Occurs when an interstitial ad was clicked.
        /// </summary>
        public event Action OnInterstitialAdClickedEvent
        {
            add { IronSourceEvents.onInterstitialAdClickedEvent += value; }
            remove { IronSourceEvents.onInterstitialAdClickedEvent -= value; }
        }

        // ******************************* Banner Ad Events ******************************* //

        /// <summary>
        /// Occurs when a banner ad loaded.
        /// </summary>
        public event Action OnBannerAdLoadedEvent
        {
            add { IronSourceEvents.onBannerAdLoadedEvent += value; }
            remove { IronSourceEvents.onBannerAdLoadedEvent -= value; }
        }

        /// <summary>
        /// Occurs when a banner ad failed to load.
        /// </summary>
        public event Action<IronSourceError> OnBannerAdLoadedFailedEvent
        {
            add { IronSourceEvents.onBannerAdLoadFailedEvent += value; }
            remove { IronSourceEvents.onBannerAdLoadFailedEvent -= value; }
        }

        /// <summary>
        /// Occurs when a banner ad was clicked.
        /// </summary>
        public event Action OnBannerAdClickedEvent
        {
            add { IronSourceEvents.onBannerAdClickedEvent += value; }
            remove { IronSourceEvents.onBannerAdClickedEvent -= value; }
        }

        /// <summary>
        /// Occurs when a banner ad has been presented.
        /// </summary>
        public event Action OnBannerAdScreenPresentedEvent
        {
            add { IronSourceEvents.onBannerAdScreenPresentedEvent += value; }
            remove { IronSourceEvents.onBannerAdScreenPresentedEvent -= value; }
        }

        /// <summary>
        /// Occurs when a banner ad was dismissed.
        /// </summary>
        public event Action OnBannerAdScreenDismissedEvent
        {
            add { IronSourceEvents.onBannerAdScreenDismissedEvent += value; }
            remove { IronSourceEvents.onBannerAdScreenDismissedEvent -= value; }
        }

        /// <summary>
        /// Occurs when the user leaves your app after clicking a banner ad.
        /// </summary>
        public event Action OnBannerAdLeftApplicationEvent
        {
            add { IronSourceEvents.onBannerAdLeftApplicationEvent += value; }
            remove { IronSourceEvents.onBannerAdLeftApplicationEvent -= value; }
        }

#endif

        #endregion  // ironSource-Specific Events

        #region Singleton

        private static IronSourceClientImpl sInstance;

        private IronSourceClientImpl()
        {
        }

        public static IronSourceClientImpl CreateClient()
        {
            if (sInstance == null)
            {
                sInstance = new IronSourceClientImpl();
            }
            return sInstance;
        }

        #endregion

        #region AdClient Overrides

        public override AdNetwork Network { get { return AdNetwork.IronSource; } }

        public override bool IsBannerAdSupported { get { return true; } }

        public override bool IsInterstitialAdSupported { get { return true; } }

        public override bool IsRewardedAdSupported { get { return true; } }

        public override bool IsSdkAvail
        {
            get
            {
#if EM_IRONSOURCE
                return true;
#else
                return false;
#endif
            }
        }

        public override bool IsValidPlacement(AdPlacement placement, AdType type)
        {
#if EM_IRONSOURCE
            return true;
#else
            return false;
#endif
        }

        protected override Dictionary<AdPlacement, AdId> CustomInterstitialAdsDict { get { return null; } }

        protected override Dictionary<AdPlacement, AdId> CustomRewardedAdsDict { get { return null; } }

        protected override string NoSdkMessage { get { return NO_SDK_MESSAGE; } }

        protected override void InternalInit()
        {
#if EM_IRONSOURCE

            mIsInitialized = true;
            mAdSettings = EM_Settings.Advertising.IronSource;

            // Set GDPR consent (if any) *before* initializing the SDK as recommended at
            // https://developers.ironsrc.com/ironsource-mobile/android/advanced-settings/#step-1
            var consent = GetApplicableDataPrivacyConsent();
            ApplyDataPrivacyConsent(consent);

            // Advanced settings.
            if (mAdSettings.UseAdvancedSetting)
            {
                SetupAdvancedSetting(mAdSettings);
            }

            // IronSource requires a gameObject to pass the OnApplicationPause event to it agent.
            GameObject appStateHandler = new GameObject("IronSourceAppStateHandler");
            appStateHandler.hideFlags = HideFlags.HideAndDontSave;
            appStateHandler.AddComponent<Internal.IronSourceAppStateHandler>();

            IronSource.Agent.init(mAdSettings.AppId.Id);

            /// Add event callbacks.
            IronSourceEvents.onBannerAdClickedEvent += OnBannerAdClicked;
            IronSourceEvents.onBannerAdLeftApplicationEvent += OnBannerAdLeftApplication;
            IronSourceEvents.onBannerAdLoadedEvent += OnBannerAdLoaded;
            IronSourceEvents.onBannerAdLoadFailedEvent += OnBannerAdLoadFailed;

            IronSourceEvents.onInterstitialAdClickedEvent += OnInterstitialAdClicked;
            IronSourceEvents.onInterstitialAdClosedEvent += OnInterstititalAdClosed;
            IronSourceEvents.onInterstitialAdLoadFailedEvent += OnInterstitialAdLoadFailed;
            IronSourceEvents.onInterstitialAdOpenedEvent += OnInterstitialAdOpened;
            IronSourceEvents.onInterstitialAdReadyEvent += OnInterstitialAdReady;
            IronSourceEvents.onInterstitialAdShowSucceededEvent += OnInterstitialAdShowSucceeded;
            IronSourceEvents.onInterstitialAdShowFailedEvent += OnInterstitialAdShowFailed;

            IronSourceEvents.onRewardedVideoAdClickedEvent += OnRewardedVideoAdClicked;
            IronSourceEvents.onRewardedVideoAdClosedEvent += OnRewardedVideoClosed;
            IronSourceEvents.onRewardedVideoAdEndedEvent += OnRewardedVideoAdEnded;
            IronSourceEvents.onRewardedVideoAdOpenedEvent += OnRewardedVideoAdOpened;
            IronSourceEvents.onRewardedVideoAdRewardedEvent += OnRewardedVideoAdRewarded;
            IronSourceEvents.onRewardedVideoAdShowFailedEvent += OnRewardedVideoAdShowFailed;
            IronSourceEvents.onRewardedVideoAdStartedEvent += OnRewardedVideoAdStarted;
            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += OnRewardedVideoAvailabilityChanged;

            IronSourceEvents.onSegmentReceivedEvent += OnSegmentReceived;

            Debug.Log("ironSource client has been initialized.");
#endif
        }

        protected override void InternalShowBannerAd(AdPlacement placement, BannerAdPosition position, BannerAdSize size)
        {
#if EM_IRONSOURCE
            // If player requests a banner with different position or size,
            // we have to load a new banner.
            var newPos = ToIronSourceBannerPosition(position);
            var newSize = ToIronSourceBannerSize(size);

            if (mCurrentBannerAdPlacement != placement)
            {
                mCurrentBannerAdPlacement = placement;
                mIsBannerAdLoaded = false;
            }

            if (mCurrentBannerAdPos != newPos)
            {
                mCurrentBannerAdPos = newPos;
                mIsBannerAdLoaded = false;
            }

            if (mCurrentBannerAdSize != newSize)
            {
                mCurrentBannerAdSize = newSize;
                mIsBannerAdLoaded = false;
            }

            if (!mIsBannerAdLoaded)
            {
                string placementName = ToIronSourcePlacementName(mCurrentBannerAdPlacement);

                if (string.IsNullOrEmpty(placementName))
                    IronSource.Agent.loadBanner(mCurrentBannerAdSize, mCurrentBannerAdPos);
                else
                    IronSource.Agent.loadBanner(mCurrentBannerAdSize, mCurrentBannerAdPos, placementName);
            }

            IronSource.Agent.displayBanner();
#endif
        }

        protected override void InternalHideBannerAd(AdPlacement _)
        {
#if EM_IRONSOURCE
            IronSource.Agent.hideBanner();
#endif
        }

        protected override void InternalDestroyBannerAd(AdPlacement _)
        {
#if EM_IRONSOURCE
            IronSource.Agent.destroyBanner();
            mIsBannerAdLoaded = false;
#endif
        }

        protected override bool InternalIsInterstitialAdReady(AdPlacement _)
        {
#if EM_IRONSOURCE
            return IronSource.Agent.isInterstitialReady();
#else
            return false;
#endif
        }

        protected override void InternalLoadInterstitialAd(AdPlacement _)
        {
#if EM_IRONSOURCE
            IronSource.Agent.loadInterstitial();
#endif
        }

        protected override void InternalShowInterstitialAd(AdPlacement placement)
        {
#if EM_IRONSOURCE
            mLastShownInterstitialPlacement = placement;

            string placementName = ToIronSourcePlacementName(placement);

            if (string.IsNullOrEmpty(placementName))
                IronSource.Agent.showInterstitial();
            else
                IronSource.Agent.showInterstitial(placementName);
#endif
        }

        protected override bool InternalIsRewardedAdReady(AdPlacement _)
        {
#if EM_IRONSOURCE
            return IronSource.Agent.isRewardedVideoAvailable();
#else
            return false;
#endif
        }

        protected override void InternalLoadRewardedAd(AdPlacement _)
        {
            // IronSource loads rewarded video ads in the background automatically,
            // so we don't need to do anything here.
        }

        protected override void InternalShowRewardedAd(AdPlacement placement)
        {
#if EM_IRONSOURCE
            mLastShownRewardedAdPlacement = placement;

            string placementName = ToIronSourcePlacementName(placement);

            if (string.IsNullOrEmpty(placementName))
                IronSource.Agent.showRewardedVideo();
            else
                IronSource.Agent.showRewardedVideo(placementName);
#endif
        }

        #endregion

        #region IConsentRequirable Overrides

        private const string DATA_PRIVACY_CONSENT_KEY = "EM_Ads_IronSource_DataPrivacyConsent";

        protected override string DataPrivacyConsentSaveKey { get { return DATA_PRIVACY_CONSENT_KEY; } }

        protected override void ApplyDataPrivacyConsent(ConsentStatus consent)
        {
#if EM_IRONSOURCE
            switch (consent)
            {
                case ConsentStatus.Granted:
                    IronSource.Agent.setConsent(true);
                    break;
                case ConsentStatus.Revoked:
                    IronSource.Agent.setConsent(false);
                    break;
                case ConsentStatus.Unknown:
                default:
                    break;
            }
#endif
        }

        #endregion

        #region Helper Methods

#if EM_IRONSOURCE

        protected string ToIronSourcePlacementName(AdPlacement placement)
        {
            return placement == null || placement == AdPlacement.Default ? null : placement.Name;
        }

        protected IronSourceBannerSize ToIronSourceBannerSize(IronSourceSettings.IronSourceBannerType bannerType)
        {
            switch (bannerType)
            {
                case IronSourceSettings.IronSourceBannerType.Banner:
                    return IronSourceBannerSize.BANNER;

                case IronSourceSettings.IronSourceBannerType.LargeBanner:
                    return IronSourceBannerSize.LARGE;

                case IronSourceSettings.IronSourceBannerType.RectangleBanner:
                    return IronSourceBannerSize.RECTANGLE;

                case IronSourceSettings.IronSourceBannerType.SmartBanner:
                    return IronSourceBannerSize.SMART;

                default:
                    return IronSourceBannerSize.BANNER;
            }
        }

        protected IronSourceBannerPosition ToIronSourceBannerPosition(BannerAdPosition pos)
        {
            switch (pos)
            {
                case BannerAdPosition.Top:
                case BannerAdPosition.TopLeft:
                case BannerAdPosition.TopRight:
                    return IronSourceBannerPosition.TOP;

                case BannerAdPosition.Bottom:
                case BannerAdPosition.BottomLeft:
                case BannerAdPosition.BottomRight:
                default:
                    return IronSourceBannerPosition.BOTTOM;
            }
        }

        protected IronSourceBannerSize ToIronSourceBannerSize(BannerAdSize adSize)
        {
            return adSize.IsSmartBanner ? IronSourceBannerSize.SMART : ToIronSourceNearestSize(adSize);
        }

        protected virtual IronSourceBannerSize ToIronSourceNearestSize(BannerAdSize adSize)
        {
            if (adSize.Height < 70) // (50+90)/2
                return IronSourceBannerSize.BANNER; // screen width x 50
            else if (adSize.Height < 170)   // (90 + 250)/2
                return IronSourceBannerSize.LARGE;   // screen width x 90
            else
                return IronSourceBannerSize.RECTANGLE;   // screen width x 250
        }

#endif

        #endregion

        #region Advanced Settings

        protected virtual void SetupAdvancedSetting(IronSourceSettings adSettings)
        {
#if EM_IRONSOURCE
            SetupSegment(adSettings.Segments);
#endif
        }

        protected virtual void SetupSegment(IronSourceSettings.SegmentSettings segmentSettings)
        {
#if EM_IRONSOURCE
            if (segmentSettings == null)
            {
                Debug.LogError("SengmentSettings is null!!!");
                return;
            }

            IronSourceSegment newSegment = segmentSettings.ToIronSourceSegment();
            if (newSegment == null)
            {
                Debug.LogError("Segment is null!!!");
                return;
            }

            IronSource.Agent.setSegment(newSegment);
#endif
        }

        #endregion

        #region Ad Event Handlers

#if EM_IRONSOURCE

        private void OnBannerAdClicked()
        {

        }

        private void OnBannerAdLeftApplication()
        {

        }

        private void OnBannerAdLoaded()
        {
            mIsBannerAdLoaded = true;
            Debug.Log("Banner ad is loaded.");
        }

        private void OnBannerAdLoadFailed(IronSourceError error)
        {
            mIsBannerAdLoaded = false;
            Debug.Log("Failed to load banner ad. Error: " + error);
        }

        private void OnInterstitialAdClicked()
        {

        }

        private void OnInterstititalAdClosed()
        {
            OnInterstitialAdCompleted(mLastShownInterstitialPlacement);
            mLastShownInterstitialPlacement = AdPlacement.Default;
        }

        private void OnInterstitialAdLoadFailed(IronSourceError error)
        {
            Debug.Log("Failed to load interstitial ad. Error: " + error);
        }

        private void OnInterstitialAdOpened()
        {

        }

        private void OnInterstitialAdReady()
        {

        }

        private void OnInterstitialAdShowSucceeded()
        {
        }

        private void OnInterstitialAdShowFailed(IronSourceError error)
        {
            Debug.Log("Failed to show interstitial ad. Error: " + error);
        }

        private void OnRewardedVideoAdClicked(IronSourcePlacement obj)
        {

        }

        private void OnRewardedVideoClosed()
        {
            if (!mRewardedVideoIsCompleted)
            {
                OnRewardedAdSkipped(mLastShownRewardedAdPlacement);
            }
            else
            {
                OnRewardedAdCompleted(mLastShownRewardedAdPlacement);
            }

            mRewardedVideoIsCompleted = false;
            mLastShownRewardedAdPlacement = AdPlacement.Default;
        }

        private void OnRewardedVideoAdEnded()
        {

        }

        private void OnRewardedVideoAdOpened()
        {
            mRewardedVideoIsCompleted = false;
        }

        private void OnRewardedVideoAdRewarded(IronSourcePlacement placement)
        {
            if (placement == null)
                return;

            mRewardedVideoIsCompleted = true;
        }

        private void OnRewardedVideoAdShowFailed(IronSourceError error)
        {
            Debug.Log("Failed to show rewarded video ad. Error: " + error);
            mRewardedVideoIsCompleted = false;
        }

        private void OnRewardedVideoAdStarted()
        {
            mRewardedVideoIsCompleted = false;
        }

        private void OnRewardedVideoAvailabilityChanged(bool obj)
        {

        }

        private void OnSegmentReceived(string segment)
        {
            Debug.Log("Received segment: " + segment);
        }

#endif

        #endregion

    }
}

namespace EasyMobile.Internal
{
    /// <summary>
    /// An component used to pass the OnApplicationPause event to IronSource agent.
    /// </summary>
    public sealed class IronSourceAppStateHandler : MonoBehaviour
    {
#if EM_IRONSOURCE
        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private void OnApplicationPause(bool pause)
        {
            IronSource.Agent.onApplicationPause(pause);
        }
#endif
    }
}
