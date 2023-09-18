using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

namespace EasyMobile
{
#if EM_ADMOB
    using GoogleMobileAds;
    using GoogleMobileAds.Api;
    using EasyMobile.Internal;
    using System.Text;
#endif

    public class AdMobClientImpl : AdClientImpl
    {
        private const string NO_SDK_MESSAGE = "SDK missing. Please import the AdMob (Google Mobile Ads) plugin.";

#if EM_ADMOB

        /// <summary>
        /// Used to specify that only non-personalized ads should be requested when creating ad request in
        /// <see cref="CreateAdMobAdRequest"/>
        /// </summary>
        private KeyValuePair<string, string> mNonPersonalizedPair = new KeyValuePair<string, string>("npa", "1");

        private AdMobSettings mAdSettings = null;
        private BannerView mDefaultBanner = null;
        private BannerAdSize mCurrentDefaultBannerSize = new BannerAdSize(-1, -1);
        private InterstitialAd mDefaultInterstitialAd = null;
        private RewardedAd mDefaultRewardedAd = null;
        private ConsentStatus mCurrentConsent = ConsentStatus.Unknown;
        private string testDeviceID = null;

        /// <summary>
        /// We will store all the banner ads loaded with custom key here.
        /// </summary>
        private Dictionary<AdPlacement, KeyValuePair<BannerAdSize, BannerView>> mCustomBannerAds;

        /// <summary>
        /// We will store all the interstitial ads loaded with custom key here.
        /// </summary>
        private Dictionary<AdPlacement, InterstitialAd> mCustomInterstitialAds;

        /// <summary>
        /// We will store all the rewarded ads loaded with custom key here.
        /// </summary>
        private Dictionary<AdPlacement, RewardedAd> mCustomRewardedAd;

        /// <summary>
        /// Check if there is any rewarded video is currently running.
        /// </summary>
        /// Note that we can't have more than 1 rewarded video ad loaded at the same time,
        /// since it's gonna override old one's events.
        private bool mIsRewardedAdPlaying = false;

        /// <summary>
        /// Check if a rewarded ad is completed.
        /// We need this value to check if a rewarded ad was skipped or completed when it is being closed.
        /// </summary>
        private bool mIsRewardedAdCompleted = false;

#endif

        #region AdMob Events

#if EM_ADMOB

        /// <summary>
        /// Called when a banner ad request has successfully loaded.
        /// </summary>
        public event EventHandler<EventArgs> OnBannerAdLoaded;

        /// <summary>
        /// Called when a banner ad request failed to load.
        /// </summary>
        public event EventHandler<AdFailedToLoadEventArgs> OnBannerAdFailedToLoad;

        /// <summary>
        /// Called when a banner ad is clicked.
        /// </summary>
        public event EventHandler<EventArgs> OnBannerAdOpening;

        /// <summary>
        /// Called when the user returned from the app after a banner ad click.
        /// </summary>
        public event EventHandler<EventArgs> OnBannerAdClosed;

        /// <summary>
        /// Called when a banner ad click caused the user to leave the application.
        /// </summary>
        public event EventHandler<EventArgs> OnBannerAdLeavingApplication;

        /// <summary>
        /// Called when an interstitial ad request has successfully loaded.
        /// </summary>
        public event EventHandler<EventArgs> OnInterstitialAdLoaded;

        /// <summary>
        /// Called when an interstitial ad request failed to load.
        /// </summary>
        public event EventHandler<AdFailedToLoadEventArgs> OnInterstitialAdFailedToLoad;

        /// <summary>
        /// Called when an interstitial ad is shown.
        /// </summary>
        public event EventHandler<EventArgs> OnInterstititalAdOpening;

        /// <summary>
        /// Called when an interstitital ad is closed.
        /// </summary>
        public event EventHandler<EventArgs> OnInterstitialAdClosed;

        /// <summary>
        /// Called when an interstitial ad click caused the user to leave the application.
        /// </summary>
        public event EventHandler<EventArgs> OnInterstitialAdLeavingApplication;

        /// <summary>
        /// Called when a rewarded video ad request has successfully loaded.
        /// </summary>
        public event EventHandler<EventArgs> OnRewardedAdLoaded;

        /// <summary>
        /// Called when a rewarded video ad request failed to load.
        /// </summary>
        public event EventHandler<AdErrorEventArgs> OnRewardedAdFailedToLoad;

        /// <summary>
        /// Called when a rewarded video ad request failed to show.
        /// </summary>
        public event EventHandler<AdErrorEventArgs> OnRewardedAdFailedToShow;

        /// <summary>
        /// Called when a rewared video ad is shown.
        /// </summary>
        public event EventHandler<EventArgs> OnRewardedAdOpening;

        /// <summary>
        /// Called when a rewarded video ad starts to play.
        /// </summary>
        public event EventHandler<EventArgs> OnRewardedAdStarted;

        /// <summary>
        /// Called when the user should be rewarded for watching a video.
        /// </summary>
        public event EventHandler<Reward> OnRewardedAdRewarded;

        /// <summary>
        /// Called when a rewarded video ad is closed.
        /// </summary>
        public event EventHandler<EventArgs> OnRewardedAdClosed;

        /// <summary>
        /// Called when a rewarded video ad click caused the user to leave the application.
        /// </summary>
        public event EventHandler<EventArgs> OnRewardedAdLeavingApplication;

#endif

        #endregion  // AdMob Events

        #region Singleton

        private static AdMobClientImpl sInstance;

        private AdMobClientImpl()
        {
        }

        /// <summary>
        /// Returns the singleton client.
        /// </summary>
        /// <returns>The client.</returns>
        public static AdMobClientImpl CreateClient()
        {
            if (sInstance == null)
            {
                sInstance = new AdMobClientImpl();
            }
            return sInstance;
        }

        #endregion  // Singleton

        #region AdClient Overrides

        public override AdNetwork Network { get { return AdNetwork.AdMob; } }

        public override bool IsBannerAdSupported { get { return true; } }

        public override bool IsInterstitialAdSupported { get { return true; } }

        public override bool IsRewardedAdSupported { get { return true; } }

        public override bool IsSdkAvail
        {
            get
            {
#if EM_ADMOB
                return true;
#else
                return false;
#endif
            }
        }

        public override bool IsValidPlacement(AdPlacement placement, AdType type)
        {
#if EM_ADMOB
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
#if EM_ADMOB
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
#if EM_ADMOB
                return mAdSettings == null ? null : mAdSettings.CustomRewardedAdIds;
#else
                return null;
#endif
            }
        }

        protected override string NoSdkMessage { get { return NO_SDK_MESSAGE; } }

        protected override void InternalInit()
        {
#if EM_ADMOB
            mAdSettings = EM_Settings.Advertising.AdMob;

            // Set GDPR consent if any.
            var consent = GetApplicableDataPrivacyConsent();
            ApplyDataPrivacyConsent(consent);

            // Init.
            MobileAds.Initialize(initStatus =>
            {
                RuntimeHelper.RunOnMainThread(() =>
                {
                    mCustomBannerAds = new Dictionary<AdPlacement, KeyValuePair<BannerAdSize, BannerView>>();
                    mCustomInterstitialAds = new Dictionary<AdPlacement, InterstitialAd>();
                    mCustomRewardedAd = new Dictionary<AdPlacement, RewardedAd>();
                    if (mAdSettings.EnableTestMode)
                    {
#if UNITY_ANDROID
                        testDeviceID = SystemInfo.deviceUniqueIdentifier.ToUpper();
#elif UNITY_IOS
                        testDeviceID = UnityEngine.iOS.Device.advertisingIdentifier;
                        testDeviceID = CreateMD5(testDeviceID);
                        testDeviceID = testDeviceID.ToLower();
#endif
                    }

                    mIsInitialized = true;
                    Debug.Log("AdMob client has been initialized.");
                });
            });
#endif
        }

        //------------------------------------------------------------
        // Banner Ads.
        //------------------------------------------------------------

        protected override void InternalShowBannerAd(AdPlacement placement, BannerAdPosition position, BannerAdSize size)
        {
#if EM_ADMOB
            string id = placement == AdPlacement.Default ?
                mAdSettings.DefaultBannerAdId.Id :
                FindIdForPlacement(mAdSettings.CustomBannerAdIds, placement);

            if (string.IsNullOrEmpty(id))
            {
                Debug.Log("Attempting to show AdMob banner ad with an undefined ID at placement " + AdPlacement.GetPrintableName(placement));
                return;
            }

            // If the requested banner (default or custom) doesn't exist or player request a banner with different size, create a new one and show it.
            // Otherwise just show the existing banner (which might be hidden before).

            if (placement == AdPlacement.Default) // Default banner...
            {
                if (mDefaultBanner == null || mCurrentDefaultBannerSize != size)
                {
                    mDefaultBanner = CreateNewBanner(position, size, id);
                    mCurrentDefaultBannerSize = size;
                    Debug.Log("Creating new default banner...");
                }

                mDefaultBanner.SetPosition(ToAdMobAdPosition(position));
                mDefaultBanner.Show();
            }
            else // Custom banner...
            {
                if (!mCustomBannerAds.ContainsKey(placement) || mCustomBannerAds[placement].Value == null || mCustomBannerAds[placement].Key != size)
                {
                    mCustomBannerAds[placement] = new KeyValuePair<BannerAdSize, BannerView>(size, CreateNewBanner(position, size, id));
                    Debug.Log("Creating new custom banner...");
                }

                mCustomBannerAds[placement].Value.SetPosition(ToAdMobAdPosition(position));
                mCustomBannerAds[placement].Value.Show();
            }
#endif
        }

        protected override void InternalHideBannerAd(AdPlacement placement)
        {
#if EM_ADMOB
            if (placement == AdPlacement.Default) // Default banner...
            {
                if (mDefaultBanner != null)
                    mDefaultBanner.Hide();
            }
            else // Custom banner...
            {
                if (mCustomBannerAds == null)
                    return;

                if (!mCustomBannerAds.ContainsKey(placement) || mCustomBannerAds[placement].Value == null)
                    return;

                mCustomBannerAds[placement].Value.Hide();
            }
#endif
        }

        protected override void InternalDestroyBannerAd(AdPlacement placement)
        {
#if EM_ADMOB
            if (placement == AdPlacement.Default) // Default banner...
            {
                if (mDefaultBanner != null)
                {
                    mDefaultBanner.Destroy();
                    mDefaultBanner = null;
                }
            }
            else // Custom banner...
            {
                if (mCustomBannerAds == null)
                    return;

                if (!mCustomBannerAds.ContainsKey(placement) || mCustomBannerAds[placement].Value == null)
                    return;

                mCustomBannerAds[placement].Value.Destroy();
                mCustomBannerAds.Remove(placement);
            }
#endif
        }

        //------------------------------------------------------------
        // Interstitial Ads.
        //------------------------------------------------------------

        protected override void InternalLoadInterstitialAd(AdPlacement placement)
        {
#if EM_ADMOB
            string id = placement == AdPlacement.Default ?
                mAdSettings.DefaultInterstitialAdId.Id :
                FindIdForPlacement(mAdSettings.CustomInterstitialAdIds, placement);

            if (string.IsNullOrEmpty(id))
            {
                Debug.Log("Attempting to load AdMob interstitial ad with an undefined ID at placement " + AdPlacement.GetPrintableName(placement));
                return;
            }

            if (placement == AdPlacement.Default) // Default interstitial...
            {
                // Note: On iOS, InterstitialAd objects are one time use objects. 
                // That means once an interstitial is shown, the InterstitialAd object can't be used to load another ad. 
                // To request another interstitial, you'll need to create a new InterstitialAd object.
                if (mDefaultInterstitialAd == null)
                    mDefaultInterstitialAd = CreateNewInterstitialAd(id, placement);

                mDefaultInterstitialAd.LoadAd(CreateAdMobAdRequest());
            }
            else // Custom interstitial
            {
                /// Create a new custom interstitial ad and load it.
                if (!mCustomInterstitialAds.ContainsKey(placement) || mCustomInterstitialAds[placement] == null)
                    mCustomInterstitialAds[placement] = CreateNewInterstitialAd(id, placement);

                mCustomInterstitialAds[placement].LoadAd(CreateAdMobAdRequest());
            }
#endif
        }

        protected override bool InternalIsInterstitialAdReady(AdPlacement placement)
        {
#if EM_ADMOB
            if (placement == AdPlacement.Default) // Default interstitial ad...
            {
                return mDefaultInterstitialAd != null && mDefaultInterstitialAd.IsLoaded();
            }
            else // Custom interstitial ad...
            {
                return mCustomInterstitialAds.ContainsKey(placement) &&
                mCustomInterstitialAds[placement] != null &&
                mCustomInterstitialAds[placement].IsLoaded();
            }
#else
            return false;
#endif
        }

        protected override void InternalShowInterstitialAd(AdPlacement placement)
        {
#if EM_ADMOB
            if (placement == AdPlacement.Default) // Default interstitial ad...
            {
                if (mDefaultInterstitialAd != null)
                    mDefaultInterstitialAd.Show();
            }
            else // Custom interstitial ad...
            {
                if (mCustomInterstitialAds.ContainsKey(placement) && mCustomInterstitialAds[placement] != null)
                    mCustomInterstitialAds[placement].Show();
            }
#endif
        }

        //------------------------------------------------------------
        // Rewarded Ads.
        //------------------------------------------------------------

        /// <summary>
        /// Instructs the underlaying SDK to load a rewarded ad. Only invoked if the client is initialized.
        /// </summary>
        /// <param name="placement">Placement.</param>
        protected override void InternalLoadRewardedAd(AdPlacement placement)
        {
#if EM_ADMOB
            // Loading a new rewarded ad seems to disable all events of the currently playing ad,
            // so we shouldn't perform rewarded ad loading while playing another one.
            if (mIsRewardedAdPlaying)
                return;

            string id = placement == AdPlacement.Default ?
                mAdSettings.DefaultRewardedAdId.Id :
                FindIdForPlacement(mAdSettings.CustomRewardedAdIds, placement);

            if (string.IsNullOrEmpty(id))
            {
                Debug.Log("Attempting to load AdMob rewarded ad with an undefined ID at placement " + AdPlacement.GetPrintableName(placement));
                return;
            }

            if (placement == AdPlacement.Default) // Default rewarded ad...
            {
                if (mDefaultRewardedAd == null)
                    mDefaultRewardedAd = CreateNewRewardedAd(id, placement);
                mDefaultRewardedAd.LoadAd(CreateAdMobAdRequest());
            }
            else // Custom rewarded ad...
            {
                /// Create a new custom reward ad and load it.
                if (!mCustomRewardedAd.ContainsKey(placement) || mCustomRewardedAd[placement] == null)
                    mCustomRewardedAd[placement] = CreateNewRewardedAd(id, placement);
                mCustomRewardedAd[placement].LoadAd(CreateAdMobAdRequest());
            }
#endif
        }

        protected override bool InternalIsRewardedAdReady(AdPlacement placement)
        {
#if EM_ADMOB
            if (placement == AdPlacement.Default) // Default rewarded ad...
            {
                return
                mDefaultRewardedAd != null &&
                mDefaultRewardedAd.IsLoaded();
            }
            else // Custom rewarded ad...
            {
                return
                mCustomRewardedAd.ContainsKey(placement) &&
                mCustomRewardedAd[placement] != null &&
                mCustomRewardedAd[placement].IsLoaded();
            }
#else
            return false;
#endif
        }

        protected override void InternalShowRewardedAd(AdPlacement placement)
        {
#if EM_ADMOB
            mIsRewardedAdPlaying = true;
            if (placement == AdPlacement.Default) // Default rewarded ad...
            {
                if (mDefaultRewardedAd != null)
                    mDefaultRewardedAd.Show();
            }
            else
            {
                if (mCustomRewardedAd.ContainsKey(placement) && mCustomRewardedAd[placement] != null)
                    mCustomRewardedAd[placement].Show();
            }


#endif
        }

        #endregion  // AdClient Overrides

        #region IConsentRequirable Overrides

        private const string DATA_PRIVACY_CONSENT_KEY = "EM_Ads_AdMob_DataPrivacyConsent";

        protected override string DataPrivacyConsentSaveKey { get { return DATA_PRIVACY_CONSENT_KEY; } }

        protected override void ApplyDataPrivacyConsent(ConsentStatus consent)
        {
#if EM_ADMOB
            mCurrentConsent = consent;
#endif
            // See CreateAdMobAdRequest method for usage...
        }

        #endregion

        #region Private Methods

#if EM_ADMOB

        private static string CreateMD5(string input)
        {
            if (string.IsNullOrEmpty(input)) return string.Empty;

            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }

        private AdSize ToAdMobAdSize(BannerAdSize adSize)
        {
            return adSize.IsSmartBanner ? AdSize.SmartBanner : new AdSize(adSize.Width, adSize.Height);
        }

        private AdPosition ToAdMobAdPosition(BannerAdPosition pos)
        {
            switch (pos)
            {
                case BannerAdPosition.Top:
                    return AdPosition.Top;
                case BannerAdPosition.Bottom:
                    return AdPosition.Bottom;
                case BannerAdPosition.TopLeft:
                    return AdPosition.TopLeft;
                case BannerAdPosition.TopRight:
                    return AdPosition.TopRight;
                case BannerAdPosition.BottomLeft:
                    return AdPosition.BottomLeft;
                case BannerAdPosition.BottomRight:
                    return AdPosition.BottomRight;
                default:
                    return AdPosition.Top;
            }
        }

        private AdRequest CreateAdMobAdRequest()
        {
            AdRequest.Builder adBuilder = new AdRequest.Builder();

            // Targeting settings.
            var targeting = mAdSettings.TargetingSettings;

            // Child-directed.
            if (targeting.TagForChildDirectedTreatment != AdChildDirectedTreatment.Unspecified)
                adBuilder.TagForChildDirectedTreatment(targeting.TagForChildDirectedTreatment == AdChildDirectedTreatment.Yes);

            // Extras.
            if (targeting.ExtraOptions != null)
            {
                foreach (var extra in targeting.ExtraOptions)
                {
                    if (!string.IsNullOrEmpty(extra.Key) && !string.IsNullOrEmpty(extra.Value))
                        adBuilder.AddExtra(extra.Key, extra.Value);
                }
            }

            // Test mode.
            if (mAdSettings.EnableTestMode)
            {
                // Add all emulators
                adBuilder.AddTestDevice(AdRequest.TestDeviceSimulator);

                // Add user-specified test devices
                if (!String.IsNullOrEmpty(testDeviceID))
                    adBuilder.AddTestDevice(Util.AutoTrimId(testDeviceID));
            }

            // Configure the ad request to serve non-personalized ads.
            // The default behavior of the Google Mobile Ads SDK is to serve personalized ads,
            // we only do this if the user has explicitly denied to grant consent.
            // https://developers.google.com/admob/unity/eu-consent
            if (mCurrentConsent == ConsentStatus.Revoked)
                adBuilder.AddExtra(mNonPersonalizedPair.Key, mNonPersonalizedPair.Value);

            return adBuilder.Build();
        }

        /// <summary>
        /// Create new banner, register all the events and load it automatically.
        /// </summary>
        /// <param name="position">The new banner will be placed at this position.</param>
        /// <param name="size">Size of the new banner.</param>
        /// <param name="bannerId">Id to request new banner.</param>
        private BannerView CreateNewBanner(BannerAdPosition position, BannerAdSize size, string bannerId)
        {
            BannerView newBanner = new BannerView(
                                       bannerId,
                                       ToAdMobAdSize(size),
                                       ToAdMobAdPosition(position)
                                   );

            /// Register for banner ad events.
            newBanner.OnAdLoaded += HandleAdMobBannerAdLoaded;
            newBanner.OnAdFailedToLoad += HandleAdMobBannerAdFailedToLoad;
            newBanner.OnAdOpening += HandleAdMobBannerAdOpening;
            newBanner.OnAdClosed += HandleAdMobBannerAdClosed;
            newBanner.OnAdLeavingApplication += HandleAdMobBannerAdLeftApplication;

            newBanner.LoadAd(CreateAdMobAdRequest());

            return newBanner;
        }

        /// <summary>
        /// Create new interstitial ad and register all the events.
        /// </summary>
        /// <param name="interstitialAdId">Id to request new interstitial ad.</param>
        /// <param name="placement">Used when invoking events.</param>
        private InterstitialAd CreateNewInterstitialAd(string interstitialAdId, AdPlacement placement)
        {
            // Create new interstitial object.
            InterstitialAd defaultInterstitialAd = new InterstitialAd(interstitialAdId);

            // Register for interstitial ad events.
            defaultInterstitialAd.OnAdLoaded += HandleAdMobInterstitialLoaded;
            defaultInterstitialAd.OnAdFailedToLoad += HandleAdMobInterstitialFailedToLoad;
            defaultInterstitialAd.OnAdOpening += HandleAdMobInterstitialOpening;
            defaultInterstitialAd.OnAdClosed += (sender, param) => HandleAdMobInterstitialClosed(sender, param, placement);
            defaultInterstitialAd.OnAdLeavingApplication += HandleAdMobInterstitialLeftApplication;

            return defaultInterstitialAd;
        }

        /// <summary>
        /// Create new rewarded video ad and register all the events.
        /// </summary>
        private RewardedAd CreateNewRewardedAd(string interstitialAdId, AdPlacement placement)
        {
            RewardedAd newRewardedAd = new RewardedAd(interstitialAdId);

            // RewardBasedVideoAd is a singleton, so handlers should only be registered once.
            newRewardedAd.OnAdLoaded += HandleAdMobRewardBasedVideoLoaded;
            newRewardedAd.OnAdFailedToLoad += HandleAdMobRewardBasedVideoFailedToLoad;
            newRewardedAd.OnAdFailedToShow += HandleAdMobRewardBasedVideoFailedToShow;
            newRewardedAd.OnAdOpening += HandleAdMobRewardBasedVideoOpening;
            newRewardedAd.OnUserEarnedReward += HandleAdMobRewardBasedVideoRewarded;
            newRewardedAd.OnAdClosed += (sender, param) => HandleAdMobRewardBasedVideoClosed(sender, param, placement);

            return newRewardedAd;
        }

        /// <summary>
        /// Destroy an interstitial ad and invoke the InterstitialAdCompleted event.
        /// </summary>
        /// Called in HandleAdMobInterstitialClosed event handler.
        private void CloseInterstititlaAd(AdPlacement placement)
        {
            if (placement == AdPlacement.Default) // Default interstitial ad...
            {
                // Note: On iOS, InterstitialAd objects are one time use objects. 
                // ==> Destroy the used interstitial ad object; also reset
                // the reference to force new objects to be created when loading ads.
                if (mDefaultInterstitialAd != null)
                {
                    mDefaultInterstitialAd.Destroy();
                    mDefaultInterstitialAd = null;
                }
            }
            else // Custom interstitial ad...
            {
                if (mCustomInterstitialAds != null && mCustomInterstitialAds.ContainsKey(placement) && mCustomInterstitialAds[placement] != null)
                {
                    mCustomInterstitialAds[placement].Destroy();
                    mCustomInterstitialAds[placement] = null;
                }
            }

            OnInterstitialAdCompleted(placement);
        }



        /// <summary>
        /// Get the right action to invoke when a rewarded ad is skipped.
        /// </summary>
        private Action GetRewardedAdSkippedAction(AdPlacement placement)
        {
            return () =>
            {
                OnRewardedAdSkipped(placement);
            };

        }

        /// <summary>
        /// Get the right action to invoke when a rewarded ad is completed.
        /// </summary>
        private Action GetRewardedAdCompletedAction(AdPlacement placement)
        {
            return () =>
                {
                    OnRewardedAdCompleted(placement);
                };
        }

        /// <summary>
        /// Destroy an rewarded ad.
        /// </summary>
        /// Called in RewardedBasedVideoClosedDelayCoroutine event handler.
        private void DestroyRewardedAds(AdPlacement placement)
        {
            if (placement == AdPlacement.Default) // Default rewarded ad...
            {
                // Note: RewardedAd objects are one time use objects. 
                // ==> Destroy the used rewarded ad object; also reset
                // the reference to force new objects to be created when loading ads.
                if (mDefaultRewardedAd != null)
                {
                    mDefaultRewardedAd = null;
                }
            }
            else // Custom rewarded ad...
            {
                if (mCustomRewardedAd != null && mCustomRewardedAd.ContainsKey(placement) && mCustomRewardedAd[placement] != null)
                {
                    mCustomRewardedAd[placement] = null;
                }
            }
        }


#endif


        #endregion // Private Methods

        #region Ad Event Handlers

#if EM_ADMOB

        //------------------------------------------------------------
        // Banner Ads Callbacks.
        //------------------------------------------------------------

        private void HandleAdMobBannerAdLoaded(object sender, EventArgs args)
        {
            Debug.Log("AdMob banner ad has been loaded successfully.");

            if (OnBannerAdLoaded != null)
                OnBannerAdLoaded.Invoke(sender, args);
        }

        private void HandleAdMobBannerAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log("AdMob banner ad failed to load. Error: " + args.Message);

            if (OnBannerAdFailedToLoad != null)
                OnBannerAdFailedToLoad.Invoke(sender, args);
        }

        private void HandleAdMobBannerAdOpening(object sender, EventArgs args)
        {
            if (OnBannerAdOpening != null)
                OnBannerAdOpening.Invoke(sender, args);
        }

        private void HandleAdMobBannerAdClosed(object sender, EventArgs args)
        {
            if (OnBannerAdClosed != null)
                OnBannerAdClosed.Invoke(sender, args);
        }

        private void HandleAdMobBannerAdLeftApplication(object sender, EventArgs args)
        {
            if (OnBannerAdLeavingApplication != null)
                OnBannerAdLeavingApplication.Invoke(sender, args);
        }

        //------------------------------------------------------------
        // Interstitial Ads Callbacks.
        //------------------------------------------------------------

        private void HandleAdMobInterstitialLoaded(object sender, EventArgs args)
        {
            Debug.Log("AdMob interstitial ad has been loaded successfully.");

            if (OnInterstitialAdLoaded != null)
                OnInterstitialAdLoaded.Invoke(sender, args);
        }

        private void HandleAdMobInterstitialFailedToLoad(object sender, AdFailedToLoadEventArgs args)
        {
            Debug.Log("AdMob interstitial ad failed to load. Error: " + args.Message);

            if (OnInterstitialAdFailedToLoad != null)
                OnInterstitialAdFailedToLoad.Invoke(sender, args);
        }

        private void HandleAdMobInterstitialOpening(object sender, EventArgs args)
        {
            if (OnInterstititalAdOpening != null)
                OnInterstititalAdOpening.Invoke(sender, args);
        }

        private void HandleAdMobInterstitialClosed(object sender, EventArgs args, AdPlacement placement)
        {
            CloseInterstititlaAd(placement);

            if (OnInterstitialAdClosed != null)
                OnInterstitialAdClosed.Invoke(sender, args);
        }

        private void HandleAdMobInterstitialLeftApplication(object sender, EventArgs args)
        {
            if (OnInterstitialAdLeavingApplication != null)
                OnInterstitialAdLeavingApplication.Invoke(sender, args);
        }

        //------------------------------------------------------------
        // Rewarded Ads Callbacks.
        //------------------------------------------------------------

        private void HandleAdMobRewardBasedVideoLoaded(object sender, EventArgs args)
        {
            Debug.Log("AdMob rewarded video ad has been loaded successfully.");

            if (OnRewardedAdLoaded != null)
                OnRewardedAdLoaded.Invoke(sender, args);
        }

        private void HandleAdMobRewardBasedVideoFailedToLoad(object sender, AdErrorEventArgs args)
        {
            Debug.Log("AdMob rewarded video ad failed to load. Message: " + args.Message);

            if (OnRewardedAdFailedToLoad != null)
                OnRewardedAdFailedToLoad.Invoke(sender, args);
        }

        private void HandleAdMobRewardBasedVideoFailedToShow(object sender, AdErrorEventArgs args)
        {
            Debug.Log("AdMob rewarded video ad failed to show. Message: " + args.Message);

            if (OnRewardedAdFailedToShow != null)
                OnRewardedAdFailedToShow.Invoke(sender, args);
        }

        private void HandleAdMobRewardBasedVideoOpening(object sender, EventArgs args)
        {
            if (OnRewardedAdOpening != null)
                OnRewardedAdOpening.Invoke(sender, args);
        }

        private void HandleAdMobRewardBasedVideoStarted(object sender, EventArgs args)
        {
            if (OnRewardedAdStarted != null)
                OnRewardedAdStarted.Invoke(sender, args);
        }

        private void HandleAdMobRewardBasedVideoClosed(object sender, EventArgs args, AdPlacement placement)
        {
            // Make sure this method always be called after the OnAdRewarded event.
            RuntimeHelper.RunOnMainThread(() =>
                RuntimeHelper.RunCoroutine(RewardedBasedVideoClosedDelayCoroutine(sender, args, placement)));
        }

        private IEnumerator RewardedBasedVideoClosedDelayCoroutine(object sender, EventArgs args, AdPlacement placement)
        {
            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            // Ad is not playing anymore.
            mIsRewardedAdPlaying = false;

            // If the ad was completed, the "rewarded" event should be fired previously,
            // setting the completed bool to true. Otherwise the ad was skipped.
            // Events are raised on main thread.
            Action callback = mIsRewardedAdCompleted ? GetRewardedAdCompletedAction(placement) : GetRewardedAdSkippedAction(placement);
            callback();

            // Reset the completed flag.
            mIsRewardedAdCompleted = false;

            //Destroy the RewardedAd to create a new one.
            DestroyRewardedAds(placement);

            if (OnRewardedAdClosed != null)
                OnRewardedAdClosed.Invoke(sender, args);
        }

        private void HandleAdMobRewardBasedVideoRewarded(object sender, Reward args)
        {
            mIsRewardedAdCompleted = true;

            if (OnRewardedAdRewarded != null)
                OnRewardedAdRewarded.Invoke(sender, args);
        }

        private void HandleAdMobRewardBasedVideoLeftApplication(object sender, EventArgs args)
        {
            if (OnRewardedAdLeavingApplication != null)
                OnRewardedAdLeavingApplication.Invoke(sender, args);
        }

#endif

        #endregion // Ad Event Handlers
    }
}