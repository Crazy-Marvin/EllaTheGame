using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EasyMobile
{
#if EM_UNITY_ADS || UNITY_MONETIZATION
    using UnityEngine.Advertisements;
#endif

    public class UnityAdsClientImpl : AdClientImpl
    {
        private const string NO_SDK_MESSAGE = "SDK missing. Please enable UnityAds service.";
        private const string BANNER_UNSUPPORTED_MESSAGE = "This version of UnityAds package does not support banner ad format.";

#if EM_UNITY_ADS || UNITY_MONETIZATION
        private UnityAdsSettings mAdSettings;
#endif

        #region UnityAds Events

#if EM_UNITY_ADS || UNITY_MONETIZATION
        /// <summary>
        /// Occurs when a banner ad is shown.
        /// </summary>
        public event Action<AdPlacement> BannerAdShownCallback;

        /// <summary>
        /// Occurs when a banner ad is closed.
        /// </summary>
        public event Action<AdPlacement> BannerAdHiddenCallback;

        /// <summary>
        /// Event for interstitial ad activities.
        /// </summary>
        public event Action<ShowResult, AdPlacement> InterstitialAdCallback;

        /// <summary>
        /// Event for rewarded ad activities.
        /// </summary>
        public event Action<ShowResult, AdPlacement> RewardedAdCallback;

#endif

        #endregion

        #region Singleton

        private static UnityAdsClientImpl sInstance;

        private UnityAdsClientImpl()
        {
        }

        /// <summary>
        /// Returns the singleton client.
        /// </summary>
        /// <returns>The client.</returns>
        public static UnityAdsClientImpl CreateClient()
        {
            if (sInstance == null)
            {
                sInstance = new UnityAdsClientImpl();
            }
            return sInstance;
        }

        #endregion  // Object Creators

        #region AdClient Overrides

        public override AdNetwork Network { get { return AdNetwork.UnityAds; } }

        public override bool IsBannerAdSupported
        {
            get
            {
#if UNITY_MONETIZATION
                return true;
#else
                return false;
#endif
            }
        }

        public override bool IsInterstitialAdSupported { get { return true; } }

        public override bool IsRewardedAdSupported { get { return true; } }

        public override bool IsInitialized
        {
            get
            {
#if EM_UNITY_ADS || UNITY_MONETIZATION
                return mIsInitialized && Advertisement.isInitialized;
#else
				return mIsInitialized;
#endif
            }
        }

        protected override Dictionary<AdPlacement, AdId> CustomInterstitialAdsDict
        {
            get
            {
#if EM_UNITY_ADS || UNITY_MONETIZATION
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
#if EM_UNITY_ADS || UNITY_MONETIZATION
                return mAdSettings == null ? null : mAdSettings.CustomRewardedAdIds;
#else
				return null;
#endif
            }
        }

        protected override string NoSdkMessage { get { return NO_SDK_MESSAGE; } }

        public override bool IsSdkAvail
        {
            get
            {
#if EM_UNITY_ADS || UNITY_MONETIZATION
                return true;
#else
				return false;
#endif
            }
        }

        public override bool IsValidPlacement(AdPlacement placement, AdType type)
        {
#if EM_UNITY_ADS
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
                        return false;
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
                        return false;
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

        protected override void InternalInit()
        {
#if EM_UNITY_ADS || UNITY_MONETIZATION
            if (!mIsInitialized)
            {
                mIsInitialized = true;
                mAdSettings = EM_Settings.Advertising.UnityAds;

                // Set GDPR consent (if any).
                var consent = GetApplicableDataPrivacyConsent();
                ApplyDataPrivacyConsent(consent);

#if UNITY_MONETIZATION
                Advertisement.AddListener(new UnityAdsListener(this));
                if (string.IsNullOrEmpty(mAdSettings.AppId.Id))
                {
                    Debug.LogWarning("Attempting to initialize UnityAds with an empty App ID.");
                }
                Advertisement.Initialize(mAdSettings.AppId.Id, mAdSettings.EnableTestMode);
#endif

                Debug.Log("UnityAds client has been initialized.");
            }
#endif
        }

        //------------------------------------------------------------
        // Banner Ads.
        //------------------------------------------------------------


        protected override void InternalShowBannerAd(AdPlacement placement, BannerAdPosition position, BannerAdSize size)
        {
#if UNITY_MONETIZATION
            string id = placement == AdPlacement.Default ?
                mAdSettings.DefaultBannerAdId.Id :
                FindIdForPlacement(mAdSettings.CustomBannerAdIds, placement);

            if (string.IsNullOrEmpty(id))
            {
                Debug.Log("Attempting to show UnityAds banner ad with an undefined ID at placement " + AdPlacement.GetPrintableName(placement));
                return;
            }

            if (!Advertisement.Banner.isLoaded)
            {
                BannerLoadOptions options = new BannerLoadOptions();

                options.errorCallback += ((string message) =>
                {
                    Debug.Log("Error loading Unity banner ad: " + message);
                });

                options.loadCallback += (() =>
                {
                    DoShowBannerAd(id, placement);
                });

                Advertisement.Banner.SetPosition(ToUnityAdsBannerPosition(position));
                Advertisement.Banner.Load(id, options);
            }
            else
            {
                DoShowBannerAd(id, placement);
            }

#else
            Debug.LogWarning(BANNER_UNSUPPORTED_MESSAGE);
#endif
        }

        protected override void InternalHideBannerAd(AdPlacement placement)
        {
#if UNITY_MONETIZATION
            var hideOptions = new BannerOptions
            {
                hideCallback = () =>
                {
                    InternalHideBannerAdCallback(placement);
                }
            };
            Advertisement.Banner.Hide(false);
#else
            Debug.LogWarning(BANNER_UNSUPPORTED_MESSAGE);
#endif
        }

        protected override void InternalDestroyBannerAd(AdPlacement placement)
        {
#if UNITY_MONETIZATION
            var hideOptions = new BannerOptions
            {
                hideCallback = () =>
                {
                    InternalHideBannerAdCallback(placement);
                }
            };
            Advertisement.Banner.Hide(true);
#else
            Debug.LogWarning(BANNER_UNSUPPORTED_MESSAGE);
#endif
        }

        //------------------------------------------------------------
        // Interstitial Ads.
        //------------------------------------------------------------

        protected override void InternalLoadInterstitialAd(AdPlacement _)
        {
            // Unity Ads handles loading automatically.
        }

        protected override bool InternalIsInterstitialAdReady(AdPlacement placement)
        {
#if EM_UNITY_ADS || UNITY_MONETIZATION
            string placementId = placement == AdPlacement.Default ?
                mAdSettings.DefaultInterstitialAdId.Id : FindIdForPlacement(mAdSettings.CustomInterstitialAdIds, placement);

            if (placementId == string.Empty)
                return false;

            return Advertisement.IsReady(placementId);
#else
			return false;
#endif
        }

        protected override void InternalShowInterstitialAd(AdPlacement placement)
        {
#if EM_UNITY_ADS || UNITY_MONETIZATION
            string id = placement == AdPlacement.Default ?
                mAdSettings.DefaultInterstitialAdId.Id : FindIdForPlacement(mAdSettings.CustomInterstitialAdIds, placement);

            if (string.IsNullOrEmpty(id))
            {
                Debug.LogFormat("Attempting to show {0} interstitial ad with an undefined ID at placement {1}",
                    Network.ToString(),
                    AdPlacement.GetPrintableName(placement));
                return;
            }

#if !UNITY_MONETIZATION
            var showOptions = new ShowOptions
            {
                resultCallback = (result) =>
                {
                    InternalInterstitialAdCallback(result, placement);
                }
            };
            Advertisement.Show(id, showOptions);
#else
            Advertisement.Show(id);
#endif
#endif
        }

        //------------------------------------------------------------
        // Rewarded Ads.
        //------------------------------------------------------------

        protected override void InternalLoadRewardedAd(AdPlacement _)
        {
            // Unity Ads handles loading automatically.
        }

        protected override bool InternalIsRewardedAdReady(AdPlacement placement)
        {
#if EM_UNITY_ADS || UNITY_MONETIZATION
            string placementId = placement == AdPlacement.Default ?
                mAdSettings.DefaultRewardedAdId.Id : FindIdForPlacement(mAdSettings.CustomRewardedAdIds, placement);

            if (placementId == string.Empty)
                return false;

            return Advertisement.IsReady(placementId);
#else
			return false;
#endif
        }

        protected override void InternalShowRewardedAd(AdPlacement placement)
        {
#if EM_UNITY_ADS || UNITY_MONETIZATION
            string id = placement == AdPlacement.Default ?
                mAdSettings.DefaultRewardedAdId.Id : FindIdForPlacement(mAdSettings.CustomRewardedAdIds, placement);

            if (string.IsNullOrEmpty(id))
            {
                Debug.LogFormat("Attempting to show {0} rewarded ad with an undefined ID at placement {1}",
                    Network.ToString(),
                    AdPlacement.GetPrintableName(placement));
                return;
            }
#if !UNITY_MONETIZATION
            var showOptions = new ShowOptions
            {
                resultCallback = (result) =>
                {
                    InternalRewardedAdCallback(result, placement);
                }
            };
            Advertisement.Show(id, showOptions);
#else
            Advertisement.Show(id);
#endif
#endif
        }

        #endregion  // AdClient Overrides

        #region IConsentRequirable Overrides

        private const string DATA_PRIVACY_CONSENT_KEY = "EM_Ads_UnityAds_DataPrivacyConsent";

        protected override string DataPrivacyConsentSaveKey { get { return DATA_PRIVACY_CONSENT_KEY; } }

        protected override void ApplyDataPrivacyConsent(ConsentStatus consent)
        {
#if EM_UNITY_ADS || UNITY_MONETIZATION
            // https://unityads.unity3d.com/help/legal/gdpr
            switch (consent)
            {
                case ConsentStatus.Granted:
                    SetGdprMetadata(true);
                    break;
                case ConsentStatus.Revoked:
                    SetGdprMetadata(false);
                    break;
                case ConsentStatus.Unknown:
                default:
                    break;
            }
#endif
        }

        #endregion

        #region GDPR Stuff

#if EM_UNITY_ADS || UNITY_MONETIZATION

        /// <summary>
        /// Set appropriate GDPR metadata. This can be done either before or after initialization.
        /// https://unityads.unity3d.com/help/legal/gdpr
        /// </summary>
        /// <param name="hasConsent">If set to <c>true</c> has consent.</param>
        private void SetGdprMetadata(bool hasConsent)
        {
            MetaData metaData = new MetaData("gdpr");
            metaData.Set("consent", hasConsent ? "true" : "false");
            Advertisement.SetMetaData(metaData);
        }

#endif

        #endregion

        #region Ad Event Handlers

#if EM_UNITY_ADS || UNITY_MONETIZATION

        void InternalShowBannerAdCallback(AdPlacement placement)
        {
            if (BannerAdShownCallback != null)
                BannerAdShownCallback(placement);
        }

        void InternalHideBannerAdCallback(AdPlacement placement)
        {
            if (BannerAdHiddenCallback != null)
                BannerAdHiddenCallback(placement);
        }

        void InternalInterstitialAdCallback(ShowResult result, AdPlacement placement)
        {
            switch (result)
            {
                case ShowResult.Finished:
                    OnInterstitialAdCompleted(placement);
                    break;
                case ShowResult.Skipped:
                    OnInterstitialAdCompleted(placement);
                    break;
                case ShowResult.Failed:
                    break;
            }

            if (InterstitialAdCallback != null)
                InterstitialAdCallback(result, placement);
        }

        void InternalRewardedAdCallback(ShowResult result, AdPlacement placement)
        {
            switch (result)
            {
                case ShowResult.Finished:
                    OnRewardedAdCompleted(placement);
                    break;
                case ShowResult.Skipped:
                    OnRewardedAdSkipped(placement);
                    break;
                case ShowResult.Failed:
                    break;
            }

            if (RewardedAdCallback != null)
                RewardedAdCallback(result, placement);
        }
#endif
        #endregion  // Ad Event Handlers

        #region IUnityAdsListener implementation & UnityAds banner helpers

#if UNITY_MONETIZATION

        private class UnityAdsListener : IUnityAdsListener
        {
            private UnityAdsClientImpl mClient;

            public UnityAdsListener(UnityAdsClientImpl client)
            {
                mClient = client;
            }

            public void OnUnityAdsDidError(string message)
            {
                Debug.Log("OnUnityAdsDidError: " + message);
            }

            public void OnUnityAdsDidFinish(string placementId, ShowResult showResult)
            {
                Debug.Log("OnUnityAdsDidFinish: " + placementId);
                AdType adType;
                var placement = mClient.FindAdPlacementWithId(placementId, out adType);

                if (placement != null)
                {
                    if (adType == AdType.Interstitial)
                    {
                        mClient.InternalInterstitialAdCallback(showResult, placement);
                    }
                    else if (adType == AdType.Rewarded)
                    {
                        mClient.InternalRewardedAdCallback(showResult, placement);
                    }
                }
                else
                {
                    Debug.Log("Unable to find a placement with placementId: " + placementId);
                }
            }

            public void OnUnityAdsDidStart(string placementId)
            {
                Debug.Log("OnUnityAdsDidStart: " + placementId);
            }

            public void OnUnityAdsReady(string placementId)
            {
                Debug.Log("OnUnityAdsReady: " + placementId);
            }
        }

        private void DoShowBannerAd(string id, AdPlacement placement)
        {
            var showOptions = new BannerOptions
            {
                showCallback = () =>
                {
                    InternalShowBannerAdCallback(placement);
                }
            };
            Advertisement.Banner.Show(id, showOptions);
        }

        private BannerPosition ToUnityAdsBannerPosition(BannerAdPosition position)
        {
            switch (position)
            {
                case BannerAdPosition.Bottom:
                    return BannerPosition.BOTTOM_CENTER;
                case BannerAdPosition.BottomLeft:
                    return BannerPosition.BOTTOM_LEFT;
                case BannerAdPosition.BottomRight:
                    return BannerPosition.BOTTOM_RIGHT;
                case BannerAdPosition.Top:
                    return BannerPosition.TOP_CENTER;
                case BannerAdPosition.TopLeft:
                    return BannerPosition.TOP_LEFT;
                case BannerAdPosition.TopRight:
                    return BannerPosition.TOP_RIGHT;
                default:
                    return BannerPosition.CENTER;
            }
        }

#endif
        #endregion


        #region Private stuff

#if EM_UNITY_ADS || UNITY_MONETIZATION

        private AdPlacement FindBannerPlacementWithId(string placementId)
        {
            if (string.IsNullOrEmpty(placementId))
            {
                return null;
            }
            else if (mAdSettings.DefaultBannerAdId.Id.Equals(placementId))
            {
                return AdPlacement.Default;
            }
            else
            {
                foreach (var pair in mAdSettings.CustomBannerAdIds)
                {
                    if (pair.Value.Id.Equals(placementId))
                        return pair.Key;
                }
            }

            return null;
        }

        private AdPlacement FindInterstitialPlacementWithId(string placementId)
        {
            if (string.IsNullOrEmpty(placementId))
            {
                return null;
            }
            else if (mAdSettings.DefaultInterstitialAdId.Id.Equals(placementId))
            {
                return AdPlacement.Default;
            }
            else
            {
                foreach (var pair in mAdSettings.CustomInterstitialAdIds)
                {
                    if (pair.Value.Id.Equals(placementId))
                        return pair.Key;
                }
            }

            return null;
        }

        private AdPlacement FindRewardedPlacementWithId(string placementId)
        {
            if (string.IsNullOrEmpty(placementId))
            {
                return null;
            }
            else if (mAdSettings.DefaultRewardedAdId.Id.Equals(placementId))
            {
                return AdPlacement.Default;
            }
            else
            {
                foreach (var pair in mAdSettings.CustomRewardedAdIds)
                {
                    if (pair.Value.Id.Equals(placementId))
                        return pair.Key;
                }
            }

            return null;
        }

        private AdPlacement FindAdPlacementWithId(string placementId, out AdType adType)
        {
            // Is this a banner placement?
            var placement = FindBannerPlacementWithId(placementId);

            if (placement != null)
            {
                adType = AdType.Banner;
                return placement;
            }

            // or an interstitial placement?
            placement = FindInterstitialPlacementWithId(placementId);

            if (placement != null)
            {
                adType = AdType.Interstitial;
                return placement;
            }

            // or a rewarded placement?
            placement = FindRewardedPlacementWithId(placementId);

            if (placement != null)
            {
                adType = AdType.Rewarded;
                return placement;
            }

            adType = AdType.Banner;
            return null;
        }

#endif
        #endregion
    }
}
