using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace EasyMobile
{
#if EM_FAIRBID
    using Fyber;
#endif

    public class FairBidClientImpl : AdClientImpl
    {
        private const string NO_SDK_MESSAGE = "SDK missing. Please import the FairBid plugin.";

#if EM_FAIRBID
        private const string FAIR_BID_DEFAULT_TAG = "default";

        private FairBidSettings mGlobalAdSettings = null;

        /// <summary>
        /// Banner ad event listener.
        /// </summary>
        public MyBannerListener BannerAdCallbackListener;

        /// <summary>
        /// Interstitial ad event listener.
        /// </summary>
        public MyInterstitialListener InterstitialAdCallbackListener;

        /// <summary>
        /// Rewarded ad event listener.
        /// </summary>
        public MyRewardedListener RewardedAdCallbackListener;
#endif

        #region FairBid Events

#if EM_FAIRBID

        /// <summary>
        /// Occurs when a banner ad is loaded or failed to be loaded.
        /// </summary>
        public event Action<string, bool> BannerLoadedEvent;

        /// <summary>
        /// Occurs when a banner ad is displayed.
        /// </summary>
        public event Action<string, ImpressionData> BannerDisplayedEvent;

        /// <summary>
        /// Occurs when an interstitial ad is loaded or failed to be loaded.
        /// </summary>
        public event Action<string, bool> InterstitialLoadedEvent;

        /// <summary>
        /// Occurs when an interstitial ad is displayed.
        /// </summary>
        public event Action<string, bool> InterstitialDisplayedEvent;

        /// <summary>
        /// Occurs when an interstitial ad is closed.
        /// </summary>
        public event Action<string> InterstitialHiddenEvent;

        /// <summary>
        /// Occurs when a rewarded ad is loaded or failed to be loaded.
        /// </summary>
        public event Action<string, bool> RewardedLoadedEvent;

        /// <summary>
        /// Occurs when a rewarded ad is displayed.
        /// </summary>
        public event Action<string> RewardedDisplayedEvent;

        /// <summary>
        /// Occurs when a rewarded a is closed.
        /// </summary>
        public event Action<string> RewardedHiddenEvent;

        /// <summary>
        /// Occurs when an ad is clicked. This means the user will be leaving your app soon.
        /// </summary>
        public event Action<string> AdClickedEvent;
#endif

        #endregion  // FairBid Events

        #region Singleton

        private static FairBidClientImpl sInstance;

        private FairBidClientImpl()
        {
        }

        /// <summary>
        /// Returns the singleton client.
        /// </summary>
        /// <returns>The client.</returns>
        public static FairBidClientImpl CreateClient()
        {
            if (sInstance == null)
            {
                sInstance = new FairBidClientImpl();
            }
            return sInstance;
        }

        #endregion  // Object Creators

        #region AdClient Overrides

        public override AdNetwork Network { get { return AdNetwork.FairBid; } }

        public override bool IsBannerAdSupported { get { return true; } }

        public override bool IsInterstitialAdSupported { get { return true; } }

        public override bool IsRewardedAdSupported { get { return true; } }

        public override bool IsSdkAvail
        {
            get
            {
#if EM_FAIRBID
                return true;
#else
                return false;
#endif
            }
        }

        public override bool IsValidPlacement(AdPlacement placement, AdType type)
        {
#if EM_FAIRBID         
            return true;
#else
            return false;
#endif
        }

        protected override Dictionary<AdPlacement, AdId> CustomInterstitialAdsDict
        {
            get
            {
#if EM_FAIRBID
                return mGlobalAdSettings == null ? null : mGlobalAdSettings.CustomInterstitialPlacements.ToDictionary(key => key, _ => new AdId("", ""));
#else
                return null;
#endif
            }
        }

        protected override Dictionary<AdPlacement, AdId> CustomRewardedAdsDict
        {
            get
            {
#if EM_FAIRBID
                return mGlobalAdSettings == null ? null : mGlobalAdSettings.CustomRewardedPlacements.ToDictionary(key => key, _ => new AdId("", ""));
#else
                return null;
#endif
            }
        }

        protected override string NoSdkMessage { get { return NO_SDK_MESSAGE; } }

        protected override void InternalInit()
        {
#if EM_FAIRBID
            // Store a reference to the global settings.
            mGlobalAdSettings = EM_Settings.Advertising.FairBid;

            // Set GPDR consent (if any) *before* starting the SDK
            // https://dev-unity.fyber.com/docs/gdpr
            var consent = GetApplicableDataPrivacyConsent();
            ApplyDataPrivacyConsent(consent);

            // Start FairBid with no automatic fetching since we'll handle ad loading.
            FairBid.ConfigureForAppId(mGlobalAdSettings.PublisherId)
                   .DisableAutoRequesting()
                   .Start();

            // Add callback handlers
            BannerAdCallbackListener = new MyBannerListener();
            BannerAdCallbackListener.setAdClient(this);
            InterstitialAdCallbackListener = new MyInterstitialListener();
            InterstitialAdCallbackListener.setAdClient(this);
            RewardedAdCallbackListener = new MyRewardedListener();
            RewardedAdCallbackListener.setAdClient(this);
            Banner.SetBannerListener(BannerAdCallbackListener);
            Interstitial.SetInterstitialListener(InterstitialAdCallbackListener);
            Rewarded.SetRewardedListener(RewardedAdCallbackListener);

            mIsInitialized = true;
            Debug.Log("FairBid client has been initialized.");
#else
            Debug.LogError(NO_SDK_MESSAGE);
#endif
        }

        //------------------------------------------------------------
        // Show Test Suite (not IAdClient method)
        //------------------------------------------------------------

        public void ShowTestSuite()
        {
#if EM_FAIRBID
            FairBid.ShowTestSuite();
#else
            Debug.LogError(NO_SDK_MESSAGE);
#endif
        }

        //------------------------------------------------------------
        // Banner Ads.
        //------------------------------------------------------------

        protected override void InternalShowBannerAd(AdPlacement placement, BannerAdPosition position, BannerAdSize __)
        {
#if EM_FAIRBID
            string id = placement == AdPlacement.Default ?
                mGlobalAdSettings.DefaultBannerId :
                ToFairBidAdTag(placement);

            BannerOptions showOptions = new BannerOptions();
            switch (position)
            {
                case BannerAdPosition.TopLeft:
                case BannerAdPosition.TopRight:
                case BannerAdPosition.Top:
                    showOptions.DisplayAtTheTop();
                    break;
                case BannerAdPosition.BottomLeft:
                case BannerAdPosition.BottomRight:
                case BannerAdPosition.Bottom:
                    showOptions.DisplayAtTheBottom();
                    break;
                default:
                    showOptions.DisplayAtTheBottom();
                    break;
            }
            Banner.Show(id, showOptions);
#endif
        }

        protected override void InternalHideBannerAd(AdPlacement placement)
        {
#if EM_FAIRBID
            string id = placement == AdPlacement.Default ?
              mGlobalAdSettings.DefaultBannerId :
              ToFairBidAdTag(placement);
            Banner.Destroy(id);
#endif
        }

        protected override void InternalDestroyBannerAd(AdPlacement placement)
        {
#if EM_FAIRBID
            string id = placement == AdPlacement.Default ?
               mGlobalAdSettings.DefaultBannerId :
               ToFairBidAdTag(placement);
            Banner.Destroy(id);
#endif
        }

        //------------------------------------------------------------
        // Interstitial Ads.
        //------------------------------------------------------------

        protected override void InternalLoadInterstitialAd(AdPlacement placement)
        {
#if EM_FAIRBID
            string id = placement == AdPlacement.Default ?
               mGlobalAdSettings.DefaultInterstitialAdId :
               ToFairBidAdTag(placement);
            Interstitial.Request(id);

#endif
        }

        protected override bool InternalIsInterstitialAdReady(AdPlacement placement)
        {
#if EM_FAIRBID
            string id = placement == AdPlacement.Default ?
               mGlobalAdSettings.DefaultInterstitialAdId :
               ToFairBidAdTag(placement);
            return Interstitial.IsAvailable(id);
#else
            return false;
#endif
        }

        protected override void InternalShowInterstitialAd(AdPlacement placement)
        {
#if EM_FAIRBID
            string id = placement == AdPlacement.Default ?
              mGlobalAdSettings.DefaultInterstitialAdId :
              ToFairBidAdTag(placement);
            Interstitial.Show(id);
#endif
        }

        //------------------------------------------------------------
        // Rewarded Ads.
        //------------------------------------------------------------

        protected override void InternalLoadRewardedAd(AdPlacement placement)
        {
#if EM_FAIRBID
            string id = placement == AdPlacement.Default ?
              mGlobalAdSettings.DefaultRewardedAdId :
              ToFairBidAdTag(placement);
            Rewarded.Request(id);
#endif
        }

        protected override bool InternalIsRewardedAdReady(AdPlacement placement)
        {
#if EM_FAIRBID
            string id = placement == AdPlacement.Default ?
              mGlobalAdSettings.DefaultRewardedAdId :
              ToFairBidAdTag(placement);
            return Rewarded.IsAvailable(id);
#else
            return false;
#endif
        }

        protected override void InternalShowRewardedAd(AdPlacement placement)
        {
#if EM_FAIRBID
            string id = placement == AdPlacement.Default ?
              mGlobalAdSettings.DefaultRewardedAdId :
              ToFairBidAdTag(placement);
            Rewarded.Show(id);

#endif
        }

        #endregion  // AdClient Overrides

        #region IConsentRequirable Overrides

        private const string DATA_PRIVACY_CONSENT_KEY = "EM_Ads_FairBid_DataPrivacyConsent";

        protected override string DataPrivacyConsentSaveKey { get { return DATA_PRIVACY_CONSENT_KEY; } }

        protected override void ApplyDataPrivacyConsent(ConsentStatus consent)
        {
#if EM_FAIRBID
            // Only set the GDPR consent if an explicit one is specified.
            switch (consent)
            {
                case ConsentStatus.Granted:
                    UserInfo.SetGdprConsent(true);
                    break;
                case ConsentStatus.Revoked:
                    UserInfo.SetGdprConsent(false);
                    break;
                case ConsentStatus.Unknown:
                default:
                    break;
            }
#endif
        }

        #endregion

        #region Helpers

#if EM_FAIRBID

        private string ToFairBidAdPosition(BannerAdPosition pos)
        {
            switch (pos)
            {
                case BannerAdPosition.TopLeft:
                case BannerAdPosition.TopRight:
                case BannerAdPosition.Top:
                    return "top";
                case BannerAdPosition.BottomLeft:
                case BannerAdPosition.BottomRight:
                case BannerAdPosition.Bottom:
                    return "bottom";
                default:
                    return "bottom";
            }
        }

        private string ToFairBidAdTag(AdPlacement placement)
        {
            return AdPlacement.GetPrintableName(placement);
        }

        private AdPlacement ToAdPlacement(string fairBidAdTag)
        {
            return (string.IsNullOrEmpty(fairBidAdTag) || fairBidAdTag.Equals(FAIR_BID_DEFAULT_TAG))
                    ? AdPlacement.Default
                    : AdPlacement.PlacementWithName(fairBidAdTag);
        }

#endif

        #endregion

        #region Ad Event Handlers

#if EM_FAIRBID

        public class MyBannerListener : BannerListener
        {
            private FairBidClientImpl adClient;

            public void setAdClient(FairBidClientImpl inputClient)
            {
                this.adClient = inputClient;
            }
            public void OnError(string placementId, string error)
            {
                adClient.OnBannerErrorEvent(placementId, error);
            }

            public void OnLoad(string placementId)
            {
                adClient.OnBannerLoadedEvent(placementId, true);
            }

            public void OnShow(string placementId, ImpressionData impressionData)
            {
                adClient.OnBannerDisplayedEvent(placementId, impressionData);
            }
            public void OnClick(string placementId)
            {
                adClient.OnAdClicked(placementId, AdType.Banner);
            }

            public void OnRequestStart(string placementId)
            {
            }
        }


        public class MyInterstitialListener : InterstitialListener
        {
            private FairBidClientImpl adClient;

            public void setAdClient(FairBidClientImpl inputClient)
            {
                this.adClient = inputClient;
            }
            public void OnShow(string placementId, ImpressionData impressionData)
            {
                // Called when an Interstitial from placement 'placementId' shows up. In case the ad is a video, audio play will start here.
                // On Android, this callback might be called only once the ad is closed.
                adClient.OnInterstitialDisplayedEvent(placementId, true);
            }

            public void OnClick(string placementId)
            {
                adClient.OnAdClicked(placementId, AdType.Interstitial);
            }

            public void OnHide(string placementId)
            {
                // Called when an Interstitial from placement 'placementId' hides.
                adClient.OnInterstitialHiddenEvent(placementId);
            }

            public void OnShowFailure(string placementId, ImpressionData impressionData)
            {
                adClient.OnInterstitialDisplayedEvent(placementId, false);
            }

            public void OnAvailable(string placementId)
            {
                adClient.OnInterstitialLoadedEvent(placementId, true);
            }

            public void OnUnavailable(string placementId)
            {
                // Called when an Interstitial from placement 'placementId' becomes unavailable
                adClient.OnInterstitialLoadedEvent(placementId, false);
            }

            public void OnRequestStart(string placementId)
            {
                // Called when an Interstitial from placement 'placementId' is going to be requested
            }
        }

        public class MyRewardedListener : RewardedListener
        {
            private FairBidClientImpl adClient;

            public void setAdClient(FairBidClientImpl inputClient)
            {
                this.adClient = inputClient;
            }
            public void OnShow(string placementId, ImpressionData impressionData)
            {
                // Called when a rewarded ad from placementId shows up. In case the ad is a video, audio play will start here.
                // On Android, this callback might be called only once the ad is closed.
                adClient.OnRewardedDisplayedEvent(placementId, true);

            }

            public void OnClick(string placementId)
            {
                adClient.OnAdClicked(placementId, AdType.Rewarded);
            }

            public void OnHide(string placementId)
            {
                adClient.OnRewardedHiddenEvent(placementId);
            }

            public void OnShowFailure(string placementId, ImpressionData impressionData)
            {
                // Called when an error arises when showing a rewarded ad from placement 'placementId'
                adClient.OnRewardedDisplayedEvent(placementId, false);
            }

            public void OnAvailable(string placementId)
            {
                adClient.OnRewardedLoadedEvent(placementId, true);
            }

            public void OnUnavailable(string placementId)
            {
                // Called when a rewarded ad from placement 'placementId' becomes unavailable
                adClient.OnRewardedLoadedEvent(placementId, false);
            }

            public void OnCompletion(string placementId, bool userRewarded)
            {
                // Called when a rewarded ad from placement 'placementId' finishes playing. In case the ad is a video, audio play will stop here.
                adClient.OnRewardedCompletedEvent(placementId, userRewarded);
            }

            public void OnRequestStart(string placementId)
            {
            }
        }

        private void OnBannerLoadedEvent(string placementId, bool success)
        {
            if (success)
            {
                Debug.Log("FairBid: Banner ad loaded at placement: " + placementId);
            }
            else
            {
                Debug.Log("FairBid: Banner ad is unavailable at placement: " + placementId);
            }

            if (BannerLoadedEvent != null)
                BannerLoadedEvent(placementId, success);
        }

        private void OnBannerErrorEvent(string placementId, string error)
        {

            Debug.Log("FairBid: Banner ad at placement: " + placementId + " is error: " + error);

            if (BannerLoadedEvent != null)
                BannerLoadedEvent(placementId, false);
        }

        private void OnBannerDisplayedEvent(string placement, ImpressionData impressionData)
        {
            if (BannerDisplayedEvent != null)
                BannerDisplayedEvent(placement, impressionData);
        }

        private void OnInterstitialLoadedEvent(string placement, bool success)
        {
            if (success)
            {
                Debug.Log("FairBid: Interstitial ad loaded at placement: " + placement);
            }
            else
            {
                Debug.Log("FairBid: Interstitial ad is unavailable at placement: " + placement);
            }

            if (InterstitialLoadedEvent != null)
                InterstitialLoadedEvent(placement, success);
        }


        private void OnInterstitialHiddenEvent(string placementId)
        {

            OnInterstitialAdCompleted(ToAdPlacement(placementId));
            if (InterstitialHiddenEvent != null)
                InterstitialHiddenEvent(placementId);
        }

        private void OnInterstitialDisplayedEvent(string placementId, bool isSuccess)
        {
            if (isSuccess)
            {
                if (InterstitialDisplayedEvent != null)
                    InterstitialDisplayedEvent(placementId, isSuccess);
            }
            else
            {
                if (InterstitialDisplayedEvent != null)
                    InterstitialDisplayedEvent(placementId, isSuccess);
            }
        }

        private void OnRewardedLoadedEvent(string placement, bool success)
        {
            if (success)
            {
                Debug.Log("FairBid: Rewarded ad loaded at placement: " + placement);
            }
            else
            {
                Debug.Log("FairBid: Rewarded ad is unavailable at placement: " + placement);
            }

            if (RewardedLoadedEvent != null)
                RewardedLoadedEvent(placement, success);
        }

        private void OnRewardedHiddenEvent(string placement)
        {
            if (RewardedHiddenEvent != null)
                RewardedHiddenEvent(placement);
        }

        private void OnRewardedDisplayedEvent(string placement, bool isSucess)
        {
            if (isSucess)
            {
                if (RewardedDisplayedEvent != null)
                    RewardedDisplayedEvent(placement);
            }
            else
            {
                Debug.Log("FairBid: Rewarded ad failed to show at placement: " + placement);
            }
        }

        private void OnRewardedCompletedEvent(string placement, bool userRewarded)
        {
            if (userRewarded)
            {
                OnRewardedAdCompleted(ToAdPlacement(placement));
            }
            else
            {
                OnRewardedAdSkipped(ToAdPlacement(placement));
            }
        }

        private void OnAdClicked(string placement, AdType adType)
        {
            if (adType == AdType.Banner)
            {
                Debug.Log("FairBid banner ad at placement: " + placement + " has been clicked.");
            }
            else if (adType == AdType.Interstitial)
            {
                Debug.Log("FairBid interstitial ad at placement: " + placement + " has been clicked.");
            }
            else if (adType == AdType.Rewarded)
            {
                Debug.Log("FairBid rewarded ad at placement: " + placement + " has been clicked.");
            }
            if (AdClickedEvent != null)
                AdClickedEvent(placement);
        }

#endif

        #endregion  // Ad Event Handlers
    }
}