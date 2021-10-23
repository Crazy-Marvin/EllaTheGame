using UnityEngine;
using System.Collections;
using System.Runtime.InteropServices;
using Yodo1.MAS;

public class Yodo1U3dAdsIOS
{
#if UNITY_IPHONE
    private const string LIB_NAME = "__Internal";//对外扩展接口的库名

    /// <summary>
    /// Unity3ds the set user consent.
    /// </summary>
    /// <param name="consent">If set to <c>true</c> consent.</param>
    [DllImport(LIB_NAME)]
    private static extern void UnityMasSetUserConsent(bool consent);
    public static void SetUserConsent(bool consent)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UnityMasSetUserConsent(consent);
        }
    }

    /// <summary>
    /// Unity3ds the set tag for under age of consent.
    /// </summary>
    /// <param name="isBelowConsentAge">If set to <c>true</c> is below consent age.</param>
    [DllImport(LIB_NAME)]
    private static extern void UnityMasSetTagForUnderAgeOfConsent(bool isBelowConsentAge);
    public static void SetTagForUnderAgeOfConsent(bool isBelowConsentAge)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UnityMasSetTagForUnderAgeOfConsent(isBelowConsentAge);
        }
    }

    /// <summary>
    /// Unity3ds the set do not sell.
    /// </summary>
    /// <param name="doNotSell"></param>
    [DllImport(LIB_NAME)]
    private static extern void UnityMasSetDoNotSell(bool doNotSell);
    public static void SetDoNotSell(bool doNotSell)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UnityMasSetDoNotSell(doNotSell);
        }
    }

    /// <summary>
    /// 初始化SDK
    /// </summary>
    [DllImport(LIB_NAME)]
    private static extern void UnityMasInitWithAppKey(string appKey, string gameObject, string methodName);

    public static void InitWithAppKey(string appKey)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UnityMasInitWithAppKey(appKey, Yodo1U3dMasCallback.Instance.SdkObjectName, Yodo1U3dMasCallback.Instance.SdkMethodName);
        }
    }

    /// <summary>
    /// 设置广告配置
    /// </summary>
    [DllImport(LIB_NAME)]
    private static extern void UnitySetAdBuildConfig(string config);
    public static void SetAdBuildConfig(string config)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UnitySetAdBuildConfig(config);
        }
    }

    #region  Banner
    /// <summary>
    /// 检查Banner 是否缓存好
    /// </summary>
    /// <returns></returns>
    [DllImport(LIB_NAME)]
    private static extern bool UnityIsBannerAdLoaded();
    public static bool IsBannerAdLoaded()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return UnityIsBannerAdLoaded();
        }
        return false;
    }

    [DllImport(LIB_NAME)]
    private static extern void UnityShowBannerAd();
    public static void ShowBannerAd()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UnityShowBannerAd();
        }
    }

    [DllImport(LIB_NAME)]
    private static extern void UnityShowBannerAdWithPlacement(string placement);
    public static void ShowBannerAd(string placement)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UnityShowBannerAdWithPlacement(placement);
        }
    }

    /// <summary>
    /// 显示广告
    /// </summary>
    [DllImport(LIB_NAME)]
    private static extern void UnityShowBannerAdWithAlign(int align);
    public static void ShowBannerAd(int align)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UnityShowBannerAdWithAlign(align);
        }
    }

    /// <summary>
    /// 显示广告
    /// </summary>
    [DllImport(LIB_NAME)]
    private static extern void UnityShowBannerAdWithAlignAndOffset(int align, int offsetX, int offsetY);
    public static void ShowBannerAd(int align, int offsetX, int offsetY)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UnityShowBannerAdWithAlignAndOffset(align, offsetX, offsetY);
        }
    }

    [DllImport(LIB_NAME)]
    private static extern void UnityShowBannerAdWithPlacementAndAlignAndOffset(string placement, int align, int offsetX, int offsetY);
    public static void ShowBannerAd(string placement, int align, int offsetX, int offsetY)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UnityShowBannerAdWithPlacementAndAlignAndOffset(placement, align, offsetX, offsetY);
        }
    }


    /// <summary>
    /// dismiss banner ad
    /// </summary>
    [DllImport(LIB_NAME)]
    private static extern void UnityDismissBannerAd();
    public static void DismissBannerAd()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UnityDismissBannerAd();
        }
    }

    /// <summary>
    /// dismiss banner ad
    /// </summary>
    [DllImport(LIB_NAME)]
    private static extern void UnityDismissBannerAdWithDestroy(bool destroy);
    public static void DismissBannerAd(bool destroy)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UnityDismissBannerAdWithDestroy(destroy);
        }
    }

    #endregion

    #region  Interstitial

    /// <summary>
    /// 插屏广告是否可以播放
    /// </summary>
    /// <returns><c>true</c>, if switch full screen ad was unityed, <c>false</c> otherwise.</returns>
    [DllImport(LIB_NAME)]
    private static extern bool UnityIsInterstitialLoaded();

    public static bool IsInterstitialLoaded()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return UnityIsInterstitialLoaded();
        }
        return false;
    }


    [DllImport(LIB_NAME)]
    private static extern void UnityShowInterstitialAd();
    /// <summary>
    /// 显示插屏广告
    /// </summary>
    public static void ShowInterstitialAd()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UnityShowInterstitialAd();
        }
    }

    [DllImport(LIB_NAME)]
    private static extern void UnityShowInterstitialAdWithPlacementId(string placementId);
    /// <summary>
    /// 显示插屏广告
    /// </summary>
    public static void ShowInterstitialAd(string placementId)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UnityShowInterstitialAdWithPlacementId(placementId);
        }
    }

    #endregion

    #region  Video

    [DllImport(LIB_NAME)]
    private static extern bool UnityIsRewardedAdLoaded();
    /// <summary>
    /// Video是否已经准备好
    /// </summary>
    /// <returns><c>true</c>, if switch ad video was unityed, <c>false</c> otherwise.</returns>
    public static bool IsRewardedAdLoaded()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return UnityIsRewardedAdLoaded();
        }
        return false;
    }

    [DllImport(LIB_NAME)]
    private static extern void UnityShowRewardedAd();
    /// <summary>
    /// 显示Video广告
    /// </summary>
    public static void ShowRewardedAd()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UnityShowRewardedAd();
        }
    }

    [DllImport(LIB_NAME)]
    private static extern void UnityShowRewardedAdWithPlacementId(string placementId);
    /// <summary>
    /// 显示Video广告
    /// </summary>
    public static void ShowRewardedAd(string placementId)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UnityShowRewardedAdWithPlacementId(placementId);
        }
    }

    #endregion

#endif
}
