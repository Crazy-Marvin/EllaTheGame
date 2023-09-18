#if UNITY_EDITOR
using UnityEditor;
using UnityEngine;

public class InterstitialSampleAds : MonoBehaviour
{
    public RectTransform LogoRectTransform;
    private void OnEnable()
    {
        string[] res = UnityStats.screenRes.Split('x');
        if (int.Parse(res[1]) > int.Parse(res[0]))
        {
            LogoRectTransform.anchorMin = new Vector2(0f, 0.5f);
            LogoRectTransform.anchorMax = new Vector2(1, 0.5f);
            LogoRectTransform.pivot = new Vector2(0.5f, 0.5f);
            LogoRectTransform.localScale = new Vector3(1, ((float)Screen.height / (float)Screen.width), 1);
            LogoRectTransform.offsetMin = Vector2.zero;
            LogoRectTransform.offsetMax = new Vector2(0, 417);
            LogoRectTransform.localPosition = Vector3.zero;
        }
        else
        {
            float TempVal = float.Parse(res[1]) / float.Parse(res[0]);
            LogoRectTransform.localScale = new Vector3(TempVal, TempVal, 1);
        }
    }
    public void CloseInterstitialAds()
    {
        Yodo1EditorAds.CloseInterstitialAdsInEditor();
    }
}
#endif