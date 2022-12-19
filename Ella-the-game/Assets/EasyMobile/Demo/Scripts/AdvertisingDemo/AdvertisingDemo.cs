using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyMobile.Demo
{
    public class AdvertisingDemo : MonoBehaviour
    {
        [SerializeField]
        private DemoUtils demoUtils = null;

        [SerializeField]
        private GameObject curtain = null, isAdRemovedInfo = null;

        [SerializeField]
        private Text defaultBannerNetworkText = null, defaultInterstitialNetworkText = null, defaultRewardedNetworkText = null;

        [SerializeField]
        private Dropdown autoLoadModeDropdown = null;

        [Space]
        [SerializeField]
        private BannerSection bannerUI = null;

        [SerializeField]
        private InterstitialSection interstitialUI = null;

        [SerializeField]
        private RewardedVideoSection rewardedVideoUI = null;

        private AutoAdLoadingMode lastAutoLoadMode = AutoAdLoadingMode.None;

        void Awake()
        {
            // Init EM runtime if needed (useful in case only this scene is built).
            if (!RuntimeManager.IsInitialized())
                RuntimeManager.Init();
        }

        private void Start()
        {
            curtain.SetActive(!EM_Settings.IsAdModuleEnable);

            bannerUI.Start();
            interstitialUI.Start(demoUtils);
            rewardedVideoUI.Start(demoUtils);
            SetupDefaultNetworkTexts();
            SetupAutoLoadModeDropdown();
        }

        private void Update()
        {
            interstitialUI.Update();
            rewardedVideoUI.Update();

            // Check if ads were removed.
            bool adRemoved = Advertising.IsAdRemoved();
            string adRemovedMessage = adRemoved ? "Ads were removed" : "Ads are enabled";
            demoUtils.DisplayBool(isAdRemovedInfo, !adRemoved, adRemovedMessage);

            // Check if the Advertising.AutoLoadAdsMode value was updated somewhere else.
            if (lastAutoLoadMode != Advertising.AutoAdLoadingMode)
            {
                autoLoadModeDropdown.value = (int)Advertising.AutoAdLoadingMode;
                lastAutoLoadMode = Advertising.AutoAdLoadingMode;
            }
        }

        /// <summary>
        /// Removes the ads.
        /// </summary>
        public void RemoveAds()
        {
            Advertising.RemoveAds();
            NativeUI.Alert("Alert", "Ads were removed. Future ads won't be shown except rewarded ads.");
        }

        /// <summary>
        /// Resets the remove ads.
        /// </summary>
        public void ResetRemoveAds()
        {
            Advertising.ResetRemoveAds();
            NativeUI.Alert("Alert", "Remove Ads status was reset. Ads will be shown normally.");
        }

        protected void SetupDefaultNetworkTexts()
        {
            AdSettings.DefaultAdNetworks defaultNetwork = new AdSettings.DefaultAdNetworks(BannerAdNetwork.None, InterstitialAdNetwork.None, RewardedAdNetwork.None);

#if UNITY_ANDROID
            defaultNetwork = EM_Settings.Advertising.AndroidDefaultAdNetworks;
#elif UNITY_IOS
            defaultNetwork = EM_Settings.Advertising.IosDefaultAdNetworks;
#endif

            defaultBannerNetworkText.text = "Default banner ad network: " + defaultNetwork.bannerAdNetwork.ToString();
            defaultInterstitialNetworkText.text = "Default interstitial ad network: " + defaultNetwork.interstitialAdNetwork.ToString();
            defaultRewardedNetworkText.text = "Default rewarded video network: " + defaultNetwork.rewardedAdNetwork.ToString();
        }

        protected void SetupAutoLoadModeDropdown()
        {
            List<Dropdown.OptionData> options = new List<Dropdown.OptionData>();
            foreach (AutoAdLoadingMode mode in Enum.GetValues(typeof(AutoAdLoadingMode)))
            {
                options.Add(new Dropdown.OptionData(mode.ToString()));
            }
            autoLoadModeDropdown.ClearOptions();
            autoLoadModeDropdown.AddOptions(options);

            autoLoadModeDropdown.value = (int)Advertising.AutoAdLoadingMode;

            autoLoadModeDropdown.onValueChanged.AddListener(mode =>
            {
                Advertising.AutoAdLoadingMode = (AutoAdLoadingMode)mode;
                lastAutoLoadMode = (AutoAdLoadingMode)mode;
            });
        }

        #region Events callback

        private void OnEnable()
        {
            Advertising.RewardedAdSkipped += OnRewardedAdSkipped;
            Advertising.RewardedAdCompleted += OnRewardedAdCompleted;
            Advertising.InterstitialAdCompleted += OnInterstitialAdCompleted;
        }

        private void OnDisable()
        {
            Advertising.RewardedAdSkipped -= OnRewardedAdSkipped;
            Advertising.RewardedAdCompleted -= OnRewardedAdCompleted;
            Advertising.InterstitialAdCompleted -= OnInterstitialAdCompleted;
        }

        private void OnInterstitialAdCompleted(InterstitialAdNetwork network, AdPlacement placement)
        {
            StartCoroutine(DelayCoroutine(GetPopupDelayTime((AdNetwork)network), () =>
                NativeUI.Alert("Interstitial Ad Completed", string.Format(
                    "Interstitial ad has been closed. Network: {0}, Placement: {1}",
                    network, AdPlacement.GetPrintableName(placement)))));

            Debug.Log(string.Format(
                "Interstitial ad has been closed. Network: {0}, Placement: {1}",
                network, AdPlacement.GetPrintableName(placement)));
        }

        private void OnRewardedAdCompleted(RewardedAdNetwork network, AdPlacement placement)
        {
            StartCoroutine(DelayCoroutine(GetPopupDelayTime((AdNetwork)network), () =>
                NativeUI.Alert("Rewarded Ad Completed", string.Format(
                    "The rewarded ad has completed, this is when you should reward the user. Network: {0}, Placement: {1}",
                    network, AdPlacement.GetPrintableName(placement)))));

            Debug.Log(string.Format(
                "The rewarded ad has completed, this is when you should reward the user. Network: {0}, Placement: {1}",
                network, AdPlacement.GetPrintableName(placement)));
        }

        private void OnRewardedAdSkipped(RewardedAdNetwork network, AdPlacement placement)
        {
            StartCoroutine(DelayCoroutine(GetPopupDelayTime((AdNetwork)network), () =>
                NativeUI.Alert("Rewarded Ad Skipped", string.Format(
                    "The rewarded ad was skipped, and the user shouldn't get the reward. Network: {0}, Placement: {1}",
                    network, AdPlacement.GetPrintableName(placement)))));

            Debug.Log(string.Format(
                "The rewarded ad was skipped, and the user shouldn't get the reward. Network: {0}, Placement: {1}",
                network, AdPlacement.GetPrintableName(placement)));
        }

        private IEnumerator DelayCoroutine(float time, Action action)
        {
            if (time < 0 || action == null)
                yield break;

            yield return new WaitForSeconds(time);
            action();
        }

        private float GetPopupDelayTime(AdNetwork network)
        {
            /// iOS, IronSource (v6.7.9.1).
            /// Work around for an issue that stop the alert popup from showing:
            /// "Warning: Attempt to present <UIAlertController: 0x136c3bc00>
            /// on <UnityDefaultViewController: 0x135d28f80> whose view is not in the window hierarchy!"
            /// * Actually we now just add a delay for all networks to avoid potential issues when new networks are added *
            return 0.5f;
        }

        #endregion
    }
}

