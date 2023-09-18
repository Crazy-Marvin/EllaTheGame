#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Yodo1.MAS;

public class Yodo1EditorAds : MonoBehaviour
{
    public static GameObject AdHolder;
    public static Canvas AdHolderCanvas;
    private static Dictionary<string, GameObject> BannerSampleAdEditor;
    private static Dictionary<string, GameObject> NativeSampleAdEditor;
    private static GameObject InterstitialSampleAdEditor;
    private static GameObject RewardedVideoSampleAdEditor;
    private static GameObject RewardedInterstitialSampleAdEditor;
    private static GameObject AppOpenAdsSampleAdEditor;

    public static bool DisableGUI = false;
    public static bool GrantReward = false;
    public static int TotalBannerCount = 0;

    private static GameObject BannerSampleAdEditorTemp;
    private static GameObject NativeSampleAdEditorTemp;


    public static void InitializeAds()
    {
        BannerSampleAdEditor = new Dictionary<string, GameObject>();
        NativeSampleAdEditor = new Dictionary<string, GameObject>();
        EventSystem sceneEventSystem = FindObjectOfType<EventSystem>();
        if (AdHolder == null)
        {
            AdHolder = Instantiate(Resources.Load("SampleAds/AdHolder") as GameObject);
            AdHolder.name = "Yodo1AdCanvas";
            AdHolderCanvas = AdHolder.transform.GetChild(0).GetComponent<Canvas>();
            AdHolderCanvas.sortingOrder = HighestOrderCanvas();
        }
        AdHolder.SetActive(true);
        if (sceneEventSystem == null)
        {
            var eventSystem = new GameObject("EventSystem", typeof(EventSystem), typeof(StandaloneInputModule));
        }

        if (InterstitialSampleAdEditor == null)
        {
            if (AdHolderCanvas != null)
            {
                InterstitialSampleAdEditor = Instantiate(Resources.Load("SampleAds/InterstitialSampleAdPanel") as GameObject, AdHolderCanvas.transform);
                InterstitialSampleAdEditor.transform.SetAsLastSibling();
            }
        }
        if (RewardedVideoSampleAdEditor == null)
        {
            if (AdHolderCanvas != null)
            {
                RewardedVideoSampleAdEditor = Instantiate(Resources.Load("SampleAds/RewardedVideoSampleAdPanel") as GameObject, AdHolderCanvas.transform);
                RewardedVideoSampleAdEditor.transform.SetAsLastSibling();
            }
        }
        if (RewardedInterstitialSampleAdEditor == null)
        {
            if (AdHolderCanvas != null)
            {
                RewardedInterstitialSampleAdEditor = Instantiate(Resources.Load("SampleAds/RewardedInterstitialSampleAdPanel") as GameObject, AdHolderCanvas.transform);
                RewardedInterstitialSampleAdEditor.transform.SetAsLastSibling();
            }
        }
        if (AppOpenAdsSampleAdEditor == null)
        {
            if (AdHolderCanvas != null)
            {
                AppOpenAdsSampleAdEditor = Instantiate(Resources.Load("SampleAds/AppOpenSampleAd") as GameObject, AdHolderCanvas.transform);
                AppOpenAdsSampleAdEditor.transform.SetAsLastSibling();
            }
        }
    }

    public static int HighestOrderCanvas()
    {
        Canvas[] canvases = GameObject.FindObjectsOfType<Canvas>();
        int length = canvases.Length;
        int highestOrder = canvases[0].sortingOrder;
        for (int i = 1; i < length; i++)
        {
            if (highestOrder < canvases[i].sortingOrder)
            {
                highestOrder = canvases[i].sortingOrder;
            }
        }
        return highestOrder + 1;
    }
    public static void ShowStamdardBannerAdsInEditor(string IndexId)
    {
        if (AdHolder == null)
        {
            AdHolder = Instantiate(Resources.Load("SampleAds/AdHolder") as GameObject);
            AdHolder.name = "Yodo1AdCanvas";
            AdHolderCanvas = AdHolder.transform.GetChild(0).GetComponent<Canvas>();
            AdHolderCanvas.sortingOrder = HighestOrderCanvas();
        }
        if (BannerSampleAdEditor == null)
        {
            BannerSampleAdEditor = new Dictionary<string, GameObject>();
            AdHolder.SetActive(false);
        }
        GameObject BannerAd;
        if (!BannerSampleAdEditor.TryGetValue(IndexId, out BannerAd))
        {
            if (AdHolderCanvas != null)
            {
                BannerSampleAdEditorTemp = Instantiate(Resources.Load("SampleAds/StandardBannerSampleAdPanel") as GameObject, AdHolderCanvas.transform);

                BannerSampleAdEditorTemp.GetComponent<RectTransform>().anchorMin = new Vector2(0.5f, 1f);
                BannerSampleAdEditorTemp.GetComponent<RectTransform>().anchorMax = new Vector2(0.5f, 1f);
                BannerSampleAdEditorTemp.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 1f);
                BannerSampleAdEditorTemp.GetComponent<RectTransform>().anchoredPosition = new Vector2(0f, 0f);
                BannerSampleAdEditorTemp.SetActive(true);

                BannerSampleAdEditor.Add(IndexId, BannerSampleAdEditorTemp);
            }
        }
        else
        {
            BannerAd.SetActive(true);
        }
        if (AdHolder.activeSelf)
        {
            Debug.Log(Yodo1U3dMas.TAG + "Editor Banner ad opened");
        }


    }
    private static float anchorMinX, anchorMinY, anchorMaxX, anchorMaxY, pivotX, pivotY, anchoredPositionX, anchoredPositionY;
    public static int tempAlign = 0;


    private static void CalculateAnchoringForStandardBanner(int align, out float anchorMinX, out float anchorMinY, out float anchorMaxX, out float anchorMaxY, out float pivotX, out float pivotY, out float anchoredPositionX, out float anchoredPositionY)
    {
        if (align == 0)
        {
            align = tempAlign;
        }
        tempAlign = align;
        anchorMinX = 0.5f;
        anchorMinY = 1f;
        anchorMaxX = 0.5f;
        anchorMaxY = 1f;
        pivotX = 0.5f;
        pivotY = 1f;
        anchoredPositionX = 0f;
        anchoredPositionY = 0f;
        if ((align & (int)Yodo1U3dBannerAdPosition.BannerHorizontalCenter) == (int)Yodo1U3dBannerAdPosition.BannerHorizontalCenter)
        {
            anchorMinX = 0.5f;
            anchorMaxX = 0.5f;
            pivotX = 0.5f;
            anchoredPositionX = 0f;

        }
        else if ((align & (int)Yodo1U3dBannerAdPosition.BannerRight) == (int)Yodo1U3dBannerAdPosition.BannerRight)
        {
            anchorMinX = 1f;
            anchorMaxX = 1f;
            pivotX = 0.5f;
            anchoredPositionX = -320f;
        }
        else if ((align & (int)Yodo1U3dBannerAdPosition.BannerLeft) == (int)Yodo1U3dBannerAdPosition.BannerLeft)
        {
            anchorMinX = 0f;
            anchorMaxX = 0f;
            pivotX = 0.5f;
            anchoredPositionX = 320f;
        }

        if ((align & (int)Yodo1U3dBannerAdPosition.BannerVerticalCenter) == (int)Yodo1U3dBannerAdPosition.BannerVerticalCenter)
        {
            anchorMinY = 0.5f;
            anchorMaxY = 0.5f;
            pivotY = 0f;
            anchoredPositionY = -60f;
        }
        else if ((align & (int)Yodo1U3dBannerAdPosition.BannerBottom) == (int)Yodo1U3dBannerAdPosition.BannerBottom)
        {
            anchorMinY = 0f;
            anchorMaxY = 0f;
            pivotY = 1f;
            anchoredPositionY = 120f;
        }
        else if ((align & (int)Yodo1U3dBannerAdPosition.BannerTop) == (int)Yodo1U3dBannerAdPosition.BannerTop)
        {
            anchorMinY = 1f;
            anchorMaxY = 1f;
            pivotY = 1f;
            anchoredPositionY = 0f;
        }
    }
    private static void CalculateAnchoringForLargeBanner(int align, out float anchorMinX, out float anchorMinY, out float anchorMaxX, out float anchorMaxY, out float pivotX, out float pivotY, out float anchoredPositionX, out float anchoredPositionY)
    {
        if (align == 0)
        {
            align = tempAlign;
        }
        tempAlign = align;
        anchorMinX = 0.5f;
        anchorMinY = 1f;
        anchorMaxX = 0.5f;
        anchorMaxY = 1f;
        pivotX = 0.5f;
        pivotY = 0.5f;
        anchoredPositionX = 0f;
        anchoredPositionY = -100f;
        if ((align & (int)Yodo1U3dBannerAdPosition.BannerHorizontalCenter) == (int)Yodo1U3dBannerAdPosition.BannerHorizontalCenter)
        {
            anchorMinX = 0.5f;
            anchorMaxX = 0.5f;
            pivotX = 0.5f;
            anchoredPositionX = 0f;

        }
        else if ((align & (int)Yodo1U3dBannerAdPosition.BannerRight) == (int)Yodo1U3dBannerAdPosition.BannerRight)
        {
            anchorMinX = 1f;
            anchorMaxX = 1f;
            pivotX = 0.5f;
            anchoredPositionX = -320f;
        }
        else if ((align & (int)Yodo1U3dBannerAdPosition.BannerLeft) == (int)Yodo1U3dBannerAdPosition.BannerLeft)
        {
            anchorMinX = 0f;
            anchorMaxX = 0f;
            pivotX = 0.5f;
            anchoredPositionX = 320f;
        }

        if ((align & (int)Yodo1U3dBannerAdPosition.BannerVerticalCenter) == (int)Yodo1U3dBannerAdPosition.BannerVerticalCenter)
        {
            anchorMinY = 0.5f;
            anchorMaxY = 0.5f;
            pivotY = 0.5f;
            anchoredPositionY = 0f;
        }
        else if ((align & (int)Yodo1U3dBannerAdPosition.BannerBottom) == (int)Yodo1U3dBannerAdPosition.BannerBottom)
        {
            anchorMinY = 0f;
            anchorMaxY = 0f;
            pivotY = 0.5f;
            anchoredPositionY = 100f;
        }
        else if ((align & (int)Yodo1U3dBannerAdPosition.BannerTop) == (int)Yodo1U3dBannerAdPosition.BannerTop)
        {
            anchorMinY = 1f;
            anchorMaxY = 1f;
            pivotY = 0.5f;
            anchoredPositionY = -100f;
        }
    }
    private static void CalculateAnchoringForIABBanner(int align, out float anchorMinX, out float anchorMinY, out float anchorMaxX, out float anchorMaxY, out float pivotX, out float pivotY, out float anchoredPositionX, out float anchoredPositionY)
    {
        if (align == 0)
        {
            align = tempAlign;
        }
        tempAlign = align;
        anchorMinX = 0.5f;
        anchorMinY = 1f;
        anchorMaxX = 0.5f;
        anchorMaxY = 1f;
        pivotX = 0.5f;
        pivotY = 0.5f;
        anchoredPositionX = 0f;
        anchoredPositionY = -250f;
        if ((align & (int)Yodo1U3dBannerAdPosition.BannerHorizontalCenter) == (int)Yodo1U3dBannerAdPosition.BannerHorizontalCenter)
        {
            anchorMinX = 0.5f;
            anchorMaxX = 0.5f;
            pivotX = 0.5f;
            anchoredPositionX = 0f;

        }
        else if ((align & (int)Yodo1U3dBannerAdPosition.BannerRight) == (int)Yodo1U3dBannerAdPosition.BannerRight)
        {
            anchorMinX = 1f;
            anchorMaxX = 1f;
            pivotX = 0.5f;
            anchoredPositionX = -320f;
        }
        else if ((align & (int)Yodo1U3dBannerAdPosition.BannerLeft) == (int)Yodo1U3dBannerAdPosition.BannerLeft)
        {
            anchorMinX = 0f;
            anchorMaxX = 0f;
            pivotX = 0.5f;
            anchoredPositionX = 320f;
        }

        if ((align & (int)Yodo1U3dBannerAdPosition.BannerVerticalCenter) == (int)Yodo1U3dBannerAdPosition.BannerVerticalCenter)
        {
            anchorMinY = 0.5f;
            anchorMaxY = 0.5f;
            pivotY = 0.5f;
            anchoredPositionY = 0f;
        }
        else if ((align & (int)Yodo1U3dBannerAdPosition.BannerBottom) == (int)Yodo1U3dBannerAdPosition.BannerBottom)
        {
            anchorMinY = 0f;
            anchorMaxY = 0f;
            pivotY = 0.5f;
            anchoredPositionY = 250f;
        }
        else if ((align & (int)Yodo1U3dBannerAdPosition.BannerTop) == (int)Yodo1U3dBannerAdPosition.BannerTop)
        {
            anchorMinY = 1f;
            anchorMaxY = 1f;
            pivotY = 0.5f;
            anchoredPositionY = -250f;
        }
    }
    private static void CalculateAnchoringForAdaptiveBanner(int align, out float anchorMinX, out float anchorMinY, out float anchorMaxX, out float anchorMaxY, out float pivotX, out float pivotY, out float anchoredPositionX, out float anchoredPositionY)
    {
        if (align == 0)
        {
            align = tempAlign;
        }
        tempAlign = align;
        anchorMinX = 0f;
        anchorMinY = 1f;
        anchorMaxX = 1f;
        anchorMaxY = 1f;
        pivotX = 0.5f;
        pivotY = 0.5f;
        anchoredPositionX = 0f;
        anchoredPositionY = -60f;

        if ((align & (int)Yodo1U3dBannerAdPosition.BannerVerticalCenter) == (int)Yodo1U3dBannerAdPosition.BannerVerticalCenter)
        {
            anchorMinY = 0.5f;
            anchorMaxY = 0.5f;
            pivotY = 0.5f;
            anchoredPositionY = 0f;
        }
        else if ((align & (int)Yodo1U3dBannerAdPosition.BannerBottom) == (int)Yodo1U3dBannerAdPosition.BannerBottom)
        {
            anchorMinY = 0f;
            anchorMaxY = 0f;
            pivotY = 0.5f;
            anchoredPositionY = 60f;
        }
        else if ((align & (int)Yodo1U3dBannerAdPosition.BannerTop) == (int)Yodo1U3dBannerAdPosition.BannerTop)
        {
            anchorMinY = 1f;
            anchorMaxY = 1f;
            pivotY = 0.5f;
            anchoredPositionY = -60f;
        }
    }
    private static void CalculateAnchoringForSmartBanner(int align, out float anchorMinX, out float anchorMinY, out float anchorMaxX, out float anchorMaxY, out float pivotX, out float pivotY, out float anchoredPositionX, out float anchoredPositionY)
    {
        if (align == 0)
        {
            align = tempAlign;
        }
        tempAlign = align;
        anchorMinX = 0f;
        anchorMinY = 1f;
        anchorMaxX = 1f;
        anchorMaxY = 1f;
        pivotX = 0.5f;
        pivotY = 0.5f;
        anchoredPositionX = 0f;
        anchoredPositionY = -40f;

        if ((align & (int)Yodo1U3dBannerAdPosition.BannerVerticalCenter) == (int)Yodo1U3dBannerAdPosition.BannerVerticalCenter)
        {
            anchorMinY = 0.5f;
            anchorMaxY = 0.5f;
            pivotY = 0.5f;
            anchoredPositionY = 0f;
        }
        else if ((align & (int)Yodo1U3dBannerAdPosition.BannerBottom) == (int)Yodo1U3dBannerAdPosition.BannerBottom)
        {
            anchorMinY = 0f;
            anchorMaxY = 0f;
            pivotY = 0.5f;
            anchoredPositionY = 40f;
        }
        else if ((align & (int)Yodo1U3dBannerAdPosition.BannerTop) == (int)Yodo1U3dBannerAdPosition.BannerTop)
        {
            anchorMinY = 1f;
            anchorMaxY = 1f;
            pivotY = 0.5f;
            anchoredPositionY = -40f;
        }
    }
    public static void ShowBannerAdsInEditor(string IndexId, int align, int size, int offsetX, int offsetY)
    {
        if (AdHolder == null)
        {
            AdHolder = Instantiate(Resources.Load("SampleAds/AdHolder") as GameObject);
            AdHolder.name = "Yodo1AdCanvas";
            AdHolderCanvas = AdHolder.transform.GetChild(0).GetComponent<Canvas>();
            AdHolderCanvas.sortingOrder = HighestOrderCanvas();

        }
        if (BannerSampleAdEditor == null)
        {
            BannerSampleAdEditor = new Dictionary<string, GameObject>();
            AdHolder.SetActive(false);
        }
        GameObject BannerAd;
        if (!BannerSampleAdEditor.TryGetValue(IndexId, out BannerAd))
        {
            if (size == 0)
            {
                if (AdHolderCanvas != null)
                {
                    BannerSampleAdEditorTemp = Instantiate(Resources.Load("SampleAds/StandardBannerSampleAdPanel") as GameObject, AdHolderCanvas.transform);
                    CalculateAnchoringForStandardBanner(align, out anchorMinX, out anchorMinY, out anchorMaxX, out anchorMaxY, out pivotX, out pivotY, out anchoredPositionX, out anchoredPositionY);
                }
            }
            else if (size == 1)
            {
                if (AdHolderCanvas != null)
                {
                    BannerSampleAdEditorTemp = Instantiate(Resources.Load("SampleAds/LargeBanner") as GameObject, AdHolderCanvas.transform);
                    CalculateAnchoringForLargeBanner(align, out anchorMinX, out anchorMinY, out anchorMaxX, out anchorMaxY, out pivotX, out pivotY, out anchoredPositionX, out anchoredPositionY);
                }
            }
            else if (size == 2)
            {
                if (AdHolderCanvas != null)
                {
                    BannerSampleAdEditorTemp = Instantiate(Resources.Load("SampleAds/IABMediumRectangleBanner") as GameObject, AdHolderCanvas.transform);
                    CalculateAnchoringForIABBanner(align, out anchorMinX, out anchorMinY, out anchorMaxX, out anchorMaxY, out pivotX, out pivotY, out anchoredPositionX, out anchoredPositionY);
                }
            }
            else if (size == 3)
            {

                string[] res = UnityStats.screenRes.Split('x');
                if (int.Parse(res[1]) > int.Parse(res[0]))
                {
                    if (AdHolderCanvas != null)
                    {
                        BannerSampleAdEditorTemp = Instantiate(Resources.Load("SampleAds/SmartBannerPortrait") as GameObject, AdHolderCanvas.transform);
                        CalculateAnchoringForAdaptiveBanner(align, out anchorMinX, out anchorMinY, out anchorMaxX, out anchorMaxY, out pivotX, out pivotY, out anchoredPositionX, out anchoredPositionY);
                    }
                }
                else
                {
                    if (AdHolderCanvas != null)
                    {
                        BannerSampleAdEditorTemp = Instantiate(Resources.Load("SampleAds/SmartBannerLandscape") as GameObject, AdHolderCanvas.transform);
                        CalculateAnchoringForSmartBanner(align, out anchorMinX, out anchorMinY, out anchorMaxX, out anchorMaxY, out pivotX, out pivotY, out anchoredPositionX, out anchoredPositionY);
                    }
                }
            }
            else if (size == 4)
            {
                if (AdHolderCanvas != null)
                {
                    BannerSampleAdEditorTemp = Instantiate(Resources.Load("SampleAds/AdaptiveBanner") as GameObject, AdHolderCanvas.transform);
                    CalculateAnchoringForAdaptiveBanner(align, out anchorMinX, out anchorMinY, out anchorMaxX, out anchorMaxY, out pivotX, out pivotY, out anchoredPositionX, out anchoredPositionY);
                }
            }
            BannerSampleAdEditorTemp.transform.SetSiblingIndex(BannerSampleAdEditorTemp.transform.parent.childCount - 3);
            BannerSampleAdEditorTemp.GetComponent<RectTransform>().anchorMin = new Vector2(anchorMinX, anchorMinY);
            BannerSampleAdEditorTemp.GetComponent<RectTransform>().anchorMax = new Vector2(anchorMaxX, anchorMaxY);
            BannerSampleAdEditorTemp.GetComponent<RectTransform>().pivot = new Vector2(pivotX, pivotY);
            BannerSampleAdEditorTemp.GetComponent<RectTransform>().anchoredPosition = new Vector2(anchoredPositionX, anchoredPositionY);
            BannerSampleAdEditorTemp.SetActive(true);
            BannerSampleAdEditor.Add(IndexId, BannerSampleAdEditorTemp);
        }
        else
        {
            BannerAd.SetActive(true);
        }
        if (AdHolder.activeSelf)
        {
            Debug.Log(Yodo1U3dMas.TAG + "Editor Banner ad opened");
        }
    }
    private static void CalculateAnchoringForNativeAds(int align, float width, float height, out float anchorMinX, out float anchorMinY, out float anchorMaxX, out float anchorMaxY, out float pivotX, out float pivotY, out float anchoredPositionX, out float anchoredPositionY)
    {
        if (align == 0)
        {
            align = tempAlign;
        }
        tempAlign = align;
        anchorMinX = 0.5f;
        anchorMinY = 1f;
        anchorMaxX = 0.5f;
        anchorMaxY = 1f;
        pivotX = 0.5f;
        pivotY = 1f;
        anchoredPositionX = 0f;
        anchoredPositionY = 0f;
        if ((align & (int)Yodo1U3dNativeAdPosition.NativeHorizontalCenter) == (int)Yodo1U3dNativeAdPosition.NativeHorizontalCenter)
        {
            anchorMinX = 0.5f;
            anchorMaxX = 0.5f;
            pivotX = 0.5f;
            anchoredPositionX = 0f;

        }
        else if ((align & (int)Yodo1U3dNativeAdPosition.NativeRight) == (int)Yodo1U3dNativeAdPosition.NativeRight)
        {
            anchorMinX = 1f;
            anchorMaxX = 1f;
            pivotX = 0.5f;
            anchoredPositionX = -width / 2;
        }
        else if ((align & (int)Yodo1U3dNativeAdPosition.NativeLeft) == (int)Yodo1U3dNativeAdPosition.NativeLeft)
        {
            anchorMinX = 0f;
            anchorMaxX = 0f;
            pivotX = 0.5f;
            anchoredPositionX = width / 2;
        }

        if ((align & (int)Yodo1U3dNativeAdPosition.NativeVerticalCenter) == (int)Yodo1U3dNativeAdPosition.NativeVerticalCenter)
        {
            anchorMinY = 0.5f;
            anchorMaxY = 0.5f;
            pivotY = 0f;
            anchoredPositionY = -height / 2;
        }
        else if ((align & (int)Yodo1U3dNativeAdPosition.NativeBottom) == (int)Yodo1U3dNativeAdPosition.NativeBottom)
        {
            anchorMinY = 0f;
            anchorMaxY = 0f;
            pivotY = 1f;
            anchoredPositionY = height;
        }
        else if ((align & (int)Yodo1U3dNativeAdPosition.NativeTop) == (int)Yodo1U3dNativeAdPosition.NativeTop)
        {
            anchorMinY = 1f;
            anchorMaxY = 1f;
            pivotY = 1f;
            anchoredPositionY = 0f;
        }
    }
    public static void ShowNativeAdsInEditor(string IndexId, int width, int height, int offsetX, int offsetY, Color colorVal)
    {
        if (AdHolder == null)
        {
            AdHolder = Instantiate(Resources.Load("SampleAds/AdHolder") as GameObject);
            AdHolder.name = "Yodo1AdCanvas";
            AdHolderCanvas = AdHolder.transform.GetChild(0).GetComponent<Canvas>();
            AdHolderCanvas.sortingOrder = HighestOrderCanvas();

        }
        if (NativeSampleAdEditor == null)
        {
            NativeSampleAdEditor = new Dictionary<string, GameObject>();
            AdHolder.SetActive(false);
        }
        GameObject NativeAd;
        if (!NativeSampleAdEditor.TryGetValue(IndexId, out NativeAd))
        {
            string[] res = UnityStats.screenRes.Split('x');
            if (width > int.Parse(res[0]))
            {
                width = int.Parse(res[0]);
            }
            if (height > int.Parse(res[1]))
            {
                height = int.Parse(res[1]);
            }
            int xVal = (width / 2) + offsetX;
            int yVal = -(height / 2) - offsetY;
            if ((width / height) > 3.5)
            {
                if (AdHolderCanvas != null)
                {
                    NativeSampleAdEditorTemp = Instantiate(Resources.Load("SampleAds/TemplateNativeAdSmall") as GameObject, AdHolderCanvas.transform);
                    if (height > 60 && width > 360)
                    {
                        NativeSampleAdEditorTemp.GetComponent<RectTransform>().localScale = new Vector2((float)width / 360, (float)height / 60);

                    }
                    else
                    {
                        NativeSampleAdEditorTemp.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);

                    }
                }
            }
            else
            {
                if (AdHolderCanvas != null)
                {
                    NativeSampleAdEditorTemp = Instantiate(Resources.Load("SampleAds/TemplateNativeAdMedium") as GameObject, AdHolderCanvas.transform);
                    if (height > 300 && width > 360)
                    {
                        NativeSampleAdEditorTemp.GetComponent<RectTransform>().localScale = new Vector2((float)width / 360, (float)height / 300);

                    }
                    else
                    {
                        NativeSampleAdEditorTemp.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
                    }
                    if (height < 200)
                    {
                        NativeSampleAdEditorTemp.transform.GetChild(0).gameObject.SetActive(false);
                        NativeSampleAdEditorTemp.transform.GetChild(2).gameObject.SetActive(false);
                        NativeSampleAdEditorTemp.transform.GetChild(4).gameObject.SetActive(false);
                    }
                }
            }

            //NativeSampleAdEditorTemp.transform.SetAsFirstSibling();
            NativeSampleAdEditorTemp.transform.SetSiblingIndex(NativeSampleAdEditorTemp.transform.parent.childCount - 3);

            NativeSampleAdEditorTemp.GetComponent<RectTransform>().anchorMin = new Vector2(0, 1);
            NativeSampleAdEditorTemp.GetComponent<RectTransform>().anchorMax = new Vector2(0, 1);
            NativeSampleAdEditorTemp.GetComponent<RectTransform>().pivot = new Vector2(0.5f, 0.5f);

            NativeSampleAdEditorTemp.GetComponent<Image>().color = colorVal;


            if (!((width / height) > 3.5))
            {
                if (xVal < 85)
                {
                    xVal = 85;
                }
                if (yVal > -70)
                {
                    yVal = -70;
                }
            }
            NativeSampleAdEditorTemp.GetComponent<RectTransform>().anchoredPosition = new Vector2(xVal, yVal);
            NativeSampleAdEditorTemp.SetActive(true);
            NativeSampleAdEditor.Add(IndexId, NativeSampleAdEditorTemp);
        }
        else
        {
            NativeAd.SetActive(true);
        }
        if (AdHolder.activeSelf)
        {
            Debug.Log(Yodo1U3dMas.TAG + "Editor Native ad opened");
        }
    }
    public static void ShowNativeAdsInEditor(string IndexId, int align, int width, int height, int offsetX, int offsetY, Color colorVal)
    {
        if (AdHolder == null)
        {
            AdHolder = Instantiate(Resources.Load("SampleAds/AdHolder") as GameObject);
            AdHolder.name = "Yodo1AdCanvas";
            AdHolderCanvas = AdHolder.transform.GetChild(0).GetComponent<Canvas>();
            AdHolderCanvas.sortingOrder = HighestOrderCanvas();

        }
        if (NativeSampleAdEditor == null)
        {
            NativeSampleAdEditor = new Dictionary<string, GameObject>();
            AdHolder.SetActive(false);
        }
        GameObject NativeAd;
        if (!NativeSampleAdEditor.TryGetValue(IndexId, out NativeAd))
        {
            if ((width / height) > 3.5)
            {
                if (AdHolderCanvas != null)
                {
                    NativeSampleAdEditorTemp = Instantiate(Resources.Load("SampleAds/TemplateNativeAdSmall") as GameObject, AdHolderCanvas.transform);
                    if (height > 60 && width > 360)
                    {
                        NativeSampleAdEditorTemp.GetComponent<RectTransform>().localScale = new Vector2((float)width / 360, (float)height / 60);
                        CalculateAnchoringForNativeAds(align, width, height, out anchorMinX, out anchorMinY, out anchorMaxX, out anchorMaxY, out pivotX, out pivotY, out anchoredPositionX, out anchoredPositionY);

                    }
                    else
                    {
                        NativeSampleAdEditorTemp.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
                        CalculateAnchoringForNativeAds(align, width, height, out anchorMinX, out anchorMinY, out anchorMaxX, out anchorMaxY, out pivotX, out pivotY, out anchoredPositionX, out anchoredPositionY);

                    }
                }
            }
            else
            {
                if (AdHolderCanvas != null)
                {
                    NativeSampleAdEditorTemp = Instantiate(Resources.Load("SampleAds/TemplateNativeAdMedium") as GameObject, AdHolderCanvas.transform);

                    if (height > 300 && width > 360)
                    {
                        NativeSampleAdEditorTemp.GetComponent<RectTransform>().localScale = new Vector2((float)width / 360, (float)height / 300);
                        CalculateAnchoringForNativeAds(align, width, height, out anchorMinX, out anchorMinY, out anchorMaxX, out anchorMaxY, out pivotX, out pivotY, out anchoredPositionX, out anchoredPositionY);

                    }
                    else
                    {
                        NativeSampleAdEditorTemp.GetComponent<RectTransform>().sizeDelta = new Vector2(width, height);
                        CalculateAnchoringForNativeAds(align, width, height, out anchorMinX, out anchorMinY, out anchorMaxX, out anchorMaxY, out pivotX, out pivotY, out anchoredPositionX, out anchoredPositionY);

                    }
                    if (height < 200)
                    {
                        NativeSampleAdEditorTemp.transform.GetChild(0).gameObject.SetActive(false);
                        NativeSampleAdEditorTemp.transform.GetChild(2).gameObject.SetActive(false);
                        NativeSampleAdEditorTemp.transform.GetChild(4).gameObject.SetActive(false);
                    }
                }
            }

            NativeSampleAdEditorTemp.transform.SetSiblingIndex(NativeSampleAdEditorTemp.transform.parent.childCount - 3);
            NativeSampleAdEditorTemp.GetComponent<RectTransform>().anchorMin = new Vector2(anchorMinX, anchorMinY);
            NativeSampleAdEditorTemp.GetComponent<RectTransform>().anchorMax = new Vector2(anchorMaxX, anchorMaxY);
            NativeSampleAdEditorTemp.GetComponent<RectTransform>().pivot = new Vector2(pivotX, pivotY);
            NativeSampleAdEditorTemp.GetComponent<Image>().color = colorVal;
            NativeSampleAdEditorTemp.GetComponent<RectTransform>().anchoredPosition = new Vector2(anchoredPositionX + offsetX, anchoredPositionY - offsetY);
            NativeSampleAdEditorTemp.SetActive(true);
            NativeSampleAdEditor.Add(IndexId, NativeSampleAdEditorTemp);
        }
        else
        {
            NativeAd.SetActive(true);
        }
        if (AdHolder.activeSelf)
        {
            Debug.Log(Yodo1U3dMas.TAG + "Editor Native ad opened");
        }
    }

    public static void LoadInterstitialAdsInEditor()
    {
        if (InterstitialSampleAdEditor != null)
        {
            Yodo1U3dMasCallback.ForwardEvent("onInterstitialAdLoadedEvent");
        }
    }

    public static void ShowInterstitialAdsInEditor()
    {
        if (InterstitialSampleAdEditor != null)
        {
            DisableGUI = true;
            InterstitialSampleAdEditor.SetActive(true);
            Yodo1U3dMasCallback.ForwardEvent("onInterstitialAdOpenedEvent");
        }
    }

    public static void LoadRewardedVideodsInEditor()
    {
        if (RewardedVideoSampleAdEditor != null)
        {
            Yodo1U3dMasCallback.ForwardEvent("onRewardedAdLoadedEvent");
        }
    }

    public static void ShowRewardedVideodsInEditor()
    {
        if (RewardedVideoSampleAdEditor != null)
        {
            DisableGUI = true;
            RewardedVideoSampleAdEditor.SetActive(true);
            Yodo1U3dMasCallback.ForwardEvent("onRewardedAdOpenedEvent");
        }
    }
    public static void LoadRewardedInterstitialInEditor()
    {
        if (RewardedInterstitialSampleAdEditor != null)
        {
            Yodo1U3dMasCallback.ForwardEvent("onRewardedInterstitialAdLoadedEvent");
        }
    }

    public static void ShowRewardedInterstitialInEditor()
    {
        if (RewardedInterstitialSampleAdEditor != null)
        {
            DisableGUI = true;
            RewardedInterstitialSampleAdEditor.SetActive(true);
            Yodo1U3dMasCallback.ForwardEvent("onRewardedInterstitialAdOpenedEvent");
        }
    }

    public static void LoadAppOpenInEditor()
    {
        if (AppOpenAdsSampleAdEditor != null)
        {
            Yodo1U3dMasCallback.ForwardEvent("onAppOpenAdLoadedEvent");
        }
    }

    public static void ShowAppOpenInEditor()
    {
        if (AppOpenAdsSampleAdEditor != null)
        {
            DisableGUI = true;
            AppOpenAdsSampleAdEditor.SetActive(true);
            Yodo1U3dMasCallback.ForwardEvent("onAppOpenAdOpenedEvent");
        }
    }
    public static void HideBannerAdsInEditor(string IndexId)
    {
        if (BannerSampleAdEditor != null)
        {
            GameObject BannerAd;
            if (BannerSampleAdEditor.TryGetValue(IndexId, out BannerAd))
            {

                Debug.Log(Yodo1U3dMas.TAG + "Editor Banner ad closed");
                if (BannerSampleAdEditor[IndexId] != null)
                {
                    BannerSampleAdEditor[IndexId].SetActive(false);
                }

            }

        }
    }
    public static void DestroyBannerAdsInEditor(string IndexId)
    {
        if (BannerSampleAdEditor != null)
        {
            GameObject BannerAd;
            if (BannerSampleAdEditor.TryGetValue(IndexId, out BannerAd))
            {
                Debug.Log(Yodo1U3dMas.TAG + "Editor Banner ad destroyed");
                Destroy(BannerSampleAdEditor[IndexId]);
                BannerSampleAdEditor.Remove(IndexId);

            }


        }
    }
    public static void CloseInterstitialAdsInEditor()
    {
        if (InterstitialSampleAdEditor != null)
        {
            DisableGUI = false;
            InterstitialSampleAdEditor.SetActive(false);
            Yodo1U3dMasCallback.ForwardEvent("onInterstitialAdClosedEvent");
        }
    }
    public static void CloseRewardedVideodsInEditor()
    {
        if (RewardedVideoSampleAdEditor != null)
        {
            DisableGUI = false;
            RewardedVideoSampleAdEditor.SetActive(false);
            if (GrantReward)
            {
                Yodo1U3dMasCallback.ForwardEvent("onRewardedAdReceivedRewardEvent");
                GrantReward = false;
            }
            Yodo1U3dMasCallback.ForwardEvent("onRewardedAdClosedEvent");
        }
    }

    public static void CloseRewardedInterstitialInEditor()
    {
        if (RewardedInterstitialSampleAdEditor != null)
        {
            DisableGUI = false;
            RewardedInterstitialSampleAdEditor.SetActive(false);
            if (GrantReward)
            {
                Yodo1U3dMasCallback.ForwardEvent("onRewardedInterstitialAdEarnedEvent");
                GrantReward = false;
            }
            Yodo1U3dMasCallback.ForwardEvent("onRewardedInterstitialAdClosedEvent");
        }
    }
    public static void CloseAppOpenAdsInEditor()
    {
        if (AppOpenAdsSampleAdEditor != null)
        {
            DisableGUI = false;
            AppOpenAdsSampleAdEditor.SetActive(false);
            
            Yodo1U3dMasCallback.ForwardEvent("onAppOpenAdClosedEvent");
        }
    }
    public static void GetRewardsInEditor()
    {
        GrantReward = true;
    }

    public static void HideNativeAdsInEditor(string IndexId)
    {
        if (NativeSampleAdEditor != null)
        {
            GameObject NativeAd;
            if (NativeSampleAdEditor.TryGetValue(IndexId, out NativeAd))
            {

                Debug.Log(Yodo1U3dMas.TAG + "Editor Native ad hidden");
                if (NativeSampleAdEditor[IndexId] != null)
                {
                    NativeSampleAdEditor[IndexId].SetActive(false);
                }

            }

        }
    }
    public static void DestroyNativeAdsInEditor(string IndexId)
    {
        if (NativeSampleAdEditor != null)
        {
            GameObject NativeAd;
            if (NativeSampleAdEditor.TryGetValue(IndexId, out NativeAd))
            {
                Debug.Log(Yodo1U3dMas.TAG + "Editor Native ad destroyed");
                Destroy(NativeSampleAdEditor[IndexId]);
                NativeSampleAdEditor.Remove(IndexId);

            }


        }
    }
}
#endif