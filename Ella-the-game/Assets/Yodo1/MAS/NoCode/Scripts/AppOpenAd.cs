using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using Yodo1.MAS;

[RequireComponent((typeof(Button)))]
public class AppOpenAd : MonoBehaviour
{
    Button appOpenBtn;
    [Header("PlacementID (optional) ")]
    [Tooltip("Enter your App Open Ad placement ID. Leave empty if you do not have one.")]
    public string placementID;
    [Space(10)]
    [Header("App Open AD Events")]
    [SerializeField] UnityEvent OnAppOpenAdLoaded;
    [SerializeField] UnityEvent OnAppOpenAdLoadFailed;
    [SerializeField] UnityEvent OnAppOpenAdOpened;
    [SerializeField] UnityEvent OnAppOpenAdOpenFailed;
    [SerializeField] UnityEvent OnAppOpenAdClosed;

    private void Awake()
    {
        appOpenBtn = GetComponent<Button>();
    }

    private void Start()
    {
        appOpenBtn.onClick.AddListener(TaskOnClick);

        LoadAd();
    }
    
    void TaskOnClick()
    {
        if (Yodo1U3dAppOpenAd.GetInstance().IsLoaded())
        {
            Yodo1U3dAppOpenAd.GetInstance().OnAdOpenedEvent += OnAppOpenAdOpenedEvent;
            Yodo1U3dAppOpenAd.GetInstance().OnAdOpenFailedEvent += OnAppOpenAdOpenFailedEvent;
            Yodo1U3dAppOpenAd.GetInstance().OnAdClosedEvent += OnAppOpenAdClosedEvent;

            if (string.IsNullOrEmpty(placementID))
            {
                Yodo1U3dAppOpenAd.GetInstance().ShowAd();
            }
            else
            {
                Yodo1U3dAppOpenAd.GetInstance().ShowAd(placementID);
            }
        }
        else
        {
            Debug.Log(Yodo1U3dMas.TAG + "NoCode App Open ad has not been cached.");
        }
    }

    private void LoadAd()
    {
        Yodo1U3dAppOpenAd.GetInstance().OnAdLoadedEvent -= OnAppOpenAdLoadedEvent;
        Yodo1U3dAppOpenAd.GetInstance().OnAdLoadFailedEvent -= OnAppOpenAdLoadFailedEvent;

        Yodo1U3dAppOpenAd.GetInstance().OnAdLoadedEvent += OnAppOpenAdLoadedEvent;
        Yodo1U3dAppOpenAd.GetInstance().OnAdLoadFailedEvent += OnAppOpenAdLoadFailedEvent;

        Yodo1U3dAppOpenAd.GetInstance().LoadAd();
    }

    private void OnAppOpenAdLoadedEvent(Yodo1U3dAppOpenAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode App Open ad loaded");
        OnAppOpenAdLoaded.Invoke();
    }

    private void OnAppOpenAdLoadFailedEvent(Yodo1U3dAppOpenAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode App Open ad load failed");
        OnAppOpenAdLoadFailed.Invoke();

        LoadAd();
    }

    private void OnAppOpenAdOpenedEvent(Yodo1U3dAppOpenAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode App Open ad opened");
        OnAppOpenAdOpened.Invoke();
    }

    private void OnAppOpenAdOpenFailedEvent(Yodo1U3dAppOpenAd ad, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode App Open ad open failed");
        OnAppOpenAdOpenFailed.Invoke();

        LoadAd();
    }

    private void OnAppOpenAdClosedEvent(Yodo1U3dAppOpenAd ad)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode App Open ad closed");
        OnAppOpenAdClosed.Invoke();

        Yodo1U3dAppOpenAd.GetInstance().OnAdOpenedEvent -= OnAppOpenAdOpenedEvent;
        Yodo1U3dAppOpenAd.GetInstance().OnAdOpenFailedEvent -= OnAppOpenAdOpenFailedEvent;
        Yodo1U3dAppOpenAd.GetInstance().OnAdClosedEvent -= OnAppOpenAdClosedEvent;

        LoadAd();
    }
}
