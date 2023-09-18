using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yodo1.MAS;

public class YodoAdsManager : MonoBehaviour
{
    public static YodoAdsManager instance;

    private void Awake()
    {
        instance = this;

        DontDestroyOnLoad(gameObject);
    }

    private void Start()
    {
        Initialize();
    }

    void Initialize()
    {
        Yodo1AdBuildConfig config = new Yodo1AdBuildConfig().enableUserPrivacyDialog(true); // this line take care of COPPA
        Yodo1U3dMas.SetAdBuildConfig(config);
        setDelegate();
        Yodo1U3dMas.InitializeSdk();

        ShowBanner();


    }

    void setDelegate()
    {
        Yodo1U3dMas.SetBannerAdDelegate((Yodo1U3dAdEvent adEvent, Yodo1U3dAdError error) =>
        {
            Debug.Log("[Yodo1 Mas] BannerdDelegate:" + adEvent.ToString() + "\n" + error.ToString());
            switch (adEvent)
            {
                case Yodo1U3dAdEvent.AdClosed:
                    Debug.Log("[Yodo1 Mas] Banner ad has been closed.");
                    break;
                case Yodo1U3dAdEvent.AdOpened:
                    Debug.Log("[Yodo1 Mas] Banner ad has been shown.");
                    break;
                case Yodo1U3dAdEvent.AdError:
                    Debug.Log("[Yodo1 Mas] Banner ad error, " + error.ToString());
                    break;
            }
        });

        Yodo1U3dMas.SetInterstitialAdDelegate((Yodo1U3dAdEvent adEvent, Yodo1U3dAdError error) =>
        {
            Debug.Log("[Yodo1 Mas] InterstitialAdDelegate:" + adEvent.ToString() + "\n" + error.ToString());
            switch (adEvent)
            {
                case Yodo1U3dAdEvent.AdClosed:
                    Debug.Log("[Yodo1 Mas] Interstital ad has been closed.");
                    break;
                case Yodo1U3dAdEvent.AdOpened:
                    Debug.Log("[Yodo1 Mas] Interstital ad has been shown.");
                    break;
                case Yodo1U3dAdEvent.AdError:
                    Debug.Log("[Yodo1 Mas] Interstital ad error, " + error.ToString());
                    break;
            }
        });

        Yodo1U3dMas.SetRewardedAdDelegate((Yodo1U3dAdEvent adEvent, Yodo1U3dAdError error) =>
        {
            Debug.Log("[Yodo1 Mas] RewardVideoDelegate:" + adEvent.ToString() + "\n" + error.ToString());
            switch (adEvent)
            {
                case Yodo1U3dAdEvent.AdClosed:
                    Debug.Log("[Yodo1 Mas] Reward video ad has been closed.");
                    break;
                case Yodo1U3dAdEvent.AdOpened:
                    Debug.Log("[Yodo1 Mas] Reward video ad has shown successful.");
                    break;
                case Yodo1U3dAdEvent.AdError:
                    Debug.Log("[Yodo1 Mas] Reward video ad error, " + error);
                    break;
                case Yodo1U3dAdEvent.AdReward:
                    Debug.Log("[Yodo1 Mas] Reward video ad reward, give rewards to the player.");
                    break;
            }
        });
    }


    public void ShowInterstitial()
    {

        // if (Yodo1U3dMas.IsInterstitialAdLoaded())
        if (PlayerPrefs.GetInt("NoAds") != 1)
        {
            Yodo1U3dMas.ShowInterstitialAd();
        }
        else
        {

        }
    }

    public void ShowRewarded()
    {

        // if (PlayerPrefs.GetInt("NoAds") != 1)
        // {
        Yodo1U3dMas.ShowRewardedAd();
        // }
    }

    public void ShowBanner()
    {
        if (PlayerPrefs.GetInt("NoAds") != 1)
        {
            Yodo1U3dMas.ShowBannerAd(Yodo1U3dBannerAlign.BannerTop | Yodo1U3dBannerAlign.BannerLeft);
        }
    }

    public void HideBanner()
    {
        Yodo1U3dMas.DismissBannerAd();
    }
}
