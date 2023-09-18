using UnityEngine;
using UnityEngine.Events;
using Yodo1.MAS;

public class InterstitialAtBreaks : MonoBehaviour
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

    private void OnEnable()
    {
        Yodo1U3dInterstitialAd.GetInstance().OnAdLoadedEvent += OnInterstitialAdLoadedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdLoadFailedEvent += OnInterstitialAdLoadFailedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdOpenedEvent += OnInterstitialAdOpenedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdOpenFailedEvent += OnInterstitialAdOpenFailedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdClosedEvent += OnInterstitialAdClosedEvent;

        ShowAd();
    }

    private void OnDisable()
    {
        Yodo1U3dInterstitialAd.GetInstance().OnAdLoadedEvent -= OnInterstitialAdLoadedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdLoadFailedEvent -= OnInterstitialAdLoadFailedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdOpenedEvent -= OnInterstitialAdOpenedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdOpenFailedEvent -= OnInterstitialAdOpenFailedEvent;
        Yodo1U3dInterstitialAd.GetInstance().OnAdClosedEvent -= OnInterstitialAdClosedEvent;
    }

    private void LoadAd()
    {
        Yodo1U3dInterstitialAd.GetInstance().LoadAd();
    }

    private void ShowAd()
    {
        if (Yodo1U3dInterstitialAd.GetInstance().IsLoaded())
        {
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

            LoadAd();
            Debug.Log(Yodo1U3dMas.TAG + "NoCode Interstitial ad has not been cached.");
            gameObject.SetActive(false);
        }
    }

    private void OnInterstitialAdLoadedEvent(Yodo1U3dInterstitialAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Interstitial ad loaded");
        OnInterstitialAdLoaded.Invoke();

        if (gameObject.activeSelf == true)
        {
            ShowAd();
        }
    }

    private void OnInterstitialAdLoadFailedEvent(Yodo1U3dInterstitialAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Interstitial ad load failed, error - " + adError.ToString());
        OnInterstitialAdLoadFailed.Invoke();
        //OnInterstitialAdError.Invoke();
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
    }

    private void OnInterstitialAdClosedEvent(Yodo1U3dInterstitialAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode Interstitial ad closed - AdBreaks");
        OnInterstitialAdClosed.Invoke();
        LoadAd();
        gameObject.SetActive(false);
    }
}
