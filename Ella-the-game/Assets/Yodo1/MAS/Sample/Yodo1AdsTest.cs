using UnityEngine;
using System.Collections;
using Yodo1.MAS;

public class Yodo1AdsTest : MonoBehaviour
{
    void Start()
    {
        SetPrivacy(true, false, false);
        SetDelegates();
        InitializeSdk();
        ShowBanner();
    }

    void ShowBanner()
    {
        int align = Yodo1U3dBannerAlign.BannerTop | Yodo1U3dBannerAlign.BannerHorizontalCenter;
        Yodo1U3dMas.ShowBannerAd(align);
    }

    void OnGUI()
    {
        int buttonHeight = Screen.height / 13;
        int buttonWidth = Screen.width / 2;
        int buttonSpace = buttonHeight / 2;
        int startHeight = buttonHeight / 2;

        if (GUI.Button(new Rect(Screen.width / 4, startHeight, buttonWidth, buttonHeight), "Show Banner Ad"))
        {
            ShowBanner();
        }

        if (GUI.Button(new Rect(Screen.width / 4, startHeight + buttonHeight + buttonSpace, buttonWidth, buttonHeight), "Dismiss Banner Ad"))
        {
            Yodo1U3dMas.DismissBannerAd();
        }

        if (GUI.Button(new Rect(Screen.width / 4, startHeight + buttonHeight * 2 + buttonSpace * 2, buttonWidth, buttonHeight), "Show Interstitial Ad"))
        {
            if (Yodo1U3dMas.IsInterstitialAdLoaded())
            {
                Yodo1U3dMas.ShowInterstitialAd();
            }
            else
            {
                Debug.Log("[Yodo1 Mas] Interstitial ad has not been cached.");
            }

        }

        if (GUI.Button(new Rect(Screen.width / 4, startHeight + buttonHeight * 3 + buttonSpace * 3, buttonWidth, buttonHeight), "Show Rewarded Ad"))
        {
            if (Yodo1U3dMas.IsRewardedAdLoaded())
            {
                Yodo1U3dMas.ShowRewardedAd();
            }
            else
            {
                Debug.Log("[Yodo1 Mas] Reward video ad has not been cached.");
            }
        }
    }

    private void SetPrivacy(bool gdpr, bool coppa, bool ccpa)
    {
        Yodo1U3dMas.SetGDPR(gdpr);
        Yodo1U3dMas.SetCOPPA(coppa);
        Yodo1U3dMas.SetCCPA(ccpa);
    }

    private void InitializeSdk()
    {
        Yodo1AdBuildConfig config = new Yodo1AdBuildConfig().enableAdaptiveBanner(true).enableUserPrivacyDialog(true);
        Yodo1U3dMas.SetAdBuildConfig(config);

        Yodo1U3dMas.InitializeSdk();
    }

    private void SetDelegates()
    {
        Yodo1U3dMas.SetInitializeDelegate((bool success, Yodo1U3dAdError error) =>
        {
            Debug.Log("[Yodo1 Mas] InitializeDelegate, success:" + success + ", error: \n" + error.ToString());

            if (success)
            {

            }
            else
            {

            }
        });

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

}
