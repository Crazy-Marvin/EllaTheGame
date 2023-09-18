using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace EasyMobile
{
    #if EM_CHARTBOOST
    using ChartboostSDK;
    #endif

    public class ChartboostClientImpl : AdClientImpl
    {
        private const string NO_SDK_MESSAGE = "SDK missing. Please import the Chartboost plugin.";
        private const string BANNER_UNSUPPORTED_MESSAGE = "Chartboost does not support banner ad format.";

        #if EM_CHARTBOOST
        private ChartboostSettings mAdSettings;
        private bool mIsCBRewardedAdCompleted;
        #endif

        #region Chartboost Events

        #if EM_CHARTBOOST
        
        /// <summary>
        /// Called after an interstitial has been loaded from the Chartboost API
        /// servers and cached locally. Implement to be notified of when an interstitial has
        ///	been loaded from the Chartboost API servers and cached locally for a given CBLocation.
        /// </summary>
        public event Action<CBLocation> DidCacheInterstitial
        {
            add { Chartboost.didCacheInterstitial += value; }
            remove { Chartboost.didCacheInterstitial -= value; }
        }

        /// <summary>
        /// Called after an interstitial has been clicked.
        /// Implement to be notified of when an interstitial has been click for a given CBLocation.
        /// "Clicked" is defined as clicking the creative interface for the interstitial.
        /// </summary>
        public event Action<CBLocation> DidClickInterstitial
        {
            add { Chartboost.didClickInterstitial += value; }
            remove { Chartboost.didClickInterstitial -= value; }
        }

        /// <summary>
        /// Called after an interstitial has been closed.
        /// Implement to be notified of when an interstitial has been closed for a given CBLocation.
        /// "Closed" is defined as clicking the close interface for the interstitial.
        /// </summary>
        public event Action<CBLocation> DidCloseInterstitial
        {
            add { Chartboost.didCloseInterstitial += value; }
            remove { Chartboost.didCloseInterstitial -= value; }
        }

        /// <summary>
        /// Called after an interstitial has been dismissed.
        /// Implement to be notified of when an interstitial has been dismissed for a given CBLocation.
        /// "Dismissal" is defined as any action that removed the interstitial UI such as a click or close.
        /// </summary>
        public event Action<CBLocation> DidDismissInterstitial
        {
            add { Chartboost.didDismissInterstitial += value; }
            remove { Chartboost.didDismissInterstitial -= value; }
        }

        /// <summary>
        /// Called after an interstitial has attempted to load from the Chartboost API
        /// servers but failed. Implement to be notified of when an interstitial has attempted
        ///	to load from the Chartboost API servers but failed for a given CBLocation.
        /// </summary>
        public event Action<CBLocation, CBImpressionError> DidFailToLoadInterstitial
        {
            add { Chartboost.didFailToLoadInterstitial += value; }
            remove { Chartboost.didFailToLoadInterstitial -= value; }
        }

        /// <summary>
        /// Called after a rewarded video has been loaded from the Chartboost API
        /// servers and cached locally. Implement to be notified of when a rewarded video has
        ///	been loaded from the Chartboost API servers and cached locally for a given CBLocation.
        /// </summary>
        public event Action<CBLocation> DidCacheRewardedVideo
        {
            add { Chartboost.didCacheRewardedVideo += value; }
            remove { Chartboost.didCacheRewardedVideo -= value; }
        }

        /// <summary>
        /// Called after a rewarded video has been clicked.
        /// Implement to be notified of when a rewarded video has been click for a given CBLocation.
        /// "Clicked" is defined as clicking the creative interface for the rewarded video.
        /// </summary>
        public event Action<CBLocation> DidClickRewardedVideo
        {
            add { Chartboost.didClickRewardedVideo += value; }
            remove { Chartboost.didClickRewardedVideo -= value; }
        }

        /// <summary>
        /// Called after a rewarded video has been closed.
        /// Implement to be notified of when a rewarded video has been closed for a given CBLocation.
        /// "Closed" is defined as clicking the close interface for the rewarded video.
        /// </summary>
        public event Action<CBLocation> DidCloseRewardedVideo
        {
            add { Chartboost.didCloseRewardedVideo += value; }
            remove { Chartboost.didCloseRewardedVideo -= value; }
        }

        // <summary>
        /// Called after a rewarded video has been dismissed.
        /// Implement to be notified of when a rewarded video has been dismissed for a given CBLocation.
        /// "Dismissal" is defined as any action that removed the rewarded video UI such as a click or close.
        /// </summary>
        public event Action<CBLocation> DidDismissRewardedVideo
        {
            add { Chartboost.didDismissRewardedVideo += value; }
            remove { Chartboost.didDismissRewardedVideo -= value; }
        }

        /// <summary>
        /// Called after a rewarded video has attempted to load from the Chartboost API
        /// servers but failed. Implement to be notified of when a rewarded video has attempted
        /// to load from the Chartboost API servers but failed for a given CBLocation.
        /// </summary>
        public event Action<CBLocation, CBImpressionError> DidFailToLoadRewardedVideo
        {
            add { Chartboost.didFailToLoadRewardedVideo += value; }
            remove { Chartboost.didFailToLoadRewardedVideo -= value; }
        }

        /// <summary>
        ///  Called after a rewarded video has been viewed completely and user is eligible for reward.
        ///  Implement to be notified of when a rewarded video has been viewed completely and user is eligible for reward.
        /// </summary>
        public event Action<CBLocation, int> DidCompleteRewardedVideo
        {
            add { Chartboost.didCompleteRewardedVideo += value; }
            remove { Chartboost.didCompleteRewardedVideo -= value; }
        }

        /// <summary>
        /// Called after a click is registered, but the user is not forwarded to the IOS App Store.
        /// Implement to be notified of when a click is registered, but the user is not fowrwarded
        /// to the IOS App Store for a given CBLocation.
        /// </summary>
        public event Action<CBLocation, CBClickError> DidFailToRecordClick
        {
            add { Chartboost.didFailToRecordClick += value; }
            remove { Chartboost.didFailToRecordClick -= value; }
        }

        /// <summary>
        /// Called after an the SDK has been initialized
        /// </summary>
        public event Action<bool> DidInitialize
        {
            add { Chartboost.didInitialize += value; }
            remove { Chartboost.didInitialize -= value; }
        }

        /// <summary>
        /// Called if Chartboost SDK pauses click actions awaiting confirmation from the user.
        /// Use this method to display any gating you would like to prompt the user for input.
        /// Once confirmed call didPassAgeGate:(BOOL)pass to continue execution.
        /// </summary>
        public event Action DidPauseClickForConfirmation
        {
            add { Chartboost.didPauseClickForConfirmation += value; }
            remove { Chartboost.didPauseClickForConfirmation -= value; }
        }

        /// <summary>
        /// Called before an interstitial will be displayed on the screen.
        /// Implement to control if the Charboost SDK should display an interstitial
        /// for the given CBLocation.  This is evaluated if the showInterstitial:(CBLocation)
        ///	is called.  If true is returned the operation will proceed, if false, then the
        ///	operation is treated as a no-op and nothing is displayed. Default return is true.
        /// </summary>
        public event Func<CBLocation, bool> ShouldDisplayInterstitial
        {
            add { Chartboost.shouldDisplayInterstitial += value; }
            remove { Chartboost.shouldDisplayInterstitial -= value; }
        }

        //// <summary>
        /// Called before a rewarded video will be displayed on the screen.
        /// Implement to control if the Charboost SDK should display a rewarded video
        /// for the given CBLocation.  This is evaluated if the showRewardedVideo:(CBLocation)
        ///	is called.  If true is returned the operation will proceed, if false, then the
        ///	operation is treated as a no-op and nothing is displayed. Default return is true.
        /// </summary>
        public event Func<CBLocation, bool> ShouldDisplayRewardedVideo
        {
            add { Chartboost.shouldDisplayRewardedVideo += value; }
            remove { Chartboost.shouldDisplayRewardedVideo -= value; }
        }

        /// <summary>
        /// Called just before a video will be displayed.
        /// Implement to be notified of when a video will be displayed for a given CBLocation.
        /// </summary>
        public event Action<CBLocation> WillDisplayVideo
        {
            add { Chartboost.willDisplayVideo += value; }
            remove { Chartboost.willDisplayVideo -= value; }
        }

        #endif

        #endregion  // Chartboost Events

        #region Singleton

        private static ChartboostClientImpl sInstance;

        private ChartboostClientImpl()
        {
        }

        /// <summary>
        /// Returns the singleton client.
        /// </summary>
        /// <returns>The client.</returns>
        public static ChartboostClientImpl CreateClient()
        {
            if (sInstance == null)
            {
                sInstance = new ChartboostClientImpl();
            }
            return sInstance;
        }

        #endregion  // Object Creators

        #region AdClient Overrides

        public override AdNetwork Network { get { return AdNetwork.Chartboost; } }

        public override bool IsBannerAdSupported { get { return false; } }

        public override bool IsInterstitialAdSupported { get { return true; } }

        public override bool IsRewardedAdSupported { get { return true; } }

        public override bool IsSdkAvail
        {
            get
            {
                #if EM_CHARTBOOST
                return true;
                #else
                return false;
                #endif
            }
        }

        public override bool IsValidPlacement(AdPlacement placement, AdType type)
        {
#if EM_CHARTBOOST
            return true;
#else
            return false;
#endif
        }

        protected override Dictionary<AdPlacement, AdId> CustomInterstitialAdsDict
        {
            get
            {
                #if EM_CHARTBOOST
                return mAdSettings == null ? null : mAdSettings.CustomInterstitialPlacements.ToDictionary(key => key, _ => new AdId("", ""));
                #else
                return null;
                #endif
            }
        }

        protected override Dictionary<AdPlacement, AdId> CustomRewardedAdsDict
        {
            get
            {
                #if EM_CHARTBOOST
                return mAdSettings == null ? null : mAdSettings.CustomRewardedPlacements.ToDictionary(key => key, _ => new AdId("", ""));
                #else
                return null;
                #endif
            }
        }

        protected override string NoSdkMessage { get { return NO_SDK_MESSAGE; } }

        protected override void InternalInit()
        {
            #if EM_CHARTBOOST

            mAdSettings = EM_Settings.Advertising.Chartboost;

            Chartboost.didCacheInterstitial += CBDidCacheInterstitial;
            Chartboost.didClickInterstitial += CBDidClickInterstitial;
            Chartboost.didCloseInterstitial += CBDidCloseInterstitial;
            Chartboost.didDismissInterstitial += CBDidDismissInterstitial;
            Chartboost.didFailToLoadInterstitial += CBDidFailToLoadInterstitial;

            Chartboost.didCacheRewardedVideo += CBDidCacheRewardedVideo;
            Chartboost.didClickRewardedVideo += CBDidClickRewardedVideo;
            Chartboost.didCloseRewardedVideo += CBDidCloseRewardedVideo;
            Chartboost.didDismissRewardedVideo += CBDidDismissRewardedVideo;
            Chartboost.didFailToLoadRewardedVideo += CBDidFailToLoadRewardedVideo;
            Chartboost.didCompleteRewardedVideo += CBDidCompleteRewardedVideo;

            // Create Chartboost object.
            // We'll handle ad loading, so turning off Chartboost's autocache feature.
            Chartboost.Create();
            Chartboost.setAutoCacheAds(false);

            // Set GDPR consent (if any) *after* the SDK is started,
            // as recommended by the "restrictDataCollection* method.
            var consent = GetApplicableDataPrivacyConsent();
            ApplyDataPrivacyConsent(consent);

            // Done initialization.
            mIsInitialized = true;
            Debug.Log("Chartboost client has been initialized.");

            #endif
        }

        //------------------------------------------------------------
        // Banner Ads.
        //------------------------------------------------------------

        protected override void InternalShowBannerAd(AdPlacement _, BannerAdPosition __, BannerAdSize ___)
        {
            Debug.LogWarning(BANNER_UNSUPPORTED_MESSAGE);
        }

        protected override void InternalHideBannerAd(AdPlacement _)
        {
            Debug.LogWarning(BANNER_UNSUPPORTED_MESSAGE);
        }

        protected override void InternalDestroyBannerAd(AdPlacement _)
        {
            Debug.LogWarning(BANNER_UNSUPPORTED_MESSAGE);
        }

        //------------------------------------------------------------
        // Interstitial Ads.
        //------------------------------------------------------------

        protected override void InternalLoadInterstitialAd(AdPlacement placement)
        {
            #if EM_CHARTBOOST
            Chartboost.cacheInterstitial(ToCBLocation(placement));
            #endif
        }

        protected override bool InternalIsInterstitialAdReady(AdPlacement placement)
        {
            #if EM_CHARTBOOST
            return Chartboost.hasInterstitial(ToCBLocation(placement));
            #else
            return false;
            #endif
        }

        protected override void InternalShowInterstitialAd(AdPlacement placement)
        {   
            #if EM_CHARTBOOST
            Chartboost.showInterstitial(ToCBLocation(placement));
            #endif
        }

        //------------------------------------------------------------
        // Rewarded Ads.
        //------------------------------------------------------------

        protected override void InternalLoadRewardedAd(AdPlacement placement)
        {
            #if EM_CHARTBOOST
            Chartboost.cacheRewardedVideo(ToCBLocation(placement));
            #endif
        }

        protected override bool InternalIsRewardedAdReady(AdPlacement placement)
        {
            #if EM_CHARTBOOST
            return Chartboost.hasRewardedVideo(ToCBLocation(placement));
            #else
            return false;
            #endif
        }

        protected override void InternalShowRewardedAd(AdPlacement placement)
        {
            #if EM_CHARTBOOST
            Chartboost.showRewardedVideo(ToCBLocation(placement));
            #endif
        }

        #endregion  // AdClient Overrides

        #region IConsentRequirable Overrides

        private const string DATA_PRIVACY_CONSENT_KEY = "EM_Ads_Chartboost_DataPrivacyConsent";

        protected override string DataPrivacyConsentSaveKey { get { return DATA_PRIVACY_CONSENT_KEY; } }

        protected override void ApplyDataPrivacyConsent(ConsentStatus consent)
        {
            #if EM_CHARTBOOST
            // If the consent is left as Unknown (maybe due to non-EEA region) 
            // we won't do anything (pre-GDPR behaviour).
            switch (consent)
            {
                case ConsentStatus.Granted:
                    Chartboost.setPIDataUseConsent(CBPIDataUseConsent.YesBehavioral);
                    break;
                case ConsentStatus.Revoked:
                    Chartboost.setPIDataUseConsent(CBPIDataUseConsent.NoBehavioral);
                    break;
                case ConsentStatus.Unknown:
                    Chartboost.setPIDataUseConsent(CBPIDataUseConsent.Unknown);
                    break;
                default:
                    break;
            }
            #else
            Debug.Log(NO_SDK_MESSAGE);
            #endif
        }

        #endregion

        #region Private Stuff

        #if EM_CHARTBOOST
        private CBLocation ToCBLocation(AdPlacement placement)
        {
            return placement == null || placement == AdPlacement.Default ? CBLocation.Default : CBLocation.locationFromName(placement.Name);
        }
        #endif

        #endregion  // Private Stuff

        #region Ad Event Handlers

        #if EM_CHARTBOOST
        
        //------------------------------------------------------------
        // Interstitial Ad Events.
        //------------------------------------------------------------

        void CBDidCacheInterstitial(CBLocation location)
        {
            Debug.Log("Chartboost interstitial ad has been loaded successfully.");
        }

        void CBDidClickInterstitial(CBLocation location)
        {           
        }

        void CBDidCloseInterstitial(CBLocation location)
        {          
        }

        void CBDidDismissInterstitial(CBLocation location)
        {
            OnInterstitialAdCompleted(AdPlacement.PlacementWithName(location.ToString()));
        }

        void CBDidFailToLoadInterstitial(CBLocation location, CBImpressionError error)
        {
            Debug.Log("Chartboost interstitial ad failed to load.");
        }

        //------------------------------------------------------------
        // Rewarded Ad Events.
        //------------------------------------------------------------

        void CBDidCacheRewardedVideo(CBLocation location)
        {
            Debug.Log("Chartboost rewarded video ad has been loaded successfully.");
        }

        void CBDidFailToLoadRewardedVideo(CBLocation location, CBImpressionError error)
        {
            Debug.Log("Chartboost rewarded video ad failed to load.");
        }

        void CBDidCompleteRewardedVideo(CBLocation location, int reward)
        {
            mIsCBRewardedAdCompleted = true;
        }

        void CBDidClickRewardedVideo(CBLocation location)
        {
        }

        void CBDidCloseRewardedVideo(CBLocation location)
        {
        }

        void CBDidDismissRewardedVideo(CBLocation location)
        {
            if (mIsCBRewardedAdCompleted)
            {
                mIsCBRewardedAdCompleted = false;
                OnRewardedAdCompleted(AdPlacement.PlacementWithName(location.ToString()));
            }
            else
            {
                OnRewardedAdSkipped(AdPlacement.PlacementWithName(location.ToString()));
            }
        }
        #endif

        #endregion  // Ad Event Handlers
    }
}