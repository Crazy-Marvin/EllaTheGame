using System;
using System.Collections.Generic;
using UnityEngine;

namespace EasyMobile
{
    public class VungleClientImpl : AdClientImpl
    {
        private class AppStateHandler : MonoBehaviour
        {
            private static AppStateHandler instance;
            private void Awake()
            {
                if (instance != null)
                    return;

                instance = this;
                DontDestroyOnLoad(gameObject);
            }

#if EM_VUNGLE
            void OnApplicationPause(bool pauseStatus)
            {
                if (pauseStatus)
                {
                    Vungle.onPause();
                }
                else
                {
                    Vungle.onResume();
                }
            }
#endif
        }

        private static VungleClientImpl sInstance;
        /// <summary>
        /// Returns the singleton client.
        /// </summary>
        /// <returns>The client.</returns>
        public static VungleClientImpl CreateClient()
        {
            if (sInstance == null)
            {
                sInstance = new VungleClientImpl();
            }
            return sInstance;
        }

        private class VungleAd
        {
            public readonly AdId Id;
            public readonly AdPlacement Placement;
            public VungleAd(AdId id, AdPlacement placement)
            {
                this.Id = id;
                this.Placement = placement;
            }
        }

        private class VungleBannerAd : VungleAd
        {
            private bool isShowing;

            public VungleBannerAd(AdId id, AdPlacement placement) : base(id, placement) { }
#if EM_VUNGLE
            private Vungle.VungleBannerSize GetVungleBannerSize(BannerAdSize size)
            {
                if (size.IsSmartBanner)
                    return Vungle.VungleBannerSize.VungleAdSizeBanner;
                else if (size.Height < 70)
                    return Vungle.VungleBannerSize.VungleAdSizeBannerShort;
                else if (size.Height < 170)
                    return Vungle.VungleBannerSize.VungleAdSizeBannerMedium;
                else
                    return Vungle.VungleBannerSize.VungleAdSizeBannerLeaderboard;
            }

            private Vungle.VungleBannerPosition GetVungleBannerPosition(BannerAdPosition position)
            {
                switch (position)
                {
                    case BannerAdPosition.Top:
                        return Vungle.VungleBannerPosition.TopCenter;
                    case BannerAdPosition.Bottom:
                        return Vungle.VungleBannerPosition.BottomCenter;
                    case BannerAdPosition.TopLeft:
                        return Vungle.VungleBannerPosition.TopLeft;
                    case BannerAdPosition.TopRight:
                        return Vungle.VungleBannerPosition.TopRight;
                    case BannerAdPosition.BottomLeft:
                        return Vungle.VungleBannerPosition.BottomLeft;
                    case BannerAdPosition.BottomRight:
                        return Vungle.VungleBannerPosition.BottomRight;
                }
                return Vungle.VungleBannerPosition.BottomCenter;
            }
#endif

            internal void Destroy()
            {
                isShowing = false;
#if EM_VUNGLE
                Vungle.closeBanner(Id.Id);
#endif
            }

            internal void Hide()
            {
                isShowing = false;
#if EM_VUNGLE
                Vungle.closeBanner(Id.Id);
#endif
            }

            internal void Show(BannerAdSize size, BannerAdPosition position)
            {
#if EM_VUNGLE
                if (Vungle.isAdvertAvailable(Id.Id, GetVungleBannerSize(size)))
                    Vungle.showBanner(Id.Id);
                else
                    Vungle.loadBanner(Id.Id, GetVungleBannerSize(size), GetVungleBannerPosition(position));
#endif
                isShowing = true;
            }

            public void BannerLoadedShowIfRequired()
            {
                if (!isShowing)
                    return;
#if EM_VUNGLE
                Vungle.showBanner(Id.Id);
#endif
            }
        }

        private class VungleInterstitialAd : VungleAd
        {
#if EM_VUNGLE
            private Dictionary<string, object> options;
#endif
            public VungleInterstitialAd(AdId id, AdPlacement placement, Dictionary<string, object> options) : base(id, placement)
            {
#if EM_VUNGLE
                this.options = options;
#endif
            }

            public void Load()
            {
#if EM_VUNGLE
                if (Vungle.isAdvertAvailable(Id.Id))
                    return;
                Vungle.loadAd(Id.Id);
#endif
            }

            internal void Show()
            {
#if EM_VUNGLE
                if (Vungle.isAdvertAvailable(Id.Id))
                    if (options != null)
                        Vungle.playAd(options, Id.Id);
                    else
                        Vungle.playAd(Id.Id);
#endif
            }

            internal bool IsReady()
            {
#if EM_VUNGLE
                return Vungle.isAdvertAvailable(Id.Id);
#else          
                return false;
#endif
            }
        }

        private class VungleRewardedAd : VungleAd
        {
#if EM_VUNGLE
            private Dictionary<string, object> options;
#endif
            public VungleRewardedAd(AdId id, AdPlacement placement, Dictionary<string, object> options) : base(id, placement)
            {
#if EM_VUNGLE
                this.options = options;
#endif
            }

            public void Load()
            {
#if EM_VUNGLE
                if (Vungle.isAdvertAvailable(Id.Id))
                    return;
                Vungle.loadAd(Id.Id);
#endif
            }

            internal void Show()
            {
#if EM_VUNGLE
                if (Vungle.isAdvertAvailable(Id.Id))
                    if (options != null)
                        Vungle.playAd(options, Id.Id);
                    else
                        Vungle.playAd(Id.Id);
#endif
            }

            internal bool IsReady()
            {
#if EM_VUNGLE
                return Vungle.isAdvertAvailable(Id.Id);
#else          
                return false;
#endif
            }
        }

        private const string NO_SDK_MESSAGE = "SDK missing. Please import the Vungle plugin.";

        private VungleSettings mAdSettings = null;

        public override bool IsSdkAvail
        {
            get
            {
#if EM_VUNGLE
                return true;
#else
                return false;
#endif
            }
        }

        public override AdNetwork Network { get { return AdNetwork.Vungle; } }

        public override bool IsBannerAdSupported { get { return true; } }

        public override bool IsInterstitialAdSupported { get { return true; } }

        public override bool IsRewardedAdSupported { get { return true; } }

        protected override string NoSdkMessage { get { return NO_SDK_MESSAGE; } }

        protected override Dictionary<AdPlacement, AdId> CustomInterstitialAdsDict
        {
            get
            {
#if EM_VUNGLE
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
#if EM_VUNGLE
                return mAdSettings == null ? null : mAdSettings.CustomRewardedAdIds;
#else
                return null;
#endif
            }
        }

        private const string DATA_PRIVACY_CONSENT_KEY = "EM_Ads_Vungle_DataPrivacyConsent";
        protected override string DataPrivacyConsentSaveKey { get { return DATA_PRIVACY_CONSENT_KEY; } }

        private Dictionary<AdPlacement, VungleBannerAd> customBannerPlacementDict;
        private Dictionary<AdPlacement, VungleRewardedAd> customRewardedPlacementDict;
        private Dictionary<AdPlacement, VungleInterstitialAd> customInterstitialPlacementDict;

        private VungleInterstitialAd defaultInterstitial;
        private VungleRewardedAd defaultRewarded;
        private VungleBannerAd defaultBanner;

        private Dictionary<string, object> options = null;

        protected override void InternalInit()
        {
            mAdSettings = EM_Settings.Advertising.VungleAds;
#if EM_VUNGLE
            if (mAdSettings.UseAdvancedSetting)
            {
                options = new Dictionary<string, object>();
                VungleAdOrientation orientation = VungleAdOrientation.All;
                switch (mAdSettings.AdvancedSettings.adOrientation)
                {
                    case VungleSettings.VungleAdvancedSettings.AdOrientation.Portrait:
                        orientation = VungleAdOrientation.Portrait;
                        break;
#if UNITY_IOS
                    case VungleSettings.VungleAdvancedSettings.AdOrientation.LandscapeLeft:
                        orientation = VungleAdOrientation.LandscapeLeft;
                        break;
                    case VungleSettings.VungleAdvancedSettings.AdOrientation.LandscapeRight:
                        orientation = VungleAdOrientation.LandscapeRight;
                        break;
                    case VungleSettings.VungleAdvancedSettings.AdOrientation.PortraitUpsideDown:
                        orientation = VungleAdOrientation.PortraitUpsideDown;
                        break;
#endif
                    case VungleSettings.VungleAdvancedSettings.AdOrientation.Landscape:
                        orientation = VungleAdOrientation.Landscape;
                        break;
                    case VungleSettings.VungleAdvancedSettings.AdOrientation.All:
                        orientation = VungleAdOrientation.All;
                        break;
#if UNITY_IOS
                    case VungleSettings.VungleAdvancedSettings.AdOrientation.AllButUpsideDown:
                        orientation = VungleAdOrientation.AllButUpsideDown;
                        break;
#endif
#if UNITY_ANDROID
                    case VungleSettings.VungleAdvancedSettings.AdOrientation.MatchVideo:
                        orientation = VungleAdOrientation.MatchVideo;
                        break;
#endif

                }
                options.Add("orientation", orientation);
                options.Add("alertTitle", mAdSettings.AdvancedSettings.prematureAdClosePopup.alertTitle);
                options.Add("alertText", mAdSettings.AdvancedSettings.prematureAdClosePopup.alertText);
                options.Add("closeText", mAdSettings.AdvancedSettings.prematureAdClosePopup.closeText);
                options.Add("continueText", mAdSettings.AdvancedSettings.prematureAdClosePopup.continueText);
            }
#endif
            customBannerPlacementDict = new Dictionary<AdPlacement, VungleBannerAd>();
            customRewardedPlacementDict = new Dictionary<AdPlacement, VungleRewardedAd>();
            customInterstitialPlacementDict = new Dictionary<AdPlacement, VungleInterstitialAd>();

            foreach (var keyPair in mAdSettings.CustomBannerAdIds)
            {
                if (customBannerPlacementDict.ContainsKey(keyPair.Key))
                    continue;
                customBannerPlacementDict.Add(keyPair.Key, new VungleBannerAd(keyPair.Value, keyPair.Key));
            }
            foreach (var keyPair in mAdSettings.CustomInterstitialAdIds)
            {
                if (customInterstitialPlacementDict.ContainsKey(keyPair.Key))
                    continue;
                customInterstitialPlacementDict.Add(keyPair.Key, new VungleInterstitialAd(keyPair.Value, keyPair.Key, options));
            }
            foreach (var keyPair in mAdSettings.CustomRewardedAdIds)
            {
                if (customRewardedPlacementDict.ContainsKey(keyPair.Key))
                    continue;
                customRewardedPlacementDict.Add(keyPair.Key, new VungleRewardedAd(keyPair.Value, keyPair.Key, options));
            }
            defaultInterstitial = new VungleInterstitialAd(mAdSettings.DefaultInterstitialAdId, AdPlacement.Default, options);
            defaultRewarded = new VungleRewardedAd(mAdSettings.DefaultRewardedAdId, AdPlacement.Default, options);
            defaultBanner = new VungleBannerAd(mAdSettings.DefaultBannerAdId, AdPlacement.Default);

#if EM_VUNGLE
            Vungle.onInitializeEvent += OnInitialized;
            Vungle.onAdStartedEvent += OnAdStarted;
            Vungle.onAdFinishedEvent += OnAdFinished;
            Vungle.adPlayableEvent += OnAdPlayableEvent;

            if (mAdSettings.UseAdvancedSetting)
            {
                Vungle.setSoundEnabled(mAdSettings.AdvancedSettings.enableAdSound);

                Vungle.SetMinimumDiskSpaceForInitialization(mAdSettings.AdvancedSettings.minimumDiskSpaceForInitialization);
                Vungle.SetMinimumDiskSpaceForAd(mAdSettings.AdvancedSettings.minimumDiskSpaceForAds);

                Vungle.EnableHardwareIdPrivacy(mAdSettings.AdvancedSettings.disableHardwareId);
            }

            GameObject appStateHandler = new GameObject("VungleAppstateHandler");
            appStateHandler.hideFlags = HideFlags.HideAndDontSave;
            appStateHandler.AddComponent<AppStateHandler>();

            Vungle.init(mAdSettings.AppId.Id);
#endif
            ApplyDataPrivacyConsent(GetApplicableDataPrivacyConsent());
        }

        private VungleBannerAd GetBannerVungleAd(AdPlacement placement)
        {
            if (placement == AdPlacement.Default)
                return defaultBanner;
            if (customBannerPlacementDict.ContainsKey(placement))
                return customBannerPlacementDict[placement];
            return null;
        }

        private VungleInterstitialAd GetInterstitialVungleAd(AdPlacement placement)
        {
            if (placement == AdPlacement.Default)
                return defaultInterstitial;
            if (customInterstitialPlacementDict.ContainsKey(placement))
                return customInterstitialPlacementDict[placement];
            return null;
        }

        private VungleRewardedAd GetRewardedVungleAd(AdPlacement placement)
        {
            if (placement == AdPlacement.Default)
                return defaultRewarded;
            if (customRewardedPlacementDict.ContainsKey(placement))
                return customRewardedPlacementDict[placement];
            return null;
        }

        private VungleAd FindVungleAd(string id)
        {
            if (id == defaultInterstitial.Id.Id)
                return defaultInterstitial;
            if (id == defaultBanner.Id.Id)
                return defaultBanner;
            if (id == defaultRewarded.Id.Id)
                return defaultRewarded;
            foreach (var keyPair in customBannerPlacementDict)
            {
                if (keyPair.Value.Id.Id == id)
                    return keyPair.Value;
            }
            foreach (var keyPair in customInterstitialPlacementDict)
            {
                if (keyPair.Value.Id.Id == id)
                    return keyPair.Value;
            }
            foreach (var keyPair in customRewardedPlacementDict)
            {
                if (keyPair.Value.Id.Id == id)
                    return keyPair.Value;
            }
            return null;
        }

        #region Vungle callbacks
#if EM_VUNGLE
        private void OnAdPlayableEvent(string vungleId, bool loaded)
        {
            VungleAd vungleAd = FindVungleAd(vungleId);
            if (vungleAd == null)
                return;

            if (vungleAd.GetType() == typeof(VungleBannerAd))
            {
                ((VungleBannerAd)vungleAd).BannerLoadedShowIfRequired();
            }
        }

        private void OnAdFinished(string vungleId, AdFinishedEventArgs finishedEvent)
        {
            VungleAd vungleAd = FindVungleAd(vungleId);

            if (vungleAd.GetType() == typeof(VungleRewardedAd))
            {
                if (finishedEvent.IsCompletedView)
                    OnRewardedAdCompleted(vungleAd.Placement);
                else
                    OnRewardedAdSkipped(vungleAd.Placement);

                return;
            }
            if (vungleAd.GetType() == typeof(InterstitialAdNetwork))
            {
                OnInterstitialAdCompleted(vungleAd.Placement);
                return;
            }
        }

        private void OnAdStarted(string adId)
        {
            //TODO create callback
        }

        private void OnInitialized()
        {
            mIsInitialized = true;
            Debug.Log("Vungle client has been initialized.");
        }
#endif
        #endregion

        public override bool IsValidPlacement(AdPlacement placement, AdType type)
        {
#if !EM_VUNGLE
            return false;
#else
            string id = null;
            VungleAd vungleAd = null;
            switch (type)
            {
                case AdType.Banner:
                    vungleAd = GetBannerVungleAd(placement);
                    break;
                case AdType.Interstitial:
                    vungleAd = GetInterstitialVungleAd(placement);
                    break;
                case AdType.Rewarded:
                    vungleAd = GetRewardedVungleAd(placement);
                    break;
            }
            if (vungleAd != null)
                id = vungleAd.Id.Id;
            if (string.IsNullOrEmpty(id))
                return false;
            return true;
#endif
        }

        protected override void ApplyDataPrivacyConsent(ConsentStatus consent)
        {
#if EM_VUNGLE
            switch (consent)
            {
                case ConsentStatus.Granted:
                    Vungle.updateConsentStatus(Vungle.Consent.Accepted);
                    break;
                case ConsentStatus.Revoked:
                    Vungle.updateConsentStatus(Vungle.Consent.Denied);
                    break;
                case ConsentStatus.Unknown:
                    Vungle.updateConsentStatus(Vungle.Consent.Undefined);
                    break;
                default:
                    break;
            }
#endif
        }

        protected override void InternalDestroyBannerAd(AdPlacement placement)
        {
            VungleBannerAd ad = GetBannerVungleAd(placement);
            if (ad == null)
                return;
            ad.Destroy();
        }

        protected override void InternalHideBannerAd(AdPlacement placement)
        {
            VungleBannerAd ad = GetBannerVungleAd(placement);
            if (ad == null)
                return;
            ad.Hide();
        }

        protected override bool InternalIsInterstitialAdReady(AdPlacement placement)
        {
            VungleInterstitialAd ad = GetInterstitialVungleAd(placement);
            if (ad == null)
                return false;
            return ad.IsReady();
        }

        protected override bool InternalIsRewardedAdReady(AdPlacement placement)
        {
            VungleRewardedAd ad = GetRewardedVungleAd(placement);
            if (ad == null)
                return false;
            return ad.IsReady();
        }

        protected override void InternalLoadInterstitialAd(AdPlacement placement)
        {
            VungleInterstitialAd ad = GetInterstitialVungleAd(placement);
            if (ad == null)
                return;
            ad.Load();
        }

        protected override void InternalLoadRewardedAd(AdPlacement placement)
        {
            VungleRewardedAd ad = GetRewardedVungleAd(placement);
            if (ad == null)
                return;
            ad.Load();
        }

        protected override void InternalShowBannerAd(AdPlacement placement, BannerAdPosition position, BannerAdSize size)
        {
            VungleBannerAd ad = GetBannerVungleAd(placement);
            if (ad == null)
                return;
            ad.Show(size, position);
        }

        protected override void InternalShowInterstitialAd(AdPlacement placement)
        {
            VungleInterstitialAd ad = GetInterstitialVungleAd(placement);
            if (ad == null)
                return;
            ad.Show();
        }

        protected override void InternalShowRewardedAd(AdPlacement placement)
        {
            VungleRewardedAd ad = GetRewardedVungleAd(placement);
            if (ad == null)
                return;
            ad.Show();
        }
    }
}