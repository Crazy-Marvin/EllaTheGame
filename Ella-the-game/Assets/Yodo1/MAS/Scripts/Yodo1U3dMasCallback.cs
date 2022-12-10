namespace Yodo1.MAS
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;

    public class Yodo1U3dMasCallback : MonoBehaviour
    {
        private const int FLAG_INITIALIZE = 0;
        private const int FLAG_AD_EVENT = 1;

        private const int EVENT_INITIALIZE_FAILURE = 0;
        private const int EVENT_INITIALIZE_SUCCESS = 1;

        static bool _initialized = false;

        public enum AdType
        {
            Rewarded = 1,
            Interstitial = 2,
            Banner = 3,
            Native = 4,
            RewardedInterstitial = 5,
            AppOpen = 6,
        }

        public static Yodo1U3dMasCallback Instance { get; private set; }

        public string SdkMethodName
        {
            get
            {
                return "Yodo1U3dMasCallbackResult";
            }
        }

        public string SdkObjectName
        {
            get
            {
                return gameObject.name;
            }
        }

        public static bool isInitialized()
        {
            return _initialized;
        }

        private static Yodo1U3dMas.InitializeDelegate _initializeDelegate;
        public static void SetInitializeDelegate(Yodo1U3dMas.InitializeDelegate initializeDelegate)
        {
            _initializeDelegate = initializeDelegate;
        }

        public static Yodo1U3dMas.InitializeDelegate GetInitializeDelegate()
        {
            return _initializeDelegate;
        }

        #region Ad Delegate

        //InterstitialAd of delegate
        private static Yodo1U3dMas.InterstitialAdDelegate _interstitialAdDelegate;
        public static void SetInterstitialAdDelegate(Yodo1U3dMas.InterstitialAdDelegate interstitialAdDelegate)
        {
            _interstitialAdDelegate = interstitialAdDelegate;
        }

        //ShowBanner of delegate
        private static Yodo1U3dMas.BannerdAdDelegate _bannerDelegate;
        public static void SetBannerAdDelegate(Yodo1U3dMas.BannerdAdDelegate bannerDelegate)
        {
            _bannerDelegate = bannerDelegate;
        }

        //RewardVideo of delegate
        private static Yodo1U3dMas.RewardedAdDelegate _rewardedAdDelegate;
        public static void SetRewardedAdDelegate(Yodo1U3dMas.RewardedAdDelegate rewardedAdDelegate)
        {
            _rewardedAdDelegate = rewardedAdDelegate;
        }

        #endregion

        private static bool CanInvokeEvent(System.Delegate evt)
        {
            if (evt == null) return false;

            if (evt.GetInvocationList().Length > 5)
            {
            }

            return true;
        }

        private static void InvokeEvent(System.Action evt)
        {
            if (!CanInvokeEvent(evt)) return;

            evt();
        }

        public static void InvokeEvent<T>(System.Action<T> evt, T param)
        {
            if (!CanInvokeEvent(evt)) return;

            evt(param);
        }

        public static void InvokeEvent<T1, T2>(System.Action<T1, T2> evt, T1 param1, T2 param2)
        {
            if (!CanInvokeEvent(evt)) return;

            evt(param1, param2);
        }

        // Fired when the SDK has finished initializing
        private static System.Action<bool, Yodo1U3dAdError> _onSdkInitializedEvent;
        public static event System.Action<bool, Yodo1U3dAdError> OnSdkInitializedEvent
        {
            add
            {
                _onSdkInitializedEvent += value;
            }
            remove
            {
                _onSdkInitializedEvent -= value;
            }
        }

        private static System.Action _onBannerAdOpenedEvent;
        private static System.Action<Yodo1U3dAdError> _onBannerAdErrorEvent;
        private static System.Action _onBannerAdClosedEvent;

        public class Banner
        {
            public static event System.Action OnAdOpenedEvent
            {
                add
                {
                    _onBannerAdOpenedEvent += value;
                }
                remove
                {
                    _onBannerAdOpenedEvent -= value;
                }
            }

            public static event System.Action<Yodo1U3dAdError> OnAdErrorEvent
            {
                add
                {
                    _onBannerAdErrorEvent += value;
                }
                remove
                {
                    _onBannerAdErrorEvent -= value;
                }
            }

            public static event System.Action OnAdClosedEvent
            {
                add
                {
                    _onBannerAdClosedEvent += value;
                }
                remove
                {
                    _onBannerAdClosedEvent -= value;
                }
            }
        }

        private static System.Action _onInterstitialAdOpenedEvent;
        private static System.Action _onInterstitialAdClosedEvent;
        private static System.Action<Yodo1U3dAdError> _onInterstitialAdErrorEvent;

        public class Interstitial
        {
            /**
             * Fired when an interstitial ad is displayed (may not be received by Unity until the interstitial ad closes).
             */
            public static event System.Action OnAdOpenedEvent
            {
                add
                {
                    _onInterstitialAdOpenedEvent += value;
                }
                remove
                {
                    _onInterstitialAdOpenedEvent -= value;
                }
            }

            public static event System.Action OnAdClosedEvent
            {
                add
                {
                    _onInterstitialAdClosedEvent += value;
                }
                remove
                {
                    _onInterstitialAdClosedEvent -= value;
                }
            }

            //[System.Obsolete("Please use `Yodo1U3dMasCallback.Interstitial` instead.\n" +
            //"Yodo1U3dMasCallback.Interstitial.OnAdLoadFailedEvent += OnInterstitialAdLoadFailedEvent;\n" +
            //"Yodo1U3dMasCallback.Interstitial.OnAdOpenFailedEvent += OnInterstitialAdOpenFailedEvent;", false)]
            public static event System.Action<Yodo1U3dAdError> OnAdErrorEvent
            {
                add
                {
                    _onInterstitialAdErrorEvent += value;
                }
                remove
                {
                    _onInterstitialAdErrorEvent -= value;
                }
            }
        }

        private static System.Action _onRewardedAdOpenedEvent;
        private static System.Action _onRewardedAdClosedEvent;
        private static System.Action _onRewardedAdReceivedRewardEvent;
        private static System.Action<Yodo1U3dAdError> _onRewardedAdErrorEvent;

        public class Rewarded
        {
            /**
             * Fired when an rewarded ad is displayed (may not be received by Unity until the rewarded ad closes).
             */
            public static event System.Action OnAdOpenedEvent
            {
                add
                {
                    _onRewardedAdOpenedEvent += value;
                }
                remove
                {
                    _onRewardedAdOpenedEvent -= value;
                }
            }

            public static event System.Action OnAdClosedEvent
            {
                add
                {
                    _onRewardedAdClosedEvent += value;
                }
                remove
                {
                    _onRewardedAdClosedEvent -= value;
                }
            }

            public static event System.Action OnAdReceivedRewardEvent
            {
                add
                {
                    _onRewardedAdReceivedRewardEvent += value;
                }
                remove
                {
                    _onRewardedAdReceivedRewardEvent -= value;
                }
            }

            //[System.Obsolete("Please use `Yodo1U3dMasCallback.Rewarded` instead.\n" +
            //"Yodo1U3dMasCallback.Rewarded.OnAdLoadFailedEvent += OnRewardedAdLoadFailedEvent;\n" +
            //"Yodo1U3dMasCallback.Rewarded.OnAdOpenFailedEvent += OnRewardedAdOpenFailedEvent;", false)]
            public static event System.Action<Yodo1U3dAdError> OnAdErrorEvent
            {
                add
                {
                    _onRewardedAdErrorEvent += value;
                }
                remove
                {
                    _onRewardedAdErrorEvent -= value;
                }
            }
        }

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
        }

        #region Pause game

        private static bool _autoPauseGame = true;

        public static void SetAutoPauseGame(bool autoPauseGame)
        {
            _autoPauseGame = autoPauseGame;
            PrintAutoGameInfo();
        }

        public static void PrintAutoGameInfo()
        {
            if (_autoPauseGame)
            {
                Debug.Log(Yodo1U3dMas.TAG + "The feature of auto pause game is enabled, please call `Yodo1U3dMas.SetAutoPauseGame(false)` if you want to disable it");
            }
            else
            {
                Debug.Log(Yodo1U3dMas.TAG + "The feature of auto pause game is disabled, please call `Yodo1U3dMas.SetAutoPauseGame(true)` if you want to enable it");
            }
        }

        public void Pause()
        {
            if (!_autoPauseGame)
            {
                return;
            }

            Time.timeScale = 0;
            AudioListener.volume = 0;
        }

        public void UnPause()
        {
            if (!_autoPauseGame)
            {
                return;
            }

            Time.timeScale = 1;
            AudioListener.volume = 1;
        }

        #endregion

        public void Yodo1U3dMasCallbackResult(string result)
        {
            Debug.Log(Yodo1U3dMas.TAG + "The SDK callback result:" + result + "\n");

            Dictionary<string, object> obj = (Dictionary<string, object>)Yodo1JSON.Deserialize(result);
            if (obj == null)
            {
                return;
            }

            if (!obj.ContainsKey("flag") || !obj.ContainsKey("data"))
            {
                return;
            }

            string jsonData = obj["data"].ToString();
            Dictionary<string, object> dataDic = (Dictionary<string, object>)Yodo1JSON.Deserialize(jsonData);
            if (dataDic == null)
            {
                return;
            }

            int flag = int.Parse(obj["flag"].ToString());
            if (flag == FLAG_INITIALIZE)
            {
                bool success = false;
                Yodo1U3dAdError error = null;

                if (dataDic.ContainsKey("success"))
                {
                    success = int.Parse(dataDic["success"].ToString()) == EVENT_INITIALIZE_SUCCESS;
                }
                if (dataDic.ContainsKey("error"))
                {
                    error = Yodo1U3dAdError.createWithJson(dataDic["error"].ToString());
                }
                else
                {
                    error = new Yodo1U3dAdError();
                }

                _initialized = success;
                if (_initializeDelegate != null)
                {
                    _initializeDelegate(success, error);
                }
                InvokeEvent(_onSdkInitializedEvent, success, error);
            }
            else if (flag == FLAG_AD_EVENT)
            {
                AdType type = AdType.Rewarded;
                if (dataDic.ContainsKey("type"))
                {
                    type = (AdType)int.Parse(dataDic["type"].ToString());
                }
                else
                {
                    return;
                }
                Yodo1U3dAdEvent adEvent = Yodo1U3dAdEvent.AdError;
                if (dataDic.ContainsKey("code"))
                {
                    adEvent = (Yodo1U3dAdEvent)int.Parse(dataDic["code"].ToString());
                }
                string message;
                if (dataDic.ContainsKey("message"))
                {
                    message = dataDic["message"].ToString();
                }
                Yodo1U3dAdError adError = null;
                if (dataDic.ContainsKey("error"))
                {
                    adError = Yodo1U3dAdError.createWithJson(Yodo1JSON.Serialize(dataDic["error"]));
                }
                else
                {
                    adError = new Yodo1U3dAdError();
                }

                string indexId = string.Empty;
                if (dataDic.ContainsKey("indexId"))
                {
                    indexId = dataDic["indexId"].ToString();
                }

                switch (type)
                {
                    case AdType.Rewarded:
                        {
                            if (_rewardedAdDelegate != null)
                            {
                                _rewardedAdDelegate(adEvent, adError);
                            }

                            RewardedCallbacksEvent(adEvent, adError);
                            Yodo1U3dRewardAd.CallbcksEvent(adEvent, adError);
                        }
                        break;
                    case AdType.Interstitial:
                        {
                            if (_interstitialAdDelegate != null)
                            {
                                _interstitialAdDelegate(adEvent, adError);
                            }

                            InterstitialCallbacksEvent(adEvent, adError);
                            Yodo1U3dInterstitialAd.CallbcksEvent(adEvent, adError);
                        }
                        break;
                    case AdType.Banner:
                        {
                            if (_bannerDelegate != null)
                            {
                                _bannerDelegate(adEvent, adError);
                            }

                            BannerCallbacksEvent(adEvent, adError);

                            Yodo1U3dBannerAdView.CallbcksEvent(adEvent, adError, indexId);
                        }
                        break;
                    case AdType.Native:
                        {
                            Yodo1U3dNativeAdView.CallbcksEvent(adEvent, adError, indexId);
                        }
                        break;
                    case AdType.RewardedInterstitial:
                        {
                            Yodo1U3dRewardedInterstitialAd.CallbcksEvent(adEvent, adError);
                        }
                        break;
                    case AdType.AppOpen:
                        {
                            Yodo1U3dAppOpenAd.CallbcksEvent(adEvent, adError);
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        private void BannerCallbacksEvent(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError adError)
        {
            switch (adEvent)
            {
                case Yodo1U3dAdEvent.AdError:
                    InvokeEvent(_onBannerAdErrorEvent, adError);
                    break;
                case Yodo1U3dAdEvent.AdOpened:
                    InvokeEvent(_onBannerAdOpenedEvent);
                    break;
                case Yodo1U3dAdEvent.AdClosed:
                    InvokeEvent(_onBannerAdClosedEvent);
                    break;
            }
        }

        private void InterstitialCallbacksEvent(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError adError)
        {
            switch (adEvent)
            {
                case Yodo1U3dAdEvent.AdError:
                    if (adError.Code == -600201)
                    {
                        UnPause();
                    }
                    InvokeEvent(_onInterstitialAdErrorEvent, adError);
                    break;
                case Yodo1U3dAdEvent.AdOpened:
                    Pause();
                    InvokeEvent(_onInterstitialAdOpenedEvent);
                    break;
                case Yodo1U3dAdEvent.AdClosed:
                    UnPause();
                    InvokeEvent(_onInterstitialAdClosedEvent);
                    break;
            }
        }

        private void RewardedCallbacksEvent(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError adError)
        {
            switch (adEvent)
            {
                case Yodo1U3dAdEvent.AdError:
                    if (adError.Code == -600201)
                    {
                        UnPause();
                    }
                    InvokeEvent(_onRewardedAdErrorEvent, adError);
                    break;
                case Yodo1U3dAdEvent.AdOpened:
                    Pause();
                    InvokeEvent(_onRewardedAdOpenedEvent);
                    break;
                case Yodo1U3dAdEvent.AdReward:
                    InvokeEvent(_onRewardedAdReceivedRewardEvent);
                    break;
                case Yodo1U3dAdEvent.AdClosed:
                    UnPause();
                    InvokeEvent(_onRewardedAdClosedEvent);
                    break;
            }
        }
        public static void ForwardEvent(string val)
        {
            if (string.Equals(val, "onSdkInitializedEvent"))
            {
                Yodo1U3dAdError error = new Yodo1U3dAdError();
                error.Message = "No error in initialization";
                _initialized = true;
                InvokeEvent(_onSdkInitializedEvent, true, error);
            }
            else if (string.Equals(val, "onRewardedAdLoadedEvent"))
            {
                Yodo1U3dRewardAd.CallbcksEvent(Yodo1U3dAdEvent.AdLoaded, null);
            }
            else if (string.Equals(val, "onRewardedAdLoadFailedEvent"))
            {
                Yodo1U3dAdError error = new Yodo1U3dAdError();
                error.Message = "No ads found.";
                Yodo1U3dRewardAd.CallbcksEvent(Yodo1U3dAdEvent.AdLoadFail, error);
            }
            else if (string.Equals(val, "onRewardedAdOpenedEvent"))
            {
                Yodo1U3dMasCallback.Instance.Pause();
                InvokeEvent(_onRewardedAdOpenedEvent);
                Yodo1U3dRewardAd.CallbcksEvent(Yodo1U3dAdEvent.AdOpened, null);
            }
            else if (string.Equals(val, "onRewardedAdOpenFailedEvent"))
            {
                Yodo1U3dAdError error = new Yodo1U3dAdError();
                error.Message = "Ad failed to play.";
                Yodo1U3dRewardAd.CallbcksEvent(Yodo1U3dAdEvent.AdOpenFail, error);
            }
            else if (string.Equals(val, "onRewardedAdClosedEvent"))
            {
                Yodo1U3dMasCallback.Instance.UnPause();
                InvokeEvent(_onRewardedAdClosedEvent);
                Yodo1U3dRewardAd.CallbcksEvent(Yodo1U3dAdEvent.AdClosed, null);
            }
            else if (string.Equals(val, "onRewardedAdReceivedRewardEvent"))
            {
                InvokeEvent(_onRewardedAdReceivedRewardEvent);
                Yodo1U3dRewardAd.CallbcksEvent(Yodo1U3dAdEvent.AdReward, null);
            }
            else if (string.Equals(val, "onInterstitialAdLoadedEvent"))
            {
                Yodo1U3dInterstitialAd.CallbcksEvent(Yodo1U3dAdEvent.AdLoaded, null);
            }
            else if (string.Equals(val, "onInterstitialAdLoadFailedEvent"))
            {
                Yodo1U3dAdError error = new Yodo1U3dAdError();
                error.Message = "No Ads found.";
                Yodo1U3dInterstitialAd.CallbcksEvent(Yodo1U3dAdEvent.AdLoadFail, error);
            }
            else if (string.Equals(val, "onInterstitialAdOpenedEvent"))
            {
                Yodo1U3dMasCallback.Instance.Pause();
                InvokeEvent(_onInterstitialAdOpenedEvent);
                Yodo1U3dInterstitialAd.CallbcksEvent(Yodo1U3dAdEvent.AdOpened, null);
            }
            else if (string.Equals(val, "onInterstitialAdOpenFailedEvent"))
            {
                Yodo1U3dAdError error = new Yodo1U3dAdError();
                error.Message = "Ad failed to play.";
                Yodo1U3dInterstitialAd.CallbcksEvent(Yodo1U3dAdEvent.AdOpenFail, error);
            }
            else if (string.Equals(val, "onInterstitialAdClosedEvent"))
            {
                Yodo1U3dMasCallback.Instance.UnPause();
                InvokeEvent(_onInterstitialAdClosedEvent);
                Yodo1U3dInterstitialAd.CallbcksEvent(Yodo1U3dAdEvent.AdClosed, null);
            }
            else if (string.Equals(val, "onRewardedInterstitialAdLoadedEvent"))
            {
                Yodo1U3dRewardedInterstitialAd.CallbcksEvent(Yodo1U3dAdEvent.AdLoaded, null);
            }
            else if (string.Equals(val, "onRewardedInterstitialAdLoadFailedEvent"))
            {
                Yodo1U3dAdError error = new Yodo1U3dAdError();
                error.Message = "No ads found.";
                Yodo1U3dRewardedInterstitialAd.CallbcksEvent(Yodo1U3dAdEvent.AdLoadFail, error);
            }
            else if (string.Equals(val, "onRewardedInterstitialAdOpenedEvent"))
            {
                Yodo1U3dMasCallback.Instance.Pause();
                InvokeEvent(_onRewardedAdOpenedEvent);
                Yodo1U3dRewardedInterstitialAd.CallbcksEvent(Yodo1U3dAdEvent.AdOpened, null);
            }
            else if (string.Equals(val, "onRewardedInterstitialAdOpenFailedEvent"))
            {
                Yodo1U3dAdError error = new Yodo1U3dAdError();
                error.Message = "Ad failed to play.";
                Yodo1U3dRewardedInterstitialAd.CallbcksEvent(Yodo1U3dAdEvent.AdOpenFail, error);
            }
            else if (string.Equals(val, "onRewardedInterstitialAdClosedEvent"))
            {
                Yodo1U3dMasCallback.Instance.UnPause();
                InvokeEvent(_onRewardedAdClosedEvent);
                Yodo1U3dRewardedInterstitialAd.CallbcksEvent(Yodo1U3dAdEvent.AdClosed, null);
            }
            else if (string.Equals(val, "onRewardedInterstitialAdEarnedEvent"))
            {
                InvokeEvent(_onRewardedAdReceivedRewardEvent);
                Yodo1U3dRewardedInterstitialAd.CallbcksEvent(Yodo1U3dAdEvent.AdReward, null);
            }
            else if (string.Equals(val, "onAppOpenAdLoadedEvent"))
            {
                Yodo1U3dAppOpenAd.CallbcksEvent(Yodo1U3dAdEvent.AdLoaded, null);
            }
            else if (string.Equals(val, "onAppOpenAdLoadFailedEvent"))
            {
                Yodo1U3dAdError error = new Yodo1U3dAdError();
                error.Message = "No ads found.";
                Yodo1U3dAppOpenAd.CallbcksEvent(Yodo1U3dAdEvent.AdLoadFail, error);
            }
            else if (string.Equals(val, "onAppOpenAdOpenedEvent"))
            {
                Yodo1U3dMasCallback.Instance.Pause();
                InvokeEvent(_onRewardedAdOpenedEvent);
                Yodo1U3dAppOpenAd.CallbcksEvent(Yodo1U3dAdEvent.AdOpened, null);
            }
            else if (string.Equals(val, "onAppOpenAdOpenFailedEvent"))
            {
                Yodo1U3dAdError error = new Yodo1U3dAdError();
                error.Message = "Ad failed to play.";
                Yodo1U3dAppOpenAd.CallbcksEvent(Yodo1U3dAdEvent.AdOpenFail, error);
            }
            else if (string.Equals(val, "onAppOpenAdClosedEvent"))
            {
                Yodo1U3dMasCallback.Instance.UnPause();
                InvokeEvent(_onRewardedAdClosedEvent);
                Yodo1U3dAppOpenAd.CallbcksEvent(Yodo1U3dAdEvent.AdClosed, null);
            }
            else if (string.Equals(val, "onBannerAdOpenedEvent"))
            {
                InvokeEvent(_onBannerAdOpenedEvent);
            }
            else if (string.Equals(val, "onBannerAdClosedEvent"))
            {
                InvokeEvent(_onBannerAdClosedEvent);
            }

        }
    }
}