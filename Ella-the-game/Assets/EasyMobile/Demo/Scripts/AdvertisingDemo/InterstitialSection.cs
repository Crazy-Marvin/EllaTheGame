using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyMobile.Demo
{
    [Serializable]
    public class InterstitialSection : LoadAndShowSection<InterstitialSection.DefaultInterstitialUI, InterstitialSection.CustomInterstitialUI>
    {
        [Serializable]
        public class DefaultInterstitialUI : DefaultElement
        {
            protected override string AdReadyMessage { get { return "IsInterstitialAdReady: TRUE"; } }

            protected override string AdNotReadyMessage { get { return "IsInterstitialAdReady: FALSE"; } }

            protected override string UnavailableAdAlertMessage { get { return "Interstitial ad is not loaded."; } }

            protected override bool IsAdReady()
            {
                return Advertising.IsInterstitialAdReady();
            }

            /// <summary>
            /// Load default interstitial ad.
            /// </summary>
            protected override void LoadAd()
            {
                if (Advertising.AutoAdLoadingMode == AutoAdLoadingMode.LoadAllDefinedPlacements || Advertising.AutoAdLoadingMode == AutoAdLoadingMode.LoadDefaultAds)
                {
                    NativeUI.Alert("Alert", "autoLoadDefaultAds is currently enabled. " +
                        "Ads will be loaded automatically in background without you having to do anything.");
                }

                Advertising.LoadInterstitialAd();
            }

            /// <summary>
            /// Show default interstitial ad.
            /// </summary>
            protected override void ShowAd()
            {
                Advertising.ShowInterstitialAd();
            }
        }

        [Serializable]
        public class CustomInterstitialUI : CustomElement
        {
            private List<InterstitialAdNetwork> allInterstitialNetworks;

            protected override string AdReadyMessage
            {
                get { return string.Format("IsInterstitialAdReady{0}: TRUE", string.IsNullOrEmpty(CustomKey) ? "" : "(" + CustomKey + ")"); }
            }

            protected override string AdNotReadyMessage
            {
                get { return string.Format("IsInterstitialAdReady{0}: FALSE", string.IsNullOrEmpty(CustomKey) ? "" : "(" + CustomKey + ")"); }
            }

            protected override string UnavailableAdAlertMessage
            {
                get { return string.Format("The interstitial ad at the {0} placement is not loaded.", string.IsNullOrEmpty(CustomKey) ? "default" : CustomKey); }
            }

            private InterstitialAdNetwork SelectedNetwork
            {
                get { return allInterstitialNetworks[networkSelector.value]; }
            }

            protected override void InitNetworkDropdown()
            {
                allInterstitialNetworks = new List<InterstitialAdNetwork>();
                List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();

                foreach (InterstitialAdNetwork network in Enum.GetValues(typeof(InterstitialAdNetwork)))
                {
                    allInterstitialNetworks.Add(network);
                    optionDatas.Add(new Dropdown.OptionData(network.ToString()));
                }

                networkSelector.ClearOptions();
                networkSelector.AddOptions(optionDatas);
            }

            protected override bool IsAdReady()
            {
                return Advertising.IsInterstitialAdReady(SelectedNetwork, AdPlacement.PlacementWithName(CustomKey));
            }

            protected override void LoadAd()
            {
                Advertising.LoadInterstitialAd(SelectedNetwork, AdPlacement.PlacementWithName(CustomKey));
            }

            protected override void ShowAd()
            {
                Advertising.ShowInterstitialAd(SelectedNetwork, AdPlacement.PlacementWithName(CustomKey));
            }
        }
    }
}
    