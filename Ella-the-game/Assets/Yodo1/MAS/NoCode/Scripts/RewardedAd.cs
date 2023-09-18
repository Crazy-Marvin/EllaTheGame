using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Yodo1.MAS;

[RequireComponent((typeof(Button)))]
public class RewardedAd : MonoBehaviour
{
    Button rvBtn;
    [Header("PlacementID (optional) ")]
    [Tooltip("Enter your Rewarded Ad placement ID. Leave empty if you do not have one.")]
    public string placementID;
    [Space(10)]
    [Header("Rewarded AD Events")]
    [SerializeField] UnityEvent OnRewardedAdLoaded;
    [SerializeField] UnityEvent OnRewardedAdLoadFailed;
    [SerializeField] UnityEvent OnRewardedAdOpened;
    [SerializeField] UnityEvent OnRewardedAdOpenFailed;
    [SerializeField] UnityEvent OnRewardedAdClosed;
    [Header("Award User Here")]
    [SerializeField] UnityEvent OnAdReceivedReward;

    //[System.Obsolete("Please use `OnAdLoadFailedEvent` and `OnAdOpenFailedEvent` instead.", false)]
    //[SerializeField] UnityEvent OnRewardedAdError;

    private void Awake()
    {
        rvBtn = GetComponent<Button>();
    }

    private void Start()
    {
        rvBtn.onClick.AddListener(TaskOnClick);

        LoadAd();
    }

    void TaskOnClick()
    {
        if (Yodo1U3dRewardAd.GetInstance().IsLoaded())
        {
            Yodo1U3dRewardAd.GetInstance().OnAdOpenedEvent += OnRewardAdOpenedEvent;
            Yodo1U3dRewardAd.GetInstance().OnAdOpenFailedEvent += OnRewardAdOpenFailedEvent;
            Yodo1U3dRewardAd.GetInstance().OnAdClosedEvent += OnRewardAdClosedEvent;
            Yodo1U3dRewardAd.GetInstance().OnAdEarnedEvent += OnRewardAdEarnedEvent;

            if (string.IsNullOrEmpty(placementID))
            {
                Yodo1U3dRewardAd.GetInstance().ShowAd();
            }
            else
            {
                Yodo1U3dRewardAd.GetInstance().ShowAd(placementID);
            }
        }
        else
        {
            Debug.Log(Yodo1U3dMas.TAG + "NoCode Reward video ad has not been cached.");
        }
    }

    private void LoadAd()
    {
        Yodo1U3dRewardAd.GetInstance().OnAdLoadedEvent -= OnRewardAdLoadedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdLoadFailedEvent -= OnRewardAdLoadFailedEvent;

        Yodo1U3dRewardAd.GetInstance().OnAdLoadedEvent += OnRewardAdLoadedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdLoadFailedEvent += OnRewardAdLoadFailedEvent;

        Yodo1U3dRewardAd.GetInstance().LoadAd();
    }

    private void OnRewardAdLoadedEvent(Yodo1U3dRewardAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Rewarded ad loaded");
        OnRewardedAdLoaded.Invoke();
    }

    private void OnRewardAdLoadFailedEvent(Yodo1U3dRewardAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Rewarded ad load failed");
        OnRewardedAdLoadFailed.Invoke();
        //OnRewardedAdError.Invoke();

        LoadAd();
    }

    private void OnRewardAdOpenedEvent(Yodo1U3dRewardAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Rewarded ad opened");
        OnRewardedAdOpened.Invoke();
    }

    private void OnRewardAdOpenFailedEvent(Yodo1U3dRewardAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Rewarded ad open failed");
        OnRewardedAdOpenFailed.Invoke();
        //OnRewardedAdError.Invoke();

        LoadAd();
    }

    private void OnRewardAdClosedEvent(Yodo1U3dRewardAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Rewarded ad closed");
        OnRewardedAdClosed.Invoke();

        Yodo1U3dRewardAd.GetInstance().OnAdOpenedEvent -= OnRewardAdOpenedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdOpenFailedEvent -= OnRewardAdOpenFailedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdClosedEvent -= OnRewardAdClosedEvent;
        Yodo1U3dRewardAd.GetInstance().OnAdEarnedEvent -= OnRewardAdEarnedEvent;

        LoadAd();
    }

    private void OnRewardAdEarnedEvent(Yodo1U3dRewardAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Rewarded ad received reward");
        OnAdReceivedReward.Invoke();
    }
}
