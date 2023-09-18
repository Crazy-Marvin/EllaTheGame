using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Yodo1.MAS;

[RequireComponent((typeof(Button)))]
public class InterstitialAtButton : MonoBehaviour
{
    [Header("PlacementID (optional) ")]
    public string placementID;
    [Space(10)]
    [Header("Interstitial AD Events (optional) ")]
    [SerializeField] UnityEvent OnInterstitialAdLoaded;
    [SerializeField] UnityEvent OnInterstitialAdLoadFailed;
    [SerializeField] UnityEvent OnInterstitialAdOpened;
    [SerializeField] UnityEvent OnInterstitialAdOpenFailed;
    [SerializeField] UnityEvent OnInterstitialAdClosed;

    //[System.Obsolete("Please use `OnAdLoadFailedEvent` and `OnAdOpenFailedEvent` instead.", false)]
    //[SerializeField] UnityEvent OnInterstitialAdError;

    Button btn;

    private void Awake()
    {
        btn = GetComponent<Button>();
    }

    private void Start()
    {
        btn.onClick.AddListener(TaskOnClick);

        LoadAd();
    }

    private void LoadAd()
    {
        Yodo1U3dInterstitialAd.GetInstance().OnAdLoadedEvent += OnInterstitialAdLoadedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdLoadFailedEvent += OnInterstitialAdLoadFailedEvent;

        Yodo1U3dInterstitialAd.GetInstance().LoadAd();
    }

    void TaskOnClick()
    {
        if (Yodo1U3dInterstitialAd.GetInstance().IsLoaded())
        {
            Yodo1U3dInterstitialAd.GetInstance().OnAdOpenedEvent += OnInterstitialAdOpenedEvent;
            Yodo1U3dInterstitialAd.GetInstance().OnAdOpenFailedEvent += OnInterstitialAdOpenFailedEvent;
            Yodo1U3dInterstitialAd.GetInstance().OnAdClosedEvent += OnInterstitialAdClosedEvent;

            if (string.IsNullOrEmpty(placementID))
            {
                Yodo1U3dInterstitialAd.GetInstance().ShowAd();
            }
            else
            {
                Yodo1U3dInterstitialAd.GetInstance().ShowAd(placementID);
            }
        }
        else
        {
            Debug.Log(Yodo1U3dMas.TAG + "NoCode Interstitial ad has not been cached.");
        }
    }

    private void OnInterstitialAdLoadedEvent(Yodo1U3dInterstitialAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Interstitial ad loaded");
        OnInterstitialAdLoaded.Invoke();
    }

    private void OnInterstitialAdLoadFailedEvent(Yodo1U3dInterstitialAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Interstitial ad load failed, error - " + adError.ToString());
        OnInterstitialAdLoadFailed.Invoke();
        //OnInterstitialAdError.Invoke();

        LoadAd();
    }

    private void OnInterstitialAdOpenedEvent(Yodo1U3dInterstitialAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Interstitial ad opened");
        OnInterstitialAdOpened.Invoke();
    }

    private void OnInterstitialAdOpenFailedEvent(Yodo1U3dInterstitialAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Interstitial ad open failed, error - " + adError.ToString());
        OnInterstitialAdOpenFailed.Invoke();
        //OnInterstitialAdError.Invoke();

        LoadAd();
    }

    private void OnInterstitialAdClosedEvent(Yodo1U3dInterstitialAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Interstitial ad closed - AdButton");
        OnInterstitialAdClosed.Invoke();

        Yodo1U3dInterstitialAd.GetInstance().OnAdLoadedEvent -= OnInterstitialAdLoadedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdLoadFailedEvent -= OnInterstitialAdLoadFailedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdOpenedEvent -= OnInterstitialAdOpenedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdOpenFailedEvent -= OnInterstitialAdOpenFailedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdClosedEvent -= OnInterstitialAdClosedEvent;

        LoadAd();
    }
}
