using UnityEngine;
using UnityEngine.UI;
using Yodo1.MAS;

public class Yodo1AdsTest : MonoBehaviour
{
    [Header("Ad Placements")]
    public InputField interstitialAdPlacement;
    public InputField rewardAdPlacement;
    public InputField rewardInterstitialAdPlacement;
    public InputField appOpenAdPlacement;

    bool enableInterstitialApiV2 = true;
    bool enabledRewardVideoV2 = true;

    void Start()
    {
        Yodo1U3dMasCallback.OnSdkInitializedEvent += (success, error) =>
        {
            Debug.Log(Yodo1U3dMas.TAG + "OnSdkInitializedEvent, success:" + success + ", error: " + error.ToString());
            Debug.Log(Yodo1U3dMas.TAG + "OnSdkInitializedEvent, age:" + Yodo1U3dMas.GetUserAge());
            if (success)
            {
                InitializeBannerAds();
                InitializeInterstitialAds();
                InitializeRewardedAds();
                InitializeNativeAds();
                InitializeRewardedInterstitialAds();
                InitializeAppOpenAds();
            }
        };

        Yodo1MasUserPrivacyConfig userPrivacyConfig = new Yodo1MasUserPrivacyConfig()
            .titleBackgroundColor(Color.green)
            .titleTextColor(Color.blue)
            .contentBackgroundColor(Color.black)
            .contentTextColor(Color.white)
            .buttonBackgroundColor(Color.red)
            .buttonTextColor(Color.green);

        Yodo1AdBuildConfig config = new Yodo1AdBuildConfig()
            .enableAdaptiveBanner(true)
            .enableUserPrivacyDialog(true)
            .userPrivacyConfig(userPrivacyConfig);
        Yodo1U3dMas.SetAdBuildConfig(config);

        Yodo1U3dMas.InitializeMasSdk();
    }
    

    #region Banner Ad Methods
    private void InitializeBannerAds()
    {
        InitializeBannerAdsV2();
    }

    public void ShowBannerAds(string bannerType)
    {
        Yodo1U3dBannerAdSize.Type type = Yodo1U3dBannerAdSize.Type.Banner;
        if (!string.IsNullOrEmpty(bannerType))
        {
            if (bannerType.Equals("standard"))
            {
                type = Yodo1U3dBannerAdSize.Type.Banner;
            }
            else if (bannerType.Equals("large"))
            {
                type = Yodo1U3dBannerAdSize.Type.LargeBanner;
            }
            else if (bannerType.Equals("IAB"))
            {
                type = Yodo1U3dBannerAdSize.Type.IABMediumRectangle;
            }
            else if (bannerType.Equals("smart"))
            {
                type = Yodo1U3dBannerAdSize.Type.SmartBanner;
            }
            else if (bannerType.Equals("adaptive"))
            {
                type = Yodo1U3dBannerAdSize.Type.AdaptiveBanner;
            }
        }
        ShowBannerAdsV2(type);
    }

    public void HideAllBannerAds()
    {
        HideAllBannerAdsV2();
    }
    #endregion

    #region Banner Ad Methods - V2
    Yodo1U3dBannerAdView bannerAdView = null;
    Yodo1U3dBannerAdView standardBannerAdView = null;
    Yodo1U3dBannerAdView largeBannerAdView = null;
    Yodo1U3dBannerAdView IABBannerAdView = null;
    Yodo1U3dBannerAdView smartBannerAdView = null;
    Yodo1U3dBannerAdView adaptiveBannerAdView = null;

    /// <summary>
    /// The banner is displayed automatically after loaded
    /// </summary>
    private void InitializeBannerAdsV2()
    {
        // Clean up banner before reusing
        if (bannerAdView != null)
        {
            bannerAdView.Destroy();
            bannerAdView = null;
        }

        // Create a 320x50 banner
        bannerAdView = new Yodo1U3dBannerAdView(Yodo1U3dBannerAdSize.Banner, Yodo1U3dBannerAdPosition.BannerBottom | Yodo1U3dBannerAdPosition.BannerHorizontalCenter);

        // Add Events
        bannerAdView.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        bannerAdView.OnAdFailedToLoadEvent += OnBannerAdFailedToLoadEvent;
        bannerAdView.OnAdOpenedEvent += OnBannerAdOpenedEvent;
        bannerAdView.OnAdFailedToOpenEvent += OnBannerAdFailedToOpenEvent;
        bannerAdView.OnAdClosedEvent += OnBannerAdClosedEvent;

        // Load banner ads, the banner ad will be displayed automatically after loaded
        bannerAdView.LoadAd();

        // Standard Banner
        if (standardBannerAdView != null)
        {
            standardBannerAdView.Destroy();
            standardBannerAdView = null;
        }

        // Create a 320x50 banner
        standardBannerAdView = new Yodo1U3dBannerAdView(Yodo1U3dBannerAdSize.Banner, Yodo1U3dBannerAdPosition.BannerBottom | Yodo1U3dBannerAdPosition.BannerHorizontalCenter);

        // Add Events
        standardBannerAdView.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        standardBannerAdView.OnAdFailedToLoadEvent += OnBannerAdFailedToLoadEvent;
        standardBannerAdView.OnAdOpenedEvent += OnBannerAdOpenedEvent;
        standardBannerAdView.OnAdFailedToOpenEvent += OnBannerAdFailedToOpenEvent;
        standardBannerAdView.OnAdClosedEvent += OnBannerAdClosedEvent;

        // Large Banner
        if (largeBannerAdView != null)
        {
            largeBannerAdView.Destroy();
            largeBannerAdView = null;
        }

        // Create a 320x100 banner 
        largeBannerAdView = new Yodo1U3dBannerAdView(Yodo1U3dBannerAdSize.LargeBanner, Yodo1U3dBannerAdPosition.BannerLeft | Yodo1U3dBannerAdPosition.BannerHorizontalCenter);

        // Add Events
        largeBannerAdView.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        largeBannerAdView.OnAdFailedToLoadEvent += OnBannerAdFailedToLoadEvent;
        largeBannerAdView.OnAdOpenedEvent += OnBannerAdOpenedEvent;
        largeBannerAdView.OnAdFailedToOpenEvent += OnBannerAdFailedToOpenEvent;
        largeBannerAdView.OnAdClosedEvent += OnBannerAdClosedEvent;

        // IAB Banner
        if (IABBannerAdView != null)
        {
            IABBannerAdView.Destroy();
            IABBannerAdView = null;
        }

        // Create a 300x250 banner
        IABBannerAdView = new Yodo1U3dBannerAdView(Yodo1U3dBannerAdSize.IABMediumRectangle, Yodo1U3dBannerAdPosition.BannerRight | Yodo1U3dBannerAdPosition.BannerVerticalCenter);

        // Add Events
        IABBannerAdView.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        IABBannerAdView.OnAdFailedToLoadEvent += OnBannerAdFailedToLoadEvent;
        IABBannerAdView.OnAdOpenedEvent += OnBannerAdOpenedEvent;
        IABBannerAdView.OnAdFailedToOpenEvent += OnBannerAdFailedToOpenEvent;
        IABBannerAdView.OnAdClosedEvent += OnBannerAdClosedEvent;

        // Smart Banner
        if (smartBannerAdView != null)
        {
            smartBannerAdView.Destroy();
            smartBannerAdView = null;
        }

        smartBannerAdView = new Yodo1U3dBannerAdView(Yodo1U3dBannerAdSize.SmartBanner, Yodo1U3dBannerAdPosition.BannerBottom | Yodo1U3dBannerAdPosition.BannerHorizontalCenter);

        // Add Events
        smartBannerAdView.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        smartBannerAdView.OnAdFailedToLoadEvent += OnBannerAdFailedToLoadEvent;
        smartBannerAdView.OnAdOpenedEvent += OnBannerAdOpenedEvent;
        smartBannerAdView.OnAdFailedToOpenEvent += OnBannerAdFailedToOpenEvent;
        smartBannerAdView.OnAdClosedEvent += OnBannerAdClosedEvent;

        // Adaptive Banner
        if (adaptiveBannerAdView != null)
        {
            adaptiveBannerAdView.Destroy();
            adaptiveBannerAdView = null;
        }

        adaptiveBannerAdView = new Yodo1U3dBannerAdView(Yodo1U3dBannerAdSize.AdaptiveBanner, Yodo1U3dBannerAdPosition.BannerTop | Yodo1U3dBannerAdPosition.BannerHorizontalCenter);

        // Add Events
        adaptiveBannerAdView.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        adaptiveBannerAdView.OnAdFailedToLoadEvent += OnBannerAdFailedToLoadEvent;
        adaptiveBannerAdView.OnAdOpenedEvent += OnBannerAdOpenedEvent;
        adaptiveBannerAdView.OnAdFailedToOpenEvent += OnBannerAdFailedToOpenEvent;
        adaptiveBannerAdView.OnAdClosedEvent += OnBannerAdClosedEvent;
    }

    private void OnBannerAdLoadedEvent(Yodo1U3dBannerAdView adView)
    {
        // Banner ad is ready to be shown.
        Debug.Log(Yodo1U3dMas.TAG + "BannerV2 ad loaded, " + adView.ToJsonString());
    }

    private void OnBannerAdFailedToLoadEvent(Yodo1U3dBannerAdView adView, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "BannerV2 ad failed to load with error code: " + adError.ToString());
    }

    private void OnBannerAdOpenedEvent(Yodo1U3dBannerAdView adView)
    {
        Debug.Log(Yodo1U3dMas.TAG + "BannerV2 ad opened, " + adView.ToJsonString());
    }

    private void OnBannerAdFailedToOpenEvent(Yodo1U3dBannerAdView adView, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "BannerV2 ad failed to load with error code: " + adError.ToString());
    }

    private void OnBannerAdClosedEvent(Yodo1U3dBannerAdView adView)
    {
        Debug.Log(Yodo1U3dMas.TAG + "BannerV2 ad closed, " + adView.ToJsonString());
    }

    /// <summary>
    /// (Optional) Show banner ads
    /// </summary>
    private void ShowBannerAdsV2(Yodo1U3dBannerAdSize.Type type)
    {
        if (type == Yodo1U3dBannerAdSize.Type.Banner)
        {
            if (standardBannerAdView != null)
            {
                // Load banner ads, the banner ad will be displayed automatically after loaded
                standardBannerAdView.LoadAd();
            }
        }
        else if (type == Yodo1U3dBannerAdSize.Type.LargeBanner)
        {
            if (largeBannerAdView != null)
            {
                // Load banner ads, the banner ad will be displayed automatically after loaded
                largeBannerAdView.LoadAd();
            }
        }
        else if (type == Yodo1U3dBannerAdSize.Type.IABMediumRectangle)
        {
            if (IABBannerAdView != null)
            {
                // Load banner ads, the banner ad will be displayed automatically after loaded
                IABBannerAdView.LoadAd();
            }
        }
        else if (type == Yodo1U3dBannerAdSize.Type.SmartBanner)
        {
            if (smartBannerAdView != null)
            {
                // Load banner ads, the banner ad will be displayed automatically after loaded
                smartBannerAdView.LoadAd();
            }
        }
        else if (type == Yodo1U3dBannerAdSize.Type.AdaptiveBanner)
        {

            if (adaptiveBannerAdView != null)
            {
                // Load banner ads, the banner ad will be displayed automatically after loaded
                adaptiveBannerAdView.LoadAd();
            }
        }
    }

    /// <summary>
    /// (Optional) Hide banner ads
    /// </summary>
    private void HideAllBannerAdsV2()
    {
        if (bannerAdView != null)
        {
            bannerAdView.Hide();
        }
        if (standardBannerAdView != null)
        {
            standardBannerAdView.Hide();
        }
        if (largeBannerAdView != null)
        {
            largeBannerAdView.Hide();
        }
        if (IABBannerAdView != null)
        {
            IABBannerAdView.Hide();
        }
        if (smartBannerAdView != null)
        {
            smartBannerAdView.Hide();
        }
        if (adaptiveBannerAdView != null)
        {
            adaptiveBannerAdView.Hide();
        }
    }
    #endregion

    #region Interstitial Ad Methods
    private void InitializeInterstitialAds()
    {
        if (!enableInterstitialApiV2)
        {
            InitializeInterstitialAdsV1();
        }
        else
        {
            InitializeInterstitialAdsV2();
        }
    }

    public void ShowInterstitialAds()
    {
        string adPlacement = string.Empty;
        if (interstitialAdPlacement != null && !string.IsNullOrEmpty(interstitialAdPlacement.text))
        {
            adPlacement = interstitialAdPlacement.text;
        }

        if (!enableInterstitialApiV2)
        {
            ShowInterstitialAdsV1(adPlacement);
        }
        else
        {
            ShowInterstitialAdsV2(adPlacement);
        }

    }
    #endregion

    #region Interstitial Ad Methods - V1

    private void InitializeInterstitialAdsV1()
    {
        Yodo1U3dMasCallback.Interstitial.OnAdOpenedEvent += OnInterstitialAdOpenedEvent;
        Yodo1U3dMasCallback.Interstitial.OnAdClosedEvent += OnInterstitialAdClosedEvent;
        Yodo1U3dMasCallback.Interstitial.OnAdErrorEvent += OnInterstitialAdErorEvent;
    }

    private void OnInterstitialAdOpenedEvent()
    {
        Debug.Log(Yodo1U3dMas.TAG + "Interstitial ad opened");
    }

    private void OnInterstitialAdClosedEvent()
    {
        Debug.Log(Yodo1U3dMas.TAG + "Interstitial ad closed");
    }

    private void OnInterstitialAdErorEvent(Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "Interstitial ad error - " + adError.ToString());
    }

    private void ShowInterstitialAdsV1(string adPlacement)
    {
        if (Yodo1U3dMas.IsInterstitialAdLoaded())
        {
            if (string.IsNullOrEmpty(adPlacement))
            {
                Yodo1U3dMas.ShowInterstitialAd();
            }
            else
            {
                Yodo1U3dMas.ShowInterstitialAd(adPlacement);
            }
        }
        else
        {
            Debug.Log(Yodo1U3dMas.TAG + "Interstitial ad has not been cached.");
        }
    }

    #endregion

    #region Interstitial Ad Methods - V2
    private void InitializeInterstitialAdsV2()
    {
        Yodo1U3dInterstitialAd.GetInstance().OnAdLoadedEvent += OnInterstitialAdLoadedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdLoadFailedEvent += OnInterstitialAdLoadFailedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdOpenedEvent += OnInterstitialAdOpenedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdOpenFailedEvent += OnInterstitialAdOpenFailedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdClosedEvent += OnInterstitialAdClosedEvent;
    }

    public void LoadInterstitialAdV2()
    {
        if (!enableInterstitialApiV2)
        {
            return;
        }
        Yodo1U3dInterstitialAd.GetInstance().LoadAd();
    }

    private void OnInterstitialAdLoadedEvent(Yodo1U3dInterstitialAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "OnInterstitialAdLoadedEvent event received");
    }

    private void OnInterstitialAdLoadFailedEvent(Yodo1U3dInterstitialAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "OnInterstitialAdLoadFailedEvent event received with error: " + adError.ToString());
    }

    private void OnInterstitialAdOpenedEvent(Yodo1U3dInterstitialAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "OnInterstitialAdOpenedEvent event received");
    }

    private void OnInterstitialAdOpenFailedEvent(Yodo1U3dInterstitialAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "OnInterstitialAdOpenFailedEvent event received with error: " + adError.ToString());
    }

    private void OnInterstitialAdClosedEvent(Yodo1U3dInterstitialAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "OnInterstitialAdClosedEvent event received");
    }

    private void ShowInterstitialAdsV2(string adPlacement)
    {
        if (string.IsNullOrEmpty(adPlacement))
        {
            Yodo1U3dInterstitialAd.GetInstance().ShowAd();
        }
        else
        {
            Yodo1U3dInterstitialAd.GetInstance().ShowAd(adPlacement);
        }
    }

    #endregion


    #region Reward video Ad Methods
    private void InitializeRewardedAds()
    {
        if (!enableInterstitialApiV2)
        {
            InitializeRewardedAdsV1();
        }
        else
        {
            InitializeRewardedAdsV2();
        }
    }

    public void ShowRewardedAds()
    {
        string adPlacement = string.Empty;
        if (rewardAdPlacement != null && !string.IsNullOrEmpty(rewardAdPlacement.text))
        {
            adPlacement = rewardAdPlacement.text;
        }

        if (!enabledRewardVideoV2)
        {
            ShowRewardedAdsV1(adPlacement);
        }
        else
        {
            ShowRewardedAdsV2(adPlacement);
        }
    }
    #endregion

    #region Reward video Ad Methods - V1
    private void InitializeRewardedAdsV1()
    {
        Yodo1U3dMasCallback.Rewarded.OnAdOpenedEvent += OnRewardedAdOpenedEvent;
        Yodo1U3dMasCallback.Rewarded.OnAdClosedEvent += OnRewardedAdClosedEvent;
        Yodo1U3dMasCallback.Rewarded.OnAdReceivedRewardEvent += OnAdReceivedRewardEvent;
        Yodo1U3dMasCallback.Rewarded.OnAdErrorEvent += OnRewardedAdErorEvent;
    }

    private void OnRewardedAdOpenedEvent()
    {
        Debug.Log(Yodo1U3dMas.TAG + "Rewarded ad opened");
    }

    private void OnRewardedAdClosedEvent()
    {
        Debug.Log(Yodo1U3dMas.TAG + "Rewarded ad closed");
    }

    private void OnAdReceivedRewardEvent()
    {
        Debug.Log(Yodo1U3dMas.TAG + "Rewarded ad received reward");
    }

    private void OnRewardedAdErorEvent(Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "Rewarded ad error - " + adError.ToString());
    }

    private void ShowRewardedAdsV1(string adPlacement)
    {
        if (Yodo1U3dMas.IsRewardedAdLoaded())
        {
            if (string.IsNullOrEmpty(adPlacement))
            {
                Yodo1U3dMas.ShowRewardedAd();
            }
            else
            {
                Yodo1U3dMas.ShowRewardedAd(adPlacement);
            }
        }
        else
        {
            Debug.Log(Yodo1U3dMas.TAG + "Reward video ad has not been cached.");
        }
    }

    #endregion

    #region Reward video Ad Methods - V2

    private void InitializeRewardedAdsV2()
    {
        Yodo1U3dRewardAd.GetInstance().OnAdLoadedEvent += OnRewardAdLoadedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdLoadFailedEvent += OnRewardAdLoadFailedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdOpenedEvent += OnRewardAdOpenedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdOpenFailedEvent += OnRewardAdOpenFailedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdClosedEvent += OnRewardAdClosedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdEarnedEvent += OnRewardAdEarnedEvent;
    }

    public void LoadRewardAdV2()
    {
        if (!enabledRewardVideoV2)
        {
            return;
        }
        Yodo1U3dRewardAd.GetInstance().LoadAd();
    }

    private void OnRewardAdLoadedEvent(Yodo1U3dRewardAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "OnRewardAdLoadedEvent event received");
    }

    private void OnRewardAdLoadFailedEvent(Yodo1U3dRewardAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "OnRewardAdLoadFailedEvent event received with error: " + adError.ToString());
    }

    private void OnRewardAdOpenedEvent(Yodo1U3dRewardAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "OnRewardAdOpenedEvent event received");
    }

    private void OnRewardAdOpenFailedEvent(Yodo1U3dRewardAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "OnRewardAdOpenFailedEvent event received with error: " + adError.ToString());
    }

    private void OnRewardAdClosedEvent(Yodo1U3dRewardAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "OnRewardAdClosedEvent event received");
    }

    private void OnRewardAdEarnedEvent(Yodo1U3dRewardAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "OnRewardAdEarnedEvent event received");
    }

    private void ShowRewardedAdsV2(string adPlacement)
    {
        if (string.IsNullOrEmpty(adPlacement))
        {
            Yodo1U3dRewardAd.GetInstance().ShowAd();
        }
        else
        {
            Yodo1U3dRewardAd.GetInstance().ShowAd(adPlacement);
        }
    }

    #endregion

    #region Yodo1U3dNativeAdView
    Yodo1U3dNativeAdView nativeAdView = null;
    Yodo1U3dNativeAdView nativeAdView2 = null;


    /// <summary>
    /// The banner is displayed automatically after loaded
    /// </summary>
    private void InitializeNativeAds()
    {
        // Clean up native before reusing
        if (nativeAdView != null)
        {
            nativeAdView.Destroy();
            nativeAdView = null;
        }

        nativeAdView = new Yodo1U3dNativeAdView(Yodo1U3dNativeAdPosition.NativeTop | Yodo1U3dNativeAdPosition.NativeLeft, 0, 0, 360, 300);

        nativeAdView.SetBackgroundColor(Color.grey);
        // Add Events
        nativeAdView.OnAdLoadedEvent += OnNativeAdLoadedEvent;
        nativeAdView.OnAdFailedToLoadEvent += OnNativeAdFailedToLoadEvent;


        // Clean up native before reusing
        if (nativeAdView2 != null)
        {
            nativeAdView2.Destroy();
            nativeAdView2 = null;
        }

        nativeAdView2 = new Yodo1U3dNativeAdView(Yodo1U3dNativeAdPosition.NativeTop | Yodo1U3dNativeAdPosition.NativeRight, 0, 0, 360, 300);

        nativeAdView2.SetBackgroundColor(Color.grey);
        // Add Events
        nativeAdView2.OnAdLoadedEvent += OnNativeAdLoadedEvent;
        nativeAdView2.OnAdFailedToLoadEvent += OnNativeAdFailedToLoadEvent;
    }

    public void ShowNativeAd(string adPlacement)
    {
        // Load native ads, the native ad will be displayed automatically after loaded
        if (nativeAdView != null && adPlacement.Equals("test_native_placement_left"))
        {
            nativeAdView.SetAdPlacement(adPlacement);
            nativeAdView.LoadAd();
        }
        if (nativeAdView2 != null && adPlacement.Equals("test_native_placement_right"))
        {
            nativeAdView2.SetAdPlacement(adPlacement);
            nativeAdView2.LoadAd();
        }
    }

    public void HideAllNativeAds()
    {
        if (nativeAdView != null)
        {
            nativeAdView.Hide();
        }
        if (nativeAdView2 != null)
        {
            nativeAdView2.Hide();
        }
    }

    private void OnNativeAdLoadedEvent(Yodo1U3dNativeAdView adView)
    {
        Debug.Log(Yodo1U3dMas.TAG + "Native ad loaded");
    }

    private void OnNativeAdFailedToLoadEvent(Yodo1U3dNativeAdView adView, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "Native ad failed to load with error code: " + adError.ToString());
    }
    #endregion

    #region RewardedInterstitial Ad Methods
    
    private void InitializeRewardedInterstitialAds()
    {
        // Instantiate
        Yodo1U3dRewardedInterstitialAd.GetInstance();

        // Ad Events
        Yodo1U3dRewardedInterstitialAd.GetInstance().OnAdLoadedEvent += OnRewardedInterstitialAdLoadedEvent;
        Yodo1U3dRewardedInterstitialAd.GetInstance().OnAdLoadFailedEvent += OnRewardedInterstitialAdLoadFailedEvent;
        Yodo1U3dRewardedInterstitialAd.GetInstance().OnAdOpenedEvent += OnRewardedInterstitialAdOpenedEvent;
        Yodo1U3dRewardedInterstitialAd.GetInstance().OnAdOpenFailedEvent += OnRewardedInterstitialAdOpenFailedEvent;
        Yodo1U3dRewardedInterstitialAd.GetInstance().OnAdClosedEvent += OnRewardedInterstitialAdClosedEvent;
        Yodo1U3dRewardedInterstitialAd.GetInstance().OnAdEarnedEvent += OnRewardedInterstitialAdEarnedEvent;
    }

    public void LoadRewardedInterstitialAds()
    {
        Yodo1U3dRewardedInterstitialAd.GetInstance().LoadAd();
    }

    public void ShowRewardedInterstitialAds()
    {
        string adPlacement = string.Empty;
        if (rewardInterstitialAdPlacement != null && !string.IsNullOrEmpty(rewardInterstitialAdPlacement.text))
        {
            adPlacement = rewardInterstitialAdPlacement.text;
        }

        ShowRewardedInterstitialAds(adPlacement);
    }

    private void ShowRewardedInterstitialAds(string adPlacement)
    {
        bool isLoaded = Yodo1U3dRewardedInterstitialAd.GetInstance().IsLoaded();

        if (isLoaded) Yodo1U3dRewardedInterstitialAd.GetInstance().ShowAd(adPlacement);
    }

    private void OnRewardedInterstitialAdLoadedEvent(Yodo1U3dRewardedInterstitialAd ad)
    {
       Debug.Log("[Yodo1 Mas] OnRewardedInterstitialAdLoadedEvent event received");
    }

    private void OnRewardedInterstitialAdLoadFailedEvent(Yodo1U3dRewardedInterstitialAd ad, Yodo1U3dAdError adError)
    {
      Debug.Log("[Yodo1 Mas] OnRewardedInterstitialAdLoadFailedEvent event received with error: " + adError.ToString());
    }

    private void OnRewardedInterstitialAdOpenedEvent(Yodo1U3dRewardedInterstitialAd ad)
    {
        Debug.Log("[Yodo1 Mas] OnRewardedInterstitialAdOpenedEvent event received");
    }

    private void OnRewardedInterstitialAdOpenFailedEvent(Yodo1U3dRewardedInterstitialAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log("[Yodo1 Mas] OnRewardedInterstitialAdOpenFailedEvent event received with error: " + adError.ToString());
        // Load the next ad
        this.LoadRewardedInterstitialAds();
    }

    private void OnRewardedInterstitialAdClosedEvent(Yodo1U3dRewardedInterstitialAd ad)
    {
        Debug.Log("[Yodo1 Mas] OnRewardedInterstitialAdClosedEvent event received");
        // Load the next ad
        this.LoadRewardedInterstitialAds();
    }

    private void OnRewardedInterstitialAdEarnedEvent(Yodo1U3dRewardedInterstitialAd ad)
    {
        Debug.Log("[Yodo1 Mas] OnRewardedInterstitialAdEarnedEvent event received");
        // Add your reward code here
    }
    #endregion


    #region AppOpen Ad Methods
    private void InitializeAppOpenAds()
    {
        // Instantiate
        Yodo1U3dAppOpenAd.GetInstance();

        // Ad Events
        Yodo1U3dAppOpenAd.GetInstance().OnAdLoadedEvent += OnAppOpenAdLoadedEvent;
        Yodo1U3dAppOpenAd.GetInstance().OnAdLoadFailedEvent += OnAppOpenAdLoadFailedEvent;
        Yodo1U3dAppOpenAd.GetInstance().OnAdOpenedEvent += OnAppOpenAdOpenedEvent;
        Yodo1U3dAppOpenAd.GetInstance().OnAdOpenFailedEvent += OnAppOpenAdOpenFailedEvent;
        Yodo1U3dAppOpenAd.GetInstance().OnAdClosedEvent += OnAppOpenAdClosedEvent;
        Yodo1U3dAppOpenAd.GetInstance().LoadAd();
    }

    public void LoadAppOpenAds()
    {
        Yodo1U3dAppOpenAd.GetInstance().LoadAd();
    }

    public void ShowAppOpenAds()
    {
        string adPlacement = string.Empty;
        if (appOpenAdPlacement != null && !string.IsNullOrEmpty(appOpenAdPlacement.text))
        {
            adPlacement = appOpenAdPlacement.text;
        }

        ShowAppOpenAds(adPlacement);
    }

    private void ShowAppOpenAds(string adPlacement)
    {
        bool isLoaded = Yodo1U3dAppOpenAd.GetInstance().IsLoaded();

        if (isLoaded) Yodo1U3dAppOpenAd.GetInstance().ShowAd(adPlacement);
    }

    private void OnAppOpenAdLoadedEvent(Yodo1U3dAppOpenAd ad)
    {
        Debug.Log("[Yodo1 Mas] OnAppOpenAdLoadedEvent event received");
    }

    private void OnAppOpenAdLoadFailedEvent(Yodo1U3dAppOpenAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log("[Yodo1 Mas] OnAppOpenAdLoadFailedEvent event received with error: " + adError.ToString());
    }

    private void OnAppOpenAdOpenedEvent(Yodo1U3dAppOpenAd ad)
    {
        Debug.Log("[Yodo1 Mas] OnAppOpenAdOpenedEvent event received");
    }

    private void OnAppOpenAdOpenFailedEvent(Yodo1U3dAppOpenAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log("[Yodo1 Mas] OnAppOpenAdOpenFailedEvent event received with error: " + adError.ToString());
        // Load the next ad
        this.LoadAppOpenAds();
    }

    private void OnAppOpenAdClosedEvent(Yodo1U3dAppOpenAd ad)
    {
        Debug.Log("[Yodo1 Mas] OnAppOpenAdClosedEvent event received");
        // Load the next ad
        this.LoadAppOpenAds();
    }
    #endregion
}
