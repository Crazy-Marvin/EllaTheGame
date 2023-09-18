using UnityEngine;
using System;

[Obsolete("Please use Yodo1U3dMas", false)]
public class Yodo1U3dAds {

    public static void InitializeSdk() 
    {
        Yodo1.MAS.Yodo1U3dMas.InitializeSdk();
    }

    public static void SetUserConsent(bool consent) 
    {
        Yodo1.MAS.Yodo1U3dMas.SetGDPR(consent);
    }

    public static void SetTagForUnderAgeOfConsent(bool underAgeOfConsent) 
    {
        Yodo1.MAS.Yodo1U3dMas.SetCOPPA(underAgeOfConsent);
    }

    public static void SetDoNotSell(bool doNotSell)
    {
        Yodo1.MAS.Yodo1U3dMas.SetCCPA(doNotSell);
    }

    #region Banner
    private static int bannerAlign = (int)(Yodo1U3dConstants.BannerAdAlign.BannerAdAlignBotton | Yodo1U3dConstants.BannerAdAlign.BannerAdAlignHorizontalCenter);
    private static int bannerX = 0;
    private static int bannerY = 0;

    public static void SetBannerAlign(Yodo1U3dConstants.BannerAdAlign align) 
    {
        bannerAlign = (int)align;
    }

    public static void SetBannerOffset(float x, float y) 
    {
        bannerX = (int)x;
        bannerY = (int)y;
    }

    public static bool BannerIsReady() 
    {
        return Yodo1.MAS.Yodo1U3dMas.IsBannerAdLoaded();
    }

    public static void ShowBanner() 
    {
        Yodo1.MAS.Yodo1U3dMas.ShowBannerAd(bannerAlign, bannerX, bannerY);
    }

    public static void ShowBanner(string placementId) 
    {
        Yodo1.MAS.Yodo1U3dMas.ShowBannerAd(placementId, bannerAlign, bannerX, bannerY);
    }

    public static void HideBanner()
    {
        Yodo1.MAS.Yodo1U3dMas.DismissBannerAd();
    }

    public static void RemoveBanner()
    {
        Yodo1.MAS.Yodo1U3dMas.DismissBannerAd(true);
    }

    #endregion


    #region Interstitial
    public static bool InterstitialIsReady()
    {
      
        return Yodo1.MAS.Yodo1U3dMas.IsInterstitialAdLoaded();
    }

    public static void ShowInterstitial()
    {
        Yodo1.MAS.Yodo1U3dMas.ShowInterstitialAd();
    }

    public static void ShowInterstitial(string placementId)
    {
        Yodo1.MAS.Yodo1U3dMas.ShowInterstitialAd(placementId);
    }
    #endregion

    #region Video
    public static bool VideoIsReady()
    {
        return Yodo1.MAS.Yodo1U3dMas.IsRewardedAdLoaded();
    }

    public static void ShowVideo()
    {
        Yodo1.MAS.Yodo1U3dMas.ShowRewardedAd();
    }

    public static void ShowVideo(string placementId)
    {
        Yodo1.MAS.Yodo1U3dMas.ShowRewardedAd(placementId);
    }
    #endregion
}