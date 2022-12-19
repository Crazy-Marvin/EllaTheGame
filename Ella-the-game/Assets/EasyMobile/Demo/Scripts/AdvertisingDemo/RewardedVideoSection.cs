using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace EasyMobile.Demo
{
    [Serializable]
    public class RewardedVideoSection : LoadAndShowSection<RewardedVideoSection.DefaultRewardedVideolUI, RewardedVideoSection.CustomRewardedVideoUI>
    {
        [Serializable]
        public class DefaultRewardedVideolUI : DefaultElement
        {
            protected override string AdReadyMessage { get { return "IsRewardedAdReady: TRUE"; } }

            protected override string AdNotReadyMessage { get { return "IsRewardedAdReady: FALSE"; } }

            protected override string UnavailableAdAlertMessage { get { return "Rewarded ad is not loaded."; } }

            protected override bool IsAdReady()
            {
                return Advertising.IsRewardedAdReady();
            }

            /// <summary>
            /// Load default rewarded video.
            /// </summary>
            protected override void LoadAd()
            {
                if (Advertising.AutoAdLoadingMode == AutoAdLoadingMode.LoadAllDefinedPlacements || Advertising.AutoAdLoadingMode == AutoAdLoadingMode.LoadDefaultAds)
                {
                    NativeUI.Alert("Alert", "autoLoadDefaultAds is currently enabled. " +
                        "Ads will be loaded automatically in background without you having to do anything.");
                }

                Advertising.LoadRewardedAd();
            }

            /// <summary>
            /// Show default rewarded video.
            /// </summary>
            protected override void ShowAd()
            {
                Advertising.ShowRewardedAd();
            }
        }

        [Serializable]
        public class CustomRewardedVideoUI : CustomElement
        {
            private List<RewardedAdNetwork> allRewardedNetworks;

            protected override string AdReadyMessage
            {
                get { return string.Format("IsRewardedAdReady{0}: TRUE", string.IsNullOrEmpty(CustomKey) ? "" : "(" + CustomKey + ")"); }
            }

            protected override string AdNotReadyMessage
            {
                get { return string.Format("IsRewardedAdReady{0}: FALSE", string.IsNullOrEmpty(CustomKey) ? "" : "(" + CustomKey + ")"); }
            }

            protected override string UnavailableAdAlertMessage
            {
                get { return string.Format("The rewarded ad at the {0} placement is not loaded.", string.IsNullOrEmpty(CustomKey) ? "default" : CustomKey); }
            }

            private RewardedAdNetwork SelectedNetwork
            {
                get { return allRewardedNetworks[networkSelector.value]; }
            }

            protected override void InitNetworkDropdown()
            {
                allRewardedNetworks = new List<RewardedAdNetwork>();
                List<Dropdown.OptionData> optionDatas = new List<Dropdown.OptionData>();

                foreach (RewardedAdNetwork network in Enum.GetValues(typeof(RewardedAdNetwork)))
                {
                    allRewardedNetworks.Add(network);
                    optionDatas.Add(new Dropdown.OptionData(network.ToString()));
                }

                networkSelector.ClearOptions();
                networkSelector.AddOptions(optionDatas);
            }

            protected override bool IsAdReady()
            {
                return Advertising.IsRewardedAdReady(SelectedNetwork, AdPlacement.PlacementWithName(CustomKey));
            }

            protected override void LoadAd()
            {
                Advertising.LoadRewardedAd(SelectedNetwork, AdPlacement.PlacementWithName(CustomKey));
            }

            protected override void ShowAd()
            {
                Advertising.ShowRewardedAd(SelectedNetwork, AdPlacement.PlacementWithName(CustomKey));
            }
        }
    }
}

