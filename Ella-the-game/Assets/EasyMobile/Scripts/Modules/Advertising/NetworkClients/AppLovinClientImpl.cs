using UnityEngine;
using System;
using System.Collections.Generic;

namespace EasyMobile
{
#if EM_APPLOVIN
    using EasyMobile.Internal;
#endif

    public class AppLovinClientImpl : AdClientImpl
    {
        private const string NO_SDK_MESSAGE = "SDK missing. Please import the AppLovin plugin.";
        private const string DATA_PRIVACY_CONSENT_KEY = "EM_Ads_AppLovin_DataPrivacyConsent";

#if EM_APPLOVIN       

        private AppLovinSettings mAdSettings = null;
        private AdPlacement currentShowingInterstitial = null;
        private AdPlacement currentShowingRewarded = null;
        private Dictionary<AdPlacement, bool> rewardedCompleted = new Dictionary<AdPlacement, bool>();

        /// <summary>
        /// We're gonna save all the loaded banners here so that we can re use them later.
        /// </summary>
        /// Key: The AdPlacement used to load the banner.
        /// Value: Loaded banner's position & size.
        private Dictionary<AdPlacement, KeyValuePair<BannerAdPosition, BannerAdSize>> mCreatedBanners;
#endif
        #region Singleton

        private static AppLovinClientImpl sInstance;

        private AppLovinClientImpl()
        {
        }

        /// <summary>
        /// Returns the singleton client.
        /// </summary>
        /// <returns>The client.</returns>
        public static AppLovinClientImpl CreateClient()
        {
            if (sInstance == null)
            {
                sInstance = new AppLovinClientImpl();
            }
            return sInstance;
        }

        #endregion  // Object Creators

        #region IAdClient Overrides

        public override AdNetwork Network { get { return AdNetwork.AppLovin; } }

        public override bool IsBannerAdSupported { get { return true; } }

        public override bool IsInterstitialAdSupported { get { return true; } }

        public override bool IsRewardedAdSupported { get { return true; } }

        public override bool IsSdkAvail
        {
            get
            {
#if EM_APPLOVIN
                return true;
#else
                return false;
#endif
            }
        }

        public override bool IsValidPlacement(AdPlacement placement, AdType type)
        {
#if EM_APPLOVIN
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
                        id = mAdSettings.DefaultBannerAdId.Id;
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
                        id = FindIdForPlacement(mAdSettings.CustomBannerAdIds, placement);
                        break;
                }
            }

            return true;
#else
            return false;
#endif
        }

        protected override string NoSdkMessage
        {
            get { return NO_SDK_MESSAGE; }
        }

        protected override Dictionary<AdPlacement, AdId> CustomInterstitialAdsDict
        {
            get
            {
#if EM_APPLOVIN
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
#if EM_APPLOVIN
                return mAdSettings == null ? null : mAdSettings.CustomRewardedAdIds;
#else
                return null;
#endif
            }
        }

        protected override void InternalInit()
        {
#if EM_APPLOVIN
            mAdSettings = EM_Settings.Advertising.AppLovin;
            mCreatedBanners = new Dictionary<AdPlacement, KeyValuePair<BannerAdPosition, BannerAdSize>>();

            // Set GDPR consent if any.
            var consent = GetApplicableDataPrivacyConsent();
            ApplyDataPrivacyConsent(consent);

            MaxSdk.SetSdkKey(mAdSettings.SDKKey);
            MaxSdk.InitializeSdk();

            // Age-Restricted 
            if (mAdSettings.AgeRestrictMode)
                MaxSdk.SetIsAgeRestrictedUser(true);
            else
                MaxSdk.SetIsAgeRestrictedUser(false);

            //Create ad unit event listener
            new InterstitialEvents(this);
            new RewardedAdEvents(this);
            new BannerEvents(this);

            LoadInterstitialAd();
            LoadRewardedAd();

            // Done Initialization.
            mIsInitialized = true;
            Debug.Log("AppLovin client has been initialized.");
#endif
        }

        protected override string DataPrivacyConsentSaveKey { get { return DATA_PRIVACY_CONSENT_KEY; } }

        protected override void ApplyDataPrivacyConsent(ConsentStatus consent)
        {
#if EM_APPLOVIN
            switch (consent)
            {
                case ConsentStatus.Granted:
                    MaxSdk.SetHasUserConsent(true);
                    break;
                case ConsentStatus.Revoked:
                    MaxSdk.SetHasUserConsent(false);
                    break;
                case ConsentStatus.Unknown:
                default:
                    break;
            }
#endif
        }

        //------------------------------------------------------------
        // Banner Ads.
        //------------------------------------------------------------

        protected override void InternalShowBannerAd(AdPlacement placement, BannerAdPosition position, BannerAdSize size)
        {
#if EM_APPLOVIN
            string id = placement == AdPlacement.Default ?
                mAdSettings.DefaultBannerAdId.Id :
                FindIdForPlacement(mAdSettings.CustomBannerAdIds, placement);

            if (string.IsNullOrEmpty(id))
                return;

            bool shouldCreateNewBanner = true;

            if (mCreatedBanners.ContainsKey(placement)
            && mCreatedBanners[placement].Key == position
            && mCreatedBanners[placement].Value == size)
            {
                shouldCreateNewBanner = false;
            }

            if (shouldCreateNewBanner)
            {
                DestroyBannerAd(placement);
                MaxSdk.CreateBanner(id, ToAppLovinAdPosition(position));
                MaxSdk.SetBannerBackgroundColor(id, new Color(1, 1, 1, 0));

                if (!mCreatedBanners.ContainsKey(placement))
                    mCreatedBanners.Add(placement, new KeyValuePair<BannerAdPosition, BannerAdSize>(position, size));
                mCreatedBanners[placement] = new KeyValuePair<BannerAdPosition, BannerAdSize>(position, size);
            }

            MaxSdk.ShowBanner(id);
#endif
        }

        protected override void InternalHideBannerAd(AdPlacement placement)
        {
#if EM_APPLOVIN
            string id =
            placement == AdPlacement.Default ?
                mAdSettings.DefaultBannerAdId.Id :
                FindIdForPlacement(mAdSettings.CustomBannerAdIds, placement);
            if (string.IsNullOrEmpty(id))
                return;
            MaxSdk.HideBanner(id);
#endif
        }

        protected override void InternalDestroyBannerAd(AdPlacement placement)
        {
#if EM_APPLOVIN
            string id =
            placement == AdPlacement.Default ?
                mAdSettings.DefaultBannerAdId.Id :
                FindIdForPlacement(mAdSettings.CustomBannerAdIds, placement);
            if (string.IsNullOrEmpty(id))
                return;
            MaxSdk.DestroyBanner(id);
            mCreatedBanners.Remove(placement);
#endif
        }

        //------------------------------------------------------------
        // Interstitial Ads.
        //------------------------------------------------------------

        protected override bool InternalIsInterstitialAdReady(AdPlacement placement)
        {
#if EM_APPLOVIN
            string id = placement == AdPlacement.Default ?
               mAdSettings.DefaultInterstitialAdId.Id :
               FindIdForPlacement(mAdSettings.CustomInterstitialAdIds, placement);
            if (string.IsNullOrEmpty(id))
                return false;
            return MaxSdk.IsInterstitialReady(id);
#else
            return false;
#endif
        }

        protected override void InternalLoadInterstitialAd(AdPlacement placement)
        {
#if EM_APPLOVIN
            string id = placement == AdPlacement.Default ?
                mAdSettings.DefaultInterstitialAdId.Id :
                FindIdForPlacement(mAdSettings.CustomInterstitialAdIds, placement);

            if (string.IsNullOrEmpty(id))
                return;
            MaxSdk.LoadInterstitial(id);
#endif
        }

        protected override void InternalShowInterstitialAd(AdPlacement placement)
        {
#if EM_APPLOVIN
            string id = placement == AdPlacement.Default ?
                mAdSettings.DefaultInterstitialAdId.Id :
                FindIdForPlacement(mAdSettings.CustomInterstitialAdIds, placement);
            if (string.IsNullOrEmpty(id))
                return;
            MaxSdk.ShowInterstitial(id);
            currentShowingInterstitial = placement;
#endif
        }

        //------------------------------------------------------------
        // Rewarded Ads.
        //------------------------------------------------------------

        protected override bool InternalIsRewardedAdReady(AdPlacement placement)
        {
#if EM_APPLOVIN
            string id = placement == AdPlacement.Default ?
                mAdSettings.DefaultRewardedAdId.Id :
                FindIdForPlacement(mAdSettings.CustomRewardedAdIds, placement);
            if (string.IsNullOrEmpty(id))
                return false;
            return MaxSdk.IsRewardedAdReady(id);
#else
            return false;
#endif
        }

        protected override void InternalLoadRewardedAd(AdPlacement placement)
        {
#if EM_APPLOVIN
            string id = placement == AdPlacement.Default ?
                mAdSettings.DefaultRewardedAdId.Id :
                FindIdForPlacement(mAdSettings.CustomRewardedAdIds, placement);
            if (string.IsNullOrEmpty(id))
                return;
            MaxSdk.LoadRewardedAd(id);
#endif
        }

        protected override void InternalShowRewardedAd(AdPlacement placement)
        {
#if EM_APPLOVIN
            string id = placement == AdPlacement.Default ?
               mAdSettings.DefaultRewardedAdId.Id :
               FindIdForPlacement(mAdSettings.CustomRewardedAdIds, placement);
            if (string.IsNullOrEmpty(id))
                return;

            //Make sure to mark this rewarded ad placement as not completed
            if (!rewardedCompleted.ContainsKey(placement))
                rewardedCompleted.Add(placement, false);
            rewardedCompleted[placement] = false;

            MaxSdk.ShowRewardedAd(id);
            currentShowingRewarded = placement;
#endif
        }
        #endregion

        #region Other Methods

#if EM_APPLOVIN
        private MaxSdk.BannerPosition ToAppLovinAdPosition(BannerAdPosition pos)
        {
            switch (pos)
            {
                case BannerAdPosition.Top:
                    return MaxSdk.BannerPosition.TopCenter;
                case BannerAdPosition.Bottom:
                    return MaxSdk.BannerPosition.BottomCenter;
                case BannerAdPosition.TopLeft:
                    return MaxSdk.BannerPosition.TopLeft;
                case BannerAdPosition.TopRight:
                    return MaxSdk.BannerPosition.TopRight;
                case BannerAdPosition.BottomLeft:
                    return MaxSdk.BannerPosition.BottomLeft;
                case BannerAdPosition.BottomRight:
                    return MaxSdk.BannerPosition.BottomRight;
                default:
                    return MaxSdk.BannerPosition.Centered;
            }
        }
#endif

        #endregion

        #region  Ad Event Handlers
        private class RewardedAdEvents
        {
#if EM_APPLOVIN
            private readonly AppLovinClientImpl client;

            public RewardedAdEvents(AppLovinClientImpl client)
            {
                this.client = client;
                SubscribeToRewardedEvents();
            }
#endif

            private void SubscribeToRewardedEvents()
            {
#if EM_APPLOVIN
                MaxSdkCallbacks.OnRewardedAdLoadedEvent += MaxSDKRewardedAdLoadedEvent;
                MaxSdkCallbacks.OnRewardedAdLoadFailedEvent += MaxSDKRewardedAdLoadFailedEvent;
                MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent += MaxSDKRewardedAdFailedToDisplayEvent;
                MaxSdkCallbacks.OnRewardedAdDisplayedEvent += MaxSDKRewardedAdDisplayedEvent;
                MaxSdkCallbacks.OnRewardedAdClickedEvent += MaxSDKRewardedAdClickedEvent;
                MaxSdkCallbacks.OnRewardedAdHiddenEvent += MaxSDKRewardedAdDismissedEvent;
                MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent += MaxSDKRewardedAdReceivedRewardEvent;
#endif
            }
#if EM_APPLOVIN
            private void MaxSDKRewardedAdReceivedRewardEvent(string adUnitId, MaxSdkBase.Reward reward)
            {
                client.OnRewardedReceivedReward();
            }

            private void MaxSDKRewardedAdDismissedEvent(string adUnitId)
            {
                client.OnRewardedDismissed();
            }

            private void MaxSDKRewardedAdClickedEvent(string adUnitId)
            {
                client.OnRewardedClicked();
            }

            private void MaxSDKRewardedAdDisplayedEvent(string adUnitId)
            {
                client.OnRewardedDisplayedEvent();
            }

            private void MaxSDKRewardedAdFailedToDisplayEvent(string adUnitId, int errorCode)
            {
                client.OnRewardedFailedToDisplay();
            }

            private void MaxSDKRewardedAdLoadFailedEvent(string adUnitId, int errorCode)
            {
                client.OnRewardedLoadedEvent(false);
            }

            private void MaxSDKRewardedAdLoadedEvent(string adUnitId)
            {
                client.OnRewardedLoadedEvent(true);
            }
#endif
        }

        private class InterstitialEvents
        {
#if EM_APPLOVIN
            private readonly AppLovinClientImpl client;

            public InterstitialEvents(AppLovinClientImpl client)
            {
                this.client = client;
                SubscribeToInterstitialEvents();
            }
#endif

            private void SubscribeToInterstitialEvents()
            {
#if EM_APPLOVIN
                MaxSdkCallbacks.OnInterstitialLoadedEvent += MaxSDKInterstitialLoaded;
                MaxSdkCallbacks.OnInterstitialLoadFailedEvent += MaxSDKInterstitialAdLoadFailed;
                MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent += MaxSDKInterstitialFallToDisplay;
                MaxSdkCallbacks.OnInterstitialHiddenEvent += MaxSDKInterstitialHidden;
                MaxSdkCallbacks.OnInterstitialDisplayedEvent += MaxSDKInterstitialDisplayed;
                MaxSdkCallbacks.OnInterstitialClickedEvent += MaxSDKInterstitalClicked;
#endif
            }

#if EM_APPLOVIN
            private void MaxSDKInterstitalClicked(string adUnitId)
            {
                client.OnInterstitialAdClicked();
            }

            private void MaxSDKInterstitialHidden(string adUnitId)
            {
                client.OnInterstitialHiddenEvent();
            }

            private void MaxSDKInterstitialDisplayed(string adUnitId)
            {
                client.OnInterstitialDisplayedEvent();
            }

            private void MaxSDKInterstitialFallToDisplay(string adUnitId, int errorCode)
            {
                client.OnInterstitialFailedToDisplay();
            }

            private void MaxSDKInterstitialAdLoadFailed(string adUnitId, int errorCode)
            {
                client.OnInterstitialLoadedEvent(false);
            }

            private void MaxSDKInterstitialLoaded(string adUnitId)
            {
                client.OnInterstitialLoadedEvent(true);
            }
#endif
        }

        private class BannerEvents
        {
#if EM_APPLOVIN
            private readonly AppLovinClientImpl client;

            public BannerEvents(AppLovinClientImpl client)
            {
                this.client = client;
                SubscribeToBannerEvents();
            }
#endif

            private void SubscribeToBannerEvents()
            {
#if EM_APPLOVIN
                MaxSdkCallbacks.OnBannerAdLoadedEvent += MaxSDKBannerLoaded;
                MaxSdkCallbacks.OnBannerAdExpandedEvent += MaxSDKBannerExpanded;
                MaxSdkCallbacks.OnBannerAdLoadFailedEvent += MaxSDKBannerLoadFailed;
                MaxSdkCallbacks.OnBannerAdClickedEvent += MaxSDKBannerCollapsed;
                MaxSdkCallbacks.OnBannerAdClickedEvent += MaxSDKBannerClicked;
#endif
            }

#if EM_APPLOVIN
            private void MaxSDKBannerClicked(string obj)
            {
                client.OnBannerClicked();
            }

            private void MaxSDKBannerCollapsed(string obj)
            {
                client.OnBannerHidden();
            }

            private void MaxSDKBannerExpanded(string adUnitId)
            {
                client.OnBannerDisplayedEvent();
            }

            private void MaxSDKBannerLoadFailed(string adUnitId, int errorCode)
            {
                client.OnBannerLoadedEvent(false);
            }

            private void MaxSDKBannerLoaded(string adUnitId)
            {
                client.OnBannerLoadedEvent(true);
            }
#endif
        }

#if EM_APPLOVIN
        private void OnRewardedLoadedEvent(bool success)
        {
            if (RewardedLoaded != null)
                RewardedLoaded(success);
        }

        private void OnRewardedFailedToDisplay()
        {
            if (RewardedFailedToDisplay != null)
                RewardedFailedToDisplay();
        }

        private void OnRewardedDisplayedEvent()
        {
            if (RewardedDisplayed != null)
                RewardedDisplayed();
        }

        private void OnRewardedClicked()
        {
            if (RewardedClicked != null)
                RewardedClicked();
        }

        private void OnRewardedDismissed()
        {
            if (RewardedDismissed != null)
                RewardedDismissed();

            //Call complete if this placement is mark as completed
            //otherwise call skipped

            if (rewardedCompleted.ContainsKey(currentShowingRewarded))
            {
                if (rewardedCompleted[currentShowingRewarded])
                {
                    OnRewardedAdCompleted(currentShowingRewarded);
                    return;
                }
            }
            OnRewardedAdSkipped(currentShowingRewarded);
        }

        private void OnRewardedReceivedReward()
        {
            if (RewardedReceivedReward != null)
                RewardedReceivedReward();

            //Mark this rewarded ad is complete
            //This flag will be reset the next time another rewarded ad is called under this placement
            if (rewardedCompleted.ContainsKey(currentShowingRewarded))
                rewardedCompleted[currentShowingRewarded] = true;
        }

        private void OnInterstitialLoadedEvent(bool success)
        {
            if (InterstitialLoaded != null)
                InterstitialLoaded(success);
        }

        private void OnInterstitialFailedToDisplay()
        {
            if (InterstitialFailedToDisplay != null)
                InterstitialDisplayed();
        }

        private void OnInterstitialDisplayedEvent()
        {
            if (InterstitialDisplayed != null)
                InterstitialDisplayed();
        }

        private void OnInterstitialHiddenEvent()
        {
            if (InterstitialHidden != null)
                InterstitialHidden();

            //Invoke EM callback
            OnInterstitialAdCompleted(currentShowingInterstitial);
        }

        private void OnInterstitialAdClicked()
        {
            if (InterstitialClicked != null)
                InterstitialClicked();
        }

        private void OnBannerLoadedEvent(bool success)
        {
            if (BannerLoaded != null)
                BannerLoaded(success);
        }

        private void OnBannerDisplayedEvent()
        {
            if (BannerDisplayed != null)
                BannerDisplayed();
        }

        private void OnBannerHidden()
        {
            if (BannerHidden != null)
                BannerHidden();
        }

        private void OnBannerClicked()
        {
            if (BannerClicked != null)
                BannerClicked();
        }

#endif
        #endregion

        #region AppLovin Raw Events

#if EM_APPLOVIN
        public event Action<bool> RewardedLoaded;
        public event Action RewardedFailedToDisplay;
        public event Action RewardedDisplayed;
        public event Action RewardedClicked;
        public event Action RewardedDismissed;
        public event Action RewardedReceivedReward;
        public event Action<bool> InterstitialLoaded;
        public event Action InterstitialFailedToDisplay;
        public event Action InterstitialDisplayed;
        public event Action InterstitialHidden;
        public event Action InterstitialClicked;
        public event Action<bool> BannerLoaded;
        public event Action BannerDisplayed;
        public event Action BannerHidden;
        public event Action BannerClicked;
#endif

        #endregion
    }
}