using UnityEngine;
using System;
using System.Linq;
using System.Collections.Generic;

namespace EasyMobile
{
    #if EM_ADCOLONY
    using AdColony;
    #endif

    public class AdColonyClientImpl : AdClientImpl
    {
        #region AdColony Events

        #if EM_ADCOLONY

        /// <summary>
        /// Event that is triggered after a call to Configure has completed.
        /// On Android, the Zone objects aren't fully populated by the time this is called, use GetZone() after a delay if required.
        /// If the configuration is not successful, the list of zones will be empty.
        /// </summary>
        public event Action<List<Zone>> OnConfigurationCompleted
        {
            add { Ads.OnConfigurationCompleted += value; }
            remove { Ads.OnConfigurationCompleted -= value; }
        }

        /// <summary>
        /// Event that is triggered after a call to RequestInterstitialAd has completed successfully.
        /// The InterstitialAd object returned can be used to show an interstitial ad when ready.
        /// </summary>
        public event Action<InterstitialAd> OnRequestInterstitial
        {
            add { Ads.OnRequestInterstitial += value; }
            remove { Ads.OnRequestInterstitial -= value; }
        }

        /// <summary>
        /// Event triggered after a call to RequestInterstitialAd has failed.
        /// Parameter 1: zoneId
        /// </summary>
        public event Action<string> OnRequestInterstitialFailedWithZone
        {
            add { Ads.OnRequestInterstitialFailedWithZone += value; }
            remove { Ads.OnRequestInterstitialFailedWithZone -= value; }
        }

        /// <summary>
        /// Event triggered after an InterstitialAd is opened.
        /// </summary>
        public event Action<InterstitialAd> OnOpened
        {
            add { Ads.OnOpened += value; }
            remove { Ads.OnOpened -= value; }
        }

        /// <summary>
        /// Event triggered after an InterstitialAd is closed.
        /// It's recommended to request a new ad within this callback.
        /// </summary>
        public event Action<InterstitialAd> OnClosed
        {
            add { Ads.OnClosed += value; }
            remove { Ads.OnClosed -= value; }
        }

        /// <summary>
        /// Event triggered after an InterstitialAd expires and is no longer valid for playback.
        /// This does not get triggered when the expired flag is set because it has been viewed.
        /// It's recommended to request a new ad within this callback.
        /// </summary>
        public event Action<InterstitialAd> OnExpiring
        {
            add { Ads.OnExpiring += value; }
            remove { Ads.OnExpiring -= value; }
        }

        /// <summary>
        /// Event triggered if action with ad caused the application to background.
        /// </summary>
        public event Action<InterstitialAd> OnLeftApplication
        {
            add { Ads.OnLeftApplication += value; }
            remove { Ads.OnLeftApplication -= value; }
        }

        /// <summary>
        /// Event triggered after an InterstitialAd was clicked.
        /// </summary>
        public event Action<InterstitialAd> OnClicked
        {
            add { Ads.OnClicked += value; }
            remove { Ads.OnClicked -= value; }
        }

        /// <summary>
        /// Event triggered after V4VC ad has been completed.
        /// Client-side reward implementations should consider incrementing the user's currency balance in this method.
        /// Server-side reward implementations should consider the success parameter and then contact the game server to determine the current total balance for the virtual currency.
        /// Parameter 1: zone ID
        /// Parameter 2: success
        /// Parameter 3: name of reward type
        /// Parameter 4: reward quantity
        /// </summary>
        public event Action<string, bool, string, int> OnRewardGranted
        {
            add { Ads.OnRewardGranted += value; }
            remove { Ads.OnRewardGranted -= value; }
        }

        /// <summary>
        /// Event triggered after an InterstitialAd's audio has started.
        /// </summary>
        public event Action<InterstitialAd> OnAudioStarted
        {
            add { Ads.OnAudioStarted += value; }
            remove { Ads.OnAudioStarted -= value; }
        }

        /// <summary>
        /// Event triggered after an InterstitialAd's audio has stopped.
        /// </summary>
        public event Action<InterstitialAd> OnAudioStopped
        {
            add { Ads.OnAudioStopped += value; }
            remove { Ads.OnAudioStopped -= value; }
        }

        /// <summary>
        /// Event triggered after a custom message is received by the SDK.
        /// Parameter 1: type
        /// Parameter 2: message
        /// </summary>
        public event Action<string, string> OnCustomMessageReceived
        {
            add { Ads.OnCustomMessageReceived += value; }
            remove { Ads.OnCustomMessageReceived -= value; }
        }

        /// <summary>
        /// Event triggered after the ad triggers an IAP opportunity.
        /// Parameter 1: ad
        /// Parameter 2: IAP product ID
        /// Parameter 3: engagement type
        /// </summary>
        public event Action<InterstitialAd, string, AdsIAPEngagementType> OnIAPOpportunity
        {
            add { Ads.OnIAPOpportunity += value; }
            remove { Ads.OnIAPOpportunity -= value; }
        }

        /// <summary>
        /// Event triggered after a call to RequestInterstitialAd has failed.
        /// </summary>
        /// <remarks>
        /// DEPRECATED: use OnRequestInterstitialFailedWithZone instead
        /// </remarks>
        public event Action OnRequestInterstitialFailed
        {
            add { Ads.OnRequestInterstitialFailed += value; }
            remove { Ads.OnRequestInterstitialFailed -= value; }
        }

        #endif

        #endregion  // AdColony Events

        private const string NO_SDK_MESSAGE = "SDK missing. Please import the AdColony plugin.";
        private const string BANNER_UNSUPPORTED_MESSAGE = "AdColony does not support banner ad format.";

        #if EM_ADCOLONY

        private AdColonySettings mAdSettings = null;
        private bool mSubscribedEvents = false;
        private bool mIsRewardedAdCompleted = false;
        private InterstitialAd mDefaultInterstitialAd = null;
        private InterstitialAd mDefaultRewardedAd = null;
        private AdColonyAdView mDefaultBannerAd = null;
        private Dictionary<AdPlacement, InterstitialAd> mCustomInterstitialAds = null;
        private Dictionary<AdPlacement, InterstitialAd> mCustomRewardedAds = null;
        private Dictionary<AdPlacement, AdColonyAdView> mCustomBannerAds = null;
        private bool showDefaultBannerAd = false;
        private Dictionary<AdPlacement, bool> showCustomBannerAds = null;

        #endif

        #region Singleton

        private static AdColonyClientImpl sInstance;

        private AdColonyClientImpl()
        {
        }

        /// <summary>
        /// Returns the singleton client.
        /// </summary>
        /// <returns>The client.</returns>
        public static AdColonyClientImpl CreateClient()
        {
            if (sInstance == null)
            {
                sInstance = new AdColonyClientImpl();
            }
            return sInstance;
        }

        #endregion  // Object Creators

        #region IAdClient Overrides

        public override AdNetwork Network { get { return AdNetwork.AdColony; } }

        public override bool IsBannerAdSupported { get { return true; } }

        public override bool IsInterstitialAdSupported { get { return true; } }

        public override bool IsRewardedAdSupported { get { return true; } }

        public override bool IsSdkAvail
        { 
            get
            {
                #if EM_ADCOLONY 
                return true;
                #else
				return false;
                #endif 
            } 
        }

        public override bool IsValidPlacement(AdPlacement placement, AdType type)
        {
#if EM_ADCOLONY
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

        protected override Dictionary<AdPlacement, AdId> CustomInterstitialAdsDict
        {
            get
            {
                #if EM_ADCOLONY
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
                #if EM_ADCOLONY
                return mAdSettings == null ? null : mAdSettings.CustomRewardedAdIds;
                #else
                return null;
                #endif
            }
        }

        protected override string NoSdkMessage
        { 
            get { return NO_SDK_MESSAGE; }
        }

        protected override void InternalInit()
        {
            #if EM_ADCOLONY

            // Store a reference to the global settings.
            mAdSettings = EM_Settings.Advertising.AdColony;

            if (mCustomInterstitialAds == null)
                mCustomInterstitialAds = new Dictionary<AdPlacement, InterstitialAd>();

            if (mCustomRewardedAds == null)
                mCustomRewardedAds = new Dictionary<AdPlacement, InterstitialAd>();

            if (mCustomBannerAds == null)
                mCustomBannerAds = new Dictionary<AdPlacement, AdColonyAdView>();

            if (showCustomBannerAds == null)
                showCustomBannerAds = new Dictionary<AdPlacement, bool>();
            // Subscribe ad events.
            if (!mSubscribedEvents)
            {
                mSubscribedEvents = true;
                Ads.OnConfigurationCompleted += OnConfigurationCompletedHandle;
                Ads.OnRequestInterstitial += OnRequestAdCompleted;
                Ads.OnAdViewLoaded += OnAdViewLoaded;
                Ads.OnRequestInterstitialFailedWithZone += OnRequestAdFailedWithZone;
                Ads.OnOpened += OnAdOpened;
                Ads.OnClosed += OnAdClosed;
                Ads.OnExpiring += OnAdExpiring;
                Ads.OnLeftApplication += OnLeftApplicationHandle;
                Ads.OnClicked += OnAdClicked;
                Ads.OnRewardGranted += OnRewardGrantedHandle;
                Ads.OnAudioStarted += OnAudioStartedHandle;
                Ads.OnAudioStopped += OnAudioStoppedHandle;
                Ads.OnCustomMessageReceived += OnCustomMessageReceivedHandle;
                Ads.OnIAPOpportunity += OnIAPOpportunityHandle;
                Ads.OnRequestInterstitialFailed += OnRequestInterstitialFailedHandle;
            }

            // Start configuring.
            Configure();

            #endif
        }

        protected override void InternalShowBannerAd(AdPlacement placement, BannerAdPosition position, BannerAdSize size)
        {
            #if EM_ADCOLONY
            AdColonyAdView adview = null;

            if (placement == AdPlacement.Default)
            {
                adview = mDefaultBannerAd;
                showDefaultBannerAd = true;
            }
            else
            {
                mCustomBannerAds.TryGetValue(placement, out adview);
                showCustomBannerAds[placement] = true;
            }

            if (adview != null)
                adview.ShowAdView();
            else
                LoadBannerAd(placement, position, size);
            #endif
        }

        private void LoadBannerAd(AdPlacement placement, BannerAdPosition position, BannerAdSize size)
        {
            #if EM_ADCOLONY
            string id = placement == AdPlacement.Default ?
                mAdSettings.DefaultBannerAdId.Id : FindIdForPlacement(mAdSettings.CustomBannerAdIds, placement);

            AdColony.AdPosition bannerPosition = AdPosition.Bottom;
            switch (position)
            {
                case BannerAdPosition.Top:
                    bannerPosition = AdPosition.Top;
                    break;
                case BannerAdPosition.Bottom:
                    bannerPosition = AdPosition.Bottom;
                    break;
                case BannerAdPosition.TopLeft:
                    bannerPosition = AdPosition.TopLeft;
                    break;
                case BannerAdPosition.TopRight:
                    bannerPosition = AdPosition.TopRight;
                    break;
                case BannerAdPosition.BottomLeft:
                    bannerPosition = AdPosition.BottomLeft;
                    break;
                case BannerAdPosition.BottomRight:
                    bannerPosition = AdPosition.BottomRight;
                    break;
                default:
                    break;
            }
            var adOptions = new AdOptions()
            {
                ShowPrePopup = mAdSettings.EnableRewardedAdPrePopup,
                ShowPostPopup = mAdSettings.EnableRewardedAdPostPopup
            };

            if (!string.IsNullOrEmpty(id))
                AdColony.Ads.RequestAdView(id, AdColony.AdSize.Banner, bannerPosition, adOptions);
            else
                Debug.Log("Attempting to load AdColony banner ad with an undefined ID at placement " + AdPlacement.GetPrintableName(placement));
            #endif
        }

        protected override void InternalHideBannerAd(AdPlacement placement)
        {
            #if EM_ADCOLONY
            AdColonyAdView adview = null;

            if (placement == AdPlacement.Default)
            {
                adview = mDefaultBannerAd;
                showDefaultBannerAd = false;
            }
            else
            {
                mCustomBannerAds.TryGetValue(placement, out adview);
                showCustomBannerAds[placement] = false;
            }

            if (adview != null)
                adview.HideAdView();
            #endif
        }

        protected override void InternalDestroyBannerAd(AdPlacement placement)
        {
            #if EM_ADCOLONY
            AdColonyAdView adview = null;

            if (placement == AdPlacement.Default)
                adview = mDefaultBannerAd;
            else
                mCustomBannerAds.TryGetValue(placement, out adview);


            if (adview != null)
                adview.DestroyAdView();

            if (placement == AdPlacement.Default)
            {
                mDefaultBannerAd = null;
                showDefaultBannerAd = false;
            }
            else
            {
                mCustomBannerAds[placement] = null;
                showCustomBannerAds[placement] = false;
            }

            #endif
        }

        protected override void InternalLoadInterstitialAd(AdPlacement placement)
        {
            #if EM_ADCOLONY
            string id = placement == AdPlacement.Default ? 
                mAdSettings.DefaultInterstitialAdId.Id : FindIdForPlacement(mAdSettings.CustomInterstitialAdIds, placement);

            if (!string.IsNullOrEmpty(id))
                Ads.RequestInterstitialAd(id, null);
            else
                Debug.Log("Attempting to load AdColony interstitial ad with an undefined ID at placement " + AdPlacement.GetPrintableName(placement));
            #endif
        }

        protected override bool InternalIsInterstitialAdReady(AdPlacement placement)
        {
            #if EM_ADCOLONY
            if (placement == AdPlacement.Default)
            {
                return mDefaultInterstitialAd != null && !mDefaultInterstitialAd.Expired;
            }
            else
            {
                var ad = GetCustomInterstitialAd(placement);
                return ad != null && !ad.Expired;
            }
            #else
            return false;
            #endif
        }

        protected override void InternalShowInterstitialAd(AdPlacement placement)
        {
            #if EM_ADCOLONY
            if (placement == AdPlacement.Default)
                Ads.ShowAd(mDefaultInterstitialAd);
            else
                Ads.ShowAd(GetCustomInterstitialAd(placement));
            #endif
        }

        protected override void InternalLoadRewardedAd(AdPlacement placement)
        {
            #if EM_ADCOLONY
            var adOptions = new AdOptions()
            { 
                ShowPrePopup = mAdSettings.EnableRewardedAdPrePopup,
                ShowPostPopup = mAdSettings.EnableRewardedAdPostPopup
            };

            string id = placement == AdPlacement.Default ? 
                mAdSettings.DefaultRewardedAdId.Id : FindIdForPlacement(mAdSettings.CustomRewardedAdIds, placement);

            if (!string.IsNullOrEmpty(id))
                Ads.RequestInterstitialAd(id, adOptions);
            else
                Debug.Log("Attempting to load AdColony rewarded ad with an undefined ID at placement " + AdPlacement.GetPrintableName(placement));
            #endif
        }

        protected override bool InternalIsRewardedAdReady(AdPlacement placement)
        {
            #if EM_ADCOLONY
            if (placement == AdPlacement.Default)
            {
                return mDefaultRewardedAd != null && !mDefaultRewardedAd.Expired;
            }
            else
            {
                var ad = GetCustomRewardedAd(placement);
                return ad != null && !ad.Expired;
            }
            #else
            return false;
            #endif
        }

        protected override void InternalShowRewardedAd(AdPlacement placement)
        {
            #if EM_ADCOLONY
            if (placement == AdPlacement.Default)
                Ads.ShowAd(mDefaultRewardedAd);
            else
                Ads.ShowAd(GetCustomRewardedAd(placement));
            #endif
        }

        #endregion

        #region IConsentRequirable Overrides

        private const string DATA_PRIVACY_CONSENT_KEY = "EM_Ads_AdColony_DataPrivacyConsent";

        protected override string DataPrivacyConsentSaveKey { get { return DATA_PRIVACY_CONSENT_KEY; } }

        protected override void ApplyDataPrivacyConsent(ConsentStatus consent)
        {
            #if EM_ADCOLONY
            // If the client has been initialized, reconfigure it with new consent.
            // Otherwise, the consent will take effect during the first initialization.
            if (mIsInitialized)
                Configure();
            #endif
        }

        #endregion

        #region Internal Stuff

        #if EM_ADCOLONY
        
        private void Configure()
        {
            if (mAdSettings == null)
            {
                Debug.LogWarning("Configuring AdColony with null settings.");
                return;
            }

            var appOptions = new AppOptions()
            {
                AdOrientation = ToAdColonyAdOrientation(mAdSettings.Orientation)
            };

            // Set GDPR consent (if any).
            // If there's no explicit consent given, this step is bypassed
            // which means the "pre-GDPR" configuring will be done.
            var consent = GetApplicableDataPrivacyConsent();
            if (consent != ConsentStatus.Unknown)
            {
                appOptions.GdprRequired = true;
                appOptions.GdprConsentString = ToAdColonyConsentString(consent);
            }

            // Store all defined zone ids into a list.
            List<string> zoneIds = new List<string>
            {
                mAdSettings.DefaultInterstitialAdId.Id,
                mAdSettings.DefaultRewardedAdId.Id,
                mAdSettings.DefaultBannerAdId.Id
            };
            AddCustomZoneIDs(zoneIds, mAdSettings.CustomInterstitialAdIds);
            AddCustomZoneIDs(zoneIds, mAdSettings.CustomRewardedAdIds);
            AddCustomZoneIDs(zoneIds, mAdSettings.CustomBannerAdIds);
            Ads.Configure(
                mAdSettings.AppId.Id,
                appOptions,
                zoneIds.ToArray()
            );
        }

        private string GetCustomInterstitialAdId(AdPlacement placement)
        {
            AdId idObj = null;
            if (placement != null && mAdSettings != null && mAdSettings.CustomInterstitialAdIds != null)
            {
                mAdSettings.CustomInterstitialAdIds.TryGetValue(placement, out idObj);
            }

            if (idObj != null && !string.IsNullOrEmpty(idObj.Id))
            {
                return idObj.Id;
            }
            else
            {
                Debug.Log("Could not find AdColony interstitial ad ID for placement " + placement.Name);
                return string.Empty;
            }
        }

        private InterstitialAd GetCustomInterstitialAd(AdPlacement placement)
        {
            InterstitialAd ad = null;
            if (placement != null && mCustomInterstitialAds != null)
            {
                mCustomInterstitialAds.TryGetValue(placement, out ad);
            }
            return ad;
        }

        private string GetCustomRewardedAdId(AdPlacement placement)
        {
            AdId idObj = null;
            if (placement != null && mAdSettings != null && mAdSettings.CustomRewardedAdIds != null)
            {
                mAdSettings.CustomRewardedAdIds.TryGetValue(placement, out idObj);
            }

            if (idObj != null && !string.IsNullOrEmpty(idObj.Id))
            {
                return idObj.Id;
            }
            else
            {
                Debug.Log("Could not find AdColony rewarded ad ID for placement " + placement.Name);
                return string.Empty;
            }
        }

        private InterstitialAd GetCustomRewardedAd(AdPlacement placement)
        {
            InterstitialAd ad = null;
            if (placement != null && mCustomRewardedAds != null)
            {
                mCustomRewardedAds.TryGetValue(placement, out ad);
            }
            return ad;
        }

        private string ToAdColonyConsentString(ConsentStatus consent)
        {
            return consent == ConsentStatus.Unknown ? string.Empty : 
                consent == ConsentStatus.Granted ? "1" : "0";
        }

        private static AdOrientationType ToAdColonyAdOrientation(AdOrientation orientation)
        {
            switch (orientation)
            {
                case AdOrientation.AdOrientationLandscape:
                    return AdOrientationType.AdColonyOrientationLandscape;
                case AdOrientation.AdOrientationPortrait:
                    return AdOrientationType.AdColonyOrientationPortrait;
                case AdOrientation.AdOrientationAll:
                    return AdOrientationType.AdColonyOrientationAll;
                default:
                    return AdOrientationType.AdColonyOrientationAll;
            }
        }

        private void AddCustomZoneIDs(List<string> result, Dictionary<AdPlacement, AdId> source)
        {
            if (source == null)
                return;

            foreach (var id in source.Values)
            {
                if (result.Contains(id.Id))
                {
                    continue;
                }

                result.Add(id.Id);
            }
        }

        /// <summary>
        /// Save a requested ad to use later.
        /// </summary>
        /// Called in OnRequestAdCompleted event handlers.
        /// <param name="ad">Requested ad.</param>
        private void SaveRequestedAd(InterstitialAd ad)
        {
            /// Check if the requested ad is default ad.
            if (IsDefaultInterstitialAd(ad.ZoneId))
            {
                mDefaultInterstitialAd = ad;
            }
            else if (IsDefaultRewardedAd(ad.ZoneId))
            {
                mDefaultRewardedAd = ad;
            }
            /// Check if the requested ad is custom interstitial ad or custom rewarded ad.
            else
            {
                // Is custom interstitial ad?
                AdPlacement interstitialPlm = FindPlacementOfCustomInterstitialAd(ad.ZoneId);

                if (interstitialPlm != null)
                {
                    // Save the loaded custom interstitial ad so we can use it later, override existing ad if any.
                    mCustomInterstitialAds[interstitialPlm] = ad;
                    return;
                }

                // Not interstitial, is custom rewarded ad?
                AdPlacement rewardedPlm = FindPlacementOfCustomRewardedAd(ad.ZoneId);

                if (rewardedPlm != null)
                {
                    // Save the loaded rewarded ad so we can use it later, override existing ad if any.
                    mCustomRewardedAds[rewardedPlm] = ad;
                    return;
                }
            }
        }

        /// <summary>
        /// Save a requested ad to use later.
        /// </summary>
        /// Called in OnAdViewLoaded event handlers.
        /// <param name="adView">Requested ad.</param>
        private void SaveRequestedAdView(AdColonyAdView adView)
        {
            // Check if the requested ad is default ad
            if(IsDefaultBannerAd(adView.ZoneId))
            {
                mDefaultBannerAd = adView;
                if (showDefaultBannerAd)
                    mDefaultBannerAd.ShowAdView();
                return;
            }
            var bannerPlm = FindPlacementOfCustomBannerAd(adView.ZoneId);
            if(bannerPlm != null)
            {
                mCustomBannerAds[bannerPlm] = adView;
                bool allowToShow = false;
                showCustomBannerAds.TryGetValue(bannerPlm.ToAdPlacement(), out allowToShow);
                if (allowToShow)
                    mCustomBannerAds[bannerPlm].ShowAdView();
            }
        }

        /// <summary>
        /// When an ad is closed. Check the ad's info and raise an event if possible.
        /// </summary>
        /// Called in OnAdClosed event handler.
        /// <param name="ad">Closed ad.</param>
        private void InvokeAdEvents(InterstitialAd ad)
        {
            /// Default interstitial ad?
            if (IsDefaultInterstitialAd(ad.ZoneId))
            {
                OnInterstitialAdCompleted(AdPlacement.Default);
                return;
            }

            /// Custom interstitial ad?
            AdPlacement customInterPlm = FindPlacementOfCustomInterstitialAd(ad.ZoneId);

            if (customInterPlm != null)
            {
                OnInterstitialAdCompleted(customInterPlm);
                return;
            }

            /// Default rewarded video ad?
            if (IsDefaultRewardedAd(ad.ZoneId) && mIsRewardedAdCompleted)
            {
                mIsRewardedAdCompleted = false;
                OnRewardedAdCompleted(AdPlacement.Default);
                return;
            }

            /// Custom rewarded video ad?
            AdPlacement customRewardedPlm = FindPlacementOfCustomRewardedAd(ad.ZoneId);

            if (customRewardedPlm != null && mIsRewardedAdCompleted)
            {
                mIsRewardedAdCompleted = false;
                OnRewardedAdCompleted(customRewardedPlm);
                return;
            }
        }

        /// <summary>
        /// Check if an ad is defined in the setttings as default interstitial ad.
        /// </summary>
        private bool IsDefaultInterstitialAd(string zoneId)
        {
            if (mAdSettings == null)
                return false;
            
            if (zoneId == null)
                return false;

            return zoneId.Equals(mAdSettings.DefaultInterstitialAdId.Id);
        }

        /// <summary>
        /// Check if an ad is defined in the settings as default rewarded video ad.
        /// </summary>
        private bool IsDefaultRewardedAd(string zoneId)
        {
            if (mAdSettings == null)
                return false;
            
            if (zoneId == null)
                return false;

            return zoneId.Equals(mAdSettings.DefaultRewardedAdId.Id);
        }

        /// <summary>
        /// Check if an ad is defined in the settings as default banner ad.
        /// </summary>
        private bool IsDefaultBannerAd(string zoneId)
        {
            if (mAdSettings == null)
                return false;

            if (zoneId == null)
                return false;

            return zoneId.Equals(mAdSettings.DefaultBannerAdId.Id);
        }

        /// <summary>
        /// Finds the placement associated with the custom interstitial ad with the specified ID.
        /// Returns null if no such placement found.
        /// </summary>
        /// <returns>The placement of custom interstitial ad.</returns>
        /// <param name="zoneId">Zone identifier.</param>
        private AdPlacement FindPlacementOfCustomInterstitialAd(string zoneId)
        {
            if (mAdSettings == null || mAdSettings.CustomInterstitialAdIds == null)
                return null;
        
            return mAdSettings.CustomInterstitialAdIds.FirstOrDefault(kvp => kvp.Value.Id.Equals(zoneId)).Key;
        }

        /// <summary>
        /// Finds the placement associated with the custom rewarded ad with the specified ID.
        /// Returns null if no such placement found.
        /// </summary>
        /// <returns>The placement of custom rewarded ad.</returns>
        /// <param name="zoneId">Zone identifier.</param>
        private AdPlacement FindPlacementOfCustomRewardedAd(string zoneId)
        {
            if (mAdSettings == null || mAdSettings.CustomRewardedAdIds == null)
                return null;

            return mAdSettings.CustomRewardedAdIds.FirstOrDefault(kvp => kvp.Value.Id.Equals(zoneId)).Key;
        }

        /// <summary>
        /// Finds the placement associated with the custom banner ad with the specified ID.
        /// Returns null if no such placement found.
        /// </summary>
        /// <returns>The placement of custom banner ad.</returns>
        /// <param name="zoneId">Zone identifier.</param>
        private AdPlacement FindPlacementOfCustomBannerAd(string zoneId)
        {
            if (mAdSettings == null || mAdSettings.CustomBannerAdIds == null)
                return null;

            return mAdSettings.CustomBannerAdIds.FirstOrDefault(kvp => kvp.Value.Id.Equals(zoneId)).Key;
        }

#endif

        #endregion // Private Methods

        #region Ad Event Handlers

#if EM_ADCOLONY

        private void OnConfigurationCompletedHandle(List<Zone> obj)
        {
            // Done initializing.
            mIsInitialized = true;
            Debug.Log("AdColony client has been initialized.");
        }

        private void OnRequestAdFailedWithZone(string zoneId)
        {
            Debug.Log("AdColony request ad failed with zoneId: " + zoneId);
        }

        private void OnRequestAdCompleted(InterstitialAd ad)
        {
            Debug.Log("AdColony successfully loaded ad with zoneId: " + ad.ZoneId);
            SaveRequestedAd(ad);
        }

        private void OnAdViewLoaded(AdColonyAdView adView)
        {
            Debug.Log("AdColony succesfully loaded banner ad with zoneId: " + adView.ZoneId);
            SaveRequestedAdView(adView);
        }

        private void OnAdOpened(InterstitialAd ad)
        {
        }

        private void OnAdClosed(InterstitialAd ad)
        {
            InvokeAdEvents(ad);
        }

        private void OnAdExpiring(InterstitialAd ad)
        {
        }

        private void OnLeftApplicationHandle(InterstitialAd ad)
        {
        }

        private void OnAdClicked(InterstitialAd ad)
        {
        }

        private void OnRewardGrantedHandle(string zoneId, bool success, string rewardName, int rewardQuantity)
        {
            mIsRewardedAdCompleted = success;
        }

        private void OnAudioStartedHandle(InterstitialAd obj)
        {
        }

        private void OnAudioStoppedHandle(InterstitialAd obj)
        {

        }

        private void OnCustomMessageReceivedHandle(string arg1, string arg2)
        {
        }

        private void OnIAPOpportunityHandle(InterstitialAd arg1, string arg2, AdsIAPEngagementType arg3)
        {
        }

        private void OnRequestInterstitialFailedHandle()
        {
        }

#endif

        #endregion  // Ad Event Handlers

    }
}
  