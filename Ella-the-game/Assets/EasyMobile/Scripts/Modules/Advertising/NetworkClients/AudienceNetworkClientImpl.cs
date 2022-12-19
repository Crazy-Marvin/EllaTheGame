using System;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile;

namespace EasyMobile
{
    using System.Text;
#if EM_FBAN
    using AudienceNetwork;
#endif

    public class AudienceNetworkClientImpl : AdClientImpl
    {
        #region Inner classes

#if EM_FBAN

        /// <summary>
        /// Hold data of a banner ad.
        /// </summary>
        protected class BannerAd
        {
            private bool isLoaded;

            public AdView Ad { get; set; }

            public AdSize CurrentSize { get; private set; }

            public bool IsLoaded
            {
                get { return Ad != null && isLoaded; }
                set { isLoaded = value; }
            }

            public BannerAd(AdView adView, AdSize adSize)
            {
                Ad = adView;
                CurrentSize = adSize;

                Ad.AdViewDidLoad += () => isLoaded = true;
            }
        }

        /// <summary>
        /// Hold data of an interstitial ad.
        /// </summary>
        protected class Interstitial
        {
            private bool isReady;

            public InterstitialAd Ad { get; set; }

            public bool IsLoading { get; set; }

            public bool IsReady
            {
                get { return Ad != null && isReady; }
                set { isReady = value; }
            }

            public Interstitial(InterstitialAd interstitialAd)
            {
                Ad = interstitialAd;

                /// It seems like the IsValid() method in Intersitital ads can't be used to check if an ad is loaded or not (???).
                /// So we have to detect the DidLoad events (invoked when those ads are loaded) and raise a bool manually like this.
                Ad.InterstitialAdDidLoad += () =>
                {
                    IsLoading = false;
                    isReady = true;
                };
            }
        }

        /// <summary>
        /// Hold data of a rewarded video ad.
        /// </summary>
        protected class RewardedVideo
        {
            private bool isReady;

            public RewardedVideoAd Ad { get; set; }

            public bool IsLoading { get; set; }

            public bool IsReady
            {
                get { return Ad != null && isReady; }
                set { isReady = value; }
            }

            public RewardedVideo(RewardedVideoAd rewardedVideo)
            {
                Ad = rewardedVideo;

                /// Detect when the ad is loaded.
                Ad.RewardedVideoAdDidLoad += () =>
                {
                    IsLoading = false;
                    isReady = true;
                };
            }
        }

#endif

        #endregion  // Inner Classes

        #region FB Audience Events

#if EM_FBAN

        /// <summary>
        /// Occurs when ad view did load.
        /// </summary>
        public event FBAdViewBridgeCallback AdViewDidLoad;

        /// <summary>
        /// Occurs when ad view will log impression.
        /// </summary>
        public event FBAdViewBridgeCallback AdViewWillLogImpression;

        /// <summary>
        /// Occurs when ad view did fail with error.
        /// </summary>
        public event FBAdViewBridgeErrorCallback AdViewDidFailWithError;

        /// <summary>
        /// Occurs when ad view did click.
        /// </summary>
        public event FBAdViewBridgeCallback AdViewDidClick;

        /// <summary>
        /// Occurs when ad view did finish click.
        /// </summary>
        public event FBAdViewBridgeCallback AdViewDidFinishClick;

        /// <summary>
        /// Occurs when interstitial ad did load.
        /// </summary>
        public event FBInterstitialAdBridgeCallback InterstitialAdDidLoad;

        /// <summary>
        /// Occurs when interstitial ad will log impression.
        /// </summary>
        public event FBInterstitialAdBridgeCallback InterstitialAdWillLogImpression;

        /// <summary>
        /// Occurs when interstitial ad did fail with error.
        /// </summary>
        public event FBInterstitialAdBridgeErrorCallback InterstitialAdDidFailWithError;

        /// <summary>
        /// Occurs when interstitial ad did click.
        /// </summary>
        public event FBInterstitialAdBridgeCallback InterstitialAdDidClick;

        /// <summary>
        /// Occurs when interstitial ad will close.
        /// </summary>
        public event FBInterstitialAdBridgeCallback InterstitialAdWillClose;

        /// <summary>
        /// Occurs when interstitial ad did close.
        /// </summary>
        public event FBInterstitialAdBridgeCallback InterstitialAdDidClose;

#if UNITY_ANDROID
        /// <summary>
        /// Only relevant to Android.
        /// This event will only occur if the Interstitial activity has
        /// been destroyed without being properly closed. This can happen if an
        /// app with launchMode:singleTask (such as a Unity game) goes to
        /// background and is then relaunched by tapping the icon.
        /// </summary>
        public event FBInterstitialAdBridgeCallback InterstitialAdActivityDestroyed;
#endif

        /// <summary>
        /// Occurs when rewarded video ad did load.
        /// </summary>
        public event FBRewardedVideoAdBridgeCallback RewardedVideoAdDidLoad;

        /// <summary>
        /// Occurs when rewarded video ad will log impression.
        /// </summary>
        public event FBRewardedVideoAdBridgeCallback RewardedVideoAdWillLogImpression;

        /// <summary>
        /// Occurs when rewarded video ad did fail with error.
        /// </summary>
        public event FBRewardedVideoAdBridgeErrorCallback RewardedVideoAdDidFailWithError;

        /// <summary>
        /// Occurs when rewarded video ad did click.
        /// </summary>
        public event FBRewardedVideoAdBridgeCallback RewardedVideoAdDidClick;

        /// <summary>
        /// Occurs when rewarded video ad will close.
        /// </summary>
        public event FBRewardedVideoAdBridgeCallback RewardedVideoAdWillClose;

        /// <summary>
        /// Occurs when rewarded video ad did close.
        /// </summary>
        public event FBRewardedVideoAdBridgeCallback RewardedVideoAdDidClose;

        /// <summary>
        /// Occurs when rewarded video ad complete.
        /// </summary>
        public event FBRewardedVideoAdBridgeCallback RewardedVideoAdComplete;

        /// <summary>
        /// Occurs when rewarded video ad did succeed.
        /// </summary>
        public event FBRewardedVideoAdBridgeCallback RewardedVideoAdDidSucceed;

        /// <summary>
        /// Occurs when rewarded video ad did fail.
        /// </summary>
        public event FBRewardedVideoAdBridgeCallback RewardedVideoAdDidFail;

#if UNITY_ANDROID
        /// <summary>
        /// Only relevant to Android.
        /// This event will only occur if the Rewarded Video activity
        /// has been destroyed without being properly closed.This can happen if
        /// an app with launchMode:singleTask(such as a Unity game) goes to
        /// background and is then relaunched by tapping the icon.
        /// </summary>
        public event FBRewardedVideoAdBridgeCallback RewardedVideoAdActivityDestroyed;
#endif

#endif

        #endregion  // FBAN-Specific events

        private const string NO_SDK_MESSAGE = "SDK missing. Please import the FB Audience Network plugin.";

        /// <summary>
        /// Name of the Unity's GameObject that will be used to register this network (required by FaceBook Audience Network).
        /// </summary>
        private const string AD_HANDLER_GO_NAME = "EM_Ads_FBAN_Handler";

#if EM_FBAN

        /// <summary>
        /// The banner ad will be moved to this position when hiding.
        /// </summary>
        /// Since Facebook Audience doesn't has a method to hide banner ad,
        /// we will move it far away from the camera instead.
        private readonly Vector2 BANNER_HIDE_POSITION = new Vector2(9999, 9999);

        private AudienceNetworkSettings mAdSettings;
        private GameObject mAdHandlerObject;

        private BannerAd mDefaultBanner;
        private Interstitial mDefaultInterstitial;
        private RewardedVideo mDefaultRewardedVideo;

        /// <summary>
        /// We're gonna save all loaded custom banner ads here.
        /// </summary>
        /// Key: The AdPlacement used to load the banner.
        /// Value: Loaded banner.
        private Dictionary<AdPlacement, BannerAd> mCustomBannerAds;

        /// <summary>
        /// We're gonna save all loaded custom interstitial ads here.
        /// </summary>
        /// Key: The AdPlacement used to load the interstitial ad.
        /// Value: Loaded interstitial ad.
        private Dictionary<AdPlacement, Interstitial> mCustomInterstitialAds;

        /// <summary>
        /// We're gonna save all loaded custom rewarded video ads here.
        /// </summary>
        /// Key: The AdPlacement used to load the rewarded ad.
        /// Value: Loaded rewarded ad.
        private Dictionary<AdPlacement, RewardedVideo> mCustomRewardedVideoAds;

#endif

        #region Singleton

        private static AudienceNetworkClientImpl sInstance;

        private AudienceNetworkClientImpl()
        {
        }

        public static AudienceNetworkClientImpl CreateClient()
        {
            if (sInstance == null)
            {
                sInstance = new AudienceNetworkClientImpl();
            }
            return sInstance;
        }

        #endregion

        #region AdClient Overrides

        public override AdNetwork Network { get { return AdNetwork.AudienceNetwork; } }

        public override bool IsBannerAdSupported { get { return true; } }

        public override bool IsInterstitialAdSupported { get { return true; } }

        public override bool IsRewardedAdSupported { get { return true; } }

        public override bool IsSdkAvail
        {
            get
            {
#if EM_FBAN
                return true;
#else
                return false;
#endif
            }
        }

        public override bool IsValidPlacement(AdPlacement placement, AdType type)
        {
#if EM_FBAN
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
#if EM_FBAN
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
#if EM_FBAN
                return mAdSettings == null ? null : mAdSettings.CustomRewardedAdIds;
#else
                return null;
#endif
            }
        }

        protected override string NoSdkMessage { get { return NO_SDK_MESSAGE; } }

        protected override void InternalInit()
        {
#if EM_FBAN

            mIsInitialized = true;
            mAdSettings = EM_Settings.Advertising.AudienceNetwork;

            mCustomBannerAds = new Dictionary<AdPlacement, BannerAd>();
            mCustomInterstitialAds = new Dictionary<AdPlacement, Interstitial>();
            mCustomRewardedVideoAds = new Dictionary<AdPlacement, RewardedVideo>();

            if (mAdSettings.EnableTestMode)
            {
                SetupTestMode(mAdSettings);
            }

            // The FB Audience Network ads need to be registered to a gameObject,
            // so we just create one here and register them to it.
            mAdHandlerObject = new GameObject(AD_HANDLER_GO_NAME);

            // This game object should persist across scenes.
            UnityEngine.Object.DontDestroyOnLoad(mAdHandlerObject);

            Debug.Log("Audience Network client has been initialized.");
#endif
        }

        protected override void InternalShowBannerAd(AdPlacement placement, BannerAdPosition position, BannerAdSize size)
        {
#if EM_FBAN
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
                mDefaultBanner = ShowBannerAd(mDefaultBanner, id, position, size, placement);
            }
            else // Custom banner...
            {
                if (!mCustomBannerAds.ContainsKey(placement))
                {
                    mCustomBannerAds.Add(placement, CreateNewBannerAd(id, ToFBAdSize(size)));
                }

                mCustomBannerAds[placement] = ShowBannerAd(mCustomBannerAds[placement], id, position, size, placement);
            }
#endif
        }

        protected override void InternalHideBannerAd(AdPlacement placement)
        {
#if EM_FBAN
            if (placement.Equals(AdPlacement.Default)) // Default banner...
            {
                HideBannerAd(mDefaultBanner);
            }
            else // Custom banner...
            {
                if (!mCustomBannerAds.ContainsKey(placement))
                    return;

                HideBannerAd(mCustomBannerAds[placement]);
            }
#endif
        }

        protected override void InternalDestroyBannerAd(AdPlacement placement)
        {
#if EM_FBAN
            if (placement.Equals(AdPlacement.Default)) // Default banner...
            {
                DestroyBannerAd(mDefaultBanner);
            }
            else // Custom banner...
            {
                if (!mCustomBannerAds.ContainsKey(placement))
                    return;

                DestroyBannerAd(mCustomBannerAds[placement]);
            }
#endif
        }

        protected override bool InternalIsInterstitialAdReady(AdPlacement placement)
        {
#if EM_FBAN
            if (placement.Equals(AdPlacement.Default)) // Default interstitial ad...
            {
                return mDefaultInterstitial != null && mDefaultInterstitial.IsReady;
            }
            else // Custom interstitial ad...
            {
                return mCustomInterstitialAds != null &&
                mCustomInterstitialAds.ContainsKey(placement) &&
                mCustomInterstitialAds[placement] != null &&
                mCustomInterstitialAds[placement].IsReady;
            }
#else
            return false;
#endif
        }

        protected override void InternalLoadInterstitialAd(AdPlacement placement)
        {
#if EM_FBAN
            if (placement.Equals(AdPlacement.Default)) // Default interstitial ad...
            {
                if (mDefaultInterstitial == null)
                    mDefaultInterstitial = CreateNewInterstitialAd(mAdSettings.DefaultInterstitialAdId.Id, AdPlacement.Default);

                if (!mDefaultInterstitial.IsLoading && !mDefaultInterstitial.IsReady)
                {
                    mDefaultInterstitial.IsLoading = true;
                    mDefaultInterstitial.Ad.LoadAd();
                }
            }
            else // Custom interstitial ad...
            {
                if (!mCustomInterstitialAds.ContainsKey(placement) || mCustomInterstitialAds[placement] == null)
                {
                    string id = FindIdForPlacement(mAdSettings.CustomInterstitialAdIds, placement);
                    if (string.IsNullOrEmpty(id))
                    {
                        Debug.LogFormat("Attempting to load {0} interstitial ad with an undefined ID at placement {1}",
                            Network.ToString(),
                            AdPlacement.GetPrintableName(placement));
                        return;
                    }

                    mCustomInterstitialAds[placement] = CreateNewInterstitialAd(id, placement);
                }

                if (!mCustomInterstitialAds[placement].IsLoading && !mCustomInterstitialAds[placement].IsReady)
                {
                    mCustomInterstitialAds[placement].IsLoading = true;
                    mCustomInterstitialAds[placement].Ad.LoadAd();
                }
            }
#endif
        }

        protected override void InternalShowInterstitialAd(AdPlacement placement)
        {
#if EM_FBAN
            if (placement.Equals(AdPlacement.Default)) // Default interstitial ad...
            {
                if (mDefaultInterstitial == null)
                    return;

                mDefaultInterstitial.Ad.Show();
                mDefaultInterstitial.IsReady = false;
            }
            else // Custom interstitial ad...
            {
                if (!mCustomInterstitialAds.ContainsKey(placement) || mCustomInterstitialAds[placement] == null)
                    return;

                mCustomInterstitialAds[placement].Ad.Show();
                mCustomInterstitialAds[placement].IsReady = false;
            }
#endif
        }

        protected override bool InternalIsRewardedAdReady(AdPlacement placement)
        {
#if EM_FBAN
            if (placement.Equals(AdPlacement.Default)) // Default rewarded ad...
            {
                return mDefaultRewardedVideo != null && mDefaultRewardedVideo.IsReady;
            }
            else // Custom rewarded ad...
            {
                return mCustomRewardedVideoAds != null &&
                mCustomRewardedVideoAds.ContainsKey(placement) &&
                mCustomRewardedVideoAds[placement] != null &&
                mCustomRewardedVideoAds[placement].IsReady;
            }
#else
            return false;
#endif
        }

        protected override void InternalLoadRewardedAd(AdPlacement placement)
        {
#if EM_FBAN
            if (placement.Equals(AdPlacement.Default)) // Default rewarded ad...
            {
                if (mDefaultRewardedVideo == null)
                    mDefaultRewardedVideo = CreateNewRewardedVideoAd(mAdSettings.DefaultRewardedAdId.Id, AdPlacement.Default);

                mDefaultRewardedVideo.Ad.LoadAd();
            }
            else // Custom rewarded ad...
            {
                if (!mCustomRewardedVideoAds.ContainsKey(placement) || mCustomRewardedVideoAds[placement] == null)
                {
                    string id = FindIdForPlacement(mAdSettings.CustomRewardedAdIds, placement);
                    if (string.IsNullOrEmpty(id))
                    {
                        Debug.LogFormat("Attempting to load {0} rewarded ad with an undefined ID at placement {1}",
                            Network.ToString(),
                            AdPlacement.GetPrintableName(placement));
                        return;
                    }
                    mCustomRewardedVideoAds[placement] = CreateNewRewardedVideoAd(id, placement);
                }

                mCustomRewardedVideoAds[placement].Ad.LoadAd();
            }
#endif
        }

        protected override void InternalShowRewardedAd(AdPlacement placement)
        {
#if EM_FBAN
            if (placement.Equals(AdPlacement.Default)) // Default rewarded ad...
            {
                if (mDefaultRewardedVideo == null)
                    return;

                mDefaultRewardedVideo.Ad.Show();
                mDefaultRewardedVideo.IsReady = false;
            }
            else // Custom rewarded ad...
            {
                if (!mCustomRewardedVideoAds.ContainsKey(placement) || mCustomRewardedVideoAds[placement] == null)
                    return;

                mCustomRewardedVideoAds[placement].Ad.Show();
                mCustomRewardedVideoAds[placement].IsReady = false;
            }
#endif
        }

        #endregion

        #region IConsentRequirable Overrides

        private const string DATA_PRIVACY_CONSENT_KEY = "EM_Ads_FacebookAudience_DataPrivacyConsent";

        protected override string DataPrivacyConsentSaveKey { get { return DATA_PRIVACY_CONSENT_KEY; } }

        protected override void ApplyDataPrivacyConsent(ConsentStatus consent)
        {
            // FB Audience Network doesn't ask for any consent...
        }

        #endregion

        #region  Ad Event Handlers

#if EM_FBAN

        private void OnBannerAdFailedWithError(string error)
        {
            if (AdViewDidFailWithError != null)
                AdViewDidFailWithError(error);
        }

        private void OnBannerAdLoaded()
        {
            if (AdViewDidLoad != null)
                AdViewDidLoad();
        }

        private void OnBannerAdClicked()
        {
            if (AdViewDidClick != null)
                AdViewDidClick();
        }

        private void OnBannerAdFinishClicked()
        {
            if (AdViewDidFinishClick != null)
                AdViewDidFinishClick();
        }

        private void OnBannerAdWillLogImpression()
        {
            if (AdViewWillLogImpression != null)
                AdViewWillLogImpression();
        }

        private void OnInterstitialAdClosed(AdPlacement placement)
        {
            if (placement == null)
                return;

            // Dispose ad.
            if (placement.Equals(AdPlacement.Default))
            {
                if (mDefaultInterstitial != null && mDefaultInterstitial.Ad != null)
                {
                    mDefaultInterstitial.Ad.Dispose();
                    mDefaultInterstitial = null;
                }
            }
            else
            {
                if (mCustomInterstitialAds.ContainsKey(placement) &&
                    mCustomInterstitialAds[placement] != null &&
                    mCustomInterstitialAds[placement].Ad != null)
                {
                    mCustomInterstitialAds[placement].Ad.Dispose();
                    mCustomInterstitialAds.Remove(placement);
                }
            }

            // Raise top level event.
            OnInterstitialAdCompleted(placement);

            if (InterstitialAdDidClose != null)
                InterstitialAdDidClose();
        }

        private void OnInterstitialAdClicked(AdPlacement placement)
        {
            if (InterstitialAdDidClick != null)
                InterstitialAdDidClick();
        }

        private void OnInterstitialAdFailedWithError(AdPlacement placement, string error)
        {
            Debug.Log("Interstitial ad failed to load with error: " + error);

            if (InterstitialAdDidFailWithError != null)
                InterstitialAdDidFailWithError(error);
        }

        private void OnInterstitialAdLoaded(AdPlacement placement)
        {
            if (InterstitialAdDidLoad != null)
                InterstitialAdDidLoad();
        }

        private void OnInterstitialWillClose(AdPlacement placement)
        {
            if (InterstitialAdWillClose != null)
                InterstitialAdWillClose();
        }

        private void OnInterstitialWillLogImpression(AdPlacement placement)
        {
            if (InterstitialAdWillLogImpression != null)
                InterstitialAdWillLogImpression();
        }

#if UNITY_ANDROID
        private void OnInterstitialAdActivityDestroyed(AdPlacement placement)
        {
            if (placement == null)
                return;

            if (placement.Equals(AdPlacement.Default))
                mDefaultInterstitial = null;
            else
                mCustomInterstitialAds.Remove(placement);

            if (InterstitialAdActivityDestroyed != null)
                InterstitialAdActivityDestroyed();
        }
#endif

        private void OnRewardVideoAdComplete(AdPlacement placement)
        {
            OnRewardedAdCompleted(placement);

            if (RewardedVideoAdComplete != null)
                RewardedVideoAdComplete();
        }

        private void OnRewardedVideoAdClicked(AdPlacement placement)
        {
            if (RewardedVideoAdDidClick != null)
                RewardedVideoAdDidClick();
        }

        private void OnRewaredVideoAdClosed(AdPlacement placement)
        {
            if (placement == null)
                return;

            // Dispose ad.
            if (placement.Equals(AdPlacement.Default))
            {
                if (mDefaultRewardedVideo != null && mDefaultRewardedVideo.Ad != null)
                {
                    mDefaultRewardedVideo.Ad.Dispose();
                    mDefaultRewardedVideo = null;
                }
            }
            else
            {
                if (mCustomRewardedVideoAds.ContainsKey(placement) &&
                    mCustomRewardedVideoAds[placement] != null &&
                    mCustomRewardedVideoAds[placement].Ad != null)
                {
                    mCustomRewardedVideoAds[placement].Ad.Dispose();
                    mCustomRewardedVideoAds.Remove(placement);
                }
            }

            if (RewardedVideoAdDidClose != null)
                RewardedVideoAdDidClose();
        }

        private void OnRewardedVideoAdFailedWithError(AdPlacement placement, string error)
        {
            Debug.Log("RewardedVideo ad failed to load with error: " + error);

            if (RewardedVideoAdDidFailWithError != null)
                RewardedVideoAdDidFailWithError(error);
        }

        private void OnRewardVideoAdLoaded(AdPlacement placement)
        {
            if (RewardedVideoAdDidLoad != null)
                RewardedVideoAdDidLoad();
        }

        private void OnRewardVideoAdFailed(AdPlacement placement)
        {
            if (placement == null)
                return;

            Debug.Log("Rewarded video ad not validated, or no response from server. Placement: " + placement);

            if (RewardedVideoAdDidFail != null)
                RewardedVideoAdDidFail();
        }

        private void OnRewardedVideoAdSucceeded(AdPlacement placement)
        {
            if (placement == null)
                return;

            Debug.Log("Rewarded video ad validated by server. Placement: " + placement);

            if (RewardedVideoAdDidSucceed != null)
                RewardedVideoAdDidSucceed();
        }

        private void OnRewardedVideoAdWillClose(AdPlacement placement)
        {
            if (RewardedVideoAdWillClose != null)
                RewardedVideoAdWillClose();
        }

        private void OnRewardedVideoAdWillLogImpression(AdPlacement placement)
        {
            if (RewardedVideoAdWillLogImpression != null)
                RewardedVideoAdWillLogImpression();
        }

#if UNITY_ANDROID
        private void OnRewardedVideoAdActivityDestroyed(AdPlacement placement)
        {
            if (placement == null)
                return;

            if (placement.Equals(AdPlacement.Default))
                mDefaultRewardedVideo = null;
            else
                mCustomRewardedVideoAds.Remove(placement);

            if (RewardedVideoAdActivityDestroyed != null)
                RewardedVideoAdActivityDestroyed();
        }
#endif

#endif

        #endregion

        #region Create & Setup Events methods

#if EM_FBAN

        /// <summary>
        /// Create new banner ad.
        /// </summary>
        protected virtual BannerAd CreateNewBannerAd(string bannerId, AdSize adSize)
        {
            if (string.IsNullOrEmpty(bannerId))
            {
                return null;
            }

            AdView newBanner = new AdView(bannerId, adSize);
            newBanner.Register(mAdHandlerObject);
            SetupBannerAdEvents(newBanner);

            return new BannerAd(newBanner, adSize);
        }

        protected virtual void SetupBannerAdEvents(AdView bannerAd)
        {
            if (bannerAd != null)
            {
                bannerAd.AdViewDidFailWithError += OnBannerAdFailedWithError;
                bannerAd.AdViewDidLoad += OnBannerAdLoaded;
                bannerAd.AdViewDidClick += OnBannerAdClicked;
                bannerAd.AdViewDidFinishClick += OnBannerAdFinishClicked;
                bannerAd.AdViewWillLogImpression += OnBannerAdWillLogImpression;
            }
        }

        /// <summary>
        /// Create new interstitial with specific id.
        /// </summary>
        protected virtual Interstitial CreateNewInterstitialAd(string interstitialId, AdPlacement placement)
        {
            if (string.IsNullOrEmpty(interstitialId))
            {
                return null;
            }

            InterstitialAd newInterstitial = new InterstitialAd(interstitialId);
            SetupInterstitialAdEvents(newInterstitial, placement);

            newInterstitial.Register(mAdHandlerObject);

            return new Interstitial(newInterstitial);
        }

        protected virtual void SetupInterstitialAdEvents(InterstitialAd interstitialAd, AdPlacement placement)
        {
            if (interstitialAd != null)
            {
                interstitialAd.InterstitialAdDidClose += () => OnInterstitialAdClosed(placement);
                interstitialAd.InterstitialAdDidClick += () => OnInterstitialAdClicked(placement);
                interstitialAd.InterstitialAdDidFailWithError += (error) => OnInterstitialAdFailedWithError(placement, error);
                interstitialAd.InterstitialAdDidLoad += () => OnInterstitialAdLoaded(placement);
                interstitialAd.InterstitialAdWillClose += () => OnInterstitialWillClose(placement);
                interstitialAd.InterstitialAdWillLogImpression += () => OnInterstitialWillLogImpression(placement);
#if UNITY_ANDROID
                interstitialAd.InterstitialAdActivityDestroyed += () => OnInterstitialAdActivityDestroyed(placement);
#endif
            }
        }

        /// <summary>
        /// Create new rewarded video ad with specific id.
        /// </summary>
        protected virtual RewardedVideo CreateNewRewardedVideoAd(string rewardedId, AdPlacement placement)
        {
            if (string.IsNullOrEmpty(rewardedId))
            {
                return null;
            }

            RewardedVideoAd newRewardedVideoAd = new RewardedVideoAd(rewardedId);
            SetupRewardedVideoEvents(newRewardedVideoAd, placement);

            newRewardedVideoAd.Register(mAdHandlerObject);

            return new RewardedVideo(newRewardedVideoAd);
        }

        protected void SetupRewardedVideoEvents(RewardedVideoAd rewardedVideoAd, AdPlacement placement)
        {
            if (rewardedVideoAd != null)
            {
                rewardedVideoAd.RewardedVideoAdComplete += () => OnRewardVideoAdComplete(placement);
                rewardedVideoAd.RewardedVideoAdDidClick += () => OnRewardedVideoAdClicked(placement);
                rewardedVideoAd.RewardedVideoAdDidClose += () => OnRewaredVideoAdClosed(placement);
                rewardedVideoAd.RewardedVideoAdDidFailWithError += (error) => OnRewardedVideoAdFailedWithError(placement, error);
                rewardedVideoAd.RewardedVideoAdDidLoad += () => OnRewardVideoAdLoaded(placement);
                rewardedVideoAd.RewardedVideoAdDidFail += () => OnRewardVideoAdFailed(placement);
                rewardedVideoAd.RewardedVideoAdDidSucceed += () => OnRewardedVideoAdSucceeded(placement);
                rewardedVideoAd.RewardedVideoAdWillClose += () => OnRewardedVideoAdWillClose(placement);
                rewardedVideoAd.RewardedVideoAdWillLogImpression += () => OnRewardedVideoAdWillLogImpression(placement);
#if UNITY_ANDROID
                rewardedVideoAd.RewardedVideoAdActivityDestroyed += () => OnRewardedVideoAdActivityDestroyed(placement);
#endif
            }
        }

#endif

        #endregion

        #region Test Mode related methods

        protected virtual void SetupTestMode(AudienceNetworkSettings adSettings)
        {
            SetupTestDevices(adSettings.TestDevices);
        }

        protected virtual void SetupTestDevices(string[] ids)
        {
#if EM_FBAN
            if (ids == null)
                return;

            foreach (string id in ids)
            {
                if (id == null)
                    continue;

                AudienceNetwork.AdSettings.AddTestDevice(id);
            }
#else
            Debug.Log(NO_SDK_MESSAGE);
#endif
        }

        #endregion

        #region Other stuff

#if EM_FBAN

        protected virtual BannerAd ShowBannerAd(BannerAd banner, string bannerID, BannerAdPosition position, BannerAdSize size,
                                                AdPlacement placement)
        {
            /// If the default banner is null or user request a new banner with different size with the created one,
            /// create a new banner (since we can only set fb's banner ad size when creating it).
            if (banner == null || banner.Ad == null || banner.CurrentSize != ToFBAdSize(size))
            {
                /// Destroy old banner.
                DestroyBannerAd(placement);

                /// Create new one.
                banner = CreateNewBannerAd(bannerID, ToFBAdSize(size));
            }

            /// Load the banner if it hasn't been loaded yet.
            if (!banner.IsLoaded)
                banner.Ad.LoadAd();

            banner.Ad.Show(ToFBAudienceAdPosition(position));

            return banner;
        }

        protected virtual void HideBannerAd(BannerAd banner)
        {
            if (banner == null || banner.Ad == null)
                return;

            // The FB Audience's banner ad doesn't have Hide method,
            // so we just move it far away from the camera instead.
            banner.Ad.Show(BANNER_HIDE_POSITION.x, BANNER_HIDE_POSITION.y);
        }

        protected virtual void DestroyBannerAd(BannerAd banner)
        {
            if (banner != null)
            {
                HideBannerAd(banner);
                banner.Ad = null;
                banner.IsLoaded = false;
            }
        }

        protected virtual AdSize ToFBAdSize(BannerAdSize adSize)
        {
            return adSize.IsSmartBanner ? AdSize.BANNER_HEIGHT_50 : ToFBNearestSize(adSize);
        }

        protected virtual AdSize ToFBNearestSize(BannerAdSize adSize)
        {
            if (adSize.Height < 75)
                return AdSize.BANNER_HEIGHT_50;

            if (adSize.Height < 150)
                return AdSize.BANNER_HEIGHT_90;

            return AdSize.RECTANGLE_HEIGHT_250;
        }

        protected virtual AdSize ToAdSize(AudienceNetworkSettings.FBAudienceBannerAdSize bannerAdSize)
        {
            switch (bannerAdSize)
            {
                case AudienceNetworkSettings.FBAudienceBannerAdSize._50:
                    return AdSize.BANNER_HEIGHT_50;

                case AudienceNetworkSettings.FBAudienceBannerAdSize._90:
                    return AdSize.BANNER_HEIGHT_90;

                case AudienceNetworkSettings.FBAudienceBannerAdSize._250:
                    return AdSize.RECTANGLE_HEIGHT_250;

                default:
                    return AdSize.BANNER_HEIGHT_50;
            }
        }

        protected virtual AdPosition ToFBAudienceAdPosition(BannerAdPosition pos)
        {
            switch (pos)
            {
                case BannerAdPosition.Bottom:
                case BannerAdPosition.BottomLeft:
                case BannerAdPosition.BottomRight:
                    return AdPosition.BOTTOM;

                case BannerAdPosition.Top:
                case BannerAdPosition.TopLeft:
                case BannerAdPosition.TopRight:
                default:
                    return AdPosition.TOP;
            }
        }

#endif

        #endregion

    }
}
