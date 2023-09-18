using UnityEngine;
using System;
using System.Collections.Generic;

namespace Yodo1.MAS
{
    public class Yodo1U3dInterstitialAd
    {

        private string adPlacement = "";

        private Action<Yodo1U3dInterstitialAd> _onInterstitialAdLoadedEvent;
        private Action<Yodo1U3dInterstitialAd, Yodo1U3dAdError> _onInterstitialAdLoadFailedEvent;
        private Action<Yodo1U3dInterstitialAd> _onInterstitialAdOpenedEvent;
        private Action<Yodo1U3dInterstitialAd, Yodo1U3dAdError> _onInterstitialAdOpenFailedEvent;
        private Action<Yodo1U3dInterstitialAd> _onInterstitialAdClosedEvent;

        public event Action<Yodo1U3dInterstitialAd> OnAdLoadedEvent
        {
            add
            {
                _onInterstitialAdLoadedEvent += value;
            }
            remove
            {
                _onInterstitialAdLoadedEvent -= value;
            }
        }

        public event Action<Yodo1U3dInterstitialAd, Yodo1U3dAdError> OnAdLoadFailedEvent
        {
            add
            {
                _onInterstitialAdLoadFailedEvent += value;
            }
            remove
            {
                _onInterstitialAdLoadFailedEvent -= value;
            }
        }

        public event Action<Yodo1U3dInterstitialAd> OnAdOpenedEvent
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

        public event Action<Yodo1U3dInterstitialAd, Yodo1U3dAdError> OnAdOpenFailedEvent
        {
            add
            {
                _onInterstitialAdOpenFailedEvent += value;
            }
            remove
            {
                _onInterstitialAdOpenFailedEvent -= value;
            }
        }

        public event Action<Yodo1U3dInterstitialAd> OnAdClosedEvent
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

        private static class HelperHolder
        {
            public static Yodo1U3dInterstitialAd Helper = new Yodo1U3dInterstitialAd();
        }

        public static Yodo1U3dInterstitialAd GetInstance()
        {
            return HelperHolder.Helper;
        }


        public static void CallbcksEvent(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError adError)
        {
            Yodo1U3dInterstitialAd.GetInstance().Callbacks(adEvent, adError);
        }

        private void Callbacks(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError adError)
        {
            switch (adEvent)
            {
                case Yodo1U3dAdEvent.AdLoaded:
                    Yodo1U3dMasCallback.InvokeEvent(_onInterstitialAdLoadedEvent, this);
                    break;
                case Yodo1U3dAdEvent.AdLoadFail:
                    Yodo1U3dMasCallback.InvokeEvent(_onInterstitialAdLoadFailedEvent, this, adError);
                    break;
                case Yodo1U3dAdEvent.AdOpened:
                    Yodo1U3dMasCallback.Instance.Pause();
                    Yodo1U3dMasCallback.InvokeEvent(_onInterstitialAdOpenedEvent, this);
                    break;
                case Yodo1U3dAdEvent.AdOpenFail:
                    Yodo1U3dMasCallback.Instance.UnPause();
                    Yodo1U3dMasCallback.InvokeEvent(_onInterstitialAdOpenFailedEvent, this, adError);
                    break;
                case Yodo1U3dAdEvent.AdClosed:
                    Yodo1U3dMasCallback.Instance.UnPause();
                    Yodo1U3dMasCallback.InvokeEvent(_onInterstitialAdClosedEvent, this);
                    break;
            }
        }

        public bool autoDelayIfLoadFail = false;

        /// <summary>
        /// The default `Yodo1U3dInterstitialAd` constructor
        /// </summary>
        private Yodo1U3dInterstitialAd()
        {

        }

        private void SetAdPlacement(string adPlacement)
        {
            this.adPlacement = adPlacement;
        }

        private void InterstitialV2(string methodName)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.InterstitialV2(methodName, this.ToJsonString());
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.InterstitialV2(methodName, this.ToJsonString());
#endif
            }
        }

        private bool IsAdLoadedV2(string methodName)
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

        public void LoadAd()
        {
#if UNITY_EDITOR
            Yodo1EditorAds.LoadInterstitialAdsInEditor();
#endif
#if !UNITY_EDITOR
            InterstitialV2("loadInterstitialAdV2");
#endif
        }

        public bool IsLoaded()
        {
            return IsAdLoadedV2("isInterstitialAdLoadedV2");
        }

        /// <summary>
        /// Show interstitial ads
        /// </summary>
        public void ShowAd()
        {
            SetAdPlacement(string.Empty);
#if UNITY_EDITOR
            Yodo1EditorAds.ShowInterstitialAdsInEditor();
#endif
#if !UNITY_EDITOR
            InterstitialV2("showInterstitialAdV2");
#endif
        }

        public void ShowAd(string placement)
        {
            SetAdPlacement(placement);
#if UNITY_EDITOR
            Yodo1EditorAds.ShowInterstitialAdsInEditor();
#endif
#if !UNITY_EDITOR
            InterstitialV2("showInterstitialAdV2");
#endif
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