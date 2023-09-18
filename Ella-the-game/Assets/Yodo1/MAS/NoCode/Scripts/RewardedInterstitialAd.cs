using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Yodo1.MAS;

[RequireComponent((typeof(Button)))]
public class RewardedInterstitialAd : MonoBehaviour
{
    Button interstitialRewardBtn;
    [Header("PlacementID (optional) ")]
    [Tooltip("Enter your Interstitial Rewarded Ad placement ID. Leave empty if you do not have one.")]
    public string placementID;
    [Space(10)]
    [Header("Interstitial Rewarded AD Events")]
    [SerializeField] UnityEvent OnRewardedIntertitialAdLoaded;
    [SerializeField] UnityEvent OnRewardedIntertitialAdLoadFailed;
    [SerializeField] UnityEvent OnRewardedIntertitialAdOpened;
    [SerializeField] UnityEvent OnRewardedIntertitialAdOpenFailed;
    [SerializeField] UnityEvent OnRewardedIntertitialAdClosed;
    [Header("Award User Here")]
    [SerializeField] UnityEvent OnAdReceivedReward;

    private void Awake()
    {
        interstitialRewardBtn = GetComponent<Button>();
    }

    private void Start()
    {
        interstitialRewardBtn.onClick.AddListener(TaskOnClick);

        LoadAd();
    }
    void TaskOnClick()
    {
        if (Yodo1U3dRewardedInterstitialAd.GetInstance().IsLoaded())
        {
            Yodo1U3dRewardedInterstitialAd.GetInstance().OnAdOpenedEvent += OnRewardedIntertitialAdOpenedEvent;
            Yodo1U3dRewardedInterstitialAd.GetInstance().OnAdOpenFailedEvent += OnRewardedIntertitialAdOpenFailedEvent;
            Yodo1U3dRewardedInterstitialAd.GetInstance().OnAdClosedEvent += OnRewardedIntertitialAdClosedEvent;
            Yodo1U3dRewardedInterstitialAd.GetInstance().OnAdEarnedEvent += OnRewardedIntertitialAdEarnedEvent;

            if (string.IsNullOrEmpty(placementID))
            {
                Yodo1U3dRewardedInterstitialAd.GetInstance().ShowAd();
            }
            else
            {
                Yodo1U3dRewardedInterstitialAd.GetInstance().ShowAd(placementID);
            }
        }
        else
        {
            Debug.Log(Yodo1U3dMas.TAG + "NoCode Rewarded interstitial ad has not been cached.");
        }
    }

    private void LoadAd()
    {
        Yodo1U3dRewardedInterstitialAd.GetInstance().OnAdLoadedEvent -= OnRewardedIntertitialAdLoadedEvent;
        Yodo1U3dRewardedInterstitialAd.GetInstance().OnAdLoadFailedEvent -= OnRewardedIntertitialAdLoadFailedEvent;

        Yodo1U3dRewardedInterstitialAd.GetInstance().OnAdLoadedEvent += OnRewardedIntertitialAdLoadedEvent;
        Yodo1U3dRewardedInterstitialAd.GetInstance().OnAdLoadFailedEvent += OnRewardedIntertitialAdLoadFailedEvent;

        Yodo1U3dRewardedInterstitialAd.GetInstance().LoadAd();
    }

    private void OnRewardedIntertitialAdLoadedEvent(Yodo1U3dRewardedInterstitialAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Rewarded Interstitial ad loaded");
        OnRewardedIntertitialAdLoaded.Invoke();
    }

    private void OnRewardedIntertitialAdLoadFailedEvent(Yodo1U3dRewardedInterstitialAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Rewarded Interstitial ad load failed");
        OnRewardedIntertitialAdLoadFailed.Invoke();

        LoadAd();
    }

    private void OnRewardedIntertitialAdOpenedEvent(Yodo1U3dRewardedInterstitialAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Rewarded Interstitial ad opened");
        OnRewardedIntertitialAdOpened.Invoke();
    }

    private void OnRewardedIntertitialAdOpenFailedEvent(Yodo1U3dRewardedInterstitialAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Rewarded Interstitial ad open failed");
        OnRewardedIntertitialAdOpenFailed.Invoke();

        LoadAd();
    }

    private void OnRewardedIntertitialAdClosedEvent(Yodo1U3dRewardedInterstitialAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Rewarded Interstitial ad closed");
        OnRewardedIntertitialAdClosed.Invoke();

        Yodo1U3dRewardedInterstitialAd.GetInstance().OnAdOpenedEvent -= OnRewardedIntertitialAdOpenedEvent;
        Yodo1U3dRewardedInterstitialAd.GetInstance().OnAdOpenFailedEvent -= OnRewardedIntertitialAdOpenFailedEvent;
        Yodo1U3dRewardedInterstitialAd.GetInstance().OnAdClosedEvent -= OnRewardedIntertitialAdClosedEvent;
        Yodo1U3dRewardedInterstitialAd.GetInstance().OnAdEarnedEvent -= OnRewardedIntertitialAdEarnedEvent;

        LoadAd();
    }

    private void OnRewardedIntertitialAdEarnedEvent(Yodo1U3dRewardedInterstitialAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Rewarded Interstitial ad received reward");
        OnAdReceivedReward.Invoke();
    }
}
