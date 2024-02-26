namespace Yodo1.MAS
{
    using UnityEngine;
    using System;
    using System.Collections.Generic;
    using UnityEngine.SceneManagement;
    using UnityEngine.EventSystems;

    public class Yodo1U3dMasCallback : MonoBehaviour
    {
        private const int FLAG_INITIALIZE = 0;
        private const int FLAG_AD_EVENT = 1;
        private const int FLAG_APP_EVENT = 2;

        private const int EVENT_INITIALIZE_FAILURE = 0;
        private const int EVENT_INITIALIZE_SUCCESS = 1;

        private const int EVENT_APP_FOREGROUND = 1;

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

        private static System.Action _onAppEnterForegroundEvent;
        public static event System.Action OnAppEnterForegroundEvent
        {
            add
            {
                _onAppEnterForegroundEvent += value;
            }
            remove
            {
                _onAppEnterForegroundEvent -= value;
            }
        }

        private static System.Action _onBannerAdOpenedEvent;
        private static System.Action<Yodo1U3dAdError> _onBannerAdErrorEvent;
        private static System.Action _onBannerAdClosedEvent;
        [System.Obsolete("Please use `Yodo1U3dBannerAdView` instead", true)]
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

        private static System.Action _onInterstitialAdOpeningEvent;
        private static System.Action _onInterstitialAdOpenedEvent;
        private static System.Action _onInterstitialAdClosedEvent;
        private static System.Action<Yodo1U3dAdError> _onInterstitialAdErrorEvent;

        [Obsolete("Please use Yodo1U3dInterstitialAd.GetInstance()", true)]
        public class Interstitial
        {
            public static event System.Action OnAdOpeningEvent
            {
                add
                {
                    _onInterstitialAdOpeningEvent += value;
                }
                remove
                {
                    _onInterstitialAdOpeningEvent -= value;
                }
            }

            /**
             * Fired when an interstitial ad is displayed (may not be received by Unity until the interstitial ad closes).
             */
            [Obsolete("Please use Yodo1U3dInterstitialAd.GetInstance().OnAdOpenedEvent", true)]
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
            [Obsolete("Please use Yodo1U3dInterstitialAd.GetInstance().OnAdClosedEvent", true)]
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

            [Obsolete("Please use Yodo1U3dInterstitialAd.GetInstance().OnAdOpenFailedEvent", true)]
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

        [Obsolete("Please use Yodo1U3dRewardAd.GetInstance()", true)]
        public class Rewarded
        {
            /**
             * Fired when an rewarded ad is displayed (may not be received by Unity until the rewarded ad closes).
             */
            [Obsolete("Please use Yodo1U3dRewardAd.GetInstance().OnAdOpenedEvent", true)]
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
            [Obsolete("Please use Yodo1U3dRewardAd.GetInstance().OnAdClosedEvent", true)]
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
            [Obsolete("Please use Yodo1U3dRewardAd.GetInstance().OnAdEarnedEvent", true)]
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

            [Obsolete("Please use Yodo1U3dRewardAd.GetInstance().OnAdOpenFailedEvent", true)]
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
#if UNITY_EDITOR
        private void OnEnable()
        {
            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        private void OnDisable()
        {
            SceneManager.sceneLoaded -= OnSceneLoaded;
        }
        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            EventSystem sceneEventSystem = FindObjectOfType<EventSystem>();
            if (GameObject.Find("Yodo1AdCanvas") == null)
            {
                Yodo1EditorAds.AdHolder = Instantiate(Resources.Load("SampleAds/AdHolder") as GameObject);
                Yodo1EditorAds.AdHolder.name = "Yodo1AdCanvas";
                Yodo1EditorAds.AdHolderCanvas = Yodo1EditorAds.AdHolder.transform.GetChild(0).GetComponent<Canvas>();
                Yodo1EditorAds.AdHolderCanvas.sortingOrder = Yodo1EditorAds.HighestOrder;
            }
            if (sceneEventSystem == null)
            {
                var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
            }
            Yodo1EditorAds.InitializeAds();

        }
#endif
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
                            Yodo1U3dRewardAd.CallbcksEvent(adEvent, adError);
                        }
                        break;
                    case AdType.Interstitial:
                        {
                            Yodo1U3dInterstitialAd.CallbcksEvent(adEvent, adError);
                        }
                        break;
                    case AdType.Banner:
                        {
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
            else if (flag == FLAG_APP_EVENT)
            {
                if (dataDic.ContainsKey("status"))
                {
                    int status = int.Parse(dataDic["status"].ToString());
                    if (status == EVENT_APP_FOREGROUND)
                    {
                        InvokeEvent(_onAppEnterForegroundEvent);
                    }
                }
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