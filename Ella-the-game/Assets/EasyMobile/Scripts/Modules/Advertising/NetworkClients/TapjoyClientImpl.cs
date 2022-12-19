using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using EasyMobile.Internal;

namespace EasyMobile
{
#if EM_TAPJOY
    using TapjoyUnity;
#endif

    public class TapjoyClientImpl : AdClientImpl
    {
        private const string NO_SDK_MESSAGE = "SDK missing. Please import the TapJoy plugin.";
        private const string BANNER_UNSUPPORTED_MESSAGE = "TapJoy does not support banner ad format";

#if EM_TAPJOY

        private TapjoySettings mAdSettings = null;

        private TJPlacement mDefaultInterstitialPlacement;
        private TJPlacement mDefaultRewaredVideoPlacement;

        /// <summary>
        /// We're gonna save all the loaded custom interstitial placements here.
        /// </summary>
        /// Key: The AdPlacement used to load the interstitial ad.
        /// Value: Loaded interstitial ad.
        private Dictionary<AdPlacement, TJPlacement> mCustomInterstitialPlacements;

        /// <summary>
        /// We're gonna save all the loaded rewarded video placements here.
        /// </summary>
        /// Key: The AdPlacement used to load the rewarded ad.
        /// Value: Loaded rewarded ad.
        private Dictionary<AdPlacement, TJPlacement> mCustomRewardedPlacements;

        private IEnumerator mAutoReconnectCoroutine;
        private bool mIsAutoReconnectCoroutineRunning;

#endif

        #region TapJoy Events

#if EM_TAPJOY

        /// <summary>
        /// Occurs on connect success.
        /// </summary>
        public event Tapjoy.OnConnectSuccessHandler OnConnectSuccess
        {
            add { Tapjoy.OnConnectSuccess += value; }
            remove { Tapjoy.OnConnectSuccess -= value; }
        }

        /// <summary>
        /// Occurs on connect failure.
        /// </summary>
        public event Tapjoy.OnConnectFailureHandler OnConnectFailure
        {
            add { Tapjoy.OnConnectFailure += value; }
            remove { Tapjoy.OnConnectFailure -= value; }
        }

        /// <summary>
        /// Occurs when set user ID successfully.
        /// </summary>
        public event Tapjoy.OnSetUserIDSuccessHandler OnSetUserIDSuccess
        {
            add { Tapjoy.OnSetUserIDSuccess += value; }
            remove { Tapjoy.OnSetUserIDSuccess -= value; }
        }

        /// <summary>
        /// Occurs when failed set user ID.
        /// </summary>
        public event Tapjoy.OnSetUserIDFailureHandler OnSetUserIDFailure
        {
            add { Tapjoy.OnSetUserIDFailure += value; }
            remove { Tapjoy.OnSetUserIDFailure -= value; }
        }

        /// <summary>
        /// Occurs when get currency balance response.
        /// </summary>
        public event Tapjoy.OnGetCurrencyBalanceResponseHandler OnGetCurrencyBalanceResponse
        {
            add { Tapjoy.OnGetCurrencyBalanceResponse += value; }
            remove { Tapjoy.OnGetCurrencyBalanceResponse -= value; }
        }

        /// <summary>
        /// Occurs on spend currency response.
        /// </summary>
        public event Tapjoy.OnSpendCurrencyResponseHandler OnSpendCurrencyResponse
        {
            add { Tapjoy.OnSpendCurrencyResponse += value; }
            remove { Tapjoy.OnSpendCurrencyResponse -= value; }
        }

        /// <summary>
        /// Occurs on spend currency response failure.
        /// </summary>
        public event Tapjoy.OnSpendCurrencyResponseFailureHandler OnSpendCurrencyResponseFailure
        {
            add { Tapjoy.OnSpendCurrencyResponseFailure += value; }
            remove { Tapjoy.OnSpendCurrencyResponseFailure -= value; }
        }

        /// <summary>
        /// Occurs on get currency balance response failure.
        /// </summary>
        public event Tapjoy.OnGetCurrencyBalanceResponseFailureHandler OnGetCurrencyBalanceResponseFailure
        {
            add { Tapjoy.OnGetCurrencyBalanceResponseFailure += value; }
            remove { Tapjoy.OnGetCurrencyBalanceResponseFailure -= value; }
        }

        /// <summary>
        /// Occurs on award currency response failure.
        /// </summary>
        public event Tapjoy.OnAwardCurrencyResponseFailureHandler OnAwardCurrencyResponseFailure
        {
            add { Tapjoy.OnAwardCurrencyResponseFailure += value; }
            remove { Tapjoy.OnAwardCurrencyResponseFailure -= value; }
        }

        /// <summary>
        /// Occurs when earned currency.
        /// </summary>
        public event Tapjoy.OnEarnedCurrencyHandler OnEarnedCurrency
        {
            add { Tapjoy.OnEarnedCurrency += value; }
            remove { Tapjoy.OnEarnedCurrency -= value; }
        }

        /// <summary>
        /// Occurs on TapJoy video start.
        /// </summary>
        public event Tapjoy.OnVideoStartHandler OnTapJoyVideoStart
        {
            add { Tapjoy.OnVideoStart += value; }
            remove { Tapjoy.OnVideoStart -= value; }
        }

        /// <summary>
        /// Occurs on TapJoy video error.
        /// </summary>
        public event Tapjoy.OnVideoErrorHandler OnTapJoyVideoError
        {
            add { Tapjoy.OnVideoError += value; }
            remove { Tapjoy.OnVideoError -= value; }
        }

        /// <summary>
        /// Occurs on TapJoy video complete.
        /// </summary>
        public event Tapjoy.OnVideoCompleteHandler OnTapJoyVideoComplete
        {
            add { Tapjoy.OnVideoComplete += value; }
            remove { Tapjoy.OnVideoComplete -= value; }
        }

        /// <summary>
        /// Occurs on award currency response.
        /// </summary>
        public event Tapjoy.OnAwardCurrencyResponseHandler OnAwardCurrencyResponse
        {
            add { Tapjoy.OnAwardCurrencyResponse += value; }
            remove { Tapjoy.OnAwardCurrencyResponse -= value; }
        }

        /// <summary>
        /// Occurs on TapJoy placement video complete.
        /// </summary>
        public event TJPlacement.OnVideoCompleteHandler OnTJPlacementVideoComplete
        {
            add { TJPlacement.OnVideoComplete += value; }
            remove { TJPlacement.OnVideoComplete -= value; }
        }

        /// <summary>
        /// Occurs on TapJoy placement video error.
        /// </summary>
        public event TJPlacement.OnVideoErrorHandler OnTJPlacementVideoError
        {
            add { TJPlacement.OnVideoError += value; }
            remove { TJPlacement.OnVideoError -= value; }
        }

        /// <summary>
        /// Occurs on TapJoy placement video start.
        /// </summary>
        public event TJPlacement.OnVideoStartHandler OnTJPlacementVideoStart
        {
            add { TJPlacement.OnVideoStart += value; }
            remove { TJPlacement.OnVideoStart -= value; }
        }

        /// <summary>
        /// Occurs on reward request.
        /// </summary>
        public event TJPlacement.OnRewardRequestHandler OnRewardRequest
        {
            add { TJPlacement.OnRewardRequest += value; }
            remove { TJPlacement.OnRewardRequest -= value; }
        }

        /// <summary>
        /// Occurs on content dismiss.
        /// </summary>
        public event TJPlacement.OnContentDismissHandler OnContentDismiss
        {
            add { TJPlacement.OnContentDismiss += value; }
            remove { TJPlacement.OnContentDismiss -= value; }
        }

        /// <summary>
        /// Occurs on content show.
        /// </summary>
        public event TJPlacement.OnContentShowHandler OnContentShow
        {
            add { TJPlacement.OnContentShow += value; }
            remove { TJPlacement.OnContentShow -= value; }
        }

        /// <summary>
        /// Occurs on content ready.
        /// </summary>
        public event TJPlacement.OnContentReadyHandler OnContentReady
        {
            add { TJPlacement.OnContentReady += value; }
            remove { TJPlacement.OnContentReady -= value; }
        }

        /// <summary>
        /// Occurs on request failure.
        /// </summary>
        public event TJPlacement.OnRequestFailureHandler OnRequestFailure
        {
            add { TJPlacement.OnRequestFailure += value; }
            remove { TJPlacement.OnRequestFailure -= value; }
        }

        /// <summary>
        /// Occurs on purchase request.
        /// </summary>
        public event TJPlacement.OnPurchaseRequestHandler OnPurchaseRequest
        {
            add { TJPlacement.OnPurchaseRequest += value; }
            remove { TJPlacement.OnPurchaseRequest -= value; }
        }

        /// <summary>
        /// Occurs on request success.
        /// </summary>
        public event TJPlacement.OnRequestSuccessHandler OnRequestSuccess
        {
            add { TJPlacement.OnRequestSuccess += value; }
            remove { TJPlacement.OnRequestSuccess -= value; }
        }

#endif

        #endregion  // TapJoy-Specific Events

        #region Singleton

        private static TapjoyClientImpl sInstance = null;

        private TapjoyClientImpl()
        {
        }

        public static TapjoyClientImpl CreateClient()
        {
            if (sInstance == null)
            {
                sInstance = new TapjoyClientImpl();
            }

            return sInstance;
        }

        #endregion

        #region AdClient Overrides

        public override AdNetwork Network { get { return AdNetwork.TapJoy; } }

        public override bool IsBannerAdSupported { get { return false; } }

        public override bool IsInterstitialAdSupported { get { return true; } }

        public override bool IsRewardedAdSupported { get { return true; } }

        public override bool IsSdkAvail
        {
            get
            {
#if EM_TAPJOY
                return true;
#else
                return false;
#endif
            }
        }

        public override bool IsValidPlacement(AdPlacement placement, AdType type)
        {
#if EM_TAPJOY
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
#if EM_TAPJOY
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
#if EM_TAPJOY
                return mAdSettings == null ? null : mAdSettings.CustomRewardedAdIds;
#else
                return null;
#endif
            }
        }

        protected override string NoSdkMessage { get { return NO_SDK_MESSAGE; } }

        protected override void InternalInit()
        {
#if EM_TAPJOY

            mIsInitialized = true;
            mAdSettings = EM_Settings.Advertising.Tapjoy;

            mCustomInterstitialPlacements = new Dictionary<AdPlacement, TJPlacement>();
            mCustomRewardedPlacements = new Dictionary<AdPlacement, TJPlacement>();

            // TapJoy's placements can only be created after the game is successfully connnected to server.
            // If it is already connected when we're initializing this client, create all of them normally...
            if (Tapjoy.IsConnected)
            {
                CreateDefaultAdPlacements(mAdSettings);
            }
            // ...otherwise, we need to wait until it has been connected.
            else
            {
                Tapjoy.OnConnectSuccess += () =>
                {
                    CreateDefaultAdPlacements(mAdSettings);
                };
            }

            // Set GDPR consent (if any) *before* requesting any content from TapJoy.
            // https://dev.tapjoy.com/sdk-integration/#sdk11122_gdpr_release
            var consent = GetApplicableDataPrivacyConsent();
            ApplyDataPrivacyConsent(consent);

            // Subscribe to events.
            SetupTapJoyEventCallbacks();
            SetupTapJoyPlacementEventCallbacks();

            Debug.Log("Tapjoy client has been initialized.");
#endif
        }

        protected override void InternalShowBannerAd(AdPlacement _, BannerAdPosition __, BannerAdSize ___)
        {
            Debug.Log(BANNER_UNSUPPORTED_MESSAGE);
        }

        protected override void InternalHideBannerAd(AdPlacement _)
        {
            Debug.Log(BANNER_UNSUPPORTED_MESSAGE);
        }

        protected override void InternalDestroyBannerAd(AdPlacement _)
        {
            Debug.Log(BANNER_UNSUPPORTED_MESSAGE);
        }

        protected override bool InternalIsInterstitialAdReady(AdPlacement placement)
        {
#if EM_TAPJOY
            if (placement == AdPlacement.Default) // Default interstitial ad...
            {
                return IsPlacementAvailable(mDefaultInterstitialPlacement);
            }
            else // Custom interstitial ad...
            {
                return mCustomInterstitialPlacements != null &&
                mCustomInterstitialPlacements.ContainsKey(placement) &&
                IsPlacementAvailable(mCustomInterstitialPlacements[placement]);
            }
#else
            return false;
#endif
        }

        protected override void InternalLoadInterstitialAd(AdPlacement placement)
        {
#if EM_TAPJOY
            if (placement == AdPlacement.Default) // Default interstitial ad...
            {
                LoadPlacement(mDefaultInterstitialPlacement);
            }
            else // Custom interstitial...
            {
                string id = FindIdForPlacement(mAdSettings.CustomInterstitialAdIds, placement);

                if (string.IsNullOrEmpty(id))
                {
                    Debug.LogFormat("Attempting to load {0} interstitial ad with an undefined ID at placement {1}",
                        Network.ToString(),
                        AdPlacement.GetPrintableName(placement));
                    return;
                }

                if (!mCustomInterstitialPlacements.ContainsKey(placement) || mCustomInterstitialPlacements[placement] == null)
                    mCustomInterstitialPlacements[placement] = TJPlacement.CreatePlacement(id);

                LoadPlacement(mCustomInterstitialPlacements[placement]);
            }
#endif
        }

        protected override void InternalShowInterstitialAd(AdPlacement placement)
        {
#if EM_TAPJOY
            if (placement == AdPlacement.Default) // Default interstitial ad...
            {
                ShowPlacement(mDefaultInterstitialPlacement);
            }
            else // Custom interstitial ad...
            {
                if (mCustomInterstitialPlacements.ContainsKey(placement))
                    ShowPlacement(mCustomInterstitialPlacements[placement]);
            }
#endif
        }

        protected override bool InternalIsRewardedAdReady(AdPlacement placement)
        {
#if EM_TAPJOY
            if (placement == AdPlacement.Default) // Default rewarded ad...
            {
                return IsPlacementAvailable(mDefaultRewaredVideoPlacement);
            }
            else // Custom rewarded ad...
            {
                return mCustomRewardedPlacements != null &&
                mCustomRewardedPlacements.ContainsKey(placement) &&
                IsPlacementAvailable(mCustomRewardedPlacements[placement]);
            }
#else
            return false;
#endif
        }

        protected override void InternalLoadRewardedAd(AdPlacement placement)
        {
#if EM_TAPJOY
            if (placement == AdPlacement.Default) // Default rewarded ad...
            {
                LoadPlacement(mDefaultRewaredVideoPlacement);
            }
            else // Custom rewarded ad...
            {
                string id = FindIdForPlacement(mAdSettings.CustomRewardedAdIds, placement);

                if (string.IsNullOrEmpty(id))
                {
                    Debug.LogFormat("Attempting to load {0} rewarded ad with an undefined ID at placement {1}",
                        Network.ToString(),
                        AdPlacement.GetPrintableName(placement));
                    return;
                }

                if (!mCustomRewardedPlacements.ContainsKey(placement) || mCustomRewardedPlacements[placement] == null)
                    mCustomRewardedPlacements[placement] = TJPlacement.CreatePlacement(id);

                LoadPlacement(mCustomRewardedPlacements[placement]);
            }
#endif
        }

        protected override void InternalShowRewardedAd(AdPlacement placement)
        {
#if EM_TAPJOY
            if (placement == AdPlacement.Default)
            {
                ShowPlacement(mDefaultRewaredVideoPlacement);
            }
            else
            {
                if (mCustomRewardedPlacements.ContainsKey(placement))
                    ShowPlacement(mCustomRewardedPlacements[placement]);
            }
#endif
        }

        #endregion

        #region IConsentRequirable Overrides

        private const string DATA_PRIVACY_CONSENT_KEY = "EM_Ads_TapJoy_DataPrivacyConsent";

        protected override string DataPrivacyConsentSaveKey { get { return DATA_PRIVACY_CONSENT_KEY; } }

        protected override void ApplyDataPrivacyConsent(ConsentStatus consent)
        {
#if EM_TAPJOY
            // https://ltv.tapjoy.com/sdk/api/csharp/class_tapjoy_unity_1_1_tapjoy.html#ad59030716ab973c1656d939315c81d74
            switch (consent)
            {
                case ConsentStatus.Granted:
                    Tapjoy.SetUserConsent("1");
                    break;
                case ConsentStatus.Revoked:
                    Tapjoy.SetUserConsent("0");
                    break;
                case ConsentStatus.Unknown:
                default:
                    break;
            }
#endif
        }

        #endregion

#if EM_TAPJOY

        #region Protected methods

        protected IEnumerator AutoReconnectCoroutine(float interval)
        {
            if (interval < 0)
                yield break;

            while (true)
            {
                Tapjoy.Connect();
                Debug.Log("Connecting to Tapjoy server...");
                yield return new WaitForSeconds(interval);
            }
        }

        /// <summary>
        /// Creates default InterstitialPlacement and RewaredVideoPlacement.
        /// </summary>
        protected virtual void CreateDefaultAdPlacements(TapjoySettings adSettings)
        {
            mDefaultInterstitialPlacement = TJPlacement.CreatePlacement(adSettings.DefaultInterstitialAdId.Id);
            mDefaultRewaredVideoPlacement = TJPlacement.CreatePlacement(adSettings.DefaultRewardedAdId.Id);
        }

        /// <summary>
        /// Registers all TapJoy's events into right handlers.
        /// </summary>
        protected virtual void SetupTapJoyEventCallbacks()
        {
            Tapjoy.OnConnectSuccess += HandleOnConnectSuccess;
            Tapjoy.OnConnectFailure += HandleOnConnectFailure;

            Tapjoy.OnAwardCurrencyResponse += HandleOnAwardCurrencyResponse;
            Tapjoy.OnAwardCurrencyResponseFailure += HandleOnAwardCurrencyResponceFailure;
            Tapjoy.OnEarnedCurrency += HandleOnEarnedCurrency;

            Tapjoy.OnVideoComplete += HandleTapJoyOnVideoComplete;
            Tapjoy.OnVideoError += HandleTapJoyOnVideoError;
            Tapjoy.OnVideoStart += HandleTapJoyOnVideoStart;
        }

        /// <summary>
        /// Registers all TJPlacement's events into right handlers.
        /// </summary>
        protected virtual void SetupTapJoyPlacementEventCallbacks()
        {
            TJPlacement.OnContentDismiss += HandleOnContentDismiss;
            TJPlacement.OnContentReady += HandleOnContentReady;
            TJPlacement.OnContentShow += HandleOnContentShow;
            TJPlacement.OnPurchaseRequest += HandleOnPurchaseRequest;
            TJPlacement.OnRequestFailure += HandleOnRequestFailure;
            TJPlacement.OnRequestSuccess += HandleOnRequestSuccess;
            TJPlacement.OnRewardRequest += HandleOnRewardRequest;

            TJPlacement.OnVideoComplete += HandleTJPlacementOnVideoComplete;
            TJPlacement.OnVideoError += HandleTJPlacementOnVideoError;
            TJPlacement.OnVideoStart += HandleTJPlacementOnVideoStart;
        }

        /// <summary>
        /// Checks if a specific TapJoy placement is available or not.
        /// </summary>
        protected virtual bool IsPlacementAvailable(TJPlacement placement)
        {
            return placement == null ? false : placement.IsContentReady();
        }

        /// <summary>
        /// Loads a specific TapJoy placement if it's not null.
        /// </summary>
        /// <param name="nullMessage">This message will be logged to the console if the placement is null.</param>
        protected virtual void LoadPlacement(TJPlacement placement, string nullMessage = "Attempting to load a null Tapjoy placement.")
        {
            if (!IsInitialized)
            {
                Debug.Log("The TapJoy network is not ready...");
                return;
            }

            if (placement == null)
            {
                Debug.Log(nullMessage);
                return;
            }

            placement.RequestContent();
        }

        /// <summary>
        /// Shows a specific TapJoy placement if it's not null.
        /// </summary>
        /// <param name="nullMessage">This message will be logged to the console if the placement is null.</param>
        protected virtual void ShowPlacement(TJPlacement placement, string nullMessage = "Attempting to show a null Tapjoy placement.")
        {
            if (!IsInitialized)
            {
                Debug.Log("The TapJoy network is not ready...");
                return;
            }

            if (placement == null)
            {
                Debug.Log(nullMessage);
                return;
            }

            placement.ShowContent();
        }

        /// <summary>
        /// Checks the placement name and invoke right completed event.
        /// </summary>
        /// Called in HandleTJPlacementOnVideoComplete event handler.
        protected virtual void InvokePlacementCompleteEvent(TJPlacement tjPlacement)
        {
            if (mAdSettings == null || tjPlacement == null)
            {
                Debug.Log("Null value(s)!!!");
                return;
            }

            string targetName = tjPlacement.GetName();

            /// Check if the tjPlacement is the default interstitial placement.
            if (mDefaultInterstitialPlacement != null && mDefaultInterstitialPlacement.GetName().Equals(targetName))
            {
                OnInterstitialAdCompleted(AdPlacement.Default);
                return;
            }

            /// Check if the tjPlacement is the default rewarded video placement.
            if (mDefaultRewaredVideoPlacement != null && mDefaultRewaredVideoPlacement.GetName().Equals(targetName))
            {
                OnRewardedAdCompleted(AdPlacement.Default);
                return;
            }

            /// Check if the tjPlacement is one of loaded custom interstitial ads.
            AdPlacement adPlacement = GetAdPlacementFromTJPlacement(mCustomInterstitialPlacements, targetName);
            if (adPlacement != null)
            {
                OnInterstitialAdCompleted(adPlacement);
                return;
            }

            /// Check if the tjPlacement is one of loaded custom rewarded video ads.
            adPlacement = GetAdPlacementFromTJPlacement(mCustomRewardedPlacements, targetName);
            if (adPlacement != null)
            {
                OnRewardedAdCompleted(adPlacement);
                return;
            }

            /// Otherwise
            Debug.LogWarning("Tried to invoke completed event of an unexpected custom placement. Name: " + targetName);
        }

        /// <summary>
        /// Find the AdPlacement used to load a specific TJPlacement.
        /// </summary>
        /// <param name="loadedSource">
        /// <see cref="mCustomInterstitialPlacements"/> or <see cref="mCustomRewardedPlacements"/>.
        /// </param>
        /// <param name="placementName">Name of the target TJPlacement.</param>
        protected AdPlacement GetAdPlacementFromTJPlacement(Dictionary<AdPlacement, TJPlacement> loadedSource, string placementName)
        {
            if (loadedSource == null || placementName == null)
                return null;

            foreach (var pair in loadedSource)
            {
                if (pair.Value.GetName().Equals(placementName))
                    return pair.Key;
            }

            return null;
        }

        #endregion

        #region Ad Event Handlers

        /// TapJoy's events.

        private void HandleOnConnectSuccess()
        {
            Debug.Log("Connect to TapJoy server successfully");

            /// When we connected to Tapjoy server,
            /// stop the reconnect progress if it's running.
            if (mIsAutoReconnectCoroutineRunning)
            {
                mIsAutoReconnectCoroutineRunning = false;
                RuntimeHelper.EndCoroutine(mAutoReconnectCoroutine);
            }
        }

        private void HandleOnConnectFailure()
        {
            Debug.Log("Failed to connect to TapJoy server");

            /// At default, Tapjoy only connect to the server once when the app is opened,
            /// so we have to start a coroutine to reconnect automatically.
            /// Otherwise players will never be able to connect to the server
            /// if they open the app when their device is offline.
            if (mAdSettings.EnableAutoReconnect && !mIsAutoReconnectCoroutineRunning)
            {
                mAutoReconnectCoroutine = AutoReconnectCoroutine(mAdSettings.AutoReconnectInterval);
                RuntimeHelper.RunCoroutine(mAutoReconnectCoroutine);

                mIsAutoReconnectCoroutineRunning = true;
            }
        }

        private void HandleOnAwardCurrencyResponse(string currencyName, int balance)
        {

        }

        private void HandleOnAwardCurrencyResponceFailure(string errorMessage)
        {

        }

        private void HandleOnEarnedCurrency(string currencyName, int amount)
        {

        }

        private void HandleTapJoyOnVideoComplete()
        {

        }

        private void HandleTapJoyOnVideoError(string errorMessage)
        {

        }

        private void HandleTapJoyOnVideoStart()
        {

        }

        /// TJPlacement's events

        private void HandleOnContentDismiss(TJPlacement placement)
        {

        }

        private void HandleOnContentReady(TJPlacement placement)
        {

        }

        private void HandleOnContentShow(TJPlacement placement)
        {

        }

        private void HandleOnPurchaseRequest(TJPlacement placement, TJActionRequest request, string productId)
        {

        }

        private void HandleOnRequestFailure(TJPlacement placement, string error)
        {

        }

        private void HandleOnRequestSuccess(TJPlacement placement)
        {

        }

        private void HandleOnRewardRequest(TJPlacement placement, TJActionRequest request, string itemId, int quantity)
        {

        }

        private void HandleTJPlacementOnVideoComplete(TJPlacement placement)
        {
            InvokePlacementCompleteEvent(placement);
        }

        private void HandleTJPlacementOnVideoError(TJPlacement placement, string errorMessage)
        {

        }

        private void HandleTJPlacementOnVideoStart(TJPlacement placement)
        {

        }

        #endregion

#endif
    }
}
