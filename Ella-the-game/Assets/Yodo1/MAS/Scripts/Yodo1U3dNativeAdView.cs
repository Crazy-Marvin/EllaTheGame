using UnityEngine;
using System;
using System.Collections.Generic;

namespace Yodo1.MAS
{
    public enum Yodo1U3dNativeAdPosition
    {
        NativeNone = 0,
        NativeLeft = 1,
        NativeHorizontalCenter = 1 << 1,
        NativeRight = 1 << 2,
        NativeTop = 1 << 3,
        NativeVerticalCenter = 1 << 4,
        NativeBottom = 1 << 5,
    }
    public class Yodo1U3dNativeAdView
    {
        private static List<Yodo1U3dNativeAdView> NativeAdViews = new List<Yodo1U3dNativeAdView>();
        private readonly string indexId = ((DateTime.Now.ToUniversalTime().Ticks - 621355968000000000) / 10000) + NativeAdViews.Count + "";
        private string adPlacement = string.Empty;
        private string customData = string.Empty;
        private Yodo1U3dNativeAdPosition adPosition = Yodo1U3dNativeAdPosition.NativeNone;
        private int adOffsetX = 0;
        private int adOffsetY = 0;
        private int adPositionX = 0;
        private int adPositionY = 0;
        private int adWidth = -1;
        private int adHeight = -1;
        private Color backgroundColor;

        private Action<Yodo1U3dNativeAdView> _onAdLoadedEvent;
        private Action<Yodo1U3dNativeAdView, Yodo1U3dAdError> _onAdFailedToLoadEvent;
        private Action<Yodo1U3dNativeAdView, Yodo1U3dAdValue> _onAdPayRevenueEvent;

        public event Action<Yodo1U3dNativeAdView> OnAdLoadedEvent
        {
            add
            {
                _onAdLoadedEvent += value;
            }
            remove
            {
                _onAdLoadedEvent -= value;
            }
        }

        public event Action<Yodo1U3dNativeAdView, Yodo1U3dAdError> OnAdFailedToLoadEvent
        {
            add
            {
                _onAdFailedToLoadEvent += value;
            }
            remove
            {
                _onAdFailedToLoadEvent -= value;
            }
        }

        public event Action<Yodo1U3dNativeAdView, Yodo1U3dAdValue> OnAdPayRevenueEvent
        {
            add
            {
                _onAdPayRevenueEvent += value;
            }
            remove
            {
                _onAdPayRevenueEvent -= value;
            }
        }

        public static void CallbcksEvent(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError adError, string indexId, Yodo1U3dAdValue adValue)
        {
            if (string.IsNullOrEmpty(indexId))
            {
                return;
            }

            foreach (Yodo1U3dNativeAdView nativeAdView in Yodo1U3dNativeAdView.NativeAdViews)
            {
                if (nativeAdView != null && indexId.Equals(nativeAdView.indexId))
                {
                    nativeAdView.Callbacks(adEvent, adError, adValue);
                }
            }
        }

        private void Callbacks(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError adError, Yodo1U3dAdValue adValue)
        {
            switch (adEvent)
            {
                case Yodo1U3dAdEvent.AdError:
                    break;
                case (Yodo1U3dAdEvent)1003:
                    Yodo1U3dMasCallback.InvokeEvent(_onAdLoadedEvent, this);
                    break;
                case (Yodo1U3dAdEvent)1004:
                    Yodo1U3dMasCallback.InvokeEvent(_onAdFailedToLoadEvent, this, adError);
                    break;
                case Yodo1U3dAdEvent.AdPayRevenue:
                    Yodo1U3dMasCallback.InvokeEvent(_onAdPayRevenueEvent, this, adValue);
                    break;
            }
        }

        /// <summary>
        /// Custom ad position.
        /// For greater control over where a `Yodo1U3dNativeAdView` is placed on screen than what's offered by `Yodo1U3dNativeAdPosition` values,
        /// use the `Yodo1U3dNativeAdView` constructor that has x- and y-coordinates as parameters.
        ///
        /// The top-left corner of the `Yodo1U3dNativeAdView` will be positioned at the x and y values passed to the constructor, where the origin is the top-left of the screen.
        /// </summary>
        /// <param name="x">X-coordinates in pixels</param>
        /// <param name="y">Y-coordinates in pixels</param>
        /// <param name="width">width in pixels</param>
        /// <param name="height">height in pixels</param>
        public Yodo1U3dNativeAdView(int x, int y, int width, int height)
        {
            this.adPosition = Yodo1U3dNativeAdPosition.NativeNone;
            this.adOffsetX = 0;
            this.adOffsetY = 0;
            this.adPositionX = x;
            this.adPositionY = y;
            this.adWidth = width;
            this.adHeight = height;
            NativeAdViews.Add(this);
        }

        public Yodo1U3dNativeAdView(Yodo1U3dNativeAdPosition position, int width, int height)
        {
            this.adPosition = position;
            this.adOffsetX = 0;
            this.adOffsetY = 0;
            this.adPositionX = 0;
            this.adPositionY = 0;
            this.adWidth = width;
            this.adHeight = height;
            NativeAdViews.Add(this);
        }

        public Yodo1U3dNativeAdView(Yodo1U3dNativeAdPosition position, int offsetX, int offsetY, int width, int height)
        {
            this.adPosition = position;
            this.adOffsetX = offsetX;
            this.adOffsetY = offsetY;
            this.adPositionX = 0;
            this.adPositionY = 0;
            this.adWidth = width;
            this.adHeight = height;
            NativeAdViews.Add(this);
        }

        private void Native(string methodName)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                Yodo1U3dAdsIOS.Native(methodName, this.ToJsonString());
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                Yodo1U3dAdsAndroid.Native(methodName, this.ToJsonString());
#endif
            }
        }

        /// <summary>
        /// Load native ads, the native ad will be displayed automatically after loaded
        /// </summary>
        public void LoadAd()
        {
#if UNITY_EDITOR
            if (adPosition == Yodo1U3dNativeAdPosition.NativeNone)
            {
                Yodo1EditorAds.ShowNativeAdsInEditor(indexId, adWidth, adHeight, adPositionX, adPositionY, backgroundColor);
            }
            else
            {
                Yodo1EditorAds.ShowNativeAdsInEditor(indexId, (int)adPosition, adWidth, adHeight, adOffsetX, adOffsetY, backgroundColor);
            }
#endif
#if !UNITY_EDITOR
            Native("loadNativeAd");
#endif
        }

        /// <summary>
        /// Show native ads
        /// </summary>
        public void Show()
        {
#if UNITY_EDITOR
            Yodo1EditorAds.ShowNativeAdsInEditor(indexId, (int)adPosition, adWidth, adHeight, adPositionX, adPositionY, backgroundColor);
#endif
#if !UNITY_EDITOR
            Native("showNativeAd");
#endif
        }

        /// <summary>
        /// Hide native ads
        /// </summary>
        public void Hide()
        {
#if UNITY_EDITOR
            Yodo1EditorAds.HideNativeAdsInEditor(indexId);
#endif
#if !UNITY_EDITOR
            Native("hideNativeAd");
#endif
        }

        /// <summary>
        /// Destroy native ads
        /// </summary>
        public void Destroy()
        {
#if UNITY_EDITOR
            Yodo1EditorAds.DestroyNativeAdsInEditor(indexId);
#endif
            Native("destroyNativeAd");
            NativeAdViews.Remove(this);
        }

        public void SetAdPlacement(string adPlacement)
        {
            this.adPlacement = adPlacement;
        }

        public void SetCustomData(string customData)
        {
            this.customData = customData;
        }

        public void SetBackgroundColor(Color backgroundColor)
        {
            this.backgroundColor = backgroundColor;
        }

        public string ToJsonString()
        {
            Dictionary<string, object> dic = new Dictionary<string, object>();
            dic.Add("position", (int)this.adPosition);
            dic.Add("offsetX", this.adOffsetX);
            dic.Add("offsetY", this.adOffsetY);
            dic.Add("x", this.adPositionX);
            dic.Add("y", this.adPositionY);
            dic.Add("width", this.adWidth);
            dic.Add("height", this.adHeight);
            dic.Add("indexId", this.indexId);
            if (string.IsNullOrEmpty(this.adPlacement))
            {
                dic.Add("adPlacement", "");
            }
            else
            {
                dic.Add("adPlacement", this.adPlacement);
            }

            if (string.IsNullOrEmpty(this.customData))
            {
                dic.Add("customData", "");
            }
            else
            {
                dic.Add("customData", this.customData);
            }
            if (!this.backgroundColor.Equals(Color.clear))
            {
                dic.Add("backgroundColor", "#" + ColorUtility.ToHtmlStringRGB(this.backgroundColor));
            }
            if (_onAdPayRevenueEvent == null)
            {
                dic.Add("payRevenueEventCount", 0);
            }
            else
            {
                dic.Add("payRevenueEventCount", _onAdPayRevenueEvent.GetInvocationList().Length);
            }
            return Yodo1JSON.Serialize(dic);
        }
    }
}