using UnityEngine;
using System;
using System.Collections.Generic;

namespace Yodo1.MAS
{
    public class Yodo1U3dAppOpenAd
    {
        private string adPlacement = string.Empty;

        private Action<Yodo1U3dAppOpenAd> _onAppOpenAdLoadedEvent;
        private Action<Yodo1U3dAppOpenAd, Yodo1U3dAdError> _onAppOpenAdLoadFailedEvent;
        private Action<Yodo1U3dAppOpenAd> _onAppOpenAdOpeningdEvent;
        private Action<Yodo1U3dAppOpenAd> _onAppOpenAdOpenedEvent;
        private Action<Yodo1U3dAppOpenAd, Yodo1U3dAdError> _onAppOpenAdOpenFailedEvent;
        private Action<Yodo1U3dAppOpenAd> _onAppOpenAdClosedEvent;

        public event Action<Yodo1U3dAppOpenAd> OnAdLoadedEvent
        {
            add
            {
                _onAppOpenAdLoadedEvent += value;
            }
            remove
            {
                _onAppOpenAdLoadedEvent -= value;
            }
        }

        public event Action<Yodo1U3dAppOpenAd, Yodo1U3dAdError> OnAdLoadFailedEvent
        {
            add
            {
                _onAppOpenAdLoadFailedEvent += value;
            }
            remove
            {
                _onAppOpenAdLoadFailedEvent -= value;
            }
        }

        public event Action<Yodo1U3dAppOpenAd> OnAdOpeningEvent
        {
            add
            {
                _onAppOpenAdOpeningdEvent += value;
            }
            remove
            {
                _onAppOpenAdOpeningdEvent -= value;
            }
        }

        public event Action<Yodo1U3dAppOpenAd> OnAdOpenedEvent
        {
            add
            {
                _onAppOpenAdOpenedEvent += value;
            }
            remove
            {
                _onAppOpenAdOpenedEvent -= value;
            }
        }

        public event Action<Yodo1U3dAppOpenAd, Yodo1U3dAdError> OnAdOpenFailedEvent
        {
            add
            {
                _onAppOpenAdOpenFailedEvent += value;
            }
            remove
            {
                _onAppOpenAdOpenFailedEvent -= value;
            }
        }

        public event Action<Yodo1U3dAppOpenAd> OnAdClosedEvent
        {
            add
            {
                _onAppOpenAdClosedEvent += value;
            }
            remove
            {
                _onAppOpenAdClosedEvent -= value;
            }
        }

        private static class HelperHolder
        {
            public static Yodo1U3dAppOpenAd Helper = new Yodo1U3dAppOpenAd();
        }

        public static Yodo1U3dAppOpenAd GetInstance()
        {
            return HelperHolder.Helper;
        }

        public static void CallbcksEvent(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError adError)
        {
            Yodo1U3dAppOpenAd.GetInstance().Callbacks(adEvent, adError);
        }

        private void Callbacks(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError adError)
        {
            switch (adEvent)
            {
                case Yodo1U3dAdEvent.AdLoaded:
                    Yodo1U3dMasCallback.InvokeEvent(_onAppOpenAdLoadedEvent, this);
                    break;
                case Yodo1U3dAdEvent.AdLoadFail:
                    Yodo1U3dMasCallback.InvokeEvent(_onAppOpenAdLoadFailedEvent, this, adError);
                    break;
                case Yodo1U3dAdEvent.AdOpening:
                    Yodo1U3dMasCallback.InvokeEvent(_onAppOpenAdOpeningdEvent, this);
                    break;
                case Yodo1U3dAdEvent.AdOpened:
                    Yodo1U3dMasCallback.Instance.Pause();
                    Yodo1U3dMasCallback.InvokeEvent(_onAppOpenAdOpenedEvent, this);
                    break;
                case Yodo1U3dAdEvent.AdOpenFail:
                    Yodo1U3dMasCallback.Instance.UnPause();
                    Yodo1U3dMasCallback.InvokeEvent(_onAppOpenAdOpenFailedEvent, this, adError);
                    break;
                case Yodo1U3dAdEvent.AdClosed:
                    Yodo1U3dMasCallback.Instance.UnPause();
                    Yodo1U3dMasCallback.InvokeEvent(_onAppOpenAdClosedEvent, this);
                    break;
            }
        }

        public bool autoDelayIfLoadFail = false;

        /// <summary>
        /// The default `Yodo1U3dAppOpenAd` constructor
        /// </summary>
        private Yodo1U3dAppOpenAd()
        {

        }

        private void AppOpen(string methodName)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.AppOpen(methodName, this.ToJsonString());
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.AppOpen(methodName, this.ToJsonString());
#endif
            }
        }

        private bool IsAdLoaded(string methodName)
        {
#if UNITY_EDITOR
            return true;
#else
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                return Yodo1U3dAdsIOS.IsAdLoadedV2(methodName, this.ToJsonString());
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                return Yodo1U3dAdsAndroid.IsAdLoadedV2(methodName);
#endif
            }
            return false;
#endif
        }

        public void SetAdPlacement(string adPlacement)
        {
            this.adPlacement = adPlacement;
        }

        public void LoadAd()
        {
#if UNITY_EDITOR
            Yodo1EditorAds.LoadAppOpenInEditor();
#endif
#if !UNITY_EDITOR
        AppOpen("loadAppOpenAd");
#endif
        }

        public bool IsLoaded()
        {
            return IsAdLoaded("isAppOpenAdLoaded");
        }

        /// <summary>
        /// Show reward ads
        /// </summary>
        public void ShowAd()
        {
            handleOpningEvent();
            this.adPlacement = string.Empty;
#if UNITY_EDITOR
            Yodo1EditorAds.ShowAppOpenInEditor();
#endif
#if !UNITY_EDITOR
        AppOpen("showAppOpenAd");
#endif
        }

        public void ShowAd(string placement)
        {
            handleOpningEvent();
            this.adPlacement = placement;
#if UNITY_EDITOR
            Yodo1EditorAds.ShowAppOpenInEditor();
#endif
#if !UNITY_EDITOR
        AppOpen("showAppOpenAd");
#endif
        }

        private void handleOpningEvent()
        {
            if (IsLoaded())
            {
                CallbcksEvent(Yodo1U3dAdEvent.AdOpening, null);
            }
        }

        public string ToJsonString()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("adPlacement", this.adPlacement);
            dic.Add("autoDelayIfLoadFail", this.autoDelayIfLoadFail);
            return Yodo1JSON.Serialize(dic);
        }
    }
}