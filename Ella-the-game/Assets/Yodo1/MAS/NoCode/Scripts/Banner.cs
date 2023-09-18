using UnityEngine;
using Yodo1.MAS;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Banner : MonoBehaviour
{
    public enum BannerSize
    {
        StandardBanner, LargeBanner, SmartBanner, AdaptiveBanner, IABMediumRectangle
    };
    public enum Vertical
    {
        BannerTop, BannerBottom, BannerVerticalCenter
    };
    public enum Horizontal
    {
        BannerHorizontalCenter, BannerLeft, BannerRight
    };

    [Header("Banner Type")]
    [Tooltip("Please Select the banner type you want to show.")]
    public BannerSize bannerSize = BannerSize.StandardBanner;

    [Header("Banner Alignment")]
    [Tooltip("Please set the banner Alignment according to where banner should appear.")]
    public Vertical VerticalAlignment = Vertical.BannerBottom;
    public Horizontal HorizontalAlignment = Horizontal.BannerHorizontalCenter;

    [Header("Show Banner On all scenes")]
    [Tooltip("Uncheck this if you want to show this banner on all scenes.")]
    public bool bannerOnAllScenes = true;

    [Header("Banner Placement (Optional) ")]
    public string PlacementID = string.Empty;

    [Space(10)]
    [Header("Banner AD Events (optional) ")]
    [SerializeField] UnityEvent OnBannerLoaded;
    [SerializeField] UnityEvent OnBannerFailedToLoaded;
    [SerializeField] UnityEvent OnBannerOpen;
    [SerializeField] UnityEvent OnBannerFailToOpen;
    [SerializeField] UnityEvent OnBannerClosed;

    Yodo1U3dBannerAdView bannerAdView = null;

    [Header("Load Banner Ad manually or use buttons")]
    public bool LoadManually = false;
    [ConditionalHide("LoadManually", true)]
    public Button ShowBannerButton;
    [ConditionalHide("LoadManually", true)]
    public Button HideBannerButton;
    [ConditionalHide("LoadManually", true)]
    public Button DestroyBannerButton;

    private void Start()
    {
        if (!LoadManually)
        {
            Invoke("LoadBanner", 2f);
        }
        else
        {
            if (ShowBannerButton != null)
            {
                ShowBannerButton.onClick.AddListener(() => { LoadBanner(); });
            }
            if (HideBannerButton != null)
            {
                HideBannerButton.onClick.AddListener(() =>
                {
                    if (bannerAdView != null)
                    {
                        bannerAdView.Hide();
                    }
                });
            }
            if (DestroyBannerButton != null)
            {
                DestroyBannerButton.onClick.AddListener(() =>
                {
                    if (bannerAdView != null)
                    {
                        bannerAdView.Destroy();
                    }
                });
            }
        }

        if (bannerOnAllScenes)
        {
            if (transform.parent != null)
            {
                DontDestroyOnLoad(transform.parent);
            }
            else
            {
                DontDestroyOnLoad(gameObject);
            }
        }


    }

    private void OnEnable()
    {
#if UNITY_EDITOR
        SceneManager.sceneLoaded += OnSceneLoaded;
#endif
        if (bannerAdView != null)
        {
            bannerAdView.OnAdLoadedEvent += OnBannerAdLoadedEvent;
            bannerAdView.OnAdFailedToLoadEvent += OnBannerAdFailedToLoadEvent;
            bannerAdView.OnAdOpenedEvent += OnBannerAdOpenedEvent;
            bannerAdView.OnAdFailedToOpenEvent += OnBannerAdFailedToOpenEvent;
            bannerAdView.OnAdClosedEvent += OnBannerAdClosedEvent;

            bannerAdView.Show();
        }
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        SceneManager.sceneLoaded -= OnSceneLoaded;
#endif
        if (bannerAdView != null)
        {
            bannerAdView.OnAdLoadedEvent -= OnBannerAdLoadedEvent;
            bannerAdView.OnAdFailedToLoadEvent -= OnBannerAdFailedToLoadEvent;
            bannerAdView.OnAdOpenedEvent -= OnBannerAdOpenedEvent;
            bannerAdView.OnAdFailedToOpenEvent -= OnBannerAdFailedToOpenEvent;
            bannerAdView.OnAdClosedEvent -= OnBannerAdClosedEvent;

            bannerAdView.Hide();
        }
    }

    private void OnDestroy()
    {
        if (!bannerOnAllScenes)
        {
            if (bannerAdView != null)
            {
                bannerAdView.Destroy();
                bannerAdView = null;
            }
        }
    }

    private void LoadBanner()
    {
        Yodo1U3dBannerAdPosition[] vertical = new Yodo1U3dBannerAdPosition[] { Yodo1U3dBannerAdPosition.BannerTop, Yodo1U3dBannerAdPosition.BannerBottom, Yodo1U3dBannerAdPosition.BannerVerticalCenter };
        Yodo1U3dBannerAdPosition[] horizontal = new Yodo1U3dBannerAdPosition[] { Yodo1U3dBannerAdPosition.BannerHorizontalCenter, Yodo1U3dBannerAdPosition.BannerLeft, Yodo1U3dBannerAdPosition.BannerRight };
        Yodo1U3dBannerAdSize[] banners = new Yodo1U3dBannerAdSize[] { Yodo1U3dBannerAdSize.Banner, Yodo1U3dBannerAdSize.LargeBanner, Yodo1U3dBannerAdSize.SmartBanner, Yodo1U3dBannerAdSize.AdaptiveBanner, Yodo1U3dBannerAdSize.IABMediumRectangle };

        // Clean up banner before reusing
        if (bannerAdView != null)
        {
            bannerAdView.Destroy();
            bannerAdView = null;
        }

        bannerAdView = new Yodo1U3dBannerAdView(banners[((int)bannerSize)], horizontal[((int)HorizontalAlignment)] | vertical[((int)VerticalAlignment)]);
        if (!string.IsNullOrEmpty(PlacementID))
        {
            bannerAdView.SetAdPlacement(PlacementID);
        }
        bannerAdView.OnAdLoadedEvent += OnBannerAdLoadedEvent;
        bannerAdView.OnAdFailedToLoadEvent += OnBannerAdFailedToLoadEvent;
        bannerAdView.OnAdOpenedEvent += OnBannerAdOpenedEvent;
        bannerAdView.OnAdFailedToOpenEvent += OnBannerAdFailedToOpenEvent;
        bannerAdView.OnAdClosedEvent += OnBannerAdClosedEvent;


        bannerAdView.LoadAd();
    }
    public void ShowBannerAd()
    {
        if (bannerAdView != null)
        {
            bannerAdView.Show();
        }
    }
    public void HideBannerAd()
    {
        if (bannerAdView != null)
        {
            bannerAdView.Hide();
        }
    }
#if UNITY_EDITOR
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (bannerAdView != null)
        {
            if (bannerOnAllScenes)
            {
                int sceneNumber = SceneManager.GetActiveScene().buildIndex;
                if (sceneNumber != 0)
                {
                    if (!LoadManually)
                    {
                        LoadBanner();
                    }
                }
            }
        }
    }
#endif
    private void OnBannerAdLoadedEvent(Yodo1U3dBannerAdView adView)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode BannerV2 ad loaded");
        OnBannerLoaded.Invoke();
    }

    private void OnBannerAdFailedToLoadEvent(Yodo1U3dBannerAdView adView, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode BannerV2 ad failed to load with error code: " + adError.ToString());
        OnBannerFailedToLoaded.Invoke();
    }

    private void OnBannerAdOpenedEvent(Yodo1U3dBannerAdView adView)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode BannerV2 ad opened");
        OnBannerOpen.Invoke();
    }

    private void OnBannerAdFailedToOpenEvent(Yodo1U3dBannerAdView adView, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode BannerV2 ad failed to load with error code: " + adError.ToString());
        OnBannerFailToOpen.Invoke();
    }

    private void OnBannerAdClosedEvent(Yodo1U3dBannerAdView adView)
    {
        Debug.Log(Yodo1U3dMas.TAG + "NoCode BannerV2 ad closed");
        OnBannerClosed.Invoke();
    }
}