using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Yodo1.MAS
{
    public class Yodo1U3dBannerAdSize
    {
        public enum Type
        {
            Banner = 0, // 320x50
            LargeBanner = 1, //320x100
            IABMediumRectangle = 2, //300x250
            SmartBanner = 3,
            AdaptiveBanner = 4,
        }

        private Type type;
        private int width;
        private int height;

        public static readonly Yodo1U3dBannerAdSize Banner = new Yodo1U3dBannerAdSize(320, 50, Type.Banner);
        public static readonly Yodo1U3dBannerAdSize LargeBanner = new Yodo1U3dBannerAdSize(320, 100, Type.LargeBanner);
        public static readonly Yodo1U3dBannerAdSize IABMediumRectangle = new Yodo1U3dBannerAdSize(300, 250, Type.IABMediumRectangle);
        public static readonly Yodo1U3dBannerAdSize SmartBanner = new Yodo1U3dBannerAdSize(0, 0, Type.SmartBanner);
        public static readonly Yodo1U3dBannerAdSize AdaptiveBanner = new Yodo1U3dBannerAdSize(0, 0, Type.AdaptiveBanner);

        public int Width => width;

        public int Height => height;

        public Type AdType => type;

        private Yodo1U3dBannerAdSize(int width, int height, Type type)
        {
            this.width = GetWidth((int)type);
            this.height = GetHeight((int)type);
            this.type = type;
        }

        private int GetHeight(int type)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                return (int)Yodo1U3dAdsIOS.GetBannerHeightInPixels(type);
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                return (int)Yodo1U3dAdsAndroid.GetBannerHeightInPixels(type);
#endif
            }
            return 0;
        }

        private int GetWidth(int type)
        {
            if (Application.platform == RuntimePlatform.IPhonePlayer)
            {
#if UNITY_IPHONE
                return (int)Yodo1U3dAdsIOS.GetBannerWidthInPixels(type);
#endif
            }
            else if (Application.platform == RuntimePlatform.Android)
            {
#if UNITY_ANDROID
                return (int)Yodo1U3dAdsAndroid.GetBannerWidthInPixels(type);
#endif
            }
            return 0;
        }
    }
}