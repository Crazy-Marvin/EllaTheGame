using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AppOpenSampleAds : MonoBehaviour
{
    public void CloseAppOpenAds()
    {
#if UNITY_EDITOR
        Yodo1EditorAds.CloseAppOpenAdsInEditor();
#endif
    }
    public void Onclick()
    {
        Application.OpenURL("https://developers.yodo1.com/");
    }
}
