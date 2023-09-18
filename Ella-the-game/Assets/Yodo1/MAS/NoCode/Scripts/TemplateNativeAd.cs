using UnityEngine;
using Yodo1.MAS;
using UnityEngine.Events;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TemplateNativeAd : MonoBehaviour
{
    public enum Vertical
    {
        NativeTop, NativeBottom, NativeVerticalCenter
    };
    public enum Horizontal
    {
        NativeHorizontalCenter, NativeLeft, NativeRight
    };
    [Header("Load Native Ad with predefined positions")]
    public bool LoadPredefinedPositions = true;

    [Header("Native Ad Alignment")]
    [Tooltip("Please set the native ad alignment according to where native ad should appear.")]
    [ConditionalHide("LoadPredefinedPositions", true)]
    public Vertical VerticalAlignment = Vertical.NativeTop;
    [ConditionalHide("LoadPredefinedPositions", true)]
    public Horizontal HorizontalAlignment = Horizontal.NativeHorizontalCenter;
    [Header("Native Ad X position from the above specified position")]
    [Tooltip("Please Select X-coordinate offset in pixels.")]
    public int XPosition = 0;
    [Header("Native Ad Y position from the above specified position")]
    [Tooltip("Please Select Y-coordinate offset in pixels.")]
    public int YPosition = 0;
    [Header("Native Ad width")]
    [Tooltip("If native ad type is small, the ratio should be 3:1, if medium, it should be 6:5.")]
    public int width = 950;
    [Header("Native Ad height")]
    [Tooltip("If native ad type is small, the ratio should be 3:1, if medium, it should be 6:5.")]
    public int height = 600;

    [Header("Show Native ad on all scenes")]
    [Tooltip("Check this if you want to show this native ad on all scenes.")]
    public bool nativeAdOnAllScenes = true;

    [Header("Native ad Placement (Optional) ")]
    public string PlacementID = string.Empty;
    [Header("Native ad background color (Optional) ")]
    public Color backGroundColor = Color.black;
    [Space(10)]
    [Header("Native AD Events (optional) ")]
    [SerializeField] UnityEvent OnNativeAdLoaded;
    [SerializeField] UnityEvent OnNativeAdFailedToLoad;

    Yodo1U3dNativeAdView nativeAdView = null;


    [Header("Load Native Ad manually or use buttons in Unity editor")]
    public bool LoadManually = false;
    [ConditionalHide("LoadManually", true)]
    public Button ShowNativeAdButton;
    [ConditionalHide("LoadManually", true)]
    public Button HideNativeAdButton;
    [ConditionalHide("LoadManually", true)]
    public Button DestroyNativeAdButton;

    private void Start()
    {
        if (!LoadManually)
        {
            Invoke("LoadNativeAd", 2f);
        }
        else
        {
            if (ShowNativeAdButton != null)
            {
                ShowNativeAdButton.onClick.AddListener(() => { LoadNativeAd(); });
            }
            if (HideNativeAdButton != null)
            {
                HideNativeAdButton.onClick.AddListener(() =>
                {
                    if (nativeAdView != null)
                    {
                        nativeAdView.Hide();
                    }
                });
            }
            if (DestroyNativeAdButton != null)
            {
                DestroyNativeAdButton.onClick.AddListener(() =>
                {
                    if (nativeAdView != null)
                    {
                        nativeAdView.Destroy();
                    }
                });
            }
        }
#if UNITY_EDITOR
        if (nativeAdOnAllScenes)
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

#endif
    }
    private void OnEnable()
    {
#if UNITY_EDITOR
        SceneManager.sceneLoaded += OnSceneLoaded;
#endif
        if (nativeAdView != null)
        {
            nativeAdView.OnAdLoadedEvent += OnNativeAdLoadedEvent;
            nativeAdView.OnAdFailedToLoadEvent += OnNativeAdFailedToLoadEvent;

            nativeAdView.Show();
        }
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        SceneManager.sceneLoaded -= OnSceneLoaded;
#endif
        if (nativeAdView != null)
        {
            nativeAdView.OnAdLoadedEvent -= OnNativeAdLoadedEvent;
            nativeAdView.OnAdFailedToLoadEvent -= OnNativeAdFailedToLoadEvent;

            nativeAdView.Hide();
        }
    }

    private void OnDestroy()
    {
        if (!nativeAdOnAllScenes)
        {
            if (nativeAdView != null)
            {
                nativeAdView.Destroy();
                nativeAdView = null;
            }
        }
    }
    private void LoadNativeAd()
    {
        // Clean up native before reusing
        if (nativeAdView != null)
        {
            nativeAdView.Destroy();
            nativeAdView = null;
        }
        if (LoadPredefinedPositions)
        {
            Yodo1U3dNativeAdPosition[] vertical = new Yodo1U3dNativeAdPosition[] { Yodo1U3dNativeAdPosition.NativeTop, Yodo1U3dNativeAdPosition.NativeBottom, Yodo1U3dNativeAdPosition.NativeVerticalCenter };
            Yodo1U3dNativeAdPosition[] horizontal = new Yodo1U3dNativeAdPosition[] { Yodo1U3dNativeAdPosition.NativeHorizontalCenter, Yodo1U3dNativeAdPosition.NativeLeft, Yodo1U3dNativeAdPosition.NativeRight };

            nativeAdView = new Yodo1U3dNativeAdView(horizontal[((int)HorizontalAlignment)] | vertical[((int)VerticalAlignment)], XPosition, YPosition, width, height);

        }
        else
        {
            nativeAdView = new Yodo1U3dNativeAdView(XPosition, YPosition, width, height);

        }
        if (!string.IsNullOrEmpty(PlacementID))
        {
            nativeAdView.SetAdPlacement(PlacementID);
        }
        nativeAdView.SetBackgroundColor(backGroundColor);
        // Add Events
        nativeAdView.OnAdLoadedEvent += OnNativeAdLoadedEvent;
        nativeAdView.OnAdFailedToLoadEvent += OnNativeAdFailedToLoadEvent;

        nativeAdView.LoadAd();
    }
    public void ShowNativeAd()
    {
        if (nativeAdView != null)
        {
            nativeAdView.Show();
        }
    }
    public void HideNativeAd()
    {
        if (nativeAdView != null)
        {
            nativeAdView.Hide();
        }
    }
#if UNITY_EDITOR
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (nativeAdView != null)
        {
            if (nativeAdOnAllScenes)
            {
                int sceneNumber = SceneManager.GetActiveScene().buildIndex;
                if (sceneNumber != 0)
                {
                    if (!LoadManually)
                    {
                        LoadNativeAd();
                    }
                }
            }
        }
    }
#endif
    private void OnNativeAdLoadedEvent(Yodo1U3dNativeAdView adView)
    {
        // Banner ad is ready to be shown.
        Debug.Log(Yodo1U3dMas.TAG + "Native ad loaded");
        OnNativeAdLoaded.Invoke();
    }

    private void OnNativeAdFailedToLoadEvent(Yodo1U3dNativeAdView adView, Yodo1U3dAdError adError)
    {
        Debug.Log(Yodo1U3dMas.TAG + "Native ad failed to load with error code: " + adError.ToString());
        OnNativeAdFailedToLoad.Invoke();
    }
}
