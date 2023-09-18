using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Yodo1.MAS;

public class YodoAdsManager : MonoBehaviour
{
    public static YodoAdsManager instance;
    private Yodo1U3dBannerAdView bannerAdView;

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
        Yodo1AdBuildConfig config =
 new Yodo1AdBuildConfig().enableUserPrivacyDialog(true);
        Yodo1U3dMas.SetAdBuildConfig(config);
        Yodo1U3dInterstitialAd.GetInstance().autoDelayIfLoadFail = true;
        Yodo1U3dRewardAd.GetInstance().autoDelayIfLoadFail = true;
        this.InitializeInterstitial();
        this.RequestInterstitial();
        this.InitializeRewardedAds();
        this.RequestRewardedAds();
        Yodo1U3dMasCallback.OnSdkInitializedEvent += (success, error) =>
        {
            Debug.Log("[Yodo1 Mas] OnSdkInitializedEvent, success:" + success + ", error: " + error.ToString());
            if (success)
            {
                this.RequestBanner();
                Debug.Log("[Yodo1 Mas] The initialization has succeeded");
            }
            else
            {
                Debug.Log("[Yodo1 Mas] The initialization has failed");
            }
        };
        Yodo1U3dMas.InitializeMasSdk();
        ShowBanner();


    }

    private void RequestBanner()
    {
        if (PlayerPrefs.GetInt("NoAds") != 1)
        {
            // Clean up banner before reusing
            if (bannerAdView != null)
            {
                bannerAdView.Destroy();
            }

            // Create a 320x50 banner at top of the screen
            bannerAdView = new Yodo1U3dBannerAdView(Yodo1U3dBannerAdSize.Banner, Yodo1U3dBannerAdPosition.BannerLeft);

            // Ad Events
            bannerAdView.OnAdLoadedEvent += OnBannerAdLoadedEvent;
            bannerAdView.OnAdFailedToLoadEvent += OnBannerAdFailedToLoadEvent;
            bannerAdView.OnAdOpenedEvent += OnBannerAdOpenedEvent;
            bannerAdView.OnAdFailedToOpenEvent += OnBannerAdFailedToOpenEvent;
            bannerAdView.OnAdClosedEvent += OnBannerAdClosedEvent;

            // Load banner ads, the banner ad will be displayed automatically after loaded
            bannerAdView.LoadAd();
        }
    }

    private void OnBannerAdLoadedEvent(Yodo1U3dBannerAdView adView)
    {
        // Banner ad is ready to be shown.
        Debug.Log("[Yodo1 Mas] OnBannerAdLoadedEvent event received");
    }

    private void OnBannerAdFailedToLoadEvent(Yodo1U3dBannerAdView adView, Yodo1U3dAdError adError)
    {
        Debug.Log("[Yodo1 Mas] OnBannerAdFailedToLoadEvent event received with error: " + adError.ToString());
    }

    private void OnBannerAdOpenedEvent(Yodo1U3dBannerAdView adView)
    {
        Debug.Log("[Yodo1 Mas] OnBannerAdOpenedEvent event received");
    }

    private void OnBannerAdFailedToOpenEvent(Yodo1U3dBannerAdView adView, Yodo1U3dAdError adError)
    {
        Debug.Log("[Yodo1 Mas] OnBannerAdFailedToOpenEvent event received with error: " + adError.ToString());
    }

    private void OnBannerAdClosedEvent(Yodo1U3dBannerAdView adView)
    {
        Debug.Log("[Yodo1 Mas] OnBannerAdClosedEvent event received");
    }

    private void InitializeInterstitial()
    {
        if (PlayerPrefs.GetInt("NoAds") != 1)
        {
            // Instantiate
            Yodo1U3dInterstitialAd.GetInstance();

            // Ad Events
            Yodo1U3dInterstitialAd.GetInstance().OnAdLoadedEvent += OnInterstitialAdLoadedEvent;
            Yodo1U3dInterstitialAd.GetInstance().OnAdLoadFailedEvent += OnInterstitialAdLoadFailedEvent;
            Yodo1U3dInterstitialAd.GetInstance().OnAdOpenedEvent += OnInterstitialAdOpenedEvent;
            Yodo1U3dInterstitialAd.GetInstance().OnAdOpenFailedEvent += OnInterstitialAdOpenFailedEvent;
            Yodo1U3dInterstitialAd.GetInstance().OnAdClosedEvent += OnInterstitialAdClosedEvent;
        }
    }

    private void RequestInterstitial()
    {
        Yodo1U3dInterstitialAd.GetInstance().LoadAd();
    }

   

    private void OnInterstitialAdLoadedEvent(Yodo1U3dInterstitialAd ad)
    {
        Debug.Log("[Yodo1 Mas] OnInterstitialAdLoadedEvent event received");
    }

    private void OnInterstitialAdLoadFailedEvent(Yodo1U3dInterstitialAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log("[Yodo1 Mas] OnInterstitialAdLoadFailedEvent event received with error: " + adError.ToString());
    }

    private void OnInterstitialAdOpenedEvent(Yodo1U3dInterstitialAd ad)
    {
        Debug.Log("[Yodo1 Mas] OnInterstitialAdOpenedEvent event received");
    }

    private void OnInterstitialAdOpenFailedEvent(Yodo1U3dInterstitialAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log("[Yodo1 Mas] OnInterstitialAdOpenFailedEvent event received with error: " + adError.ToString());
        // Load the next ad
        this.RequestInterstitial();
    }

    private void OnInterstitialAdClosedEvent(Yodo1U3dInterstitialAd ad)
    {
        Debug.Log("[Yodo1 Mas] OnInterstitialAdClosedEvent event received");
        // Load the next ad
        this.RequestInterstitial();
    }
    public void ShowInterstitial()
    {

        // if (Yodo1U3dMas.IsInterstitialAdLoaded())
        if (PlayerPrefs.GetInt("NoAds") != 1)
        {
            Yodo1U3dInterstitialAd.GetInstance().ShowAd();
        }
        else
        {

        }
    }


    private void InitializeRewardedAds()
    {
        // Instantiate
        Yodo1U3dRewardAd.GetInstance();

        // Ad Events
        Yodo1U3dRewardAd.GetInstance().OnAdLoadedEvent += OnRewardAdLoadedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdLoadFailedEvent += OnRewardAdLoadFailedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdOpenedEvent += OnRewardAdOpenedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdOpenFailedEvent += OnRewardAdOpenFailedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdClosedEvent += OnRewardAdClosedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdEarnedEvent += OnRewardAdEarnedEvent;
    }

    private void RequestRewardedAds()
    {
        Yodo1U3dRewardAd.GetInstance().LoadAd();
    }

   

    private void OnRewardAdLoadedEvent(Yodo1U3dRewardAd ad)
    {
        Debug.Log("[Yodo1 Mas] OnRewardAdLoadedEvent event received");
    }

    private void OnRewardAdLoadFailedEvent(Yodo1U3dRewardAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log("[Yodo1 Mas] OnRewardAdLoadFailedEvent event received with error: " + adError.ToString());
    }

    private void OnRewardAdOpenedEvent(Yodo1U3dRewardAd ad)
    {
        Debug.Log("[Yodo1 Mas] OnRewardAdOpenedEvent event received");
    }

    private void OnRewardAdOpenFailedEvent(Yodo1U3dRewardAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log("[Yodo1 Mas] OnRewardAdOpenFailedEvent event received with error: " + adError.ToString());
        // Load the next ad
        this.RequestRewardedAds();
    }

    private void OnRewardAdClosedEvent(Yodo1U3dRewardAd ad)
    {
        Debug.Log("[Yodo1 Mas] OnRewardAdClosedEvent event received");
        // Load the next ad
        this.RequestRewardedAds();
    }

    private void OnRewardAdEarnedEvent(Yodo1U3dRewardAd ad)
    {
        Debug.Log("[Yodo1 Mas] OnRewardAdEarnedEvent event received");
        // Add your reward code here
    }
    public void ShowRewarded()
    {

        // if (PlayerPrefs.GetInt("NoAds") != 1)
        // {
        Yodo1U3dRewardAd.GetInstance().ShowAd();
        // }
    }

    public void ShowBanner()
    {
        if (PlayerPrefs.GetInt("NoAds") != 1)
        {
            if (bannerAdView != null)
                bannerAdView.Show();
        }
    }

    public void HideBanner()
    {
        if(bannerAdView!=null)
        bannerAdView.Hide();
    }
}
