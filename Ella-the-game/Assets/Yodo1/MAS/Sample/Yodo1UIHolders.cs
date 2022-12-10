using UnityEngine;

public class Yodo1UIHolders : MonoBehaviour
{
    [Header("Holders")]
    public GameObject menuHolder;
    public GameObject bannerHolder;
    public GameObject interstitialHolder;
    public GameObject rewardHolder;
    public GameObject nativeAdsHolder;
    public GameObject rewardedInterstitialHolder;
    public GameObject appOpenHolder;

    public void OpenHolder(string holderName)
    {
        if (menuHolder != null)
        {
            menuHolder.SetActive(holderName.Equals("menu"));
        }
        if (bannerHolder != null)
        {
            bannerHolder.SetActive(holderName.Equals("banner"));
        }
        if (interstitialHolder != null)
        {
            interstitialHolder.SetActive(holderName.Equals("interstitial"));
        }
        if (rewardHolder != null)
        {
            rewardHolder.SetActive(holderName.Equals("reward"));
        }
        if (nativeAdsHolder != null)
        {
            nativeAdsHolder.SetActive(holderName.Equals("native"));
        }
        if (rewardedInterstitialHolder != null)
        {
            rewardedInterstitialHolder.SetActive(holderName.Equals("rewardedInterstitial"));
        }
        if (appOpenHolder != null)
        {
            appOpenHolder.SetActive(holderName.Equals("appOpen"));
        }
    }
}
