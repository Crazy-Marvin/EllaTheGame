using UnityEngine;
using System;
using System.Collections.Generic;
using Yodo1.MAS;

[Obsolete("Please use Yodo1U3dMas", false)]
public class Yodo1U3dSDK
{
    //ResultCode
    public const int RESULT_CODE_FAILED = 0;
    public const int RESULT_CODE_SUCCESS = 1;
    public const int RESULT_CODE_CANCEL = 2;

    public static Yodo1U3dSDK Instance { get; private set; }

    public string SdkMethodName
    {
        get
        {
            return Yodo1U3dMasCallback.Instance.SdkMethodName;
        }
    }

    public string SdkObjectName
    {
        get
        {
            return Yodo1U3dMasCallback.Instance.SdkObjectName;
        }
    }

    #region Ad Delegate

    //ShowInterstitialAd of delegate
    public delegate void InterstitialAdDelegate(Yodo1U3dConstants.AdEvent adEvent, string error);

    private static InterstitialAdDelegate _interstitialAdDelegate;

    public static void setInterstitialAdDelegate(InterstitialAdDelegate interstitialAdDelegate)
    {
        _interstitialAdDelegate = interstitialAdDelegate;

        Yodo1.MAS.Yodo1U3dMas.InterstitialAdDelegate adDelegate = new Yodo1.MAS.Yodo1U3dMas.InterstitialAdDelegate(CallbackInterstitial);
        Yodo1.MAS.Yodo1U3dMas.SetInterstitialAdDelegate(adDelegate);
    }

    //ShowBanner of delegate
    public delegate void BannerdDelegate(Yodo1U3dConstants.AdEvent adEvent, string error);

    private static BannerdDelegate _bannerDelegate;

    public static void setBannerdDelegate(BannerdDelegate bannerDelegate)
    {
        _bannerDelegate = bannerDelegate;
        Yodo1.MAS.Yodo1U3dMas.BannerdAdDelegate adDelegate = new Yodo1.MAS.Yodo1U3dMas.BannerdAdDelegate(CallbackBanner);
        Yodo1.MAS.Yodo1U3dMas.SetBannerAdDelegate(adDelegate);
    }

    //RewardVideo of delegate
    public delegate void RewardVideoDelegate(Yodo1U3dConstants.AdEvent adEvent, string error);
    private static RewardVideoDelegate _rewardVideoDelegate;

    public static void setRewardVideoDelegate(RewardVideoDelegate rewardVideoDelegate)
    {
        _rewardVideoDelegate = rewardVideoDelegate;
        Yodo1.MAS.Yodo1U3dMas.RewardedAdDelegate adDelegate = new Yodo1.MAS.Yodo1U3dMas.RewardedAdDelegate(CallbackVideo);
        Yodo1.MAS.Yodo1U3dMas.SetRewardedAdDelegate(adDelegate);
    }
    #endregion

    public void Awake()
    {
        Yodo1U3dMasCallback.Instance.Awake();
    }

    static void CallbackBanner(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError error)
    {
        if (_bannerDelegate != null) {
            switch (adEvent) {
                case Yodo1U3dAdEvent.AdOpened:
                _bannerDelegate(Yodo1U3dConstants.AdEvent.AdEventShowSuccess, null);
                break;
                case Yodo1U3dAdEvent.AdClosed:
                _bannerDelegate(Yodo1U3dConstants.AdEvent.AdEventClose, null);
                break;
                case Yodo1U3dAdEvent.AdError:
                _bannerDelegate(Yodo1U3dConstants.AdEvent.AdEventShowFail, error.ToString());
                break;
            }
        }
    }

    static void CallbackInterstitial(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError error)
    {
        if (_interstitialAdDelegate != null) {
            switch (adEvent) {
                case Yodo1U3dAdEvent.AdOpened:
                _interstitialAdDelegate(Yodo1U3dConstants.AdEvent.AdEventShowSuccess, null);
                break;
                case Yodo1U3dAdEvent.AdClosed:
                _interstitialAdDelegate(Yodo1U3dConstants.AdEvent.AdEventClose, null);
                break;
                case Yodo1U3dAdEvent.AdError:
                _interstitialAdDelegate(Yodo1U3dConstants.AdEvent.AdEventShowFail, error.ToString());
                break;
            }
        }
    }

    static void CallbackVideo(Yodo1U3dAdEvent adEvent, Yodo1U3dAdError error) 
    {
        if (_rewardVideoDelegate != null) {
            switch (adEvent) {
                case Yodo1U3dAdEvent.AdOpened:
                _rewardVideoDelegate(Yodo1U3dConstants.AdEvent.AdEventShowSuccess, null);
                break;
                case Yodo1U3dAdEvent.AdClosed:
                _rewardVideoDelegate(Yodo1U3dConstants.AdEvent.AdEventClose, null);
                break;
                case Yodo1U3dAdEvent.AdError:
                _rewardVideoDelegate(Yodo1U3dConstants.AdEvent.AdEventShowFail, error.ToString());
                break;
                case Yodo1U3dAdEvent.AdReward:
                _rewardVideoDelegate(Yodo1U3dConstants.AdEvent.AdEventFinish, null);
                break;
            }
        }
    }
}
