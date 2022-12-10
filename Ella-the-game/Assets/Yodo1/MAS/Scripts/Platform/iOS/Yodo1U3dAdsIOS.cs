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

    [DllImport(LIB_NAME)]
    private static extern bool UnityMasIsUserConsent();
    public static bool IsGDPRUserConsent()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return UnityMasIsUserConsent();
        }
        return false;
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

    [DllImport(LIB_NAME)]
    private static extern bool UnityMasIsTagForUnderAgeOfConsent();
    public static bool IsCOPPAAgeRestricted()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return UnityMasIsTagForUnderAgeOfConsent();
        }
        return false;
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

    [DllImport(LIB_NAME)]
    private static extern bool UnityMasIsDoNotSell();
    public static bool IsCCPADoNotSell()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return UnityMasIsDoNotSell();
        }
        return false;
    }

    [DllImport(LIB_NAME)]
    private static extern int UnityMasUserAge();
    public static int GetUserAge()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return UnityMasUserAge();
        }
        return 0;
    }

    [DllImport(LIB_NAME)]
    private static extern int UnityMasAttrackingStatus();
    public static int GetAttrackingStatus()
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return UnityMasAttrackingStatus();
        }
        return 0;
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

    [DllImport(LIB_NAME)]
    private static extern void UnityMasInitMasWithAppKey(string appKey, string gameObject, string methodName);

    public static void InitMasWithAppKey(string appKey)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            UnityMasInitMasWithAppKey(appKey, Yodo1U3dMasCallback.Instance.SdkObjectName, Yodo1U3dMasCallback.Instance.SdkMethodName);
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

    [DllImport(LIB_NAME)]
    private static extern void UnityLoadBannerAdV2(string param);
    [DllImport(LIB_NAME)]
    private static extern void UnityShowBannerAdV2(string param);
    [DllImport(LIB_NAME)]
    private static extern void UnityHideBannerAdV2(string param);
    [DllImport(LIB_NAME)]
    private static extern void UnityDestroyBannerAdV2(string param);

    public static void BannerV2(string methodName, string param)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (methodName.Equals("loadBannerAdV2"))
            {
                UnityLoadBannerAdV2(param);
            }
            if (methodName.Equals("showBannerAdV2"))
            {
                UnityShowBannerAdV2(param);
            }
            if (methodName.Equals("hideBannerAdV2"))
            {
                UnityHideBannerAdV2(param);
            }
            if (methodName.Equals("destroyBannerAdV2"))
            {
                UnityDestroyBannerAdV2(param);
            }
        }
    }

    [DllImport(LIB_NAME)]
    private static extern int UnityGetBannerHeightV2(int type);
    public static int GetBannerHeight(int type)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return UnityGetBannerHeightV2(type);
        }
        return 0;
    }

    [DllImport(LIB_NAME)]
    private static extern int UnityGetBannerWidthV2(int type);
    public static int GetBannerWidth(int type)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return UnityGetBannerWidthV2(type);
        }
        return 0;
    }

    [DllImport(LIB_NAME)]
    private static extern float UnityGetBannerHeightInPixelsV2(int type);
    public static float GetBannerHeightInPixels(int type)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return UnityGetBannerHeightInPixelsV2(type);
        }
        return 0;
    }

    [DllImport(LIB_NAME)]
    private static extern float UnityGetBannerWidthInPixelsV2(int type);
    public static float GetBannerWidthInPixels(int type)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            return UnityGetBannerWidthInPixelsV2(type);
        }
        return 0;
    }

    #endregion

    #region  Native
    [DllImport(LIB_NAME)]
    private static extern void UnityLoadNativeAd(string param);
    [DllImport(LIB_NAME)]
    private static extern void UnityShowNativeAd(string param);
    [DllImport(LIB_NAME)]
    private static extern void UnityHideNativeAd(string param);
    [DllImport(LIB_NAME)]
    private static extern void UnityDestroyNativeAd(string param);
    public static void Native(string methodName, string param)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (methodName.Equals("loadNativeAd"))
            {
                UnityLoadNativeAd(param);
            }
            if (methodName.Equals("showNativeAd"))
            {
                UnityShowNativeAd(param);
            }
            if (methodName.Equals("hideNativeAd"))
            {
                UnityHideNativeAd(param);
            }
            if (methodName.Equals("destroyNativeAd"))
            {
                UnityDestroyNativeAd(param);
            }
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

    [DllImport(LIB_NAME)]
    private static extern void UnityLoadInterstitialAdV2(string param);
    [DllImport(LIB_NAME)]
    private static extern void UnityShowInterstitialAdV2(string param);
    [DllImport(LIB_NAME)]
    private static extern void UnityDestroyInterstitialAdV2(string param);
    [DllImport(LIB_NAME)]
    private static extern bool UnityIsInterstitialLoadedV2(string param);
    public static void InterstitialV2(string methodName, string param)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (methodName.Equals("loadInterstitialAdV2"))
            {
                UnityLoadInterstitialAdV2(param);
            }
            if (methodName.Equals("showInterstitialAdV2"))
            {
                UnityShowInterstitialAdV2(param);
            }
            if (methodName.Equals("destroyInterstitialAdV2"))
            {
                UnityDestroyInterstitialAdV2(param);
            }
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


    [DllImport(LIB_NAME)]
    private static extern void UnityLoadRewardAdV2(string param);
    [DllImport(LIB_NAME)]
    private static extern void UnityShowRewardAdV2(string param);
    [DllImport(LIB_NAME)]
    private static extern void UnityDestroyRewardAdV2(string param);
    [DllImport(LIB_NAME)]
    private static extern bool UnityIsRewardedAdLoadedV2(string param);
    public static void RewardV2(string methodName, string param)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (methodName.Equals("loadRewardAdV2"))
            {
                UnityLoadRewardAdV2(param);
            }
            if (methodName.Equals("showRewardAdV2"))
            {
                UnityShowRewardAdV2(param);
            }
            if (methodName.Equals("destroyRewardAdV2"))
            {
                UnityDestroyRewardAdV2(param);
            }
        }
    }


    public static bool IsAdLoadedV2(string methodName, string param)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (methodName.Equals("isInterstitialAdLoadedV2"))
            {
                return UnityIsInterstitialLoadedV2(param);
            }
            if (methodName.Equals("isRewardedAdLoadedV2"))
            {
                return UnityIsRewardedAdLoadedV2(param);
            }
            if (methodName.Equals("isRewardedInterstitialAdLoaded"))
            {
                return UnityIsRewardedInterstitialAdLoaded(param);
            }
            if (methodName.Equals("isAppOpenAdLoaded"))
            {
                return UnityIsAppOpenAdLoaded(param);
            }
        }
        return false;
    }

    #endregion


    #region  AppOpen
    [DllImport(LIB_NAME)]
    private static extern void UnityLoadAppOpenAd(string param);
    [DllImport(LIB_NAME)]
    private static extern void UnityShowAppOpenAd(string param);
    [DllImport(LIB_NAME)]
    private static extern void UnityDestroyAppOpenAd(string param);
    [DllImport(LIB_NAME)]
    private static extern bool UnityIsAppOpenAdLoaded(string param);
    public static void AppOpen(string methodName, string param)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (methodName.Equals("loadAppOpenAd"))
            {
                UnityLoadAppOpenAd(param);
            }
            if (methodName.Equals("showAppOpenAd"))
            {
                UnityShowAppOpenAd(param);
            }
            if (methodName.Equals("destroyAppOpenAd"))
            {
                UnityDestroyAppOpenAd(param);
            }
        }
    }
    #endregion

    #region  RewardedInterstitial
    [DllImport(LIB_NAME)]
    private static extern void UnityLoadRewardedInterstitialAd(string param);
    [DllImport(LIB_NAME)]
    private static extern void UnityShowRewardedInterstitialAd(string param);
    [DllImport(LIB_NAME)]
    private static extern void UnityDestroyRewardedInterstitialAd(string param);
    [DllImport(LIB_NAME)]
    private static extern bool UnityIsRewardedInterstitialAdLoaded(string param);
    public static void RewardedInterstitial(string methodName, string param)
    {
        if (Application.platform == RuntimePlatform.IPhonePlayer)
        {
            if (methodName.Equals("loadRewardedInterstitialAd"))
            {
                UnityLoadRewardedInterstitialAd(param);
            }
            if (methodName.Equals("showRewardedInterstitialAd"))
            {
                UnityShowRewardedInterstitialAd(param);
            }
            if (methodName.Equals("destroyRewardedInterstitialAd"))
            {
                UnityDestroyRewardedInterstitialAd(param);
            }
        }
    }
    #endregion
#endif
}
