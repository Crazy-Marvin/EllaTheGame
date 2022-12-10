using UnityEngine;
using System;
using System.Collections.Generic;

namespace Yodo1.MAS
{
    public class Yodo1U3dRewardAd
    {
        private string adPlacement = string.Empty;

        private Action<Yodo1U3dRewardAd> _onRewardAdLoadedEvent;
        private Action<Yodo1U3dRewardAd, Yodo1U3dAdError> _onRewardAdLoadFailedEvent;
        private Action<Yodo1U3dRewardAd> _onRewardAdOpenedEvent;
        private Action<Yodo1U3dRewardAd, Yodo1U3dAdError> _onRewardAdOpenFailedEvent;
        private Action<Yodo1U3dRewardAd> _onRewardAdClosedEvent;
        private Action<Yodo1U3dRewardAd> _onRewardAdEarnedEvent;

        public event Action<Yodo1U3dRewardAd> OnAdLoadedEvent
        {
            add
            {
                _onRewardAdLoadedEvent += value;
            }
            remove
            {
                _onRewardAdLoadedEvent -= value;
            }
        }

        public event Action<Yodo1U3dRewardAd, Yodo1U3dAdError> OnAdLoadFailedEvent
        {
            add
            {
                _onRewardAdLoadFailedEvent += value;
            }
            remove
            {
                _onRewardAdLoadFailedEvent -= value;
            }
        }

        public event Action<Yodo1U3dRewardAd> OnAdOpenedEvent
        {
            add
            {
                _onRewardAdOpenedEvent += value;
            }
            remove
            {
                _onRewardAdOpenedEvent -= value;
            }
        }

        public event Action<Yodo1U3dRewardAd, Yodo1U3dAdError> OnAdOpenFailedEvent
        {
            add
            {
                _onRewardAdOpenFailedEvent += value;
            }
            remove
            {
                _onRewardAdOpenFailedEvent -= value;
            }
        }

        public event Action<Yodo1U3dRewardAd> OnAdClosedEvent
        {
            add
            {
                _onRewardAdClosedEvent += value;
            }
            remove
            {
                _onRewardAdClosedEvent -= value;
            }
        }

        public event Action<Yodo1U3dRewardAd> OnAdEarnedEvent
        {
            add
            {
                _onRewardAdEarnedEvent += value;
            }
            remove
            {
                _onRewardAdEarnedEvent -= value;
            }
        }

        private static class HelperHolder
        {
            public static Yodo1U3dRewardAd Helper = new Yodo1U3dRewardAd();
        }

        public static Yodo1U3dRewardAd GetInstance()
        {
            return HelperHolder.Helper;
        }

        public static void CallbcksEvent(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError adError)
        {
            Yodo1U3dRewardAd.GetInstance().Callbacks(adEvent, adError);
        }

        private void Callbacks(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError adError)
        {
            switch (adEvent)
            {
                case Yodo1U3dAdEvent.AdLoaded:
                    Yodo1U3dMasCallback.InvokeEvent(_onRewardAdLoadedEvent, this);
                    break;
                case Yodo1U3dAdEvent.AdLoadFail:
                    Yodo1U3dMasCallback.InvokeEvent(_onRewardAdLoadFailedEvent, this, adError);
                    break;
                case Yodo1U3dAdEvent.AdOpened:
                    Yodo1U3dMasCallback.Instance.Pause();
                    Yodo1U3dMasCallback.InvokeEvent(_onRewardAdOpenedEvent, this);
                    break;
                case Yodo1U3dAdEvent.AdOpenFail:
                    Yodo1U3dMasCallback.Instance.UnPause();
                    Yodo1U3dMasCallback.InvokeEvent(_onRewardAdOpenFailedEvent, this, adError);
                    break;
                case Yodo1U3dAdEvent.AdClosed:
                    Yodo1U3dMasCallback.Instance.UnPause();
                    Yodo1U3dMasCallback.InvokeEvent(_onRewardAdClosedEvent, this);
                    break;
                case Yodo1U3dAdEvent.AdReward:
                    Yodo1U3dMasCallback.InvokeEvent(_onRewardAdEarnedEvent, this);
                    break;
            }
        }

        public bool autoDelayIfLoadFail = false;

        /// <summary>
        /// The default `Yodo1U3dRewardAd` constructor
        /// </summary>
        private Yodo1U3dRewardAd()
        {

        }

        private void RewardV2(string methodName)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.RewardV2(methodName, this.ToJsonString());
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.RewardV2(methodName, this.ToJsonString());
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

        private void SetAdPlacement(string adPlacement)
        {
            this.adPlacement = adPlacement;
        }

        public void LoadAd()
        {
#if UNITY_EDITOR
            Yodo1EditorAds.LoadRewardedVideodsInEditor();
#endif
#if !UNITY_EDITOR
            RewardV2("loadRewardAdV2");
#endif
        }

        public bool IsLoaded()
        {
            return IsAdLoadedV2("isRewardedAdLoadedV2");
        }

        /// <summary>
        /// Show reward ads
        /// </summary>
        public void ShowAd()
        {
            this.adPlacement = string.Empty;
#if UNITY_EDITOR
            Yodo1EditorAds.ShowRewardedVideodsInEditor();
#endif
#if !UNITY_EDITOR
            RewardV2("showRewardAdV2");
#endif
        }

        public void ShowAd(string placement)
        {
            this.adPlacement = placement;
#if UNITY_EDITOR
            Yodo1EditorAds.ShowRewardedVideodsInEditor();
#endif
#if !UNITY_EDITOR
            RewardV2("showRewardAdV2");
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