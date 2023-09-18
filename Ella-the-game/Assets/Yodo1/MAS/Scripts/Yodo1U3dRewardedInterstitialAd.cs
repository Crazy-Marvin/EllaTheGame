using UnityEngine;
using System;
using System.Collections.Generic;

namespace Yodo1.MAS
{
    public class Yodo1U3dRewardedInterstitialAd
    {
        private string adPlacement = string.Empty;

        private Action<Yodo1U3dRewardedInterstitialAd> _onRewardedInterstitialAdLoadedEvent;
        private Action<Yodo1U3dRewardedInterstitialAd, Yodo1U3dAdError> _onRewardedInterstitialAdLoadFailedEvent;
        private Action<Yodo1U3dRewardedInterstitialAd> _onRewardedInterstitialAdOpeningEvent;
        private Action<Yodo1U3dRewardedInterstitialAd> _onRewardedInterstitialAdOpenedEvent;
        private Action<Yodo1U3dRewardedInterstitialAd, Yodo1U3dAdError> _onRewardedInterstitialAdOpenFailedEvent;
        private Action<Yodo1U3dRewardedInterstitialAd> _onRewardedInterstitialAdClosedEvent;
        private Action<Yodo1U3dRewardedInterstitialAd> _onRewardedInterstitialAdEarnedEvent;

        public event Action<Yodo1U3dRewardedInterstitialAd> OnAdLoadedEvent
        {
            add
            {
                _onRewardedInterstitialAdLoadedEvent += value;
            }
            remove
            {
                _onRewardedInterstitialAdLoadedEvent -= value;
            }
        }

        public event Action<Yodo1U3dRewardedInterstitialAd, Yodo1U3dAdError> OnAdLoadFailedEvent
        {
            add
            {
                _onRewardedInterstitialAdLoadFailedEvent += value;
            }
            remove
            {
                _onRewardedInterstitialAdLoadFailedEvent -= value;
            }
        }

        public event Action<Yodo1U3dRewardedInterstitialAd> OnAdOpeningEvent
        {
            add
            {
                _onRewardedInterstitialAdOpeningEvent += value;
            }
            remove
            {
                _onRewardedInterstitialAdOpeningEvent -= value;
            }
        }

        public event Action<Yodo1U3dRewardedInterstitialAd> OnAdOpenedEvent
        {
            add
            {
                _onRewardedInterstitialAdOpenedEvent += value;
            }
            remove
            {
                _onRewardedInterstitialAdOpenedEvent -= value;
            }
        }

        public event Action<Yodo1U3dRewardedInterstitialAd, Yodo1U3dAdError> OnAdOpenFailedEvent
        {
            add
            {
                _onRewardedInterstitialAdOpenFailedEvent += value;
            }
            remove
            {
                _onRewardedInterstitialAdOpenFailedEvent -= value;
            }
        }

        public event Action<Yodo1U3dRewardedInterstitialAd> OnAdClosedEvent
        {
            add
            {
                _onRewardedInterstitialAdClosedEvent += value;
            }
            remove
            {
                _onRewardedInterstitialAdClosedEvent -= value;
            }
        }

        public event Action<Yodo1U3dRewardedInterstitialAd> OnAdEarnedEvent
        {
            add
            {
                _onRewardedInterstitialAdEarnedEvent += value;
            }
            remove
            {
                _onRewardedInterstitialAdEarnedEvent -= value;
            }
        }

        private static class HelperHolder
        {
            public static Yodo1U3dRewardedInterstitialAd Helper = new Yodo1U3dRewardedInterstitialAd();
        }

        public static Yodo1U3dRewardedInterstitialAd GetInstance()
        {
            return HelperHolder.Helper;
        }

        public static void CallbcksEvent(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError adError)
        {
            Yodo1U3dRewardedInterstitialAd.GetInstance().Callbacks(adEvent, adError);
        }

        private void Callbacks(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError adError)
        {
            switch (adEvent)
            {
                case Yodo1U3dAdEvent.AdLoaded:
                    Yodo1U3dMasCallback.InvokeEvent(_onRewardedInterstitialAdLoadedEvent, this);
                    break;
                case Yodo1U3dAdEvent.AdLoadFail:
                    Yodo1U3dMasCallback.InvokeEvent(_onRewardedInterstitialAdLoadFailedEvent, this, adError);
                    break;
                case Yodo1U3dAdEvent.AdOpening:
                    Yodo1U3dMasCallback.InvokeEvent(_onRewardedInterstitialAdOpeningEvent, this);
                    break;
                case Yodo1U3dAdEvent.AdOpened:
                    Yodo1U3dMasCallback.Instance.Pause();
                    Yodo1U3dMasCallback.InvokeEvent(_onRewardedInterstitialAdOpenedEvent, this);
                    break;
                case Yodo1U3dAdEvent.AdOpenFail:
                    Yodo1U3dMasCallback.Instance.UnPause();
                    Yodo1U3dMasCallback.InvokeEvent(_onRewardedInterstitialAdOpenFailedEvent, this, adError);
                    break;
                case Yodo1U3dAdEvent.AdClosed:
                    Yodo1U3dMasCallback.Instance.UnPause();
                    Yodo1U3dMasCallback.InvokeEvent(_onRewardedInterstitialAdClosedEvent, this);
                    break;
                case Yodo1U3dAdEvent.AdReward:
                    Yodo1U3dMasCallback.InvokeEvent(_onRewardedInterstitialAdEarnedEvent, this);
                    break;
            }
        }

        public bool autoDelayIfLoadFail = false;

        /// <summary>
        /// The default `Yodo1U3dRewardedInterstitialAd` constructor
        /// </summary>
        private Yodo1U3dRewardedInterstitialAd()
        {

        }

        private void RewardedInterstitial(string methodName)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.RewardedInterstitial(methodName, this.ToJsonString());
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.RewardedInterstitial(methodName, this.ToJsonString());
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
            Yodo1EditorAds.LoadRewardedInterstitialInEditor();
#endif
#if !UNITY_EDITOR
        RewardedInterstitial("loadRewardedInterstitialAd");
#endif
        }

        public bool IsLoaded()
        {
            return IsAdLoaded("isRewardedInterstitialAdLoaded");
        }

        /// <summary>
        /// Show reward ads
        /// </summary>
        public void ShowAd()
        {
            handleOpningEvent();
            this.adPlacement = string.Empty;
#if UNITY_EDITOR
            Yodo1EditorAds.ShowRewardedInterstitialInEditor();
#endif
#if !UNITY_EDITOR
        RewardedInterstitial("showRewardedInterstitialAd");
#endif
        }

        public void ShowAd(string placement)
        {
            handleOpningEvent();
            this.adPlacement = placement;
#if UNITY_EDITOR
            Yodo1EditorAds.ShowRewardedInterstitialInEditor();
#endif
#if !UNITY_EDITOR
        RewardedInterstitial("showRewardedInterstitialAd");
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